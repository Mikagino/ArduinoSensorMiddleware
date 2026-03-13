#ifndef GENERIC_ANALOG_SENSOR
#define GENERIC_ANALOG_SENSOR

#include "AbstractSensor.h"

class GenericAnalogSensor : public AbstractSensor {
private:
  unsigned int _pin;

public:
  void updateLastValue() override;
};

#endif