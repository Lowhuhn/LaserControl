using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserControl.HardwareAPI
{
    public enum ToolType : int
    {
        [Description("No Tool")]
        NoTool = 0,
        [Description("Laser")]
        Laser = 1,
        [Description("Camera")]
        Camera = 2
    }
}
