using System;
using System.Runtime.Serialization;

namespace OnYourWayHome.Events
{
    // Published when a grocery item is added/removed from the cart
    [DataContract]
    public class ItemIsInCartChangedEvent : ItemEvent
    {
        public ItemIsInCartChangedEvent()
        {
        }

        [DataMember]
        public bool IsInCart
        {
            get;
            set;
        }
    }
}
