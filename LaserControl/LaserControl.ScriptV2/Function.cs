using LaserControl.Data;
using LaserControl.ScriptV2.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserControl.ScriptV2
{
    public class Function
    {

        protected List<string> ParameterNames = null;
        protected List<Type> ParameterTypes = null;

        protected List<Token> TokenList = null;

        public Type ReturnType
        {
            get;
            protected set;
        }

        public string Code
        {
            get;
            protected set;
        }

        public string FunctionName
        {
            get;
            protected set;
        }

        public int ParameterCount
        {
            get
            {
                return ParameterTypes.Count;
            }
        }

        public Function(string name, string code)
            : this(name, code, typeof(bool))
        {
            //nix 
        }

        public Function(string name, string code, Type returnType)
        {
            FunctionName = name.ToUpper();
            Code = code;
            ReturnType = returnType;
            this.TokenList = Scanner.GenerateTokenList(Code);

            ParameterNames = new List<string>();
            ParameterTypes = new List<Type>();
        }

        public void Reset(string code, Type returnType)
        {
            Code = code;
            ReturnType = returnType;

            TokenList.Clear();
            this.TokenList = Scanner.GenerateTokenList(Code);

            ParameterNames.Clear();
            ParameterTypes.Clear();
        }

        #region Add Parameter

        public void AddParameter(string name, Type type)
        {
            ParameterNames.Add(name.ToUpper());
            ParameterTypes.Add(type);
        }

        public void AddParameterInt(string name)
        {
            ParameterNames.Add(name.ToUpper());
            ParameterTypes.Add(typeof(int));
        }

        public void AddParameterDouble(string name)
        {
            ParameterNames.Add(name.ToUpper());
            ParameterTypes.Add(typeof(double));
        }

        public void AddParameterString(string name)
        {
            ParameterNames.Add(name.ToUpper());
            ParameterTypes.Add(typeof(string));
        }

        public void AddParameterBool(string name)
        {
            ParameterNames.Add(name.ToUpper());
            ParameterTypes.Add(typeof(bool));
        }

        #endregion //Add Parameter

        public Node GetFunctionNode(List<Node> paramNodes, bool displayLines = false)
        {
            if (this.ReturnType == typeof(int))
            {
                return new FunctionNode<int>(this.FunctionName, this.TokenList, this.ParameterNames, this.ParameterTypes, paramNodes, displayLines);
            }
            else if (this.ReturnType == typeof(bool))
            {
                return new FunctionNode<bool>(this.FunctionName, this.TokenList, this.ParameterNames, this.ParameterTypes, paramNodes, displayLines);
            }
            else if (this.ReturnType == typeof(double))
            {
                return new FunctionNode<double>(this.FunctionName, this.TokenList, this.ParameterNames, this.ParameterTypes, paramNodes, displayLines);
            }
            return null;
        }

        public Type GetParamtype(int position)
        {
            return ParameterTypes[position];
        }

        public string GetParamName(int position)
        {
            return ParameterNames[position];
        }

        public void Clear()
        {
            TokenList.Clear();
            ParameterTypes.Clear();
            ParameterNames.Clear();
        }

        #region Load & Save

        public static Function Load(string name)
        {
            name = name.ToUpper();
            DataSafe ds = new DataSafe(Data.Paths.SettingsScriptFunctionsPath, name);

            //Function f = new Function(name, ds.Strings["Code", ""]);
            Type t = typeof(bool);
            int rt = ds.Ints["ReturnType", 0];            
            t = Function.IndexToType(rt);
            Function f = new Function(name, ds.Strings["Code", ""], t);
            int pCount = ds.Ints["Parameter", 0];
            for (int i = 0; i < pCount; ++i)
            {
                string pname = ds.Strings["ParamName-" + i, ""];
                f.AddParameter(pname, Function.IndexToType(ds.Ints["ParamType-" + i, 0]));
            }

            return f;
        }

        public void Save()
        {
            DataSafe ds = new DataSafe(Data.Paths.SettingsScriptFunctionsPath, this.FunctionName);
            ds.Ints["ReturnType"] = Function.TypeToIndex(this.ReturnType);
            ds.Strings["Code"] = this.Code;
            ds.Ints["Parameter"] = ParameterCount;
            for (int i = 0; i < ParameterCount; ++i)
            {
                ds.Strings["ParamName-" + i] = ParameterNames[i];
                ds.Ints["ParamType-" + i] = Function.TypeToIndex(ParameterTypes[i]);
            }
        }

        #endregion //Load & Save

        #region Static Function for Converting

        public static int TypeToIndex(Type t)
        {
            if (t == typeof(bool))
            {
                return 0;
            }
            if (t == typeof(int))
            {
                return 1;
            }
            if (t == typeof(double))
            {
                return 2;
            }
            if (t == typeof(string))
            {
                return 3;
            }

            //Default is 0;
            return 0;
        }

        public static Type IndexToType(int index)
        {
            switch (index)
            {
                case 0:
                    return typeof(bool);
                case 1:
                    return typeof(int);
                case 2:
                    return typeof(double);
                case 3:
                    return typeof(string);
                default:
                    return typeof(bool);
            }
        }

        public static string TypeToString(Type t)
        {
            if (t == typeof(bool))
            {
                return "bool";
            }
            if (t == typeof(int))
            {
                return "int";
            }
            if (t == typeof(double))
            {
                return "double";
            }
            if (t == typeof(string))
            {
                return "string";
            }

            //Default is 0;
            return "bool";
        }

        public static Type StringToType(string s)
        {
            switch (s)
            {
                case "bool":
                    return typeof(bool);
                case "int":
                    return typeof(int);
                case "double":
                    return typeof(double);
                case "string":
                    return typeof(string);
                default:
                    return typeof(bool);
            }
        }
        #endregion //Static Function for Converting
    }
}
