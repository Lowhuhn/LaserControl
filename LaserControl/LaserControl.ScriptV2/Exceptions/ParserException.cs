using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LaserControl.ScriptV2.Exceptions
{
    public class ParserException : Exception
    {
        public int ParserErrorLine
        {
            get;
            protected set;
        }

        public int ParserErrorCode
        {
            get;
            protected set;
        }

        public string ParserErrorMessage
        {
            get
            {
                return GetErrorMessage(this.ParserErrorCode);
            }
        }

        public string ParserErrorAdditionalMessage
        {
            get;
            protected set;
        }

        public string ParserFunctionName
        {
            get;
            protected set;
        }

        public ParserException(string name, int code, int line)
        {
            ParserErrorCode = code;
            ParserFunctionName = name;
            ParserErrorLine = line;
        }

        public ParserException(string name, int code, int line, string additionalmessage) : this(name, code, line)
        {
            ParserErrorAdditionalMessage = additionalmessage;            
        }

        protected string GetErrorMessage(int code)
        {
            switch(code)
            {
                case 1:
                    return "Token missmatch!";
            }
            return "";
        }

        public override string Message
        {
            get
            {
                if (ParserErrorCode == 0)
                {
                    return String.Format("{0} : {2}->{1}", ParserErrorCode, ParserErrorMessage, ParserFunctionName);
                }
                return String.Format("{0} : {2}->{1}\n{3}", ParserErrorCode, ParserErrorMessage, ParserFunctionName, ParserErrorAdditionalMessage);
            }
        }

    }
}
