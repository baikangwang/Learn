using System;
using System.ServiceModel;
using WCFSample.Console.Basic.Service.Contract;
using WCFSample.Contract;
using System.ServiceModel.Description;

namespace WCFSample.Console.Basic.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            Uri baseAddress = new Uri("http://localhost:8000/WCFSample/Service");
            ServiceHost selfHost = new ServiceHost(typeof(CalculatorService), baseAddress);
            try
            {
                selfHost.AddServiceEndpoint(typeof (ICalculator), new WSHttpBinding(), "CalculatorService");
                ServiceMetadataBehavior smb=new ServiceMetadataBehavior {HttpGetEnabled = true};
                selfHost.Description.Behaviors.Add(smb);
                selfHost.Open();
                System.Console.WriteLine("The service is ready.");
                System.Console.WriteLine("Press <ENTER> to terminate service.");
                System.Console.WriteLine();
                System.Console.ReadLine();

                // Close the ServiceHostBase to shutdown the service.
                selfHost.Close();
            }
            catch (CommunicationException ex)
            {
                System.Console.WriteLine("An exception occurred: {0}", ex.Message);
                selfHost.Abort();
            }
        }
    }
}
