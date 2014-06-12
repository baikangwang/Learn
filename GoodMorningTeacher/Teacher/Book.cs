using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Teacher
{
    public class Book
    {
        public string Name
        {
            get { return _name; }
        }

        private int _bookId;
        private int _studyPlanBookId;
        private int _userId;
        private string _name;
        private int _needtime;

        public Book(string name, int bookId, int studyPlanBookId,int needtime,int userId)
        {
            this._name = name;
            this._bookId = bookId;
            this._studyPlanBookId = studyPlanBookId;
            this._userId=userId;
            this._needtime = needtime;
        }

        public void UpdateTime(int minites)
        {
            if(this._needtime<=0)
                return;

            string viewState
                ="%2FwEPDwUKMTAyNDY3NTkwNg8WBB4Fc3RhcnQFEjIwMTQtNi0xMiAxMjowMDowMh4GQm9va0lEBQIyORYCAgMPZBYCAg0PDxYCHgRUZXh0BQEwZGRklySbmxcKwi5TZc0xsYKHw27CuP4%3D";
            string eventValidation= "%2FwEWBQKrudu7CAKz8dy8BQKu%2BOnrDgKE6rWYAwKR9JYa0CqP3iCl6ZiMWVdMl2TQLRa1imc%3D";
            //List<Cookie> cookies;
            //AcquirePostInfo(out viewState, out eventValidation, out cookies);

            //if (string.IsNullOrEmpty(viewState) || string.IsNullOrEmpty(eventValidation))
            //{
            //    Console.WriteLine("{1}: {0} -> Failed, {2}", this.Name, DateTime.Now.ToShortTimeString(), "post info required");
            //    return;
            //}

            //viewState = HttpUtility.UrlEncode(viewState);
            //eventValidation = HttpUtility.UrlEncode(eventValidation);

            string url = string.Format("http://nankaiqu.fxlxm.teacher.com.cn/GuoPeiAdmin/CourseStudy/RecordTimeNew.aspx?bookid={0}&StudyPlanBookID={1}", this._bookId, this._studyPlanBookId);

            int txtUserId = this._userId;
            string eventTarget = "timeupdate";
            string eventArg = string.Empty;
            if (_needtime <= minites)
            {
                minites = _needtime;
            }
            
            int passedtime = minites * 60; //17*60;

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));

            request.Method = WebRequestMethods.Http.Post;

            request.Referer =
                string.Format("http://nankaiqu.fxlxm.teacher.com.cn/GuoPeiAdmin/CourseStudy/RecordTimeNew.aspx?bookid={0}&StudyPlanBookID={1}",this._bookId,this._studyPlanBookId);
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Headers.Add("Accept-Language", "en-US");
            request.Headers.Add("Accept-Encoding", "gzip,deflate");
            request.KeepAlive = true;
            request.Headers.Add("Cache-Control", "no-cache");

            //request.CookieContainer=new CookieContainer();
            //foreach (Cookie cookie in cookies)
            //{
            //    request.CookieContainer.Add(cookie);
            //}

            string form =
                string.Format(
                    "__EVENTTARGET={0}&__EVENTARGUMENT={1}&__VIEWSTATE={2}&txtUserId={3}&passedtime={4}&__EVENTVALIDATION={5}",
                    eventTarget, eventArg, viewState, txtUserId, passedtime, eventValidation);
            byte[] content = Encoding.Default.GetBytes(form);
            request.ContentLength = content.Length;
            using (Stream s = request.GetRequestStream())
            {
                s.Write(content, 0, content.Length);
                s.Flush();
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            
            if (response.StatusCode == HttpStatusCode.OK)
            {
                //long total = response.ContentLength;
                int current = 1;
                content=new byte[1024];
                StringBuilder sb=new StringBuilder();
                using (Stream rs=response.GetResponseStream())
                {
                    if (rs != null)
                    {
                        while (current>0)
                        {
                            current = rs.Read(content, 0, content.Length);
                            sb.Append(Encoding.UTF8.GetString(content, 0, content.Length));
                        }
                        rs.Flush();
                    }
                    else
                    {
                        Console.WriteLine("{1}: {0} -> Failed, Cannot get response stream.", this.Name,
                                          DateTime.Now.ToShortTimeString());
                        return;
                    }
                }

                form = sb.ToString();

                string pattern1 = "(alert\\(\\'你最后一次更新时间是.*?目前系统时间为.*?时间还不够20分钟，所以这次更新时间操作无效。\\'\\));|(alert\\(\\'计时出错，重新计时！\\'\\);)";
                Regex r = new Regex(pattern1, RegexOptions.Compiled);
                Match m = r.Match(form);
                if (!m.Success)
                {
                    if (_needtime <= minites)
                    {
                        _needtime = 0;
                    }
                    else
                    {
                        _needtime -= minites;
                    }

                    if (!IsCompleted)
                    {
                        Console.WriteLine("{3}: {0} -> Updated {1}, Remaining {2}", this.Name, minites, this._needtime, DateTime.Now.ToShortTimeString());
                    }
                    else
                    {
                        Console.WriteLine("{2}: {0} -> Updated {1}, Completed!", this.Name, minites, DateTime.Now.ToShortTimeString());
                    }
                }
                else
                {
                    Console.WriteLine("{1}: {0} -> Failed, {2}", this.Name, DateTime.Now.ToShortTimeString(),
                                      "< 20 mins or timer error");
                }
            }
            else
            {
                Console.WriteLine("{1}: {0} -> Failed, {2}", this.Name, DateTime.Now.ToShortTimeString(),response.StatusDescription);
            }
        }

        public void AcquirePostInfo(out string viewstate,out string validation,out List<Cookie> cookies)
        {
            viewstate = string.Empty;
            validation = string.Empty;
            cookies = new List<Cookie>();

            string url = string.Format("http://nankaiqu.fxlxm.teacher.com.cn/GuoPeiAdmin/CourseStudy/RecordTimeNew.aspx?bookid={0}&StudyPlanBookID={1}", this._bookId, this._studyPlanBookId);
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
            request.Method = WebRequestMethods.Http.Get;
            request.Referer =
                string.Format("http://nankaiqu.fxlxm.teacher.com.cn/GuoPeiAdmin/CourseStudy/StudyfrmNew.aspx?StudyPlanBookID={0}",this._studyPlanBookId);
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Headers.Add("Accept-Language", "en-US");
            request.Headers.Add("Accept-Encoding", "gzip,deflate");
            request.KeepAlive = true;
            request.Headers.Add("Cache-Control", "no-cache");

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            string strcookie = response.Headers["Set-Cookie"];
            if (strcookie.Substring(strcookie.Length - 2, 1) != ";")
            {
                strcookie += ";";
            }
            foreach (Match mc in Regex.Matches(strcookie,"(?<key>[^,]*?)=(?<value>.*?);\\s*(expires=(?<expires>.*?);)?\\s*path=(?<path>.*?);\\s*(?<httponly>HttpOnly)?",RegexOptions.IgnoreCase|RegexOptions.Compiled))
            {
                if (mc.Success)
                {
                    string key = mc.Groups["key"].Value.Trim();
                    string value = mc.Groups["value"].Value.Trim();
                    string expires = mc.Groups["expires"].Value.Trim();
                    string path = mc.Groups["path"].Value.Trim();
                    string httpOnly = mc.Groups["httponly"].Value.Trim();
                    Cookie c=new Cookie(key,value,path,response.ResponseUri.Host);
                    if (!string.IsNullOrEmpty(expires))
                    {
                        c.Expires = DateTime.Parse(expires);
                    }
                    if (httpOnly == "HttpOnly")
                    {
                        c.HttpOnly = true;
                    }
                    cookies.Add(c);
                }
            }

            //long total = response.ContentLength;
            int current = 1;
            byte[] content = new byte[1024];
            StringBuilder sb = new StringBuilder();
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
                    return;
                }
            }

            string form = sb.ToString();

            Match m= Regex.Match(form,
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
        }

        public bool IsCompleted { get { return _needtime <= 0; } }
    }
}
