#pragma once

#include "../ArsemiArduinoCore.h"
#include "../Sensors/AnalogSensor.h"
#include "../Sensors/DigitalSensor.h"
#include "../Sensors/I2CSensor.h"
#include "../Sensors/MAX30102Sensor.h"
#include <Arduino.h>
#include <stdint.h>

class MessageParsing {
private:
  uint8_t availableBytes;
  int _queuedActionCode = -1;
  const int queueSize = 8;
  int *queuedPackage = new int[queueSize];
  int parseNextActionCode();
  ArsemiArduinoCore& arsemiArduinoCore;

public:
  MessageParsing(ArsemiArduinoCore& newArsemiArduinoCore);
  void parseMessage();
  void parseAddSensorAction();
  bool hasRequiredParameters(uint8_t parameterCount,
                             uint8_t requiredParameterCount);
};