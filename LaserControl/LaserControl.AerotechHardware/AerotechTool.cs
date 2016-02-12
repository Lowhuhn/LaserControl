using Aerotech.A3200.Commands;
using LaserControl.HardwareAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LaserControl.AerotechHardware
{
    public class AerotechTool : Tool
    {
        protected Aerotech.A3200.Controller A3200Controller;
        protected PSOCommands PSO;

        public AerotechTool(string name, string controlident, ToolType tt)
            : base(name, controlident, tt)
        {
            //nix
        }

        public void Connect(Aerotech.A3200.Controller con)
        {
            A3200Controller = con;
            PSO = A3200Controller.Commands.PSO;
        }

        public override void Arm(Axis axis, int start, int end)
        {
            if (MyToolType == ToolType.Laser)
            {
                PSO.WindowReset("X", 1, 0);
                if (axis.ControlIdent == "X")
                {
                    PSO.WindowInput("X", 1, 0);
                }
                else if (axis.ControlIdent == "Y")
                {
                    PSO.WindowInput("X", 1, 4);
                }
                else
                {
                    this.Disarm();
                    throw new NotImplementedException("No Arm implemented for Axis " + axis.ControlIdent + ". Only X and Y are valid");
                }

                PSO.WindowRange("X", 1, start, end);
                PSO.OutputWindow("X");
                PSO.WindowLoad("X", 1, 0);
                PSO.Control("X", PsoMode.Arm);
            }
        }

        public override void Disarm()
        {
            try
            {
                PSO.WindowOff("X", 1);
                PSO.Control("X", PsoMode.Off);
                base.Disarm();
            }
            catch
            {

            }
        }
    }
}
