using System.Text.Json;

namespace Arsemi {
    /// <summary>
    /// Provides static functionality for storing and loading the ArsemiCore to/from path
    /// </summary>
    public static class ConfigSaver {
        private static readonly JsonSerializerOptions _options = new() { WriteIndented = true };


        /// <summary>
        /// Stores the settings from the ArsemiCore asynchronously in JSON-Format to path if the new config is different than the stored
        /// </summary>
        public static async Task<bool> SaveTo(ArsemiCore arsemiCore, string configPath) {
            string jsonString = JsonSerializer.Serialize(arsemiCore, _options);
            if(File.Exists(configPath)) {
                string storedConfig = await File.ReadAllTextAsync(configPath);
                if(jsonString == storedConfig) {
                    return true;
                }
            }
            await File.WriteAllTextAsync(configPath, jsonString);
            return true;
        }


        /// <summary>
        /// </summary>
        /// <param name="configPath"></param>
        /// <returns>A new ArsemiCore based on the config in the file at path</returns>
        public static async Task<ArsemiCore> LoadConfiguration(string configPath) {
            if(!File.Exists(configPath)) {
                throw new FileNotFoundException("There is no file at the path: " + configPath);
            }

            string jsonString = await File.ReadAllTextAsync(configPath);

            ArsemiCore? arsemiCoreFromConfig = JsonSerializer.Deserialize<ArsemiCore>(jsonString);
            if(arsemiCoreFromConfig == null) {
                throw new FileLoadException("Couldn't load config from path: " + configPath);
            }

            return arsemiCoreFromConfig;
        }
    }
}