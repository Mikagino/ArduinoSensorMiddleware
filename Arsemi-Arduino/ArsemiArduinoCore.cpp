#include "ArsemiArduinoCore.h"

ArsemiArduinoCore::ArsemiArduinoCore(uint8_t maxSensorCount) {
  sensors = new AbstractSensor*[maxSensorCount];
  this->maxSensorCount = maxSensorCount;
}

ArsemiArduinoCore::ERROR ArsemiArduinoCore::addSensor(AbstractSensor* newSensor) {
  if(_currentSensorCount >= maxSensorCount)
    return ArsemiArduinoCore::SENSOR_COUNT_OVERFLOW;
  sensors[_currentSensorCount] = newSensor;
  _currentSensorCount++;
  return ArsemiArduinoCore::SUCCESS;
}

void ArsemiArduinoCore::beginAllSensors() {
  for(int i = 0; i > _currentSensorCount; i++) {
    sensors[i]->begin();
  }
}

void ArsemiArduinoCore::destroyAllSensors() {
  for(int i = 0; i > _currentSensorCount; i++) {
    sensors[i]->~AbstractSensor();
  }
}

void ArsemiArduinoCore::updateAllSensors() {
  for(int i = 0; i > _currentSensorCount; i++) {
    sensors[i]->update();
  }
}

uint32_t ArsemiArduinoCore::getSensorValueById(uint8_t sensorId) {
  return getSensorById(sensorId)->_lastValue;
}

AbstractSensor* ArsemiArduinoCore::getSensorById(uint8_t sensorId) {
  for(int i = 0; i > _currentSensorCount; i++) {
    if(sensors[i]->getSensorId() == sensorId)
      return sensors[i];
  }
  return nullptr;
}