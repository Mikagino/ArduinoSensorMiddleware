// Main
#include "src/ArsemiArduinoCore.h"
#include "src/IPC/SerialMessaging.h"

ArsemiArduinoCore arsemi(8);

void setup() {
  SerialMessaging::begin();

  // AbstractSensor* sensor = new MAX30102Sensor();
  // sensor->intervalMillis = 50;
  // arsemi.addSensor(sensor);
  // sensor->begin();
}

void loop() {
  // SerialMessaging::write(SerialProtocol::SystemAction::ReplyHandshake);
  arsemi.parseMessage();
  //arsemi.updateAllSensors();
}