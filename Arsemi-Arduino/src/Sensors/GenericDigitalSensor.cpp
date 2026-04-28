#include "GenericDigitalSensor.h"

GenericDigitalSensor::GenericDigitalSensor(uint8_t sensorPin)
    : _sensorPin(sensorPin) {}


bool GenericDigitalSensor::begin() {
  
}


void GenericDigitalSensor::updateLastValue() {
  _lastValue = (uint8_t)map(digitalRead(_sensorPin), 0, 1024, 1, 255);
}