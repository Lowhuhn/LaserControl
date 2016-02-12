using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LaserControl.Library;

namespace LaserControl.ScriptV2.Nodes
{
    public class CurrentLineNode : ContentNode<int>
    {

        public static LineChangeEvent OnCurrentLineNodeEvaluate;

        public CurrentLineNode(int line)
            : base(line)
        {

        }

        protected override void InnerEvaluate()
        {
            base.InnerEvaluate();
            //GlobalEvents.WriteLine(this.Content+" - "+this.ScriptThreadID);
            if (OnCurrentLineNodeEvaluate != null)
            {
                OnCurrentLineNodeEvaluate(this.Content);
            }
        }

    }
}
