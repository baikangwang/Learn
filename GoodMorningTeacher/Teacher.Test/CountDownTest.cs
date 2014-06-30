using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Teacher.Test
{
    using System.Threading;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CountDownTest
    {
        [TestMethod]
        public void Test()
        {
            int interval = 1000 * 60 * 11;
            for (int i = 0; i < interval; i++)
            {
                TimeSpan t = new TimeSpan(i);
                Console.CursorLeft = 22;
                Console.Write(t.ToString("hh\\:mm\\:ss\\"));
                Thread.Sleep(1000);
            }
        }
    }
}
