using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LaserControl.HardwareAPI
{
    public class HardwareController
    {

        /// <summary>
        /// Liste aller Achsen
        /// </summary>
        protected List<Axis> AllAxes;

        public string[] AxesControlIdents
        {
            get
            {
                string[] res = new string[AllAxes.Count];
                for (int i = 0; i < AllAxes.Count; ++i)
                    res[i] = AllAxes[i].ControlIdent;
                return res;
            }
        }

        /// <summary>
        /// Liste aller IOcontroller
        /// </summary>
        protected List<IOController> AllIOs;

        public string[] IOControllerControlIdents
        {
            get
            {
                string[] res = new string[AllIOs.Count];
                for (int i = 0; i < AllIOs.Count; ++i)
                    res[i] = AllIOs[i].ControlIdent;
                return res;            
            }
        }

        /// <summary>
        /// Liste von allen Tools
        /// </summary>
        protected List<Tool> AllTools;

        public string[] ToolsControlIdents
        {
            get
            {
                string[] res = new string[AllTools.Count];
                for (int i = 0; i < AllTools.Count; ++i)
                {
                    res[i] = AllTools[i].ControlIdent;
                }
                return res;
            }
        }

        protected Camera CurrentCamera;
        public Camera Camera
        {
            get
            {
                return CurrentCamera;
            }
        }

        #region Properties

        #region Tools

        /// <summary>
        /// Das No Tool Objekt, der Fallback, wenn kein Tool ausgewählt ist oder wird.
        /// </summary>
        protected readonly Tool NoTool = null;//new Tool("No Tool", "NO-TOOL", "", ToolType.NoTool);

        /// <summary>
        /// Das vorher ausgewählte Tool
        /// </summary>
        public Tool PreviousSelectedTool
        {
            get;
            protected set;
        }

        /// <summary>
        /// Das aktell ausgewählte Tool
        /// </summary>
        public Tool SelectedTool
        {
            get;
            protected set;
        }

        #endregion //Tools

        #region Koordinaten

        /// <summary>
        /// Gibt an ob das Absolute oder Relative Koordinatensystem genutzt werden soll.
        /// </summary>
        public bool UseAbsCoordSystem
        {
            get;
            protected set;
        }

        /// <summary>
        /// Abstand eines Kreuzes in der rechten oberen Ecke des Substrates, von der rechten oberen Ecke
        /// </summary>
        public PointXD<int> CrossOffsetRightUpper
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gibt die Ladeposition von X,Y,Z an
        /// </summary>
        public PointXD<int> LoadPosition
        {
            get;
            protected set;
        }

        /// <summary>
        /// Tischmittelpunkt, Mitte der U-Achse // Rotationspunkt
        /// </summary>
        public PointXD<int> TableCenter
        {
            get;
            protected set;
        }

        /// <summary>
        /// Tischnullpunkt, rechte obere Ecke des Substrathalters
        /// </summary>
        public PointXD<int> TableOrigin
        {
            get;
            protected set;
        }

        /// <summary>
        /// Abstand des Substrates zum Tischnullpunkt(TableOrigin)
        /// </summary>
        public PointXD<int> ProbeOffset
        {
            get;
            protected set;
        }

        #endregion //Koordinaten

        public bool HomeWhileScribing
        {
            get;
            set;
        }

        public int HomeWhileScribingNumber
        {
            get;
            set;
        }

        public int HomeWhileScribingCount
        {
            get;
            protected set;
        }

        public List<string> HomeWhileScribingAxes
        {
            get;
            protected set;
        }

        #region Sonstiges

        /// <summary>
        /// Gibt an ob in beide Richtungen gelasert werden soll!
        /// </summary>
        public bool IntelligentScribe
        {
            get;
            set;
        }

        #endregion //Sonstiges

        #endregion //Properties

        //Initialisiert einen neuen HardwareController
        public HardwareController()
        {
            //LaserControl.ScriptV2.GlobalObjects.HardwareController = this;

            AllAxes = new List<Axis>();
            AllIOs = new List<IOController>();
            AllTools = new List<Tool>();

            /*SelectedTool = NoTool;
            PreviousSelectedTool = NoTool;
            AllTools.Add(NoTool);*/

            UseAbsCoordSystem = false;
            CrossOffsetRightUpper = new PointXD<int>(2);
            TableCenter = new PointXD<int>(2);
            TableOrigin = new PointXD<int>(2);
            ProbeOffset = new PointXD<int>(2);
            LoadPosition = new PointXD<int>(3);

            HomeWhileScribingCount = 0;
            HomeWhileScribingAxes = new List<string>();

            InternalLoad();



            NewTool("No-TOOL");
            NoTool = GetTool("NO-TOOL");
            NoTool.MyToolType = ToolType.NoTool;
            SelectedTool = NoTool;
            PreviousSelectedTool = NoTool;

            LaserControl.ScriptV2.GlobalObjects.HWC = this;
            
            /*
#warning Static implementation of values for testing Table Origin
            TableOrigin[0] = 4422180;
            TableOrigin[1] = 1274006;

#warning Static implementation of values for testing Table Center
            TableCenter[0] = 6457181;
            TableCenter[1] = 2956500;

            LoadPosition[0] = 500000;
            LoadPosition[1] = 600000;
            LoadPosition[2] = 0;

            IntelligentScribe = true;*/

            

        }

        /// <summary>
        /// Lädt die Konfiguration des HardwareControllers, wenn dieser erstellt wird, aus der entsprechenden XML Datei
        /// </summary>
        protected virtual void InternalLoad()
        {
            Data.DataSafe ds = new Data.DataSafe(Data.Paths.SettingsConfigurationPath, "HardwareController");
            int ac = ds.Ints["AxisCount",0];
            for (int i = 0; i < ac; ++i)
            {
                string a = ds.Strings["Axis-" + i, ""].ToUpper();
                NewAxis(a);
            }

            int ic = ds.Ints["IOCount", 0];
            for (int i = 0; i < ic; ++i)
            {
                string a = ds.Strings["IO-" + i, ""].ToUpper();
                NewIOController(a);
            }
            this.AllIOs.Sort(delegate(IOController i1, IOController i2)
            {
                if (i1 == null || i2 == null) return 0;
                return string.Compare(i1.ControlIdent, i2.ControlIdent);
            });

            int tc = ds.Ints["ToolCount", 0];
            for (int i = 0; i < tc; ++i)
            {
                string a = ds.Strings["Tool-" + i, ""].ToUpper();
                if(a != "" && a != "NO-TOOL"){
                    NewTool(a);
                }

            }

            //Camera
            CurrentCamera = NewCamera(ds.Strings["CameraURL", "http://localhost:8080/CameraService"]);
            LaserControl.ScriptV2.GlobalObjects.Camera = CurrentCamera;

            //Properties
            this.TableOrigin[0] = ds.Ints["TableOrigin-X",0];
            this.TableOrigin[1] = ds.Ints["TableOrigin-Y", 0];

            this.TableCenter[0] = ds.Ints["TableCenter-X", 0];
            this.TableCenter[1] = ds.Ints["TableCenter-Y", 0];

            this.LoadPosition[0] = ds.Ints["LoadPosition-X", 0];
            this.LoadPosition[1] = ds.Ints["LoadPosition-Y", 0];
            this.LoadPosition[2] = ds.Ints["LoadPosition-Z", 0];

            this.IntelligentScribe = ds.Bools["IntelligentScribe", false];

            this.HomeWhileScribing = ds.Bools["HomeWhileScribing", false];
            this.HomeWhileScribingNumber = ds.Ints["HomeWhileScribingNumber",0];
            int hwsac = ds.Ints["HomeWhileScribingAxesCount", 0];
            this.HomeWhileScribingAxes.Clear();
            for (int i = 0; i < hwsac; ++i)
            {
                this.HomeWhileScribingAxes.Add(ds.Strings["HWSAxis-"+i, ""]);
            }
        }

        public virtual void Save()
        {
            Data.DataSafe ds = new Data.DataSafe(Data.Paths.SettingsConfigurationPath, "HardwareController");

            ds.Ints["AxisCount"] = AllAxes.Count;
            ds.Ints["IOCount"] = AllIOs.Count;
            ds.Ints["ToolCount"] = AllTools.Count-1;

            for (int i = 0; i < AllAxes.Count; ++i)
            {
                ds.Strings["Axis-"+i] = AllAxes[i].ControlIdent;
                AllAxes[i].Save();
            }

            for (int i = 0; i < AllIOs.Count; ++i)
            {
                ds.Strings["IO-"+i] = AllIOs[i].ControlIdent;
                AllIOs[i].Save();
            }

            int index = 0;
            for (int i = 0; i < AllTools.Count; ++i)
            {
                if (AllTools[i] != NoTool)
                {
                    ds.Strings["Tool-" + index] = AllTools[i].ControlIdent;
                    AllTools[i].Save();
                    ++index;
                }
            }

            //Camera
            ds.Strings["CameraURL"] = CurrentCamera.Path;

            //Properties
            ds.Ints["TableOrigin-X"] = this.TableOrigin[0];
            ds.Ints["TableOrigin-Y"] = this.TableOrigin[1];

            ds.Ints["TableCenter-X"] = this.TableCenter[0];
            ds.Ints["TableCenter-Y"] = this.TableCenter[1];

            ds.Ints["LoadPosition-X"] = this.LoadPosition[0];
            ds.Ints["LoadPosition-Y"] = this.LoadPosition[1];
            ds.Ints["LoadPosition-Z"] = this.LoadPosition[2];

            ds.Bools["IntelligentScribe"] = this.IntelligentScribe;

            ds.Bools["HomeWhileScribing"] = this.HomeWhileScribing;
            ds.Ints["HomeWhileScribingNumber"] = this.HomeWhileScribingNumber;

            ds.Ints["HomeWhileScribingAxesCount"] = this.HomeWhileScribingAxes.Count;            
            for (int i = 0; i < this.HomeWhileScribingAxes.Count; ++i)
            {
                ds.Strings["HWSAxis-" + i] = this.HomeWhileScribingAxes[i];
            }

        }

        #region New & Delete

        /// <summary>
        /// Erstellt eine neue Achse, speichert diese in AllAxes und gibt diese zurück.
        /// </summary>
        /// <param name="controlident">ControlIdent der neuen Achse</param>
        /// <returns>Die neu erstellte Achse</returns>
        public virtual Axis NewAxis(string controlident)
        {
            controlident = controlident.ToUpper();
            if (AllAxes.Exists(x => x.ControlIdent == controlident))
            {
                throw new Exception("An axis with this controlident allready exitst!");
            }
            Axis a = new Axis(controlident);
            AllAxes.Add(a);
            return a;
        }

        /// <summary>
        /// Erstellt einen neue IOController, speichert diese in AllIOs und gibt diesen zurück.
        /// </summary>
        /// <param name="controlident">ControlIdent des neuen IOControllers</param>
        /// <returns>Der neu erstellte IOController</returns>
        public virtual IOController NewIOController(string controlident)
        {
            controlident = controlident.ToUpper();
            if (AllIOs.Exists(x => x.ControlIdent == controlident))
            {
                throw new Exception("An iocontroller with this controlident allready exitst!");
            }
            IOController a = new IOController(controlident);
            AllIOs.Add(a);
            return a;
        }

        /// <summary>
        /// Erstellt eine neues Tool, speichert diese in AllTools und gibt dieses zurück.
        /// </summary>
        /// <param name="controlident">ControlIdent des neuen Tools</param>
        /// <returns>Das neu erstellte Tool</returns>
        public virtual Tool NewTool(string controlident)
        {
            controlident = controlident.ToUpper();
            if (AllTools.Exists(x => x.ControlIdent == controlident))
            {
                throw new Exception("A tool with this controlident allready exitst!");
            }
            Tool a = new Tool("", controlident, ToolType.NoTool);
            AllTools.Add(a);
            return a;
        }

        /// <summary>
        /// Erstellt eine Neue Camera und gib diese wieder zurück
        /// </summary>
        /// <param name="path">Pfad / Url des Services der Kamera</param>
        /// <returns>Das Kamera Objekt</returns>
        public virtual Camera NewCamera(string path)
        {            
            return new Camera(path);
        }

        #endregion //New & Delete

        #region Exists (Contains)

        /// <summary>
        /// Gibt zurück ob eine Achse mit controlident im HardwareController vorhanden ist.
        /// </summary>
        /// <param name="controlident">ControlIdent der Achse</param>
        /// <returns>Existenz der Achse im HardwareController</returns>
        public virtual bool AxisExists(string controlident)
        {
            controlident = controlident.ToUpper();
            return AllAxes.Exists(x => x.ControlIdent == controlident);
        }

        /// <summary>
        /// Gibt zurück ob ein IOController mit controlident im HardwareController vorhanden ist.
        /// </summary>
        /// <param name="controlident">ControlIdent des IOController</param>
        /// <returns>Existenz des IOControllers im HardwareController</returns>
        public virtual bool IOControllerExists(string controlident)
        {
            controlident = controlident.ToUpper();
            return AllIOs.Exists(x => x.ControlIdent == controlident);
        }

        /// <summary>
        /// Gibt zurück ob ein Tool mit controlident im HardwareController vorhanden ist.
        /// </summary>
        /// <param name="controlident">ControlIdent des Tools</param>
        /// <returns>Existenz des Tools im HardwareController</returns>
        public virtual bool ToolExists(string controlident)
        {
            controlident = controlident.ToUpper();
            return AllTools.Exists(x => x.ControlIdent == controlident);
        }

        #endregion //Contains

        #region Getter

        /// <summary>
        /// Gibt eine Achse zurück, sofern diese Existiert.
        /// </summary>
        /// <param name="controlIdent">ControlIdent der Achse</param>
        /// <returns>Die gesuchte Achse</returns>
        public virtual Axis GetAxis(string controlIdent)
        {
            controlIdent = controlIdent.ToUpper();
            return AllAxes.Find(x => x.ControlIdent == controlIdent);
        }

        /// <summary>
        /// Gibt einen IOController zurück, sofern dieser Existiert.
        /// </summary>
        /// <param name="controlIdent">ControlIdent des IOControllers</param>
        /// <returns>Der gesuchte IOController</returns>
        public virtual IOController GetIOController(string controlIdent)
        {
            controlIdent = controlIdent.ToUpper();
            return AllIOs.Find(x => x.ControlIdent == controlIdent);
        }

        /// <summary>
        /// Gibt ein Tool zurück, sofern diese Existiert.
        /// </summary>
        /// <param name="controlIdent">ControlIdent des Tools</param>
        /// <returns>Das gesuchte Tool</returns>
        public virtual Tool GetTool(string controlIdent)
        {
            controlIdent = controlIdent.ToUpper();
            return AllTools.Find(x => x.ControlIdent == controlIdent);
        }

        #endregion //Getter

        #region Koordinatensystem Methoden

        /// <summary>
        /// Konvertiert die angegebenen Koordinaten in das entsprechende Relative Koordinatensystem, sofern dies durch UseAbsCoordSystem=false geschehen soll.
        /// </summary>
        /// <param name="axis">Achse für die die Werte verändert werden sollen</param>
        /// <param name="coord">Koordinaten mit relativem Abstand zum Nullpunkt</param>
        /// <returns>Die konvertierte Koordinate</returns>
        public virtual int ConvertCoordinates(string axis, int coord)
        {
            if (!UseAbsCoordSystem)
            {
                axis = axis.ToUpper();
                if (axis == "X")
                    return coord + TableOrigin[0] + SelectedTool.CameraOffset[0] + ProbeOffset[0];
                if (axis == "Y")
                    return coord + TableOrigin[1] + SelectedTool.CameraOffset[1] + ProbeOffset[1];
            }
            return coord;
        }

        /// <summary>
        /// Konvertiert die angegebenen Koordinaten in das entsprechende Koordinatensystem, egal welcher Wert in UseAbsCoordSystem angegeben ist!!!
        /// <see cref="ConvertCoordinates"/>
        /// </summary>
        /// <param name="axis">Achse für die die Werte verändert werden sollen</param>
        /// <param name="coord">Koordinaten mit relativem Abstand zum Nullpunkt</param>
        /// <returns>Die konvertierte Koordinate</returns>
        public virtual int ConvertCoordinatesAlways(string axis, int coord)
        {
            axis = axis.ToUpper();
            if (axis == "X")
                return coord + TableOrigin[0] + SelectedTool.CameraOffset[0] + ProbeOffset[0];
            if (axis == "Y")
                return coord + TableOrigin[1] + SelectedTool.CameraOffset[1] + ProbeOffset[1];
            return coord;
        }

        /// <summary>
        /// Setzt den abstand der Probe zum Tischnullpunkt mithilfe des rechten oberen Kreuzes
        /// </summary>
        public virtual void SetProbeOffset()
        {
            //int x = GetAxis("X").Position;
            //int y = GetAxis("Y").Position;
            //ProbeOffset[0] = x - CrossOffsetRightUpper[0] - TableOrigin[0];
            //ProbeOffset[1] = y - CrossOffsetRightUpper[1] - TableOrigin[1];
            SetProbeOffset(CrossOffsetRightUpper[0], CrossOffsetRightUpper[1]);
        }

        public virtual void SetProbeOffset(int xo, int yo)
        {
            int x = GetAxis("X").Position;
            int y = GetAxis("Y").Position;
            ProbeOffset[0] = x - xo - TableOrigin[0];
            ProbeOffset[1] = y - yo - TableOrigin[1];
        }

        public virtual void ResetProbeOffset()
        {
            ProbeOffset[0] = 0;
            ProbeOffset[1] = 0;
        }

        public virtual int GetTableCenter(string axis)
        {
            if (axis.ToUpper() == "Y")
                return TableCenter[1];
            return TableCenter[0];
        }

        #endregion // Koordinatensystem Methoden

        #region Axis-Methoden

        public virtual int AxisPosition(string axis)
        {
            if(AxisExists(axis))
                return GetAxis(axis.ToUpper()).Position;
            return 0;
        }

        public virtual void AxisEnable(string axis)
        {
            if (AxisExists(axis))
                GetAxis(axis.ToUpper()).Enable();
        }

        public virtual void AxisDisable(string axis)
        {
            if (AxisExists(axis))
                GetAxis(axis.ToUpper()).Disable();
        }

        public virtual void AxisClearFaults(string axis)
        {
            if (AxisExists(axis))
                GetAxis(axis.ToUpper()).ClearFaults();
        }

        public virtual void Home(string axis)
        {
            if (AxisExists(axis))
            {
                GetAxis(axis).Home();
            }
        }

        #endregion

        #region Move Methoden

        /// <summary>
        /// Verfährt die Achse axis auf die Position pos mit der Geschwindigkeit velocity
        /// </summary>
        /// <param name="Axis">ControlIdent der Achse</param>
        /// <param name="pos">Position auf die die Achse verfahren werdne soll</param>
        /// <param name="velocity">Geschwindigkeit zum verfahren</param>
        /// <param name="wait">Gibt an ob auf das Beenden des Verfahrens gewartet werden soll.</param>
        public virtual void MoveTo(string axis, int pos, int velocity, bool wait = true)
        {
            axis = axis.ToUpper();
            if (AxisExists(axis))
            {
                pos = ConvertCoordinates(axis, pos);
                GetAxis(axis).MoveTo(pos, velocity, wait);
            }
        }

        /// <summary>
        /// Verfährt die Achse axis auf die Position pos mit der positionierungs Geschwindigkeit
        /// </summary>
        /// <param name="axis">ControlIdent der Achse</param>
        /// <param name="pos">Position auf die die Achse verfahren werdne soll</param>
        /// <param name="wait">Gibt an ob auf das Beenden des Verfahrens gewartet werden soll.</param>
        public virtual void MoveTo(string axis, int pos, bool wait = true)
        {
            axis = axis.ToUpper();
            if(AxisExists(axis)){
                pos = ConvertCoordinates(axis, pos);
                GetAxis(axis).MoveTo(pos, wait);
            }
        }

        public virtual void MoveTo(string axis, int pos, int velocity)
        {
            MoveTo(axis, pos, velocity, true);
        }

        public virtual void MoveTo(string axis, int pos)
        {
            MoveTo(axis, pos, true);
        }

        public virtual void MoveToLoadPosition()
        {
            GetAxis("Z").MoveTo(LoadPosition[2], false);
            GetAxis("Y").MoveTo(LoadPosition[1], false);
            GetAxis("X").MoveTo(LoadPosition[0], true);
        }

        public virtual void MoveToolFocus()
        {
            Axis a = GetAxis("Z");
            a.MoveTo(SelectedTool.Fokus);
        }

        public virtual void MoveIncremental(string axis, int distance, bool wait = true)
        {
            Axis a = GetAxis(axis);
            if (a != null)
            {
                a.IncrementalMove(distance, a.PositionSpeed, wait);
            }
        }

        public virtual void MoveIncremental(string axis, int distance, int velocity, bool wait = true)
        {
            Axis a = GetAxis(axis);
            if (a != null)
            {
                a.IncrementalMove(distance, velocity, wait);
            }
        }

        #endregion //Move Methoden

        #region IO Methoden

        public virtual bool ReadInValue(string controller, int bit)
        {
            IOController ic = this.GetIOController(controller);
            if (ic == null)
            {
                return false;
            }
            return ic.ReadInValue(bit);
        }

        public virtual bool ReadOutValue(string controller, int bit)
        {
            IOController ic = this.GetIOController(controller);
            if (ic == null)
            {
                return false;
            }
            return ic.ReadOutValue(bit);
        }

        public virtual void WriteOutValue(string controller, int bit, bool val)
        {
            controller = controller.ToUpper();
            if (IOControllerExists(controller))
            {
                GetIOController(controller).WriteOutValue(bit, val);
            }
        }

        #endregion //IO Methoden

        #region Scribe Methoden

        public virtual void DoHomeWhileScribing()
        {
            if (HomeWhileScribing)
            {
                //Incrememt Scribe Count
                this.HomeWhileScribingCount++;

                if (this.HomeWhileScribingCount >= this.HomeWhileScribingNumber)
                {
                    // Home alle Achsen
                    foreach (Axis a in AllAxes)
                    {
                        int pos = a.Position;
                        a.Home();
                        a.WaitForMotionDone();
                        a.MoveTo(pos);
                    }
                    this.HomeWhileScribingCount = 0;
                }
            }
        }

        public virtual void ScribeLine(string axisIdent, int start, int end, int velocity)
        {
            start = ConvertCoordinates(axisIdent, start);
            end = ConvertCoordinates(axisIdent, end);
            if (axisIdent == "X")
            {
                Axis a = GetAxis("Y");
            }
            else if (axisIdent == "Y")
            {
                Axis a = GetAxis("X");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="axis1">Achse 1</param>
        /// <param name="axis2">Achse 2</param>
        /// <param name="x0">Startpunkt Achse 1</param>
        /// <param name="y0">Startpunkt Achse 2</param>
        /// <param name="x1">Endpunkt Achse 1</param>
        /// <param name="y1">Endpunkt Achse 2</param>
        /// <param name="axis1Velocity">Geschwindigkeit Achse 1</param>
        public virtual void ScribeDiag(string axis1, string axis2, int x0, int y0, int x1, int y1, int axis1Velocity)
        {
            //nichts;
        }

        #endregion //Scribe Methoden

        #region Tool Methoden


        public virtual void SetUseAbsCoordSystem(bool val)
        {
            this.UseAbsCoordSystem = val;
        }

        public virtual void SelectTool(string toolCI)
        {
            if (!ToolExists(toolCI))
            {
                throw new Exception("No such tool: "+toolCI);
            }

            this.PreviousSelectedTool = SelectedTool;
            this.SelectedTool = GetTool(toolCI);
        }

        #endregion //Tool Methoden        

        #region Configuration Methods

        public virtual void ShowConfigurationWindow()
        {
            MessageBox.Show("HardwareAPI.HardwareController don't has a configuration Window.", "No Configration Window", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        #endregion //Konfiguration Methods
    }
}
