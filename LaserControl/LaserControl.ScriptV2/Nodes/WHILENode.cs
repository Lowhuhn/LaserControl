using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LaserControl.ScriptV2.Nodes
{
    public class WHILENode : Node
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
                foreach (Node n in Whilenodes)
                {
                    n.ScriptThreadID = value;
                }
            }
        }

        protected ContentNode<bool> Decider = null;
        protected List<Node> Whilenodes;

        public WHILENode(ContentNode<bool> decider, List<Node> whilenodes) : base()
        {
            Decider = decider;
            Whilenodes = whilenodes;
        }

        protected override void InnerEvaluate()
        {
            bool runner = false;
            Decider.Evaluate(out runner);
            while (runner)
            {
                foreach (var n in Whilenodes)
                {
                    bool s = false;
                    n.Evaluate(out s);
                }
                Decider.Evaluate(out runner);
            }
        }

        public override void Clean()
        {
            base.Clean();

            Decider.Clean();
            Decider = null;

            foreach (var n in Whilenodes)
            {
                n.Clean();
            }
            Whilenodes.Clear();
            Whilenodes = null;
        }
    }
}
