using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Tracing;

namespace System.Xml.Serialization
{
    [EventSource(Name = "System.Xml.Serialzation.XmlSerialization")]
    internal class XmlSerializationEventSource : EventSource
    {
        internal static XmlSerializationEventSource Log = new XmlSerializationEventSource();

        [Event(EventIds.XmlSerializerExpired, Level = EventLevel.Informational, 
            Message = "Pre-generated serializer '{0}' has expired. You need to re-generate serializer for '{1}'")]
        internal void XmlSerializerExpired(string serializerName, string type)
        {
            WriteEvent(EventIds.XmlSerializerExpired, serializerName, type);
        }

        public class EventIds
        {
            public const int XmlSerializerExpired = 1;
        }
    }
}
