using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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

using System.ServiceModel;
using System.Threading;

namespace LaserCameraCSaC.TestUSW
{
    /// <summary>
    /// Interaktionslogik für FinalWindow.xaml
    /// </summary>
    public partial class FinalWindow : Window
    {
        //public static Bitmap Current = new Bitmap(1,1);
        protected int fps = 0;

        public FinalWindow()
        {
            InitializeComponent();
        }

        public void NewImage(Bitmap bmp)
        {
            
            lock(Service.CameraService.Current){
                Service.CameraService.Current.Dispose();
                Service.CameraService.Current = bmp.Clone() as Bitmap;
            }            
            this.DispImage.Dispatcher.Invoke(new Action(() =>
            {                
                DispImage.Source = BitmapToImageSource(bmp);
            }));
        }

        #region Menu Functions 

        private void UncheckAll()
        {
            imagetype_a.IsChecked = false;
            imagetype_b.IsChecked = false;
            imagetype_c.IsChecked = false;
            imagetype_d.IsChecked = false;
            imagetype_e.IsChecked = false;
            imagetype_f.IsChecked = false;
        }

        private void imagetype_a_Checked(object sender, RoutedEventArgs e)
        {
            UncheckAll();
            CWrapper.SetDispImage(0);
            imagetype_a.IsChecked = true;
        }

        private void imagetype_b_Checked(object sender, RoutedEventArgs e)
        {
            UncheckAll();
            CWrapper.SetDispImage(1);
            imagetype_b.IsChecked = true;
        }

        private void imagetype_c_Checked(object sender, RoutedEventArgs e)
        {
            UncheckAll();
            CWrapper.SetDispImage(2);
            imagetype_c.IsChecked = true;
        }

        private void imagetype_d_Checked(object sender, RoutedEventArgs e)
        {
            UncheckAll();
            CWrapper.SetDispImage(3);
            imagetype_d.IsChecked = true;
        }

        private void imagetype_e_Checked(object sender, RoutedEventArgs e)
        {
            UncheckAll();
            CWrapper.SetDispImage(4);
            imagetype_e.IsChecked = true;
        }

        private void imagetype_f_Checked(object sender, RoutedEventArgs e)
        {
            UncheckAll();
            CWrapper.SetDispImage(5);
            imagetype_f.IsChecked = true;
        }

        private void CB_1_Click(object sender, RoutedEventArgs e)
        {
            CWrapper.SetOverlayer(1, CB_1.IsChecked == true ? 1 : 0);
        }

        private void CB_0_Click(object sender, RoutedEventArgs e)
        {
            CWrapper.SetOverlayer(0, CB_0.IsChecked == true ? 1 : 0);
        }

        private void CB_2_Click(object sender, RoutedEventArgs e)
        {
            CWrapper.SetOverlayer(2, CB_2.IsChecked == true ? 1 : 0);
        }

        private void InvThreshold_Click(object sender, RoutedEventArgs e)
        {
            CWrapper.SetOverlayer(3, this.InvThreshold.IsChecked == true ? 1 : 0);
        }

        private void ThresSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int val = (int)ThresSlider.Value;
            this.SliderValue.Header = string.Format("Threshold: {0}",val);
            CWrapper.SetOverlayer(4, val);           
        }


        #endregion //Menu Functions

        #region Static Functions
        private static ImageSource BitmapToImageSource(System.Drawing.Bitmap bmp)
        {
            BitmapImage bitImg = new BitmapImage();
            using (var ms = new MemoryStream())
            {
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                bitImg.BeginInit();
                bitImg.StreamSource = new MemoryStream(ms.ToArray());
                bitImg.EndInit();
            }
            //bmp.Dispose();
            return bitImg;
        }
        #endregion //Static Functions

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void pt_processed_Click(object sender, RoutedEventArgs e)
        {
            pt_camera.IsChecked = false;
            pt_processed.IsChecked = true;
            CWrapper.SetDoProcess(1);
        }

        private void pt_camera_Click(object sender, RoutedEventArgs e)
        {
            pt_camera.IsChecked = true;
            pt_processed.IsChecked = false;
            CWrapper.SetDoProcess(0);
        }
    }
}
