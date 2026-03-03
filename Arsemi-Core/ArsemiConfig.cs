
namespace Arsemi {
    public static class ArsemiConfig {

        public enum ConfigType {
            INI,
            JSON,
        }


        /// <summary>
        /// TODO: Stores the settings from the ArsemiCore
        /// </summary>
        public static void SaveTo(ArsemiCore arsemiCore, string pathToConfigFile, ConfigType configurationType) {
        }


        /// <summary>
        /// TODO: Loads the complete configuration from a JSON file into a new ArsemiCore object. Detects ConfigurationType automaticly
        /// </summary>
        public static ArsemiCore LoadConfiguration(string pathToConfigFile) {
            return new();
        }
    }
}