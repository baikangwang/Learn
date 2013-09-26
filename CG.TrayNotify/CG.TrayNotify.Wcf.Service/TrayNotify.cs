using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;
using CG.TrayNotify.Common.Contract;
using CG.TrayNotify.Common.Interface;
using CG.TrayNotify.Common.Threading;

namespace CG.TrayNotify.Wcf.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "TrayNotify" in code, svc and config file together.
    [ServiceBehavior(ConfigurationName = "CG.TrayNotify.Wcf.Service:CG.TrayNotify.Wcf.Service.TrayNotifyEndpoint", InstanceContextMode = InstanceContextMode.Single)]
    public class TrayNotifyEndpoint : ITrayNotify
    {
        private Monitor _monitor = Monitor.Create();

        private DictionarySync<ReaderWriterAutoLock, Guid, Client> _clients = new DictionarySync<ReaderWriterAutoLock, Guid, Client>();

        public TrayNotifyEndpoint()
        {
            _monitor.FileEvent += new FileEventHandler(OnFileEvent);
        }

        private void OnFileEvent(object sender, FileEventArgs e)
        {
            RemoveInvalidClients();
            using (AutoLock.LockToRead(_clients.Lock, 5000))
            {
                foreach (Client client in _clients.Values)
                {
                    ThreadPool.QueueUserWorkItem(NotifyThreadProc, NotifyThreadStateInfo.Create(client, e));
                }
            }
        }

        private void NotifyThreadProc(object state)
        {
            NotifyThreadStateInfo stateInfo = state as NotifyThreadStateInfo;

            if (stateInfo == null) return;

            try
            {
                stateInfo.Client.Callback.OnFileChangeEvent(stateInfo.Args);
            }
            catch (TimeoutException)
            {

                stateInfo.Client.Invalidate();
            }
        }

        private void RemoveInvalidClients()
        {
            List<Guid> removeClientList = new List<Guid>();

            using (AutoLock.LockToRead(_clients.Lock, 5000))
            {
                foreach (Client client in _clients.Values)
                {
                    if (!client.IsValid)
                    {
                        removeClientList.Add(client.Id);
                    }
                }
            }

            foreach (Guid id in removeClientList)
            {
                if (_clients.ContainsKey(id))
                {
                    _clients.Remove(id);
                }
            }
        }

        public void Register(Guid instanceId)
        {
            ITrayNotifyCallback caller = OperationContext.Current.GetCallbackChannel<ITrayNotifyCallback>();
            if (caller != null)
            {
                if (!_clients.ContainsKey(instanceId))
                {
                    _clients.Add(instanceId, Client.Create(instanceId, caller));
                }
            }
        }

        public void UnRegister(Guid instanceId)
        {
            if (_clients.ContainsKey(instanceId))
            {
                _clients.Remove(instanceId);
            }
        }

        public void Start(Guid instanceId, string folderToMonitor)
        {
            _monitor.Add(folderToMonitor);
            _monitor.Start();
        }

        public void Stop(Guid instanceId, string folderToMonitor)
        {
            _monitor.Stop();
            _monitor.Remove(folderToMonitor);
        }
    }
}
