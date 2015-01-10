//---------------------------------------------------------------------------------
// Copyright (c), Microsoft Corporation
//---------------------------------------------------------------------------------

namespace OnYourWayHome.ServiceBus
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Xml;
    using OnYourWayHome.AccessControl;

    internal abstract class ServiceBusRequestAsyncResult<T> : RestAsyncResult<T>
    {
        private static readonly XmlWriterSettings defaultXmlWriterSettings = new XmlWriterSettings() { Encoding = Encoding.UTF8 };

        private readonly TokenProvider tokenProvider;

        public ServiceBusRequestAsyncResult(TokenProvider tokenProvider)
        {
            if (tokenProvider == null)
            {
                throw new ArgumentNullException("tokenProvider");
            }

            this.tokenProvider = tokenProvider;
        }

        public TokenProvider TokenProvider
        {
            get { return this.tokenProvider; }
        }

        protected sealed override void OnSendRequest(HttpWebRequest request, Stream requestStream)
        {
            this.OnGetToken(request, requestStream);
        }

        protected virtual void OnGetToken(HttpWebRequest request, Stream requestStream)
        {
            var asyncState = new GetTokenAsyncState { Request = request, RequestStream = requestStream };
            this.tokenProvider.BeginGetToken(false, this.GetTokenCompleted, asyncState);
        }

        protected virtual void OnSendRequest(HttpWebRequest request, Stream requestStream, SimpleWebToken token)
        {
            request.Headers[HttpRequestHeader.Authorization] = string.Format("WRAP access_token=\"{0}\"", token.TokenString);

            if (requestStream != null)
            {
                requestStream.Dispose();
            }

            base.OnSendRequest(request, requestStream);
        }

        private void GetTokenCompleted(IAsyncResult ar)
        {
            try
            {
                var state = (GetTokenAsyncState)ar.AsyncState;
                var token = this.tokenProvider.EndGetToken(ar);
                this.OnSendRequest(state.Request, state.RequestStream, token);
            }
            catch (Exception exception)
            {
                this.SetCompleted(exception, false);
            }
        }

        private class GetTokenAsyncState
        {
            public HttpWebRequest Request { get; set; }

            public Stream RequestStream { get; set; }
        }
    }
}
