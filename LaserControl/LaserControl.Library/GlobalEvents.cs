using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LaserControl.Library
{
    public delegate void GlobalClearEvent();

    public delegate void GlobalWriteEvent(string text);

    public delegate void GlobalInformationEvent(string msg, string twhere, int line);

    public class GlobalEvents
    {
        static GlobalEvents()
        {
            //ShowTypeInfo(typeof(GlobalEvents));
        }

        public static void ShowTypeInfo(Type t)
        {
            Console.WriteLine("Name: {0}", t.Name);
            Console.WriteLine("Full Name: {0}", t.FullName);
            Console.WriteLine("ToString:  {0}", t.ToString());
            Console.WriteLine("Assembly Qualified Name: {0}",
                              t.AssemblyQualifiedName);
            Console.WriteLine();
        }

        #region Errors / Warnings / Informations

        public static GlobalInformationEvent NewInformation;

        public static void RaiseNewInformationEvent(string msg, string twhere, int line = -1)
        {
            if (NewInformation != null)
            {
                NewInformation(msg, twhere, line);
            }
        }        

        #endregion //Errors / Warnings / Informations

        #region Write

        public static GlobalWriteEvent OnWrite;
        public static GlobalClearEvent OnClear;

        public static void Write(string txt)
        {
            Console.Write(txt);
            if (OnWrite != null)
            {
                OnWrite(txt);
            }
        }

        public static void Write(int i)
        {
            Write(i.ToString());
        }

        public static void Write(bool b)
        {
            Write(b.ToString());
        }

        public static void Write(double d)
        {
            Write(d.ToString());
        }

        public static void WriteLine(string txt)
        {
            Write(txt + "\n");
        }

        public static void WriteLine(int i)
        {
            Write(i);
            Write("\n");
        }

        public static void WriteLine(bool b)
        {
            Write(b);
            Write("\n");
        }

        public static void WriteLine(double d)
        {
            Write(d);
            Write("\n");
        }

        public static void Clear()
        {
            if (OnClear != null)
            {
                OnClear();
            }
        }

        #endregion //Write


    }
}
