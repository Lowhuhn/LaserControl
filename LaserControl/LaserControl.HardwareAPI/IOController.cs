using LaserControl.Data;
using LaserControl.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LaserControl.HardwareAPI
{
    public class IOController : Controller
    {
        protected List<IOPort> AllIOPorts;
        public IOPort[] All
        {
            get
            {
                return AllIOPorts.ToArray();
            }
        }

        

        #region Events

        private TrackedThread EventThread2;

        public IOControllerIntBoolEvent OnInBitValueChange;
        public IOControllerIntBoolEvent OnOutBitValueChange;

        #endregion //Events

        /// <summary>
        /// Erstellt einen neuen IOController und initialisiert diesen.
        /// </summary>
        /// <param name="controlident">Eindeutiges zeichen des Controllers</param>
        public IOController(string controlident)
            : this("", controlident)
        {
            ;
        }

        /// <summary>
        /// Erstellt einen neuen IOController und initialisiert diesen.
        /// </summary>
        /// <param name="name">Name des Controllers</param>
        /// <param name="controlident">Eindeutiges zeichen des Controllers</param>
        public IOController(string name, string controlident)
            : base(name, controlident)
        {
            AllIOPorts = new List<IOPort>();
            Load();
            EventThread2 = new TrackedThread(ControlIdent + " - EventThread 2 (IO)", EventThread2Method);
            EventThread2.Start();
        }

        public virtual void AddPort(IOPort port)
        {
            this.AllIOPorts.Add(port);
        }

        public virtual void RemoveAllPorts()
        {
            this.AllIOPorts.Clear();
        }

        /// <summary>
        /// Überprüft ob ein Port mit Bit in der Liste aller Ports vorhanden ist.
        /// </summary>
        /// <param name="bit"></param>
        /// <returns></returns>
        public virtual bool ContainsPort(int bit)
        {
            return AllIOPorts.Exists(x => x.Bit == bit);
        }

        /// <summary>
        /// Überprüft ob der angegebene Port(Bit) vorhanden ist und vom gesuchtem PortType ist.
        /// </summary>
        /// <param name="bit">Gesuchter Port</param>
        /// <param name="pt">Gesuchter PortType</param>
        /// <returns></returns>
        public virtual bool IsPort(int bit, IOPortType pt)
        {
            return AllIOPorts.Exists(x => x.Bit == bit && x.PortType == pt);
        }

        /// <summary>
        /// Gibt einen IOPort mit Bit zurück, sofern dieses in dem IOController vorhanden ist.
        /// </summary>
        /// <param name="bit">Bit der IOPorts</param>
        /// <returns>Einen IOPort oder null, wenn der IOPort nicht gefunden werden kann.</returns>
        public virtual IOPort GetPort(int bit)
        {
            return AllIOPorts.Find(x => x.Bit == bit);
        }

        /// <summary>
        /// Liest den aktuellen Wert an einem EINGANG eines IOPort
        /// </summary>
        /// <param name="bit">Port welcher gelesen werden soll.</param>
        /// <returns>Wert des Portes (Aktiv = true, Inaktiv = false)</returns>
        public virtual bool ReadInValue(int bit)
        {
            if (IsPort(bit, IOPortType.IN) || IsPort(bit, IOPortType.INOUT))
            {
                return GetPort(bit).InValue;
            }
            return false;
        }

        /// <summary>
        /// Liest den aktuellen Wert an einem AUSGANG eines IOPort
        /// </summary>
        /// <param name="bit">Port welcher gelesen werden soll.</param>
        /// <returns>Wert des Portes (Aktiv = true, Inaktiv = false)</returns>
        public virtual bool ReadOutValue(int bit)
        {
            if (IsPort(bit, IOPortType.OUT) || IsPort(bit, IOPortType.INOUT))
            {
                return GetPort(bit).OutValue;
            }
            return false;
        }

        /// <summary>
        /// Setzt den Ausgang eines IOPorts auf den Wert val
        /// </summary>
        /// <param name="bit">Bit welches gesetzt/geändert werden soll</param>
        /// <param name="val">Wert auf den das Bit gesetzt werden soll</param>
        public virtual void WriteOutValue(int bit, bool val)
        {
            if (IsPort(bit, IOPortType.OUT) || IsPort(bit, IOPortType.INOUT))
            {
                GetPort(bit).OutValue = val;                
            }
        }

        /// <summary>
        /// Lädt alle Ports für diesen IOController aus der Konfigurationsdatei
        /// </summary>
        public virtual void Load()
        {
            Data.DataSafe ds = new Data.DataSafe(Data.Paths.SettingsConfigurationPath, ControlIdent + "-IO.xml");
            if (!ds.containsKey("ControlIdent"))
            {
                throw new Exception("Incorect save file for an IO Controller!");
            }

            this.Name = ds.Strings["Name", ""];
            int count = ds.Ints["Count", 0];

            this.AllIOPorts.Clear();

            for (int i = 0; i < count; ++i)
            {
                IOPort p = IOPort.Load(i, ds);
                AllIOPorts.Add(p);
            }
            SortIOPorts();
        }

        protected virtual void SortIOPorts()
        {
            this.AllIOPorts.Sort(delegate(IOPort p1, IOPort p2)
            {
                if (p1 == null || p2 == null) return 0;
                if (p1.Bit == p2.Bit) return 0;
                else if (p1.Bit < p2.Bit) return -1;
                else return 1;
            });
        }

        /// <summary>
        /// Speichert alle Konfigurationen für alle Ports für diesen IOController in die Konfigurationsdatei
        /// </summary>
        public virtual void Save()
        {
            Data.DataSafe ds = new Data.DataSafe(Data.Paths.SettingsConfigurationPath, ControlIdent + "-IO.xml");
            ds.Strings["ControlIdent"] = ControlIdent;
            ds.Strings["Name"] = Name;

            ds.Ints["Count"] = AllIOPorts.Count;

            for (int i = 0; i < AllIOPorts.Count; ++i)
            {
                AllIOPorts[i].Save(i, ds);
            }
        }

        #region Thread Methode
        /// <summary>
        /// Gibt den Status der verschiedenen Ports aus
        /// </summary>
        private void EventThread2Method()
        {
            Dictionary<int, BoolBool> vals = new Dictionary<int, BoolBool>();
            BoolBool bb;
            while (true)
            {
                foreach (IOPort p in AllIOPorts)
                {
                    if (p == null)
                        continue;

                    if (!vals.ContainsKey(p.Bit))
                    {
                        vals.Add(p.Bit, new BoolBool(!p.InValue, !p.OutValue));
                    }
                    bb = vals[p.Bit];
                    if (OnInBitValueChange != null)
                    {
                        if(p.PortType == IOPortType.IN || p.PortType == IOPortType.INOUT)
                        {
                            if(p.InValue != bb.A)
                            {
                                bb.A = p.InValue;
                                OnInBitValueChange(this, p.Bit, p.InValue);
                            }
                        }
                    }

                    if (OnOutBitValueChange != null)
                    {
                        if (p.PortType == IOPortType.OUT || p.PortType == IOPortType.INOUT)
                        {
                            if (p.OutValue != bb.B)
                            {
                                bb.B = p.OutValue;
                                OnOutBitValueChange(this, p.Bit, p.OutValue);
                            }
                        }
                    }
                }
                Thread.Sleep(1);
            }
        }
        protected class BoolBool
        {
            public bool A;
            public bool B;
            public BoolBool(bool a, bool b)
            {
                A = a;
                B = b;
            }
        }
        #endregion //Thread Methode

    }
}
