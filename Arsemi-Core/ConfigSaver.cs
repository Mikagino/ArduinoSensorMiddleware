using System.Text.Json;

namespace Arsemi {
    /// <summary>
    /// Provides static functionality for storing and loading the ArsemiCore to/from path
    /// </summary>
    public static class ConfigSaver {
        private static readonly JsonSerializerOptions _options = new() { WriteIndented = true };
        private const string ConfigFileName = "ArsemiConfig.json";
        private const string ConstantsFileName = "ArsemiConstants.cs";

        #region Constant file strings
        private const string ConstantFileHeader = @"
        namespace ExampleConstants {
            /// <summary>
            /// This file will be generated upon calling -INSERT METHOD- or when using the GUI for generating the configuration.
            /// </summary>
        ";
        private const string ConstantFileSensorEnumHeader = @"
            public enum Sensors {
        ";
        private const string ConstantFileEventClassHeader = @"
            public class Events {
        ";
        #endregion  Constant file strings


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
        /// <param name="asmCore"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static async Task GenerateConstants(ArsemiCore asmCore, string configDirectory) {
            string constantFileText = ConstantFileHeader;

            #region Sensor enum
            constantFileText += ConstantFileSensorEnumHeader;
            for(int i = 0; i > asmCore.Sensors.Count; i++) {
                constantFileText += asmCore.Sensors.Keys.ElementAt(i) + ",\n";
            }
            constantFileText += "}\n\n";
            #endregion Sensor enum

            #region Event actions
            constantFileText += ConstantFileEventClassHeader;
            #endregion Event actions
        }
    }
}