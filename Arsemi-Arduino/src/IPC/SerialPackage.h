#pragma once

#include <Arduino.h>
#include <stdint.h>

class SerialPackage {
public:
  /// @brief 1/4th of serial queue (should be definitely enough for all
  /// packages)
  static const uint8_t MaximumParameterCount = 64 / 4;
  static const uint8_t MaximumPackageSize = MaximumParameterCount + 3;
  uint8_t ActionCode = 0;
  uint8_t Crc8 = 0;
  bool Done = false;

private:
  uint8_t parameters[MaximumParameterCount] = {};
  uint8_t parameterCount = 0;

public:
  // Constructors
  SerialPackage() {}

  SerialPackage(uint8_t actionCode, uint8_t *parameters, uint8_t parameterCount)
      : ActionCode(actionCode), parameterCount(parameterCount) {
    appendParameters(parameters, parameterCount);
  }

  SerialPackage(uint8_t actionCode, uint8_t parameter) {
    ActionCode = actionCode;
    parameters[0] = parameter;
    parameterCount = 1;
  }

  SerialPackage(uint8_t actionCode) : ActionCode(actionCode) {}

  uint8_t getParameter(uint8_t index) { return parameters[index]; }
  uint8_t getLastParameter() { return parameters[parameterCount - 1]; }
  void removeLastParameters(uint8_t count = 1) { parameterCount -= count; }
  uint8_t getParameterCount() { return parameterCount; }
  bool appendParameters(uint8_t *addedParameters, uint8_t addedParameterCount);
  bool appendParameters(uint8_t addedParameter);
  uint8_t operator[](int index);
  uint8_t *Serialize();
  void reset();
};