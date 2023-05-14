using System.IO;

namespace dsEngine
{
    internal class Settings
    {
        public static string SETTINGS_PATH
        {
            get
            {
                string path = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar
                    + "data" + Path.DirectorySeparatorChar;

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                return path;
            }
        }

        public static string DEALER_SETTINGS_PATH
        {
            get
            {
                string path = SETTINGS_PATH + "dealers" + Path.DirectorySeparatorChar;

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                return path;
            }
        }
    }
}
