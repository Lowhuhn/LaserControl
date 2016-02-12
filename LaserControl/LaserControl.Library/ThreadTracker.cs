using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserControl.Library
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Threading;

    public class TrackedThread
    {
        private static readonly IList<Thread> threadList = new List<Thread>();

        private readonly Thread thread;

        private readonly ParameterizedThreadStart start1;

        private readonly ThreadStart start2;

        public string Name
        {
            get;
            private set;
        }

        private TrackedThread(string name)
        {
            Console.WriteLine("Thread: {0}", name);
            GlobalEvents.RaiseNewInformationEvent("Thread: " + name, "TrackedThread");
            Name = name;
        }

        public TrackedThread(string name, ParameterizedThreadStart start):this(name)
        {
            this.start1 = start;
            this.thread = new Thread(this.StartThreadParameterized);
            lock (threadList)
            {
                threadList.Add(this.thread);
            }
        }

        public TrackedThread(string name, ThreadStart start, bool big = false)
            : this(name)
        {
            this.start2 = start;
            if (big)
            {
                //10 mb
                this.thread = new Thread(this.StartThread);
            }
            else
            {
                this.thread = new Thread(this.StartThread);
            }
            lock (threadList)
            {
                threadList.Add(this.thread);
            }
        }

        public TrackedThread(string name, ParameterizedThreadStart start, int maxStackSize)
            : this(name)
        {
            this.start1 = start;
            this.thread = new Thread(this.StartThreadParameterized, maxStackSize);
            lock (threadList)
            {
                threadList.Add(this.thread);
            }
        }

        public TrackedThread(string name, ThreadStart start, int maxStackSize)
            : this(name)
        {
            this.start2 = start;
            this.thread = new Thread(this.StartThread, maxStackSize);
            lock (threadList)
            {
                threadList.Add(this.thread);
            }
        }

        public static int Count
        {
            get
            {
                lock (threadList)
                {
                    return threadList.Count;
                }
            }
        }

        public static IEnumerable<Thread> ThreadList
        {
            get
            {
                lock (threadList)
                {
                    return new ReadOnlyCollection<Thread>(threadList);
                }
            }
        }

        // either: (a) expose the thread object itself via a property or,
        // (b) expose the other Thread public methods you need to replicate.
        // This example uses (a).
        public Thread Thread
        {
            get
            {
                return this.thread;
            }
        }

        private void StartThreadParameterized(object obj)
        {
            try
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
                this.start1(obj);
            }
            finally
            {
                lock (threadList)
                {
                    threadList.Remove(this.thread);
                }
            }
        }

        private void StartThread()
        {
            try
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
                this.start2();
            }
            finally
            {
                lock (threadList)
                {
                    threadList.Remove(this.thread);
                }
            }
        }

        public void Start(bool background = true)
        {
            this.thread.IsBackground = background;
            this.thread.Start();
        }

        public void Start(object o, bool background = true)
        {
            this.thread.IsBackground = background;
            this.thread.Start(o);
        }

        public void Abort()
        {
            if (this.thread.IsAlive)
            {
                try
                {
                    this.thread.Abort();
                    this.thread.Interrupt();
                }
                catch
                {
                    //Nix
                }
            }
        }

        public ApartmentState ApartmentState
        {
            get
            {
                return this.thread.GetApartmentState();
            }
            set
            {
                this.thread.SetApartmentState(value);
            }
        }
    }
}

/*
 * private static void m()
        {
            var thread1 = new TrackedThread(DoNothingForFiveSeconds);
            var thread2 = new TrackedThread(DoNothingForTenSeconds);
            var thread3 = new TrackedThread(DoNothingForSomeTime);

            thread1.Start();
            thread2.Thread.Start();
            thread3.Thread.Start(15);
            while (TrackedThread.Count > 0)
            {
                Console.WriteLine(TrackedThread.Count);
                Thread.Sleep(1000);
            }

            Console.ReadLine();
        }

        private static void DoNothingForFiveSeconds()
        {
            Thread.Sleep(5000);
        }

        private static void DoNothingForTenSeconds()
        {
            Thread.Sleep(10000);
        }

        private static void DoNothingForSomeTime(object seconds)
        {
            Thread.Sleep(1000 * (int)seconds);
        }
 * 
 * */
