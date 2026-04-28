
#pragma once

#include "AbstractSensor.h"
#include "MAX3010x/MAX30102.h"
#include <Arduino.h>
#include <stdint.h>

class SensorMAX30102 : public AbstractSensor {
private:
  MAX30102 _sensor;
  static const uint8_t _parameterByteCount = 0;

public:
  // TODO: Rework all sensor's constructors to use byte[] and each class parses
  // the bytes themselves
  SensorMAX30102();
  bool begin() override;
  void updateLastValue() override;
};