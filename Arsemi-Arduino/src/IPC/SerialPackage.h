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

  /// @brief Packs action code and parameters into an array
  /// @return Array containing action code and all the parameters
  uint8_t *Serialize() {
    uint8_t *result = new uint8_t[parameterCount + 1];
    result[0] = ActionCode;
    memcpy(&(result[1]), parameters, sizeof(parameters[0]) * parameterCount);
    return result;
  }

  /// @brief Appends all the parameters, adds from the start when there are no
  /// parameters in the array
  /// @param addedParameters what parameters to append
  /// @param addedParameterCount how many parameters are about to be added
  /// @return false if the parameters are too many for the array with a maximum
  /// of MaximumParameterCount entries, otherwise true
  bool appendParameters(uint8_t *addedParameters, uint8_t addedParameterCount) {
    uint8_t resultingParameterCount = parameterCount + addedParameterCount;
    if (resultingParameterCount > MaximumParameterCount)
      return false;
    memcpy(parameters + parameterCount, addedParameters,
           sizeof(addedParameters[0]) * addedParameterCount);
    parameterCount = resultingParameterCount;
    return true;
  }

  uint8_t getParameter(uint8_t index) { return parameters[index]; }
  uint8_t getLastParameter() { return parameters[parameterCount - 1]; }
  void removeLastParameters(uint8_t count = 1) { parameterCount -= count; }
  uint8_t getParameterCount() { return parameterCount; }

  uint8_t operator[](int index) {
    if (index == 0)
      return ActionCode;
    return parameters[index];
  }

  void reset() {
    ActionCode = 0;
    parameterCount = 0;
    parameters[0] = 0;
  }
};