using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Aerotech.A3200;

using LaserControl.HardwareAPI;

namespace LaserControl.AerotechHardware
{
    public class AerotechHardwareController : HardwareController
    {
        /// <summary>
        /// Schnittstelle zur API von Aerotech
        /// </summary>
        protected Aerotech.A3200.Controller A3200Controller;

        public AerotechHardwareController()
            : base()
        {
            Connect();
        }
        

        protected void Connect()
        {
            try
            {
                A3200Controller = Aerotech.A3200.Controller.Connect();

                foreach (AerotechAxis a in AllAxes)
                {
                    a.Connect(this.A3200Controller);
                }

                foreach (AerotechIOController a in AllIOs)
                {
                    a.Connect(this.A3200Controller);
                }

                foreach (AerotechTool a in AllTools)
                {
                    a.Connect(this.A3200Controller);
                }

                A3200Controller.Commands.Motion.Setup.Absolute();
                A3200Controller.Commands.Motion.Setup.RampType(Aerotech.A3200.Commands.RampType.Scurve);
            }
            catch
            {
                throw new Exception("Can't connect to Aerotech A3200 Controller!\n (Service:A3200CommService)");
            }
        }

        #region New / Delete

        /// <summary>
        /// Erstellt eine neue Achse, speichert diese in AllAxes und gibt diese zurück.
        /// </summary>
        /// <param name="controlident">ControlIdent der neuen Achse</param>
        /// <returns>Die neu erstellte Achse</returns>
        public override Axis NewAxis(string controlident)
        {
            controlident = controlident.ToUpper();
            if (AllAxes.Exists(x => x.ControlIdent == controlident))
            {
                throw new Exception("An axis with this controlident allready exitst!");
            }
            AerotechAxis a = new AerotechAxis(controlident);            
            AllAxes.Add(a);
            return a;
        }

        public override IOController NewIOController(string controlident)
        {
            controlident = controlident.ToUpper();
            if (AllIOs.Exists(x => x.ControlIdent == controlident))
            {
                throw new Exception("An IOController with this controlident allready exitst!");
            }
            AerotechIOController a = new AerotechIOController(controlident);
            AllIOs.Add(a);
            return a;
        }

        public override Tool NewTool(string controlident)
        {
            controlident = controlident.ToUpper();
            if (AllTools.Count>0 && AllTools.Exists(x => x.ControlIdent == controlident))
            {
                throw new Exception("A tool with this controlident allready exitst!");
            }
            AerotechTool a = new AerotechTool("", controlident, ToolType.NoTool);
            AllTools.Add(a);
            return a;
        }

        public override Camera NewCamera(string path)
        {
            return new AerotechCamera(path);
        }

        #endregion //New / Delete

        #region Scribe

        public override void ScribeLine(string axisIdent, int start, int end, int velocity)
        {
            DoHomeWhileScribing();

            //Umwandlung in absolute Koordinaten
            start = ConvertCoordinates(axisIdent, start);
            end = ConvertCoordinates(axisIdent, end);

            Axis a = GetAxis(axisIdent);
            //Inteligentes Screiben
            if (IntelligentScribe)
            {
                int pos = a.Position;
                if (Math.Abs(end - pos) < Math.Abs(start - pos))
                {
                    int zw = start;
                    start = end;
                    end = zw;
                }
            }

            int da = a.AccelerationRampDistance(velocity);
            int dd = a.DeaccelerationRampDistance(velocity);
            int realStart = start, realEnd = end;
            if (start < end)
            {
                realStart -= da;
                realEnd += dd;
            }
            else
            {
                realStart += da;
                realEnd -= dd;
            }

            int armstart = da;
            int armend = da + (end - start);
            if (start < end)
            {
                armend *= (-1);
                armstart *= (-1);
            }
            else
            {
                armstart = da;
                armend = da + (start - end);
            }

            if (armstart > armend)
            {
                int zw = armstart;
                armstart = armend;
                armend = zw;
            }
            a.MoveTo(realStart);            
            SelectedTool.Arm(a, armstart, armend);
            a.MoveTo(realEnd, velocity);
            SelectedTool.Disarm();
        }

        public override void ScribeDiag(string axis1, string axis2, int x0, int y0, int x1, int y1, int axis1Velocity)
        {
            DoHomeWhileScribing();

            x0 = ConvertCoordinates(axis1, x0);
            y0 = ConvertCoordinates(axis2, y0);
            x1 = ConvertCoordinates(axis1, x1);
            y1 = ConvertCoordinates(axis2, y1);

            Axis a1 = this.GetAxis(axis1);
            Axis a2 = this.GetAxis(axis2);

            //Berechne Geschwindigkeit für Achse 2
            double t1 = ((double)(x1 - x0)) / ((double)axis1Velocity);
            int axis2Velocity = (int)Math.Abs(((double)(y1 - y0)) / t1);
            if (axis2Velocity > axis1Velocity)
            {
                ScribeDiag(axis2, axis1, y0, x0, y1, x1, axis1Velocity);
                return;
            }

            //Beide Geschwindigkeiten sind berechnet!
            //Jetzt müssen diese umgerechnet werden, da axis1Velocity die bahngeschwindigkeit angibt.
            double a = ((double)axis2Velocity) / ((double)axis1Velocity);
            double ac1 = Math.Sqrt(Math.Pow(axis1Velocity,2) / (1.0 + a * a));
            double ac2 = a * ac1;
            axis1Velocity = (int)ac1;
            axis2Velocity = (int)ac2;

            int accel1 = a1.AccelerationRampDistance(axis1Velocity);
            int accel2 = a2.AccelerationRampDistance(axis2Velocity);

            double time1 = a1.AccelrationRampDistanceTime(axis1Velocity, accel1);
            double time2 = a2.AccelrationRampDistanceTime(axis2Velocity, accel2);

            double diff = time1 - time2;
            if (diff > double.Epsilon)
            {
                //Anfahrtszeit von Achse 1 ist länger
                accel2 += Math.Abs((int)((diff * axis2Velocity)));                
            }
            else if (diff < -double.Epsilon)
            {
                //Anfahrtszeit von Achse 2 ist länger                
                accel1 += Math.Abs((int)((diff * axis1Velocity)));
            }
            else 
            { 
                //Beide Zeiten sind gleich lang.
                Console.WriteLine("Beide Zeiten sind gleich lang.");
            }            
            //Die Wege sind "gleich lang"

            //Berechne Abbremsweg
            int deccel1 = a1.DeaccelerationRampDistance(axis1Velocity);
            int deccel2 = a2.DeaccelerationRampDistance(axis2Velocity);

            //Berechne zuerst Startpunkte von Achse 2
            int start2 = y0 - accel2;
            int end2 = y1 + deccel2;
            if (y0 > y1)
            {
                start2 = y0 + accel2;
                end2 = y1 - deccel2;
            }

            //Berechne Startpunkt von Achse 1
            int realStart = x0, realEnd = x1;
            if (x0 <= x1)
            {
                realStart -= accel1;
                realEnd += deccel1;
            }
            else
            {
                realStart += accel1;
                realEnd -= deccel2;
            }
            //Berechne PSO Punkte
            int armstart = accel1;
            int armend = accel1 + (x1 - x0);
            if (x0 <= x1)
            {
                armend *= (-1);
                armstart *= (-1);
            }
            else
            {
                armstart = accel1;
                armend = accel1 + (x0 - x1);
            }
            if (armstart > armend)
            {
                int zw = armstart;
                armstart = armend;
                armend = zw;
            } 
            Aerotech.A3200.Commands.AxesMotionCommands amc = A3200Controller.Commands.Axes["X", "Y"].Motion;

            a1.MoveTo(realStart, false);
            a2.MoveTo(start2, true);
            a1.WaitForMotionDone();

            SelectedTool.Arm(a1, armstart, armend);

            //a.MoveTo(realEnd, velocity);
            A3200Controller.Commands.Axes[axis1, axis2].Motion.Rapid(new double[] { ((double)realEnd) / 10000.0, ((double)end2) / 10000.0 },
                                                                     new double[] { ((double)axis1Velocity) / 10000.0, ((double)axis2Velocity) / 10000.0 });

            SelectedTool.Disarm();

            return;
        }

        #endregion //Scribe
    }
}

/*
CALLO("HWC", "SetUseAbsCoordSystem", TRUE);

CALLO("HWC", "ScribeDiag", "X", "Y", 0, 0, 10000, 5000, 6000000);
*/