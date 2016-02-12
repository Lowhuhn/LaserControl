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

using LaserControl.UIWindows.Dialogs;

namespace LaserControl.UIElements
{
    public class UI_HWC_ToolsDataHandler : INotifyPropertyChanged
    {
        private string _ControlIdent;
        public string ControlIdent
        {
            get { return _ControlIdent; }
            set
            {
                _ControlIdent = value;
                NotifyPropertyChanged("ControlIdent");
            }
        }

        private string _CName;
        public string CName
        {
            get { return _CName; }
            set
            {
                _CName = value;
                NotifyPropertyChanged("CName");
            }
        }

        private string _Focus;
        public string Focus
        {
            get { return _Focus; }
            set
            {
                _Focus = value;
                NotifyPropertyChanged("Focus");
            }
        }

        private List<string> _ToolType;
        public List<string> ToolType
        {
            get { return _ToolType; }
            set
            {
                _ToolType = value;
                NotifyPropertyChanged("ToolType");
            }
        }

        private int _ToolTypeIndex;
        public int ToolTypeIndex
        {
            get { return _ToolTypeIndex;  }
            set
            {
                _ToolTypeIndex = value;
                NotifyPropertyChanged("ToolTypeIndex");
            }
        }

        private string _CO_X;
        public string CO_X
        {
            get { return _CO_X; }
            set
            {
                _CO_X = value;
                NotifyPropertyChanged("CO_X");
            }
        }

        private string _CO_Y;
        public string CO_Y
        {
            get { return _CO_Y; }
            set
            {
                _CO_Y = value;
                NotifyPropertyChanged("CO_Y");
            }
        }

        private Visibility _RightVisibility;
        public Visibility RightVisibility
        {
            get { return _RightVisibility; }
            set
            {
                _RightVisibility = value;
                NotifyPropertyChanged("RightVisibility");
            }
        }

        private ObservableCollection<String> _AllToolsItems;
        public ObservableCollection<String> AllToolsItems
        {
            get
            {
                if (_AllToolsItems == null)
                {
                    _AllToolsItems = new ObservableCollection<String>();
                }
                return _AllToolsItems;
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
    /// Interaktionslogik für UI_HWC_Tools.xaml
    /// </summary>
    public partial class UI_HWC_Tools : UserControl
    {
        protected UI_HWC_ToolsDataHandler Data;
        protected HardwareController HWC;
        protected Tool Current = null;

        public UI_HWC_Tools()
        {
            InitializeComponent();

            Data = FindResource("datahandler") as UI_HWC_ToolsDataHandler;

            List<string> l = new List<string>();
            var values = Enum.GetValues(typeof(ToolType)).Cast<ToolType>();
            foreach (var v in values)
            {
                l.Add(v.ToString());
            }

            Data.ToolType = l;
        }

        public void SetHWC(HardwareController hwc)
        {
            HWC = hwc;
            string[] all = HWC.ToolsControlIdents;
            Array.Sort<string>(all);
            foreach (string s in all)
            {
                Data.AllToolsItems.Add(s);
            }
        }

        private void AllTools_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AllTools.SelectedItem == null)
            {
                return;
            }
            Tool t = HWC.GetTool(AllTools.SelectedItem.ToString());
            if (t != null)
            {
                Current = t;
                Data.ControlIdent = t.ControlIdent;
                Data.CName = t.Name;
                Data.Focus = Axis.SConvertIntToStr(t.Fokus);
                Data.ToolTypeIndex = (int)t.MyToolType;
                Data.CO_X = Axis.SConvertIntToStr(t.CameraOffset[0]);
                Data.CO_Y = Axis.SConvertIntToStr(t.CameraOffset[1]);

                Data.RightVisibility = System.Windows.Visibility.Visible;

            }
            

        }

        private void AddToolBtn_Click(object sender, RoutedEventArgs e)
        {
            string newID = Prompts.ShowDialog_Text("New Tool", "New tool control id", "");
            Tool t = HWC.GetTool(newID);
            if (t != null)
            {
                return;
            }
            t = HWC.NewTool(newID.ToUpper());
            Data.AllToolsItems.Add(t.ControlIdent);
            AllTools.SelectedItem = t.ControlIdent;
            HWC.Save();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Current != null)
            {
                Current.Name = Data.CName;
                Current.Fokus = Axis.SConvertDoubleString(Data.Focus);
                Current.CameraOffset[0] = Axis.SConvertDoubleString(Data.CO_X);
                Current.CameraOffset[1] = Axis.SConvertDoubleString(Data.CO_Y);
                Current.MyToolType = (ToolType)Data.ToolTypeIndex;

                HWC.Save();
            }
        }

    }
}
