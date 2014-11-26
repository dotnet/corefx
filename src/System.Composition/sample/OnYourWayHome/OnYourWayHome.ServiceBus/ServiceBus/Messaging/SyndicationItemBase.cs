//---------------------------------------------------------------------------------
// Copyright (c), Microsoft Corporation
//---------------------------------------------------------------------------------

namespace OnYourWayHome.ServiceBus.Messaging
{
    using System;

    internal abstract class SyndicationItemBase
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public DateTimeOffset? PublishDate { get; set; }

        public DateTimeOffset? LastUpdatedTime { get; set; }

        public string Author { get; set; }

        public Uri SelfLink { get; set; }
    }
}
