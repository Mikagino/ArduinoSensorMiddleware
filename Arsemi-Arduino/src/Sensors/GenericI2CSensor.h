#pragma once

#include "AbstractSensor.h"
#include <Arduino.h>
#include <Wire.h>
#include <stdint.h>

// Generic I2C Sensor that works with really basic I2C sensors (fails for more
// complex ones, use the specific classes instead!)
class GenericI2CSensor : public AbstractSensor {
protected:
  const uint8_t _address;
  const uint8_t _reg;
  const uint8_t _bytes;
  float _scale;
  static const uint8_t _parameterByteCount = 5;

public:
  // TODO: Rework all sensor's constructors to use byte[] and each class parses
  // the bytes themselves
  GenericI2CSensor(uint8_t address = 0x00, uint8_t reg = 0x00,
                   uint8_t bytes = 2, float scale = 1);
  virtual bool begin() override;
  void updateLastValue() override;
};