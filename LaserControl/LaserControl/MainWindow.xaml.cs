using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using LaserControl.Data;
using LaserControl.Library;
using System.Threading;

using LaserControl.Startup;
using LaserControl.HardwareAPI;
using System.Globalization;

using LaserControl.ScriptV2;

using LaserControl.Design.Custom;
using LaserControl.UIWindows;

#if DEBUG
    #warning DEBUG is defined
#endif
#if TEST
    #warning TEST is defined
#endif

namespace LaserControl
{
    public static class CustomCommands
    {
        public static RoutedCommand CloseTab = new RoutedCommand();

        public static RoutedCommand Pause = new RoutedCommand();
        public static RoutedCommand Run = new RoutedCommand();
        
    }

    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
#if TEST
        public MainWindowDataHandler Data;
#else
        protected MainWindowDataHandler Data;
#endif

        protected TrackedThread ThreadUpdateMenu;
        protected TrackedThread ThreadUpdateStatusBar;

        protected HardwareController HWC;

        protected CloseableTabItem CurrentTab
        {
            get
            {
                return (CloseableTabItem)ContentTabControl.SelectedItem;
            }
        }

        public MainWindow()
        {
            
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");

            InitializeComponent();

            Data = FindResource("datahandler") as MainWindowDataHandler;
            StartupInit();

            ThreadUpdateMenu = new TrackedThread("MainWindow - Update Menu Bars", DisplayFileInfoAndUpdateMenuBars);
            ThreadUpdateMenu.Start();

            ThreadUpdateStatusBar = new TrackedThread("MainWindow - Update Status Bar", DisplayStatusBarInformation);
            ThreadUpdateStatusBar.Start();

            Data.StatusBarBackground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 122, 204));

        }
        

        public void StartupInit()
        {
            DataSafe ds = new DataSafe(Paths.SettingsPath, "MainWindow");

            if (!ds.containsKey("SelectedHardware") || ds.Ints["SelectedHardware"] < 0)
            {
                HardwareSelector hws = new HardwareSelector(ds);
                if (hws.ShowDialog() != true)
                {
                    Environment.Exit(0);
                }
            }
            LoadingWindow lw = new LoadingWindow(ds);
            lw.Load();
            
            this.WindowState = ds.Bools["Maximized", false] ? System.Windows.WindowState.Maximized : System.Windows.WindowState.Normal;
            this.HWC = lw.HWC;
            CameraClickAndMove.SetHWC(this.HWC);

            //Init 1 CloseableTabItem
            Data.TabItems_Add(new CloseableTabItem("new"));

            // Init UI Elemente
            AxesControl.SetHardwareControllerAndInitGUI(HWC);
            IOControl.SetHardwareControllerAndInitGUI(HWC);

            //Verbinde EventHandler
            ScriptHandler.OnState1Changed += this.OnScriptHandlerStateChange_1;
            ScriptHandler.OnState2Changed += this.OnScriptHandlerStateChange_2;

            //Data.CodeTextFieldItems = new List<string>();
            //Load last X Commands 
            List<string> s = new List<string>();
            int x = ds.Ints["CodeTextFieldItemsCount", 0];
            for (int i = 0; i < x; ++i)
            {
                s.Add(ds.Strings["CTFI-" + i, ""]);
            }
            Data.CodeTextFieldItems = s;

#if TEST
            Tests.TestClass.RunTest(this, HWC);
#endif
        }

        protected void GUIInit()
        {
#if DEBUG || TEST

            MITest.Visibility = System.Windows.Visibility.Visible;

#else
            MITest.Visibility = System.Windows.Visibility.Collapsed;
#endif 
        }

        #region Threads 

        protected void DisplayFileInfoAndUpdateMenuBars()
        {
            CloseableTabItem cti = null;
            Camera cam = null;
            while (true)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    cti = (CloseableTabItem)ContentTabControl.SelectedItem;

                    Data.RedoEnabled = cti.CanRedo;
                    Data.SavedEnabled = !cti.IsSaved;
                    Data.UndoEnabled = cti.CanUndo;

                }));


                cam = HWC.Camera;
                Data.CameraConnected = cam.IsConnected;

                Thread.Sleep(100);
            }
        }

        protected void DisplayStatusBarInformation()
        {
            PerformanceCounter cpuCounter = new PerformanceCounter("Process", "% Processor Time", Process.GetCurrentProcess().ProcessName);

            PerformanceCounter ramCounter = new PerformanceCounter("Process", "Private Bytes", Process.GetCurrentProcess().ProcessName);
            while (true)
            {
                Data.CPULoad = string.Format("CPU: {0:0}%", cpuCounter.NextValue());

                Data.RAMLoad = string.Format("Mem: {0:0}MB", ramCounter.NextValue()/1000000.0);

                string s = string.Format("Threads: {0} ({1})", Process.GetCurrentProcess().Threads.Count, TrackedThread.Count);
                Data.Threads = s;                

                Thread.Sleep(500);
            }
        }

        #endregion Threads

        
        #region Menu Click Events

        private void CamClickMove_Click(object sender, RoutedEventArgs e)
        {
            //CameraClickAndMove ccam = new CameraClickAndMove(HWC);
            //ccam.ShowDialog();
            //ccam.ShowDialog();            
            CameraClickAndMove.Display();
        }

        private void CloseTabBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Data.TabItems_Remove((CloseableTabItem)this.ContentTabControl.SelectedItem);
        }

        private void MIHardwareSettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            UIWindows.HardwareSettings hs = new UIWindows.HardwareSettings(HWC);
            hs.ShowDialog();
        }

        private void MIHardwareChangeBtn_Click(object sender, RoutedEventArgs e)
        {
            DataSafe ds = new DataSafe(Paths.SettingsPath, "MainWindow");
            HardwareSelector hws = new HardwareSelector(ds);
            if (hws.ShowDialog() == true)
            {
                MessageBox.Show("LaserControl will exit now. \nPlease open it again to apply the hardware change!", "Hardware changed exit", MessageBoxButton.OK, MessageBoxImage.Information);
                Environment.Exit(0);
            }
        }

        private void NewBtn_Click(object sender, RoutedEventArgs e)
        {
            CloseableTabItem cti = new CloseableTabItem("new");            
            this.Data.TabItems_Add(cti);
        }

        private void OpenBtn_Click(object sender, RoutedEventArgs e)
        {
            CloseableTabItem cti = new CloseableTabItem("new_open", true);
            this.Data.TabItems_Add(cti);
        }

        private void PauseBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Data.PauseEnabled)
            {
                ScriptHandler.PauseThread1();
            }
        }

        private void RedoBtn_Click(object sender, RoutedEventArgs e)
        {
            CurrentTab.Redo();
        }

        private void ResumeBtn_Click(object sender, RoutedEventArgs e)
        {
            ScriptHandler.ResumeThread1();
        }

        private void RunBtn_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Run");
            if (Data.RunEnabled)
            {
                CloseableTabItem cur = (CloseableTabItem)ContentTabControl.SelectedItem;
                cur.Run();
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            CloseableTabItem cur = (CloseableTabItem)ContentTabControl.SelectedItem;
            cur.Save();
        }

        private void SaveAllBtn_Click(object sender, RoutedEventArgs e)
        {
            foreach (CloseableTabItem cti in ContentTabControl.Items)
            {
                cti.Save();
            }
        }

        private void SaveToBtn_Click(object sender, RoutedEventArgs e)
        {
            CloseableTabItem cur = (CloseableTabItem)ContentTabControl.SelectedItem;
            cur.SaveTo();
        }

        private void ScriptSettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            UIWindows.ScriptSettings ss = new UIWindows.ScriptSettings();
#warning Show oder ShowDialog ???
            //ss.ShowDialog();
            ss.Show();
        }

        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            ScriptHandler.StopThread1();
        }

        private void StopAllBtn_Click(object sender, RoutedEventArgs e)
        {
            ScriptHandler.StopThread1();
            ScriptHandler.StopThread2();
        }

        private void TFPauseBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Data.TFPauseEnabled)
            {
                ScriptHandler.PauseThread2();
            }
        }

        private void TFResumeBtn_Click(object sender, RoutedEventArgs e)
        {
            ScriptHandler.ResumeThread2();
        }

        private void TFRunBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Data.TFRunEnabled)
            {
                string code = Data.TextFieldCode;

                ScriptHandler.SetCodeThread2(code);

                List<string> s = Data.CodeTextFieldItems;
                if (s == null)
                {
                    s = new List<string>();
                }
                s.Reverse(); 
                if (s.Count == 0)
                {
                    s.Add(code);
                }
                else
                {
                    s.RemoveAll(x => x == code);
                    s.Add(code);
                }
                while (s.Count > 20)
                {
                    s.RemoveAt(0);
                }
                s.Reverse();

                Data.CodeTextFieldItems = new List<string>(s) ;
                s.Clear();
            }
        }

        private void TFStopBtn_Click(object sender, RoutedEventArgs e)
        {
            ScriptHandler.StopThread2();
        }

        private void UndoBtn_Click(object sender, RoutedEventArgs e)
        {
            CurrentTab.Undo();
        }

        #endregion       

        private void ImgMenuItem_Click(object sender, RoutedEventArgs e)
        {            
            Camera c = HWC.Camera;
            if (c.IsConnected)
            {
                c.SetDisplayingImageType(2);
                c.SetOverlay(false, true, true);
                c.SetThreshold(128);
                c.SetProcessing(true);
            }
        }

        private void SBIThreads_Click(object sender, MouseButtonEventArgs e)
        {
            ThreadsWindow tw = new ThreadsWindow();
            tw.Show();
        }

        private void TBCodeTextField_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                TFRunBtn_Click(sender, new RoutedEventArgs());
            }
            //e.Handled = true;
        }

       

        

        

        

    }
}
