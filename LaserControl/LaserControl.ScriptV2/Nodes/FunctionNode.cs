using LaserControl.ScriptV2.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LaserControl.ScriptV2.Nodes
{
    public class FunctionNode<T> : ContentNode<T>
        where T:IConvertible
    {
        public override int ScriptThreadID
        {
            get
            {
                return base.ScriptThreadID;
            }
            set
            {
                base.ScriptThreadID = value;
                foreach (Node n in ParameterNodes)
                {
                    n.ScriptThreadID = value;
                }
            }
        }
        protected string FunctionName;
        protected List<Token> TokenList = null;
        protected List<string> ParameterNames = null;
        protected List<Type> ParameterTypes = null;
        protected List<Node> ParameterNodes = null;
        protected bool DisplayLines = false;

        public FunctionNode(string name, List<Token> tokens, List<string> paramNames, List<Type> paramTypes, List<Node> paramNodes, bool displayLines = false)
            : base()
        {
            FunctionName = name;
            TokenList = tokens;
            ParameterNames = paramNames;
            ParameterNodes = paramNodes;
            ParameterTypes = paramTypes;
            DisplayLines = displayLines;
        }

        protected override void InnerEvaluate()
        {
            LSParser p = new LSParser(TokenList, this.FunctionName, this.DisplayLines);

            VariableNode<T> returnNode = new VariableNode<T>("RETURN");

            List<Node> vars = new List<Node>();
            vars.Add(returnNode);

            #region Parameter
            for (int i = 0; i < ParameterNames.Count; ++i)
            {
                if (ParameterTypes[i] == typeof(int))
                {                    
                    
                    if (i >= ParameterNodes.Count)
                    {
                        vars.Add(new VariableNode<int>(ParameterNames[i], 0, false));
                    }
                    else
                    {
                        int intVal = 0;
                        ParameterNodes[i].Evaluate(out intVal); 
                        vars.Add(new VariableNode<int>(ParameterNames[i], intVal, true));
                    }                    
                }
                else if (ParameterTypes[i] == typeof(double))
                {
                    if (i >= ParameterNodes.Count)
                    {
                        vars.Add(new VariableNode<double>(ParameterNames[i], 0.0, false));
                    }
                    else
                    {
                        double dVal = 0;
                        ParameterNodes[i].Evaluate(out dVal);
                        vars.Add(new VariableNode<double>(ParameterNames[i], dVal, true));
                    }
                }
                else if (ParameterTypes[i] == typeof(bool))
                {
                    if (i >= ParameterNodes.Count)
                    {
                        vars.Add(new VariableNode<bool>(ParameterNames[i], false, false));
                    }
                    else
                    {
                        bool bVal = false;
                        ParameterNodes[i].Evaluate(out bVal);
                        vars.Add(new VariableNode<bool>(ParameterNames[i], bVal, true));
                    }
                }
                else if (ParameterTypes[i] == typeof(string))
                {
                    if (i >= ParameterNodes.Count)
                    {
                        vars.Add(new VariableNode<string>(ParameterNames[i], "", false));
                    }
                    else
                    {
                        string sVal = "";
                        ParameterNodes[i].Evaluate(out sVal);
                        vars.Add(new VariableNode<string>(ParameterNames[i], sVal, true));
                    }
                }
            }
            #endregion //Parameter

            List<Node> nodes = null;
            //try
            //{
                nodes = p.GenerateNodes(vars);
            //}
            //catch(Exception ex)
            //{
            //    Console.WriteLine("Error while creating nodes for function!");
            //    return;
            //}
            if (nodes != null)
            {
                foreach (Node n in nodes)
                {
                    bool s = false;
                    try
                    {
                        n.ScriptThreadID = this.ScriptThreadID;
                        n.Evaluate(out s);
                        //Node.PauseWait();
                    }
                    /*catch(ThreadAbortException ex)
                    {
                        //Mache nichts
                    }
                    catch (ReturnException returnExe)
                    {
                        
                    }*/
                    catch(Exception ex)
                    {
                        Exception inner = ex.InnerException;
                        if (inner == null)
                            inner = ex;

                        if (inner.GetType() == typeof(ThreadAbortException))
                        {
                            //mache nichts
                        }
                        else if (inner.GetType() == typeof(ReturnException))
                        {
                            //Console.WriteLine("Hallo Welt");

                            //Clean current node
                            n.Clean();
                            
                            //Clean all nodes!
                            int pos = nodes.IndexOf(n);                            
                            for (int i = pos + 1; i < nodes.Count; ++i)
                            {
                                nodes[i].Clean();
                            }

                            //leave for loop
                            break;
                        }
                        else if (inner.GetType() == typeof(ParserException))
                        {
                            //weiterleitung der exception
                            throw ex;
                        }
                        else
                        {
                            Console.WriteLine("Error in evaluation of node!");
                        }
                    }
                    n.Clean();
                }
                /*foreach (Node n in nodes)
                {
                    n.Clean();
                }*/
            }

            //Ende der Methode
            this.Content = returnNode.Content;

            //Clean Param Nodes
            nodes.Clear();
            vars.Clear();
        }

        public override void Clean()
        {
            base.Clean();
            /*TokenList.Clear();
            TokenList = null;

            ParameterNames.Clear();
            ParameterNames = null;

            ParameterTypes.Clear();
            ParameterTypes = null;*/

            ParameterNodes.Clear();
            ParameterNodes = null;
        }
    }
}
