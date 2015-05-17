using System;

namespace MindHelper
{
    class Program
    {
        static Math.Expression _Expression = new Math.Expression();

        static void Main(string[] args)
        {
            #region test Expression
            //WriteExpressionAndAnswer("√2^2", "2");
            //WriteExpressionAndAnswer("0!", "1");
            //WriteExpressionAndAnswer("1!", "1");
            //WriteExpressionAndAnswer("4!", "24");
            //WriteExpressionAndAnswer("4L", "");
            //WriteExpressionAndAnswer("2π", "6.28318530717959");
            //WriteExpressionAndAnswer("S2+C2+T2+s2+c2+t2+4!+2+2π+4L12", "");
            //WriteExpressionAndAnswer("2^4", "16");
            //WriteExpressionAndAnswer("2*-4", "-8");
            //WriteExpressionAndAnswer("-1+-1", "-2");
            //WriteExpressionAndAnswer("-1+1", "0");
            //WriteExpressionAndAnswer("1+-1", "0");
            //WriteExpressionAndAnswer("-1--1", "0");
            //WriteExpressionAndAnswer("-12÷2", "-6");
            //WriteExpressionAndAnswer("(-12÷2+(1+1))+10", "6");
            //WriteExpressionAndAnswer("(1+1)*(1-1)", "0");
            //WriteExpressionAndAnswer("(1+1)*(1-1+(2*2))", "-8");
            //WriteExpressionAndAnswer("√((1+1)*(1-1+(2*2))*-1)", "2.82842712474619");
            //Console.ReadLine();
            #endregion

            #region test GCF

            Console.WriteLine(Math.GCF.Get(new int[] {  20, 50 , 120 }));

            Console.ReadLine();

            #endregion

            #region test LCM

            Console.WriteLine(Math.LCM.Get(new int[] { 330, 65, 15 }));

            Console.ReadLine();

            #endregion
        }

        private static void WriteExpressionAndAnswer(string Expression, string ExpectedAnswer)
        {
            _Expression.LoadExpression(Expression);
            string Answer = _Expression.Solve();
            if (Answer == ExpectedAnswer)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            Console.WriteLine(Expression + "  =   " + Answer);
        }
    }
}
