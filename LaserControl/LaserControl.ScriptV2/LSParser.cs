using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LaserControl.ScriptV2.Exceptions;
using LaserControl.ScriptV2.Nodes;

namespace LaserControl.ScriptV2
{
    public class LSParser
    {
        protected int Index = 0;
        List<Token> MyTokens = null;
        Dictionary<string, Node> Variables;

        protected List<Node> CurrentNodes;

        protected Token Current
        {
            get
            {
                return MyTokens[Index];
            }
        }

        protected int Count
        {
            get
            {
                return MyTokens.Count;
            }
        }

        protected bool DisplayLines = false;
        protected int _CurrentLine = -1;
        protected int CurrentLine
        {
            get { return _CurrentLine; }
            set
            {
                if (value != _CurrentLine)
                {
                    if (_CurrentLine != value)
                    {
                        _CurrentLine = value;
                        if (DisplayLines && this.CurrentNodes != null)
                        {
                            //Console.WriteLine(value);
                            this.CurrentNodes.Add(new CurrentLineNode(value));
                        }
                    }
                    
                }
            }
        }

        protected string FunctionName;

        public LSParser(List<Token> tokens)
        {
            MyTokens = tokens;
            Variables = new Dictionary<string, Node>();
            DisplayLines = false;
            FunctionName = "";
        }

        public LSParser(List<Token> tokens, string funcName) : this(tokens)
        {
            FunctionName = funcName;
        }

        public LSParser(List<Token> tokens, string funcName, bool displayLine) : this(tokens, funcName)
        {
            DisplayLines = displayLine;
            if (displayLine)
            {
                CurrentLine = tokens[0].Line;
            }
        }

        protected void ThrowParserException(int id, string message)
        {
            throw new ParserException(FunctionName, id, this.CurrentLine, message);            
        }

        protected void AcceptAny()
        {
            ++Index;
            if (Index < MyTokens.Count)
                CurrentLine = MyTokens[Index].Line;
        }

        protected void Accept(TT tokentype)
        {
            if (Current.TokenType != tokentype)
            {
//#warning Script Implement Error Handling
                //Console.WriteLine("Error - 1");
                //throw new ParserException(FunctionName, 1, this.CurrentLine, String.Format("Tokentype expected {1}, current {0}", Current.TokenType, tokentype));                
                ThrowParserException(1, String.Format("Tokentype expected {1}, current {0}", Current.TokenType, tokentype));
            }
            ++Index;
            if (Index < MyTokens.Count)
                CurrentLine = MyTokens[Index].Line;            
        }

        protected VariableNode<V> GetVaraible<V>(string name) where V: IConvertible
        {
            if (Variables.ContainsKey(name))
            {
                if (Variables[name] is VariableNode<V>)
                {
                    return Variables[name] as VariableNode<V>;
                }
                else
                {
#warning Script Implement Error Handling
                    Console.WriteLine("Error - 2 ({0})", name);
                    return null;
                }
            }
            else
            {
                VariableNode<V> v = new VariableNode<V>(name);
                Variables.Add(name, v);
                return v;
            }
        }

        protected Node GetVariableAsNode(string name)
        {
            if (Variables.ContainsKey(name))
            {
                return Variables[name];
            }
            else
            {
                //Unknown Variable
//#warning Script Implement Error Handling
//                Console.WriteLine("Error - 3 ({0} -> {1})", this.FunctionName, name);
                //throw new ParserException(FunctionName, 3, this.CurrentLine, String.Format("Unknown variable: ${0}", name));
                ThrowParserException(3, String.Format("Unknown variable: ${0}", name));
            }
            return null;
        }

        protected bool VariableNodeExists(string name)
        {
            return Variables.ContainsKey(name);
        }

        public List<Node> GenerateNodes(List<Node> initVars = null)
        {
            Variables.Clear();
            if (initVars != null)
            {
                foreach (Node n in initVars)
                {
                    if (n is VariableNode<int>)
                    {
                        Variables.Add(((VariableNode<int>)n).VariableName.ToUpper(), n);
                    }
                    else if (n is VariableNode<double>)
                    {
                        Variables.Add(((VariableNode<double>)n).VariableName.ToUpper(), n);
                    }
                    else if (n is VariableNode<bool>)
                    {
                        Variables.Add(((VariableNode<bool>)n).VariableName.ToUpper(), n);
                    }
                    else if (n is VariableNode<string>)
                    {
                        Variables.Add(((VariableNode<string>)n).VariableName.ToUpper(), n);
                    }
                }
            }
            Index = 0;
            return StartingNodes();
        }

        protected List<Node> StartingNodes()
        {            
            List<Node> nodes = new List<Node>();
            this.CurrentNodes = nodes;

            this._CurrentLine = -1;
            this.CurrentLine = 0;

            while (Index < Count)
            {
                Node n = Input();
                if (n != null)
                    nodes.Add(n);
            }

            //Reset Current Line!
            CurrentLine = -1;

            return nodes;
        }

        protected Node Input()
        {
            switch (Current.TokenType)
            {
                case TT.TT_Dollar:
                    return VariableAssignment();
                case TT.TT_Semicolon:
                    Accept(TT.TT_Semicolon);
                    break;
                case TT.TT_IdentCALLC:
                    return CALLC();
                case TT.TT_IdentCALLO:
                    return CALLO();
                case TT.TT_IdentInline:
                    return IdentInline();
                case TT.TT_IdentFOR:
                    return FOR();
                case TT.TT_IdentWHILE:
                    return WHILE();
                case TT.TT_IdentIF:
                    return IF();                    
                case TT.TT_Ident:
                    return Ident();
                case TT.TT_IdentPAUSE:
                    return PAUSE();
                case TT.TT_IdentRETURN:
                    return RETURN();
                default:
#warning Script Implement Error Handling
                    Console.WriteLine("Error - 4");
                    return null;
            }
            return null;
        }

        #region Mathematik

        protected Node Addition()
        {
            Node m = Multiplikation();

            while (Current.TokenType == TT.TT_Plus || Current.TokenType == TT.TT_Minus || 
                    Current.TokenType == TT.TT_EqualTest || Current.TokenType == TT.TT_UnEqualSign ||
                    Current.TokenType == TT.TT_SignGreater || 
                    Current.TokenType == TT.TT_SignLess || Current.TokenType == TT.TT_SignLessOrEqual)
            {
                switch (Current.TokenType)
                {
                    case TT.TT_Plus:
                        Accept(TT.TT_Plus);
                        m = NodeFactory.GenMathNode(MathNodeOperator.PLUS, m, Multiplikation());
                        break;
                    case TT.TT_Minus:
                        Accept(TT.TT_Minus);
                        m = NodeFactory.GenMathNode(MathNodeOperator.MINUS, m, Multiplikation());
                        break;

                    case TT.TT_EqualTest:
                        Accept(TT.TT_EqualTest);
                        m = NodeFactory.GenLogicNode(LogicOperator.Equal, m, Multiplikation());
                        break;
                    case TT.TT_UnEqualSign:
                        Accept(TT.TT_UnEqualSign);
                        m = NodeFactory.GenLogicNode(LogicOperator.UnEqual, m, Multiplikation());
                        break;
                    case TT.TT_SignGreater:
                        Accept(TT.TT_SignGreater);
                        m = NodeFactory.GenLogicNode(LogicOperator.Greater, m, Multiplikation());
                        break;
                    case TT.TT_SignLess:
                        Accept(TT.TT_SignLess);
                        m = NodeFactory.GenLogicNode(LogicOperator.Lower, m, Multiplikation());
                        break;
                    case TT.TT_SignLessOrEqual:
                        Accept(TT.TT_SignLessOrEqual);
                        m = NodeFactory.GenLogicNode(LogicOperator.LowerEqual, m, Multiplikation());
                        break;
                }
            }

            return m;
        }        

        protected Node Klammer()
        {
            int sign = 1;
            if (Current.TokenType == TT.TT_Plus || Current.TokenType == TT.TT_Minus)
            {
                if (Current.TokenType == TT.TT_Minus)
                    sign = -1;
                AcceptAny(); //Accept + | -
            }

            switch (Current.TokenType)
            {
                case TT.TT_IdentCALLC:
                    return CALLC();
                case TT.TT_IdentCALLO:
                    return CALLO();
                case TT.TT_ExclaMark:
                    Accept(TT.TT_ExclaMark);
                    Node e = Klammer();
                    if(e.ContentType == typeof(bool)){
                        return new LogicNode<bool, bool>(LogicOperator.Equal, (ContentNode<bool>)e, new ContentNode<bool>(false));
                    }
                    break;
                case TT.TT_Dollar:
                    Node v = Variable();
                    if (sign < 0)
                    {
                        v = NodeFactory.GenMathNode(MathNodeOperator.MULTI, v, new ContentNode<int>(-1));
                    }
                    return v;
                case TT.TT_ParenLeft:
                    //Klammer auf
                    Accept(TT.TT_ParenLeft);
                    Node pl = Addition();
                    if (sign < 0)
                    {
                        if (pl.IsContentNode)
                        {
                            pl = NodeFactory.GenMathNode(MathNodeOperator.MULTI, pl, new ContentNode<int>(-1));
                        }
                        else
                        {
#warning Script Implement Error Handling
                            Console.WriteLine("Error - 5");
                        }
                    }
                    Accept(TT.TT_ParenRight);
                    return pl;
                case TT.TT_IntNumber:
                    ContentNode<int> i = new ContentNode<int>(((TokenWithContent<int>)(Current)).Content);
                    Accept(TT.TT_IntNumber);
                    if(sign < 0){                        
                        return new MathNode<int, int>(MathNodeOperator.MULTI, i, new ContentNode<int>(-1));
                    }
                    return i;
                case TT.TT_DoubleNumber:
                    ContentNode<double> d = new ContentNode<double>(((TokenWithContent<double>)(Current)).Content);
                    Accept(TT.TT_DoubleNumber);
                    if(sign < 0){
                        return new MathNode<double, int>(MathNodeOperator.MULTI, d, new ContentNode<int>(-1));
                    }
                    return d;
                case TT.TT_IdentCast:
                    return IdentCast();
                case TT.TT_IdentInline:
                    Node n = IdentInline();
                    if (sign < 0)
                    {
                        n = NodeFactory.GenMathNode(MathNodeOperator.MULTI, n, new ContentNode<int>(-1));
                    }
                    return n;
                case TT.TT_IdentISSET:
                    return IdentIsset();

                case TT.TT_IdentTRUEFALSE:
                    Node tf = new ContentNode<bool>(((TokenWithContent<bool>)Current).Content);
                    Accept(TT.TT_IdentTRUEFALSE);
                    return tf;
                case TT.TT_String:
                    Node s = new ContentNode<string>(((TokenWithContent<string>)Current).Content);
                    Accept(TT.TT_String);
                    return s;
                default:
#warning Script Implement Error Handling
                    Console.WriteLine("Error - 6");
                    break;
            }

            return null;
        }

        protected Node Multiplikation()
        {
            Node result = Klammer();

            while (Current.TokenType == TT.TT_Multi || 
                   Current.TokenType == TT.TT_Divide || 
                   Current.TokenType == TT.TT_Percent ||
                   Current.TokenType == TT.TT_Power)
            {
                switch (Current.TokenType)
                {
                    case TT.TT_Multi:
                        Accept(TT.TT_Multi);
                        result = NodeFactory.GenMathNode(MathNodeOperator.MULTI, result, Klammer());                        
                        break;
                    case TT.TT_Divide:
                        Accept(TT.TT_Divide);
                        result = NodeFactory.GenMathNode(MathNodeOperator.DIVIDE, result, Klammer());
                        break;
                    case TT.TT_Percent:
                        Accept(TT.TT_Percent);
                        result = NodeFactory.GenMathNode(MathNodeOperator.MODULO, result, Klammer());
                        break;
                    case TT.TT_Power:
                        Accept(TT.TT_Power);
                        result = NodeFactory.GenMathNode(MathNodeOperator.POWER, result, Klammer());
                        break; 
                    default:
#warning Script Implement Error Handling
                        Console.WriteLine("Error - 7");
                        break;
                }
            }

            return result;
        }

        protected Node Zahl()
        {
            if (!Current.HasContent)
#warning Script Implement Error Handling
                Console.WriteLine("Error - 8");
            if (Current.TokenType == TT.TT_DoubleNumber)
            {
                TokenWithContent<double> d = (TokenWithContent<double>)Current;
                Accept(TT.TT_DoubleNumber);
                return new ContentNode<double>(d.Content);
            }
            TokenWithContent<int> i = (TokenWithContent<int>)Current;
            Accept(TT.TT_IntNumber);
            return new ContentNode<int>(i.Content);
        }

        #endregion //Mathematik

        protected Node Variable()
        {
            Accept(TT.TT_Dollar);
            if (!Current.HasContent || Current.TokenType != TT.TT_Ident)
            {
#warning Script Implement Error Handling
                Console.WriteLine("Error - 9");
                return null;
            }
            TokenWithContent<string> s = (TokenWithContent<string>)Current;
            string varName = s.Content;
            Accept(TT.TT_Ident);
            return GetVariableAsNode(varName);
        }

        protected Node VariableAssignment()
        {
            Accept(TT.TT_Dollar);
            if(!Current.HasContent || Current.TokenType != TT.TT_Ident)
            {
#warning Script Implement Error Handling
                Console.WriteLine("Error - 10");
                return null;
            }
            TokenWithContent<string> s = (TokenWithContent<string>)Current;
            string varName = s.Content;
            Accept(TT.TT_Ident);

            //"="
            Accept(TT.TT_EqualSign);
            Node a = null;
            switch (Current.TokenType)
            {
                case TT.TT_IdentID:
                    a = ID();
                    break;
                case TT.TT_IdentCALLC:
                    a = CALLC();
                    break;
                case TT.TT_IdentCALLO:
                    a = CALLO();
                    break;
                case TT.TT_IdentTRUEFALSE:
                case TT.TT_Minus:
                case TT.TT_Plus:
                case TT.TT_ParenLeft:
                case TT.TT_IntNumber:
                case TT.TT_DoubleNumber:
                case TT.TT_Dollar:
                case TT.TT_IdentInline:
                case TT.TT_IdentCast:
                case TT.TT_String:
                    //Node a = Addition();
                    a = Addition();
                    break;
                

                default:
#warning Script Implement Error Handling
                    Console.WriteLine("Error - 11");
                    break;
            }

            if (a != null && a.IsContentNode)
            {
                if (VariableNodeExists(varName))
                {
                    Node varNode = GetVariableAsNode(varName);
                    if (varNode != null && a.ContentType != varNode.ContentType)
                    {
#warning Script Implement Error Handling
                        //need to cast 
                        Console.WriteLine("Error - 102");
                    }

                }
                if (a.ContentType == typeof(int))
                    return new VariableAssignmentNode<int>(GetVaraible<int>(varName), a);
                else if (a.ContentType == typeof(double))
                    return new VariableAssignmentNode<double>(GetVaraible<double>(varName), a);
                else if (a.ContentType == typeof(bool))
                    return new VariableAssignmentNode<bool>(GetVaraible<bool>(varName), a);
                else if (a.ContentType == typeof(string))
                    return new VariableAssignmentNode<string>(GetVaraible<string>(varName), a);
            }

            return null;
        }

        protected Node CALLC()
        {
            Accept(TT.TT_IdentCALLC);

            //Class Name
            if (Current.TokenType != TT.TT_String)
            {
                Console.WriteLine("Error");
            }
            string className = ((TokenWithContent<string>)Current).Content;
            Accept(TT.TT_String);

            Accept(TT.TT_Comma);

            //Class Name
            if (Current.TokenType != TT.TT_String)
            {
                Console.WriteLine("Error");
            }
            string methodName = ((TokenWithContent<string>)Current).Content;
            Accept(TT.TT_String);


            List<Node> parameter = new List<Node>();
            while (Current.TokenType == TT.TT_Comma)
            {
                Accept(TT.TT_Comma);
                switch (Current.TokenType)
                {
                    case TT.TT_Minus:
                    case TT.TT_Plus:
                    case TT.TT_ParenLeft:
                    case TT.TT_IntNumber:
                    case TT.TT_DoubleNumber:
                    case TT.TT_Dollar:
                    case TT.TT_String:
                    case TT.TT_IdentTRUEFALSE:
                        parameter.Add(Addition());
                        break;
                    case TT.TT_IdentCALLC:
                        parameter.Add(CALLC());
                        break;
                    case TT.TT_IdentCALLO:
                        parameter.Add(CALLO());
                        break;
                    default:
                        Console.WriteLine("Error");
                        break;
                }
            }
            Accept(TT.TT_ParenRight);
            
            return NodeFactory.GenCallNodeStatic(className, methodName, parameter);          
        }

        protected Node CALLO()
        {
            Accept(TT.TT_IdentCALLO);

            //Class Name
            if (Current.TokenType != TT.TT_String)
            {
#warning Script Implement Error Handling
                Console.WriteLine("Error - 12");
            }
            string className = ((TokenWithContent<string>)Current).Content;
            Accept(TT.TT_String);

            Accept(TT.TT_Comma);

            //Class Name
            if (Current.TokenType != TT.TT_String)
            {
#warning Script Implement Error Handling
                Console.WriteLine("Error - 13");
            }
            string methodName = ((TokenWithContent<string>)Current).Content;
            Accept(TT.TT_String);


            List<Node> parameter = new List<Node>();
            while (Current.TokenType == TT.TT_Comma)
            {
                Accept(TT.TT_Comma);
                switch (Current.TokenType)
                {
                    case TT.TT_IdentTRUEFALSE:
                    case TT.TT_Minus:
                    case TT.TT_Plus:
                    case TT.TT_ParenLeft:
                    case TT.TT_IntNumber:
                    case TT.TT_DoubleNumber:
                    case TT.TT_Dollar:
                    case TT.TT_String:
                    case TT.TT_IdentCast:
                        parameter.Add(Addition());
                        break;
                    case TT.TT_IdentCALLC:
                        parameter.Add(CALLC());
                        break;
                    case TT.TT_IdentCALLO:
                        parameter.Add(CALLO());
                        break;
                    default:
#warning Script Implement Error Handling
                        Console.WriteLine("Error - 14");
                        break;
                }
            }
            Accept(TT.TT_ParenRight);

            return NodeFactory.GenCallNodeObject(className, methodName, parameter);
        }

        protected Node IdentCast()
        {
            string name = ((TokenWithContent<string>)Current).Content;
            Accept(TT.TT_IdentCast);

            Node a = Addition();
            if (a == null)
            {
#warning Script Implement Error Handling
                Console.WriteLine("Error - 15");
                return null;
            }
            if (!a.IsContentNode)
            {
#warning Script Implement Error Handling
                Console.WriteLine("Error - 16");
                return null;
            }

            Node con = null;
            switch (name)
            {
                case "INT":
                    if (a.ContentType == typeof(double))
                    {
                        con = new CastNode<int, double>((ContentNode<double>)a);
                    }
                    else
                    {
                        //Int muss nicht auf int gecastet werden
                        con = a;
                    }
                    break;                   
                case "DOUBLE":
                    if (a.ContentType == typeof(int))
                    {
                        con = new CastNode<double, int>((ContentNode<int>)a);
                    }
                    else
                    {
                        con = a;
                    }
                    break;
            }
            Accept(TT.TT_ParenRight);
            return con;
        }

        protected Node IdentInline()
        {

            string name = ((TokenWithContent<string>)Current).Content;
            Accept(TT.TT_IdentInline);

            List<Node> parameter = new List<Node>();
            while (Current.TokenType != TT.TT_ParenRight)
            {
                
                switch (Current.TokenType)
                {
                    case TT.TT_IdentTRUEFALSE:
                    case TT.TT_Minus:
                    case TT.TT_Plus:
                    case TT.TT_ParenLeft:
                    case TT.TT_IntNumber:
                    case TT.TT_DoubleNumber:
                    case TT.TT_Dollar:
                    case TT.TT_String:
                    case TT.TT_IdentInline:
                    case TT.TT_IdentCast:
                        parameter.Add(Addition());
                        break;
                    case TT.TT_IdentCALLC:
                        parameter.Add(CALLC());
                        break;
                    case TT.TT_IdentCALLO:
                        parameter.Add(CALLO());
                        break;
                    case TT.TT_IdentID:
                        parameter.Add(ID());
                        break;
                    default:
#warning Script Implement Error Handling
                        Console.WriteLine("Error - 17");
                        break;
                }
                if (Current.TokenType == TT.TT_Comma)
                {
                    Accept(TT.TT_Comma);
                }
            }
            Accept(TT.TT_ParenRight);
            Function f = FunctionLib.GetFunction(name);

            if (f == null)
            {
#warning Script Implement Error Handling
                Console.WriteLine("Error - 18");
            }

            return f.GetFunctionNode(parameter);
        }

        protected Node IdentIsset()
        {
            Accept(TT.TT_IdentISSET);

            Node res = null;
            Node v = Variable();
            if(v.ContentType == typeof(int))
            {
                res = new IsSetNode<int>((VariableNode<int>)v);
            }
            else if (v.ContentType == typeof(double))
            {
                res = new IsSetNode<double>((VariableNode<double>)v);
            }
            else if (v.ContentType == typeof(bool))
            {
                res = new IsSetNode<bool>((VariableNode<bool>)v);
            }

            Accept(TT.TT_ParenRight);

            return res;
        }

        protected Node Ident()
        {
            string name = ((TokenWithContent<string>)Current).Content;
            Accept(TT.TT_Ident);

            List<Node> parameter = new List<Node>();
            while (Current.TokenType != TT.TT_Semicolon)
            {
                switch (Current.TokenType)
                {
                    case TT.TT_IdentTRUEFALSE:
                    case TT.TT_Minus:
                    case TT.TT_Plus:
                    case TT.TT_ParenLeft:
                    case TT.TT_IntNumber:
                    case TT.TT_DoubleNumber:
                    case TT.TT_Dollar:
                    case TT.TT_String:
                    case TT.TT_IdentInline:
                    case TT.TT_IdentCast:
                        parameter.Add(Klammer());
                        break;
                    case TT.TT_IdentCALLC:
                        parameter.Add(CALLC());
                        break;
                    case TT.TT_IdentCALLO:
                        parameter.Add(CALLO());
                        break;
                    case TT.TT_IdentID:
                        parameter.Add(ID());
                        break;
                    default:
#warning Script Implement Error Handling
                        Console.WriteLine("Error - 19");
                        break;
                }
            }

            Function f = FunctionLib.GetFunction(name);

            if (f == null)
            {
#warning Script Implement Error Handling
                Console.WriteLine("Error - 20(Unknown function({0}) in {1}!", name, this.FunctionName);
            }
            
            return f.GetFunctionNode(parameter);
        }

        #region Bedingungen

        protected Node FOR()
        {
            Accept(TT.TT_IdentFOR);

            //Init Node
            Node init = VariableAssignment();            
            if (init == null)
            {
#warning Script Implement Error Handling
                Console.WriteLine("Error - 21");
                return null;
            }
            Accept(TT.TT_Semicolon);

            //Entscheidungs Node
            Node decNode = Addition();
            if (decNode == null)
            {
#warning Script Implement Error Handling
                Console.WriteLine("Error - 22");
                return null;
            }
            if (!decNode.IsContentNode)
            {
#warning Script Implement Error Handling
                Console.WriteLine("Error - 23");
                return null;
            }
            ContentNode<bool> dec;
            if (decNode.ContentType == typeof(bool))
            {
                dec = (ContentNode<bool>)decNode;
            }
            else
            {
                if (decNode.ContentType == typeof(int))
                {
                    dec = new CastNode<bool, int>((ContentNode<int>)decNode);
                }
                else if (decNode.ContentType == typeof(double))
                {
                    dec = new CastNode<bool, double>((ContentNode<double>)decNode);
                }
                else
                {
                    dec = new CastNode<bool,string>((ContentNode<string>)decNode);
                }
            }
            Accept(TT.TT_Semicolon);

            //Iter Node
            Node iter = VariableAssignment();
            if (iter == null)
            {
#warning Script Implement Error Handling
                Console.WriteLine("Error - 24");
                return null;
            }
            Accept(TT.TT_ParenRight);
            Accept(TT.TT_BlockBegin);
            List<Node> nodes = new List<Node>();
            while (Current.TokenType != TT.TT_BlockEnd)
            {
                Node n = Input();
                if (n != null)
                    nodes.Add(n);
            }
            Accept(TT.TT_BlockEnd);

            return new FORNode(init, dec, iter, nodes);
        }

        protected Node IF()
        {
            Accept(TT.TT_IdentIF);

            Node a = Addition();
            if (a == null)
            {
#warning Script Implement Error Handling
                Console.WriteLine("Error - 25");
                return null;
            }
            if (!a.IsContentNode)
            {
#warning Script Implement Error Handling
                Console.WriteLine("Error - 26");
                return null;
            }

            ContentNode<bool> dec;
            if (a.ContentType == typeof(bool))
            {
                dec = (ContentNode<bool>)a;
            }
            else
            {
                if (a.ContentType == typeof(int))
                {
                    dec = new CastNode<bool, int>((ContentNode<int>)a);
                }
                else if (a.ContentType == typeof(double))
                {
                    dec = new CastNode<bool, double>((ContentNode<double>)a);
                }
                else
                {
                    dec = null;
                }
            }

            Accept(TT.TT_ParenRight);
            Accept(TT.TT_BlockBegin);

            List<Node> ifnodes = new List<Node>();
            while (Current.TokenType != TT.TT_BlockEnd)
            {
                Node n = Input();
                if(n != null)
                    ifnodes.Add(n);
            }
            Accept(TT.TT_BlockEnd);

            if (Current.TokenType == TT.TT_IdentElse)
            {
                Accept(TT.TT_IdentElse);
                Accept(TT.TT_BlockBegin);
                List<Node> elsenodes = new List<Node>();
                while (Current.TokenType != TT.TT_BlockEnd)
                {
                    Node n = Input();
                    if (n != null)
                        elsenodes.Add(n);
                }
                Accept(TT.TT_BlockEnd);
                return new IFNode(dec, ifnodes, elsenodes);
            }

            return new IFNode(dec, ifnodes);
        }

        protected Node WHILE()
        {
            Accept(TT.TT_IdentWHILE);

            //Entscheidungs Node
            Node decNode = Addition();
            if (decNode == null)
            {
#warning Script Implement Error Handling
                Console.WriteLine("Error - 27");
                return null;
            }
            if (!decNode.IsContentNode)
            {
#warning Script Implement Error Handling
                Console.WriteLine("Error - 28");
                return null;
            }
            ContentNode<bool> dec;
            if (decNode.ContentType == typeof(bool))
            {
                dec = (ContentNode<bool>)decNode;
            }
            else
            {
                if (decNode.ContentType == typeof(int))
                {
                    dec = new CastNode<bool, int>((ContentNode<int>)decNode);
                }
                else if (decNode.ContentType == typeof(double))
                {
                    dec = new CastNode<bool, double>((ContentNode<double>)decNode);
                }
                else
                {
                    dec = new CastNode<bool, string>((ContentNode<string>)decNode);
                }
            }

            Accept(TT.TT_ParenRight);
            Accept(TT.TT_BlockBegin);
            List<Node> nodes = new List<Node>();
            while (Current.TokenType != TT.TT_BlockEnd)
            {
                Node n = Input();
                if (n != null)
                    nodes.Add(n);
            }
            Accept(TT.TT_BlockEnd);

            return new WHILENode(dec, nodes);
        }

        #endregion //Bedingungen

        protected Node PAUSE()
        {
            Accept(TT.TT_IdentPAUSE);
            return new PauseNode();
        }

        protected Node ID()
        {
            Accept(TT.TT_IdentID);
            return new IDNode();
        }

        protected Node RETURN()
        {
            Accept(TT.TT_IdentRETURN);

            Node rval = null;
            switch (Current.TokenType)
            {
                case TT.TT_IdentTRUEFALSE:
                case TT.TT_Minus:
                case TT.TT_Plus:
                case TT.TT_ParenLeft:
                case TT.TT_IntNumber:
                case TT.TT_DoubleNumber:
                case TT.TT_Dollar:
                case TT.TT_String:
                case TT.TT_IdentCast:
                    rval = Addition();
                    break;
                case TT.TT_IdentCALLC:
                    rval = CALLC();
                    break;
                case TT.TT_IdentCALLO:
                    rval = CALLO();
                    break;
                case TT.TT_Semicolon:
                    rval = null;
                    break;
                default:
#warning Script Implement Error Handling
                    Console.WriteLine("Error - 101");
                    break;
            }

            if (rval != null && rval.IsContentNode)
            {
                Node varNode = GetVariableAsNode("RETURN");
                if (varNode != null && rval.ContentType != varNode.ContentType)
                {
#warning Script Implement Error Handling
                    //need to cast 
                    Console.WriteLine("Error - 103");
                }

                if (rval.ContentType == typeof(int))
                    rval = new VariableAssignmentNode<int>(GetVaraible<int>("RETURN"), rval);
                else if (rval.ContentType == typeof(double))
                    rval = new VariableAssignmentNode<double>(GetVaraible<double>("RETURN"), rval);
                else if (rval.ContentType == typeof(bool))
                    rval = new VariableAssignmentNode<bool>(GetVaraible<bool>("RETURN"), rval);
                else if (rval.ContentType == typeof(string))
                    rval = new VariableAssignmentNode<string>(GetVaraible<string>("RETURN"), rval);
            }

            /*
            Node retNode = GetVariableAsNode("RETURN");

            if (retNode != null && rval != null && rval.IsContentNode && rval.ContentType == retNode.ContentType)
            {

            }

            /*
            if (a != null && a.IsContentNode)
            {
                if (a.ContentType == typeof(int))
                    return new VariableAssignmentNode<int>(GetVaraible<int>(varName), a);
                else if (a.ContentType == typeof(double))
                    return new VariableAssignmentNode<double>(GetVaraible<double>(varName), a);
                else if (a.ContentType == typeof(bool))
                    return new VariableAssignmentNode<bool>(GetVaraible<bool>(varName), a);
            }*/
            Node exe = NodeFactory.GenCallNodeStatic("LaserControl.ScriptV2.Exceptions.ExceptionRaiser", "RaiseReturn", new List<Node>());
            if (rval != null)
            {
                NodeCollection nc = new NodeCollection();
                nc.Add(rval);
                nc.Add(exe);
                return nc;
            }
            return exe;
        }
    }
}
