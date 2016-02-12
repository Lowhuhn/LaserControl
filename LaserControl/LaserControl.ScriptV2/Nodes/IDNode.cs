using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LaserControl.ScriptV2.Nodes
{
    public class IDNode : ContentNode<int>
    {

        protected override void InnerEvaluate()
        {
            Content = this.ScriptThreadID;
        }

    }
}
