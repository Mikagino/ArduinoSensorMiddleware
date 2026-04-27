using System.Globalization;
using System.IO.Ports;
using System.Text.Json.Serialization;
using Arsemi.IPC;
using Arsemi.Sensor;
using NullFX.CRC;

namespace Arsemi {
  /// <summary>
  /// Flexible API for microcontroller based sensor networks that are supplied to the user in an easy accessible manner.
  /// Multiple sensors can be setup, those settings can be stored and loaded.
  /// Filters can be applied to each sensor and much more...
  /// For more information: TODO<Insert Link>
  /// </summary>
  public class ArsemiCore {
    [JsonInclude] public Dictionary<string, AbstractSensor> Sensors = [];

    // IPC
    private readonly MemoryMappedSensorData _memoryMappedSensorData = new("SensorData", 1024);
    private SerialMessaging _serialMessaging = new();

    private List<Timer> _timers = [];

    private long _tick = 0; // DEBUG
    private float[] _testValues = [0, 1, 2, 1, 2, 3, 4, 7, 4, 3];


    private int _queuedActionCode = 0;


    /// <summary>
    /// TODO: Setup communication with arduino an other important things :> (can this be combined with FinishSetup() into one method?)
    /// </summary>
    public void StartSetup() {
    }


    /// <summary>
    /// TODO: Loops when program is running (in Program.cs?)
    /// </summary>
    public void Update() {
    }


    // /// <summary>
    // /// TODO: Stores sensor values received from the microcontroller in shared memory | 
    // /// NICE TO HAVE: Make timed for each sensor individually -> for performance setting / more control
    // /// </summary>
    private void StoreSensorData(uint sensorId) {
      AbstractSensor currentSensor = Sensors[AbstractSensor.ParseSensorIdToName(sensorId)];
      currentSensor.ApplyFilters();
      _memoryMappedSensorData.Write(currentSensor.Data);
    }


    /// <summary>
    /// TODO: Setup new sensors and store them for later use, call Finish() when the initial sensor setup is done
    /// </summary>
    /// <returns>New sensor for using it in a stacked setup</returns>
    public AbstractSensor AddSensor(AbstractSensor sensor, string name) {
      Sensors.Add(name, sensor);
      return sensor;
    }


    /// <summary>
    /// </summary>
    /// <returns>TODO: Current sensor value based on filters, timings, etc.</returns>
    public float GetSensorValue(uint sensorID) {
      return Sensors[AbstractSensor.ParseSensorIdToName(sensorID)].Data.Value;
      // return _memoryMappedSensorData.ReadAll().Value; //DEBUG
    }


    /// <summary>
    /// Sets the interval for the sensor with the sensorName
    /// </summary>
    /// <returns></returns>
    public void SetInterval(string sensorName, byte milliseconds) {
      Sensors[sensorName].SetInterval(milliseconds);
    }


    /// <summary>
    /// Connects the microcontroller at the specified serial port.
    /// </summary>
    /// <param name="portName">If null the first accessible port is used.</param>
    /// <param name="baudRate"></param>
    /// <param name="receivedBytesThreshold"></param>
    public void ConnectMicrocontroller(string? portName = null, int baudRate = SerialProtocol.BaudRate, int receivedBytesThreshold = SerialProtocol.ReceivedBytesThreshold) {
      portName ??= SerialPort.GetPortNames()[0];
      _serialMessaging.Begin(portName, baudRate, receivedBytesThreshold);
    }



    /// <summary>
    /// TODO: Sends sensor setup data to microcontroller and starts it's execution  (can this be combined with StartSetup() into one method?)
    /// DONE: Sends 2 types of setup messages over the serial port (ClearConfiguration, AddSensor for each sensor)
    /// </summary>
    public void FinishSetup() {
      _serialMessaging.WriteBytes(new SerialPackage(SerialProtocol.SetupCodes.ClearConfiguration).Serialize());

      foreach(AbstractSensor sensor in Sensors.Values) {
        _serialMessaging.WriteBytes(new SerialPackage(SerialProtocol.SetupCodes.AddSensor, sensor.GetDataAsBytes()).Serialize());
      }

    }


    /// <summary>
    /// TODO: Wakes the microcontrollers update loop until Stop() is called. |
    /// DONE: Sends WakeMicrocontroller over the serial port. |
    /// DEBUG: Starts a timer for 1000ms and calls ContinueLoop everytime it's finished |
    /// </summary>
    public void StartLoop() {
      _serialMessaging.WriteBytes(new SerialPackage(SerialProtocol.SystemCodes.WakeMicrocontroller).Serialize());
      // _timers.Add(new Timer(new TimerCallback(ContinueLoop), this, 0, Sensors[ArsemiGlobals.Sensors.Heartrate.ToString()].Data.IntervalMS));
      _serialMessaging.DataReceivedAction += ParseMessage;
    }


    /// <summary>
    /// DEBUG
    /// </summary>
    private void ContinueLoop(object? state) {
      // AbstractSensor currentSensor = Sensors[ArsemiGlobals.Sensors.Heartrate.ToString()];
      // currentSensor.Data.Value++;
      // currentSensor.RawBuffer.Push(new(_tick, currentSensor.Data.Value));
      // currentSensor.ApplyFilters();
      // currentSensor.FilteredBuffer.Push(new(_tick, currentSensor.Data.Value));

      // // Console.WriteLine(currentSensor.Data.Value);
      // currentSensor.CheckEventsConditions();
      // // StoreSensorData((uint)ArsemiGlobals.Sensors.Heartrate);

      // // currentSensor.RawBuffer.Push(new(_tick * currentSensor.Data.IntervalMS, _testValues[RingBuffer.PosMod((int)_tick, _testValues.Length)]));
      // _tick++;
      // File.AppendAllText("/home/mika/Downloads/filter.csv", $"{_tick}, {currentSensor.RawBuffer[0].Y}, {currentSensor.FilteredBuffer[0].Y}\n");
    }


    /// <summary>
    /// DisposesAsync() all sensor timers.
    /// TODO: Suspends the microcontrollers update loop until StartLoop() is called + threadpooling, wait for all
    /// </summary>
    public async Task StopLoop() {
      for(int i = 0; i < Sensors.Count; i++) {
        _ = _timers[i].DisposeAsync();
      }
      _serialMessaging.DataReceivedAction -= ParseMessage;
    }


    /// <summary>
    /// Converts a new message from string to package and then matches the action code to the required actions
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void ParseMessage() {
      while(_serialMessaging.AvailableBytes()) {

        if(_queuedActionCode == 0) {
          _queuedActionCode = _serialMessaging.ParsePackageStart();
        }

        if(_queuedActionCode == -1) {
          return;
        }

        switch(_queuedActionCode) {
        /// Codes meant for sending to the microcontroller -> no need to implement
        // case SerialProtocol.SystemCodes.HibernateMicrocontroller:
        //   throw new NotImplementedException();
        // case SerialProtocol.SystemCodes.WakeMicrocontroller:
        //   throw new NotImplementedException();
        // case SerialProtocol.SetupCodes.ClearConfiguration:
        //   throw new NotImplementedException();
        // case SerialProtocol.SetupCodes.AddSensor:
        //   throw new NotImplementedException();

        /// Codes meant for receiving from the microcontroller
        case SerialProtocol.SystemCodes.SystemError:
          ParseSystemError();
          break;
        case SerialProtocol.SystemCodes.RequestHandshake:
          throw new NotImplementedException();
        case SerialProtocol.SystemCodes.ReplyHandshake:
          throw new NotImplementedException();
        case SerialProtocol.SensorCodes.NewSample:
          ParseNewSample();
          break;
          // default:
          //   throw new NotImplementedException("The action code in the message can't be associated with a command.");
        }
      }
    }


    /// <summary>
    /// Parses sensorId, value and checksum from the next 3 bytes in the serial stream.
    /// </summary>
    private void ParseNewSample() {
      if(!_serialMessaging.AvailableBytes(3)) {
        return;
      }

      byte sensorId = _serialMessaging.ReadByte();
      byte value = _serialMessaging.ReadByte();
      byte checksum = _serialMessaging.ReadByte();

      byte computedChecksum = SerialMessaging.CRC8(SerialProtocol.SensorCodes.NewSample, sensorId, value);

      if(checksum != computedChecksum) {
        throw new Exception("HEY! Loss of packages... :c");
      }

      Console.Write("Received sensor data from sensorId: " + sensorId + " with a value of: " + value);
      Console.WriteLine(" | Sensorname: " + AbstractSensor.ParseSensorIdToName(sensorId));
      Console.WriteLine("---");

      Sensors[AbstractSensor.ParseSensorIdToName(sensorId)].Data.Value = value;
      _queuedActionCode = 0;
    }


    /// <summary>
    /// Matches the different sensor errors to their error messages.
    /// </summary>
    private void ParseSystemError() {
      if(!_serialMessaging.AvailableBytes(1)) {
        return;
      }

      byte errorCode = _serialMessaging.ReadByte();
      switch(errorCode) {
      default:
        Console.WriteLine("Received sensor error code: " + errorCode);
        break;
      }
      _queuedActionCode = 0;
    }
  }
}