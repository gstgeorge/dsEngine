namespace dsEngine
{
    internal partial class Settings
    {
        internal class UserConfig
        {
            public string LogoFilename { get; set; } // TODO: remove later

            public string LogoPath 
            { 
                get 
                { 
                    if (string.IsNullOrEmpty(LogoFilename))
                    {
                        return null;
                    }

                    else return Settings.LOGO_DIR + LogoFilename; 
                } 
            }

            public string[] CompanyInfo { get; set; } = new string[5];

            public System.Drawing.Color InvoiceAccentColor { get; set; } = System.Drawing.Color.FromArgb(0, 77, 153);
        }
    }
}
