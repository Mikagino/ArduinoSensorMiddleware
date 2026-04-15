#include "GenericAnalogSensor.h"

GenericAnalogSensor::GenericAnalogSensor(uint8_t sensorPin)
    : _sensorPin(sensorPin) {}

void GenericAnalogSensor::updateLastValue() {
  _lastValue = analogRead(_sensorPin);
}