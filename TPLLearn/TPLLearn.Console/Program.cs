using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TPLLearn
{
    using System.Diagnostics;
    using System.Threading.Tasks;

    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch watcher = Stopwatch.StartNew();
            Parallel.For(2, 20, (i) =>
            {
                var result = SumRootN(i);
                Console.WriteLine("root {0} : {1}", i, result);
            });

            Console.WriteLine(watcher.Elapsed);
            Console.Read();
        }

        public static double SumRootN(int root)
        {
            double result = 0;
            for (int i = 1; i < 10000000; i++)
            {
                result += Math.Exp(Math.Log(i) / root);
            }

            return result;
        }
    }
}
