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

  switch (_queuedActionCode) {
  case SerialProtocol::Action::System::RequestHandshake:
    SerialMessaging::write(SerialProtocol::Action::System::ReplyHandshake);
    _queuedActionCode = -1;
    break;

  case SerialProtocol::Action::System::HibernateMicrocontroller:
    // Serial.println("Hibernate");
    // TODO
    _queuedActionCode = -1;
    break;

  case SerialProtocol::Action::System::WakeMicrocontroller:
    // Serial.println("Wake");
    // TODO
    _queuedActionCode = -1;
    break;

  case SerialProtocol::Action::Setup::ClearConfiguration:
    // Serial.println("Clear");
    arsemiArduinoCore.destroyAllSensors();
    _queuedActionCode = -1;
    break;

  case SerialProtocol::Action::Setup::AddSensor:
    // Serial.println("AddSensor");
    parseAddSensorAction();
    _queuedActionCode = -1;
    break;

  default:
    // Serial.println("ERROR!");
    SerialPackage errorPackage(
        SerialProtocol::Action::System::Error,
        new uint8_t[SerialProtocol::Error::Package::InvalidActionCode],
        (uint8_t)2);
    _queuedActionCode = -1; // discard "broken" package
  }
}

/// @brief Peeks for StartByte and discards everything until the action code
/// @returns Action code which follows after the next start byte, otherwise -1
int MessageParsing::parseNextActionCode() {
  if (!SerialMessaging::isPackageAvailable())
    return -1;

  if (Serial.peek() == SerialProtocol::StartByte) {
    Serial.read(); // discard StartByte
    return Serial.read();
  }

  // Discard all until StartByte
  while (Serial.available() < 2 && Serial.read() != SerialProtocol::StartByte) {
  }
  return Serial.read();
}

/// @brief Parse action "Add Sensor".
/// Package parameters: [ sensorType | intervalMs | constructorParameters[] ]
void MessageParsing::parseAddSensorAction() {
  SerialMessaging::write(
      SerialProtocol::Action::Setup::SuccessfullyAddedSensor);

  uint8_t availableBytes = Serial.available();

  /// Check if enough parameters for each sensor type are in the serial buffer
  if (queuedPackage[0] == AbstractSensor::SensorTypes::TYPE_GENERIC_ANALOG &&
      !hasRequiredParameters(availableBytes,
                             (AnalogSensor::parameterByteCount + 1)))
    return;
  else if (queuedPackage[0] ==
               AbstractSensor::SensorTypes::TYPE_GENERIC_DIGITAL &&
           !hasRequiredParameters(availableBytes,
                                  (DigitalSensor::parameterByteCount + 1)))
    return;
  else if (queuedPackage[0] == AbstractSensor::SensorTypes::TYPE_GENERIC_I2C &&
           !hasRequiredParameters(availableBytes,
                                  (I2CSensor::parameterByteCount + 1)))
    return;
  else if (queuedPackage[0] == AbstractSensor::SensorTypes::TYPE_MAX30102 &&
           !hasRequiredParameters(availableBytes,
                                  (MAX30102Sensor::parameterByteCount + 1)))
    return;
  else if (queuedPackage[0] == 0)
    queuedPackage[0] = -1;

  /// Allocations for the sensor creation
  uint8_t parameters[availableBytes] = {};
  // int parameterCount = Serial.readBytesUntil(SerialProtocol::StartByte,
  //                                            parameters, availableBytes);
  AbstractSensor *newSensor;

  // TODO: check the CRC8

  /// Create new sensor depending on id in queuedPackage[0]
  switch (queuedPackage[0]) {
  case AbstractSensor::SensorTypes::TYPE_GENERIC_ANALOG: {
    newSensor = new AnalogSensor(parameters[1]);
    break;
  }

  case AbstractSensor::TYPE_GENERIC_DIGITAL: {
    newSensor = new DigitalSensor(parameters[1]);
    break;
  }

  case AbstractSensor::SensorTypes::TYPE_GENERIC_I2C: {
    newSensor = new I2CSensor(parameters[1], parameters[2], parameters[3]);
    break;
  }

  case AbstractSensor::SensorTypes::TYPE_MAX30102: {
    newSensor = new MAX30102Sensor();
    break;
  }

  default:
    SerialMessaging::write(
        SerialProtocol::Action::System::Error,
        SerialProtocol::Error::Package::InvalidSensorParameters);
    return;
  }

  newSensor->intervalMillis = parameters[0];

  if (arsemiArduinoCore.addSensor(newSensor)) {
    SerialMessaging::write(
        SerialProtocol::Action::Setup::SuccessfullyAddedSensor);
    newSensor->begin();
  } else
    SerialMessaging::write(SerialProtocol::Action::System::Error,
                           SerialProtocol::Error::Package::SensorCountOverflow);

  queuedPackage[0] = -1;
}

/// @brief Checks if the following parameters (next bytes) are enough, if not it
/// sends an error message to serial
/// @param parameterCount Count of the parameters in a serial message
/// @param requiredParameterCount Count of parameters the message should have
/// @returns true if successful, otherwise false and writes
/// InvalidSensorParameters message over UART
bool MessageParsing::hasRequiredParameters(uint8_t parameterCount,
                                           uint8_t requiredParameterCount) {
  if (parameterCount < requiredParameterCount) {
    // SerialPackage errorPackage(
    //     SerialProtocol::SystemAction::Error,
    //     SerialProtocol::PackageError::InvalidSensorParameters);
    // SerialMessaging::write(errorPackage);
    return false;
  }
  return true;
}
