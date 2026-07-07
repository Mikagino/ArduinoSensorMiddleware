#include "SerialPackage.h"

/// @brief Packs action code and parameters into an array
/// @return Array containing action code and all the parameters
uint8_t *SerialPackage::Serialize() {
  uint8_t *result = new uint8_t[parameterCount + 1];
  result[0] = ActionCode;
  memcpy(result + 1, parameters, sizeof(parameters[0]) * parameterCount);
  return result;
}

/// @brief Deletes the last parameter and returns it
/// @return Last parameter of the package
uint8_t SerialPackage::popLastParameter() {
  uint8_t result = getLastParameter();
  removeLastParameters();
  return result;
}

/// @brief Remove the x first parameters (default is 1)
/// @param count how many parameters to remove from the end
void SerialPackage::removeLastParameters(uint8_t count) {
  parameterCount -= count;
  length -= count;
}

/// @brief Appends all the parameters, adds from the start when there are no
/// parameters in the array
/// @param addedParameters what parameters to append
/// @param addedParameterCount how many parameters are about to be added
/// @return false if the parameters are too many for the array with a maximum
/// of MaximumParameterCount entries, otherwise true
bool SerialPackage::appendParameters(uint8_t *addedParameters,
                                     uint8_t addedParameterCount) {
  uint8_t lengthWithoutParams = length - parameterCount;
  uint8_t resultingParameterCount = parameterCount + addedParameterCount;
  if (resultingParameterCount > MaximumParameterCount)
    return false;
  memcpy(parameters + parameterCount, addedParameters,
         sizeof(addedParameters[0]) * addedParameterCount);
  parameterCount = resultingParameterCount;
  length = lengthWithoutParams + resultingParameterCount;
  return true;
}

/// @brief Appends a single parameter, adds from the start when there are no
/// parameters in the array
/// @param addedParameter what parameter to append
/// @return false if the parameters are too many for the array with a maximum
/// of MaximumParameterCount entries, otherwise true
bool SerialPackage::appendParameters(uint8_t addedParameter) {
  if (parameterCount + 1 > MaximumParameterCount)
    return false;
  parameters[parameterCount] = addedParameter;
  length = length - parameterCount + 1;
  parameterCount += 1;
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
  length = 0;
}