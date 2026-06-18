#pragma once

#include "SerialPackage.h"
#include <Arduino.h>
#include <stdint.h>

class SerialProtocol {
private:
  static const uint8_t _categorySize = 32;

  class Categories {
  public:
    static const uint8_t System = _categorySize * 0;
    static const uint8_t Setup = _categorySize * 1;
    static const uint8_t Sensor = _categorySize * 2;
    static const uint8_t Package = _categorySize * 3;
  };

public:
  static const uint16_t BaudRate = 9600;
  static const uint8_t MaximumSubPackageCount = 8;
  static const uint8_t StartByte = 0;

  // TODO: use package for each command?
  // struct CommandStructure {
  // public:
  //   static const uint8_t Value;
  //   static const uint8_t ParameterCount;
  // };

  class Action {
  public:
    struct System {
    public:
      static const uint8_t HibernateMicrocontroller = Categories::System + 1;
      static const uint8_t WakeMicrocontroller = Categories::System + 2;
      static const uint8_t Error = Categories::System + 3;
      static const uint8_t RequestHandshake = Categories::System + 4;
      static const uint8_t ReplyHandshake = Categories::System + 5;
      static const uint8_t Debug = Categories::System + 6;
      static const uint8_t Heartbeat = Categories::System + 7;
    };

    struct Setup {
    public:
      static const uint8_t ClearConfiguration = Categories::Setup + 1;
      static const uint8_t SuccessfullyClearedConfiguration = Categories::Setup + 2;
      static const uint8_t AddSensor = Categories::Setup + 3;
      static const uint8_t SuccessfullyAddedSensor = Categories::Setup + 4;
    };

    struct Sensor {
    public:
      static const uint8_t NewSample = Categories::Sensor + 1;
    };
  };

  class Error {
  public:
    struct Package {
    public:
      static const uint8_t InvalidActionCode = Categories::Package + 1;
      static const uint8_t InvalidSensorParameters = Categories::Package + 2;
      static const uint8_t SensorCountOverflow = Categories::Package + 3;
      static const uint8_t InvalidSensorType = Categories::Package + 4;
      static const uint8_t PackageSizeOverflow = Categories::Package + 5;
      static const uint8_t InvalidChecksum = Categories::Package + 6;
    };
  };
};