using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Teacher.Test
{
    [TestClass]
    public class ViewStateChangerTest
    {
        [TestMethod]
        public void SerializeViewStateTest()
        {
            string input =
                "/wEPDwUKMTAyNDY3NTkwNg8WBB4Fc3RhcnQFEjIwMTQtNi0yNCAyMDowNDo0MR4GQm9va0lEBQIzMhYCAgMPZBYCAg0PDxYCHgRUZXh0BQIxNmRkZKy2mnBvCpiOPP52mVGR0VDpJodm";

            ViewStateChanger changer=new ViewStateChanger();

            string file = changer.SerializeViewState(input);

            Process.Start(file);
        }
    }
}
