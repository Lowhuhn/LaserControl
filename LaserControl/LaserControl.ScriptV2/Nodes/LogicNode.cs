using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserControl.ScriptV2.Nodes
{
    public enum LogicOperator
    {
        Equal = 0,
        Greater = 1,
        Lower = 2,
        UnEqual = 3,
        LowerEqual = 4
    }

    public class LogicNode<T1, T2> : ContentNode<bool>
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
                Left.ScriptThreadID = value;
                Right.ScriptThreadID = value;
            }
        }

        protected LogicOperator LogOp;
        protected ContentNode<T1> Left;
        protected ContentNode<T2> Right;

        public LogicNode(LogicOperator logOp, ContentNode<T1> l, ContentNode<T2> r)
        {
            LogOp = logOp;
            Left = l;
            Right = r;
        }

        protected override void InnerEvaluate()
        {
            switch (LogOp)
            {
                case LogicOperator.Equal:
                    this.EqualEvaluate();
                    break;
                case LogicOperator.UnEqual:
                    this.UNEqualEvaluate();
                    break;
                case LogicOperator.Lower:
                    this.LowerEvaluate();
                    break;
                case LogicOperator.Greater:
                    this.GreaterEvaluate();
                    break;
                case LogicOperator.LowerEqual:
                    this.LowerEqualEvaluate();
                    break;
            }
        }

        protected void EqualEvaluate()
        {

             if (typeof(T1) == typeof(int))
            {
                int a = 0;
                Left.Evaluate(out a);
                if (typeof(T2) == typeof(int))
                {
                    int b = 0;
                    Right.Evaluate(out b);
                    this.Content = (a == b);
                }
                else if (typeof(T2) == typeof(double))
                {
                    double b = 0;
                    Right.Evaluate(out b);
                    this.Content = a == b;
                }

            }

            if (typeof(T1) == typeof(double))
            {
                double a = 0.0;
                Left.Evaluate(out a);

                if (typeof(T2) == typeof(double))
                {
                    double b = 0.0;
                    Right.Evaluate(out b);
                    this.Content = a == b;
                }
            }

            if (typeof(T1) == typeof(bool))
            {
                bool a = false;
                Left.Evaluate(out a);

                if (typeof(T2) == typeof(bool))
                {
                    bool b = false;
                    Right.Evaluate(out b);
                    this.Content = a == b;
                }
            }

        }

        protected void UNEqualEvaluate()
        {

            if (typeof(T1) == typeof(int))
            {
                int a = 0;
                Left.Evaluate(out a);
                if (typeof(T2) == typeof(int))
                {
                    int b = 0;
                    Right.Evaluate(out b);
                    this.Content = (a != b);
                }
                else if (typeof(T2) == typeof(double))
                {
                    double b = 0;
                    Right.Evaluate(out b);
                    this.Content = a != b;
                }

            }

            if (typeof(T1) == typeof(double))
            {
                double a = 0.0;
                Left.Evaluate(out a);

                if (typeof(T2) == typeof(double))
                {
                    double b = 0.0;
                    Right.Evaluate(out b);
                    this.Content = a != b;
                }
            }

            if (typeof(T1) == typeof(bool))
            {
                bool a = false;
                Left.Evaluate(out a);

                if (typeof(T2) == typeof(bool))
                {
                    bool b = false;
                    Right.Evaluate(out b);
                    this.Content = a != b;
                }
            }

        }

        protected void GreaterEvaluate()
        {

            if (typeof(T1) == typeof(int))
            {
                int a = 0;
                Left.Evaluate(out a);
                if (typeof(T2) == typeof(int))
                {
                    int b = 0;
                    Right.Evaluate(out b);
                    this.Content = (a > b);
                }
                else if (typeof(T2) == typeof(double))
                {
                    double b = 0;
                    Right.Evaluate(out b);
                    this.Content = a > b;
                }

            }

        }

        protected void LowerEvaluate()
        {

            if (typeof(T1) == typeof(int))
            {
                int a = 0;
                Left.Evaluate(out a);
                if (typeof(T2) == typeof(int))
                {
                    int b = 0;
                    Right.Evaluate(out b);
                    this.Content = (a < b);
                }
                else if (typeof(T2) == typeof(double))
                {
                    double b = 0;
                    Right.Evaluate(out b);
                    this.Content = a < b;
                }

            }

        }

        protected void LowerEqualEvaluate()
        {
            if (typeof(T1) == typeof(int))
            {
                int a = 0;
                Left.Evaluate(out a);
                if (typeof(T2) == typeof(int))
                {
                    int b = 0;
                    Right.Evaluate(out b);
                    this.Content = (a <= b);
                }
                else if (typeof(T2) == typeof(double))
                {
                    double b = 0;
                    Right.Evaluate(out b);
                    this.Content = a <= b;
                }

            }
        }

        public override void Clean()
        {
            base.Clean();
            Left.Clean();
            Left = null;
            Right.Clean();
            Right = null;
        }
    }
}
