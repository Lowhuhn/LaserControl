using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LaserControl.HardwareAPI;
using Aerotech.A3200.Status;

namespace LaserControl.AerotechHardware
{
    public class AerotechAxis : Axis 
    {
        /// <summary>
        /// Schnittstelle zur API von Aerotech
        /// </summary>
        protected Aerotech.A3200.Controller A3200Controller;

        /// <summary>
        /// Enthält die bewegungsbefehle der Aerotech API
        /// </summary>
        protected Aerotech.A3200.Commands.AxesRootCommands AxesRoot;


        public AerotechAxis(string controlident)
            : base(controlident)
        {
            ;
        }

        public AerotechAxis(string name, string controlident)
            : base(name, controlident)
        {
            ;
        }

        /// <summary>
        /// Verbindet das Objekt mit der Softwareschnittstelle!
        /// </summary>
        /// <param name="controller"></param>
        internal void Connect(Aerotech.A3200.Controller controller)
        {
            this.A3200Controller = controller;
            AxesRoot = controller.Commands.Axes[ControlIdent];
            A3200Controller.ControlCenter.Diagnostics.NewDiagPacketArrived += Diagnostics_NewDiagPacketArrived;
            AxesRoot.Motion.Setup.RampMode(Aerotech.A3200.Commands.RampMode.Rate);
            AxesRoot.Motion.Setup.RampRate(new double[] { RampRate });
        }

        #region Aerotech Callback

        protected void Diagnostics_NewDiagPacketArrived(object sender, NewDiagPacketArrivedEventArgs e)
        {
            this.IsEnable = e.Data[ControlIdent].DriveStatus.Enabled;
            this.HasFault = e.Data[ControlIdent].AxisFault.None != true;
            
            this.Position = (int)(e.Data[ControlIdent].PositionCommand * 10000);
            
            this.Velocity = (int)(e.Data[ControlIdent].VelocityCommand * 10000);
        }

        #endregion //Aerotech Callback

        public override void Enable()
        {
            try
            {
                AxesRoot.Motion.Enable();
            }
            catch
            {

            }
        }

        public override void Disable()
        {
            try
            {
                AxesRoot.Motion.Disable();
            }
            catch
            {

            }
        }

        public override void ClearFaults()
        {
            if (AxesRoot != null)
            {
                AxesRoot.Motion.FaultAck();
                this.Enable();
            }
        }

        public override void Home()
        {
            if (this.IsReadyToMove)
            {
                WaitForMotionDone();
                A3200Controller.Commands.Motion.Advanced.HomeAsync(ControlIdent);
            }
        }

        public override void MoveTo(int position, bool wait = true)
        {
            /*if (IsReadyToMove)
            {
                A3200Controller.Commands.Motion.MoveAbs(ControlIdent, ConvertInt(position), ConvertInt(this.PositionSpeed));

                if (wait)
                {
                    WaitForMotionDone();
                }
            }*/
            this.MoveTo(position, this.PositionSpeed, wait);
        }

        public override void MoveTo(int position, int velocity, bool wait = true)
        {
            if (IsReadyToMove)
            {
                A3200Controller.Commands.Motion.MoveAbs(ControlIdent, ConvertInt(position), ConvertInt(velocity));

                if (wait)
                {
                    WaitForMotionDone();
                }
            }
        }

        public override void IncrementalMove(int distance, int velocity, bool wait = true)
        {
            if (IsReadyToMove)
            {
                A3200Controller.Commands.Motion.MoveInc(ControlIdent, ConvertInt(distance), ConvertInt(velocity));
                if (wait)
                {
                    WaitForMotionDone();
                }
            }
        }

        public override void WaitForMotionDone()
        {
            AxesRoot.Motion.WaitForMotionDone(Aerotech.A3200.Commands.WaitOption.MoveDone);
            AxesRoot.Motion.WaitForMotionDone(Aerotech.A3200.Commands.WaitOption.InPosition);            
        }

        public override void StartFreeRun(int speed)
        {
            if (IsReadyToMove)
            {
                A3200Controller.Commands.Motion.FreeRun(ControlIdent, ConvertInt(speed));
            }
        }

        public override void StopFreeRun()
        {
            A3200Controller.Commands.Motion.FreeRunStop(ControlIdent);
        }
    }
}
