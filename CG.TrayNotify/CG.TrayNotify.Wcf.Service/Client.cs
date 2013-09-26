using System;
using CG.TrayNotify.Common.Interface;

namespace CG.TrayNotify.Wcf.Service
{
    internal class Client
    {
        public bool IsValid { get; private set; }

        public Guid Id { get; private set; }

        public ITrayNotifyCallback Callback { get; private set; }

        public void Invalidate()
        {
            IsValid = false;
        }

        public static Client Create(Guid instanceId, ITrayNotifyCallback caller)
        {
            return new Client() {Id = instanceId, Callback = caller, IsValid = true};
        }
    }
}