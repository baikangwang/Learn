using System;
using System.ServiceModel;
using CG.TrayNotify.Common.Contract;

namespace CG.TrayNotify.Common.Interface
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ITrayNotify" in both code and config file together.
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(ITrayNotifyCallback))]
    public interface ITrayNotify
    {
        [OperationContract]
        void Register(Guid instanceId);

        [OperationContract]
        void UnRegister(Guid instanceId);

        [OperationContract]
        void Start(Guid instanceId, string folderToMonitor);

        [OperationContract]
        void Stop(Guid instanceId, string folderToMonitor);
    }

    [ServiceContract]
    public interface ITrayNotifyCallback
    {
        [OperationContract]
        void OnFileChangeEvent(FileEventArgs e);
    }
}
