using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dsEngine
{
    internal partial class Settings
    {
        internal class Config
        {
            private string _logoFilename;

            public string LogoFilename 
            {
                get => _logoFilename;
                set 
                {
                    _logoFilename = (value ==  null) ? null : Settings.LOGO_DIR + value;
                } 
            }
        }
    }
}
