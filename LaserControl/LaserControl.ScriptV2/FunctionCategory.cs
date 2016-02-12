using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LaserControl.ScriptV2
{
    public class FunctionCategory : IComparable
    {
        public string Title
        {
            get;
            set;
        }

        public List<string> Functions
        {
            get;
            set;
        }

        public FunctionCategory()
        {
            Functions = new List<string>();
        }

        public FunctionCategory(string name):this()
        {
            Title = name.ToUpper();
        }

        public void Sort()
        {
            Functions.Sort();
        }

        public int CompareTo(object obj)
        {
            FunctionCategory f = obj as FunctionCategory;

            return String.Compare(this.Title, f.Title);
        }
    }
}
