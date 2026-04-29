#pragma once

#include <Arduino.h>
#include <Wire.h>
#include <stdint.h>

/// @brief Abstraction for setting up multiple sensors in a big array.
// Polymorphism for more sophisticated sensors.
class AbstractSensor {
protected:
  uint8_t _sensorId = 0;
  uint32_t _lastReadMillis = 0; // stores the last time the sensor was read
  uint8_t _lastValue;
  static const uint8_t _parameterByteCount;

public:
  enum SensorTypes {
    EMPTY = 0,
    TYPE_GENERIC_ANALOG = 1,
    TYPE_GENERIC_DIGITAL = 2,
    TYPE_GENERIC_I2C = 3,
    TYPE_MAX30102 = 4,
  };

  // interval in which new sensor data is sent over serial
  uint8_t intervalMillis = 100;
  virtual bool begin() = 0;
  inline bool checkInterval();
  virtual bool update();
  virtual void updateLastValue() = 0;

  // Getters for private fields
  uint8_t inline getSensorId() { return _sensorId; }
  uint8_t inline getLastValue() { return _lastValue; }
  static inline uint8_t getParameterByteCount() { return _parameterByteCount; }
};
