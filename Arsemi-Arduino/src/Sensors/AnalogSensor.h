#pragma once

#include "AbstractSensor.h"

// Generic Analog sensor, works for most simple sensors!
class AnalogSensor : public AbstractSensor {
private:
  const uint8_t _sensorPin;

public:
  static const uint8_t parameterByteCount = 1;
  
  // TODO: Rework all sensor's constructors to use byte[] and each class parses
  // the bytes themselves
  AnalogSensor(uint8_t sensorPin);
  bool begin() override;
  void updateLastValue() override;
};