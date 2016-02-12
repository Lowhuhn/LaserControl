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
using System.Windows.Navigation;
using System.Windows.Shapes;

using LaserControl.HardwareAPI;

namespace LaserControl.UIElements
{

    public class UI_HWC_IOsDataHandler : INotifyPropertyChanged
    {
       
        protected List<IOGridEntry> _Parameters;
        public List<IOGridEntry> Parameters
        {
            get { return _Parameters; }
            set
            {
                this._Parameters = value;
                this.NotifyPropertyChanged("Parameters");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }

    public class IOGridEntry
    {
        public string ParamController { get; set; }
        public int ParamBit { get; set; }
        public IOPortType ParamPortType { get; set; }
        public string ParamInName { get; set; }
        public string ParamOutName { get; set; }
    }

    /// <summary>
    /// Interaktionslogik für UI_HWC_IOs.xaml
    /// </summary>
    public partial class UI_HWC_IOs : UserControl
    {
        private HardwareController HWC;
        private UI_HWC_IOsDataHandler Data;

        public UI_HWC_IOs()
        {
            InitializeComponent();

            Data = FindResource("datahandler") as UI_HWC_IOsDataHandler;

            
        }

        public void SetHWC(HardwareController hwc)
        {
            HWC = hwc;
            string[] idents = hwc.IOControllerControlIdents;
            Array.Sort<string>(idents);
            ParameterControllerColumn.ItemsSource = new List<string>(idents);            
            IOPortType[] s = new IOPortType[] { IOPortType.NOUSE, IOPortType.IN, IOPortType.INOUT, IOPortType.OUT };
            ParameterPortTypeColumn.ItemsSource = new List<IOPortType>(s);

            List<IOGridEntry> entrys = new List<IOGridEntry>();
            foreach (var i in idents)
            {
                IOController ic = hwc.GetIOController(i);
                IOPort[] all = ic.All;
                foreach (var p in all)
                {
                    entrys.Add(new IOGridEntry()
                    {
                        ParamController = i,
                        ParamBit = p.Bit,
                        ParamPortType = p.PortType,
                        ParamInName = p.InName,
                        ParamOutName = p.OutName
                    });
                }
            }
            Data.Parameters = entrys;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            string[] all = HWC.IOControllerControlIdents;
            foreach (string s in all)
            {
                IOController i = HWC.GetIOController(s);
                i.RemoveAllPorts();
            }

            foreach (var r in Data.Parameters)
            {               
                if (!string.IsNullOrEmpty(r.ParamController) && !string.IsNullOrWhiteSpace(r.ParamController) )
                {
                    IOController ic = HWC.GetIOController(r.ParamController);
                    if (ic != null)
                    {
                        if (!ic.ContainsPort(r.ParamBit))
                        {
                            //Port neu erstellen
                            IOPort pn = new IOPort(r.ParamBit, r.ParamPortType);
                            pn.InName = r.ParamInName;
                            pn.OutName = r.ParamOutName;
                            ic.AddPort(pn);
                        }
                        else
                        {
                            //Port verändern
                            IOPort p = ic.GetPort(r.ParamBit);
                            p.PortType = r.ParamPortType;
                            p.InName = r.ParamInName;
                            p.OutName = r.ParamOutName;
                        }
                    }
                }
            }
            HWC.Save();
        }
    }
}
