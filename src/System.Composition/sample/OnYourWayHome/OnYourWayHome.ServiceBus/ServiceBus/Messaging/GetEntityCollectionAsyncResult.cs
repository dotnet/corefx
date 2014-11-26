//---------------------------------------------------------------------------------
// Copyright (c), Microsoft Corporation
//---------------------------------------------------------------------------------

namespace OnYourWayHome.ServiceBus.Messaging
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Xml;
    using OnYourWayHome.AccessControl;
    using System.Collections.ObjectModel;

    internal sealed class GetEntityCollectionAsyncResult<T> : ServiceBusRequestAsyncResult<IEnumerable<Entity<T>>>
    {
        private readonly string path;
        private readonly Uri uri;

        public GetEntityCollectionAsyncResult(string path, TokenProvider tokenProvider)
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
                    var result = new List<Entity<T>>();
                    var syndicationFeed = ReadAtomFeed(responseStream);

                    foreach (var item in syndicationFeed.Items)
                    {
                        result.Add(new Entity<T>(item));
                    }

                    this.Result = new ReadOnlyCollection<Entity<T>>(result);
                }
            }

            base.OnReceiveResponse(request, response);
        }

        private static SyndicationFeed<T> ReadAtomFeed(Stream stream)
        {
            SyndicationFeed<T> result;

            using (var reader = XmlReader.Create(stream))
            {
                result = Entity<T>.Serializer.DeserializeFeed(reader);
            }

            return result;
        }
    }
}
