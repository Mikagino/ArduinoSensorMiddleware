// Main
#include "ArsemiArduinoCore.h"
#include "SensorMAX30102.h"
#include "SerialProtocol.h"

ArsemiArduinoCore arsemi(8);
int intervalMillis = 2000;

void setup() {
  Serial.begin(SerialProtocol::BaudRate);
  Serial.println("Setup started");

  arsemi.addSensor(new SensorMAX30102());
  arsemi.getSensorById(0)->intervalMillis = 250;
  arsemi.beginAllSensors();

  Serial.println("Setup finished!");
}

void loop() {
  arsemi.updateAllSensors();
}