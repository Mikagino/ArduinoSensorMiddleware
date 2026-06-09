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
    // SerialMessaging::write(SerialProtocol::Action::Setup::SuccessfullyAddedSensor);
    uint8_t pack[2] = {SerialProtocol::Action::System::Debug,
                       _queuedActionCode};
    SerialMessaging::write(pack, 2);
  }

  if (_queuedActionCode == -1 || _queuedActionCode == 0) {
    return;
  }

  bool done = false;

  switch (_queuedActionCode) {
  case SerialProtocol::Action::System::RequestHandshake:
    if (checkNextCrc8Checksum(_queuedActionCode)) {
      SerialMessaging::write(SerialProtocol::Action::System::ReplyHandshake);
    }
    done = true;
    break;

  case SerialProtocol::Action::System::HibernateMicrocontroller:
    if (checkNextCrc8Checksum(_queuedActionCode)) {
      arsemiArduinoCore.execution = false;
    }
    done = true;
    break;

  case SerialProtocol::Action::System::WakeMicrocontroller:
    if (checkNextCrc8Checksum(_queuedActionCode)) {
      arsemiArduinoCore.execution = true;
    }
    done = true;
    break;

  case SerialProtocol::Action::Setup::ClearConfiguration:
    if (checkNextCrc8Checksum(_queuedActionCode)) {
      arsemiArduinoCore.destroyAllSensors();
      SerialMessaging::write(
          SerialProtocol::Action::Setup::SuccessfullyClearedConfiguration);
    }
    done = true;
    break;

  case SerialProtocol::Action::Setup::AddSensor:
    SerialMessaging::write(
        SerialProtocol::Action::Setup::SuccessfullyAddedSensor);
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

/// @brief Peeks for StartByte and discards everything until the action code.
/// Packages are only done if they contain [StartByte + ActionCode + CRC8], thus
/// this method waits for 3 bytes in the stream. Further checks and processing
/// must be done by another method.
/// @returns Action code which follows after the next start byte, otherwise -1
int MessageParsing::parseNextActionCode() {
  while (SerialMessaging::isPackageAvailable()) {
    if (Serial.peek() == SerialProtocol::StartByte) {
      Serial.read(); // discard StartByte
      return Serial.read();
    }

    Serial.read(); // discard
  }
  return -1;
}

/// @brief Parse action "Add Sensor".
/// Package parameters: [ sensorType | intervalMs |
/// constructorParameters[] (optional, different for each sensor type) ]
/// @return false when still waiting for parameters or the parameters are not
/// enough, otherwise true (it will also return true to discard invalid
/// packages)
bool MessageParsing::parseAddSensorAction() {
  if (Serial.available() == 0)
    return false;

  if (queuedPackage.getParameter(0) == 0)
    queuedPackage.appendParameters(Serial.read(), 1); // read sensor type

  if (queuedSensor == nullptr)
    queuedSensor = SensorFactory::createNewSensor(
        AbstractSensor::SensorTypes(queuedPackage.getParameter(0)));

  if (queuedSensor == nullptr)
    SerialMessaging::write(SerialProtocol::Action::System::Error,
                           SerialProtocol::Error::Package::InvalidSensorType);

  if (queuedSensor->getParameterByteCount() > Serial.available()) {
    SerialMessaging::write(
        SerialProtocol::Action::System::Error,
        SerialProtocol::Error::Package::InvalidSensorParameters);
    return false;
  }

  for (int i = queuedPackage.getParameterCount();
       i <= queuedSensor->getParameterByteCount(); i++) {
    if (Serial.peek() == SerialProtocol::StartByte) {
      SerialMessaging::write(
          SerialProtocol::Action::System::Error,
          SerialProtocol::Error::Package::InvalidSensorParameters);
      return true;
    }
    queuedPackage.appendParameters(Serial.read(), 1);
  }

  if (!checkNextCrc8Checksum(queuedPackage))
    return true;

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
bool MessageParsing::checkNextCrc8Checksum(SerialPackage &package) {
  uint8_t nextCrc8Checksum = Serial.read();
  uint8_t calculatedCrc8Checksum = SerialMessaging::CRC8(package);

  uint8_t *serializedPackage = package.Serialize();

  if (nextCrc8Checksum != calculatedCrc8Checksum) {
    uint8_t package[4] = {SerialProtocol::Action::System::Error,
                          SerialProtocol::Error::Package::InvalidChecksum,
                          nextCrc8Checksum, calculatedCrc8Checksum};
    SerialMessaging::write(package, 4);
    // SerialMessaging::write(SerialProtocol::Action::System::Error,
    //                        SerialProtocol::Error::Package::InvalidChecksum);
    return false;
  }
  return true;
}

bool MessageParsing::checkNextCrc8Checksum(uint8_t actionCode) {
  uint8_t nextCrc8Checksum = Serial.read();
  uint8_t calculatedCrc8Checksum = SerialMessaging::CRC8(actionCode);
  if (nextCrc8Checksum != calculatedCrc8Checksum) {
    uint8_t package[4] = {SerialProtocol::Action::System::Error,
                          SerialProtocol::Error::Package::InvalidChecksum,
                          nextCrc8Checksum, calculatedCrc8Checksum};
    SerialMessaging::write(package, 4);
    // SerialMessaging::write(SerialProtocol::Action::System::Error,
    //                        SerialProtocol::Error::Package::InvalidChecksum);
    return false;
  }
  return true;
}