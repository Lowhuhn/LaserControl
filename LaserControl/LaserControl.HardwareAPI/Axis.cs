using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LaserControl.Library;
using System.Globalization;

namespace LaserControl.HardwareAPI
{
    public class Axis : Controller
    {

        #region Properties

        private TrackedThread EventThread2;

        #region Min
        /// <summary>
        /// Minimalposition der Achse
        /// </summary>
        public int Min
        {
            get;
            set;
        }
        #endregion //Min

        #region Max
        /// <summary>
        /// Maximalposition der Achse
        /// </summary>
        public int Max
        {
            get;
            set;
        }
        #endregion Max

        #region MinVelocity
        /// <summary>
        /// Minimale Geschwindigkeit
        /// </summary>
        public int MinVelocity
        {
            get;
            set;
        }
        #endregion //MinVelocity

        #region MaxVelocity
        /// <summary>
        /// Maximale Geschwindigkeit
        /// </summary>
        public int MaxVelocity
        {
            get;
            set;
        }
        #endregion //MaxVelocity

        #region PositionSpeed
        /// <summary>
        /// Positionierungsgeschwindigkeit
        /// </summary>
        public int PositionSpeed
        {
            get;
            set;
        }
        #endregion //Position Speed

        #region RampRate
        /// <summary>
        /// Anstiegsgeschwindigkeit
        /// </summary>
        public int RampRate
        {
            get;
            set;
        }
        #endregion //RampRate

        #region RampDistancePercent
        /// <summary>
        /// Prozent für den Anfahrtsweg
        /// </summary>
        public int RampDistancePercent
        {
            get;
            set;
        }
        #endregion //RampDistancePercent

        #region MinAccelRampTimeMS
        /// <summary>
        /// Minimale Anfahrtszeit in Millisekunden
        /// </summary>
        public int MinAccelRampTimeMS
        {
            get;
            set;
        }
        #endregion //MinAccelRampTimeMS

        #region StaticRampDistance
        /// <summary>
        /// Statischer Anfahrtsweg in µm
        /// </summary>
        public int StaticRampDistance
        {
            get;
            set;
        }
        #endregion //StaticRampDistance

        #region UseStaticRampDistance
        /// <summary>
        /// Gibt an ob der berechnete oder der statische Anfahrtsweg verwendet werden soll.
        /// </summary>
        public bool UseStaticRampDistance
        {
            get;
            set;
        }
        #endregion //UseStaticRampDistance


        #region IsReadyToMove
        /// <summary>
        /// Git an ob die Achse bereit zum bewegen ist
        /// </summary>
        public virtual bool IsReadyToMove
        {
            get
            {
                return IsEnable && !HasFault;
            }
        }
        #endregion //IsReadyToMove

        #region HasFault
        /// <summary>
        /// Gibt an ob die Achse einen Fehler hat.
        /// </summary>
        public bool HasFault
        {
            get;
            protected set;
        }
        #endregion //HasFault

        #region Position
        /// <summary>
        /// Ist die aktuelle absolute Position der Achse
        /// </summary>
        public int Position
        {
            get;
            protected set;
        }
        #endregion //Position

        #region Velocity
        /// <summary>
        /// Ist die aktuelle Geschwindigkeit der Achse
        /// </summary>
        public int Velocity
        {
            get;
            protected set;
        }
        #endregion //Velocity

        #region IsMoving
        /// <summary>
        /// Gibt an ob die Achse in Bewegung ist.
        /// </summary>
        public bool IsMoving
        {
            get;
            protected set;
        }
        #endregion //IsMoving

        #endregion //Properties

        #region Events

        /// <sumbmw 100 mary>
        /// Wird aufgerufen, wenn die Achse ihren Fehlerzustand ändert
        /// </summary>
        public event AxisBoolEvent HasFaultChange;

        /// <summary>
        /// Wird aufgerufen wenn die Achse ihren Bewegungszustand ändert
        /// </summary>
        public event AxisBoolEvent IsMovingEvent;

        /// <summary>
        /// Wird aufgerufen wenn sich die aktuelle Position der Achse ändert
        /// </summary>
        public event AxisIntEvent PositionChange;

        /// <summary>
        /// Wird aufgerufen wenn sich die Geschwindigkeit ändert.
        /// </summary>
        public event AxisIntEvent VelocityChange;


        #endregion //Events

        /// <summary>
        /// Erstellt eine neue Achse
        /// </summary>
        /// <param name="controlident"></param>
        public Axis(string controlident)
            : this("", controlident)
        {
            ;
        }

        /// <summary>
        /// Erstellt eine neue Achse
        /// </summary>
        /// <param name="name"></param>
        /// <param name="controlident"></param>
        public Axis(string name, string controlident)
            : base(name, controlident)
        {
            this.Load();
            IsMoving = false;
            EventThread2 = new TrackedThread(ControlIdent + " - EventThread 2 (Axis)", EventThread2Method);
            EventThread2.Start();
        }

        /// <summary>
        /// Bricht den aktuellen Vorgang der Achse ab
        /// </summary>
        public virtual void Abort()
        {
            ;
        }

        /// <summary>
        /// Löscht den Fehlerzustand
        /// </summary>
        public virtual void ClearFaults()
        {
            HasFault = false;
        }

        /// <summary>
        /// Verfährt die Achse um distance mit der Geschwindigkeit speed.
        /// </summary>
        /// <param name="distance">Distanz</param>
        /// <param name="speed">Geschwindigkeit</param>
        public virtual void FreeRunDistance(int distance, int speed)
        {
            WaitForMotionDone();
            int target = Position + distance;
            this.MoveTo(target, speed);
        }

        /// <summary>
        /// Fährt die Achse auf die Home Position
        /// </summary>
        public virtual void Home()
        {
            Position = 0;
        }

        /// <summary>
        /// Fährt die Achse auf position mit der Geschwindigkeit velocity.
        /// </summary>
        /// <param name="position">Position auf die verfahren werden soll</param>
        /// <param name="velocity">Geschwindigkeit zum verfahren</param>
        /// <param name="wait">Soll auf das Beenden der Bewegung gewartet werden?[Default = JA]</param>
        public virtual void MoveTo(int position, int velocity, bool wait = true)
        {
            Position = position;
        }

        /// <summary>
        /// Fährt die Achse auf position mit der positionierungs Geschwindigkeit.
        /// </summary>
        /// <param name="position">Position auf die verfahren werden soll</param>
        /// <param name="wait">Soll auf das Beenden der Bewegung gewartet werden?[Default = JA]</param>
        public virtual void MoveTo(int position, bool wait = true)
        {
            MoveTo(position, this.PositionSpeed, wait);
        }

        /// <summary>
        /// Verfährt die Achse um den gegebenen Wert distance mit der Geschwindigkeit velocity
        /// </summary>
        /// <param name="distance">Distanz um die verfahren werden soll</param>
        /// <param name="velocity">Geschwindigkeit zum verfahren</param>
        /// <param name="wait">Soll auf das Beenden der Bewegung gewartet werden?[Default = JA]</param>
        public virtual void IncrementalMove(int distance, int velocity, bool wait = true)
        {
            int target = Position + distance;
            this.MoveTo(target, velocity, wait);
        }

        /// <summary>
        /// Startet das freie bewegen
        /// </summary>
        /// <param name="speed">Geschwindigkeit mit der die Achse verfahren werden soll.</param>
        public virtual void StartFreeRun(int speed)
        {
            throw new System.NotImplementedException("LaserControl.Api.Axis: virtual void StartFreeRun(int speed)");
        }

        /// <summary>
        /// Beendet das freie Bewegen der Achse
        /// </summary>
        public virtual void StopFreeRun()
        {
            throw new System.NotImplementedException("LaserControl.Api.Axis: virtual void StopFreeRun()");
        }

        /// <summary>
        /// Wartet so lange bis eine Bewegung abgeschlossen wurde.
        /// </summary>
        public virtual void WaitForMotionDone()
        {
            //nix
        }

        #region Fire Events

        /// <summary>
        /// Setzt IsMoving auf den Wert b
        /// </summary>
        /// <param name="b">Wert ob die Achse ferfährt oder nicht.</param>
        protected void FireIsMoving(bool b)
        {
            IsMoving = b;
        }

        #endregion //Fire Events

        #region Acceleration / Deacceleration

        /// <summary>
        /// Berechnet den Anfahrtsweg zu einer gegebenen Geschwindigkeit.
        /// </summary>
        /// <param name="vel">Geschwindigkeit für die der Weg berechnet werden soll.</param>
        /// <returns>Anfahrtsweg in µm</returns>
        public virtual int AccelerationRampDistance(int vel)
        {
            int ret = StaticRampDistance;
            if (!UseStaticRampDistance)
            {
                double v = Math.Abs(vel) / 10000;
                //Weg für die Beschleunigung
                int s = (int)(((((v * v) / (2 * RampRate)))) * 10000);

                //Zeit für die Beschleunigung in Sekundne
                double t = v / ((double)RampRate);

                double ms = MinAccelRampTimeMS / 1000.0;
                if (t < ms)
                {
                    s = s + (int)((ms - t) * vel);
                }
                ret = (int)(s * (RampDistancePercent / 100.0));
            }

            return ret;
        }

        /// <summary>
        /// Liefert die Zeit zurück die für den Anfahrtsweg benötigt wird, für eine gegebene Geschwindigkeit
        /// Die Zeit ist angegeben in Sekunden
        /// </summary>
        /// <param name="vel">Geschwindigkeit auf diesem Weg</param>
        /// <param name="distance">Die Wegstrecke</param>
        /// <returns>Benötigte Zeit in Sekunden</returns>
        public virtual double AccelrationRampDistanceTime(int vel, int distance)
        {
            //Anfahrtsweg
            double v = Math.Abs(vel) / 10000.0;
            int s = (int)(((((v * v) / (2 * RampRate)))) * 10000);

            double t1 = v / ((double)RampRate);

            //Weg mit konstanter Geschwindigkeit
            double remaining = ((double)(distance - s)) / 10000.0;
            double t2 = Math.Abs(remaining) / v;

            return t1 + t2;
        }

        /// <summary>
        /// Berechnet dem Bremsweg für eine gegebene Geschwindigkeit.
        /// </summary>
        /// <param name="vel">Geschwindigkeit für die der Bremsweg berechnet werden soll.</param>
        /// <returns>Bremsweg in µm</returns>
        public virtual int DeaccelerationRampDistance(int vel)
        {
            int ret = StaticRampDistance;
            if (!UseStaticRampDistance)
            {
                double v = Math.Abs(vel) / 10000;
                ret = (int)(((((v * v) / (2 * RampRate)) * (RampDistancePercent / 100.0))) * 10000);
            }
            return ret;
        }

        #endregion //Acceleration / Deacceleration

        #region Thread Methode
        /// <summary>
        /// Gibt den Status der verschiedenen Variablen aus
        /// </summary>
        private void EventThread2Method()
        {
            //TimeSpan ts = new TimeSpan(100);
            bool _hasfault = this.HasFault;
            bool _isMoving = this.IsMoving;
            int _pos = this.Position;
            int _vel = this.Velocity;
            while (true)
            {
                if (_hasfault != HasFault)
                {
                    _hasfault = HasFault;
                    if (HasFaultChange != null)
                        HasFaultChange(this, _hasfault);
                }

                if (_isMoving != IsMoving)
                {
                    _isMoving = IsMoving;
                    if (IsMovingEvent != null)
                        IsMovingEvent(this, IsMoving);
                }

                if (_pos != Position)
                {
                    _pos = Position;
                    if (PositionChange != null)
                        PositionChange(this, _pos);
                }

                if (_vel != Velocity)
                {
                    _vel = Velocity;
                    if (VelocityChange != null)
                        VelocityChange(this, _vel);
                }
                Thread.Sleep(1);
            }
        }
        #endregion //Thread Methode

        #region Converter
        /// <summary>
        /// Konvertierer von µm -> mm
        /// </summary>
        /// <param name="i">Wert in µm</param>
        /// <returns>Wert in mm</returns>
        public virtual double ConvertInt(int i)
        {
            return i / 10000.0;
        }

        /// <summary>
        /// Konvertiert von mm -> µm
        /// </summary>
        /// <param name="d">Wert in mm</param>
        /// <returns>Wert in µm</returns>
        public virtual int ConvertDouble(double d)
        {
            return (int)(d * 10000.0);
        }

        /// <summary>
        /// Konvertiert von µm -> mm(String)
        /// </summary>
        /// <param name="i">Wert in µm</param>
        /// <returns>Wert in mm</returns>
        public virtual string ConvertIntToStr(int i)
        {
            double d = i / 10000.0;
            return string.Format("{0:0.0000}", Math.Round(d, 4));
        }

        #endregion //Converter

        #region Static Convetrter

        /// <summary>
        /// Konvertiert von µm -> mm
        /// </summary>
        /// <param name="i">Wert in µm</param>
        /// <returns>Wert in mm</returns>
        public static double SConvertInt(int i)
        {
            return i / 10000.0;
        }


        /// <summary>
        /// Konvertiert von mm ->µm
        /// </summary>
        /// <param name="d">Wert in mm</param>
        /// <returns>Wert in µm</returns>
        public static int SConvertDouble(double d)
        {
            return (int)(d * 10000.0);
        }

        /// <summary>
        /// Konvertiert von µm -> mm(String)
        /// </summary>
        /// <param name="i">Wert in µm</param>
        /// <returns>Wert in mm (String)</returns>
        public static string SConvertIntToStr(int i, bool fullSize = false)
        {
            double d = i / 10000.0;
            if (fullSize)
                return string.Format("{0:0.0000}", Math.Round(d, 4));
            else
                return string.Format("{0}", Math.Round(d, 4));
        }

        /// <summary>
        /// Konvertiert einen mm(string) -> µm(int)
        /// </summary>
        /// <param name="s">Wert in mm als string</param>
        /// <returns>Wert in µm als int</returns>
        public static int SConvertDoubleString(string s)
        {
            return SConvertDouble(double.Parse(s, System.Globalization.NumberStyles.Any, CultureInfo.GetCultureInfo("en-US")));
        }
        #endregion //Static Convetrter

        #region Load & Save

        /// <summary>
        /// Lädt eine Achse aus einer XML Datei
        /// </summary>
        public virtual void Load()
        {
            Data.DataSafe ds = new Data.DataSafe(Data.Paths.SettingsConfigurationPath, ControlIdent + "-Axis.xml");
            /* if(!ds.containsKey("Name") || !ds.containsKey("ControlIdent"))
             {
                 throw new Exception("Incorect save file for an Axis!");
             }*/

            this.Name = ds.Strings["Name", "Axis"];
            this.Min = ds.Ints["Min", 0];
            this.Max = ds.Ints["Max", 0];

            this.MinVelocity = ds.Ints["MinVelocity", 0];
            this.MaxVelocity = ds.Ints["MaxVelocity", 0];

            this.PositionSpeed = ds.Ints["PositionSpeed", 0];
            this.RampRate = ds.Ints["RampRate", 0];
            this.RampDistancePercent = ds.Ints["RampDistancePercent", 0];
            this.MinAccelRampTimeMS = ds.Ints["MinAccelRampTimeMS", 0];
            this.StaticRampDistance = ds.Ints["StaticRampDistance", 0];
            this.UseStaticRampDistance = ds.Bools["UseStaticRampDistance", false];
        }

        /// <summary>
        /// Speichert alle Parameter dieser Achse
        /// </summary>
        public virtual void Save()
        {
            Data.DataSafe ds = new Data.DataSafe(Data.Paths.SettingsConfigurationPath, ControlIdent + "-Axis.xml");

            ds.Strings["ControlIdent"] = this.ControlIdent;
            ds.Strings["Name"] = this.Name;

            ds.Ints["Min"] = this.Min;
            ds.Ints["Max"] = this.Max;

            ds.Ints["MinVelocity"] = this.MinVelocity;
            ds.Ints["MaxVelocity"] = this.MaxVelocity;

            ds.Ints["PositionSpeed"] = this.PositionSpeed;
            ds.Ints["RampRate"] = this.RampRate;
            ds.Ints["RampDistancePercent"] = this.RampDistancePercent;
            ds.Ints["MinAccelRampTimeMS"] = this.MinAccelRampTimeMS;
            ds.Ints["StaticRampDistance"] = this.StaticRampDistance;
            ds.Bools["UseStaticRampDistance"] = this.UseStaticRampDistance;
        }
        #endregion // Load & Save
    }
}
