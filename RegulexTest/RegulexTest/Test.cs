using System;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RegulexTest
{
    [TestClass]
    public class Test
    {
        [TestMethod]
        public void XmlTest()
        {
            string xml = @"<root><Name> ddd df*&&<>$%^%)()(dfd </Name></root>";
            //string pattern = @"<Name[^>]*>(?<value>.*?)\k<value></Name>";
            string pattern = @"<Name[^>]*>(?<value>.*)</Name>";
            MatchCollection matches = Regex.Matches(xml, pattern);
            
            Console.WriteLine("Match Count: {0}",matches.Count);
            
            
            
            foreach (Match match in matches)
            {
                //Console.WriteLine("Match Count: {0}", matches.);

                foreach (Group g in match.Groups)
                {
                    Console.WriteLine(string.Format("Group {0}:{1}, Succesful:{2}",g.Index,g.Value,g.Success));
                }
            }
            Group group = matches[0].Groups["value"];
            Console.WriteLine(string.Format("Group {0}:{1}, Succesful:{2}", group.Index, group.Value, group.Success));
        }
    }
}
