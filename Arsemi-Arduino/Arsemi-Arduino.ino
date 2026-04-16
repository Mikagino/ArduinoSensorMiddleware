// Main
#include "ArsemiArduinoCore.h"
#include "SensorMAX30102.h"
#include "SerialProtocol.h"

ArsemiArduinoCore arsemi(8);
int intervalMillis = 2000;

void setup() {
  Serial.begin(9600);
  Serial.println("Setup started");

  arsemi.addSensor(new SensorMAX30102());
  arsemi.getSensorById(0)->intervalMillis = 200;
  arsemi.beginAllSensors();

  Serial.println("Setup finished!");
  // Serial.println((sizeof(uint8_t) * 8 + 1) / 8);
}

void loop() {
  arsemi.updateAllSensors();
  delay(1000);
}