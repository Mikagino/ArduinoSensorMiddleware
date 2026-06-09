#pragma once

#include "AbstractSensor.h"

// Generic Analog sensor, works for most simple sensors!
class AnalogSensor : public AbstractSensor {
public:
  uint8_t _sensorPin;

  AnalogSensor(uint8_t sensorPin = 1);
  bool begin() override;
  void updateLastValue() override;
  bool parseParameters(SerialPackage &package) override;
  /// @brief Overriden for each sensor (Analog sensor needs: interval, sensor pin)
  /// @return The amount of parameters the sensor needs
  uint8_t inline getParameterByteCount() override { return 2; }
};