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

using LaserControl.Library;

namespace LaserControl.UIElements
{
    public class UI_OutputControlDataHandler : INotifyPropertyChanged
    {
        protected string _Text;
        public string Text
        {
            get { return _Text; }
            set
            {
                _Text = value;
                NotifyPropertyChanged("Text");
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
    /// Interaktionslogik für UI_OutputControl.xaml
    /// </summary>
    public partial class UI_OutputControl : UserControl
    {
        protected UI_OutputControlDataHandler Data;
        protected bool NextIsLineBeginning;

        public UI_OutputControl()
        {
            InitializeComponent();
            Data = FindResource("datahandler") as UI_OutputControlDataHandler;

            this.NextIsLineBeginning = true;

            GlobalEvents.OnWrite += Write;
            GlobalEvents.OnClear += Clear;
            
        }

        public void Clear()
        {
            Data.Text = "";
            this.NextIsLineBeginning = true;
        }

        public void Write(string txt)
        {

            if (NextIsLineBeginning)
            {
                Data.Text += DateTime.Now.ToString("[dd.MM.yyyy - HH:mm:ss] ");
                this.NextIsLineBeginning = false;
            }


            Data.Text += txt;
            TBOutput.Dispatcher.Invoke(new Action(() =>
            {
                TBOutput.ScrollToEnd();
                TBOutput.ScrollToEnd();
            }));

            this.NextIsLineBeginning = txt.EndsWith("\n");
            

            /*
            string[] lines = txt.Split('\n');

            

            string s = txt;
            Data.Text += s;
            TBOutput.Dispatcher.Invoke(new Action(()=>{
                TBOutput.ScrollToEnd();
                TBOutput.ScrollToEnd(); 
            }));
            */
        }

    }
}
