//---------------------------------------------------------------------------------
// Copyright (c), Microsoft Corporation
//---------------------------------------------------------------------------------

namespace OnYourWayHome.ServiceBus.Messaging
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    internal sealed class SyndicationFeed<TContent> : SyndicationItemBase
    {
        private readonly Collection<SyndicationItem<TContent>> items = new Collection<SyndicationItem<TContent>>();

        public ICollection<SyndicationItem<TContent>> Items
        {
            get { return this.items; }
        }
    }
}
