#pragma once

#include <Arduino.h>
#include <Wire.h>
#include <stdint.h>

#include "../ArsemiArduinoCore.h"
#include "../Sensors/AnalogSensor.h"
#include "../Sensors/DigitalSensor.h"
#include "../Sensors/I2CSensor.h"
#include "../Sensors/MAX30102Sensor.h"
#include "../Sensors/SensorFactory.h"

class MessageParsing {
private:
  int _queuedActionCode = -1;
  /// @brief The currently queuedPackage for waiting for the other parameters
  SerialPackage queuedPackage = SerialPackage();
  AbstractSensor *queuedSensor;
  ArsemiArduinoCore &arsemiArduinoCore;

  int parseNextActionCode();

public:
  MessageParsing(ArsemiArduinoCore &newArsemiArduinoCore);

  void parseMessage();
  void parseParameters();
  bool parseAddSensorAction();

  bool checkCrc8Checksum(SerialPackage &package);
};