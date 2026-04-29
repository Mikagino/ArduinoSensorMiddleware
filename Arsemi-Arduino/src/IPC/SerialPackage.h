#pragma once

#include <Arduino.h>
#include <stdint.h>

class SerialPackage {
public:
  uint8_t ActionCode = 0;
  uint8_t *Parameters;
  uint8_t ParameterCount = 0;

  SerialPackage(uint8_t actionCode, uint8_t *parameters, uint8_t parameterCount)
      : ActionCode(actionCode), Parameters(parameters),
        ParameterCount(parameterCount) {}

  uint8_t *Serialize() {
    uint8_t *result = new uint8_t[ParameterCount + 1];
    result[0] = ActionCode;
    for (int i = 0; i < ParameterCount; i++) {
      result[i + 1] = Parameters[i];
    }
    return result;
  }
};