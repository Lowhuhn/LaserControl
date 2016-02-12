using LaserControl.AerotechHardware.ServiceReference1;
using LaserControl.HardwareAPI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;

using LaserControl.Library;
using System.Threading;
using System.IO;

namespace LaserControl.AerotechHardware
{
    public class AerotechCamera : Camera
    {
        private CameraServiceClient Client;
        private TrackedThread UpdateThread;


        public override string Name
        {
            get
            {
                return "Basler A641f via Network";
            }
        }        

        public AerotechCamera(string path)
            : base(path)
        {
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            binding.MaxReceivedMessageSize = 2147483647;
            binding.MaxBufferSize = 2147483647;
            binding.MaxBufferPoolSize = 2147483647;
            EndpointAddress epa = new EndpointAddress(path);
            Client = new CameraServiceClient(binding, epa);

            UpdateThread = new TrackedThread("Camera " + Name + " Event Thread", UpdateThreadMethod);
            UpdateThread.Start();

        }

        #region Methods 
        public override void SetDisplayingImageType(int type)
        {
            try
            {
                Client.SetDisplayingImageType(type);
            }
            catch
            {
                //nix
            }
        }

        public override void SetOverlay(bool threshold, bool fixedCross, bool detectedCross)
        {
            try
            {
                Client.SetOverlay(threshold, fixedCross, detectedCross);
            }
            catch
            {
                //nix
            }
        }

        public override void SetProcessing(bool doprocessing)
        {
            try
            {
                Client.SetProcessing(doprocessing);
            }
            catch
            {
                //nix
            }
        }

        public override void SetThreshold(int newThreshold)
        {
            try
            {
                Client.SetThreshold(newThreshold);
            }
            catch
            {
                //nix
            }
        }

        public override void SetInvertThreshold(bool inv)
        {
            try
            {
                Client.SetInvertThreshold(inv);
            }
            catch
            {
                //nix
            }
        }

        public override Bitmap GetImage()
        {
            try
            {
                return Bitmap.FromStream(Client.GetImage()) as Bitmap;
            }
            catch
            {
                //Console.WriteLine(ex.Message);
            }
            return new Bitmap(1, 1);
        }

        public override double GetXPercent()
        {
            try
            {
                return Client.GetXPercent();
            }
            catch
            {
                //nix
            }
            return 0;
        }

        public override double GetYPercent()
        {
            try
            {
                return Client.GetYPercent();
            }
            catch
            {
                //nix
            }
            return 0;
        }

        #endregion //Methods

        protected void UpdateThreadMethod()
        {
            bool old = false;
            while (true)
            {
                try
                {
                    Client.TestIsConnected();
                    this.IsConnected = true;
                }
                catch(Exception ex)
                {
                    //Console.WriteLine("Camera not Connected");                    
                    this.IsConnected = false;
                    Thread.Sleep(500);
                }
                if (this.IsConnected != old)
                {
                    old = this.IsConnected;
                    GlobalEvents.RaiseNewInformationEvent("Camera is connexted or disconnected", "Camera");                    
                }
            }
        }
    }
}
