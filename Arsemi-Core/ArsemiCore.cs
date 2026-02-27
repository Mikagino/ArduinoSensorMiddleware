using System.IO.Ports;
using Arsemi.Sensor;
using Microsoft.VisualBasic;

namespace Arsemi {
  public class ArsemiCore {
    private Dictionary<string, AbstractSensor> _sensors = [];
    private static readonly SerialPort _serialPort = new("COM3", 9600);

    private readonly MemoryMappedSensorData _memoryMappedSensorData = new();


    /// <summary>
    /// TODO: Setup communication with arduino an other important things :> 
    /// </summary>
    public void StartSetup() {
      _serialPort.ReceivedBytesThreshold = 5;
      _serialPort.DataReceived += ReadLineSerial;
      _serialPort.Open();
    }


    /// <summary>
    /// TODO: Loops when program is running (in Program.cs?)
    /// </summary>
    public void Update() {
    }


    /// <summary>
    /// TODO: Reads a line from the USB-Port for reading data send from arduino
    /// </summary>
    private void ReadLineSerial(object _, SerialDataReceivedEventArgs e) {
      string arduinoMessage = _serialPort.ReadLine();
      Console.WriteLine(arduinoMessage);
    }

    public struct SensorFrame {
      public long Timestamp;
      public float HeartRate;
      public float Gsr;
    }


    private AnalogSensor _exampleSensor = new();

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
    /// </summary>
    /// <returns>TODO: Current sensor value based on filters, timings, etc.</returns>
    public float GetSensorValue(string sensorName) {
      return 0;
    }


    /// <summary>
    /// Sets the interval for the sensor with the sensorName
    /// </summary>
    /// <returns></returns>
    public void SetInterval(string sensorName, uint milliseconds) {

    }


    /// <summary>
    /// TODO: Sends sensor setup data to microcontroller and starts it's execution
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