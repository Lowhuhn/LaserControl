using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LaserControl.ScriptV2.Nodes
{
    public class NodeCollection : Node
    {

        protected List<Node> InnerNodes;



        public NodeCollection()
        {
            InnerNodes = new List<Node>();
        }

        public void Add(Node n)
        {
            InnerNodes.Add(n);
        }

        protected override void InnerEvaluate()
        {
            bool b = false;
            foreach (Node n in InnerNodes)
            {
                n.Evaluate(out b);
                n.Clean();
            }
            
        }
    }
}
