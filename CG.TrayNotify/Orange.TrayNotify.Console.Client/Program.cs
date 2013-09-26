using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Orange.TrayNotify.Console.Client
{
    public class Program
    {
        static void Main(string[] args)
        {
            InstanceContext site = new InstanceContext(new TrayNotifyCallback());

            TrayNotifyClient client = new TrayNotifyClient(site);

            Guid id = Guid.NewGuid();
            string path = "e:\\temp\\watcher";
            
            try
            {
                
                client.Register(id);

                client.Start(id, path);

                System.Console.WriteLine("Started!");

                System.Console.Read();
            }
            catch (TimeoutException timeProblem)
            {
                System.Console.WriteLine("The service operation timed out. " + timeProblem.Message);
                client.Stop(id,path);
                client.UnRegister(id);
                client.Abort();
                System.Console.Read();
            }
            catch (CommunicationException commProblem)
            {
                System.Console.WriteLine("There was a communication problem. " + commProblem.Message);
                client.Stop(id, path);
                client.UnRegister(id);
                client.Abort();
                System.Console.Read();
            }

        }
    }
}
