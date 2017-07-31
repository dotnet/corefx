// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.Serialization;
using System.Threading;
using Microsoft.CSharp.RuntimeBinder.Errors;

namespace Microsoft.CSharp.RuntimeBinder
{
    /// <summary>
    /// Represents an error that occurs while processing a dynamic bind in the C# runtime binder. Exceptions of this type differ from <see cref="RuntimeBinderInternalCompilerException"/> in that
    /// <see cref="RuntimeBinderException"/> represents a failure to bind in the sense of a usual compiler error, whereas <see cref="RuntimeBinderInternalCompilerException"/>
    /// represents a malfunctioning of the runtime binder itself.
    /// </summary>
    public class RuntimeBinderException : Exception
    {
        private bool _helpLinkOverwritten;

        private static class BingUriGenerator
        {
            private const string BingSearchString = "https://bingdev.cloudapp.net/BingUrl.svc/Get?selectedText=&mainLanguage=C%23&projectType=%7BFAE04EC0-301F-11D3-BF4B-00C04F79EFBC%7D&requestId={0}&clientId=&errorCode=CS{1:D4}";
            private static readonly string s_requestID = Uri.EscapeDataString(Guid.NewGuid().ToString());

            public static string BingUri(ErrorCode code) => string.Format(BingSearchString, s_requestID, (int)code);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeBinderException"/> class. 
        /// </summary>
        public RuntimeBinderException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeBinderException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public RuntimeBinderException(string message)
            : base(message)
        {
        }

        internal RuntimeBinderException(ErrorCode code, string message)
            : this(message)
        {
            ErrorCode = code;
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref="RuntimeBinderException"/> class with a specified error message
        ///  and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public RuntimeBinderException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeBinderException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/>  that contains contextual information about the source or destination.</param>
        protected RuntimeBinderException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            throw new PlatformNotSupportedException();
        }

        internal ErrorCode ErrorCode { get; }

        public override string HelpLink
        {
            get
            {
                if (_helpLinkOverwritten || ErrorCode <= 0)
                {
                    return base.HelpLink;
                }

                return BingUriGenerator.BingUri(ErrorCode);
            }
            set
            {
                _helpLinkOverwritten = true;
                base.HelpLink = value;
            }
        }
    }
}
