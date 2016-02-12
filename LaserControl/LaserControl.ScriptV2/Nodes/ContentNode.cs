using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserControl.ScriptV2.Nodes
{
    public class ContentNode<T> : Node
        where T : IConvertible
    {

        public T Content
        {
            get;
            protected set;
        }

        public override Type ContentType
        {
            get
            {
                return typeof(T);
            }
        }

        public ContentNode()
        {
            IsContentNode = true;
        }

        public ContentNode(T content) : this()
        {
            Content = content;
        }        

        public override void Evaluate(out int i)
        {
      //      Node.PauseWait();
            InnerEvaluate();
            i = Content.ToInt32(NumberFormatInfo.CurrentInfo);
        }

        public override void Evaluate(out bool b)
        {
    //        Node.PauseWait();
            InnerEvaluate();
            b = Content.ToBoolean(NumberFormatInfo.CurrentInfo);
        }

        public override void Evaluate(out string s)
        {
  //          Node.PauseWait();
            InnerEvaluate();
            s = Content.ToString();
        }

        public override void Evaluate(out double d)
        {
//            Node.PauseWait();
            InnerEvaluate();
            d = Content.ToDouble(NumberFormatInfo.CurrentInfo);
        }
    }
}
