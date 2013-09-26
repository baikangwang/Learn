

using System.IO;
using System.Runtime.Serialization;

namespace CG.TrayNotify.Common.Contract
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    #endregion

    public delegate void FileEventHandler(object sender, FileEventArgs e);

    [Serializable]
    [DataContract]
    public class FileEventArgs:EventArgs
    {
        public static FileEventArgs Create(FileSystemEventArgs args, string folder)
        {
            return new FileEventArgs()
                {
                    Folder = folder,
                    Date = DateTime.Now,
                    FileName = args.Name,
                    ChangeType = args.ChangeType,
                    Id = Guid.NewGuid()
                };
        }

        public static FileEventArgs Create(RenamedEventArgs args, string folder)
        {
            return new FileEventArgs()
                {
                    Folder = folder,
                    Date = DateTime.Now,
                    FileName = args.Name,
                    ChangeType = args.ChangeType,
                    Id = Guid.NewGuid()
                };
        }

        [DataMember]
        public WatcherChangeTypes ChangeType { get; private set; }
        [DataMember]
        public DateTime Date { get; private set; }
        [DataMember]
        public string FileName { get; private set; }
        [DataMember]
        public string Folder { get; private set; }
        [DataMember]
        public Guid Id { get; private set; }
    }
}
