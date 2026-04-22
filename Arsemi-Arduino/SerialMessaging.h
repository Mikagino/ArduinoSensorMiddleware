#pragma once

#include "Arduino.h"
#include <stdint.h>
#include "SerialProtocol.h"


class SerialMessaging {
private:
public:
  static uint8_t CRC8(uint8_t *data, uint8_t length);
  static void begin(int baudRate = SerialProtocol::BaudRate);
  static void write(uint8_t* buffer, uint8_t length);
  static void write(SerialProtocol::SerialPackage package);
};