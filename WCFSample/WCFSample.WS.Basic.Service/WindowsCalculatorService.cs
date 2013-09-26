using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;

namespace WCFSample.WS.Basic.Service
{
    public partial class WindowsCalculatorService : ServiceBase
    {
        public ServiceHost ServiceHost = null;
        
        public WindowsCalculatorService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            if(ServiceHost!=null)
                ServiceHost.Close();

            ServiceHost=new ServiceHost(typeof(CalculatorService));

            ServiceHost.Open();
        }

        protected override void OnStop()
        {
            if (ServiceHost != null)
            {
                ServiceHost.Close();
                ServiceHost = null;
            }
        }
    }
}
