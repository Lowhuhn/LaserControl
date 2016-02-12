using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace LaserCameraCSaC.Service
{
    [ServiceContract]
    public interface ICameraService
    {

        [OperationContract]
        void SetDisplayingImageType(int type);

        [OperationContract]
        void SetOverlay(bool threshold, bool fixedCross, bool detectedCross);

        [OperationContract]
        void SetProcessing(bool doprocessing);        

        [OperationContract]
        void SetThreshold(int newThreshold);

        [OperationContract]
        void SetInvertThreshold(bool inv);

        [OperationContract]
        Stream GetImage();

        [OperationContract]
        double GetXPercent();

        [OperationContract]
        double GetYPercent();

        [OperationContract]
        void TestIsConnected();
    }
}
