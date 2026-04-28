#include "AbstractSensor.h"

/// @brief Checks if the last reading has been interval millis before now
/// @return 
bool AbstractSensor::checkInterval() {
  bool result = (_lastReadMillis + intervalMillis) > millis();
  return result;
}

/// @brief Checks interval and calls updateLastValue() if interval time is over
/// @return 
bool AbstractSensor::update() {
  if (checkInterval()){
    return false;
  }

  _lastReadMillis = millis();
  updateLastValue();
  return true;
  // Serial.println("Sensor " + String(_sensorId) +
                //  " -> value: " + String(_lastValue)); // DEBUG!
}