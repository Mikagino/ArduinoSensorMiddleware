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
    //private readonly SerialMessaging _serialMessaging = new("COM3", 9600);


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


    /// <summary>
    /// TODO: Stores sensor values received from the microcontroller in shared memory | 
    /// NICE TO HAVE: Make timed for each sensor individually -> for performance setting / more control
    /// </summary>
    private void StoreSensorData(uint sensor) {
      _memoryMappedSensorData.Write(Sensors[((Examples.ExampleConstants.Sensors)sensor).ToString()].Data);
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
      return _memoryMappedSensorData.ReadAll().Value; //DEBUG
    }


    /// <summary>
    /// Sets the interval for the sensor with the sensorName
    /// </summary>
    /// <returns></returns>
    public void SetInterval(string sensorName, uint milliseconds) {

    }


    /// <summary>
    /// TODO: Sends sensor setup data to microcontroller and starts it's execution  (can this be combined with StartSetup() into one method?)
    /// </summary>
    public void FinishSetup() {

    }


    /// <summary>
    /// TODO: Wakes the microcontrollers update loop until Stop() is called.
    /// Starts a timer for 1000ms and calls ContinueLoop when it's finished
    /// </summary>
    public void StartLoop() {
      _ = new Timer(new TimerCallback(ContinueLoop), this, 0, 1000);
    }


    /// <summary>
    /// DEBUG
    /// </summary>
    private void ContinueLoop(object? state) {
      Sensors[Examples.ExampleConstants.Sensors.Heartrate.ToString()].Data.Value++;
      StoreSensorData((uint)Examples.ExampleConstants.Sensors.Heartrate);
    }


    /// <summary>
    /// TODO: Suspends the microcontrollers update loop until FinishSetup() or Start() is called
    /// </summary>
    public void StopLoop() {
    }

    public string JsonSerializer() {
      return System.Text.Json.JsonSerializer.Serialize(this);
    }
  }
}