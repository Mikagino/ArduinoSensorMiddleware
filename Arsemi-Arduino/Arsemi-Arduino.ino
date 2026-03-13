// Main
#include "SensorMAX30102.h"

const uint8_t MAX_SENSOR_COUNT = 8;
AbstractSensor* sensors[MAX_SENSOR_COUNT];

int intervalMillis = 100;

void setup() {
  createSensors();
  Serial.begin(9600);
  for (int i = 0; i < MAX_SENSOR_COUNT; i++) {
  }
}

void createSensors() {
  sensors[0] = new SensorMAX30102();
}

void destroySensors() {
  for (int i = 0; i < MAX_SENSOR_COUNT; i++) {
      sensors[i]->~AbstractSensor();
    }
}

void loop() {
  sensors[0]->update();
  Serial.println(sensors[0]->_lastValue);
  delay(intervalMillis);
}