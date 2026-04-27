#include "SerialProtocol.h"
#include "SerialMessaging.h"


void SerialMessaging::write(uint8_t* buffer, uint8_t length) {
  Serial.write(SerialProtocol::StartByte);
  
  for(int i = 0; i < length; i++) {
    Serial.write(buffer[i]);
  }
  Serial.write(CRC8(buffer, length));
}


void SerialMessaging::write(const uint8_t* buffer, uint8_t length) {
  Serial.write(SerialProtocol::StartByte);
  
  for(int i = 0; i < length; i++) {
    Serial.write(buffer[i]);
  }
  Serial.write(CRC8(buffer, length));
}


void SerialMessaging::begin(int baudRate){
  Serial.begin(baudRate);
}


uint8_t SerialMessaging::CRC8(uint8_t *data, uint8_t length) {
  uint8_t crc = 0x00;
  uint8_t extract;
  uint8_t sum;
  for(int i=0;i<length;i++){
      extract = *data;
      for (char tempI = 8; tempI; tempI--) {
         sum = (crc ^ extract) & 0x01;
         crc >>= 1;
         if (sum)
            crc ^= 0x8C;
         extract >>= 1;
      }
      data++;
   }
   return crc;
}


uint8_t SerialMessaging::CRC8(const uint8_t *data, uint8_t length) {
  uint8_t crc = 0x00;
  uint8_t extract;
  uint8_t sum;
  for(int i=0;i<length;i++){
      extract = *data;
      for (char tempI = 8; tempI; tempI--) {
         sum = (crc ^ extract) & 0x01;
         crc >>= 1;
         if (sum)
            crc ^= 0x8C;
         extract >>= 1;
      }
      data++;
   }
   return crc;
}