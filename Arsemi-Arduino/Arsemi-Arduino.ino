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
  arsemi.beginAllSensors();

  Serial.println("Setup finished!");
}

void loop() {
  arsemi.updateAllSensors();
  // Serial.println("0:302:0:3057"); // new sensor sample
  // delay(intervalMillis);
  // Serial.println("0:301:911"); // Sensor error code
  // delay(intervalMillis);
  String package[1] = {"102"};
  String message = SerialProtocol::CombineToMessage(
      millis(), SerialProtocol::SystemCodes::SystemError, 1, package);
  Serial.println("Message: " + message);

  SerialProtocol::Package package2;
  SerialProtocol::Split(message, package2);
  Serial.println("Package: " + String(package2.Timestamp) + ":" + String(package2.ActionCode));
  // Serial.println("SubPack: " + SerialProtocol::GetNextSubPackage(message));
  Serial.println("\n\n---");
  delay(intervalMillis);
}