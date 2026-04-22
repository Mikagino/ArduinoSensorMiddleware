#include "SerialMessaging.h"
#include "SerialProtocol.h"
#include "ArsemiArduinoCore.h"

ArsemiArduinoCore::ArsemiArduinoCore(uint8_t maxSensorCount) {
  sensors = new AbstractSensor *[maxSensorCount];
  this->maxSensorCount = maxSensorCount;
}

ArsemiArduinoCore::ERROR
ArsemiArduinoCore::addSensor(AbstractSensor *newSensor) {
  if (_currentSensorCount >= maxSensorCount)
    return ArsemiArduinoCore::SENSOR_COUNT_OVERFLOW;
  sensors[_currentSensorCount] = newSensor;
  _currentSensorCount++;
  return ArsemiArduinoCore::SUCCESS;
}

void ArsemiArduinoCore::beginAllSensors() {
  for (int i = 0; i < _currentSensorCount; i++) {
    sensors[i]->begin();
  }
}

void ArsemiArduinoCore::destroyAllSensors() {
  for (int i = 0; i < _currentSensorCount; i++) {
    sensors[i]->~AbstractSensor();
  }
}

bool ArsemiArduinoCore::updateAllSensors() {
  bool updated = false;
  bool currentlyUpdated;
  uint8_t package[3] = {SerialProtocol::SensorCodes::NewSample, 1, 1};
  // Serial.println("hi");

  for (int i = 0; i < _currentSensorCount; i++) {
    if(Serial.availableForWrite() < 4){
      return false;
    }
    currentlyUpdated = sensors[i]->update();
    if(currentlyUpdated) {
      package[1] = i;
      package[2] = sensors[i]->lastValue;
      SerialMessaging::write(package, 3);
    }
    updated = (updated || currentlyUpdated); 
  }
  return updated;
}

uint8_t ArsemiArduinoCore::getSensorValueById(uint8_t sensorId) {
  return getSensorById(sensorId)->lastValue;
}

AbstractSensor *ArsemiArduinoCore::getSensorById(uint8_t sensorId) {
  for (int i = 0; i < _currentSensorCount; i++) {
    if (sensors[i]->getSensorId() == sensorId)
      return sensors[i];
  }
  return nullptr;
}