#pragma once

#include "Arduino.h"
#include <stdint.h>

// Abstraction for setting up multiple sensors in a big array
// Polymorphism for more sophisticated sensors
class AbstractSensor {
protected:
  uint8_t _sensorId = 0;
  uint32_t _lastReadMillis = 0; // stores the last time the sensor was read

public:
  enum SensorTypes{
    GENERIC_ANALOG,
    GENERIC_DIGITAL,
    GENERIC_I2C,
    MAX30102,
  };
  uint8_t intervalMillis = 100; // interval in which new sensor data is sent over serial
  uint8_t lastValue;
  virtual bool begin() = 0;
  // Checks if the last reading has been interval millis before now
  inline bool checkInterval();
  // Checks interval and calls updateLastValue() if interval time is over
  virtual bool update();
  // Reads the last value (overwritten for each type of sensor)
  virtual void updateLastValue() = 0;
  // getter for sensor id
  uint8_t getSensorId() { return _sensorId; }
};