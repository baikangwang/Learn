using CG.TrayNotify.Common.Contract;

namespace CG.TrayNotify.Wcf.Service
{
    internal class NotifyThreadStateInfo
    {
        public static NotifyThreadStateInfo Create(Client client, FileEventArgs e)
        {
            return new NotifyThreadStateInfo() {Client = client, Args = e};
        }

        public Client Client { get; private set; }

        public FileEventArgs Args { get; private set; }
    }
}