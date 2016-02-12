using Aerotech.A3200.Status;
using LaserControl.HardwareAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LaserControl.AerotechHardware
{
    public class AerotechIOController : IOController
    {
        /// <summary>
        /// Schnittstelle zur API von Aerotech
        /// </summary>
        protected Aerotech.A3200.Controller A3200Controller;


        public AerotechIOController(string controlident):this("", controlident)
        {

        }

        public AerotechIOController(string name, string controlident):base(name, controlident)
        {

        }


        /// <summary>
        /// Verbindet das Objekt mit der Softwareschnittstelle!
        /// </summary>
        /// <param name="controller"></param>
        internal void Connect(Aerotech.A3200.Controller controller)
        {
            this.A3200Controller = controller;
            A3200Controller.ControlCenter.Diagnostics.NewDiagPacketArrived += Diagnostics_NewDiagPacketArrived;            
        }

        public override void WriteOutValue(int bit, bool val)
        {
            A3200Controller.Commands.IO.DigitalOutputBit(bit, ControlIdent, (val ? 1 : 0));
        }

        #region Aerotech Callback

        protected void Diagnostics_NewDiagPacketArrived(object sender, NewDiagPacketArrivedEventArgs e)
        {
            int DIn = e.Data[ControlIdent].DigitalInputs;
            int DOut = e.Data[ControlIdent].DigitalOutputs;
            foreach (var io in this.AllIOPorts)
            {
                if (io.PortType == IOPortType.IN || io.PortType == IOPortType.INOUT)
                {
                    io.InValue = (DIn & (1 << io.Bit)) != 0;
                }

                if (io.PortType == IOPortType.OUT || io.PortType == IOPortType.INOUT)
                {
                    io.OutValue = (DOut & (1 << io.Bit)) != 0;
                }
            }
        }

        #endregion //Aerotech Callback
    }
}
