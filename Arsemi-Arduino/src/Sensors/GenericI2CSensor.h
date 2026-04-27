#pragma once

#include <Arduino.h>
#include <stdint.h>
#include "AbstractSensor.h"
#include <Wire.h>

// Generic I2C Sensor that works with really basic I2C sensors (fails for more
// complex ones, use the specific classes instead!)
class GenericI2CSensor : public AbstractSensor {
protected:
  const int _address;
  const int _reg;
  const int _bytes;
  float _scale;

public:
  GenericI2CSensor(unsigned int address = 0x00, unsigned int reg = 0x00,
                   unsigned int bytes = 2, float scale = 1)
      : _address(address), _reg(reg), _bytes(bytes), _scale(scale) {}
  void updateLastValue() override;
};