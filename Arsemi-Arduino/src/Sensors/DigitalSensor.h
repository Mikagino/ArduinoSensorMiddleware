#pragma once

#include "AbstractSensor.h"
#include <Arduino.h>
#include <stdint.h>

// Generic Analog sensor, works for most simple sensors!
class DigitalSensor : public AbstractSensor {
public:
  const uint8_t ParameterByteCount = 1;
  uint8_t _sensorPin;

  // TODO: Rework all sensor's constructors to use byte[] and each class parses
  // the bytes themselves
  DigitalSensor(uint8_t sensorPin = 1);
  bool begin() override;
  void updateLastValue() override;
  bool parseParameters(uint8_t *parameter, uint8_t parameterCount) override;
};