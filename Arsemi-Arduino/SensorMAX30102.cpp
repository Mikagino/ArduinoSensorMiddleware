#include "SensorMAX30102.h"

// Returns false on error
bool SensorMAX30102::begin() {
  if (sensor.begin()) {
    Serial.println("max30102 started!");
    return true;
  } else {
    Serial.println("max30102 not found...");
    return false;
  }
}

void SensorMAX30102::updateLastValue() {
  _lastValue = sensor.readSample(1000).red;
}