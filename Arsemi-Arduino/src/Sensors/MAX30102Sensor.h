
#pragma once

#include "AbstractSensor.h"
#include "MAX3010x/MAX30102.h"
#include <Arduino.h>
#include <stdint.h>

class MAX30102Sensor : public AbstractSensor {
private:
  MAX30102 _sensor = MAX30102();

public:
  // TODO: Rework all sensor's constructors to use byte[] and each class parses
  // the bytes themselves
  MAX30102Sensor();
  ~MAX30102Sensor() {}
  bool begin() override;
  void updateLastValue() override;
  bool parseParameters(SerialPackage &package) override;
  uint8_t inline getParameterByteCount() override { return 1; }
};