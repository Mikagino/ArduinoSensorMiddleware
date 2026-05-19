using System.ComponentModel;
using System.IO.Ports;
using System.Text.Json.Serialization;
using System.Threading.Tasks.Dataflow;
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
    [JsonInclude] public AbstractSensor[] Sensors;
    private uint sensorCount = 0;

    // IPC
    private readonly MemoryMappedSensorData _memoryMappedSensorData = new("SensorData", 1024);
    private SerialMessaging _serialMessaging;


    /// <summary>
    /// Invoked when new data is received with the sensorId as parameter
    /// </summary>
    public Action<int, int>? NewDataReceived;
    public Action<string>? NewMessageReceived;


    private int _queuedActionCode = 0;


    /// <summary>
    /// Construct a new ArsemiCore and initialize arrays according to parameters
    /// </summary>
    /// <param name="maximumSensorCount"></param>
    public ArsemiCore(int maximumSensorCount = 8) {
      Sensors = new AbstractSensor[maximumSensorCount];
      _serialMessaging = new();
    }


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
    private void StoreSensorData(int sensorId) {
      AbstractSensor currentSensor = Sensors[sensorId];
      currentSensor.ApplyFilters();
      _memoryMappedSensorData.Write(currentSensor.Data);
    }


    /// <summary>
    /// TODO: Setup new sensors and store them for later use, call Finish() when the initial sensor setup is done
    /// </summary>
    /// <returns>New sensor for using it in a stacked setup</returns>
    public AbstractSensor AddSensor(AbstractSensor sensor) {
      if(sensor == null) throw new Exception("No sensor has been supplied to the function call!");
      sensor.Data.ID = (byte)(sensorCount);
      Sensors[sensorCount] = sensor;
      Console.WriteLine("Added " + sensor.Data.Name + " sensor to index: " + sensorCount);
      sensorCount++;
      return sensor;
    }


    /// <summary>
    /// </summary>
    /// <returns>TODO: Current sensor value based on filters, timings, etc.</returns>
    public float GetSensorValue(int sensorId) {
      AbstractSensor sensor = Sensors[sensorId - 1];
      if(sensor == null) throw new Exception("Sensor with id: " + sensorId + " is null, did you use the ArsemiGlobals or manually picked the index? Preferably use the ArsemiGlobals.");
      return sensor.Data.Value;
    }


    /// <summary>
    /// Sets the interval for the sensor with the sensorName
    /// </summary>
    /// <returns></returns>
    public void SetInterval(int sensorId, byte milliseconds) {
      Sensors[sensorId].SetInterval(milliseconds);
    }


    /// <summary>
    /// Connects the microcontroller at the specified serial port.
    /// </summary>
    /// <param name="portName">If null the first accessible port is used.</param>
    /// <param name="baudRate"></param>
    /// <param name="receivedBytesThreshold"></param>
    public void ConnectMicrocontroller(string? portName = null, int baudRate = SerialProtocol.BaudRate, int receivedBytesThreshold = SerialProtocol.ReceivedBytesThreshold) {
      if(portName == null) {
        string[] portNames = SerialPort.GetPortNames();
        if(portNames.Length == 0) throw new Exception("No microcontroller could be found automatically. Is it connected?");
        portName = portNames[0];
      }
      _serialMessaging.Begin(portName, baudRate, receivedBytesThreshold);
      _serialMessaging.DataReceivedAction += ParseMessage; // DEBUG -> later move to start loop
    }



    /// <summary>
    /// TODO: Sends sensor setup data to microcontroller and starts it's execution  (can this be combined with StartSetup() into one method?)
    /// DONE: Sends 2 types of setup messages over the serial port (ClearConfiguration, AddSensor for each sensor)
    /// </summary>
    public void FinishSetup() {
      _serialMessaging.WriteBytes(new SerialPackage(SerialProtocol.SetupCodes.ClearConfiguration).Serialize());

      for(int i = 0; i < sensorCount; i++) {
        if(Sensors[i] == null) throw new Exception("Sensor has been deleted somehow...");
        byte[] addSensorMessage = new SerialPackage(SerialProtocol.SetupCodes.AddSensor, Sensors[i].GetDataAsBytes()).Serialize();
        _serialMessaging.WriteBytes(addSensorMessage);
      }

    }


    /// <summary>
    /// TODO: Wakes the microcontrollers update loop until Stop() is called. |
    /// DONE: Sends WakeMicrocontroller over the serial port. |
    /// DEBUG: Starts a timer for 1000ms and calls ContinueLoop everytime it's finished |
    /// </summary>
    public void StartLoop() {
      _serialMessaging.WriteBytes(SerialProtocol.SystemCodes.WakeMicrocontroller);
      // _timers.Add(new Timer(new TimerCallback(ContinueLoop), this, 0, Sensors[ArsemiGlobals.Sensors.Heartrate.ToString()].Data.IntervalMS));
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
    /// TODO: Suspends the microcontrollers update loop until StartLoop() is called
    /// </summary>
    public async Task StopLoop() {
      _serialMessaging.DataReceivedAction -= ParseMessage;
    }


    /// <summary>
    /// Converts a new message from string to package and then matches the action code to the required actions
    /// </summary>
    public void ParseMessage() {
      Console.WriteLine("new message");
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
          Console.WriteLine("ERROR");
          ParseSystemError();
          break;
        case SerialProtocol.SystemCodes.RequestHandshake:
          throw new NotImplementedException();
        case SerialProtocol.SystemCodes.ReplyHandshake:
          throw new NotImplementedException();
        case SerialProtocol.SensorCodes.NewSample:
          Console.WriteLine("New sample");
          ParseNewSample();
          break;
        // case SerialProtocol.SetupCodes.SuccessfullyAddedSensor:
        default:
          Console.WriteLine("Queued action code: " + _queuedActionCode);
          _queuedActionCode = 0;
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
      Console.WriteLine(" | Sensorname: " + Sensors[sensorId].Data.Name);
      Console.WriteLine("---");

      Sensors[sensorId].Data.Value = value;
      NewDataReceived?.Invoke(sensorId, value);
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