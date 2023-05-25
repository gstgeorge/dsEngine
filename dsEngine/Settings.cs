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

        public static Config UserConfig { get; private set; } = new Config();

        private static string LOGO_DIR
        {
            get
            {
                return EnsurePathExists(SETTINGS_DIR + "logo" + Path.DirectorySeparatorChar);
            }
        }

        public static void SaveUserConfig()
        {
            File.WriteAllText("config.json", JsonConvert.SerializeObject(UserConfig));
        }

        public static void LoadUserConfig()
        {
            if (File.Exists(SETTINGS_DIR + "config.json"))
            {
                UserConfig = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));
            }
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
