

namespace CG.TrayNotify.Wcf.Service
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.IO;
    using CG.TrayNotify.Common.Contract;
    
    internal class Folder:IDisposable
    {
        public static Folder Create(Monitor monitor, string folderToMonitor)
        {
            return new Folder(monitor,folderToMonitor);
        }

        public void Start()
        {
            _watcher.Changed+=new FileSystemEventHandler(OnFileEvent);
            _watcher.Created+=new FileSystemEventHandler(OnFileEvent);
            _watcher.Deleted+=new FileSystemEventHandler(OnFileEvent);
            _watcher.Renamed+=new RenamedEventHandler(OnRenameEvent);

            _watcher.EnableRaisingEvents = true;
        }

        private void OnRenameEvent(object sender, RenamedEventArgs e)
        {
            _monitor.AddQueueItem(FileEventArgs.Create(e,_folder));
        }

        private void OnFileEvent(object sender, FileSystemEventArgs e)
        {
            _monitor.AddQueueItem(FileEventArgs.Create(e,_folder));
        }

        public void Stop()
        {
            _watcher.EnableRaisingEvents = false;

            _watcher.Changed -= new FileSystemEventHandler(OnFileEvent);
            _watcher.Created -= new FileSystemEventHandler(OnFileEvent);
            _watcher.Deleted -= new FileSystemEventHandler(OnFileEvent);
            _watcher.Renamed -= new RenamedEventHandler(OnRenameEvent);
        }

        private Folder(Monitor monitor, string folderToMonitor)
        {
            _monitor = monitor;
            _folder = folderToMonitor;
            _watcher.Path = folderToMonitor;
            _watcher.NotifyFilter = NotifyFilters.FileName |
                                    NotifyFilters.Attributes |
                                    NotifyFilters.LastAccess |
                                    NotifyFilters.LastWrite |
                                    NotifyFilters.Size;
        }

        private string _folder = string.Empty;
        private Monitor _monitor;
        private FileSystemWatcher _watcher=new FileSystemWatcher();
        
        protected virtual void Dispose(bool all)
        {
            if (all)
            {
                _watcher.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}