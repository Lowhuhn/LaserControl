using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace LaserControl.ScriptV2
{
    public class TVStringItem
    {
        public TVStringItem()
        {
            this.Items = new ObservableCollection<TVStringItem>();
            IsFunction = false;
        }
        public string Title { get; set; }
        public string FunctionName {get; set;}
        public bool IsFunction {get; set;}
        public ObservableCollection<TVStringItem> Items { get; set; }
    }
}
