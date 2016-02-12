using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LaserCameraCSaC.TestUSW
{
    public partial class ConfigForm : Form
    {
        public ConfigForm()
        {
            InitializeComponent();
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                double d = trackBar1.Value * 0.01;
                this.textBox1.Text = string.Format("{0:0.00}", d);
                CWrapper.SetBWThreshold(d);
            }
            catch
            {
                // nix zu tun
            }
        }
    }
}
