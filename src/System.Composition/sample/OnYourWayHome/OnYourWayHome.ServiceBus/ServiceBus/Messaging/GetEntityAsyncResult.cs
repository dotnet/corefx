//---------------------------------------------------------------------------------
// Copyright (c), Microsoft Corporation
//---------------------------------------------------------------------------------

namespace OnYourWayHome.ServiceBus.Messaging
{
    using System;
    using System.Net;
    using OnYourWayHome.AccessControl;

    internal sealed class GetEntityAsyncResult<T> : ServiceBusRequestAsyncResult<Entity<T>>
    {
        private readonly string path;
        private readonly Uri uri;

        public GetEntityAsyncResult(string path, TokenProvider tokenProvider)
            : base(tokenProvider)
        {
            this.path = path;
            this.uri = ServiceBusEnvironment.CreateServiceUri(this.TokenProvider.ServiceNamespace, path);
        }

        public string Path
        {
            get { return this.path; }
        }

        public string TopicPath { get; set; }

        protected override Uri Uri
        {
            get { return this.uri; }
        }

        protected override string Method
        {
            get { return "GET"; }
        }

        protected override void OnReceiveResponse(HttpWebRequest request, HttpWebResponse response)
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (var responseStream = response.GetResponseStream())
                {
                    this.Result = Entity<T>.Create(responseStream);
                }
            }

            base.OnReceiveResponse(request, response);
        }
    }
}
