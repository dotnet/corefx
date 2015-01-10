//---------------------------------------------------------------------------------
// Copyright (c), Microsoft Corporation
//---------------------------------------------------------------------------------

namespace OnYourWayHome.ServiceBus.Messaging
{
    using System;
    using System.IO;
    using System.Net;
    using OnYourWayHome.AccessControl;

    internal sealed class SendMessageAsyncResult : ServiceBusRequestAsyncResult<bool>
    {
        private readonly string path;
        private readonly BrokeredMessage message;
        private readonly Uri uri;

        public SendMessageAsyncResult(string path, BrokeredMessage message, TokenProvider tokenProvider)
            : base(tokenProvider)
        {
            this.path = path;
            this.message = message;

            this.uri = ServiceBusEnvironment.CreateServiceUri(this.TokenProvider.ServiceNamespace, path + "/Messages");
        }

        public string Path
        {
            get { return this.path; }
        }

        protected override Uri Uri
        {
            get { return this.uri; }
        }

        protected override string Method
        {
            get { return "POST"; }
        }

        protected override void OnSendRequest(HttpWebRequest request, Stream requestStream, SimpleWebToken token)
        {
            this.message.UpdateHeaderDictionary();
            foreach (var header in this.message.Headers.Keys)
            {
                var value = this.message.Headers[header];
                request.Headers[header] = value != null ? value.ToString() : null;
            }

            request.ContentType = this.message.ContentType;

            var buffer = new byte[1024];
            int bytesRead = 0;
            while ((bytesRead = this.message.BodyStream.Read(buffer, 0, 1024)) > 0)
            {
                requestStream.Write(buffer, 0, bytesRead);
            }

            base.OnSendRequest(request, requestStream, token);
        }
    }
}
