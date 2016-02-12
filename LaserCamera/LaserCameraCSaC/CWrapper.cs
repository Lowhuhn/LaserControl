using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LaserCameraCSaC
{
    public class CWrapper
    {
        public static readonly string Path = AppDomain.CurrentDomain.BaseDirectory+"CLib.dll";

        public static void Load()
        {
            LoadLibrary(Path);
            TestAndLoadLib();
        }
        
        [DllImport(@"CLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetImagePointer(IntPtr image, int width, int height);

        [DllImport(@"CLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ProcessImage();

        [DllImport(@"CLib.dll", CallingConvention = CallingConvention.Cdecl)]
        protected static extern void TestAndLoadLib();

        [DllImport(@"CLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetParameter(int outputImage, int outputCross);

        [DllImport(@"CLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetCrossFound();

        [DllImport(@"CLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SetBWThreshold(double val);


        [DllImport(@"CLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetDispImage(int outimg);

        [DllImport(@"CLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetOverlayer(int layer, int value);


        [DllImport(@"CLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern double X_CenterDistancePercet();

        [DllImport(@"CLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern double Y_CenterDistancePercet();

        [DllImport(@"CLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CrossDetection();

        [DllImport(@"CLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetDoProcess(int val);

        [DllImport("kernel32.dll")]
        protected static extern IntPtr LoadLibrary(string dllToLoad);
    }
}
