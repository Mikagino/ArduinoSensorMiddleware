#include "ArsemiArduinoCore.h"

ArsemiArduinoCore::ArsemiArduinoCore(uint8_t maxSensorCount) {
  sensors = new AbstractSensor *[maxSensorCount];
  this->maxSensorCount = maxSensorCount;
}

ArsemiArduinoCore::ERROR
ArsemiArduinoCore::addSensor(AbstractSensor *newSensor) {
  if (_currentSensorCount >= maxSensorCount)
    return ArsemiArduinoCore::SENSOR_COUNT_OVERFLOW;
  sensors[_currentSensorCount] = newSensor;
  _currentSensorCount++;
  return ArsemiArduinoCore::SUCCESS;
}

void ArsemiArduinoCore::beginAllSensors() {
  for (int i = 0; i < _currentSensorCount; i++) {
    sensors[i]->begin();
  }
}

void ArsemiArduinoCore::destroyAllSensors() {
  for (int i = 0; i < _currentSensorCount; i++) {
    sensors[i]->~AbstractSensor();
  }
}

bool ArsemiArduinoCore::updateAllSensors() {
  bool updated = false;
  bool currentlyUpdated;
  uint8_t package[3] = {SerialProtocol::SensorAction::NewSample, 1, 1};
  // Serial.println("hi");

  for (int i = 0; i < _currentSensorCount; i++) {
    if(Serial.availableForWrite() < 4){
      return false;
    }
    currentlyUpdated = sensors[i]->update();
    if(currentlyUpdated) {
      package[1] = i;
      package[2] = sensors[i]->lastValue;
      SerialMessaging::write(package, 3);
    }
    updated = (updated || currentlyUpdated); 
  }
  return updated;
}

uint8_t ArsemiArduinoCore::getSensorValueById(uint8_t sensorId) {
  return getSensorById(sensorId)->lastValue;
}

AbstractSensor *ArsemiArduinoCore::getSensorById(uint8_t sensorId) {
  for (int i = 0; i < _currentSensorCount; i++) {
    if (sensors[i]->getSensorId() == sensorId)
      return sensors[i];
  }
  return nullptr;
}


void ArsemiArduinoCore::ParseMessage() {
  int availableBytes = Serial.available();
  char* discardBuffer = new char[availableBytes];
  while(availableBytes > 0) {
    if(_queuedActionCode == 0) {
      Serial.readBytesUntil(SerialProtocol::StartByte, discardBuffer, availableBytes);
      _queuedActionCode = Serial.read();
    }

    if(_queuedActionCode == -1) {
      return;
    }

    switch (_queuedActionCode) {
    case SerialProtocol::SystemAction::HibernateMicrocontroller:
      break;
    case SerialProtocol::SystemAction::WakeMicrocontroller:
      break;
    case SerialProtocol::SetupAction::ClearConfiguration:
      break;
    case SerialProtocol::SetupAction::AddSensor:
      ParseAddSensorAction();
      break;
    default:
      SerialMessaging::write(SerialProtocol::PackageError::InvalidActionCode, 2);
    }
  }
}


void ArsemiArduinoCore::ParseAddSensorAction() {
  uint8_t availableBytes = Serial.available();
  if(availableBytes < 2){
    return;
  }

  int sensorType = Serial.read();
  availableBytes = Serial.available();
  uint8_t parameters[availableBytes] = {};
  int parameterCount = Serial.readBytesUntil(SerialProtocol::StartByte, parameters, availableBytes);

  switch (sensorType) {
  case AbstractSensor::SensorTypes::GENERIC_ANALOG: {
    if(parameterCount < 2) {
      SerialMessaging::write(SerialProtocol::PackageError::InvalidSensorParameters, 2);
    }
    GenericAnalogSensor* analogSensor = new GenericAnalogSensor(parameters[0]);
    analogSensor->intervalMillis = parameters[1];
    addSensor(analogSensor);
    break;
  }
  case AbstractSensor::GENERIC_DIGITAL: {
    if(parameterCount < 2) {
      SerialMessaging::write(SerialProtocol::PackageError::InvalidSensorParameters, 2);
    }
    GenericDigitalSensor* digitalSensor = new GenericDigitalSensor(parameters[0]);
    digitalSensor->intervalMillis = parameters[1];
    addSensor(digitalSensor);
    break;
  }
  // case AbstractSensor::SensorTypes::
  default:
    SerialMessaging::write(SerialProtocol::PackageError::InvalidSensorParameters, 2);
  }
}
