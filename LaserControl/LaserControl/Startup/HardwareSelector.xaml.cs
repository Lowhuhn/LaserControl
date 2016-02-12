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

namespace LaserControl.Startup
{
    /// <summary>
    /// Interaktionslogik für HardwareSelector.xaml
    /// </summary>
    public partial class HardwareSelector : Window
    {
        protected Data.DataSafe MWDS;

        public HardwareSelector(Data.DataSafe mwDataSafe)
        {
            InitializeComponent();
            MWDS = mwDataSafe;
        }

        private void OKBtn_Click(object sender, RoutedEventArgs e)
        {            
            switch (HWSelection.SelectedIndex)
            {
                case 0: //Api
                    MWDS.Ints["SelectedHardware"] = 0;
                    this.DialogResult = true;
                    this.Close();
                    break;
                case 1: // Aerotech
                    MWDS.Ints["SelectedHardware"] = 1;
                    this.DialogResult = true;
                    this.Close();
                    break;
                default :
                    MessageBox.Show("You have to select the hardware you are using.\nOr you can exit the application with a click on cancel.", 
                        "Hardware selection", 
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);                    
                    return;
            }            
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
