using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Teacher.Test
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.UI;

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string viewstate =
                "%2FwEPDwUKMTAyNDY3NTkwNg8WBB4Fc3RhcnQFEjIwMTQtNi0yNCAxNjo0NDo1Nx4GQm9va0lEBQIzMhYCAgMPZBYCAg0PDxYCHgRUZXh0BQEwZGRkFChLVzncorbAjzt7iLjU3vVncUE%3D";
                //"%2FwEPDwUKMTAyNDY3NTkwNg8WBB4Fc3RhcnQFEjIwMTQtNi0xMiAxMjowMDowMh4GQm9va0lEBQIyORYCAgMPZBYCAg0PDxYCHgRUZXh0BQEwZGRklySbmxcKwi5TZc0xsYKHw27CuP4%3D";
            string validation = "%2FwEWBQKFuv4aArPx3LwFAq746esOAoTqtZgDApH0lhr8XZzAtDXnBFKq%2Fwh%2BCLwqR8ftKg%3D%3D"; //"%2FwEWBQKrudu7CAKz8dy8BQKu%2BOnrDgKE6rWYAwKR9JYa0CqP3iCl6ZiMWVdMl2TQLRa1imc%3D";

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

            int bookId = 32;
            int studyPlanBookId = 69;
            int learned = 143;

            string url = string.Format("http://nankaiqu.fxlxm.teacher.com.cn/GuoPeiAdmin/CourseStudy/RecordTimeNew.aspx?bookid={0}&StudyPlanBookID={1}", bookId, studyPlanBookId);
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
            request.Method = WebRequestMethods.Http.Get;
            request.Referer =
                string.Format("http://nankaiqu.fxlxm.teacher.com.cn/GuoPeiAdmin/CourseStudy/StudyfrmNew.aspx?StudyPlanBookID={0}", bookId);
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
                        "<input type=\"hidden\" name=\"__EVENTVALIDATION\" id=\"__EVENTVALIDATION\" value=\"(?<validation>.*?)\"\\s*/>");
            if (m.Success)
            {
                validation = m.Groups["validation"].Value;
            }

            string suffix;



            ObjectStateFormatter parser = new ObjectStateFormatter();
            Pair dviewState = (Pair)parser.Deserialize(viewstate);

            Pair one = (Pair)(dviewState.First ?? dviewState.Second);
            Pair states = (Pair)one.Second;
            ArrayList users = (ArrayList)states.First;

            // Change start date time
            DateTime started = DateTime.Parse(users[1].ToString());
            users[1] = started.AddMinutes(-20).ToString("yyyy-MM-dd hh:mm:ss");

            bool exist = users.Cast<object>().Any(o => o is IndexedString && (o as IndexedString).Value == "BookID");
            if (!exist)
            {
                users.Add(new IndexedString("BookID"));
                users.Add(bookId.ToString(CultureInfo.InvariantCulture));
            }

            if (states.Second == null)
            {
                ArrayList controls = new ArrayList(){3,new Pair()
                {
                    First = null,
                    Second = new ArrayList() { 13, new Pair()
                                                       {
                                                           Second = null, 
                                                           First = new Pair()
                                                                       {
                                                                           Second = null, 
                                                                           First = new ArrayList() { new IndexedString("Text"), learned.ToString(CultureInfo.InvariantCulture) }
                                                                       }
                                                       } }
                } };

                states.Second = controls;
            }

            viewstate = parser.Serialize(dviewState);
            int needed = 7;
            string name = "教师心理健康与压力管理";
            int minites = 20;
            int txtUserId = 69973;
            string eventTarget = "timeupdate";
            string eventArg = string.Empty;

            int passedtime = minites * 60; //17*60;

            request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));

            request.Method = WebRequestMethods.Http.Post;

            request.Referer =
                string.Format("http://nankaiqu.fxlxm.teacher.com.cn/GuoPeiAdmin/CourseStudy/RecordTimeNew.aspx?bookid={0}&StudyPlanBookID={1}", bookId, studyPlanBookId);
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Headers.Add("Accept-Language", "en-US");
            request.Headers.Add("Accept-Encoding", "gzip,deflate");
            request.KeepAlive = true;
            request.Headers.Add("Cache-Control", "no-cache");

            form =
                string.Format(
                    "__EVENTTARGET={0}&__EVENTARGUMENT={1}&__VIEWSTATE={2}&txtUserId={3}&passedtime={4}&__EVENTVALIDATION={5}",
                    eventTarget, eventArg, viewstate, txtUserId, passedtime, validation);
            content = Encoding.Default.GetBytes(form);
            request.ContentLength = content.Length;
            using (Stream s = request.GetRequestStream())
            {
                s.Write(content, 0, content.Length);
                s.Flush();
            }

            response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                //long total = response.ContentLength;
                current = 1;
                content = new byte[1024];
                sb = new StringBuilder();
                using (Stream rs = response.GetResponseStream())
                {
                    if (rs != null)
                    {
                        while (current > 0)
                        {
                            current = rs.Read(content, 0, content.Length);
                            sb.Append(Encoding.UTF8.GetString(content, 0, content.Length));
                        }
                        rs.Flush();
                    }
                    else
                    {
                        Console.WriteLine("{1}: {0} -> Failed, Cannot get response stream.", name,
                                          DateTime.Now.ToShortTimeString());
                        return;
                    }
                }

                form = sb.ToString();

                string pattern1 = "(alert\\(\\'你最后一次更新时间是.*?目前系统时间为.*?时间还不够20分钟，所以这次更新时间操作无效。\\'\\));|(alert\\(\\'计时出错，重新计时！\\'\\);)";
                Regex r = new Regex(pattern1, RegexOptions.Compiled);
                m = r.Match(form);
                if (!m.Success)
                {
                    Console.WriteLine("{3}: {0} -> Updated {1}, Remaining {2}", name, minites, needed, DateTime.Now.ToShortTimeString());
                }
                else
                {
                    Console.WriteLine("{1}: {0} -> Failed, {2}", name, DateTime.Now.ToShortTimeString(),"< 20 mins or timer error");
                }
            }
            else
            {
                Console.WriteLine("{1}: {0} -> Failed, {2}", name, DateTime.Now.ToShortTimeString(), response.StatusDescription);
            }
        }
    }
}
