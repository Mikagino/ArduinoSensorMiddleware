// Main
#include "src/ArsemiArduinoCore.h"
#include "src/IPC/SerialMessaging.h"

ArsemiArduinoCore arsemi(8);

void setup() {
  SerialMessaging::begin();
}

void loop() {
  arsemi.ParseMessage();
  arsemi.updateAllSensors();
}