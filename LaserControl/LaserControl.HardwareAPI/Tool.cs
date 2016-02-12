using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserControl.HardwareAPI
{
    public class Tool : Controller
    {
        #region Properties

        /// <summary>
        /// Fokus des Tools 
        /// Kameras sowie Laser haben einen Fokus
        /// NoTool hat als Fokus 0
        /// </summary>
        public int Fokus
        {
            get;
            set;
        }

        /// <summary>
        /// Abstand des Laseres von der Kamera
        /// Eine Kamera1 oder das NoTool sollten eine abstand von 0 haben
        /// </summary>
        public PointXD<int> CameraOffset
        {
            get;
            protected set;
        }

        /// <summary>
        /// Der Typ dieses Tools       
        /// </summary>
        public ToolType MyToolType
        {
            get;
            set;
        }

        #endregion //Properties

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="name">Name des Tools</param>
        /// <param name="controlident">Eindeutiger Identifier im gesammten Programmsystem für DIESES Tool</param>
        /// <param name="initcode">Der LaserScript Init Code</param>
        /// <param name="tt">Der Typ des zu erstellenden Tools</param>
        public Tool(string name, string controlident, ToolType tt)
            : base(name, controlident)
        {
            CameraOffset = new PointXD<int>(2);
            MyToolType = tt; 
            Load();
        }


        /// <summary>
        /// Methode um ein Tool "scharf" zu stellen. 
        /// Wenn sich die Achse(axis) in dem Bereich zwischen *start* und *end* bewegt, soll das Tool aktiv sein.
        /// Funktioniert nur mit einem Laser Tool
        /// </summary>
        /// <param name="axis">Achse auf die gehört werden soll</param>
        /// <param name="start">Anfang des bereichs</param>
        /// <param name="end">Ende des bereichs</param>
        public virtual void Arm(Axis axis, int start, int end)
        {

        }

        /// <summary>
        /// Hebt alle Scharfstellungen wieder auf.
        /// Alle Achsen können sich danach bewegen ohne das dieses oder ein anderes Tool aktiv etwas tun.
        /// </summary>
        public virtual void Disarm()
        {

        }

        public virtual void Load()
        {
            Data.DataSafe ds = new Data.DataSafe(Data.Paths.SettingsConfigurationPath, ControlIdent + "-Tool.xml");
            Name = ds.Strings["Name", ""];

            Fokus = ds.Ints["Fokus", 0];

            CameraOffset[0] = ds.Ints["CameraOffset-X", 0];
            CameraOffset[1] = ds.Ints["CameraOffset-Y", 0];

            MyToolType = (ToolType)ds.Ints["ToolType", 0];
        }

        public virtual void Save()
        {
            Data.DataSafe ds = new Data.DataSafe(Data.Paths.SettingsConfigurationPath, ControlIdent + "-Tool.xml");

            ds.Strings["ControlIdent"] = ControlIdent;
            ds.Strings["Name"] = Name;

            ds.Ints["Fokus"] = Fokus;
            ds.Ints["CameraOffset-X"] = CameraOffset[0];
            ds.Ints["CameraOffset-Y"] = CameraOffset[1];

            ds.Ints["ToolType"] = (int)MyToolType;
        }
    }
}
