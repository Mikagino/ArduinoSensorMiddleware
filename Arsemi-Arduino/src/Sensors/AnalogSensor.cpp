#include "AnalogSensor.h"

AnalogSensor::AnalogSensor(uint8_t sensorPin) : _sensorPin(sensorPin) {}

bool AnalogSensor::begin() {}

void AnalogSensor::updateLastValue() {
  _lastValue = (uint8_t)map(analogRead(_sensorPin), 0, 1024, 1, 255);
}