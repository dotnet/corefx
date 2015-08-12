using System.Diagnostics.Tracing;
using System.Text;
using Xunit.Abstractions;

namespace System.Net.Test.Common
{
    public class VerboseTestLogging : ITestOutputHelper
    {
        private static VerboseTestLogging _instance = new VerboseTestLogging();

        private VerboseTestLogging()
        {
        }

        public static VerboseTestLogging GetInstance()
        {
            return _instance;
        }

        public void WriteLine(string message)
        {
            EventSourceTestLogging.Log.TestVerboseMessage(message);
        }

        public void WriteLine(string format, params object[] args)
        {
            EventSourceTestLogging.Log.TestVerboseMessage(string.Format(format, args));
        }
    }
}
