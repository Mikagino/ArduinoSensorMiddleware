#include "MessageParsing.h"

MessageParsing::MessageParsing(ArsemiArduinoCore &newArsemiArduinoCore)
    : arsemiArduinoCore(newArsemiArduinoCore) {}

/// @brief Parse a new serial package with its action code to the
/// according actions and invoke associated functions, with all parameters
/// of the package.
void MessageParsing::parseMessage() {
  if (!SerialMessaging::isPackageAvailable()) {
    return;
  }

  if (_queuedActionCode == -1) {
    _queuedActionCode = parseNextActionCode();
  }

  if (_queuedActionCode == -1) {
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
    //_queuedActionCode = -1; // discard package because it's not correct?
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
  if (!SerialMessaging::isPackageAvailable())
    return -1;

  if (Serial.peek() == SerialProtocol::StartByte) {
    Serial.read(); // discard StartByte
    return Serial.read();
  }

  // Discard all until StartByte
  while (Serial.available() < 3 && Serial.read() != SerialProtocol::StartByte) {
  }
  return Serial.read();
}

/// @brief Parse action "Add Sensor".
/// Package parameters: [ sensorType | intervalMs |
/// constructorParameters[] (optional, different for each sensor type) ]
/// @return false when still waiting for parameters or the parameters are not
/// enough, otherwise true (it will also return true to discard invalid
/// packages)
bool MessageParsing::parseAddSensorAction() {
  if (!SerialMessaging::isPackageAvailable())
    return false;

  if (queuedPackage.Parameters[0] == 0)
    queuedPackage.Parameters[0] = Serial.read(); // read sensor type

  if (queuedSensor == nullptr)
    queuedSensor = SensorFactory::createNewSensor(
        AbstractSensor::SensorTypes(queuedPackage.Parameters[0]));

  if (queuedSensor == nullptr)
    SerialMessaging::write(SerialProtocol::Action::System::Error,
                           SerialProtocol::Error::Package::InvalidSensorType);

  if (queuedSensor->ParameterByteCount < Serial.available())
    return false;

  for (int i = 1; i <= queuedSensor->ParameterByteCount; i++) {
    if (Serial.peek() == SerialProtocol::StartByte) {
      SerialMessaging::write(
          SerialProtocol::Action::System::Error,
          SerialProtocol::Error::Package::InvalidSensorParameters);
      return true;
    }
    queuedPackage.Parameters[i] = Serial.read();
    continue;
  }

  if (!checkNextCrc8Checksum(queuedPackage))
    return true;

  queuedSensor->parseParameters(queuedPackage.Parameters,
                                queuedSensor->ParameterByteCount);

  if (arsemiArduinoCore.addSensor(queuedSensor)) {
    SerialMessaging::write(
        SerialProtocol::Action::Setup::SuccessfullyAddedSensor);
    queuedSensor->begin();
  } else
    SerialMessaging::write(SerialProtocol::Action::System::Error,
                           SerialProtocol::Error::Package::SensorCountOverflow);

  queuedPackage.ActionCode = 0;
  return true;
}

/// @brief Check if the checksum is correct, otherwise send an error message
/// over serial
/// @param crc8Checksum the checksum of the package
/// @param package package to be checked
/// @return true if calculated checksum is similar to crc8Checksum, otherwise
/// false
bool MessageParsing::checkNextCrc8Checksum(SerialPackage &package) {
  uint8_t currentCrc8Checksum = Serial.read();
  uint8_t calculatedCrc8Checksum = SerialMessaging::CRC8(package);
  if (currentCrc8Checksum != calculatedCrc8Checksum) {
    // uint8_t package[4] = {SerialProtocol::Action::System::Error,
    //                       SerialProtocol::Error::Package::InvalidChecksum,
    //                       currentCrc8Checksum, calculatedCrc8Checksum};
    // SerialMessaging::write(package, 4);
    SerialMessaging::write(SerialProtocol::Action::System::Error,
                           SerialProtocol::Error::Package::InvalidChecksum);
    return false;
  }
  return true;
}

bool MessageParsing::checkNextCrc8Checksum(uint8_t actionCode) {
  uint8_t currentCrc8Checksum = Serial.read();
  uint8_t calculatedCrc8Checksum = SerialMessaging::CRC8(actionCode);
  if (currentCrc8Checksum != calculatedCrc8Checksum) {
    // uint8_t package[4] = {SerialProtocol::Action::System::Error,
    //                       SerialProtocol::Error::Package::InvalidChecksum,
    //                       currentCrc8Checksum, calculatedCrc8Checksum};
    // SerialMessaging::write(package, 4);
    SerialMessaging::write(SerialProtocol::Action::System::Error,
                           SerialProtocol::Error::Package::InvalidChecksum);
    return false;
  }
  return true;
}