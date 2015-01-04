using System;
using System.Threading;

namespace ProcessTest_ConsoleApp
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length > 0)
            {
                if (args[0].Equals("infinite"))
                {
                    Console.WriteLine("ProcessTest_ConsoleApp.exe started with an endless loop");
                    Thread.Sleep(-1);
                }
                if (args[0].Equals("error"))
                {
                    Exception ex = new Exception("Intentional Exception thrown");
                    throw ex;
                }
                if (args[0].Equals("input"))
                {
                    string str = Console.ReadLine();
                    Console.WriteLine(str);
                }
            }
            else
            {
                Console.WriteLine("ProcessTest_ConsoleApp.exe started");
                Console.WriteLine("ProcessTest_ConsoleApp.exe closed");
            }

            return 100;
        }
    }
}
