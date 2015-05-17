using System;

namespace MindHelper.Math
{
    public class Solve
    {
        #region OPERATION

        public static double Multiply(double Value1, double Value2)
        {
            return Value1 * Value2;
        }

        public static double Divide(double Value1, double Value2)
        {
            return Value1 / Value2;
        }

        public static double Add(double Value1, double Value2)
        {
            return Value1 + Value2;
        }

        public static double Subtract(double Value1, double Value2)
        {
            return Value1 - Value2;
        }

        #endregion

        #region FUNCTION

        public static double SquareRoot(double Value)
        {
            return System.Math.Sqrt(Value);
        }

        public static double ToThePowerOf(double Value, double Power)
        {
            return System.Math.Pow(Value, Power);
        }

        public static double Sine(double Value)
        {
            return System.Math.Sin(Value);
        }

        public static double Cosine(double Value)
        {
            return System.Math.Cos(Value);
        }

        public static double Tangent(double Value)
        {
            return System.Math.Tan(Value);
        }

        public static double Secant(double Value)
        {
            return Divide(1, System.Math.Sin(Value));
        }

        public static double Cosecant(double Value)
        {
            return Divide(1, System.Math.Cos(Value));
        }

        public static double Cotangent(double Value)
        {
            return Divide(1, System.Math.Tan(Value));
        }

        public static double Log(double Value, double N = 10)
        {
            return System.Math.Log(Value, N);
        }

        public static double Pie(double Value)
        {
            return Multiply(Value, System.Math.PI);
        }

        public static double Factorial(double Value)
        {
            if (Value < 2)
            {
                return 1;
            }
            else
            {
                return Multiply(Value, Factorial(Subtract(Value ,1)));
            }
        }

        #endregion        
    }
}
