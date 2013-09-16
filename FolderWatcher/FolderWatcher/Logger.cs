using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FolderWatcher
{
    public class Logger
    {
        public static void Log(string message)
        {
            using (TextWriter w = new StreamWriter("E://temp//log//FolderLog.txt", true))
            {
                w.WriteLine("{0} {1}",DateTime.Now,message);
            }
        }
    }
}
