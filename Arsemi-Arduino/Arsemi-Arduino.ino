// Main
#include "src/ArsemiArduinoCore.h"
#include "src/IPC/SerialMessaging.h"
#include "src/IPC/MessageParsing.h"

ArsemiArduinoCore arsemi(8);
MessageParsing messageParsing(arsemi);

void setup() {
  SerialMessaging::begin();

  // AbstractSensor* sensor = new MAX30102Sensor();
  // sensor->intervalMillis = 50;
  // arsemi.addSensor(sensor);
  // sensor->begin();
}

void loop() {
  messageParsing.parseMessage();
  arsemi.updateAllSensors();
}