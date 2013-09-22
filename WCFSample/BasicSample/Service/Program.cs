using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using WCFSample.Contract;

namespace WCFSample.Service
{
    using System.ServiceModel.Description;

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
