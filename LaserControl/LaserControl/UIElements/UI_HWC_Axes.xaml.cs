using LaserControl.HardwareAPI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LaserControl.UIElements
{
    public class UI_HWC_AxesDataHandler : INotifyPropertyChanged
    {
        protected string _Name = "";
        public string Name
        {
            get { return _Name; }
            set
            {
                this._Name = value;
                this.NotifyPropertyChanged("Name");
            }
        }

        protected string _Min = "";
        public string Min
        {
            get { return _Min; }
            set
            {
                this._Min = value;
                this.NotifyPropertyChanged("Min");
            }
        }
        protected string _Max = "";
        public string Max
        {
            get { return _Max; }
            set
            {
                this._Max = value;
                this.NotifyPropertyChanged("Max");
            }
        }
        protected string _MinVelocity = "";
        public string MinVelocity
        {
            get { return _MinVelocity; }
            set
            {
                this._MinVelocity = value;
                this.NotifyPropertyChanged("MinVelocity");
            }
        }
        protected string _MaxVelocity = "";
        public string MaxVelocity
        {
            get { return _MaxVelocity; }
            set
            {
                this._MaxVelocity = value;
                this.NotifyPropertyChanged("MaxVelocity");
            }
        }
        protected string _PositionSpeed = "";
        public string PositionSpeed
        {
            get { return _PositionSpeed; }
            set
            {
                this._PositionSpeed = value;
                this.NotifyPropertyChanged("PositionSpeed");
            }
        }
        protected string _RampRate = "";
        public string RampRate
        {
            get { return _RampRate; }
            set
            {
                this._RampRate = value;
                this.NotifyPropertyChanged("RampRate");
            }
        }
        protected string _RampDistancePercent = "";
        public string RampDistancePercent
        {
            get { return _RampDistancePercent; }
            set
            {
                this._RampDistancePercent = value;
                this.NotifyPropertyChanged("RampDistancePercent");
            }
        }
        protected string _MinAccelRampTimeMS = "";
        public string MinAccelRampTimeMS
        {
            get { return _MinAccelRampTimeMS; }
            set
            {
                this._MinAccelRampTimeMS  = value;
                this.NotifyPropertyChanged("MinAccelRampTimeMS");
            }
        }
        protected string _StaticRampDistance = "";
        public string StaticRampDistance
        {
            get { return _StaticRampDistance; }
            set
            {
                this._StaticRampDistance = value;
                this.NotifyPropertyChanged("StaticRampDistance");
            }
        }
        protected bool _UseStaticRampDistance = true;
        public bool UseStaticRampDistance
        {
            get { return _UseStaticRampDistance; }
            set
            {
                this._UseStaticRampDistance = value;
                this.NotifyPropertyChanged("UseStaticRampDistance");
            }
        }

        protected Visibility _RightVisibility;
        public Visibility RightVisibility
        {
            get { return _RightVisibility; }
            set
            {
                this._RightVisibility = value;
                this.NotifyPropertyChanged("RightVisibility");
            }
        }

        private ObservableCollection<String> _AllAxesItems;
        public ObservableCollection<String> AllAxesItems
        {
            get 
            {
                if (_AllAxesItems == null)
                {
                    _AllAxesItems = new ObservableCollection<String>();
                }
                return _AllAxesItems;
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
    /// Interaktionslogik für UI_HWC_Axes.xaml
    /// </summary>
    public partial class UI_HWC_Axes : UserControl
    {
        UI_HWC_AxesDataHandler Data;
        HardwareController HWC;
        Axis Current = null;

        public UI_HWC_Axes()
        {
            InitializeComponent();
            Data = FindResource("datahandler") as UI_HWC_AxesDataHandler;

            /*Data.AllAxesItems.Add("Axis 1");
            Data.AllAxesItems.Add("Axis 2");
            Data.AllAxesItems.Add("Axis 3");*/
        }

        public void SetHWC(HardwareController hwc)
        {
            HWC = hwc;
            string[] all = HWC.AxesControlIdents;
            foreach (string s in all)
            {
                Data.AllAxesItems.Add(s);
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Current != null)
            {
                try
                {
                    Current.Min = Axis.SConvertDoubleString(Data.Min);
                    Current.Max = Axis.SConvertDoubleString(Data.Max);

                    Current.MinVelocity = Axis.SConvertDoubleString(Data.MinVelocity);
                    Current.MaxVelocity = Axis.SConvertDoubleString(Data.MaxVelocity);

                    Current.PositionSpeed = Axis.SConvertDoubleString(Data.PositionSpeed);
                    Current.RampRate = int.Parse(Data.RampRate);

                    Current.RampDistancePercent = int.Parse(Data.RampDistancePercent);
                    Current.MinAccelRampTimeMS = int.Parse(Data.MinAccelRampTimeMS);

                    Current.StaticRampDistance = Axis.SConvertDoubleString(Data.StaticRampDistance);
                    Current.UseStaticRampDistance = Data.UseStaticRampDistance;
                }
                catch
                {
                    MessageBox.Show("Error in input data!\nPlease check all fields...");
                    return;
                }
                Current.Save();
                Data.RightVisibility = System.Windows.Visibility.Hidden;
                Current = null;
                AllAxes.UnselectAll();
            }
        }

        private void AllAxes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AllAxes.SelectedItem == null)
            {
                return;
            }

            Data.RightVisibility = System.Windows.Visibility.Visible;                        
            Axis a = HWC.GetAxis(AllAxes.SelectedItem.ToString());
            if (a != null)
            {
                Current = a;
                Data.Name = a.Name;
                Data.Min = Axis.SConvertIntToStr(a.Min);
                Data.Max = Axis.SConvertIntToStr(a.Max);
                Data.MinVelocity = Axis.SConvertIntToStr(a.MinVelocity);
                Data.MaxVelocity = Axis.SConvertIntToStr(a.MaxVelocity);

                Data.PositionSpeed = Axis.SConvertIntToStr(a.PositionSpeed);
                Data.RampRate = a.RampRate.ToString() ;

                Data.RampDistancePercent = a.RampDistancePercent.ToString();
                Data.MinAccelRampTimeMS = a.MinAccelRampTimeMS.ToString();

                Data.StaticRampDistance = Axis.SConvertIntToStr(a.StaticRampDistance);
                Data.UseStaticRampDistance = a.UseStaticRampDistance;
            }
        }
    }
}
