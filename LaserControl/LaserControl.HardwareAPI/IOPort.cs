using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserControl.HardwareAPI
{
    public class IOPort
    {
        /// <summary>
        /// PortType, um welche Art von Port es sich bei diesem Handelt. Eingang(IN), Ausgange(OUT) oder eine Kombination aus beidem.
        /// </summary>
        public IOPortType PortType;

        /// <summary>
        /// Bit gibt an, an welchem IOPort dieser Port ist.
        /// </summary>
        public int Bit;
        
        /// <summary>
        /// Wert für den Eingang dieses Portes
        /// </summary>
        public bool InValue;

        /// <summary>
        /// Wert für den Ausgang dieses Ports
        /// </summary>
        public bool OutValue;

        /// <summary>
        /// Name für den Eingang. (Meist nur wichtig für das Anzeigen in der GUI)
        /// </summary>
        public string InName;

        /// <summary>
        /// Name für den Ausgang. (Meist nur wichtig für das Anzeigen in der GUI)
        /// </summary>
        public string OutName;

        /// <summary>
        /// Erstellt einen neuen IOPort
        /// </summary>
        /// <param name="bit"></param>
        /// <param name="porttype"></param>
        public IOPort(int bit, IOPortType porttype)
        {
            Bit = bit;
            PortType = porttype;
        }

        /// <summary>
        /// Speichert die Daten: Bit, PortType, InName, OutName im DataSafe
        /// </summary>
        /// <param name="pos">Positionsindex, an dem die Werte gespeichert werden sollen.</param>
        /// <param name="ds">DataSafe zum Speichern</param>
        public virtual void Save(int pos, Data.DataSafe ds)
        {
            ds.Ints["Bit-"+pos] = this.Bit;
            ds.Ints["PortType-" + pos] = (int)this.PortType;
            ds.Strings["InName-" + pos] = this.InName;
            ds.Strings["OutName-"+pos] = this.OutName;
        }

        /// <summary>
        /// Lädt einen IOPort mit Bit aus einem DataSafe
        /// </summary>
        /// <param name="bit">Positionsindex, aus dem die Werte geladen werden sollen.</param>
        /// <param name="ds">DataSafe aus dem geladen werden soll.</param>
        /// <returns></returns>
        public static IOPort Load(int pos, Data.DataSafe ds)
        {
            int b = ds.Ints["Bit-" + pos, -1];
            if (b >= 0)
            {
                IOPort p = new IOPort(b, (IOPortType)ds.Ints["PortType-" + pos, 3]);
                p.InName = ds.Strings["InName-" + pos, ""];
                p.OutName = ds.Strings["OutName-" + pos, ""];
                return p;
            }
            return null;
        }
    }
}
