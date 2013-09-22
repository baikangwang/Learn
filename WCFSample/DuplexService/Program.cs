using System;
using System.ServiceModel;
using WCFSample.Contract;

namespace WCFSample.DuplexService
{
    using System.ServiceModel.Description;

    class Program
    {
        static void Main(string[] args)
        {
            Uri baseAddress = new Uri("http://localhost:8000/WCFSample/ServiceDuplex");
            ServiceHost selfHost = new ServiceHost(typeof(CalculatorDuplexService), baseAddress);
            try
            {
                selfHost.AddServiceEndpoint(typeof(ICalculatorDuplex), new WSDualHttpBinding() , "CalculatorDuplexService");
                ServiceMetadataBehavior smb=new ServiceMetadataBehavior {HttpGetEnabled = true};
                selfHost.Description.Behaviors.Add(smb);
                selfHost.Open();
                Console.WriteLine("The service is ready.");
                Console.WriteLine("Press <ENTER> to terminate service.");
                Console.WriteLine();
                Console.ReadLine();

                // Close the ServiceHostBase to shutdown the service.
                selfHost.Close();
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine("An exception occurred: {0}", ex.Message);
                selfHost.Abort();
            }
        }
    }
}
