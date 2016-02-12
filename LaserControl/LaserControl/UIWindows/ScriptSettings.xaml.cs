using LaserControl.ScriptV2;
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
using System.Windows.Shapes;

namespace LaserControl.UIWindows
{

    public class FunctionDataHandler : INotifyPropertyChanged
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

        protected string _Code = "";
        public string Code
        {
            get { return _Code; }
            set
            {
                this._Code = value;
                this.NotifyPropertyChanged("Code");
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


        protected int _ReturnIndex;
        public int ReturnIndex
        {
            get { return _ReturnIndex; }
            set
            {
                this._ReturnIndex = value;
                this.NotifyPropertyChanged("ReturnIndex");
            }
        }

        protected List<ParameterGridEntry> _Parameters;
        public List<ParameterGridEntry> Parameters
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

    public class ParameterGridEntry
    {
        public string ParamType { get; set; }
        public string ParamName { get; set; }
    }

    /// <summary>
    /// Interaktionslogik für ScriptSettings.xaml
    /// </summary>
    public partial class ScriptSettings : Window
    {
        Function currentFunction = null;
        FunctionDataHandler Data;


        List<string> ParameterType = new List<string>();

        public ScriptSettings()
        {


            InitializeComponent();


            Data = FindResource("datahandler") as FunctionDataHandler;


            ParameterType.Add("bool");
            ParameterType.Add("int");
            ParameterType.Add("double");
            ParameterType.Add("string");
            ParamTypeColumn.ItemsSource = ParameterType;

            TVStringItem[] tvsi = FunctionLib.GetTreeViewNodes();
            foreach (var i in tvsi)
            {
                AllFunctions.Items.Add(i);
            }

        }

        private void NewCategoryBtn_Click(object sender, RoutedEventArgs e)
        {
            string res = Dialogs.Prompts.ShowDialog_Text("New category", "Name:", "");
            if (string.IsNullOrEmpty(res) || string.IsNullOrWhiteSpace(res))
                return;

            FunctionLib.AddCategory(new FunctionCategory(res));
            FunctionLib.Save();

            AllFunctions.Items.Clear();
            TVStringItem[] tvsi = FunctionLib.GetTreeViewNodes();
            foreach (var i in tvsi)
            {
                AllFunctions.Items.Add(i);
            }
        }

        private void NewFunctionBtn_Click(object sender, RoutedEventArgs e)
        {
            string name = "";
            List<string> items = new List<string>();
            TVStringItem[] tvsi = FunctionLib.GetTreeViewNodes();
            for (int i = 1; i < tvsi.Length; ++i)
            {
                items.Add(tvsi[i].Title);
            }

            string selCat = "";
            Dialogs.Prompts.ShowDialog_Text_Combo("New Function", "Name:", "Category", ref name, items, out selCat);
            items.Clear();

            if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
                return;
            if (string.IsNullOrEmpty(selCat) || string.IsNullOrWhiteSpace(selCat))
                return;

            Function f = new Function(name, "");
            FunctionLib.AddFunction(selCat, f);


            FunctionLib.Save();
            AllFunctions.Items.Clear();
            tvsi = FunctionLib.GetTreeViewNodes();
            foreach (var i in tvsi)
            {
                AllFunctions.Items.Add(i);
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (currentFunction != null)
            {
                currentFunction.Reset(CodeEdit.Code, Function.IndexToType(Data.ReturnIndex));
                foreach (var p in Data.Parameters)
                {
                    if (!string.IsNullOrEmpty(p.ParamName) && !string.IsNullOrWhiteSpace(p.ParamName) && !string.IsNullOrEmpty(p.ParamType) && !string.IsNullOrWhiteSpace(p.ParamType))
                    {
                        currentFunction.AddParameter(p.ParamName, Function.StringToType(p.ParamType));
                    }
                }
                currentFunction.Save();
            }
        }

        private void AllFunctions_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TVStringItem item = e.NewValue as TVStringItem;
            if (item != null && item.IsFunction)
            {
                currentFunction = FunctionLib.GetFunction(item.FunctionName);

                Data.Name = currentFunction.FunctionName;
                Data.ReturnIndex = Function.TypeToIndex(currentFunction.ReturnType);
                Data.Code = currentFunction.Code;

                //Set parameter grid !
                List<ParameterGridEntry> p = new List<ParameterGridEntry>();
                for (int i = 0; i < currentFunction.ParameterCount; ++i)
                {
                    p.Add(new ParameterGridEntry()
                    {
                        ParamName = currentFunction.GetParamName(i),
                        ParamType = Function.TypeToString(currentFunction.GetParamtype(i))
                    });
                }
                List<ParameterGridEntry> old = Data.Parameters;
                Data.Parameters = p;
                if(old != null)
                    old.Clear();

                Data.RightVisibility = System.Windows.Visibility.Visible;
            }
            else
            {
                currentFunction = null;
                Data.RightVisibility = System.Windows.Visibility.Hidden;
            }
        }
    }
}
