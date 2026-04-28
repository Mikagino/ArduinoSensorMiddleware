#include "GenericAnalogSensor.h"

GenericAnalogSensor::GenericAnalogSensor(uint8_t sensorPin)
    : _sensorPin(sensorPin) {}


bool GenericAnalogSensor::begin() {
  
}


void GenericAnalogSensor::updateLastValue() {
  _lastValue = (uint8_t)map(analogRead(_sensorPin), 0, 1024, 1, 255);
}