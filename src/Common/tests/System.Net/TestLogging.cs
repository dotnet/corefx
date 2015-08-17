using System.Diagnostics.Tracing;
using System.Text;
using Xunit.Abstractions;

namespace System.Net.Test.Common
{
    public class TestLogging : ITestOutputHelper
    {
        private static TestLogging _instance = new TestLogging();

        private TestLogging()
        {
        }

        public static TestLogging GetInstance()
        {
            return _instance;
        }

        public void WriteLine(string message)
        {
            EventSourceTestLogging.Log.TestMessage(message);
        }

        public void WriteLine(string format, params object[] args)
        {
            EventSourceTestLogging.Log.TestMessage(string.Format(format, args));
        }
    }
}
