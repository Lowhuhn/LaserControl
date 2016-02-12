using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LaserControl.ScriptV2.Nodes
{
    public class Node
    {
        #region Static Propertys For Pause etc...

        //public static bool Paused = false;
        public static Dictionary<int, bool> Paused = new Dictionary<int, bool>();
        protected int _ScriptThreadID = -1;
        public virtual int ScriptThreadID
        {
            get
            {
                return _ScriptThreadID;
            }
            set
            {
                _ScriptThreadID = value;
            }
        }

        public bool IsPaused
        {
            get
            {
                if(!Paused.ContainsKey(this._ScriptThreadID))
                {
                    Paused[_ScriptThreadID] = false;
                }
                return Paused[_ScriptThreadID];
            }
        }

        //public static VoidEvent OnPaused;
        public static IntEvent OnPaused;

        #endregion // Static Propertys For Pause etc...

        public bool IsContentNode
        {
            get;
            protected set;
        }

        public virtual Type ContentType
        {
            get
            {
                return typeof(bool);
            }
        }

        public Node()
        {
            IsContentNode = false;
        }

        protected virtual void InnerEvaluate()
        {
            //Nothing to do here!!!
            //Content = Content
        }

        public virtual void Evaluate(out bool b)
        {
            //Node.PauseWait();
            InnerEvaluate();
            b = false;
        }

        public virtual void Evaluate(out int i)
        {
           // Node.PauseWait();
            InnerEvaluate();
            i = 0;
        }

        public virtual void Evaluate(out string s)
        {
           // Node.PauseWait();
            InnerEvaluate();
            s= "";
        }

        public virtual void Evaluate(out double d)
        {
          //  Node.PauseWait();
            InnerEvaluate();
            d = 0;
        }

        public virtual void Clean()
        {
            //Aufräumen
        }

        #region Static Methods For Pause etc...

        
        public static void SetPaused(int id, bool val)
        {
            Paused[id] = val;
            if (OnPaused != null && val == true)
            {
                OnPaused(id);
            }
        }

        /*
        public static void PauseWait()
        {
            while (Paused)
            {
                Thread.Sleep(100);
            }
        }
        */

        #endregion // Static Methods For Pause etc...
    }
}
