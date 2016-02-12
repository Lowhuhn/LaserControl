using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LaserControl.ScriptV2.Nodes
{
    public class FORNode : Node
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
                InitNode.ScriptThreadID = value;
                Decider.ScriptThreadID = value;
                Iteration.ScriptThreadID = value;

                foreach (Node n in ForNodes)
                {
                    n.ScriptThreadID = value;
                }
            }
        }

        protected Node InitNode = null;
        protected ContentNode<bool> Decider = null;
        protected Node Iteration = null;

        protected List<Node> ForNodes;

        public FORNode(Node init, ContentNode<bool> decider, Node iter, List<Node> fornodes) : base()
        {
            InitNode = init;
            Decider = decider;
            Iteration = iter;

            ForNodes = fornodes;
        }

        protected override void InnerEvaluate()
        {
            bool runner = false;
            bool nix = false;
            InitNode.Evaluate(out nix);

            Decider.Evaluate(out runner);
            while (runner)
            {

                foreach (var n in ForNodes)
                {
                    bool s = false;
                    n.Evaluate(out s);
                }    

                Iteration.Evaluate(out nix);
                Decider.Evaluate(out runner);
            }

        }

        public override void Clean()
        {
            base.Clean();
            InitNode.Clean();
            InitNode = null;

            Decider.Clean();
            Decider = null;

            Iteration.Clean();
            Iteration = null;

            foreach (var n in ForNodes)
            {
                n.Clean();
            }
            ForNodes.Clear();
        }
    }
}
