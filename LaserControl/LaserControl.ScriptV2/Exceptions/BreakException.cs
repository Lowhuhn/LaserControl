using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LaserControl.ScriptV2.Exceptions
{
    public class BreakException : Exception
    {
        public BreakException()
        {
            ;
        }

        public BreakException(string message)
            : base(message)
        {
            ;
        }

        public BreakException(string message, Exception inner)
            : base(message, inner)
        {
            ;
        }
    }
}
