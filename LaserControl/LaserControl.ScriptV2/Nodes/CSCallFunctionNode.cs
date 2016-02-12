using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using LaserControl.Library;

namespace LaserControl.ScriptV2.Nodes
{
    public class CSCallFunctionNode<T> : ContentNode<T>
        where T : IConvertible
    {
        protected bool IsStaticFunctionCall = true;

        protected object ObjectToCall = null;
        protected MethodInfo Method;
        protected List<Node> Parameter;


        public CSCallFunctionNode(MethodInfo method, List<Node> parameter)
            : base()
        {
            IsContentNode = true;
            Method = method;
            Parameter = parameter;
        }

        public CSCallFunctionNode(object objectToCall, MethodInfo method, List<Node> parameter)
            : this(method, parameter)
        {
            ObjectToCall = objectToCall;
        }

        protected override void InnerEvaluate()
        {
            base.InnerEvaluate();

            object[] p = new object[Parameter.Count];
            for (int i = 0; i < Parameter.Count; ++i)
            {
                if (Parameter[i].ContentType == typeof(int))
                {
                    int xI = 0;
                    Parameter[i].Evaluate(out xI);
                    p[i] = xI;
                }
                else if (Parameter[i].ContentType == typeof(double))
                {
                    double xD = 0;
                    Parameter[i].Evaluate(out xD);
                    p[i] = xD;
                }
                else if (Parameter[i].ContentType == typeof(string))
                {
                    string xD = "";
                    Parameter[i].Evaluate(out xD);
                    p[i] = xD;
                }
                else if (Parameter[i].ContentType == typeof(bool))
                {
                    bool xD = false;
                    Parameter[i].Evaluate(out xD);
                    p[i] = xD;
                }
            }
            if (Method.ReturnType == typeof(void))
            {

                Method.Invoke(ObjectToCall, p);
                Content = (T)(object)true;

            }
            else
            {
                Content = (T)(object)Method.Invoke(ObjectToCall, p);
            }

        }


        public override void Clean()
        {
            base.Clean();
            ObjectToCall = null;
            Method = null;
            Parameter.Clear();
            Parameter = null;
        }
    }
}
