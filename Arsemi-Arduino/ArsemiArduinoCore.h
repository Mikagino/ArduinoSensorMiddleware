#pragma once

#include "AbstractSensor.h"
#include "Arduino.h"
#include <stdint.h>

class ArsemiArduinoCore {
private:
  uint8_t maxSensorCount = 8;
  AbstractSensor **sensors;
  uint8_t _currentSensorCount;

public:
  enum ERROR { SUCCESS, SENSOR_COUNT_OVERFLOW };

  ArsemiArduinoCore(uint8_t maxSensorCount);
  // adds a new sensor to an array for batch calls of functions
  ArsemiArduinoCore::ERROR addSensor(AbstractSensor *newSensor);
  // calls begin() on all the sensors added with addSensor()
  void beginAllSensors();
  // calls Deconstructor on all the sensors added with addSensor()
  void destroyAllSensors();
  // calls update() on all the sensors added with addSensor() and sends the new sensor data over serial if it has been updated and the stream has enough bytes available
  bool updateAllSensors();
  // gets _lastValue from the sensor with the id
  uint8_t getSensorValueById(uint8_t sensorId);
  // iterates over all sensors and returns the sensors or nullptr when the id is
  // not found
  AbstractSensor *getSensorById(uint8_t sensorId);
};