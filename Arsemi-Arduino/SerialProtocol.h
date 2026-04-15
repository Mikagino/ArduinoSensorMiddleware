#include <cstdint>
#pragma once

#include "Arduino.h"
#include <stdint.h>

class SerialProtocol {
private:
  class Categories {
  public:
    static const uint8_t System = 1;
    static const uint8_t Setup = 2;
    static const uint8_t Sensor = 3;
  };

  /// Filters out unwanted symbols (currently only "?")
  static String FilterUnwantedSymbols(String message);
  static String GetNextSubPackage(String &message);

  public:
    static const char Delimiter = ':';
    static const uint8_t CodeShifter = 100;
    static const uint16_t BaudRate = 9600;
    static const uint8_t MaximumSubPackageCount = 8;

    class SystemCodes {
    public:
      static const uint16_t HibernateMicrocontroller =
          (Categories::System * CodeShifter) + 1;
      static const uint16_t WakeMicrocontroller =
          (Categories::System * CodeShifter) + 2;
      static const uint16_t SystemError =
          (Categories::System * CodeShifter) + 3;
      static const uint16_t RequestHandshake =
          (Categories::System * CodeShifter) + 4;
      static const uint16_t ReplyHandshake =
          (Categories::System * CodeShifter) + 5;
    };

    struct SetupCodes {
    public:
      static const uint16_t ClearConfiguration =
          (Categories::Setup * CodeShifter) + 1;
      static const uint16_t AddSensor =
          (Categories::Setup * CodeShifter) + 2;
    };

    struct SensorCodes {
    public:
      static const uint16_t NewSample =
          (Categories::Sensor * CodeShifter) + 1;
    };

    struct Package {
    public:
      uint16_t Timestamp = 0;
      uint8_t ActionCode = 0;
      String *Parameters;
    };

    /// Combines the message code and the parameters to a ready-to-send message
    /// for the serial interface with the microcontroller.
    static String CombineToMessage(uint16_t timestamp, uint8_t code,
                                   int parameterCount, String *parameters);
    /// Splits a serial message into timestamp, action code and up to (MaximumSubPackages-2) parameters
    static Package *Split(String message);
  };