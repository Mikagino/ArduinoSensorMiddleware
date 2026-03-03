using Arsemi.IPC;
using Arsemi.Sensor;
using Microsoft.VisualBasic;

namespace Arsemi {
  /// <summary>
  /// Flexible API for microcontroller based sensor networks that are supplied to the user in an easy accessible manner.
  /// Multiple sensors can be setup, those settings can be stored and loaded.
  /// Filters can be applied to each sensor and much more...
  /// For more information: TODO<Insert Link>
  /// </summary>
  public class ArsemiCore {
    private Dictionary<string, AbstractSensor> _sensors = [];

    // IPC
    private readonly MemoryMappedSensorData _memoryMappedSensorData = new();
    //private readonly SerialMessaging _serialMessaging = new("COM3", 9600);

    private readonly AnalogSensor _exampleSensor = new();


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
    private void StoreSensorData() {
      _memoryMappedSensorData.Write(_exampleSensor.Data);
    }


    /// <summary>
    /// TODO: Setup new sensors and store them for later use, call Finish() when the initial sensor setup is done
    /// </summary>
    /// <returns>New sensor for using it in a stacked setup</returns>
    public AbstractSensor AddSensor(AbstractSensor sensor, string name) {
      _sensors.Add(name, sensor);
      return sensor;
    }


    /// <summary>
    /// </summary>
    /// <returns>TODO: Current sensor value based on filters, timings, etc.</returns>
    public VariantType GetSensorValueID(uint sensorID) {
      return 0;
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
    /// TODO: Wakes the microcontrollers update loop until Stop() is called
    /// </summary>
    public void StartLoop() {
    }


    /// <summary>
    /// TODO: Suspends the microcontrollers update loop until FinishSetup() or Start() is called
    /// </summary>
    public void StopLoop() {
    }
  }
}