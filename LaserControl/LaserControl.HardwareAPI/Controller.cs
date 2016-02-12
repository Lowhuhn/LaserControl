using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using LaserControl.Library;

namespace LaserControl.HardwareAPI
{
    public class Controller
    {
        #region Porperties

        #region EventThread1
        /// <summary>
        /// Dieser Thread Aktualisiert nur "IsEnable" mit der Methode EventThreadMethod1
        /// </summary>
        private TrackedThread EventThread1;
        #endregion //EventThread1

        #region Name
        /// <summary>
        /// Gibt den Namen des Controllers an
        /// </summary>
        public string Name
        {
            get;
            set;
        }
        #endregion //name

        #region ControlIdent 
        /// <summary>
        /// Eindeutiger Identifier im gesammten Programmsystem für DIESEN Controller
        /// </summary>
        public string ControlIdent
        {
            get;
            set;
        }
        #endregion // ControlIdent

        #region IsEnable
        /// <summary>
        /// Gibt an ob der Controller verfügbar ist oder nicht
        /// </summary>
        public virtual bool IsEnable
        {
            get;
            protected set;
        }
        #endregion //IsEnable

        #endregion //Porperties

        #region Events

        /// <summary>
        /// Wird immer dann Aufgerufen wenn sich der Verfügbarkeitszustand dieses Tools verändert.
        /// </summary>
        public event ControlBoolEvent IsEnableChanged;

        #endregion //Events

        /// <summary>
        /// Erstellt einen neuen Controller, die Basisklasse für alle Komponenten der LaserControl Software.
        /// </summary>
        /// <param name="name">Name des Controllers</param>
        /// <param name="controlIdent">ControlIdent des Controller, sollte eindeutig im gesammten Programmsystem sein.</param>
        public Controller(string name, string controlIdent)
        {
            this.ControlIdent = controlIdent.ToUpper();
            this.Name = name;
            EventThread1 = new TrackedThread(ControlIdent+" - EventThread 1", EventThread1Method);
            EventThread1.Start();
        }

        /// <summary>
        /// Gibt den Status IsEnable an das Event!!!
        /// </summary>
        private void EventThread1Method()
        {
            bool _isenable = this.IsEnable;
            while (true)
            {
                if (_isenable != IsEnable)
                {
                    _isenable = IsEnable;
                    if (this.IsEnableChanged != null)
                        IsEnableChanged(this, IsEnable);
                }
                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// Enabled den Controller
        /// </summary>
        public virtual void Enable()
        {
            IsEnable = true;
        }

        /// <summary>
        /// Disabled den Controller
        /// </summary>
        public virtual void Disable()
        {
            IsEnable = false;
        }

    }
}
