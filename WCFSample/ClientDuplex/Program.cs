using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace WCFSample.ClientDuplex
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

                Console.WriteLine("Done!");

                Console.Read();
            }
            catch (TimeoutException timeProblem)
            {
                Console.WriteLine("The service operation timed out. " + timeProblem.Message);
                client.Abort();
                Console.Read();
            }
            catch (CommunicationException commProblem)
            {
                Console.WriteLine("There was a communication problem. " + commProblem.Message);
                client.Abort();
                Console.Read();
            }


        }
    }
}
