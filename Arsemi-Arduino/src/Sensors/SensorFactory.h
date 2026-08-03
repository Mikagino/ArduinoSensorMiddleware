#pragma once

#include <Arduino.h>
#include <Wire.h>
#include <stdint.h>

#include "AbstractSensor.h"
#include "AnalogSensor.h"
#include "DigitalSensor.h"
#include "I2CSensor.h"
#include "MAX30102Sensor.h"

class SensorFactory {
public:
  static AbstractSensor *createNewSensor(AbstractSensor::SensorTypes type);
};