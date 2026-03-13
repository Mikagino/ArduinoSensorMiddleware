#include "Arduino.h"
#include "GenericAnalogSensor.h"

void GenericAnalogSensor::updateLastValue() {
  _lastValue = analogRead(_pin);
}