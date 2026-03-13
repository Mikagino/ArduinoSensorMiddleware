#include <Wire.h>
#include "GenericI2CSensor.h"

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