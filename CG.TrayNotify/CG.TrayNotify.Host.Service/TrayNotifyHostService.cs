using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using CG.TrayNotify.Common;

namespace CG.TrayNotify.Host.Service
{
    partial class TrayNotifyHostService : ServiceBase
    {
        private WcfServiceHost _wcfServiceHost;
        
        public TrayNotifyHostService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            OnStop();

            if (_wcfServiceHost == null)
            {
                _wcfServiceHost = WcfServiceHost.Create(EventLog);
            }

            _wcfServiceHost.Start();
        }

        protected override void OnShutdown()
        {
            OnStop();

            _wcfServiceHost = null;
        }

        protected override void OnStop()
        {
           if(_wcfServiceHost==null)
               return;

            _wcfServiceHost.Stop();
        }
    }
}
