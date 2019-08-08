// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.Serialization;

#if ES_BUILD_STANDALONE
using Environment = Microsoft.Diagnostics.Tracing.Internal.Environment;
#endif

#if ES_BUILD_STANDALONE
namespace Microsoft.Diagnostics.Tracing
#else
namespace System.Diagnostics.Tracing
#endif
{
    /// <summary>
    /// Exception that is thrown when an error occurs during EventSource operation.
    /// </summary>
#if !ES_BUILD_PCL
    [Serializable]
#endif
    public class EventSourceException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the EventSourceException class.
        /// </summary>
        public EventSourceException() :
            base(SR.EventSource_ListenerWriteFailure) { }

        /// <summary>
        /// Initializes a new instance of the EventSourceException class with a specified error message.
        /// </summary>
        public EventSourceException(string? message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the EventSourceException class with a specified error message
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        public EventSourceException(string? message, Exception? innerException) : base(message, innerException) { }

#if !ES_BUILD_PCL
        /// <summary>
        /// Initializes a new instance of the EventSourceException class with serialized data.
        /// </summary>
        protected EventSourceException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif

        internal EventSourceException(Exception? innerException) :
            base(SR.EventSource_ListenerWriteFailure, innerException) { }
    }
}
