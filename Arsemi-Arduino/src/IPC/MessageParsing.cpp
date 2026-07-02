#include "MessageParsing.h"

MessageParsing::MessageParsing(ArsemiArduinoCore &newArsemiArduinoCore)
    : arsemiArduinoCore(newArsemiArduinoCore) {}

/// @brief Parse a new serial package with its action code to the
/// according actions and invoke associated functions, with all parameters
/// of the package.
void MessageParsing::parseMessage() {
  if (!SerialMessaging::isPackageAvailable())
    return;

  if (_queuedActionCode == -1 || _queuedActionCode == 0) {
    _queuedActionCode = parseNextActionCode();
    // uint8_t pack[2] = {SerialProtocol::Action::System::Debug,
    //                    _queuedActionCode};
    // SerialMessaging::write(pack, 2);
    if (_queuedActionCode == -1 || _queuedActionCode == 0) {
      return;
    }
  }

  if (!queuedPackage.Done)
    parseParameters();

  bool done = false;

  switch (_queuedActionCode) {
  case SerialProtocol::Action::System::RequestHandshake:
    if (checkCrc8Checksum(queuedPackage)) {
      SerialMessaging::write(SerialProtocol::Action::System::ReplyHandshake);
    }
    done = true;
    break;

  case SerialProtocol::Action::System::HibernateMicrocontroller:
    if (checkCrc8Checksum(queuedPackage)) {
      arsemiArduinoCore.execution = false;
    }
    done = true;
    break;

  case SerialProtocol::Action::System::WakeMicrocontroller:
    if (checkCrc8Checksum(queuedPackage)) {
      arsemiArduinoCore.execution = true;
    }
    done = true;
    break;

  case SerialProtocol::Action::Setup::ClearConfiguration:
    if (checkCrc8Checksum(queuedPackage)) {
      SerialMessaging::write(
          SerialProtocol::Action::Setup::SuccessfullyClearedConfiguration);
      arsemiArduinoCore.destroyAllSensors();
    }
    done = true;
    break;

  case SerialProtocol::Action::Setup::AddSensor:
    done = parseAddSensorAction();
    break;

  default:
    uint8_t data[3] = {SerialProtocol::Action::System::Error,
                       SerialProtocol::Error::Package::InvalidActionCode,
                       (uint8_t)_queuedActionCode};
    SerialMessaging::write(data, 3);
    done = true;
  }

  if (done)
    _queuedActionCode = -1;
}

/// @brief Peeks for PackageDelimiter and discards everything until the action
/// code. Packages are only done if they contain [PackageDelimiter + ActionCode
/// + CRC8], thus this method waits for 3 bytes in the stream. Further checks
/// and processing must be done by another method.
/// @returns Action code which follows after the next start byte, otherwise -1
int MessageParsing::parseNextActionCode() {
  while (SerialMessaging::isPackageAvailable()) {
    if (Serial.peek() == SerialProtocol::PackageDelimiter) {
      Serial.read(); // discard PackageDelimiter
      return Serial.read();
    }

    Serial.read(); // discard
  }
  return -1;
}

/// @brief Parse the package parameters from the message into queuedPackage
void MessageParsing::parseParameters() {
  for (int i = queuedPackage.getParameterCount();; i++) {
    int nextByte = Serial.peek();
    if (nextByte == SerialProtocol::PackageDelimiter) {
      queuedPackage.Crc8 = queuedPackage.getLastParameter();
      queuedPackage.removeLastParameters();
      queuedPackage.Done = true;
      Serial.read(); // discard PackageDelimiter at end of package
      return;
    } else if (nextByte != -1) {
      queuedPackage.appendParameters(Serial.read(), 1);
    }
  }
}

/// @brief Parse action "Add Sensor".
/// Package parameters: [ sensorType | intervalMs |
/// constructorParameters[] (optional, different for each sensor type) ]
/// @return false when still waiting for parameters or the parameters are not
/// enough, otherwise true (it will also return true to discard invalid
/// packages)
bool MessageParsing::parseAddSensorAction() {
  if (queuedPackage.getParameterCount() < 2)
    return true; // TODO: Error message (currently only discards the corrupt
                 // package)

  // SerialMessaging::write(SerialProtocol::Action::System::Debug, 69);

  if (queuedSensor == nullptr) {
    queuedSensor = SensorFactory::createNewSensor(
        AbstractSensor::SensorTypes(queuedPackage.getParameter(0)));
    SerialMessaging::write(SerialProtocol::Action::System::Debug,
                           queuedPackage.getParameter(0));

    if (queuedSensor == nullptr)
      SerialMessaging::write(SerialProtocol::Action::System::Error,
                             SerialProtocol::Error::Package::InvalidSensorType);
  }

  if (queuedSensor->getParameterByteCount() !=
      queuedPackage.getParameterCount()) {
    SerialMessaging::write(
        SerialProtocol::Action::System::Error,
        SerialProtocol::Error::Package::InvalidSensorParameters);
    return false;
  }

  if (!checkCrc8Checksum(queuedPackage))
    return true; // TODO: Error message

  queuedSensor->parseParameters(queuedPackage);

  if (arsemiArduinoCore.addSensor(queuedSensor)) {
    SerialMessaging::write(
        SerialProtocol::Action::Setup::SuccessfullyAddedSensor);
    queuedSensor->begin();
  } else
    SerialMessaging::write(SerialProtocol::Action::System::Error,
                           SerialProtocol::Error::Package::SensorCountOverflow);

  queuedPackage.reset();
  queuedSensor = nullptr;
  return true;
}

/// @brief Check if the checksum is correct, otherwise send an error message
/// over serial
/// @param crc8Checksum the checksum of the package
/// @param package package to be checked
/// @return true if calculated checksum is similar to crc8Checksum, otherwise
/// false
bool MessageParsing::checkCrc8Checksum(SerialPackage &package) {
  uint8_t nextCrc8Checksum = queuedPackage.Crc8;
  uint8_t calculatedCrc8Checksum = SerialMessaging::CRC8(queuedPackage);

  // uint8_t *serializedPackage = package.Serialize();

  if (nextCrc8Checksum != calculatedCrc8Checksum) {
    uint8_t errorPackage[4] = {SerialProtocol::Action::System::Error,
                               SerialProtocol::Error::Package::InvalidChecksum,
                               nextCrc8Checksum, calculatedCrc8Checksum};
    SerialMessaging::write(errorPackage, 4);
    // SerialMessaging::write(SerialProtocol::Action::System::Error,
    //                        SerialProtocol::Error::Package::InvalidChecksum);
    return false;
  }
  return true;
}