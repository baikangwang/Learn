using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WCFSample.Contract
{
    public abstract class CalculatorBase : ICalculator
    {
        public virtual double Add(double n1, double n2)
        {
            return n1 + n2;
        }

        public virtual double Subtract(double n1, double n2)
        {
            return n1 - n2;
        }

        public virtual double Multiply(double n1, double n2)
        {
            return n1 * n2;
        }

        public virtual double Divide(double n1, double n2)
        {
            return n1 / n2;
        }
    }
}
