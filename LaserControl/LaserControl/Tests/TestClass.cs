using LaserControl.Design.Custom;
using LaserControl.HardwareAPI;
using LaserControl.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;

using LaserControl.ScriptV2;

namespace LaserControl.Tests
{
    public class TestClass
    {
        static MainWindow MW;
        static HardwareController HWC;

        public static void RunTest(MainWindow mw, HardwareController hwc)
        {
#if TEST
            MW = mw;
            HWC = hwc;
            while (true)
            {
                TestWindow tw = new TestWindow();
                if (tw.ShowDialog() != true)
                    continue;

                int testNr = tw.SelectedTest.SelectedIndex;
                switch (testNr)
                {

                    case 0:
                        TrackedThread testThread = new TrackedThread("Test ImgButton", Test1);
                        testThread.Start();
                        return;
                    case 1:
                        TrackedThread testThread2 = new TrackedThread("Test Enable Move Move Home Disable", Test2);
                        testThread2.Start();
                        return;
                    case 2:
                        TrackedThread testThread3 = new TrackedThread("Test TabControl Main Window", Test3);
                        testThread3.Thread.SetApartmentState(ApartmentState.STA);
                        testThread3.Start();
                        return;
                    case 3:
                        TrackedThread testThread4 = new TrackedThread("Test Script with FAK", Test4);
                        testThread4.Thread.SetApartmentState(ApartmentState.STA);
                        testThread4.Start();
                        return;
                    case 4:
                        TrackedThread testThread5 = new TrackedThread("Test Script with FAK", Test5);
                        testThread5.Thread.SetApartmentState(ApartmentState.STA);
                        testThread5.Start();
                        return;
                    default:
                        MessageBox.Show("Unknown Test ");
                        break;
                }
            }
#endif
        }

        /// <summary>
        /// Test ImgButton + Enable/Disable Axes
        /// </summary>
        protected static void Test1()
        {
            string[] axes = HWC.AxesControlIdents;
            while (true)
            {
                foreach (string s in axes)
                {
                    Axis a = HWC.GetAxis(s);
                    if (a.IsEnable)
                    {
                        a.Disable();
                    }
                    else
                    {
                        a.Enable();
                    }
                    Thread.Sleep(250);
                }
            }
        }

        /// <summary>
        /// Test ImgButton + Enable/Disable Axes
        /// </summary>
        protected static void Test2()
        {
            string[] axes = HWC.AxesControlIdents;
            while (true)
            {
                foreach (string s in axes)
                {
                    Axis a = HWC.GetAxis(s);
                    a.Enable();
                    Thread.Sleep(250);
                }
                Thread.Sleep(1000);
                foreach (string s in axes)
                {
                    Axis a = HWC.GetAxis(s);
                    a.MoveTo(50000000);
                    Thread.Sleep(250);
                }

                Thread.Sleep(1000);
                foreach (string s in axes)
                {
                    Axis a = HWC.GetAxis(s);
                    a.MoveTo(0);
                    Thread.Sleep(250);
                }

                Thread.Sleep(1000);
                foreach (string s in axes)
                {
                    Axis a = HWC.GetAxis(s);
                    a.Home();
                    Thread.Sleep(250);
                }

                Thread.Sleep(1000);
                foreach (string s in axes)
                {
                    Axis a = HWC.GetAxis(s);
                    a.Disable();
                    Thread.Sleep(250);
                }

                Thread.Sleep(2000);
            }
        }

        protected static void Test3(){
#if TEST
            
#endif
        }

        protected static void Test4()
        {
            int num = 0;
            string code = "$I = FAK(1);\n";
            code += "CALLC(\"System.Console\", \"WriteLine\", $I);\n";
            code += "$I = FAK(2);\n";
            code += "CALLC(\"System.Console\", \"WriteLine\", $I);\n";
            code += "$I = FAK(3);\n";
            code += "CALLC(\"System.Console\", \"WriteLine\", $I);\n";
            code += "$I = FAK(4);\n";
            code += "CALLC(\"System.Console\", \"WriteLine\", $I);\n";
            code += "$I = FAK(5);\n";
            code += "CALLC(\"System.Console\", \"WriteLine\", $I);\n";
            code += "$I = FAK(6);\n";
            code += "CALLC(\"System.Console\", \"WriteLine\", $I);\n";
            code += "$I = FAK(7);\n";
            code += "CALLC(\"System.Console\", \"WriteLine\", $I);\n";
            code += "$I = FAK(8);\n";
            code += "CALLC(\"System.Console\", \"WriteLine\", $I);\n";
            code += "$I = FAK(9);\n";
            code += "CALLC(\"System.Console\", \"WriteLine\", $I);\n";
            code += "$I = FAK(10);\n";
            code += "CALLC(\"System.Console\", \"WriteLine\", $I);\n";
            code += "$I = FAK(11);\n";
            code += "CALLC(\"System.Console\", \"WriteLine\", $I);\n";
            code += "$I = FAK(12);\n";
            code += "CALLC(\"System.Console\", \"WriteLine\", $I);\n";
            code += "$I = FAK(13);\n";
            code += "CALLC(\"System.Console\", \"WriteLine\", $I);\n";

            while (true)
            {
                
                try { 
                    ScriptHandler.SetCodeThread1(code);
                    Console.WriteLine("------------------------------------------------------------\nRun number: {0}", ++num);
                }
                catch
                {
                    Console.WriteLine("To early! :(");
                }
                
                Thread.Sleep(500);
            }
        }

        protected static void Test5()
        {
            string code = "CALLO(\"HardwareController\", \"MoveTo\", \"X\", 8000000, 10000000);";
            code += "CALLC(\"System.Threading.Thread\", \"Sleep\", 10);";
            code += "CALLO(\"HardwareController\", \"MoveTo\", \"X\", 1000000, 10000000);";
            code += "CALLC(\"System.Threading.Thread\", \"Sleep\", 10);";
            int num = 0;
            while (true)
            {
                while (ScriptHandler.State_1 != ScriptThreadState.Waiting)
                    Thread.Sleep(1000);

                try
                {
                    Console.WriteLine("------------------------------------------------------------\nRun number: {0}", ++num);
                    ScriptHandler.SetCodeThread1(code);
                    
                }
                catch
                {                    
                    Console.WriteLine("Error");
                }

                Thread.Sleep(10);
            }
        }
    }
}
