#include "DigitalSensor.h"

DigitalSensor::DigitalSensor(uint8_t sensorPin) : _sensorPin(sensorPin) {}

bool DigitalSensor::begin() {}

void DigitalSensor::updateLastValue() {
  _lastValue = (uint8_t)map(digitalRead(_sensorPin), 0, 1024, 1, 255);
}