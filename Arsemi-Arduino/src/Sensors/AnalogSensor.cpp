#include "AnalogSensor.h"

AnalogSensor::AnalogSensor(uint8_t sensorPin) : _sensorPin(sensorPin) {}

bool AnalogSensor::begin() { return true; }

void AnalogSensor::updateLastValue() {
  _lastValue = (uint8_t)map(analogRead(_sensorPin), 0, 1024, 1, 255);
}

bool AnalogSensor::parseParameters(uint8_t *parameter, uint8_t parameterCount) {
  if (parameterCount != ParameterByteCount) {
    return false;
  }

  intervalMillis = parameter[0];
  _sensorPin = parameter[1];
  return true;
}
