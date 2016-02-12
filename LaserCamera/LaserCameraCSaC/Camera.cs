using PylonC.NET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LaserCameraCSaC
{
    public delegate void NewImage( Bitmap b);

    public delegate void ImageGrabbedHandler(object sender, ImageArgs args);

    public class ImageArgs : EventArgs
    {
        private Bitmap _bmp;

        public ImageArgs(Bitmap bmp)
            : base()
        {
            this._bmp = bmp;
        }

        public Bitmap Image
        {
            get { return _bmp; }
            set { }
        }
    }

    

    public class Camera
    {

        /*public int CrossCenterX
        {
            get { return CDN.CenterX; }
        }
        public int CrossCenterY
        {
            get { return CDN.CenterY; }
        }
        public long CrossFindingDuration
        {
            get { return CDN.DurationAverage; }
        }
        public bool CrossFound
        {
            get { return CDN.CrossFound; }
        }*/

        private static Camera instance;

        private bool grabbing;
        public bool Grabbing
        {
            get { return grabbing; }
        }
        private Thread GrabThread;

       // private CrossDetectionNils CDN;

        private PYLON_DEVICE_HANDLE dev;
        private PYLON_STREAMGRABBER_HANDLE hGrabber;
        private PYLON_WAITOBJECT_HANDLE hWait;

        private const uint NUM_BUFFERS = 5;

        //public event ImageGrabbedHandler ImageGrabbed;

        public event NewImage ImageGrabbed;

        public bool IsConnected
        {
            get { return dev != null; }
        }

        protected Stopwatch SP = new Stopwatch();

        private Camera()
        {

            grabbing = false;

            Pylon.Initialize();

            uint numDevices = Pylon.EnumerateDevices();

            if (numDevices == 0)
            {
                //throw new Exception("No Camera connected");
                Console.WriteLine("No Camera connected. \nPlease restart the programm with a connect BASLER camera!");
                dev = null;
                return;
            }

            dev = Pylon.CreateDeviceByIndex(0);

        }

        ~Camera()
        {
            if (IsConnected)
                Pylon.DestroyDevice(dev);

            Pylon.Terminate();
        }

        protected void OnGrabAndDispose( Bitmap bmp)
        {
            if (ImageGrabbed != null)
                ImageGrabbed( bmp);
            bmp.Dispose();
        }

        public void StartGrab()
        {
            grabbing = true;
            GrabThread = new Thread(ContinuousGrab);
            GrabThread.Start();
        }

        public void StopGrab()
        {
            grabbing = false;
        }

        private void ContinuousGrab()
        {
            try
            {
                lock (dev)
                {
                    // prepare grabbing, see pylon C.NET docs
                    Pylon.DeviceOpen(dev, Pylon.cPylonAccessModeControl | Pylon.cPylonAccessModeStream);
                    Pylon.DeviceFeatureFromString(dev, "AcquisitionMode", "Continuous");
                    uint payloadSize = checked((uint)Pylon.DeviceGetIntegerFeature(dev, "PayloadSize"));
                    uint nStreams = Pylon.DeviceGetNumStreamGrabberChannels(dev);
                    hGrabber = Pylon.DeviceGetStreamGrabber(dev, 0);
                    Pylon.StreamGrabberOpen(hGrabber);
                    hWait = Pylon.StreamGrabberGetWaitObject(hGrabber);
                    Pylon.StreamGrabberSetMaxNumBuffer(hGrabber, NUM_BUFFERS);
                    Pylon.StreamGrabberSetMaxBufferSize(hGrabber, payloadSize);
                    Pylon.StreamGrabberPrepareGrab(hGrabber);
                    var buffers = new PylonBuffer<Byte>[NUM_BUFFERS];
                    var handles = new PYLON_STREAMBUFFER_HANDLE[NUM_BUFFERS];
                    for (int i = 0; i < NUM_BUFFERS; i++)
                    {
                        PylonBuffer<Byte> buffer = new PylonBuffer<byte>(payloadSize, true);
                        PYLON_STREAMBUFFER_HANDLE handle = Pylon.StreamGrabberRegisterBuffer(hGrabber, ref buffer);
                        Pylon.StreamGrabberQueueBuffer(hGrabber, handle, i);
                        handles[i] = handle;
                        buffers[i] = buffer;
                    }

                    Pylon.DeviceExecuteCommandFeature(dev, "AcquisitionStart");

                    PylonGrabResult_t grabResult;
                    bool isReady;

                    while (grabbing)
                    {                        
                        int bufferIndex;
                        isReady = Pylon.WaitObjectWait(hWait, 1000);

                        if (!isReady)
                        {
                            throw new Exception("Grab timeout");
                        }
                        //

                        isReady = Pylon.StreamGrabberRetrieveResult(hGrabber, out grabResult);

                        if (!isReady)
                        {
                            throw new Exception("Failed to retrieve a grab result");
                        }

                        bufferIndex = (int)grabResult.Context;

                        if (grabResult.Status == EPylonGrabStatus.Grabbed)
                        {
                            PylonBuffer<Byte> buffer = buffers[bufferIndex];
                            OnGrabAndDispose(GrabResultToImage(grabResult, buffer));
                            //Thread.Sleep(10);
                        }
                        else if (grabResult.Status == EPylonGrabStatus.Failed)
                        {
                            Console.Error.WriteLine("Error grabbing. Error Code = {0}", grabResult.ErrorCode);
                        }

                        Pylon.StreamGrabberQueueBuffer(hGrabber, grabResult.hBuffer, bufferIndex);
                        
                    }
                    Pylon.DeviceExecuteCommandFeature(dev, "AcquisitionStop");
                    Pylon.StreamGrabberCancelGrab(hGrabber);
                    do
                    {
                        isReady = Pylon.StreamGrabberRetrieveResult(hGrabber, out grabResult);
                    } while (isReady);
                    for (int i = 0; i < NUM_BUFFERS; i++)
                    {
                        Pylon.StreamGrabberDeregisterBuffer(hGrabber, handles[i]);
                        buffers[i].Dispose();
                    }
                    Pylon.StreamGrabberFinishGrab(hGrabber);
                    Pylon.StreamGrabberClose(hGrabber);
                    Pylon.DeviceClose(dev);
                }
            }
            catch
            {
                Console.WriteLine(GenApi.GetLastErrorDetail());
            }
        }

        public void OneFrame()
        {
            try
            {
                lock (dev)
                {
                    Pylon.DeviceOpen(dev, Pylon.cPylonAccessModeControl | Pylon.cPylonAccessModeStream);

                    PylonGrabResult_t grabResult;
                    PylonBuffer<Byte> imgBuf = null;

                    if (!Pylon.DeviceGrabSingleFrame(dev, 0, ref imgBuf, out grabResult, 500))
                    {
                        Console.WriteLine("timeout");
                    }

                    if (grabResult.Status == EPylonGrabStatus.Grabbed)
                    {
                        OnGrabAndDispose(GrabResultToImage(grabResult, imgBuf));
                    }
                    else if (grabResult.Status == EPylonGrabStatus.Failed)
                    {
                        Console.Error.WriteLine("Frame wasn't grabbed successfully.  Error code = {1}", grabResult.ErrorCode);
                    }

                    Pylon.DeviceClose(dev);
                }
            }
            catch
            {
                Console.WriteLine(GenApi.GetLastErrorDetail());
            }
        }

        protected Bitmap GrabResultToImage(PylonGrabResult_t gr, PylonBuffer<Byte> buf)
        {
            Bitmap bmp = new Bitmap(gr.SizeX, gr.SizeY, PixelFormat.Format24bppRgb);

            byte[] img = buf.Array;

            unsafe
            {
                BitmapData bmd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, bmp.PixelFormat);
                int PixelSize = 3;
                int BufStride = gr.OffsetX + gr.SizeX + gr.PaddingX;
                byte* row = null;
                for (int y = 0; y < bmd.Height; y++)
                {
                    row = (byte*)bmd.Scan0 + (y * bmd.Stride);
                    for (int x = 0; x < bmd.Width; x++)
                    {
                        byte value = img[gr.OffsetY + y * BufStride + gr.OffsetX + x];

                        row[x * PixelSize] = value;
                        row[x * PixelSize + 1] = value;
                        row[x * PixelSize + 2] = value;
                    }
                }
                SP.Restart();
                CWrapper.SetImagePointer(bmd.Scan0, bmd.Width, bmd.Height);
                CWrapper.ProcessImage();
                SP.Stop();
                bmp.UnlockBits(bmd);
            }

            return bmp;
        }

        public static Camera Instance
        {
            get
            {
                if (instance == null)
                    instance = new Camera();

                return instance;
            }
        }

    }
}
