using Newtonsoft.Json;
using System.IO;

namespace dsEngine
{
    internal partial class Settings
    {
        public static string SETTINGS_DIR
        {
            get
            {
                return EnsurePathExists(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar
                    + "data" + Path.DirectorySeparatorChar);
            }
        }

        public static string DEALER_SETTINGS_DIR
        {
            get
            {
                return EnsurePathExists(SETTINGS_DIR + "dealers" + Path.DirectorySeparatorChar);
            }
        }

        public static UserConfig Config { get; private set; }

        private static string CONFIG_PATH
        {
            get
            {
                return SETTINGS_DIR + "config.json";
            }
        }

        public static void SaveUserConfig()
        {
            File.WriteAllText(CONFIG_PATH, JsonConvert.SerializeObject(Config, Formatting.Indented));
        }

        public static void LoadUserConfig()
        {
            if (File.Exists(CONFIG_PATH))
            {
                Config = JsonConvert.DeserializeObject<UserConfig>(File.ReadAllText(CONFIG_PATH));
            }

            else Config = new UserConfig();
        }

        private static string EnsurePathExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }
    }
}
