using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Tracing;

namespace System.Xml.Serialization
{
    [EventSource(Name = "System.Xml.Serialzation.XmlSerializationEventSource")]
    internal class XmlSerializationEventSource : EventSource
    {
        internal static XmlSerializationEventSource Log = new XmlSerializationEventSource();

        private const int TraceEventId = 1;

        [Event(TraceEventId, Level = EventLevel.Informational)]
        internal void Trace(string message)
        {
            WriteEvent(TraceEventId, message);
        }
    }
}
