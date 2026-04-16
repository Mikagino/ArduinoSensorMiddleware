#include "AbstractSensor.h"

bool AbstractSensor::checkInterval() {
  bool result = (_lastReadMillis + intervalMillis) > millis();
  return result;
}

bool AbstractSensor::update() {
  if (checkInterval()){
    return false;
  }

  _lastReadMillis = millis();
  updateLastValue();
  return true;
  // Serial.println("Sensor " + String(_sensorId) +
                //  " -> value: " + String(lastValue)); // DEBUG!
}