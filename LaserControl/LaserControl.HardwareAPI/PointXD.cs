using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserControl.HardwareAPI
{
    public class PointXD<T>
    {
        /// <summary>
        /// Inhalt des Punktes aka die Koordinaten
        /// </summary>
        protected T[] _Coords;

        /// <summary>
        /// Setzte einen Wert für die ausgewählte Dimension
        /// </summary>
        /// <param name="dimension">Auswahl an welche Stelle/Dimension der Wert gespeichert werden soll.</param>
        /// <returns>Wert der ausgewählten Dimension</returns>
        public T this[int dimension]
        {
            get
            {
                return _Coords[dimension];
            }
            set
            {
                _Coords[dimension] = value;
            }
        }

        /// <summary>
        /// Anzahl der Dimensionen
        /// </summary>
        public int Dimensions
        {
            get;
            protected set;
        }

        /// <summary>
        /// Anazhl der Dimensionen
        /// </summary>
        public int Count
        {
            get
            {
                return Dimensions;
            }
        }

        /// <summary>
        /// Erstellt einen leeren Punkt mit einer festen Anzahl an dimensionen.
        /// </summary>
        /// <param name="dimensions">Anazhl der Dimensionen</param>
        protected PointXD(int dimensions)
        {
            _Coords = new T[dimensions];
            this.Dimensions = dimensions;
        }

        /// <summary>
        /// Erstellt einen Punkt mit fester Anzahl an Werten und initialisiert diesen direkt.
        /// </summary>
        /// <param name="dimensions">Anzahl der Dimensionen</param>
        /// <param name="coords">Werte für jede Dimension</param>
        public PointXD(int dimensions, params T[] coords) : this(dimensions)            
        {
            for (int i = 0; i < Count; ++i)
            {
                if(i < coords.Length)
                    this[i] = coords[i];                
            }
        }

        /// <summary>
        /// Erstellt einen Punkt mit fester Anzahl an Dimensionen, welche der Anzahl der Werten entspricht.
        /// </summary>
        /// <param name="coords">Werte für jede Dimension</param>
        public PointXD(params T[] coords) : this(coords.Length, coords)
        {
            //nix
        }
    }
}
