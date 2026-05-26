#include "SerialMessaging.h"
#include "SerialProtocol.h"

void SerialMessaging::write(uint8_t *buffer, uint8_t length) {
  Serial.write(SerialProtocol::StartByte);

  for (int i = 0; i < length; i++) {
    Serial.write(buffer[i]);
  }
  Serial.write(CRC8(buffer, length));
}

void SerialMessaging::write(const uint8_t *buffer, uint8_t length) {
  Serial.write(SerialProtocol::StartByte);

  for (int i = 0; i < length; i++) {
    Serial.write(buffer[i]);
  }
  Serial.write(CRC8(buffer, length));
}

void SerialMessaging::write(SerialPackage &serialPackage) {
  write(serialPackage.Serialize(), serialPackage.ParameterCount + 1);
}

void SerialMessaging::write(const uint8_t actionCode = 1) {
  uint8_t package[1] = {actionCode};
  write(package, 1);
}

void SerialMessaging::write(const uint8_t actionCode, const uint8_t parameter) {
  uint8_t package[2] = {actionCode, parameter};
  write(package, 2);
}

/// @brief Check if at least the minimum bytes required for a package are
/// available [StartByte, Data, CRC8]
/// @param dataLength how many data bytes the package should have (action code +
/// params)
/// @return
bool SerialMessaging::isPackageAvailable(uint8_t dataLength) {
  return Serial.available() >= (2 + dataLength);
}

void SerialMessaging::begin(int baudRate) { Serial.begin(baudRate); }

/// @brief Computes a 8-bit sized checksum from the data with the CRC algorithm
/// @param data array of the data for the message
/// @param length length of the data array
/// @return 8-bit sized CRC checksum
uint8_t SerialMessaging::CRC8(uint8_t *data, uint8_t length) {
  uint8_t crc = 0x00;
  uint8_t extract;
  uint8_t sum;
  for (int i = 0; i < length; i++) {
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

/// @brief Computes a 8-bit sized checksum from the data with the CRC algorithm
/// @param data array of the data for the message
/// @param length length of the data array
/// @return 8-bit sized CRC checksum
uint8_t SerialMessaging::CRC8(const uint8_t *data, uint8_t length) {
  uint8_t crc = 0x00;
  uint8_t extract;
  uint8_t sum;
  for (int i = 0; i < length; i++) {
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

/// @brief Computes a 8-bit sized checksum from the data with the CRC algorithm
/// @param package package for which the crc will be calculated
/// @return 8-bit sized CRC checksum
uint8_t SerialMessaging::CRC8(SerialPackage &package) {
  return CRC8(package.Serialize(), package.ParameterCount);
}

/// @brief Debugging tool to send messages over serial to make serial blink
/// @param count
/// @param delayMillis
void SerialMessaging::blink(int count, int delayMillis) {
  for (int i = 0; i < count; i++) {
    Serial.println("blink");
    delay(delayMillis);
  }
}
