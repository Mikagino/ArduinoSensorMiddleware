// Main
#include "src/ArsemiArduinoCore.h"
#include "src/IPC/SerialMessaging.h"
#include "src/IPC/MessageParsing.h"
#include "src/Sensors/AbstractSensor.h"
#include "src/Sensors/MAX30102Sensor.h"

ArsemiArduinoCore arsemi(8);
MessageParsing messageParsing(arsemi);
// long lastUpdate = 0;
// long updateIntervalMs = 2000;

void setup() {
  SerialMessaging::begin();
  AbstractSensor* button = new DigitalSensor(2);
  button->intervalMillis = 50;
  arsemi.addSensor(button);

  AbstractSensor* hrSensor = new MAX30102Sensor();
  hrSensor->intervalMillis = 50;
  arsemi.addSensor(hrSensor);
  //arsemi.addSensor(DigitalSensor(2));
  //Serial.println("Arsemi started!");
}

void loop() {
  messageParsing.parseMessage();
  arsemi.updateAllSensors();
  
  // if(millis() - lastUpdate > updateIntervalMs) {
  //   lastUpdate = millis();
  //   SerialMessaging::write(SerialProtocol::Action::System::Heartbeat);
  // }
}