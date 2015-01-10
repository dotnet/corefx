using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using OnYourWayHome.ApplicationModel.Eventing;

namespace OnYourWayHome.Events
{
    // Published when the cart is checked-out
    [DataContract]
    public class CheckoutEvent : IEvent
    {
        private Collection<Guid> _ids;

        public CheckoutEvent()
        {
        }

        [DataMember]
        public Collection<Guid> Ids
        {
            get 
            { 
                 if (_ids == null)
                    _ids = new Collection<Guid>();

                return _ids; 
            }
        }
    }
}
