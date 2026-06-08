// Main
#include "src/ArsemiArduinoCore.h"
#include "src/IPC/SerialMessaging.h"
#include "src/IPC/MessageParsing.h"

ArsemiArduinoCore arsemi(8);
MessageParsing messageParsing(arsemi);

void setup() {
  SerialMessaging::begin();
}

void loop() {
  messageParsing.parseMessage();
  arsemi.updateAllSensors();
}