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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LaserControl.Design.Custom
{
    /// <summary>
    /// Interaktionslogik für ImgMenuItem.xaml
    /// </summary>
    public partial class ImgMenuItem : MenuItem
    {
        public static readonly DependencyProperty ImgSourceProperty = DependencyProperty.Register(
            "Img",
            typeof(string),
            typeof(ImgMenuItem),
            new UIPropertyMetadata("NONE", null)
            );

        public string Img
        {
            get
            {
                return (string)GetValue(ImgSourceProperty);
            }
            set
            {
                SetValue(ImgSourceProperty, value);
            }
        }

        public ImgMenuItem()
        {
            InitializeComponent();
        }
    }
}
