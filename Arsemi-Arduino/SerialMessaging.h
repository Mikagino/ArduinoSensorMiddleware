#pragma once

#include "Arduino.h"
#include <stdint.h>
#include "SerialProtocol.h"


class SerialMessaging {
private:
public:
  // calls Serial.begin() with specified baudrate
  static void begin(int baudRate = SerialProtocol::BaudRate);
  // Writes a package via serial, containing StartByte + buffer + CRC-8-Checksum
  static void write(uint8_t* buffer, uint8_t length);
  static void write(const uint8_t* buffer, uint8_t length);
  // static void write(SerialProtocol::SerialPackage package);
  
  // CRC-8 checksum generator based on the code by devcoons (Source: https://devcoons.com/crc8/)
  static uint8_t CRC8(uint8_t *data, uint8_t length);
  static uint8_t CRC8(const uint8_t *data, uint8_t length);
};