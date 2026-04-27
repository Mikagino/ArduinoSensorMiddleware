
#pragma once

#include <Arduino.h>
#include "AbstractSensor.h"
#include "MAX3010x/MAX30102.h"
#include <stdint.h>

class SensorMAX30102 : public AbstractSensor {
private:
  MAX30102 _sensor;

public:
  bool begin() override;
  void updateLastValue() override;
};