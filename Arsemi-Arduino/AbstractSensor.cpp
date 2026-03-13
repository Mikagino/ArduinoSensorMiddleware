#include "AbstractSensor.h"
#include "Arduino.h"

bool AbstractSensor::checkInterval() {
  return _lastReadMillis < (millis() - _intervalMillis);
}

bool AbstractSensor::update() {
  if (!checkInterval())
    return false;

  _lastReadMillis = millis();
  updateLastValue();
}
