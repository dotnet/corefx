//---------------------------------------------------------------------------------
// Copyright (c), Microsoft Corporation
//---------------------------------------------------------------------------------

namespace OnYourWayHome.ServiceBus.Messaging
{
    using System;
    using System.IO;
    using System.Net;
    using OnYourWayHome.AccessControl;

    internal sealed class CreateEntityAsyncResult<T> : ServiceBusRequestAsyncResult<Entity<T>>
    {
        private readonly string path;
        private readonly T requestEntity;
        private readonly Uri uri;

        public CreateEntityAsyncResult(string path, T entityDescription, TokenProvider tokenProvider)
            : base(tokenProvider)
        {
            this.path = path;
            this.requestEntity = entityDescription;

            this.uri = ServiceBusEnvironment.CreateServiceUri(this.TokenProvider.ServiceNamespace, path);
        }

        public string Path
        {
            get { return this.path; }
        }

        public T EntityDescription
        {
            get { return this.requestEntity; }
        }

        protected override Uri Uri
        {
            get { return this.uri; }
        }

        protected override string Method
        {
            get { return "PUT"; }
        }

        protected override void OnSendRequest(HttpWebRequest request, Stream requestStream, SimpleWebToken token)
        {
            request.ContentType = "application/atom+xml";

            var entity = new Entity<T>(this.path, this.requestEntity);
            using (var streamWriter = new StreamWriter(requestStream))
            {
                streamWriter.Write(entity.ToString());
                streamWriter.Flush();
            }

            base.OnSendRequest(request, requestStream, token);
        }

        protected override void OnReceiveResponse(HttpWebRequest request, HttpWebResponse response)
        {
            if (response.StatusCode == HttpStatusCode.Created)
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
