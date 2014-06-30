using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CountDown
{
    using System.Threading;

    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("Line "+i);
            }
            Console.WriteLine(DateTime.Now.ToString("hh:mm:ss"));
            int interval = 10;
            long unit = 10000000;
            for (int i = 1; i <= interval; i++)
            {
                TimeSpan t = new TimeSpan((long)i*unit);
                Thread.Sleep(1000);
                Console.CursorLeft = 0;
                Console.Write(t.ToString("hh\\:mm\\:ss"));
            }
            Console.WriteLine();
            Console.WriteLine(DateTime.Now.ToString("hh:mm:ss"));
            Console.Read();
        }
    }
}
