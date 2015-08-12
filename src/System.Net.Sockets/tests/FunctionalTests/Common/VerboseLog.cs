using Xunit.Abstractions;

namespace System.Net.Sockets.Tests
{
    public class VerboseLog
    {
        private readonly ITestOutputHelper _output;

        public VerboseLog(ITestOutputHelper output)
        {
            _output = output;
        }

        public void Log(string str)
        {
            Log("{0}", str);
        }

        public void Log(string format, params object[] arg)
        {
#if TRAVE
            _output.WriteLine(format, arg);
#endif
        }
    }
}
