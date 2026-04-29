#include "ArsemiArduinoCore.h"

ArsemiArduinoCore::ArsemiArduinoCore(uint8_t maxSensorCount) {
  sensors = new AbstractSensor *[maxSensorCount];
  this->maxSensorCount = maxSensorCount;
}

/// adds a new sensor to an array for batch calls of functions
ArsemiArduinoCore::ERROR
ArsemiArduinoCore::addSensor(AbstractSensor *newSensor) {
  if (_currentSensorCount >= maxSensorCount)
    return ArsemiArduinoCore::SENSOR_COUNT_OVERFLOW;
  sensors[_currentSensorCount] = newSensor;
  _currentSensorCount++;
  return ArsemiArduinoCore::SUCCESS;
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
  uint8_t package[3] = {SerialProtocol::SensorAction::NewSample, 1, 1};

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
/// @return
AbstractSensor *ArsemiArduinoCore::getSensorById(uint8_t sensorId) {
  for (int i = 0; i < _currentSensorCount; i++) {
    if (sensors[i]->getSensorId() == sensorId)
      return sensors[i];
  }
  return nullptr;
}

/// @brief Parse a new serial package with its action code to the according
/// actions and invoke associated functions, considering the parameters of the
/// package
void ArsemiArduinoCore::ParseMessage() {
  int availableBytes = Serial.available();
  char *discardBuffer = new char[availableBytes];
  while (availableBytes > 0) {
    if (_queuedActionCode == 0) {
      Serial.readBytesUntil(SerialProtocol::StartByte, discardBuffer,
                            availableBytes);
      _queuedActionCode = Serial.read();
    }

    if (_queuedActionCode == -1) {
      return;
    }

    switch (_queuedActionCode) {
    case SerialProtocol::SystemAction::HibernateMicrocontroller:
      break;
    case SerialProtocol::SystemAction::WakeMicrocontroller:
      break;
    case SerialProtocol::SetupAction::ClearConfiguration:
      break;
    case SerialProtocol::SetupAction::AddSensor:
      ParseAddSensorAction();
      break;
    default:
      SerialMessaging::write(SerialProtocol::PackageError::InvalidActionCode,
                             2);
    }
  }
}

/// @brief Parse action "Add Sensor"
/// Package parameters: (sensorType + intervalMs + constructorParameters[])
void ArsemiArduinoCore::ParseAddSensorAction() {
  uint8_t availableBytes = Serial.available();
  if (availableBytes < 2) {
    return;
  }

  int sensorType = Serial.read();
  availableBytes = Serial.available();
  uint8_t parameters[availableBytes] = {};
  int parameterCount = Serial.readBytesUntil(SerialProtocol::StartByte,
                                             parameters, availableBytes);
  if (parameterCount < 1) {
    return;
  }
  AbstractSensor *newSensor;

  switch (sensorType) {
  case AbstractSensor::SensorTypes::TYPE_GENERIC_ANALOG: {
    if (HasRequiredParameters(parameterCount,
                              (AnalogSensor::getParameterByteCount() + 1))) {
      newSensor = new AnalogSensor(parameters[1]);
    }
    break;
  }

  case AbstractSensor::TYPE_GENERIC_DIGITAL: {
    if (HasRequiredParameters(
            parameterCount,
            (DigitalSensor::getParameterByteCount() + 1))) {
      newSensor = new DigitalSensor(parameters[1]);
    }
    break;
  }

  case AbstractSensor::SensorTypes::TYPE_GENERIC_I2C: {
    if (HasRequiredParameters(
            parameterCount, (I2CSensor::getParameterByteCount() + 1))) {
      newSensor = new I2CSensor(parameters[1], parameters[2],
                                       parameters[3], parameters[4]);
    }
    break;
  }

  case AbstractSensor::SensorTypes::TYPE_MAX30102: {
    if (HasRequiredParameters(parameterCount,
                              (MAX30102Sensor::getParameterByteCount() + 1))) {
      newSensor = new MAX30102Sensor();
    }
    break;
  }

  default:
    SerialMessaging::write(
        SerialProtocol::PackageError::InvalidSensorParameters, 2);
    return;
  }

  newSensor->intervalMillis = parameters[0];
  addSensor(newSensor);
}

/// @brief Checks if the following parameters (next bytes) are enough, if not it
/// sends an error message to serial
/// @param requiredParameterCount
/// @return
bool ArsemiArduinoCore::HasRequiredParameters(uint8_t parameterCount,
                                              uint8_t requiredParameterCount) {
  if (parameterCount < 2) {
    SerialMessaging::write(
        SerialProtocol::PackageError::InvalidSensorParameters, 2);
    return false;
  }
  return true;
}
