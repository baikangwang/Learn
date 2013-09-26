using System;
using System.ServiceModel;

namespace WCFSample.Console.Duplex.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            InstanceContext site=new InstanceContext(new CallbackHandler());

            CalculatorDuplexClient client=new CalculatorDuplexClient(site);

            try
            {
                double value = 100.00D;
                client.AddTo(value);

                value = 50.00D;
                client.SubtractFrom(value);

                value = 17.65D;
                client.MuliplyBy(value);

                value = 2.00D;
                client.DivideBy(value);

                client.Clear();

                System.Threading.Thread.Sleep(5000);

                client.Close();

                System.Console.WriteLine("Done!");

                System.Console.Read();
            }
            catch (TimeoutException timeProblem)
            {
                System.Console.WriteLine("The service operation timed out. " + timeProblem.Message);
                client.Abort();
                System.Console.Read();
            }
            catch (CommunicationException commProblem)
            {
                System.Console.WriteLine("There was a communication problem. " + commProblem.Message);
                client.Abort();
                System.Console.Read();
            }


        }
    }
}
