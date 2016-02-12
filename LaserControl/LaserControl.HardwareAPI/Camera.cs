using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace LaserControl.HardwareAPI
{
    public class Camera
    {
        public virtual string Name
        {
            get
            {
                return "Hardware API Camera";
            }
        }

        public virtual string Path
        {
            get;
            set;
        }

        public virtual bool IsConnected
        {
            get;
            protected set;
        }

        public Camera(string path)
        {
            //tada
            Path = path;
            IsConnected = false;
        }

        public virtual void SetDisplayingImageType(int type)
        {

        }

        public virtual void SetOverlay(bool threshold, bool fixedCross, bool detectedCross)
        {

        }

        public virtual void SetProcessing(bool doprocessing)
        {

        }

        public virtual void SetThreshold(int newThreshold)
        {

        }

        public virtual void SetInvertThreshold(bool inv)
        {

        }

        public virtual Bitmap GetImage()
        {
            return new Bitmap(1,1);
        }

        public virtual void DisplayCurrentImage()
        {

        }

        public virtual double GetXPercent()
        {
            return 0;
        }

        public virtual double GetYPercent()
        {
            return 0;
        }
    }
}
