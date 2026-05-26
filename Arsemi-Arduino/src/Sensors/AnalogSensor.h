#pragma once

#include "AbstractSensor.h"

// Generic Analog sensor, works for most simple sensors!
class AnalogSensor : public AbstractSensor {
protected:
  uint8_t parameterByteCount = 1;

public:
  uint8_t _sensorPin;

  // TODO: Rework all sensor's constructors to use byte[] and each class parses
  // the bytes themselves
  AnalogSensor(uint8_t sensorPin = 1);
  bool begin() override;
  void updateLastValue() override;
  bool parseParameters(uint8_t *parameter, uint8_t parameterCount) override;
};