using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WCFSample.ClientDuplex
{
    public class CallbackHandler:ICalculatorDuplexCallback
    {
        public void Equals(double result)
        {
            Console.WriteLine("Result ({0})",result);
        }

        public void Equation(string eqn)
        {
            Console.WriteLine("Equation ({0})", eqn);
        }
    }
}
