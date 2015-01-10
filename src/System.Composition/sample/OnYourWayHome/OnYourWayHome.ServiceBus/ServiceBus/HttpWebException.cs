//---------------------------------------------------------------------------------
// Copyright (c), Microsoft Corporation
//---------------------------------------------------------------------------------

namespace OnYourWayHome.ServiceBus
{
    using System;
    using System.Net;

    public class HttpWebException : WebException
    {
        private readonly HttpStatusCode statusCode;

        public HttpWebException(HttpStatusCode statusCode, string message, Exception innerException)
            : base(message, innerException)
        {
            this.statusCode = statusCode;
        }

        public HttpStatusCode StatusCode
        {
            get { return this.statusCode; }
        }
    }
}
