#include "DigitalSensor.h"

DigitalSensor::DigitalSensor(uint8_t sensorPin) : _sensorPin(sensorPin) {}

bool DigitalSensor::begin() { return true; }

void DigitalSensor::updateLastValue() {
  _lastValue = (uint8_t)map(digitalRead(_sensorPin), 0, 1024, 1, 255);
}

bool DigitalSensor::parseParameters(uint8_t *parameter, uint8_t parameterCount) {
  if (parameterCount != ParameterByteCount) {
    return false;
  }

  intervalMillis = parameter[0];
  _sensorPin = parameter[1];
  return true;
}
