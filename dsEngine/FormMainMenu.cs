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

            Dealer.DealerDirectory.ElementAt(0).GenerateInvoice(@"C:\Users\spruce\Desktop\", DateTime.Now);
            
        }
    }
}
