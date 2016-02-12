using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using LaserControl.Library;

namespace LaserControl.ScriptV2.Nodes
{
    public class NodeFactory
    {

        public static Node GenMathNode(MathNodeOperator op, Node left, Node right)
        {

            Type l = left.ContentType;
            Type r = right.ContentType;

            if (l == typeof(int))
            {
                if (r == typeof(int))                
                    return new MathNode<int,int>(op, (ContentNode<int>)left, (ContentNode<int>)right);
                else if (r == typeof(double))
                {
                    Console.WriteLine("Warning implicit conversion int to double!");
                    return new MathNode<double, double>(op, new CastNode<double, int>((ContentNode<int>)left), (ContentNode<double>)right);
                }
            }
            else if (l == typeof(double))
            {
                if (r == typeof(int))
                {
                    Console.WriteLine("Warning implicit conversion int to double!");
                    return new MathNode<double, double>(op, (ContentNode<double>)left, new CastNode<double, int>((ContentNode<int>)right));
                }
                else if (r == typeof(double))
                    return new MathNode<double, double>(op, (ContentNode<double>)left, (ContentNode<double>)right);
            } if (l == typeof(string))
            {
                if (r == typeof(int))
                    return new MathNode<string, int>(op, (ContentNode<string>)left, (ContentNode<int>)right);
                else if (r == typeof(double))
                    return new MathNode<string, double>(op, (ContentNode<string>)left, (ContentNode<double>)right);
            }

            return null;
        }

        public static Node GenCallNodeStatic(string cname, string fname, List<Node> parameter)
        {
            Type classType = Type.GetType(cname);
            Type[] parameterTypes = new Type[parameter.Count];
            for (int i = 0; i < parameter.Count; ++i)
            {
                parameterTypes[i] = parameter[i].ContentType;
            }
            MethodInfo method = classType.GetMethod(fname, parameterTypes);

            Type returnType = method.ReturnType;

            if (returnType == typeof(void) || returnType == typeof(bool))
            {
                return new CSCallFunctionNode<bool>(method, parameter);
            }
            else if (returnType == typeof(int))
            {
                return new CSCallFunctionNode<int>(method, parameter);
            }
            else if (returnType == typeof(double))
            {
                return new CSCallFunctionNode<double>(method, parameter);
            }

            //Test auf System.Void
            Console.WriteLine(returnType);

            return null;
        }

        public static Node GenCallNodeObject(string oname, string fname, List<Node> parameter)
        {
            object o = null;
            Type classType = null;
            oname = oname.ToUpper();
            switch (oname)
            {
                /*case "GlobTestForm":
                    classType = GlobalObjects.GlobTestForm.GetType();
                    o = GlobalObjects.GlobTestForm;
                    break;*/
                case "HWC":
                case "HARDWARECONTROLLER":
                    o = GlobalObjects.HWC;
                    classType = o.GetType();
                    break;
                case "CAMERA":
                    o = GlobalObjects.Camera;
                    classType = o.GetType();
                    break;

                default:

                    break;
            }
            Type[] parameterTypes = new Type[parameter.Count];
            for (int i = 0; i < parameter.Count; ++i)
            {
                parameterTypes[i] = parameter[i].ContentType;
            }
            MethodInfo method = classType.GetMethod(fname, parameterTypes);
            Type returnType = method.ReturnType;

            if (returnType == typeof(void) || returnType == typeof(bool))
            {
                return new CSCallFunctionNode<bool>(o,method, parameter);
            }
            else if (returnType == typeof(int))
            {
                return new CSCallFunctionNode<int>(o,method, parameter);
            }
            else if (returnType == typeof(double))
            {
                return new CSCallFunctionNode<double>(o,method, parameter);
            }

            //Test auf System.Void
            Console.WriteLine(returnType);

            return null;
        }

        public static Node GenLogicNode(LogicOperator logop, Node left, Node right)
        {
            Type l = left.ContentType;
            Type r = right.ContentType;

            if (l == typeof(int))
            {
                if (r == typeof(int))
                    return new LogicNode<int, int>(logop, (ContentNode<int>)left, (ContentNode<int>)right);
            }
            if (l == typeof(double))
            {
                if (r == typeof(double))
                {
                    return new LogicNode<double, double>(logop, (ContentNode<double>)left, (ContentNode<double>)right);
                }
            }
            if (l == typeof(bool))
            {
                if (r == typeof(bool))
                {
                    return new LogicNode<bool, bool>(logop, (ContentNode<bool>)left, (ContentNode<bool>)right);
                }
            }

            return null;
        }

    }
}
