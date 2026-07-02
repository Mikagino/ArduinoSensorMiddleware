#include "SerialPackage.h"

/// @brief Packs action code and parameters into an array
/// @return Array containing action code and all the parameters
uint8_t *SerialPackage::Serialize() {
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
bool SerialPackage::appendParameters(uint8_t *addedParameters,
                                     uint8_t addedParameterCount) {
  uint8_t resultingParameterCount = parameterCount + addedParameterCount;
  if (resultingParameterCount > MaximumParameterCount)
    return false;
  memcpy(parameters + parameterCount, addedParameters,
         sizeof(addedParameters[0]) * addedParameterCount);
  parameterCount = resultingParameterCount;
  return true;
}

/// @brief Appends a single parameter, adds from the start when there are no
/// parameters in the array
/// @param addedParameter what parameter to append
/// @param addedParameterCount how many parameters are about to be added
/// @return false if the parameters are too many for the array with a maximum
/// of MaximumParameterCount entries, otherwise true
bool SerialPackage::appendParameters(uint8_t addedParameter) {
  uint8_t resultingParameterCount = parameterCount + 1;
  if (resultingParameterCount > MaximumParameterCount)
    return false;
  parameters[parameterCount] = addedParameter;
  parameterCount = resultingParameterCount;
  return true;
}

uint8_t SerialPackage::operator[](int index) {
  if (index == 0)
    return ActionCode;
  return parameters[index];
}

void SerialPackage::reset() {
  ActionCode = 0;
  parameterCount = 0;
  parameters[0] = 0;
}