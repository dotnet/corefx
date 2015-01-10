using System;
using System.Runtime.Serialization;
using OnYourWayHome.ApplicationModel.Eventing;

namespace OnYourWayHome.Events
{
    [DataContract]
    public abstract class ItemEvent : IEvent
    {
        protected ItemEvent()
        {
        }

        [DataMember]
        public Guid Id
        {
            get;
            set;
        }
    }
}
