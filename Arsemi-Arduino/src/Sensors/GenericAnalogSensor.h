#pragma once

#include "AbstractSensor.h"
#include <stdint.h>

// Generic Analog sensor, works for most simple sensors!
class GenericAnalogSensor : public AbstractSensor {
private:
  const uint8_t _sensorPin;

public:
  GenericAnalogSensor(uint8_t sensorPin);
  bool begin() override;
  void updateLastValue() override;
};