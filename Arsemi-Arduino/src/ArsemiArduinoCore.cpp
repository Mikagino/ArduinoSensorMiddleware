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

/// @brief calls begin() on all the sensors added with addSensor()
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
/// @return true if the sensor has been updated due to the interval being
/// reached, otherwise false
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