using LaserControl.HardwareAPI;
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

namespace LaserControl.UIWindows
{

    public class HardwareSettingsDataHandler : INotifyPropertyChanged
    {
        private bool _EnableAll;
        public bool EnableAll
        {
            get { return _EnableAll; }
            set
            {
                _EnableAll = value;
                NotifyPropertyChanged("EnableAll");
            }
        }

        private string _TOX;
        public string TOX
        {
            get { return _TOX; }
            set
            {
                _TOX = value;
                NotifyPropertyChanged("TOX");
            }
        }
        private string _TOY;
        public string TOY
        {
            get { return _TOY; }
            set
            {
                _TOY = value;
                NotifyPropertyChanged("TOY");
            }
        }

        private string _TCX;
        public string TCX
        {
            get { return _TCX; }
            set
            {
                _TCX = value;
                NotifyPropertyChanged("TCX");
            }
        }
        private string _TCY;
        public string TCY
        {
            get { return _TCY; }
            set
            {
                _TCY = value;
                NotifyPropertyChanged("TCY");
            }
        }

        private string _LPX;
        public string LPX
        {
            get { return _LPX; }
            set
            {
                _LPX = value;
                NotifyPropertyChanged("LPX");
            }
        }
        private string _LPY;
        public string LPY
        {
            get { return _LPY; }
            set
            {
                _LPY = value;
                NotifyPropertyChanged("LPY");
            }
        }
        private string _LPZ;
        public string LPZ
        {
            get { return _LPZ; }
            set
            {
                _LPZ = value;
                NotifyPropertyChanged("LPZ");
            }
        }

        private bool _IntelligentScribe;
        public bool IntelligentScribe
        {
            get { return _IntelligentScribe; }
            set
            {
                _IntelligentScribe = value;
                NotifyPropertyChanged("IntelligentScribe");
            }
        }

        private bool _IntelligentScribeOff;
        public bool IntelligentScribeOff
        {
            get { return _IntelligentScribeOff; }
            set
            {
                _IntelligentScribeOff = value;
                NotifyPropertyChanged("IntelligentScribeOff");
            }
        }

        private bool _HomeWhileScribing;
        public bool HomeWhileScribing
        {
            get { return _HomeWhileScribing; }
            set
            {
                _HomeWhileScribing = value;
                NotifyPropertyChanged("HomeWhileScribing");
            }
        }

        private bool _HomeWhileScribingOff;
        public bool HomeWhileScribingOff
        {
            get { return _HomeWhileScribingOff; }
            set
            {
                _HomeWhileScribingOff = value;
                NotifyPropertyChanged("HomeWhileScribingOff");
            }
        }

        private string _CameraURL;
        public string CameraURL
        {
            get { return _CameraURL; }
            set
            {
                _CameraURL = value;
                NotifyPropertyChanged("CameraURL");
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
    /// Interaktionslogik für HardwareSettings.xaml
    /// </summary>
    public partial class HardwareSettings : Window
    {
        protected HardwareController HWC;
        protected HardwareSettingsDataHandler Data;

        protected Dictionary<string, CheckBox> HomeWhileScribingDict = new Dictionary<string, CheckBox>();

        public HardwareSettings(HardwareController hwc)
        {
            InitializeComponent();

            Data = FindResource("datahandler") as HardwareSettingsDataHandler;

            HWC = hwc;
            this.SetHWC();

            AxesConfig.SetHWC(hwc);
            IOConfig.SetHWC(hwc);
            ToolsConfig.SetHWC(hwc);
        }

        protected void SetHWC()
        {
            Data.EnableAll = false;

            //Camera 
            Data.CameraURL = HWC.Camera.Path;

            //Table Origin
            Data.TOX = Axis.SConvertIntToStr(HWC.TableOrigin[0]);
            Data.TOY = Axis.SConvertIntToStr(HWC.TableOrigin[1]);

            //Table Center
            Data.TCX = Axis.SConvertIntToStr(HWC.TableCenter[0]);
            Data.TCY = Axis.SConvertIntToStr(HWC.TableCenter[1]);

            //Load Position
            Data.LPX = Axis.SConvertIntToStr(HWC.LoadPosition[0]);
            Data.LPY = Axis.SConvertIntToStr(HWC.LoadPosition[1]);
            Data.LPZ = Axis.SConvertIntToStr(HWC.LoadPosition[2]);

            //Intelligent Scribe
            Data.IntelligentScribe = HWC.IntelligentScribe;
            Data.IntelligentScribeOff = !HWC.IntelligentScribe;

            Data.HomeWhileScribing = HWC.HomeWhileScribing;
            Data.HomeWhileScribingOff = !HWC.HomeWhileScribing;

            string[] axes = HWC.AxesControlIdents;
            List<string> hwScribAxes = HWC.HomeWhileScribingAxes;

            HomeWhileScribingDict.Clear();

            foreach (string a in axes)
            {
                CheckBox cb = new CheckBox();
                cb.Content = a;
                cb.Width = 100;
                cb.IsChecked = hwScribAxes.Contains(a);

                this.HWScribingAxesPanel.Children.Add(cb);

                this.HomeWhileScribingDict.Add(a, cb);
            }

            Data.EnableAll = true;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (HWC != null)
            {
                Data.EnableAll = false;

                HWC.Camera.Path = Data.CameraURL;

                HWC.TableOrigin[0] = Axis.SConvertDoubleString(Data.TOX);
                HWC.TableOrigin[1] = Axis.SConvertDoubleString(Data.TOY);

                HWC.TableCenter[0] = Axis.SConvertDoubleString(Data.TCX);
                HWC.TableCenter[1] = Axis.SConvertDoubleString(Data.TCY);

                HWC.LoadPosition[0] = Axis.SConvertDoubleString(Data.LPX);
                HWC.LoadPosition[1] = Axis.SConvertDoubleString(Data.LPY);
                HWC.LoadPosition[2] = Axis.SConvertDoubleString(Data.LPZ);

                HWC.IntelligentScribe = Data.IntelligentScribe;


                HWC.HomeWhileScribing = Data.HomeWhileScribing;
                //List<string> hwScribAxes = HWC.HomeWhileScribingAxes;
                HWC.HomeWhileScribingAxes.Clear();
                foreach (KeyValuePair<string, CheckBox> k in HomeWhileScribingDict)
                {
                    if (k.Value.IsChecked == true)
                    {
                        HWC.HomeWhileScribingAxes.Add(k.Key);
                    }
                }

                HWC.Save();

                Data.EnableAll = true;
            }
        }
    }
}
