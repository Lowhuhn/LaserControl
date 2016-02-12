using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserControl.ScriptV2.Nodes
{
    public class VariableAssignmentNode<T> : ContentNode<bool>
        where T : IConvertible
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
                VarNode.ScriptThreadID = value;
                EvalNode.ScriptThreadID = value;
            }
        }

        protected VariableNode<T> VarNode;

        protected Node EvalNode;

        public VariableAssignmentNode(VariableNode<T> varNode, Node evalNode) : base()
        {
            this.Content = false;
            this.VarNode = varNode;
            this.EvalNode = evalNode;
        }

        protected override void InnerEvaluate()
        {
            if (typeof(T) == typeof(int))
            {
                int i = 0;
                EvalNode.Evaluate(out i);
                VarNode.SetValue(i);
                Content = true;
                return;
            }

            if (typeof(T) == typeof(double))
            {
                double d = 0;
                EvalNode.Evaluate(out d);
                VarNode.SetValue(d);
                Content = true;
                return;
            }

            if (typeof(T) == typeof(bool))
            {
                bool d = false;
                EvalNode.Evaluate(out d);
                VarNode.SetValue(d);
                Content = true;
                return;
            }
        }

        public override void Clean()
        {
            base.Clean();
            VarNode.Clean();
            VarNode = null;
            EvalNode.Clean();
            EvalNode = null;
        }
    }
}
