// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Net.Http;

namespace System.Net
{
    public partial class WebException : InvalidOperationException
    {
        private const WebExceptionStatus DefaultStatus = WebExceptionStatus.UnknownError;

        private WebExceptionStatus _status = DefaultStatus;
        private WebResponse _response = null;

        public WebException()
        {
        }

        public WebException(string message) :
            base(message)
        {
        }

        public WebException(string message, Exception innerException) :
            this(message, innerException, DefaultStatus, null)
        {
        }

        public WebException(string message, WebExceptionStatus status) :
            this(message, null, status, null)
        {
        }

        public WebException(string message,
                            Exception innerException,
                            WebExceptionStatus status,
                            WebResponse response) :
            base(message, innerException)
        {
            _status = status;
            _response = response;

            if (innerException != null)
            {
                HResult = innerException.HResult;
            }
        }

        public WebExceptionStatus Status
        {
            get
            {
                return _status;
            }
        }

        public WebResponse Response
        {
            get
            {
                return _response;
            }
        }

        internal static Exception CreateCompatibleException(Exception exception)
        {
            Debug.Assert(exception != null);
            if (exception is HttpRequestException)
            {
                Exception inner = exception.InnerException;
                string message = inner != null ?
                    string.Format("{0} {1}", exception.Message, inner.Message) :
                    exception.Message;

                return new WebException(
                    message,
                    exception,
                    GetStatusFromException(exception as HttpRequestException),
                    null);
            }

            return exception;
        }
    }
}
