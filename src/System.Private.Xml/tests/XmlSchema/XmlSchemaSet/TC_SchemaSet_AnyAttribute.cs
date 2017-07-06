using Xunit;
using Xunit.Abstractions;
using System.IO;
using System.Xml.Schema;

namespace System.Xml.Tests
{
    public class TC_SchemaSet_AnyAttribute : TC_SchemaSetBase
    {
        private ITestOutputHelper _output;

        public TC_SchemaSet_AnyAttribute(ITestOutputHelper output)
        {
            _output = output;
        }

        public bool bWarningCallback;

        public bool bErrorCallback;
        public int errorCount;
        public int warningCount;
        public bool WarningInnerExceptionSet = false;
        public bool ErrorInnerExceptionSet = false;

        public void Initialize()
        {
            bWarningCallback = bErrorCallback = false;
            errorCount = warningCount = 0;
            WarningInnerExceptionSet = ErrorInnerExceptionSet = false;
        }

        //hook up validaton callback
        public void ValidationCallback(object sender, ValidationEventArgs args)
        {
            if (args.Severity == XmlSeverityType.Warning)
            {
                _output.WriteLine("WARNING: ");
                bWarningCallback = true;
                warningCount++;
                WarningInnerExceptionSet = (args.Exception.InnerException != null);
                _output.WriteLine("\nInnerExceptionSet : " + WarningInnerExceptionSet + "\n");
            }
            else if (args.Severity == XmlSeverityType.Error)
            {
                _output.WriteLine("ERROR: ");
                bErrorCallback = true;
                errorCount++;
                ErrorInnerExceptionSet = (args.Exception.InnerException != null);
                _output.WriteLine("\nInnerExceptionSet : " + ErrorInnerExceptionSet + "\n");
            }

            _output.WriteLine(args.Message); // Print the error to the screen.
        }
    }
}
