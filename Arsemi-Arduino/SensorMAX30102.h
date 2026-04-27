
#pragma once

#include "Arduino.h"
#include <stdint.h>
#include "AbstractSensor.h"
#include <MAX3010x.h>

class SensorMAX30102 : public AbstractSensor {
private:
  MAX30102 _sensor;

public:
  bool begin() override;
  void updateLastValue() override;
};