using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LaserControl.ScriptV2.Exceptions
{
    public static class ExceptionRaiser
    {
        public static void RaiseReturn()
        {
            throw new ReturnException();
        }

        public static void BreakException()
        {
            throw new BreakException();
        }
    }
}
