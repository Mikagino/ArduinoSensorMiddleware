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
  uint8_t Parameters[MaximumParameterCount] = {};
  uint8_t ParameterCount = 0;

  SerialPackage() {}

  SerialPackage(uint8_t actionCode, uint8_t *parameters, uint8_t parameterCount)
      : ActionCode(actionCode), ParameterCount(parameterCount) {
    appendParameters(parameters, parameterCount);
  }

  SerialPackage(uint8_t actionCode, uint8_t parameter) {
    ActionCode = actionCode;
    Parameters[0] = parameter;
    ParameterCount = 1;
  }

  SerialPackage(uint8_t actionCode) : ActionCode(actionCode) {}

  /// @brief Packs action code and parameters into an array
  /// @return Array containing action code and all the parameters
  uint8_t *Serialize() {
    uint8_t *result = new uint8_t[ParameterCount + 1];
    result[0] = ActionCode;
    memcpy(&(result[1]), Parameters, sizeof(Parameters[0]) * ParameterCount);
    return result;
  }

  /// @brief Appends all the parameters, adds from the start when there are no
  /// parameters in the array
  /// @param parameters
  /// @param parameterCount
  /// @return false if the parameters are too many for the array with a maximum
  /// of 64/8 entries, otherwise true
  bool appendParameters(uint8_t *parameters, uint8_t parameterCount) {
    uint8_t resultingParameterCount = ParameterCount + parameterCount;
    if (resultingParameterCount > MaximumParameterCount)
      return false;
    memcpy(Parameters, parameters + ParameterCount,
           sizeof(parameters[0]) * parameterCount);
    ParameterCount = resultingParameterCount;
    return true;
  }
};