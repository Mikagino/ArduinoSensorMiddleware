#include "SerialProtocol.h"
#include <Wire.h>

/// Combines the message code and the parameters to a ready-to-send message for
/// the serial interface with the microcontroller.
// String SerialProtocol::CombineToMessage(uint16_t timestamp, uint8_t code,
//                                                int parameterCount,
//                                                String *parameters) {
//   String result = String(timestamp) + Delimiter + String(code);
//   // va_list argList;
//   // va_start(argList, parameters);
//   if (parameters != nullptr) {
//     for (int i = 0; i < parameterCount; i++) {
//       result += Delimiter + String(parameters[i]);
//     }
//   }
//   // va_end(argList);
//   return result;
// }


// String SerialProtocol::FilterUnwantedSymbols(String message) {
//   String filteredMessage = message;
//   short unwantedIndex = 1;
//   while(true) {
//     unwantedIndex = filteredMessage.indexOf('?');
//     if(unwantedIndex == -1){
//       break;
//     }
//     filteredMessage.remove(unwantedIndex);
//   }
//   return filteredMessage;
// }


// void SerialProtocol::Split(String& message, Package& package) {
//   package.AllSubpackages = new StringSplitter(message, SerialProtocol::Delimiter, SerialProtocol::MaximumSubPackageCount);
//   package.SubpackageCount = package.AllSubpackages->getItemCount();

//   package.Timestamp = package.AllSubpackages->getItemAtIndex(0);
//   package.ActionCode = package.AllSubpackages->getItemAtIndex(1);
// }

// static String* SerialProtocol::SplitStringPackages(String message) {
//   message.split()
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

// String SerialProtocol::GetNextSubPackage(uint8_t start, String &message) {
//   String result = message.substring(start);
//   uint8_t nextDelimiter = message.indexOf(SerialProtocol::Delimiter);
//   //Serial.println("next del: " + String(nextDelimiter));
//   if (nextDelimiter == 0) {
//     return "";
//   }
//   result = result.substring(0, nextDelimiter - 1);
//   // Serial.println("result: " + result);
//   // message.remove(0, nextDelimiter + 1);
//   return result;
// }