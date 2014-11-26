using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Web;

namespace FabrikamBooks.Parts.Tracing
{
    [Shared]
    public class TraceLogger : ILogger
    {
        public TraceLogger()
        {
        }

        public void Write(string text)
        {
            System.Diagnostics.Trace.WriteLine(text);
        }
    }
}