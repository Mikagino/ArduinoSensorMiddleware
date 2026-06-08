#pragma once

#include "AbstractSensor.h"
#include <Arduino.h>
#include <Wire.h>
#include <stdint.h>

// Generic I2C Sensor that works with really basic I2C sensors (fails for more
// complex ones, use the specific classes instead!) DEPRECATED!!!
class I2CSensor : public AbstractSensor {
protected:
  const uint8_t _address;
  const uint8_t _reg;
  const uint8_t _bytes;
  float _scale;

public:
  static const uint8_t ParameterByteCount = 5;

  // TODO: Rework all sensor's constructors to use byte[] and each class parses
  // the bytes themselves
  I2CSensor(uint8_t address = 0x00, uint8_t reg = 0x00, uint8_t bytes = 2);
  virtual bool begin() override;
  void updateLastValue() override;
  bool parseParameters(uint8_t *parameter, uint8_t parameterCount) override;
};