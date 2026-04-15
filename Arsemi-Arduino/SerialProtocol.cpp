#include "SerialProtocol.h"

/// Combines the message code and the parameters to a ready-to-send message for
/// the serial interface with the microcontroller.
String SerialProtocol::CombineToMessage(uint16_t timestamp, uint8_t code,
                                               int parameterCount,
                                               String *parameters) {
  String result = String(timestamp) + Delimiter + String(code);
  // va_list argList;
  // va_start(argList, parameters);
  if (parameters != nullptr) {
    for (int i = 0; i < parameterCount; i++) {
      result += Delimiter + String(parameters[i]);
    }
  }
  // va_end(argList);
  return result;
}

String SerialProtocol::FilterUnwantedSymbols(String message) {
  String filteredMessage = message;
  while (true) {
    filteredMessage.remove(filteredMessage.indexOf('?'));
  }
  return filteredMessage;
}

SerialProtocol::Package *SerialProtocol::Split(String message) {
  if (message.indexOf(SerialProtocol::Delimiter) == -1) {
    return new SerialProtocol::Package();
  }

  String filteredMessage = FilterUnwantedSymbols(message);
  Package* result = new Package();

  if (filteredMessage = "") {
    return nullptr;
  }

  // String[] split = filteredMessage.Split(SerialProtocol::Delimiter);

  // Parse timestamp
  String timestampString = GetNextSubPackage(filteredMessage);
  result->Timestamp = timestampString.toInt();

  // Parse action code
  String actionCodeString = GetNextSubPackage(filteredMessage);
  result->ActionCode = actionCodeString.toInt();

  for (int i = 0; i < SerialProtocol::MaximumSubPackageCount; i++) {
    result->Parameters[i] = GetNextSubPackage(filteredMessage);
  }

  // if(!uint.TryParse(split[0], out result.Timestamp)) {
  //     throw new Exception("Couldn't parse timestamp. Message may be corrupt:
  //     " + message);
  // }
  // if(!uint.TryParse(split[1], out result.ActionCode)) {
  //     throw new Exception("Couldn't parse action code. Message may be
  //     corrupt: " + message);
  // }
  // result.Parameters = [.. split.Skip(2)];
  return result;
}

// static String* SerialProtocol::SplitStringPackages(String message) {
//   bool splitting = true;
//   String* result = new String[SerialProtocol::MaximumSubPackageCount];
//   for(int i = 0; i < SerialProtocol::MaximumSubPackageCount; i++) {
//     GetNextSubPackage(message);
//   }
//   while(splitting) {
//     GetNextSubPackage(message);
//   }
//   String* subPackages;
//   return nullptr;
// }

String SerialProtocol::GetNextSubPackage(String &message) {
  uint8_t nextDelimiter = message.indexOf(SerialProtocol::Delimiter);
  if (nextDelimiter == 0) {
    return "";
  }
  String result = message.substring(0, nextDelimiter);
  message.remove(0, nextDelimiter);
  return result;
}