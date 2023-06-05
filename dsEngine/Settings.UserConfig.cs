namespace dsEngine
{
    internal partial class Settings
    {
        internal class UserConfig
        {
            public string LogoPath { get; set; }

            public string[] CompanyInfo { get; set; } = new string[5];

            public System.Drawing.Color InvoiceAccentColor { get; set; } = System.Drawing.Color.FromArgb(0, 77, 153);
        }
    }
}
