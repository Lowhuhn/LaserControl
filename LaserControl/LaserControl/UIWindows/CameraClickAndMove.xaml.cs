using LaserControl.HardwareAPI;
using LaserControl.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

using System.Threading;
using System.IO;
using LaserControl.Data;
using LaserControl.ScriptV2;

namespace LaserControl.UIWindows
{

    public class CameraClickAndMoveDataHandler : INotifyPropertyChanged
    {

        protected Visibility _ImageVisible;
        public Visibility ImageVisible
        {
            get { return _ImageVisible; }
            set
            {
                _ImageVisible = value;
                this.NotifyPropertyChanged("ImageVisible");
            }
        }

        protected Visibility _ButtonsVisible;
        public Visibility ButtonsVisible
        {
            get { return _ButtonsVisible; }
            set
            {
                _ButtonsVisible = value;
                this.NotifyPropertyChanged("ButtonsVisible");
            }
        }

        protected Visibility _ProgbarVisible;
        public Visibility ProgbarVisible
        {
            get { return _ProgbarVisible; }
            set
            {
                _ProgbarVisible = value;
                this.NotifyPropertyChanged("ProgbarVisible");
            }
        }

        protected int _ProgbarValue;
        public int ProgbarValue
        {
            get { return _ProgbarValue; }
            set
            {
                _ProgbarValue = value;
                this.NotifyPropertyChanged("ProgbarValue");
            }
        }

        protected ImageSource _Img;
        public ImageSource Img
        {
            get { return _Img; }
            set
            {
                _Img = value;
                this.NotifyPropertyChanged("Img");
            }
        }

        protected bool _ResumeEnabled;
        public bool ResumeEnabled
        {
            get { return _ResumeEnabled; }
            set
            {
                _ResumeEnabled = value;
                this.NotifyPropertyChanged("ResumeEnabled");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
    /// <summary>
    /// Interaktionslogik für CameraClickAndMove.xaml
    /// </summary>
    public partial class CameraClickAndMove : Window
    {

        protected static CameraClickAndMove SingletonWindow = null;

        TrackedThread ImgThread = null;

        protected double CamMoveXmm = 0;
        protected double CamMoveYmm = 0;

        protected CameraClickAndMoveDataHandler Data;
        protected static HardwareController HWC = null;

        protected int TimeStepsMS = 1000;

        protected WindowState PreviousWindowState;

        protected static int ScriptID = 0;

        protected CameraClickAndMove()
            : this(HWC)
        {
            //nix;
        }

        public CameraClickAndMove(HardwareController hwc)
        {
            InitializeComponent();
            this.Progress.Maximum = TimeStepsMS;

            LoadValues();

            Data = FindResource("datahandler") as CameraClickAndMoveDataHandler;

            HWC = hwc;

            //NewImage();
            ImgThread = new TrackedThread("CameraClickAndMove Window New Image Thread", NewImageContinousShot);
            ImgThread.Start();

            PreviousWindowState = System.Windows.WindowState.Normal;

            Console.WriteLine(this.GetType().AssemblyQualifiedName);
        }

        protected void LoadValues()
        {
            DataSafe ds = new DataSafe(Paths.SettingsPath, "MainWindow");
            CamMoveXmm = ds.Doubles["CamMoveXmm", 0];
            CamMoveYmm = ds.Doubles["CamMoveYmm", 0];

            ds.Doubles["CamMoveXmm"] = CamMoveXmm;
            ds.Doubles["CamMoveYmm"] = CamMoveYmm;
        }

        protected void NewImage()
        {
            TrackedThread tt = new TrackedThread("New Image from Camera Thread", () =>
            {                
                Camera c = HWC.Camera;                
                if (c.IsConnected)
                {
                    Data.ImageVisible = System.Windows.Visibility.Collapsed;
                    Data.ProgbarVisible = System.Windows.Visibility.Visible;
                    TrackedThread localthread = new TrackedThread("New Image from Camera Thread (local)", () =>
                    {
                        //c.SetOverlay(false, true, true);
                        //c.SetProcessing(true);
                        Thread.Sleep(TimeStepsMS);

                        System.Drawing.Bitmap b = c.GetImage();

                        ContentImage.Dispatcher.Invoke(new Action(() =>
                        {
                            Data.Img = BitmapToImageSource(b);
                        }));
                        
                        b.Dispose();

                        Data.ImageVisible = System.Windows.Visibility.Visible;
                        Data.ProgbarVisible = System.Windows.Visibility.Collapsed;

                    });
                    localthread.Start();

                    for (int i = 0; i < TimeStepsMS; ++i)
                    {
                        Data.ProgbarValue = i;
                        Thread.Sleep(1);
                    }
                }
            });
            tt.Start();
        }

        protected void NewImageContinousShot() 
        {
            Camera c = HWC.Camera;
            Data.ImageVisible = System.Windows.Visibility.Visible;
            Data.ProgbarVisible = System.Windows.Visibility.Collapsed;
            while (true)
            {
                if (ScriptID == 1)
                {
                    SingletonWindow.Data.ResumeEnabled = ScriptHandler.State_1 == ScriptThreadState.Paused;
                }

                if (ScriptID == 2)
                {
                    SingletonWindow.Data.ResumeEnabled = ScriptHandler.State_2 == ScriptThreadState.Paused;
                }
                try
                {
                    while (!c.IsConnected)
                    {
                        if (ScriptID == 1)
                        {
                            SingletonWindow.Data.ResumeEnabled = ScriptHandler.State_1 == ScriptThreadState.Paused;
                        }

                        if (ScriptID == 2)
                        {
                            SingletonWindow.Data.ResumeEnabled = ScriptHandler.State_2 == ScriptThreadState.Paused;
                        }
                        Thread.Sleep(500);
                    }
                    System.Drawing.Bitmap b = c.GetImage();
                    ContentImage.Dispatcher.Invoke(new Action(() =>
                    {
                        Data.Img = BitmapToImageSource(b);
                    }));
                    b.Dispose();
                    Thread.Sleep(100);
                }
                catch
                {
                    //nix
                    return;
                }
            }
        }

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

        private void ContentImage_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            /*Point p = e.GetPosition(this.ContentImage);
            double x = ((p.X - ContentImage.ActualWidth / 2.0) / ContentImage.ActualWidth) * 100.0;
            double y = ((p.Y - ContentImage.ActualHeight / 2.0) / ContentImage.ActualHeight) * 100.0;

            int incX = (int)(x * CamMoveXmm);
            int incY = (int)(y * (CamMoveYmm));

            Axis xa = HWC.GetAxis("X");
            xa.IncrementalMove(incX, xa.PositionSpeed, false);

            Axis ya = HWC.GetAxis("Y");
            ya.IncrementalMove(incY, ya.PositionSpeed, true);*/

            //NewImage();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            //Console.WriteLine("Huhu");
            ImgThread.Abort();
            SingletonWindow = null;            
        }

        private void ContentImage_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(this.ContentImage);
            double x = ((p.X - ContentImage.ActualWidth / 2.0) / ContentImage.ActualWidth) * 100.0;
            double y = ((p.Y - ContentImage.ActualHeight / 2.0) / ContentImage.ActualHeight) * 100.0;

            int incX = (int)(x * CamMoveXmm);
            int incY = (int)(y * (CamMoveYmm));

            Axis xa = HWC.GetAxis("X");
            xa.IncrementalMove(incX, xa.PositionSpeed, false);

            Axis ya = HWC.GetAxis("Y");
            ya.IncrementalMove(incY, ya.PositionSpeed, true);
        }

        public static void Display()
        {
            Display(HWC);
            if (SingletonWindow != null)
            {
                SingletonWindow.Data.ButtonsVisible = Visibility.Collapsed;
            }
        }

        public static void Display(int sciptID)
        {
            Display(HWC);
            ScriptID = sciptID;
            if (SingletonWindow != null)
            {
                SingletonWindow.Data.ButtonsVisible = Visibility.Visible;
                
                if (sciptID == 1)
                {
                    SingletonWindow.Data.ResumeEnabled = ScriptHandler.State_1 == ScriptThreadState.Paused;
                }

                if (sciptID == 2)
                {
                    SingletonWindow.Data.ResumeEnabled = ScriptHandler.State_2 == ScriptThreadState.Paused;
                }
            }
        }

        public static void Display(HardwareController hwc)
        {
            ScriptID = 0;
            if (HWC == null && hwc == null)
            {
                MessageBox.Show("Error while displaying Camera Window!");
                return;
            }
            if (SingletonWindow == null)
            {
                if (System.Threading.Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
                {
                    TrackedThread tt = new TrackedThread("Inner Thread,CameraClickAndMove.Display ", () =>
                    {
                        SingletonWindow = new CameraClickAndMove(hwc);
                        SingletonWindow.ShowDialog();
                    });
                    tt.ApartmentState = ApartmentState.STA;
                    tt.Start(true);
                    return;
                }
                SingletonWindow = new CameraClickAndMove(hwc);
                SingletonWindow.Show();
            }
            else
            {
                if (SingletonWindow.Dispatcher.Thread != System.Threading.Thread.CurrentThread)
                {
                    SingletonWindow.Dispatcher.Invoke(new Action(() =>
                    {
                        CameraClickAndMove.Display(HWC);
                    }));
                    return;
                }
                SingletonWindow.WindowState = SingletonWindow.PreviousWindowState;
                SingletonWindow.ShowInTaskbar = true;
            }
            SingletonWindow.Data.ButtonsVisible = Visibility.Collapsed;
            
        }

        public static void Mimimize()
        {
            if (SingletonWindow != null)
            {
                if (SingletonWindow.Dispatcher.Thread != System.Threading.Thread.CurrentThread)
                {
                    SingletonWindow.Dispatcher.Invoke(new Action(() =>
                    {
                        CameraClickAndMove.Mimimize();
                    }));
                    return;
                }
                SingletonWindow.WindowState = WindowState.Minimized;
                SingletonWindow.Data.ButtonsVisible = Visibility.Collapsed;
            }
        }

        public static void SetHWC(HardwareController hwc)
        {
            HWC = hwc;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState != System.Windows.WindowState.Minimized)
            {
                PreviousWindowState = this.WindowState;
            }
        }

        private void ManuallyBtn_Click(object sender, RoutedEventArgs e)
        {
            CameraClickAndMove.Mimimize();
        }

        private void ResumeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ScriptID == 1)
            {
                ScriptV2.ScriptHandler.ResumeThread1();
                Mimimize();
            }
            if (ScriptID == 2)
            {
                ScriptV2.ScriptHandler.ResumeThread2();
                Mimimize();
            }
        }
    }
}
