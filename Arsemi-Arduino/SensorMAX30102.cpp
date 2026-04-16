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
  Serial.begin(9600);
}

void SensorMAX30102::updateLastValue() {
  auto sample = sensor.readSample(1000);
  // Serial.println(sample.ir);
  // Serial.println(",");
  // Serial.println(sample.red);
  lastValue = (uint8_t)(map(sample.ir, 0, 1024, 1, 255));
}