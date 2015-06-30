
namespace NetPrimitivesUnitTests
{
    using System;
    public static class Logger
    {
        public static void LogInformation(string format, params object[] arg)
        {
            Console.WriteLine(format, arg);
        }
    }
}
