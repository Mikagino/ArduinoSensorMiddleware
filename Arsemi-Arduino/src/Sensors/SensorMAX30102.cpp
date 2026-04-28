#include "SensorMAX30102.h"

SensorMAX30102::SensorMAX30102() {}

// Returns false on error
bool SensorMAX30102::begin() {
  if (_sensor.begin()) {
    Serial.println("max30102 started!");
    return true;
  } else {
    Serial.println("max30102 not found...");
    return false;
  }
  Serial.begin(9600);
}

/// @brief Reads the last value (overwritten for each type of sensor)
void SensorMAX30102::updateLastValue() {
  MAX30102Sample sample = _sensor.readSample(1000);
  // Serial.println(sample.ir);
  // Serial.println(",");
  // Serial.println(sample.red);
  _lastValue = (uint8_t)(map(sample.ir, 0, 1024, 1, 255));
}