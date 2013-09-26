using WCFSample.Contract;

namespace WCFSample.Console.Basic.Service.Contract
{
    public class CalculatorService:ICalculator
    {
        public double Add(double d1, double d2)
        {
            double result = d1 + d2;
            System.Console.WriteLine("Received Add({0},{1})", d1, d2);
            // Code added to write output to the console window.
            System.Console.WriteLine("Return: {0}", result);
            return result;
        }

        public double Subtract(double d1, double d2)
        {
            double result = d1 - d2;
            System.Console.WriteLine("Received Subtract({0},{1})", d1, d2);
            // Code added to write output to the console window.
            System.Console.WriteLine("Return: {0}", result);
            return result;
        }

        public double Multiply(double d1, double d2)
        {
            double result = d1 * d2;
            System.Console.WriteLine("Received Multiply({0},{1})", d1, d2);
            // Code added to write output to the console window.
            System.Console.WriteLine("Return: {0}", result);
            return result;
        }

        public double Divide(double d1, double d2)
        {
            double result = d1 / d2;
            System.Console.WriteLine("Received Divide({0},{1})", d1, d2);
            // Code added to write output to the console window.
            System.Console.WriteLine("Return: {0}", result);
            return result;
        }
    }
}
