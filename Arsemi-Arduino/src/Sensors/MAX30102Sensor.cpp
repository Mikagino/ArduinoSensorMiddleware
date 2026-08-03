#include "MAX30102Sensor.h"

MAX30102Sensor::MAX30102Sensor() {}

/// @brief
/// @return true when successful, otherwise false
bool MAX30102Sensor::begin() {
  if (_sensor.begin()) {
    // Serial.println("max30102 started!");
    return true;
  } else {
    // Serial.println("max30102 not found...");
    return false;
  }
  Serial.begin(9600);
}

/// @brief Reads the last value (overwritten for each type of sensor)
void MAX30102Sensor::updateLastValue() {
  MAX30102Sample sample = _sensor.readSample(1000);
  // Serial.println(sample.ir);
  // Serial.println(",");
  // Serial.println(sample.red);
  _lastValue = (uint8_t)(map(sample.ir, 0, 1024, 1, 255));
}

/// @brief Parse the parameters to each parameter in the constructor
/// @param parameter All the parameters, the first index being the sensor type
/// @param parameterCount Count of parameters (should be equal to
/// getParameterByteCount(), else the parsing won't work)
/// @return false if not enough parameters, otherwise false
bool MAX30102Sensor::parseParameters(SerialPackage &package) {
  if (package.getParameterCount() != getParameterByteCount() + 1)
    return false;
  intervalMillis = package.getParameter(1);
}
