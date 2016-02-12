using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserControl.ScriptV2.Nodes
{
    public enum MathNodeOperator
    {
        PLUS = 0,
        MINUS = 1,
        MULTI = 2,
        DIVIDE = 3,
        MODULO = 4,
        POWER = 5
    }

    public class MathNode<T1,T2> : ContentNode<T1>
        where T1 : IConvertible  
        where T2 : IConvertible
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
                LeftNode.ScriptThreadID = value;
                RightNode.ScriptThreadID = value;
            }
        }

        public ContentNode<T1> LeftNode
        {
            get;
            protected set;
        }

        public ContentNode<T2> RightNode
        {
            get;
            protected set;
        }

        public MathNodeOperator Operator
        {
            get;
            protected set;
        }

        public MathNode(MathNodeOperator mathOperator, ContentNode<T1> left, ContentNode<T2> right) : base()
        {
            LeftNode = left;
            RightNode = right;
            this.Operator = mathOperator;
        }

        protected override void InnerEvaluate()
        {
            switch (this.Operator)
            {
                case MathNodeOperator.PLUS:
                    EvaluatePlus();
                    break;
                case MathNodeOperator.MINUS:
                    EvaluateMinus();
                    break;
                case MathNodeOperator.MULTI:
                    EvaluateMult();
                    break;
                case MathNodeOperator.DIVIDE:
                    EvaluateDivide();
                    break;
                case MathNodeOperator.MODULO:
                    EvaluateModulo();
                    break;
                case MathNodeOperator.POWER:
                    EvaluatePower();
                    break;
            }            
        }

        private void EvaluatePlus()
        {
            if (typeof(T1) == typeof(int))
            {
                int l = 0, r = 0;
                LeftNode.Evaluate(out l);
                RightNode.Evaluate(out r);
                this.Content = (T1)(Object)(l + r);
            }
            else if (typeof(T1) == typeof(double))
            {
                double l = 0, r = 0;
                LeftNode.Evaluate(out l);
                RightNode.Evaluate(out r);
                this.Content = (T1)(Object)(l + r);
            }
            else if (typeof(T1) == typeof(string))
            {
                string l = "", r = "";
                LeftNode.Evaluate(out l);
                RightNode.Evaluate(out r);
                this.Content = (T1)(Object)(string.Format("{0}{1}",l , r));
            } 
        }

        private void EvaluateMinus()
        {
            if (typeof(T1) == typeof(int))
            {
                int l = 0, r = 0;
                LeftNode.Evaluate(out l);
                RightNode.Evaluate(out r);
                this.Content = (T1)(Object)(l - r);
            }
            else if (typeof(T1) == typeof(double))
            {
                double l = 0, r = 0;
                LeftNode.Evaluate(out l);
                RightNode.Evaluate(out r);
                this.Content = (T1)(Object)(l - r);
            }
        }

        private void EvaluateMult()
        {
            if (typeof(T1) == typeof(int))
            {
                int l = 0, r = 0;
                LeftNode.Evaluate(out l);
                RightNode.Evaluate(out r);
                this.Content = (T1)(Object)(l * r);
            }
            else if (typeof(T1) == typeof(double))
            {
                double l = 0, r = 0;
                LeftNode.Evaluate(out l);
                RightNode.Evaluate(out r);
                this.Content = (T1)(Object)(l * r);
            }
        }

        private void EvaluateDivide()
        {
            if (typeof(T1) == typeof(int))
            {
                int l = 0, r = 0;
                LeftNode.Evaluate(out l);
                RightNode.Evaluate(out r);
                this.Content = (T1)(Object)(l / r);
            }
            else if (typeof(T1) == typeof(double))
            {
                double l = 0, r = 0;
                LeftNode.Evaluate(out l);
                RightNode.Evaluate(out r);
                this.Content = (T1)(Object)(l / r);
            }
        }

        private void EvaluateModulo()
        {
            if (typeof(T1) == typeof(int))
            {
                int l = 0, r = 0;
                LeftNode.Evaluate(out l);
                RightNode.Evaluate(out r);
                this.Content = (T1)(Object)(l % r);
            }
            else if (typeof(T1) == typeof(double))
            {
                double l = 0, r = 0;
                LeftNode.Evaluate(out l);
                RightNode.Evaluate(out r);
                this.Content = (T1)(Object)(l % r);
            }
        }

        private void EvaluatePower()
        {
            if (typeof(T1) == typeof(int))
            {
                int l = 0, r = 0;
                LeftNode.Evaluate(out l);
                RightNode.Evaluate(out r);
                int res = (int)Math.Pow(l, r);
                this.Content = (T1)(Object)(res);
            }
            else if (typeof(T1) == typeof(double))
            {
                double l = 0, r = 0;
                LeftNode.Evaluate(out l);
                RightNode.Evaluate(out r);
                this.Content = (T1)(Object)(Math.Pow(l,r));
            }
        }

        public override void Clean()
        {
            base.Clean();
            LeftNode.Clean();
            LeftNode = null;
            RightNode.Clean();
            RightNode = null;
        }
    }
}
