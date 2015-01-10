//---------------------------------------------------------------------------------
// Copyright (c), Microsoft Corporation
//---------------------------------------------------------------------------------

namespace OnYourWayHome.ServiceBus.Messaging
{
    using System;

    internal sealed class SyndicationItem<TContent> : SyndicationItemBase
    {
        public SyndicationItem()
        {
            this.ContentType = "application/xml";
        }

        public string ContentType { get; set; }

        public TContent Content { get; set; }
    }
}
