//---------------------------------------------------------------------------------
// Copyright (c), Microsoft Corporation
//---------------------------------------------------------------------------------

namespace OnYourWayHome.ServiceBus.Messaging
{
    using System;
    using System.Net;
    using OnYourWayHome.AccessControl;

    internal sealed class DeleteEntityAsyncResult : ServiceBusRequestAsyncResult<bool>
    {
        private readonly string path;
        private readonly Uri uri;

        public DeleteEntityAsyncResult(string path, TokenProvider tokenProvider)
            : base(tokenProvider)
        {
            this.path = path;
            this.uri = ServiceBusEnvironment.CreateServiceUri(this.TokenProvider.ServiceNamespace, path);
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
            get { return "DELETE"; }
        }

        protected override void OnReceiveResponse(HttpWebRequest request, HttpWebResponse response)
        {
            this.Result = response.StatusCode == HttpStatusCode.OK;

            base.OnReceiveResponse(request, response);
        }
    }
}
