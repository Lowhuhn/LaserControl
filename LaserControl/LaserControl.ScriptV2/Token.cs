using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserControl.ScriptV2
{
    public class Token
    {
        public int Line
        {
            get;
            protected set;
        }

        public TT TokenType
        {
            get;
            protected set;
        }

        public bool HasContent
        {
            get;
            protected set;
        }

        public Token()
        {
            HasContent = false;
            Line = 0;
        }

        public Token(TT tokentype, int line)
            : this()
        {            
            TokenType = tokentype;
            Line = line;
        }

        public virtual Type ContentType
        {
            get{
                return null;
            }
        }

        public override string ToString()
        {
            return "Token("+this.TokenType+")";
        }
    }

    public class TokenWithContent<T> : Token
        where T : IConvertible
    {
        public T Content
        {
            get;
            protected set;
        }

        public TokenWithContent(TT tokentype, int line, T content)  : base(tokentype, line)       
        {
            this.Content = content;
            this.HasContent = true;
        }

        public override Type ContentType
        {
            get
            {
                return typeof(T);
            }
        }

        public override string ToString()
        {
            return "Token<" + this.TokenType + ">(" + this.ContentType + " | " + this.Content.ToString() + ")";
        }
    }
    
}
