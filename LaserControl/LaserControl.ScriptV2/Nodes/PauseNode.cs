using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LaserControl.ScriptV2.Nodes
{
    public class PauseNode : Node
    {

        protected override void InnerEvaluate()
        {
            //base.InnerEvaluate();
            //Console.WriteLine("My Pause ID is: {0}", this.ScriptThreadID);
            Node.SetPaused(this.ScriptThreadID, true);
            while (IsPaused)
            {
                Thread.Sleep(100);
            }
        }

        public override void Clean()
        {
            base.Clean();
        }
    }
}
