using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OnYourWayHome.ApplicationModel.Presentation;

namespace OnYourWayHome.Models
{
    public class GroceryItem : Bindable
    {
        private Guid _id;
        private string _name;
        private bool _isInCart;

        public GroceryItem()
        {
        }

        public Guid Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value, "Id"); }
        }

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value, "Name"); }
        }

        public bool IsInCart
        {
            get { return _isInCart; }
            set { SetProperty(ref _isInCart, value, "IsInCart"); }
        }
    }
}
