namespace WCFSample.Console.Duplex.Client
{
    public class CallbackHandler:ICalculatorDuplexCallback
    {
        public void Equals(double result)
        {
            System.Console.WriteLine("Result ({0})",result);
        }

        public void Equation(string eqn)
        {
            System.Console.WriteLine("Equation ({0})", eqn);
        }
    }
}
