using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserControl.HardwareAPI
{
    public delegate void AxisBoolEvent(Axis a, bool b);
    public delegate void AxisIntEvent(Axis a, int i);


    public delegate void ControlBoolEvent(Controller c, bool b);

    public delegate void IOControllerIntBoolEvent(IOController io, int i, bool b);

    public delegate void ToolEvent(Tool laser);
    public delegate void ToolAxisIntIntEvent(Tool t, Axis a, int i0, int i1);
}
