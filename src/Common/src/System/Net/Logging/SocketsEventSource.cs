// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.Tracing;
using System.Globalization;
using System.Net.Sockets;

namespace System.Net
{

    [EventSource(Name = "Microsoft-System-Net-Sockets",
        Guid = "e03c0352-f9c9-56ff-0ea7-b94ba8cabc6b",
        LocalizationResources = "FxResources.System.Net.Sockets.SR")]
    internal sealed class SocketsEventSource : EventSource
    {
        private const int ACCEPTED_ID = 1;

        private static SocketsEventSource s_log = new SocketsEventSource();
        private SocketsEventSource() { }
        public static SocketsEventSource Log
        {
            get
            {
                return s_log;
            }
        }
        [NonEvent]
        internal static void Accepted(Socket socket, object localEp, object remoteEp)
        {
            if (!s_log.IsEnabled())
            {
                return;
            }
            s_log.Accepted(LoggingHash.GetObjectName(localEp), LoggingHash.GetObjectName(remoteEp), LoggingHash.HashInt(socket));
        }

        [Event(ACCEPTED_ID, Keywords = Keywords.Default,
            Level = EventLevel.Informational)]
        internal unsafe void Accepted(string localEp, string remoteEp, int socketHash)
        {
            const int SIZEDATA = 3;
            fixed (char* arg1Ptr = localEp, arg2Ptr = remoteEp)
            {

                EventData* dataDesc = stackalloc EventSource.EventData[SIZEDATA];
                dataDesc[0].DataPointer = (IntPtr)(arg1Ptr);
                dataDesc[0].Size = (localEp.Length + 1) * sizeof(char); // Size in bytes, including a null terminator. 
                dataDesc[1].DataPointer = (IntPtr)(arg2Ptr);
                dataDesc[1].Size = (remoteEp.Length + 1) * sizeof(char);
                dataDesc[2].DataPointer = (IntPtr)(&socketHash);
                dataDesc[2].Size = sizeof(int);

                WriteEventCore(ACCEPTED_ID, SIZEDATA, dataDesc);
            }
        }

        public class Keywords
        {
            public const EventKeywords Default = (EventKeywords)0x0001;
            public const EventKeywords Debug = (EventKeywords)0x0002;
        }
    }
}
