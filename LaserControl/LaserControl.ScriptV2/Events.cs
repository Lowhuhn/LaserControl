using LaserControl.ScriptV2.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LaserControl.ScriptV2
{
    public delegate void VoidEvent();
    public delegate void IntEvent(int i);

    public delegate void ScriptThreadStateEvent(ScriptThreadState state);

    public delegate void ParserExceptionEvent(ParserException pex);

    public delegate void LineChangeEvent(int newLine);
}
