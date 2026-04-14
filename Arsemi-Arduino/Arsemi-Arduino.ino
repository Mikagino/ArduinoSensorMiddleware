// Main
#include "SensorMAX30102.h"
#include "ArsemiArduinoCore.h"

ArsemiArduinoCore arsemi(8);
int intervalMillis = 2000;

void setup() {
  Serial.begin(9600);
  Serial.println("Setup started");

  arsemi.addSensor(new SensorMAX30102());
  arsemi.beginAllSensors();

  Serial.println("Setup finished!");
}

void loop() {
  arsemi.updateAllSensors();
  Serial.println("0:302:0:3057"); // new sensor sample
  delay(intervalMillis);
  Serial.println("0:301:911"); // Sensor error code
  delay(intervalMillis);
}