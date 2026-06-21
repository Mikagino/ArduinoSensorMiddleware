#include "SerialMessaging.h"
#include "SerialProtocol.h"

/// @brief Send a message over serial with the structure
/// [PackageDelimiter | data | CRC8],
/// PackageDelimiter and CRC8 are added by the function.
/// @param data the data to be sent, must include
/// [ActionCode | Parameters[] optional ]
/// @param length how many entries has the data array
void SerialMessaging::write(uint8_t *data, uint8_t length) {
  long startTime = millis();
  do {
    // if (millis() - startTime > timeoutMs) {
    //   return;
    // }
    delayMicroseconds(50);
  } while (Serial.availableForWrite() < length + 2);

  Serial.write(SerialProtocol::PackageDelimiter);
  for (int i = 0; i < length; i++) {
    Serial.write(data[i]);
  }
  Serial.write(CRC8(data, length));
  Serial.write(SerialProtocol::PackageDelimiter);
}

/// @brief Send a message over serial with the structure
///  [PackageDelimiter | data | CRC8],
/// PackageDelimiter and CRC8 are added by the function. Waits until there is space
/// available in the serial buffer or until timeoutMs is reached.
/// @param data the data to be sent, must include
/// [ActionCode | Parameters[] optional]
/// @param length how many entries has the data array
void SerialMessaging::write(const uint8_t *data, uint8_t length) {
  long startTime = millis();
  do {
    // if (millis() - startTime > timeoutMs) {
      //   return;
      // }
      delayMicroseconds(50);
    } while (Serial.availableForWrite() < length + 2);
    
    Serial.write(SerialProtocol::PackageDelimiter);
    for (int i = 0; i < length; i++) {
      Serial.write(data[i]);
    }
    Serial.write(CRC8(data, length));
    Serial.write(SerialProtocol::PackageDelimiter);
}

/// @brief Send a package over serial with the structure
/// [PackageDelimiter | data | CRC8],
/// PackageDelimiter and CRC8 are added by the function
/// @param serialPackage the package to be sent, must include ActionCode
void SerialMessaging::write(SerialPackage &serialPackage) {
  uint8_t *serializedPackage = serialPackage.Serialize();
  write(serializedPackage, serialPackage.getParameterCount() + 1);
  delete[] serializedPackage;
}

/// @brief Send a package over serial with the structure
/// [PackageDelimiter | data | CRC8],
/// PackageDelimiter and CRC8 are added by the function
/// @param actionCode the action to be sent
void SerialMessaging::write(const uint8_t actionCode = 1) {
  uint8_t package[1] = {actionCode};
  write(package, 1);
}

/// @brief Send a package over serial with the structure
/// [PackageDelimiter | data | CRC8],
/// PackageDelimiter and CRC8 are added by the function
/// @param actionCode the action to be sent,
/// @param parameter the parameter sent after the action code
void SerialMessaging::write(const uint8_t actionCode, const uint8_t parameter) {
  uint8_t package[2] = {actionCode, parameter};
  write(package, 2);
}

/// @brief Check if at least the minimum bytes required for a package are
/// available [PackageDelimiter, Data, CRC8]
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
  uint8_t *serializedPackage = package.Serialize();
  uint8_t result = CRC8(serializedPackage, package.getParameterCount() + 1);
  delete[] serializedPackage;
  return result;
}

/// @brief Computes a 8-bit sized checksum from the data with the CRC algorithm
/// @param package package for which the crc will be calculated
/// @return 8-bit sized CRC checksum
uint8_t SerialMessaging::CRC8(uint8_t actionCode) {
  // uint8_t serializedPackage[1] = {actionCode};
  return CRC8(&actionCode, 1);
}