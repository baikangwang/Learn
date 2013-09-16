using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace FolderWatcher
{
    public partial class FolderWatch : ServiceBase
    {
        public FolderWatch()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
        }

        protected override void OnStop()
        {
        }

        private void fileSystemWatcher1_Created(object sender, System.IO.FileSystemEventArgs e)
        {
            Logger.Log(string.Format("A new folder/file with name {0} has been created.",e.Name));
        }

        private void fileSystemWatcher1_Deleted(object sender, System.IO.FileSystemEventArgs e)
        {
            Logger.Log(string.Format("A new folder/file with name {0} has been deleted.", e.Name));
        }

        private void fileSystemWatcher1_Renamed(object sender, System.IO.RenamedEventArgs e)
        {
            Logger.Log(string.Format("A new folder/file with name {0} has been renamed.", e.Name));
        }
    }
}
