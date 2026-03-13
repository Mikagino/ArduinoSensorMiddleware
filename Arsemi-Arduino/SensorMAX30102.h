#ifndef SENSORMAX30102H
#define SENSORMAX30102H

#include "GenericI2CSensor.h"
#include <MAX3010x.h>

class SensorMAX30102 : public GenericI2CSensor {
private:
  MAX30102 sensor;

public:
  bool begin() override;
  void updateLastValue() override;
};

#endif