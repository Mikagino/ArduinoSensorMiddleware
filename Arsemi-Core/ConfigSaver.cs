using System.Text.Json;
using Arsemi.Sensor;

namespace Arsemi {
    /// <summary>
    /// Provides static functionality for storing and loading the ArsemiCore to/from path
    /// </summary>
    public static class ConfigSaver {
        private static readonly JsonSerializerOptions _options = new() { WriteIndented = true };
        private const string ConfigFileName = "ArsemiConfig.json";
        private const string GlobalsFileName = "ArsemiGlobals.cs";

        #region Constant file strings
        private const string GlobalsFileHeader = @"
/// <summary>
/// This file will be generated upon calling GenerateConstants() or when using the GUI for generating the configuration.
/// </summary>
using System;
using System.Collections.Generic;

namespace ArsemiGlobals {
";
        private const string GlobalsFileSensorEnumHeader = @"
    public enum SensorNames {
";
        private const string GlobalsFileEventClassHeader = @"
    public class Events {";
        private const string GlobalsFileEvent = @"
        public static Action? ";
        private const string GlobalsFileEventMapHeader = @"
        public static Dictionary<string, Func<Action?>> EventMap = new(
            [
                ";
        #endregion Constant file strings


        /// <summary>
        /// Stores the settings from the ArsemiCore asynchronously in JSON-Format to path if the new config is different than the stored
        /// </summary>
        public static async Task<bool> SaveTo(ArsemiCore arsemiCore, string configDirectory) {
            string configFile = Path.Combine(configDirectory, ConfigFileName);
            string jsonString = JsonSerializer.Serialize(arsemiCore, _options);
            if(!Directory.Exists(configDirectory)) {
                Directory.CreateDirectory(configDirectory);
            }
            if(File.Exists(configFile)) {
                string storedConfig = await File.ReadAllTextAsync(configFile);
                if(jsonString == storedConfig) {
                    return true;
                }
            }
            await File.WriteAllTextAsync(configFile, jsonString);
            return true;
        }


        /// <summary>
        /// </summary>
        /// <param name="configDirectory"></param>
        /// <returns>A new ArsemiCore based on the config in the file at path</returns>
        public static async Task<ArsemiCore> LoadConfigAsync(string configDirectory) {
            string configFile = Path.Combine(configDirectory, ConfigFileName);
            if(!File.Exists(configFile)) {
                throw new FileNotFoundException("There is no file at the path: " + configFile);
            }

            string jsonString = await File.ReadAllTextAsync(configFile);

            ArsemiCore? arsemiCoreFromConfig = JsonSerializer.Deserialize<ArsemiCore>(jsonString);
            if(arsemiCoreFromConfig == null) {
                throw new FileLoadException("Couldn't load config from path: " + configFile);
            }

            return arsemiCoreFromConfig;
        }


        /// <summary>
        /// Generates C# file containing enum with the sensor names and action delegates
        /// </summary>
        /// <param name="arsemiCore"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static async Task GenerateConstants(ArsemiCore arsemiCore, string configDirectory) {
            string constantFileText = GlobalsFileHeader;

            #region Sensor enum
            constantFileText += GlobalsFileSensorEnumHeader;
            foreach(string sensorName in arsemiCore.Sensors.Keys) {
                constantFileText += "\t\t" + sensorName + ",\n";
            }

            constantFileText += "\t}\n\n";
            #endregion Sensor enum

            #region Event actions
            constantFileText += GlobalsFileEventClassHeader;
            string allEvents = "";
            string eventMap = GlobalsFileEventMapHeader;
            foreach(AbstractSensor sensor in arsemiCore.Sensors.Values) {
                foreach(string eventName in sensor.Events.Keys) {
                    allEvents += GlobalsFileEvent + eventName + ";\n";
                    eventMap += "new(\"" + eventName + "\", " + "() => " + eventName + ",\n";
                }
            }
            eventMap += "\t\t\t]\n\t\t);";

            constantFileText += allEvents;
            constantFileText += eventMap + "\n";
            constantFileText += "\t}\n";
            constantFileText += "}";
            #endregion Event actions

            string filePath = Path.Combine(configDirectory + GlobalsFileName);
            File.WriteAllLines(filePath, [constantFileText]);
        }
    }
}