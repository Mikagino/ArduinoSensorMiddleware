using System.IO.Ports;
using System.Text.Json.Serialization;
using Arsemi.IPC;
using Arsemi.Sensor;

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
    // private void StoreSensorData(uint sensorId) {
    //   AbstractSensor currentSensor = Sensors[AbstractSensor.ParseSensorIdToName(sensorId)];
    //   currentSensor.ApplyFilters();
    //   _memoryMappedSensorData.Write(currentSensor.Data);
    // }


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
      return _memoryMappedSensorData.ReadAll().Value; //DEBUG
    }


    /// <summary>
    /// Sets the interval for the sensor with the sensorName
    /// </summary>
    /// <returns></returns>
    public void SetInterval(string sensorName, uint milliseconds) {
      Sensors[sensorName].SetInterval(milliseconds);
    }


    /// <summary>
    /// Connects the microcontroller at the specified serial port.
    /// </summary>
    /// <param name="portName"></param>
    /// <param name="baudRate"></param>
    /// <param name="receivedBytesThreshold"></param>
    public void ConnectMicrocontroller(string portName, int baudRate = SerialProtocol.BaudRate, int receivedBytesThreshold = 5) {
      _serialMessaging.Begin(portName, baudRate, receivedBytesThreshold);
    }


    /// <summary>
    /// TODO: Sends sensor setup data to microcontroller and starts it's execution  (can this be combined with StartSetup() into one method?)
    /// DONE: Sends 2 types of setup messages over the serial port (ClearConfiguration, AddSensor for each sensor)
    /// </summary>
    public void FinishSetup() {
      _serialMessaging.WriteLine(SerialProtocol.CombineToMessage(0, SerialProtocol.SetupCodes.ClearConfiguration));

      foreach(AbstractSensor sensor in Sensors.Values) {
        _serialMessaging.WriteLine(
          SerialProtocol.CombineToMessage(0, SerialProtocol.SetupCodes.AddSensor, sensor.GetDataAsStrings()));
      }

    }


    /// <summary>
    /// TODO: Wakes the microcontrollers update loop until Stop() is called. |
    /// DONE: Sends WakeMicrocontroller over the serial port. |
    /// DEBUG: Starts a timer for 1000ms and calls ContinueLoop everytime it's finished |
    /// </summary>
    public void StartLoop() {
      _serialMessaging.WriteLine(SerialProtocol.CombineToMessage(0, SerialProtocol.SystemCodes.WakeMicrocontroller));
      // _timers.Add(new Timer(new TimerCallback(ContinueLoop), this, 0, Sensors[ArsemiGlobals.Sensors.Heartrate.ToString()].Data.IntervalMS));
      _serialMessaging.DataReceived += ParseMessage;
    }


    /// <summary>
    /// DEBUG
    /// </summary>
    private void ContinueLoop(object? state) {
      AbstractSensor currentSensor = Sensors[ArsemiGlobals.Sensors.Heartrate.ToString()];
      currentSensor.Data.Value++;
      currentSensor.RawBuffer.Push(new(_tick, currentSensor.Data.Value));
      currentSensor.ApplyFilters();
      currentSensor.FilteredBuffer.Push(new(_tick, currentSensor.Data.Value));

      // Console.WriteLine(currentSensor.Data.Value);
      currentSensor.CheckEventsConditions();
      // StoreSensorData((uint)ArsemiGlobals.Sensors.Heartrate);

      // currentSensor.RawBuffer.Push(new(_tick * currentSensor.Data.IntervalMS, _testValues[RingBuffer.PosMod((int)_tick, _testValues.Length)]));
      _tick++;
      File.AppendAllText("/home/mika/Downloads/filter.csv", $"{_tick}, {currentSensor.RawBuffer[0].Y}, {currentSensor.FilteredBuffer[0].Y}\n");
    }


    /// <summary>
    /// DisposesAsync() all sensor timers.
    /// TODO: Suspends the microcontrollers update loop until StartLoop() is called + threadpooling, wait for all
    /// </summary>
    public async Task StopLoop() {
      for(int i = 0; i < Sensors.Count; i++) {
        _ = _timers[i].DisposeAsync();
      }
    }


    /// <summary>
    /// Converts a new message from string to package and then matches the action code to the required actions
    /// </summary>
    /// <param name="_"></param>
    /// <param name="e"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void ParseMessage(object _, SerialDataReceivedEventArgs e) {
      string message = _serialMessaging.ReadLine();
      // Console.WriteLine("Received message: " + message);
      SerialProtocol.Package package = SerialProtocol.Split(message);
      switch(package.ActionCode) {
      // Codes meant for sending to the microcontroller -> no need to implement
      // case SerialProtocol.SystemCodes.HibernateMicrocontroller:
      //   throw new NotImplementedException();
      // case SerialProtocol.SystemCodes.WakeMicrocontroller:
      //   throw new NotImplementedException();
      // case SerialProtocol.SetupCodes.ClearConfiguration:
      //   throw new NotImplementedException();
      // case SerialProtocol.SetupCodes.AddSensor:
      //   throw new NotImplementedException();

      // Codes meant for receiving from the microcontroller
      case SerialProtocol.SystemCodes.SystemError:
        ParseSystemError(package);
        break;
      case SerialProtocol.SystemCodes.RequestHandshake:
        throw new NotImplementedException();
      case SerialProtocol.SystemCodes.ReplyHandshake:
        throw new NotImplementedException();
      case SerialProtocol.SensorCodes.NewSample:
        ParseNewSample(package);
        break;
      }
    }


    /// <summary>
    /// Parses sensorId and value from the packages parameters
    /// </summary>
    /// <param name="package"></param>
    /// <exception cref="Exception"></exception>
    private void ParseNewSample(SerialProtocol.Package package) {
      if(package.Parameters.Length < 2) {
        throw new Exception("Package doesn't contain all parameters, message may be corrupt: " + package);
      }
      if(!uint.TryParse(package.Parameters[0], out uint sensorId)) {
        throw new Exception("Couldn't read sensor id, message may be corrupt: " + package);
      }
      if(!uint.TryParse(package.Parameters[1], out uint value)) {
        throw new Exception("Couldn't read sensor value, message may be corrupt: " + package);
      }

      Console.Write("Received sensor data from sensorId: " + sensorId + " with a value of: " + value);
      Console.WriteLine(" | Sensorname: " + AbstractSensor.ParseSensorIdToName(sensorId));

      Sensors[AbstractSensor.ParseSensorIdToName(sensorId)].Data.Value = value;
      // StoreSensorData(sensorId);
    }


    /// <summary>
    /// Parses the different sensor errors to their error messages
    /// </summary>
    /// <param name="package"></param>
    private void ParseSystemError(SerialProtocol.Package package) {
      switch(package.Parameters[0]) {
      default:
        Console.WriteLine("Received sensor error code: " + package.Parameters[0]);
        break;
      }
    }
  }
}