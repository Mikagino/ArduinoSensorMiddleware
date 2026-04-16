#include "SerialProtocol.h"
#include "SerialMessaging.h"


void SerialMessaging::Write(uint8_t* buffer, uint8_t length) {
  Serial.write(SerialProtocol::StartByte);
  
  for(int i = 0; i < length; i++){
    Serial.write(buffer[i]);
  }
}