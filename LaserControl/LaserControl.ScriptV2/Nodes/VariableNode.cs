using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserControl.ScriptV2.Nodes
{
    public class VariableNode<T> : ContentNode<T>
        where T : IConvertible
    {
        public string VariableName
        {
            get;
            protected set;
        }

        public bool IsSet
        {
            get;
            protected set;
        }

        public VariableNode(string name) : base()
        {
            VariableName = name;
            IsSet = false;
        }

        public VariableNode(string name, T initial)
            : this(name)
        {
            Content = initial;
            IsSet = false;
        }

        public VariableNode(string name, T initial, bool isset) : this(name, initial)
        {
            IsSet = isset;
        }

        public void SetValue(int val)
        {
            IsSet = true;
            Content = (T)(object)val;
        }

        public void SetValue(double val)
        {
            IsSet = true;
            Content = (T)(object)val;
        }

        public void SetValue(string val)
        {
            IsSet = true;
            Content = (T)(object)val;
        }

        public void SetValue(bool val)
        {
            IsSet = true;
            Content = (T)(object)val;
        }
    }
}
