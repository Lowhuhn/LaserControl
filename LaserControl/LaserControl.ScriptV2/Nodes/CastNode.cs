using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserControl.ScriptV2.Nodes
{
    class CastNode<T,O> : ContentNode<T> 
        where T : IConvertible 
        where O : IConvertible
    {
        public override int ScriptThreadID
        {
            get
            {
                return base.ScriptThreadID;
            }
            set
            {
                base.ScriptThreadID = value;
                OrgContentNode.ScriptThreadID = value;
            }
        }

        protected ContentNode<O> OrgContentNode;

        public CastNode(ContentNode<O> org)
        {
            OrgContentNode = org;
        }

        protected override void InnerEvaluate()
        {
            if (typeof(T) == typeof(int))
            {
                int i = 0;
                OrgContentNode.Evaluate(out i);                
                this.Content = (T)(object)i;
            }
            else if (typeof(T) == typeof(double))
            {
                double d = 0;
                OrgContentNode.Evaluate(out d);
                this.Content = (T)(object)d;
            }else if(typeof(T) == typeof(string)){
                string s = "";
                OrgContentNode.Evaluate(out s);
                this.Content = (T)(object)s;
            }
            else if (typeof(T) == typeof(bool))
            {
                if (typeof(O) == typeof(int))
                {
                    int i = 0;
                    OrgContentNode.Evaluate(out i);
                    this.Content = (T)(object)(i != 0);
                }
                else if (typeof(O) == typeof(double))
                {
                    double d = 0;
                    OrgContentNode.Evaluate(out d);
                    this.Content = (T)(object)(Math.Abs(d) > Double.Epsilon);
                }
                else
                {
                    bool b = false;
                    OrgContentNode.Evaluate(out b);
                    this.Content = (T)(object)b;
                }
            }
        }


        public override void Clean()
        {
            base.Clean();
            OrgContentNode.Clean();
            OrgContentNode = null;
        }
    }
}
