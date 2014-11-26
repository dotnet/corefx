//---------------------------------------------------------------------------------
// Copyright (c), Microsoft Corporation
//---------------------------------------------------------------------------------

namespace OnYourWayHome.ServiceBus.Messaging
{
    using System;
    using System.IO;
    using System.Net;
    using OnYourWayHome.AccessControl;

    internal sealed class PeekLockMessageAsyncResult : ServiceBusRequestAsyncResult<BrokeredMessage>
    {
        private readonly string path;
        private readonly Uri uri;
        private readonly TimeSpan timeout;

        public PeekLockMessageAsyncResult(string path, TimeSpan timeout, TokenProvider tokenProvider)
            : base(tokenProvider)
        {
            this.path = path;
            this.timeout = timeout;

            this.uri = ServiceBusEnvironment.CreateServiceUri(this.TokenProvider.ServiceNamespace, path + "/Messages/Head?timeout=" + timeout.TotalSeconds);
        }

        public string Path
        {
            get { return this.path; }
        }

        public TimeSpan Timeout
        {
            get { return this.timeout; }
        }

        protected override Uri Uri
        {
            get { return this.uri; }
        }

        protected override string Method
        {
            get { return "POST"; }
        }

        protected override void OnReceiveResponse(HttpWebRequest request, HttpWebResponse response)
        {
            if (response.StatusCode == HttpStatusCode.Created)
            {
                var bodyStream = new MemoryStream();
                using (var responseStream = response.GetResponseStream())
                {
                    var buffer = new byte[1024];
                    int bytesRead = 0;
                    while ((bytesRead = responseStream.Read(buffer, 0, 1024)) > 0)
                    {
                        bodyStream.Write(buffer, 0, bytesRead);
                    }
                }

                bodyStream.Flush();
                bodyStream.Position = 0;

                this.Result = new BrokeredMessage(bodyStream, response.Headers) { ContentType = response.ContentType };
            }

            base.OnReceiveResponse(request, response);
        }
    }
}
