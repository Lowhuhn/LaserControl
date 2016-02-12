using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserControl.HardwareAPI
{
    public enum IOPortType : int
    {
        [Description("Input Port")]
        IN = 0,
        [Description("Output Port")]
        OUT = 1,
        [Description("Input and Output Port")]
        INOUT = 2,
        [Description("No Use")]
        NOUSE = 3
    }
}
