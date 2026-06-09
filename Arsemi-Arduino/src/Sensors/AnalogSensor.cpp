#include "AnalogSensor.h"

AnalogSensor::AnalogSensor(uint8_t sensorPin) : _sensorPin(sensorPin) {}

bool AnalogSensor::begin() { return true; }

void AnalogSensor::updateLastValue() {
  _lastValue = (uint8_t)map(analogRead(_sensorPin), 0, 1024, 1, 255);
}

/// @brief Parse the parameters to each parameter in the constructor
/// @param parameter All the parameters, the first index being the sensor type
/// @param parameterCount Count of parameters (should be equal to
/// getParameterByteCount(), else the parsing won't work)
/// @return false if not enough parameters, otherwise false
bool AnalogSensor::parseParameters(SerialPackage& package) {
  if (package.getParameterCount() != getParameterByteCount() + 1) {
    return false;
  }

  intervalMillis = package.getParameter(1);
  _sensorPin = package.getParameter(2);
  return true;
}
