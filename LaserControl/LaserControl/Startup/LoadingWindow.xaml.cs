using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using LaserControl.Data;
using System.Threading;

using LaserControl.Library;
using LaserControl.ScriptV2;

namespace LaserControl.Startup
{   
    /// <summary>
    /// Interaktionslogik für LoadingWindow.xaml
    /// </summary>
    public partial class LoadingWindow : Window
    {
        protected DataSafe MWDS;

        public HardwareAPI.HardwareController HWC = null;



        public LoadingWindow( DataSafe ds)
        {
            InitializeComponent();
            MWDS = ds;
        }

        public void Load()
        {
            TrackedThread tt = new TrackedThread("LoadingWindow Loading Thread", LoadingThread);
            tt.Start();
            this.ShowDialog();
        }

        public void LoadingThread()
        {
            Thread.Sleep(1000);
            //Zuerst alles Laden, danach das geladene wieder Speichern. (Zur Sichertheit)
            int which = MWDS.Ints["SelectedHardware",0];
            if (which == 0)
            {
                HWC = new HardwareAPI.HardwareController();
            }
            else
            {
                try
                {
                    HWC = new AerotechHardware.AerotechHardwareController();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message+"\n\nPlease restart your PC, check the service or select the correct HardwareController with a restart of LaserControl!", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    MWDS.Ints["SelectedHardware"] = -1;
                    Environment.Exit(0);
                }
            }
            HWC.Save();


            Thread.Sleep(1000);
            FunctionLib.Load();

            this.Dispatcher.Invoke(new Action(() =>
            {
                this.Close();
            }));            
        }

    }
}
