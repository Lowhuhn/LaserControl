using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserControl.ScriptV2.Nodes
{
    public class IFNode : Node
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
                Decider.ScriptThreadID = value;
                foreach (Node n in IfNodes)
                {
                    n.ScriptThreadID = value;
                }

                if (ElseNodes != null)
                {
                    foreach (Node n in ElseNodes)
                    {
                        n.ScriptThreadID = value;
                    }
                }
            }
        }

        protected ContentNode<bool> Decider;
        protected List<Node> IfNodes;
        protected List<Node> ElseNodes;

        public IFNode(ContentNode<bool> decider, List<Node> ifnodes)
        {
            Decider = decider;
            IfNodes = ifnodes;
            ElseNodes = null;
        }

        public IFNode(ContentNode<bool> decider, List<Node> ifnodes, List<Node> elsenodes)
            : this(decider, ifnodes)
        {
            ElseNodes = elsenodes;
        }

        protected override void InnerEvaluate()
        {
            bool b = false;
            Decider.Evaluate(out b);
            if (b)
            {
                foreach (var n in IfNodes)
                {                   
                    bool s = false;
                    n.Evaluate(out s);
                }         
            }
            else
            {
                //Else Zweig
                if (ElseNodes != null)
                {
                    foreach (var n in ElseNodes)
                    {
                        bool s = false;
                        n.Evaluate(out s);
                    }                    
                }
            }
        }

        public override void Clean()
        {
            base.Clean();
            this.IfNodes.Clear();
            foreach (var n in IfNodes)
            {
                n.Clean();
            }
            if (ElseNodes != null)
            {
                this.ElseNodes.Clear();
                foreach (var n in ElseNodes)
                {
                    n.Clean();
                }
            }
        }
    }
}
