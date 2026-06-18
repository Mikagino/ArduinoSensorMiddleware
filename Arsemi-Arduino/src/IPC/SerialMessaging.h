#pragma once

#include "SerialPackage.h"
#include "SerialProtocol.h"
#include <Arduino.h>
#include <stdint.h>

class SerialMessaging {
private:
  static const long timeoutMs = 1000;

public:
  // calls Serial.begin() with specified baudrate
  static void begin(int baudRate = SerialProtocol::BaudRate);
  // Writes a package via serial, containing StartByte + buffer + CRC-8-Checksum
  static void write(uint8_t *buffer, uint8_t length);
  static void write(const uint8_t *buffer, uint8_t length);
  static void write(SerialPackage &package);
  static void write(const uint8_t actionCode);
  static void write(const uint8_t actionCode, const uint8_t parameter);

  static bool isPackageAvailable(uint8_t dataLength = 1);

  // CRC-8 checksum generator based on the code by devcoons (Source:
  // https://devcoons.com/crc8/)
  static uint8_t CRC8(uint8_t *data, uint8_t length);
  static uint8_t CRC8(const uint8_t *data, uint8_t length);
  static uint8_t CRC8(SerialPackage &package);
  static uint8_t CRC8(uint8_t actionCode);
};