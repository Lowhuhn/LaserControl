using LaserControl.Data;
using LaserControl.HardwareAPI;
using LaserControl.Library;
using LaserControl.UIWindows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LaserControl.UIElements
{
    public class OneAxisDataHandler : INotifyPropertyChanged
    {
        protected string _AxisName = "";
        public string AxisName
        {
            get { return _AxisName; }
            set
            {
                this._AxisName = value;
                this.NotifyPropertyChanged("AxisName");
            }
        }

        protected string _StatusStr = "";
        public string StatusStr
        {
            get { return _StatusStr; }
            set
            {
                this._StatusStr = value;
                this.NotifyPropertyChanged("StatusStr");
            }
        }

        protected SolidColorBrush _StatusColor = Brushes.White;
        public SolidColorBrush StatusColor
        {
            get { return _StatusColor; }
            set
            {
                this._StatusColor = value;
                this.NotifyPropertyChanged("StatusColor");
            }
        }

        protected string _StatusImg = "";
        public string StatusImg
        {
            get { return _StatusImg; }
            set
            {
                this._StatusImg = value;
                this.NotifyPropertyChanged("StatusImg");
            }
        }

        protected bool _HasFault = false;
        public bool HasFault
        {
            get { return _HasFault; }
            set
            {
                this._HasFault = value;
                this.NotifyPropertyChanged("HasFault");
            }
        }


        protected string _JogStr = "";
        public string JogStr
        {
            get { return _JogStr; }
            set
            {
                if (_JogStr != value)
                {
                    this._JogStr = value;
                    this.NotifyPropertyChanged("JogStr");
                }
            }
        }

        protected string _RelPosStr = "";
        public string RelPosStr
        {
            get { return _RelPosStr; }
            set
            {
                if (_RelPosStr != value)
                {
                    this._RelPosStr = value;
                    this.NotifyPropertyChanged("RelPosStr");
                }
            }
        }

        protected string _AbsPosStr = "";
        public string AbsPosStr
        {
            get { return _AbsPosStr; }
            set
            {
                if (_AbsPosStr != value)
                {
                    this._AbsPosStr = value;
                    this.NotifyPropertyChanged("AbsPosStr");
                }
            }
        }

        protected string _VelocityStr = "";
        public string VelocityStr
        {
            get { return _VelocityStr; }
            set
            {
                this._VelocityStr = value;
                this.NotifyPropertyChanged("VelocityStr");
            }
        }

        protected string _FreeRunStr = "";
        public string FreeRunStr
        {
            get { return _FreeRunStr; }
            set
            {
                this._FreeRunStr = value;
                this.NotifyPropertyChanged("FreeRunStr");
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
    /// Interaktionslogik für UI_OneAxis.xaml
    /// </summary>
    public partial class UI_OneAxis : UserControl
    {
        protected TrackedThread FreeRunThread;
        protected Queue<Action> FreeRunQueue;

        protected OneAxisDataHandler Data;
        protected Axis MyAxis;
        protected HardwareController HWC;

        protected int Speed = 0;
        protected int Distance = 0;
        protected bool UseDistance = false;

        public UI_OneAxis()
        {
            InitializeComponent();
            Data = FindResource("datahandler") as OneAxisDataHandler;

            
        }

        public void SetAxisAndInitGUI(HardwareController hwc, Axis axis)
        {
            HWC = hwc;
            MyAxis = axis;
            Data.AxisName = MyAxis.ControlIdent;

            EnableChange(axis, axis.IsEnable);
            HasFaultChange(axis, axis.HasFault);

            this.Data.AbsPosStr = Axis.SConvertIntToStr(MyAxis.Position, true);
            this.Data.RelPosStr = Axis.SConvertIntToStr(HWC.ConvertCoordinatesAlways(MyAxis.ControlIdent, -MyAxis.Position) * (-1), true);

            //Verbinde Notifyer
            MyAxis.IsEnableChanged += EnableChange;
            MyAxis.HasFaultChange += HasFaultChange;
            MyAxis.PositionChange += PositionChange;
            MyAxis.VelocityChange += VelocityChange;

            //Load Values
            DataSafe ds = new DataSafe(Paths.SettingsPath, "MainWindow");
            Speed = ds.Ints[axis.ControlIdent + "-FR-Speed", 0];
            Distance = ds.Ints[axis.ControlIdent + "-FR-Distance", 0];
            UseDistance = ds.Bools[axis.ControlIdent + "-FR-UseDis", false];
            DisplayFreeRunValues();

            FreeRunThread = new TrackedThread("Free Run Distance Thread: "+ MyAxis, this.FreeRunThreadMethod);
            FreeRunQueue = new Queue<Action>();
            FreeRunThread.Start();
        }

        protected void DisplayFreeRunValues()
        {
            if (UseDistance)
            {
                Data.FreeRunStr = "Spd: " + Axis.SConvertIntToStr(Speed) + "   Dst: " + Axis.SConvertIntToStr(Distance);
            }
            else
            {
                Data.FreeRunStr = "Speed: " + Axis.SConvertIntToStr(Speed);
            }
        }

        #region Event Handler
        protected void EnableChange(Controller c, bool b)
        {
            Axis a = c as Axis;
            if (!a.HasFault)
            {
                if (b)
                {
                    Data.StatusStr = "Enabled";
                    Data.StatusColor = Brushes.LawnGreen;
                    Data.StatusImg = "bullet_green";
                }
                else
                {
                    Data.StatusStr = "Disabled";
                    Data.StatusColor = Brushes.White;
                    Data.StatusImg = "bullet_white";
                }
            }
        }

        protected void HasFaultChange(Axis a, bool value)
        {
            if (value)
            {
                Data.StatusStr = "Fault";
                Data.StatusColor = Brushes.Red;
                Data.StatusImg = "bullet_red";
            }
            Data.HasFault = value;
        }

        protected void PositionChange(Axis a, int pos)
        {
            this.Data.AbsPosStr = Axis.SConvertIntToStr(pos, true);
            this.Data.RelPosStr = Axis.SConvertIntToStr(HWC.ConvertCoordinatesAlways(MyAxis.ControlIdent, -pos)*(-1), true);
        }

        protected void VelocityChange(Axis a, int vel)
        {
            this.Data.VelocityStr = Axis.SConvertIntToStr(vel, true);
        }

        #endregion //Event Handler

        #region Button Click Methods
        private void EnDisableBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!MyAxis.HasFault)
            {
                if (MyAxis.IsEnable)
                {
                    MyAxis.Disable();
                }
                else
                {
                    MyAxis.Enable();
                }
            }
        }

        private void HomeBtn_Click(object sender, RoutedEventArgs e)
        {
            MyAxis.Home();
        }

        private void ClearFaultBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MyAxis.ClearFaults();
            }
            catch
            {
#warning Fehler abfangen
            }
        }
        #endregion //Button Click Methods

        #region Free Run
        protected void FreeRunThreadMethod()
        {
            while (true)
            {
                if (FreeRunQueue.Count > 0)
                {
                    while (FreeRunQueue.Count > 0)
                    {
                        Action d = FreeRunQueue.Dequeue();
                        d();                        
                    }
                }
                else
                {
                    Thread.Sleep(1);
                }
            }
        }

        private void SetSpeedBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FreeRunSpeed frs = new FreeRunSpeed(Axis.SConvertIntToStr(Speed), Axis.SConvertIntToStr(Distance), UseDistance);
                if (frs.ShowDialog() == true)
                {
                    Speed = Axis.SConvertDoubleString(frs.SpeedStr);
                    Distance = Axis.SConvertDoubleString(frs.DistStr);
                    UseDistance = frs.UseDistVal;
                    //Save Values
                    DataSafe ds = new DataSafe(Paths.SettingsPath, "MainWindow");
                    ds.Ints[MyAxis.ControlIdent + "-FR-Speed"] = Speed;
                    ds.Ints[MyAxis.ControlIdent + "-FR-Distance"] = Distance;
                    ds.Bools[MyAxis.ControlIdent + "-FR-UseDis"] = UseDistance;

                    DisplayFreeRunValues();
                }
            }
            catch
            {

            }
        }

        private void MoveLeftBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!UseDistance)
            {
                MyAxis.StartFreeRun(-Speed);
            }
            else
            {
                /*TrackedThread tt = new TrackedThread("Free Run Distance Left: "+MyAxis, () =>
                {
                    MyAxis.WaitForMotionDone();
                    MyAxis.IncrementalMove(-Distance, Speed);
                });
                tt.Start();*/
                FreeRunQueue.Enqueue(delegate()
                {
                    MyAxis.WaitForMotionDone();
                    MyAxis.IncrementalMove(-Distance, Speed);
                });
            }
        }

        private void MoveRightBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!UseDistance)
            {
                MyAxis.StartFreeRun(Speed);
            }
            else
            {
                /*TrackedThread tt = new TrackedThread("Free Run Distance Right: " + MyAxis, () =>
                {
                    MyAxis.WaitForMotionDone();
                    MyAxis.IncrementalMove(Distance, Speed);
                });
                tt.Start();*/
                FreeRunQueue.Enqueue(delegate()
                {
                    MyAxis.WaitForMotionDone();
                    MyAxis.IncrementalMove(Distance, Speed);
                });
            }
        }

        private void MoveLeftBtn_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!UseDistance)
            {
                MyAxis.StopFreeRun();
            }
        }

        private void MoveRightBtn_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!UseDistance)
            {
                MyAxis.StopFreeRun();
            }
        }

        #endregion //Free Run
    }
}
