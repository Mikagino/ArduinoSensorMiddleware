#pragma once

#include "IPC/SerialMessaging.h"
#include "IPC/SerialProtocol.h"
#include "Sensors/AbstractSensor.h"
#include <Arduino.h>
#include <stdint.h>

class ArsemiArduinoCore {
private:
  uint8_t maxSensorCount = 8;
  AbstractSensor **sensors;
  uint8_t _currentSensorCount;


public:
  enum ERROR { SUCCESS, SENSOR_COUNT_OVERFLOW };

  ArsemiArduinoCore(uint8_t maxSensorCount);
  bool addSensor(AbstractSensor *newSensor);
  void beginAllSensors();
  void destroyAllSensors();
  bool updateAllSensors();
  uint8_t getSensorValueById(uint8_t sensorId);
  AbstractSensor *getSensorById(uint8_t sensorId);
};