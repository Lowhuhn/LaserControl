using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LaserControl.ScriptV2.Exceptions
{
    public class ReturnException : Exception
    {
        public ReturnException()
        {
            ;
        }

        public ReturnException(string message)
            : base(message)
        {
            ;
        }

        public ReturnException(string message, Exception inner)
            : base(message, inner)
        {
            ;
        }
    }
}
