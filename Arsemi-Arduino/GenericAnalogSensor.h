#pragma once

#include "AbstractSensor.h"
#include "Arduino.h"
#include <stdint.h>

// Generic Analog sensor, works for most simple sensors!
class GenericAnalogSensor : public AbstractSensor {
private:
  const uint8_t _sensorPin;

public:
  GenericAnalogSensor(uint8_t sensorPin);
  void updateLastValue() override;
};