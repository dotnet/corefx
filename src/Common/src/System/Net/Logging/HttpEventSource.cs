// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.Tracing;
using System.Globalization;

namespace System.Net
{
    [EventSource(Name = "Microsoft-System-Net-Http",
        Guid = "bdd9a83e-1929-5482-0d73-2fe5e1c0e16d")]
    internal sealed class HttpEventSource : EventSource
    {
        private const int ASSOCIATE_ID = 1;
        private const int URI_BASE_ADDRESS_ID = 2;

        private static HttpEventSource s_log = new HttpEventSource();
        private HttpEventSource() { }
        public static HttpEventSource Log
        {
            get
            {
                return s_log;
            }
        }

        internal static void Associate(object objA, object objB)
        {
            s_log.Associate(LoggingHash.GetObjectName(objA),
                            LoggingHash.HashInt(objA),
                            LoggingHash.GetObjectName(objB),
                            LoggingHash.HashInt(objB));
        }

        [Event(ASSOCIATE_ID, Keywords = Keywords.Default,
            Level = EventLevel.Informational, Message = "[{0}#{1}]<-->[{2}#{3}]")]
        internal unsafe void Associate(string objectA, int objectAHash, string objectB, int objectBHash)
        {
            const int SIZEDATA = 4;
            fixed (char* arg1Ptr = objectA, arg2Ptr = objectB)
            {
                EventData* dataDesc = stackalloc EventSource.EventData[SIZEDATA];

                dataDesc[0].DataPointer = (IntPtr)(arg1Ptr);
                dataDesc[0].Size = (objectA.Length + 1) * sizeof(char); // Size in bytes, including a null terminator. 
                dataDesc[1].DataPointer = (IntPtr)(&objectAHash);
                dataDesc[1].Size = sizeof(int);
                dataDesc[2].DataPointer = (IntPtr)(arg2Ptr);
                dataDesc[2].Size = (objectB.Length + 1) * sizeof(char);
                dataDesc[3].DataPointer = (IntPtr)(&objectBHash);
                dataDesc[3].Size = sizeof(int);

                WriteEventCore(ASSOCIATE_ID, SIZEDATA, dataDesc);
            }
        }
        [NonEvent]
        internal static void UriBaseAddress(object obj, string baseAddress)
        {
            s_log.UriBaseAddress(baseAddress, LoggingHash.GetObjectName(obj), LoggingHash.HashInt(obj));
        }
        [Event(URI_BASE_ADDRESS_ID, Keywords = Keywords.Debug,
            Level = EventLevel.Informational)]
        internal unsafe void UriBaseAddress(string uriBaseAddress, string objName, int objHash)
        {
            const int SIZEDATA = 3;
            fixed (char* arg1Ptr = uriBaseAddress, arg2Ptr = objName)
            {
                EventData* dataDesc = stackalloc EventSource.EventData[SIZEDATA];

                dataDesc[0].DataPointer = (IntPtr)(arg1Ptr);
                dataDesc[0].Size = (uriBaseAddress.Length + 1) * sizeof(char); // Size in bytes, including a null terminator. 
                dataDesc[1].DataPointer = (IntPtr)(arg2Ptr);
                dataDesc[1].Size = (objName.Length + 1) * sizeof(char);
                dataDesc[2].DataPointer = (IntPtr)(&objHash);
                dataDesc[2].Size = sizeof(int);

                WriteEventCore(URI_BASE_ADDRESS_ID, SIZEDATA, dataDesc);
            }
        }

        public class Keywords
        {
            public const EventKeywords Default = (EventKeywords)0x0001;
            public const EventKeywords Debug = (EventKeywords)0x0002;
        }
    }
}
