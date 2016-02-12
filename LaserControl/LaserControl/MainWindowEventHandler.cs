using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using LaserControl.Data;
using LaserControl.ScriptV2;

namespace LaserControl
{
    public partial class MainWindow
    {

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DataSafe ds = new DataSafe(Paths.SettingsPath, "MainWindow.xml");

            ds.Bools["Maximized"] = this.WindowState == System.Windows.WindowState.Maximized;


            List<string> s = Data.CodeTextFieldItems;
            if (s == null)            
                s = new List<string>();            
            ds.Ints["CodeTextFieldItemsCount"] = s.Count;
            for (int i = 0; i < s.Count; ++i)
            {
                ds.Strings["CTFI-" + i] = s[i];
            }
            
        }


        private void OnScriptHandlerStateChange_1(ScriptThreadState state)
        {
            switch (state)
            {
                case ScriptThreadState.Waiting:
                    Data.RunEnabled = true;
                    Data.PauseEnabled = false;
                    Data.ResumeEnabled = false;

                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        Data.StatusBarBackground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 122, 204));
                    }));
                    

                    break;
                case ScriptThreadState.Running:
                    Data.RunEnabled = false;
                    Data.PauseEnabled = true;
                    Data.ResumeEnabled = false;

                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        Data.StatusBarBackground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(202, 81, 0));
                    }));
                    

                    break;
                case ScriptThreadState.Paused:
                    Data.RunEnabled = false;
                    Data.PauseEnabled = false;
                    Data.ResumeEnabled = true;
                    break;
            }
        }

        private void OnScriptHandlerStateChange_2(ScriptThreadState state)
        {
            
            switch (state)
            {
                case ScriptThreadState.Waiting:
                    Data.TFRunEnabled = true;
                    Data.TFPauseEnabled = false;
                    Data.TFResumeEnabled = false;
                    break;
                case ScriptThreadState.Running:
                    Data.TFRunEnabled = false;
                    Data.TFPauseEnabled = true;
                    Data.TFResumeEnabled = false;
                    break;
                case ScriptThreadState.Paused:
                    Data.TFRunEnabled = false;
                    Data.TFPauseEnabled = false;
                    Data.TFResumeEnabled = true;
                    break;
            }            
        }

    }
}
