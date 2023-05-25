using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;


namespace dsEngine
{
    public partial class FormMainMenu : Form
    {
        public FormMainMenu()
        {
            InitializeComponent();
            
            Settings.LoadUserConfig();
            Dealer.LoadDealers();

            Settings.UserConfig.LogoFilename = "testlogo.png";
            Settings.SaveUserConfig();

            Invoice.Generate(Dealer.DealerDirectory.ElementAt(0), @"C:\Users\spruce\Desktop\test.pdf");
        }
    }
}
