// Main
#include "src/ArsemiArduinoCore.h"
#include "src/IPC/SerialMessaging.h"
#include "src/IPC/MessageParsing.h"

ArsemiArduinoCore arsemi(8);
MessageParsing messageParsing(arsemi);
// long lastUpdate = 0;
// long updateIntervalMs = 2000;

void setup() {
  SerialMessaging::begin();
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