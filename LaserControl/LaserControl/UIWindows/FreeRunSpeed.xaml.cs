using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LaserControl.UIWindows
{
    /// <summary>
    /// Interaktionslogik für FreeRunSpeed.xaml
    /// </summary>
    public partial class FreeRunSpeed : Window
    {
        public string SpeedStr = "";
        public string DistStr = "";
        public bool UseDistVal = false;


        public FreeRunSpeed(string speed, string dist, bool useDist)
        {
            InitializeComponent();
            this.SpeedTxBox.Text = speed;
            this.DistTxBox.Text = dist;
            this.UseDist.IsChecked = useDist;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SpeedStr = this.SpeedTxBox.Text;
            DistStr = this.DistTxBox.Text;
            UseDistVal = this.UseDist.IsChecked == true;

            this.DialogResult = true;
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
