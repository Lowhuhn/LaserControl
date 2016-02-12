using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using LaserControl.Library;
using LaserControl.ScriptV2.Nodes;

using LaserControl.ScriptV2.Exceptions;

namespace LaserControl.ScriptV2
{
    public enum ScriptThreadState : int
    {
        Waiting = 0,
        Running = 1,
        Paused = 2
    }

    public class ScriptHandler
    {
        static ScriptHandler()
        {
            EventThread = new TrackedThread("Script Handler State Event Thread", StateChangeEventThread);
            EventThread.Start();

            Node.OnPaused += OnPaused;

            Thread_1 = new TrackedThread("Script Thread 1", Thread_1_Handle, true);
            //Thread_1.ApartmentState = ApartmentState.STA;
            Thread_1.Start();

            Thread_2 = new TrackedThread("Script Thread 2", Thread_2_Handle, true);
            Thread_2.Start();

            
        }

        protected static TrackedThread EventThread;


        //Alll for Thread 1
        protected static TrackedThread Thread_1;
        protected static Node Thread_1_LocalNode = null;
        public static ScriptThreadState State_1
        {
            get;
            protected set;
        }
        public static ScriptThreadStateEvent OnState1Changed;
        protected static ParserExceptionEvent Thread_1_ParserException;
        protected static LineChangeEvent Thread_1_LineChangeEvent
        {
            get
            {
                return CurrentLineNode.OnCurrentLineNodeEvaluate;
            }
            set
            {
                CurrentLineNode.OnCurrentLineNodeEvaluate = value;
            }
        }

        //All for Thread 2
        protected static TrackedThread Thread_2;
        public static ScriptThreadState State_2
        {
            get;
            protected set;
        }
        public static ScriptThreadStateEvent OnState2Changed;


        #region Thread 1

        protected static Function Thread_1_Obj = null;

        protected static void Thread_1_Handle()
        {
            State_1 = ScriptThreadState.Waiting;
            while (true)
            {
                //Warte bis ein object an den Thread übergeben wird
                while (Thread_1_Obj == null)
                    Thread.Sleep(1);

                State_1 = ScriptThreadState.Running;

                Function local = null;
                lock (Thread_1_Obj)
                {
                    local = Thread_1_Obj;
                }

                Thread_1_LocalNode = local.GetFunctionNode(new List<Node>(), true);
                Thread_1_LocalNode.ScriptThreadID = 1;

                bool b = false;
                try
                {
                    Thread_1_LocalNode.Evaluate(out b);
                }
                catch(Exception ex)
                {
                    if (ex.GetType() == typeof(ParserException))
                    {
                        Console.WriteLine(ex.Message);
                        if (Thread_1_ParserException != null)
                        {
                            Thread_1_ParserException( (ParserException) ex);
                        }
                        Thread_1_ParserException -= Thread_1_ParserException;
                    }                   
                }
                Thread_1_LocalNode.Clean();

                local.Clear();
                lock (Thread_1_Obj)
                {
                    Thread_1_Obj = null;
                }

                State_1 = ScriptThreadState.Waiting;
                //Warte am ende 1ms 
                Thread.Sleep(1);
            }
        }

        public static void SetCodeThread1(string code, ParserExceptionEvent exReturn = null, LineChangeEvent evNewLine = null)
        {            
            if (Thread_1_Obj == null)
            {               
                Thread_1_Obj = new Function("MAIN", code);
                
                //Clear Parser Exception event
                Thread_1_ParserException -= Thread_1_ParserException;

                //Assign new Event
                Thread_1_ParserException += exReturn;

                Thread_1_LineChangeEvent -= Thread_1_LineChangeEvent; 
                Thread_1_LineChangeEvent += evNewLine; 
            }
            else
            {
                throw new Exception("Wait for program to end! Then start a new one!");
            }
        }

        public static void PauseThread1()
        {
            if(State_1 == ScriptThreadState.Running)
                Node.SetPaused(1,true);
        }

        public static void ResumeThread1()
        {
            if (State_1 == ScriptThreadState.Paused)
            {
                State_1 = ScriptThreadState.Running;
                Node.SetPaused(1,false);
                return;
            }
        }

        public static void StopThread1()
        {
            if (State_1 != ScriptThreadState.Waiting)
            {
                try
                {                    
                    Thread_1.Thread.Abort();

                    if (Thread_1_LocalNode != null)
                    {
                        Thread_1_LocalNode.Clean();
                    }

                    if(Thread_1_Obj != null)
                    {                        
                        lock (Thread_1_Obj)
                        {
                            Thread_1_Obj.Clear();
                            Thread_1_Obj = null;
                        }
                    }
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    Thread_1 = new TrackedThread("Script Thread 1", Thread_1_Handle, true);
                    Thread_1.Start();

                    //Reset pausing state
                    Node.SetPaused(1,false);

                    //Reset Line if there was a Event method connected
                    if (Thread_1_LineChangeEvent != null)
                    {
                        Thread_1_LineChangeEvent(-1);
                    }
                }
                catch
                {
#warning Implement error handling
                }
            }
        }

        #endregion //Thread 1

        #region Thread 2

        protected static Function Thread_2_Obj = null;

        protected static void Thread_2_Handle()
        {
            State_2 = ScriptThreadState.Waiting;
            while (true)
            {
                //Warte bis ein object an den Thread übergeben wird
                while (Thread_2_Obj == null)
                    Thread.Sleep(1);

                State_2 = ScriptThreadState.Running;

                Function local = null;
                lock (Thread_2_Obj)
                {
                    local = Thread_2_Obj;
                }

                Node localNode = local.GetFunctionNode(new List<Node>());
                localNode.ScriptThreadID = 2;

                bool b = false;
                localNode.Evaluate(out b);
                localNode.Clean();

                local.Clear();
                lock (Thread_2_Obj)
                {
                    Thread_2_Obj = null;
                }

                State_2 = ScriptThreadState.Waiting;
                //Warte am ende 1ms 
                Thread.Sleep(1);
            }
        }

        public static void SetCodeThread2(string code)
        {
            if (Thread_2_Obj == null)
            {
                Thread_2_Obj = new Function("MAIN", code);
            }
            else
            {
                throw new Exception("Wait for program to end! Then start a new one!");
            }
        }

        public static void PauseThread2()
        {
            if (State_2 == ScriptThreadState.Running)
                Node.SetPaused(2, true);
        }

        public static void ResumeThread2()
        {
            if (State_2 == ScriptThreadState.Paused)
            {
                State_2 = ScriptThreadState.Running;
                Node.SetPaused(2, false);
                return;
            }
        }

        public static void StopThread2()
        {
            if (State_2 != ScriptThreadState.Waiting)
            {
                try
                {
                    Thread_2.Thread.Abort();
                    if (Thread_2_Obj != null)
                    {
                        lock (Thread_2_Obj)
                        {
                            Thread_2_Obj = null;
                        }
                    }
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    Thread_2 = new TrackedThread("Script Thread 2", Thread_2_Handle, true);
                    Thread_2.Start();
                    Node.SetPaused(2, false);
                }
                catch
                {
#warning Implement error handling
                }
            }
        }

        #endregion //Thread 2

        protected static void StateChangeEventThread()
        {
            ScriptThreadState state1 = 0;
            ScriptThreadState state2 = 0;
            while (true)
            {
                if (State_1 != state1)
                {
                    state1 = State_1;
                    if (OnState1Changed != null)
                        OnState1Changed(State_1);                    
                }
                if (State_2 != state2)
                {
                    state2 = State_2;
                    if (OnState2Changed != null)
                        OnState2Changed(State_2);
                }
                Thread.Sleep(1);
            }
        }

        protected static void OnPaused(int id)
        {
            switch (id)
            {
                case 1:
                    State_1 = ScriptThreadState.Paused;
                    break;
                case 2:
                    State_2 = ScriptThreadState.Paused;
                    break;
                default:
                    break;
            }
            //State_1 = ScriptThreadState.Paused;
        }

        //CALLC("LaserControl.ScriptV2.ScriptHandler", "StopThread", 1);
        public static void StopThread(int i)
        {
            if (i == 1)
            {
                TrackedThread tt = new TrackedThread("Kill Script Thread 1", () =>
                {
                    StopThread1();
                });
                tt.Start();
            }
            if (i == 2)
            {
                TrackedThread tt = new TrackedThread("Kill Script Thread 2", () =>
                {
                    StopThread2();
                });
                tt.Start();
            }
        }

    }
}
