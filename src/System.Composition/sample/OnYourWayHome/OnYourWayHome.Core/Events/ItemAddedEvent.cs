using System;
using System.Runtime.Serialization;

namespace OnYourWayHome.Events
{
    // Published when a grocery item is added to a shopping list
    [DataContract]
    public class ItemAddedEvent : ItemEvent
    {
        public ItemAddedEvent()
        {
        }

        [DataMember]
        public string Name
        {
            get;
            set;
        }
    }
}
