#pragma once

#include "Arduino.h"
#include <stdint.h>


class SerialProtocol {
private:
  static const uint8_t _categorySize = 32;

  class Categories {
  public:
    static const uint8_t System = _categorySize * 0;
    static const uint8_t Setup = _categorySize * 1;
    static const uint8_t Sensor = _categorySize * 2;
    static const uint8_t Package = _categorySize * 2;
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


#pragma region Action Codes
    struct SystemAction {
    public:
      static const uint8_t HibernateMicrocontroller = Categories::System + 1;
      static const uint8_t WakeMicrocontroller = Categories::System + 2;
      static const uint8_t Error = Categories::System + 3;
      static const uint8_t RequestHandshake = Categories::System + 4;
      static const uint8_t ReplyHandshake = Categories::System + 5;
    };

    struct SetupAction {
    public:
      static const uint8_t ClearConfiguration = Categories::Setup + 1;
      static const uint8_t AddSensor = Categories::Setup + 2;
    };

    struct SensorAction {
    public:
      static const uint8_t NewSample = Categories::Sensor + 1;
    };
#pragma endregion Action Codes


#pragma region Errors 
    struct PackageError {
    public:
      static constexpr uint8_t InvalidActionCode[2] = {SystemAction::Error, Categories::Package + 1};
      static constexpr uint8_t InvalidSensorParameters[2] = {SystemAction::Error, Categories::Package + 1};
    };
#pragma region Errors


    class SerialPackage {
    public:
      uint8_t ActionCode = 0;
      byte* Parameters;
      uint8_t ParameterCount = 0;
    };
  };