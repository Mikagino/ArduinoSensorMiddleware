#include "SensorFactory.h"

AbstractSensor *SensorFactory::createNewSensor(AbstractSensor::SensorTypes type) {
  switch (type) {
  case AbstractSensor::SensorTypes::TYPE_GENERIC_ANALOG:
    return new AnalogSensor();
  case AbstractSensor::SensorTypes::TYPE_GENERIC_DIGITAL:
    return new DigitalSensor();
  case AbstractSensor::SensorTypes::TYPE_GENERIC_I2C:
    return new I2CSensor();
  case AbstractSensor::SensorTypes::TYPE_MAX30102:
    return new MAX30102Sensor();
  default:
    return nullptr;
    break;
  }
}