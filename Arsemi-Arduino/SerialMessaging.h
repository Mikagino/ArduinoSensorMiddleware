#pragma once

#include "Arduino.h"
#include <stdint.h>
#include "SerialProtocol.h"


class SerialMessaging {
public:
  static void Write(uint8_t* buffer, uint8_t length);
  static void Write(SerialProtocol::SerialPackage package);
};