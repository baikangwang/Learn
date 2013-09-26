using System;
using System.Threading;
using CG.TrayNotify.Common.Contract;
using CG.TrayNotify.Common.Threading;

namespace CG.TrayNotify.Wcf.Service
{
    internal class Monitor:IDisposable
    {
        public static Monitor Create()
        {
            return new Monitor();
        }

        public event FileEventHandler FileEvent
        {
            remove { _fileEvent -= value; }
            add { _fileEvent += value; }
        }

        public void Add(string folderMonitor)
        {
            if (!_folderDictionary.ContainsKey(folderMonitor))
            {
                _folderDictionary.Add(folderMonitor, Folder.Create(this, folderMonitor));
            }
        }

        public void Remove(string folderToMonitor)
        {
            if (_folderDictionary.ContainsKey(folderToMonitor))
            {
                _folderDictionary.Remove(folderToMonitor);
            }
        }

        public void AddQueueItem(FileEventArgs e)
        {
            _fileEventQueue.Enqueue(e);
            _queueEvent.Set();
        }

        public void Start()
        {
            if (_waitHandles == null)
            {
                _waitHandles=new WaitHandle[]{_stopEvent,_queueEvent};
            }

            if (_queueServiceThread == null)
            {
                _queueServiceThread=new Thread(new ThreadStart(QueueServiceThreadProc))
                    {
                        Priority = ThreadPriority.AboveNormal,
                        IsBackground = true
                    };

                _queueServiceThread.Start();
            }

            foreach (Folder folder in _folderDictionary.Values)
            {
                folder.Start();
            }
        }

        public void Stop()
        {
            if (_queueServiceThread != null)
            {
                _stopEvent.Set();

                _queueServiceThread = null;

                foreach (Folder folder in _folderDictionary.Values)
                {
                    folder.Stop();
                }
            }
        }

        private void QueueServiceThreadProc()
        {
            try
            {
                while (true)
                {
                    switch (WaitHandle.WaitAny(_waitHandles,Timeout.Infinite,false))
                    {
                        case StopEvent:
                            return;
                        case QueueEvent:
                            FileEventArgs args = null;
                            if (null != (args = _fileEventQueue.GetNextItem(true)))
                            {
                                FireFileEvent(args);
                                _fileEventQueue.Dequeue();
                            }
                            break;
                    }
                }
            }
            catch (Exception)
            {
                // todo: reprot error;
            }
        }

        private void FireFileEvent(FileEventArgs args)
        {
            if (_fileEvent != null)
            {
                _fileEvent(this, args);
            }
        }

        private Monitor(){}

        private DictionarySync<ReaderWriterAutoLock,string,Folder> _folderDictionary=new DictionarySync<ReaderWriterAutoLock, string, Folder>();

        private QueueSync<CriticalSectionAutoLock,FileEventArgs> _fileEventQueue=new QueueSync<CriticalSectionAutoLock, FileEventArgs>();

        private event FileEventHandler _fileEvent;

        private Thread _queueServiceThread = null;

        private WaitHandle[] _waitHandles = null;

        private AutoResetEvent _stopEvent=new AutoResetEvent(false);

        private AutoResetEvent _queueEvent=new AutoResetEvent(false);

        private const int StopEvent = 0;

        private const int QueueEvent = 1;

        protected virtual void Dispose(bool all)
        {
            if (all)
            {
                _stopEvent.Dispose();
                _queueEvent.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}