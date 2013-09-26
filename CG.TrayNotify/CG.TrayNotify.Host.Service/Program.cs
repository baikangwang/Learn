using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Text;

namespace CG.TrayNotify.Host.Service
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            TrayNotifyHostService service=new TrayNotifyHostService();
            if (IsDebugMode(args))
            {
                CG.TrayNotify.Common.WcfServiceHost.RunServiceAsConsoleApp("Tray Notify Host", service.EventLog);
                return;
            }
            try
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                    {
                        service
                    };
                ServiceBase.Run(ServicesToRun);
            }
            catch (Exception e)
            {
                service.EventLog.WriteEntry(e.Message,System.Diagnostics.EventLogEntryType.Error);
            }
        }

        private static bool IsDebugMode(string[] args)
        {
            if (args == null || args.Length == 0) return false;
            if (args[0].ToLower() == "/debug") return true;

            return false;
        }
    }
}
