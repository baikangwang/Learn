using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Teacher
{
    class Program
    {
        private static void Main(string[] args)
        {
            string name = ConfigurationManager.AppSettings["Name"];//"教学中的学生发展为本";
            int bookId =int.Parse(ConfigurationManager.AppSettings["BookId"]);//35;
            int studyPlanBookId = int.Parse(ConfigurationManager.AppSettings["StudyPlanBookId"]);//22;
            int timeneed = int.Parse(ConfigurationManager.AppSettings["TimeNeed"]);//132);
            int userId = int.Parse(ConfigurationManager.AppSettings["UserId"]);//69583;
            string viewstate = ConfigurationManager.AppSettings["EventViewState"];
                //"%2FwEPDwUKMTAyNDY3NTkwNg8WBB4Fc3RhcnQFEjIwMTQtNi0xMiAyMToxMjo1Mh4GQm9va0lEBQIzNRYCAgMPZBYCAg0PDxYCHgRUZXh0BQIyOGRkZFBX1I%2BJAmiHDXbdnQwQZ%2FLCC9aG";
            string validation = ConfigurationManager.AppSettings["EventValidation"]; //"%2FwEWBQKOsJP4AQKz8dy8BQKu%2BOnrDgKE6rWYAwKR9JYal8EFMC3%2FDso2r5399QOc38lCAyM%3D";

            FixedBookLoop(name, bookId, studyPlanBookId, viewstate, validation, timeneed, userId);
        }

        private static void BookLoop()
        {
            List<Book> books = new List<Book>();

            int userId = 69583;
            books.Add(new Book("当前学生学习指导的几个主要问题", 21, 20, 82, userId));
            books.Add(new Book("教师心理健康与压力管理", 29, 21, 82, userId));
            books.Add(new Book("《小学教师专业标准（试行）》解读", 2, 17, 21, userId));
            books.Add(new Book("《义务教育品德与生活课程标准（2011 年版）》、《义务教育品德与社会课程标准（2011 年版）》对课程内容做了哪些调整", 9, 18, 39, userId));
            books.Add(new Book("《义务教育品德与生活课程标准（2011年版）》、《义务教育品德与社会课程标准（2011年版）》修订的背景、过程及总体要求", 10, 19, 66, userId));
            books.Add(new Book("教学中的学生发展为本", 35, 22, 132, userId));
            books.Add(new Book("如何进行教学设计，正确引导学生自主学习", 47, 27, 180, userId));
            books.Add(new Book("师德情怀与教育责任", 49, 28, 180, userId));
            books.Add(new Book("师德与师爱", 55, 29, 180, userId));
            bool isCompleted = false;

            while (!isCompleted)
            {
                isCompleted = true;
                foreach (Book book in books)
                {
                    book.UpdateTime(20);
                    isCompleted &= book.IsCompleted;
                }
                Console.WriteLine("Waiting for 20 minis...");
                Thread.Sleep(1000 * 60 * 20);
            }

            Console.WriteLine("All Completed!");
            Console.WriteLine("Press any key to exit...");
            Console.Read();
        }

        private static void FixedBookLoop(string name, int bookId, int studyPlanBookId, string viewstate, string validation, int needtime,
                         int userId)
        {
            FixedBook book=new FixedBook(name,bookId,studyPlanBookId,viewstate,validation,needtime,userId);

            while (!book.IsCompleted)
            {
                book.UpdateTime(20);
                if (book.IsCompleted)
                {
                    break;
                }
                Console.WriteLine("Waiting for 20 minis...");
                int interval = 20 * 60;
                long unit = 10000000;
                for (int i = 1; i <= interval; i++)
                {
                    TimeSpan t = new TimeSpan((long)i * unit);
                    Thread.Sleep(1000);
                    Console.CursorLeft = 0;
                    Console.Write(t.ToString("hh\\:mm\\:ss"));
                }
                Console.WriteLine();
            }
            Console.WriteLine("Completed!");
            Console.WriteLine("Press any key to exit...");
            Console.Read();
        }
    }
}
