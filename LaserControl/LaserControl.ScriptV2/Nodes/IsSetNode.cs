using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LaserControl.ScriptV2.Nodes
{
    public class IsSetNode<T> : ContentNode<bool> 
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
            }
        }

        protected VariableNode<T> VarNode = null;

        public IsSetNode(VariableNode<T> node)
            : base()
        {
            VarNode = node;
        }

        protected override void InnerEvaluate()
        {
            this.Content = VarNode.IsSet;
        }

        public override void Clean()
        {
            base.Clean();
            this.VarNode = null;
        } 
    }
}
