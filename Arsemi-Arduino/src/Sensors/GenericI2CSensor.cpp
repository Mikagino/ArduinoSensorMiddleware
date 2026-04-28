#include "GenericI2CSensor.h"

GenericI2CSensor::GenericI2CSensor(uint8_t address = 0x00, uint8_t reg = 0x00,
                                   uint8_t bytes = 2, float scale = 1)
    : _address(address), _reg(reg), _bytes(bytes), _scale(scale) {}

bool GenericI2CSensor::begin() { return true; }

void GenericI2CSensor::updateLastValue() {
  Wire.beginTransmission(_address);
  Wire.write(_reg);
  Wire.endTransmission(false);

  Wire.requestFrom(_address, _bytes);

  unsigned int value = 0;

  for (int i = 0; i < _bytes; i++) {
    value = (value << 8) | Wire.read();
  }

  _lastValue = value;
}