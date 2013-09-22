using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using WCFSample.Contract;

namespace WCFSample.Service
{
    public class CalculatorService:ICalculator
    {
        public double Add(double d1, double d2)
        {
            double result = d1 + d2;
            Console.WriteLine("Received Add({0},{1})", d1, d2);
            // Code added to write output to the console window.
            Console.WriteLine("Return: {0}", result);
            return result;
        }

        public double Subtract(double d1, double d2)
        {
            double result = d1 - d2;
            Console.WriteLine("Received Subtract({0},{1})", d1, d2);
            // Code added to write output to the console window.
            Console.WriteLine("Return: {0}", result);
            return result;
        }

        public double Multiply(double d1, double d2)
        {
            double result = d1 * d2;
            Console.WriteLine("Received Multiply({0},{1})", d1, d2);
            // Code added to write output to the console window.
            Console.WriteLine("Return: {0}", result);
            return result;
        }

        public double Divide(double d1, double d2)
        {
            double result = d1 / d2;
            Console.WriteLine("Received Divide({0},{1})", d1, d2);
            // Code added to write output to the console window.
            Console.WriteLine("Return: {0}", result);
            return result;
        }
    }
}
