#pragma once

#include "AbstractSensor.h"
#include <Arduino.h>
#include <stdint.h>

// Generic Analog sensor, works for most simple sensors!
class DigitalSensor : public AbstractSensor {
private:
  const uint8_t _sensorPin;
  static const uint8_t _parameterByteCount = 1;

public:
  // TODO: Rework all sensor's constructors to use byte[] and each class parses
  // the bytes themselves
  DigitalSensor(uint8_t sensorPin);
  bool begin() override;
  void updateLastValue() override;
};