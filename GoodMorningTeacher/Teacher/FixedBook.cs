using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Teacher
{
    public class FixedBook
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
        private string _viewState;
        private string _validation;

        public bool IsCompleted { get { return _needtime <= 0; } }
        
        public FixedBook(string name, int bookId, int studyPlanBookId, string viewstate, string validation, int needtime,
                         int userId)
        {
            this._name = name;
            this._bookId = bookId;
            this._studyPlanBookId = studyPlanBookId;
            this._userId = userId;
            this._needtime = needtime;
            this._viewState = viewstate;
            this._validation = validation;
        }

        public void UpdateTime(int minites)
        {
            if (this._needtime <= 0)
                return;

            string viewState = this._viewState;
            string eventValidation = this._validation;

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
                string.Format("http://nankaiqu.fxlxm.teacher.com.cn/GuoPeiAdmin/CourseStudy/RecordTimeNew.aspx?bookid={0}&StudyPlanBookID={1}", this._bookId, this._studyPlanBookId);
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Headers.Add("Accept-Language", "en-US");
            request.Headers.Add("Accept-Encoding", "gzip,deflate");
            request.KeepAlive = true;
            request.Headers.Add("Cache-Control", "no-cache");

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
                long total = response.ContentLength;
                int current = 0;
                content = new byte[1024];
                StringBuilder sb = new StringBuilder();
                using (Stream rs = response.GetResponseStream())
                {
                    if (rs != null)
                    {
                        while (total>current)
                        {
                            current += rs.Read(content, 0, content.Length);
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
                Console.WriteLine("{1}: {0} -> Failed, {2}", this.Name, DateTime.Now.ToShortTimeString(), response.StatusDescription);
            }
        }
    }
}
