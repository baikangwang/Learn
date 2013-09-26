using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CG.TrayNotify.Common.Contract;

namespace Orange.TrayNotify.Console.Client
{
    public class TrayNotifyCallback:ITrayNotifyCallback
    {
        public void OnFileChangeEvent(FileEventArgs e)
        {
            const string path = "E://temp//output.txt";

            FileStream fs = !File.Exists(path)
                                ? new FileStream(path, FileMode.CreateNew, FileAccess.ReadWrite)
                                : new FileStream(path, FileMode.Append, FileAccess.ReadWrite);

            StringBuilder sb=new StringBuilder();
            sb.AppendLine(""+DateTime.Now);
            sb.AppendLine("\t" + e.Id);
            sb.AppendLine("\t" + e.ChangeType);
            sb.AppendLine("\t" + e.Folder);
            sb.AppendLine("\t" + e.FileName);
            sb.AppendLine("\t" + e.Date);

            byte[] content = Encoding.ASCII.GetBytes(sb.ToString());
 
            fs.Write(content,0,content.Length);

            fs.Flush(true);

            fs.Dispose();
        }
    }
}
