using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading;

namespace LaserCameraCSaC.Service
{
    public class CameraService : ICameraService
    {
        private static string Message = "";
        private static ServiceHost CameraServiceHost;

        public static System.Drawing.Bitmap Current = new System.Drawing.Bitmap(1, 1);

        static CameraService()
        {
            //Tada
            CameraServiceHost = null;
            try
            {
                Uri adress = new Uri("http://localhost:8080/CameraService");
                CameraServiceHost = new ServiceHost(typeof(LaserCameraCSaC.Service.CameraService), adress);

                BasicHttpBinding bhb = new BasicHttpBinding();
                bhb.MaxReceivedMessageSize = 2147483647;
                bhb.MaxBufferSize = 2147483647;
                bhb.MaxBufferPoolSize = 2147483647;

                CameraServiceHost.AddServiceEndpoint(typeof(LaserCameraCSaC.Service.ICameraService), bhb, "");

                Console.WriteLine(bhb.MaxBufferPoolSize);
                Console.WriteLine(bhb.MaxBufferSize);
                Console.WriteLine(bhb.MaxReceivedMessageSize);

                ServiceMetadataBehavior serviceBehavior = new ServiceMetadataBehavior();
                serviceBehavior.HttpGetEnabled = true;
                CameraServiceHost.Description.Behaviors.Add(serviceBehavior);

                CameraServiceHost.Open();
                Message = string.Format("Service is live now at : {0}", adress);
            }
            catch(Exception ex)
            {
                CameraServiceHost = null;
                Message = "There is an issue with CameraService" + ex.Message;
            }
        }

        public static void DispInitMessage()
        {
            Console.WriteLine(Message);
        }

        public void SetDisplayingImageType(int type)
        {
            try
            {
                CWrapper.SetDispImage(type);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Service Error:\n{0}", ex.Message);
            }
        }

        public void SetOverlay(bool threshold, bool fixedCross, bool detectedCross)
        {
            try
            {
                CWrapper.SetOverlayer(0, threshold == true ? 1 : 0);
                CWrapper.SetOverlayer(1, fixedCross == true ? 1 : 0);
                CWrapper.SetOverlayer(2, detectedCross == true ? 1 : 0);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Service Error:\n{0}", ex.Message);
            }
        }

        public void SetProcessing(bool doprocessing)
        {
            try { 
                CWrapper.SetDoProcess(doprocessing == true ? 1 : 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Service Error:\n{0}", ex.Message);
            }
        }

        public void SetThreshold(int newThreshold)
        {
            try
            {
                if (newThreshold < 0)
                    newThreshold = 0;
                if (newThreshold > 255)
                    newThreshold = 255;

                CWrapper.SetOverlayer(4, newThreshold);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Service Error:\n{0}", ex.Message);
            }
        }

        public void SetInvertThreshold(bool inv)
        {
            try { 
            CWrapper.SetOverlayer(3, inv == true ? 1 : 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Service Error:\n{0}", ex.Message);
            }
        }

        public Stream GetImage()
        {
            try
            {
                System.Drawing.Bitmap cur = null;
                lock (Current)
                {
                    cur = Current.Clone() as System.Drawing.Bitmap;
                }

                MemoryStream ms = new MemoryStream();
                cur.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                ms.Position = 0;
                cur.Dispose();
                return ms;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Service Error:\n{0}", ex.Message);
            }
            return new MemoryStream(0);
        }

        public double GetXPercent()
        {
            try
            {
                return CWrapper.X_CenterDistancePercet();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Service Error:\n{0}", ex.Message);
            }
            return 0;
        }

        public double GetYPercent()
        {
            try
            {
                return CWrapper.Y_CenterDistancePercet();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Service Error:\n{0}", ex.Message);
            }
            return 0;
        }

        public void TestIsConnected()
        {
            try
            {
                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Service Error:\n{0}", ex.Message);
            }
        }
    }
}
