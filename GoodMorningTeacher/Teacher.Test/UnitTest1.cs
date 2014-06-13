using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Teacher.Test
{
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string viewstate =
                "%2FwEPDwUKMTAyNDY3NTkwNg8WBB4Fc3RhcnQFEjIwMTQtNi0xMiAxMjowMDowMh4GQm9va0lEBQIyORYCAgMPZBYCAg0PDxYCHgRUZXh0BQEwZGRklySbmxcKwi5TZc0xsYKHw27CuP4%3D";
            string validation = "%2FwEWBQKrudu7CAKz8dy8BQKu%2BOnrDgKE6rWYAwKR9JYa0CqP3iCl6ZiMWVdMl2TQLRa1imc%3D";

            viewstate = HttpUtility.UrlDecode(viewstate);
            validation = HttpUtility.UrlDecode(validation);
            byte[] cviewstate = Convert.FromBase64String(viewstate);
            byte[] cvalidation = Convert.FromBase64String(validation);
            using (FileStream fs=new FileStream("viewstate.txt",FileMode.Create))
            {
                fs.Write(cviewstate,0,cviewstate.Length);
                fs.Flush();
            }

            using (FileStream fs=new FileStream("validation.txt",FileMode.Create))
            {
                fs.Write(cvalidation,0,cvalidation.Length);
                fs.Flush();
            }
        }

        [TestMethod]
        public void TestMethod2()
        {
            string viewstate = string.Empty;
            string validation = string.Empty;

            string url = string.Format("http://nankaiqu.fxlxm.teacher.com.cn/GuoPeiAdmin/CourseStudy/RecordTimeNew.aspx?bookid={0}&StudyPlanBookID={1}", 35, 22);
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
            request.Method = WebRequestMethods.Http.Get;
            request.Referer =
                string.Format("http://nankaiqu.fxlxm.teacher.com.cn/GuoPeiAdmin/CourseStudy/StudyfrmNew.aspx?StudyPlanBookID={0}", 22);
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Headers.Add("Accept-Language", "en-US");
            request.Headers.Add("Accept-Encoding", "gzip,deflate");
            request.KeepAlive = true;
            request.Headers.Add("Cache-Control", "no-cache");

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            long total = response.ContentLength;
            int current = 0;
            byte[] content = new byte[1024];
            StringBuilder sb = new StringBuilder();
            using (Stream rs = response.GetResponseStream())
            {
                if (rs != null)
                {
                    while (total > current)
                    {
                        current += rs.Read(content, 0, content.Length);
                        sb.Append(Encoding.UTF8.GetString(content, 0, content.Length));
                    }

                    rs.Flush();
                }
                else
                {
                    return;
                }
            }

            string form = sb.ToString();

            Match m = Regex.Match(form,
                        "<input type=\"hidden\" name=\"__VIEWSTATE\" id=\"__VIEWSTATE\" value=\"(?<viewstate>.*?)\" />");
            if (m.Success)
            {
                viewstate = m.Groups["viewstate"].Value;
            }

            m = Regex.Match(form,
                        "<input type=\"hidden\" name=\"__EVENTVALIDATION\" id=\"__EVENTVALIDATION\" value=\"(?<validation>.*?)\" />");
            if (m.Success)
            {
                validation = m.Groups["validation"].Value;
            }

            byte[] cviewstate = Convert.FromBase64String(viewstate);
            byte[] cvalidation = Convert.FromBase64String(validation);
            string dviewstate = Encoding.UTF8.GetString(cviewstate);
            string dvalidaion = Encoding.UTF8.GetString(cvalidation);
            using (FileStream fs = new FileStream("viewstate.txt", FileMode.Create))
            {
                fs.Write(cviewstate, 0, cviewstate.Length);
                fs.Flush();
            }

            using (FileStream fs = new FileStream("validation.txt", FileMode.Create))
            {
                fs.Write(cvalidation, 0, cvalidation.Length);
                fs.Flush();
            }
        }
    }
}
