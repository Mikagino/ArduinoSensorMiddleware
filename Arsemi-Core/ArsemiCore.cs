using System.Reflection;
using System.Text.Json.Serialization;
using Arsemi.IPC;
using Arsemi.Sensor;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

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
    private MessageParsing _messageParsing;

    /// <summary>
    /// Invoked when new data is received with the sensorId as parameter
    /// </summary>
    public Action<int, int>? NewDataReceived;
    public Action<string>? NewMessageReceived;


    /// <summary>
    /// Construct a new ArsemiCore and initialize arrays according to parameters
    /// </summary>
    /// <param name="maximumSensorCount"></param>
    public ArsemiCore(int maximumSensorCount = 8) {
      Sensors = new AbstractSensor[maximumSensorCount];
      _messageParsing = new(this);
    }



    /// <summary>
    /// TODO: Loops when program is running (in Program.cs?)
    /// </summary>
    public void Update() {
    }

    [Benchmark]
    [Arguments(null, 2000, 5000)]
    public async Task<MessageParsing.ConnectionResult> ConnectMicrocontrollerAsync(SerialPortInfo? serialPortInfo = null, int drtResetWaitMs = 2000, int timeoutMs = 5000) {
      return await _messageParsing.ConnectMicrocontrollerAsync(serialPortInfo, drtResetWaitMs, timeoutMs);
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
    /// TODO: Sends sensor setup data to microcontroller and starts it's execution  (can this be combined with StartSetup() into one method?)
    /// DONE: Sends 2 types of setup messages over the serial port (ClearConfiguration, AddSensor for each sensor)
    /// </summary>
    public void FinishSetup() {
      SerialMessaging.Write(SerialProtocol.Action.Setup.ClearConfiguration);

      for(int i = 0; i < sensorCount; i++) {
        if(Sensors[i] == null) throw new Exception("Sensor has been deleted somehow...");
        byte[] addSensorMessage = new SerialPackage(SerialProtocol.Action.Setup.AddSensor, Sensors[i].ParseDataToByteArray()).Serialize();
        SerialMessaging.Write(addSensorMessage);
      }

    }


    /// <summary>
    /// TODO: Wakes the microcontrollers update loop until Stop() is called. |
    /// DONE: Sends WakeMicrocontroller over the serial port which then starts the execution.
    /// </summary>
    public void StartLoop() {
      SerialMessaging.Write(SerialProtocol.Action.System.WakeMicrocontroller);
    }


    /// <summary>
    /// TODO: Suspends the microcontrollers update loop until StartLoop() is called
    /// </summary>
    public async Task StopLoop() {
      //SerialMessaging.DataReceivedAction -= ParseMessage;
    }



    public void DebugClass() {
      BenchmarkSwitcher.FromAssembly(Assembly.GetExecutingAssembly()).Run(null);

    }
  }
}