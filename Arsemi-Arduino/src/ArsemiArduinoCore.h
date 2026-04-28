#pragma once

#include "IPC/SerialMessaging.h"
#include "IPC/SerialProtocol.h"
#include "Sensors/AbstractSensor.h"
#include "Sensors/GenericAnalogSensor.h"
#include "Sensors/GenericDigitalSensor.h"
#include "Sensors/GenericI2CSensor.h"
#include "Sensors/SensorMAX30102.h"
#include <Arduino.h>
#include <stdint.h>

class ArsemiArduinoCore {
private:
  uint8_t maxSensorCount = 8;
  AbstractSensor **sensors;
  uint8_t _currentSensorCount;

  int _queuedActionCode;

public:
  enum ERROR { SUCCESS, SENSOR_COUNT_OVERFLOW };

  ArsemiArduinoCore(uint8_t maxSensorCount);
  ERROR addSensor(AbstractSensor *newSensor);
  void beginAllSensors();
  void destroyAllSensors();
  bool updateAllSensors();
  uint8_t getSensorValueById(uint8_t sensorId);
  AbstractSensor *getSensorById(uint8_t sensorId);
  void ParseMessage();
  void ParseAddSensorAction();
  bool HasRequiredParameters(uint8_t parameterCount,
                             uint8_t requiredParameterCount);
};