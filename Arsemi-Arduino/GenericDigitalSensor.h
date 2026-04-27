#pragma once

#include "AbstractSensor.h"
#include "Arduino.h"
#include <stdint.h>

// Generic Analog sensor, works for most simple sensors!
class GenericDigitalSensor : public AbstractSensor {
private:
  const uint8_t _sensorPin;

public:
  GenericDigitalSensor(uint8_t sensorPin);
  bool begin() override;
  void updateLastValue() override;
};