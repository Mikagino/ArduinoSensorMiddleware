#include "ArsemiArduinoCore.h"

ArsemiArduinoCore::ArsemiArduinoCore(uint8_t maxSensorCount) {
  sensors = new AbstractSensor *[maxSensorCount];
  this->maxSensorCount = maxSensorCount;
}

/// @brief adds a new sensor to an array for batch calls of functions
/// @param newSensor
/// @return false if the sensor count reaches an overflow, otherwise true
bool ArsemiArduinoCore::addSensor(AbstractSensor *newSensor) {
  if (_currentSensorCount >= maxSensorCount) {
    return false;
  }
  sensors[_currentSensorCount] = newSensor;
  _currentSensorCount++;
  return true;
}

/// calls begin() on all the sensors added with addSensor()
void ArsemiArduinoCore::beginAllSensors() {
  for (int i = 0; i < _currentSensorCount; i++) {
    sensors[i]->begin();
  }
}

/// @brief Calls Deconstructor on all the sensors added with addSensor()
void ArsemiArduinoCore::destroyAllSensors() {
  for (int i = 0; i < _currentSensorCount; i++) {
    sensors[i]->~AbstractSensor();
  }
}

/// @brief Calls update() on all the sensors added with addSensor() and sends
/// the new sensor data over serial if it has been updated and the stream has
/// enough bytes available
bool ArsemiArduinoCore::updateAllSensors() {
  bool updated = false;
  bool currentlyUpdated;
  uint8_t package[3] = {SerialProtocol::Action::Sensor::NewSample, 1, 1};

  for (int i = 0; i < _currentSensorCount; i++) {
    if (Serial.availableForWrite() < 4) {
      return false;
    }
    currentlyUpdated = sensors[i]->update();
    if (currentlyUpdated) {
      package[1] = i;
      package[2] = sensors[i]->getLastValue();
      SerialMessaging::write(package, 3);
    }
    updated = (updated || currentlyUpdated);
  }
  return updated;
}

/// @brief gets _lastValue from the sensor with the id
/// @param sensorId
/// @return
uint8_t ArsemiArduinoCore::getSensorValueById(uint8_t sensorId) {
  return getSensorById(sensorId)->getLastValue();
}

/// @brief iterates over all sensors and returns the sensors or nullptr when the
/// id is not found
/// @param sensorId
/// @return The sensor with the according id, otherwise nullptr
AbstractSensor *ArsemiArduinoCore::getSensorById(uint8_t sensorId) {
  for (int i = 0; i < _currentSensorCount; i++) {
    if (sensors[i]->getSensorId() == sensorId)
      return sensors[i];
  }
  return nullptr;
}

/// @brief Parse a new serial package with its action code to the according
/// actions and invoke associated functions, with all parameters of the
/// package.
void ArsemiArduinoCore::parseMessage() {
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
    destroyAllSensors();
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
int ArsemiArduinoCore::parseNextActionCode() {
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
void ArsemiArduinoCore::parseAddSensorAction() {
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

  if (addSensor(newSensor)) {
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
bool ArsemiArduinoCore::hasRequiredParameters(uint8_t parameterCount,
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
