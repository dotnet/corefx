using System;
using System.Runtime.Serialization;

namespace OnYourWayHome.Events
{
    // Published when a grocery item is removed from a shopping list
    [DataContract]
    public class ItemRemovedEvent : ItemEvent
    {
        public ItemRemovedEvent()
        {
        }
    }
}
