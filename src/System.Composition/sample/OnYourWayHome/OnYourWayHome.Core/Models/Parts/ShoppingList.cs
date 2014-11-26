using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using OnYourWayHome.ApplicationModel.Eventing;
using OnYourWayHome.Events;
using OnYourWayHome.ApplicationModel.Composition;

namespace OnYourWayHome.Models.Parts
{
    // Handles the shopping list
    public class ShoppingList : IStartupService, IEventHandler<ItemAddedEvent>, IEventHandler<ItemRemovedEvent>, IEventHandler<ItemIsInCartChangedEvent>, IEventHandler<CheckoutEvent>
    {
        private readonly GroceryItemCollection _groceryItems = new GroceryItemCollection();
        private readonly IEventAggregator _eventAggregator;

        public ShoppingList(IEventAggregator eventAggregator)
        {
            Requires.NotNull(eventAggregator, "eventAggregator");

            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe<ItemAddedEvent>(this);
            _eventAggregator.Subscribe<ItemRemovedEvent>(this);
            _eventAggregator.Subscribe<ItemIsInCartChangedEvent>(this);
            _eventAggregator.Subscribe<CheckoutEvent>(this);

            // TODO: Add some sample data to populate the screen until persistence is added
            _groceryItems.Add(new GroceryItem() { Name = "Cheese", Id = new Guid("{73509AFC-E032-4ED8-8C32-11388B9233F8}") });
            _groceryItems.Add(new GroceryItem() { Name = "Bread", Id = new Guid("{83509AFC-E032-4ED8-8C32-11388B9233F8}") });
            _groceryItems.Add(new GroceryItem() { Name = "Milk", Id = new Guid("{93509AFC-E032-4ED8-8C32-11388B9233F8}") });
        }

        public ReadOnlyObservableCollection<GroceryItem> GroceryItems
        {
            get { return _groceryItems; }
        }

        public void Handle(ItemAddedEvent e)
        {
            Requires.NotNull(e, "e");

            GroceryItem item = new GroceryItem();
            item.Id = e.Id;
            item.Name = e.Name;
            item.IsInCart = false;

            _groceryItems.Add(item);
        }

        public void Handle(ItemRemovedEvent e)
        {
            Requires.NotNull(e, "e");

            GroceryItem item = _groceryItems.Find(e.Id);
            if (item != null) 
                _groceryItems.Remove(item);
        }

        public void Handle(ItemIsInCartChangedEvent e)
        {
            Requires.NotNull(e, "e");

            GroceryItem item = _groceryItems.Find(e.Id);
            if (item != null)
                item.IsInCart = e.IsInCart;
        }

        public void Handle(CheckoutEvent e)
        {
            Requires.NotNull(e, "e");

            foreach (Guid id in e.Ids)
            {
                GroceryItem item = _groceryItems.Find(id);
                if (item == null)
                    continue;

                _groceryItems.Remove(item);
            }
        }

        private class GroceryItemCollection : ReadOnlyObservableCollection<GroceryItem>
        {
            public GroceryItemCollection()
                : base(new ObservableCollection<GroceryItem>())
            {
            }

            public void Add(GroceryItem item)
            {
                Items.Add(item);
                item.PropertyChanged += OnItemPropertyChanged;
            }

            public void Remove(GroceryItem item)
            {
                Items.Remove(item);
                item.PropertyChanged -= OnItemPropertyChanged;
            }

            public GroceryItem Find(Guid id)
            {
                GroceryItem item = this.Where(i => id == i.Id)
                                       .SingleOrDefault();

                return item;
            }

            private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                GroceryItem item = (GroceryItem)sender;
                int index = IndexOf(item);

                Assumes.True(index > -1);

                // Fire collection changed. 
                // (This can't just set Item[index] because Windows Phone 7.5 has a 
                // bug where doing a set throws ArgumentOutOfRangeException in the 
                // binding infrastructure.)
                Items.RemoveAt(index);
                Items.Insert(index, item);
            }
        }
    }
}
