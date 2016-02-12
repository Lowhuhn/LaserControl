using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserControl.Data
{
    public class Paths
    {
        public static string SettingsPath
        {
            get 
            {
                string p = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\LaserControl\"; 
                if (!Directory.Exists(p))
                    Directory.CreateDirectory(p);
                return p;  
            }
        }

        public static string SettingsConfigurationPath
        {
            get 
            { 
                string p = SettingsPath + @"Configuration\";
                if (!Directory.Exists(p))
                    Directory.CreateDirectory(p);
                return p;
            }
        }

        public static string SettingsScriptFunctionsPath
        {
            get 
            {
                string p = SettingsPath + @"Functions\"; 
                if (!Directory.Exists(p))
                    Directory.CreateDirectory(p);
                return p;                
            }
        }

    }
}
