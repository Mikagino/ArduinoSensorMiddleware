#include "AbstractSensor.h"

bool AbstractSensor::checkInterval() {
  return _lastReadMillis < (millis() - _intervalMillis);
}

bool AbstractSensor::update() {
  if (!checkInterval())
    return false;

  _lastReadMillis = millis();
  updateLastValue();
  Serial.println("Sensor " + String(_sensorId) + " -> value: " + String(_lastValue)); // DEBUG!
}
