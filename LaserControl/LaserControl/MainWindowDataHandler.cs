using LaserControl.Design.Custom;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LaserControl
{
    public class MainWindowDataHandler :INotifyPropertyChanged
    {
        #region StatusBar
        protected string _CPULoad = "";
        public string CPULoad
        {
            get { return _CPULoad; }
            set
            {
                this._CPULoad = value;
                this.NotifyPropertyChanged("CPULoad");
            }
        }

        protected string _RAMLoad = "";
        public string RAMLoad
        {
            get { return _RAMLoad; }
            set
            {
                this._RAMLoad = value;
                this.NotifyPropertyChanged("RAMLoad");
            }
        }

        protected string _Threads = "";
        public string Threads
        {
            get { return _Threads; }
            set
            {
                this._Threads = value;
                this.NotifyPropertyChanged("Threads");
            }
        }

        protected System.Windows.Media.Brush _StatusBarBackground;
        public System.Windows.Media.Brush StatusBarBackground
        {
            get { return _StatusBarBackground; }
            set
            {
                _StatusBarBackground = value;
                NotifyPropertyChanged("StatusBarBackground");
            }
        }

        #endregion //StatusBar

        protected List<CloseableTabItem> _TabItems = new List<CloseableTabItem>();
        public List<CloseableTabItem> TabItems
        {
            get { return _TabItems; }
            set
            {
                this._TabItems = value;
                this.NotifyPropertyChanged("TabItems");
            }
        }

        protected CloseableTabItem _SelectedTabItem = null;
        public CloseableTabItem SelectedTabItem
        {
            get { return _SelectedTabItem; }
            set
            {
                this._SelectedTabItem = value;
                this.NotifyPropertyChanged("SelectedTabItem");
            }
        }

        public void TabItems_Add(CloseableTabItem cti)
        {
            cti.CloseTab += cti_CloseTab;

            this._TabItems.Add(cti);            
            List<CloseableTabItem> old = _TabItems;

            this.TabItems = new List<CloseableTabItem>(_TabItems);
            this.SelectedTabItem = cti;

            old.Clear();
            old = null;
        }

        public void cti_CloseTab(object sender, System.Windows.RoutedEventArgs e)
        {
            this.TabItems_Remove((CloseableTabItem)sender);
        }

        public void TabItems_Remove(CloseableTabItem cti)
        {
            int pos = _TabItems.IndexOf(cti);

            if (SelectedTabItem != cti)
            {

            }

            this._TabItems.Remove(cti);
            List<CloseableTabItem> old = _TabItems;
            this.TabItems = new List<CloseableTabItem>(_TabItems);
            old.Clear();
            old = null;
            if (pos >= _TabItems.Count)
            {
                pos = _TabItems.Count - 1;
                if (pos >= 0)
                    SelectedTabItem = _TabItems[_TabItems.Count - 1];
                else
                    SelectedTabItem = null;
            }
            else
            {
                SelectedTabItem = _TabItems[pos];
            }

            if (this.TabItems.Count == 0)
            {
                TabItems_Add(new CloseableTabItem("new"));
            }

            /*if (SelectedTabItem == cti)
            {
                //Das Aktuelle wird geschlossne
            }
            else
            {
                //Ein anderes wird geschlossen
            }*/
        }

        #region Menu

        protected bool _PauseEnabled;
        public bool PauseEnabled
        {
            get { return _PauseEnabled; }
            set
            {
                _PauseEnabled = value;
                this.NotifyPropertyChanged("PauseEnabled");
            }
        }


        protected bool _RedoEnabled;
        public bool RedoEnabled
        {
            get { return _RedoEnabled; }
            set
            {
                _RedoEnabled = value;
                this.NotifyPropertyChanged("RedoEnabled");
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


        protected bool _RunEnabled;
        public bool RunEnabled
        {
            get { return _RunEnabled; }
            set
            {
                _RunEnabled = value;                
                this.NotifyPropertyChanged("RunEnabled");
            }
        }

        protected bool _SavedEnabled;
        public bool SavedEnabled
        {
            get { return _SavedEnabled; }
            set
            {
                //if (value != SavedEnabled)
                //{
                    _SavedEnabled = value;
                    this.NotifyPropertyChanged("SavedEnabled");
                //}
            }
        }

        protected bool _TFPauseEnabled;
        public bool TFPauseEnabled
        {
            get { return _TFPauseEnabled;  }
            set
            {
                _TFPauseEnabled = value;
                this.NotifyPropertyChanged("TFPauseEnabled");
            }
        }

        protected bool _TFRunEnabled;
        public bool TFRunEnabled
        {
            get { return _TFRunEnabled; }
            set
            {
                _TFRunEnabled = value;
                this.NotifyPropertyChanged("TFRunEnabled");
            }
        }

        protected bool _TFResumeEnabled;
        public bool TFResumeEnabled
        {
            get { return _TFResumeEnabled; }
            set
            {
                _TFResumeEnabled = value;
                this.NotifyPropertyChanged("TFResumeEnabled");
            }
        }

        protected bool _UndoEnabled;
        public bool UndoEnabled
        {
            get { return _UndoEnabled; }
            set
            {
                _UndoEnabled = value;
                this.NotifyPropertyChanged("UndoEnabled");                
            }
        }

        #endregion //Menu

        protected List<string> _CodeTextFieldItems;
        public List<string> CodeTextFieldItems
        {
            get { return _CodeTextFieldItems; }
            set
            {
                _CodeTextFieldItems = value;
                NPC("CodeTextFieldItems");
            }
        }

        #region Menu Camera

        protected bool _CameraConnected;
        public bool CameraConnected
        {
            get { return _CameraConnected; }
            set
            {
                _CameraConnected = value;
                if (value)
                {
                    CameraConnectedHeader = "Connected";
                    CameraConnectedImg = "connect";
                }
                else
                {
                    CameraConnectedHeader = "Disconnected";
                    CameraConnectedImg = "disconnect";
                }
            }
        }

        protected string _CameraConnectedHeader;
        public string CameraConnectedHeader
        {
            get { return _CameraConnectedHeader; }
            set
            {
                _CameraConnectedHeader = value;
                this.NotifyPropertyChanged("CameraConnectedHeader");
            }
        }

        protected string _CameraConnectedImg;
        public string CameraConnectedImg
        {
            get { return _CameraConnectedImg; }
            set
            {
                if (value != _CameraConnectedImg)
                {
                    _CameraConnectedImg = value;
                    this.NotifyPropertyChanged("CameraConnectedImg");
                }
            }
        }

        #endregion //Camera

        protected string _TextFieldCode;
        public string TextFieldCode
        {
            get
            {
                return _TextFieldCode;
            }
            set
            {
                _TextFieldCode = value;
                NPC("TextFieldCode");
            }
        }

        #region Methoden zum setzen von Parametern

        public event PropertyChangedEventHandler PropertyChanged;

        public void NPC(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        #endregion //Methoden zum setzen von Parametern
    }
}
