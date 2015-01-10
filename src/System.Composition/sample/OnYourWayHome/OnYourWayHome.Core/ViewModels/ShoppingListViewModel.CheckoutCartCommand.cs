using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using OnYourWayHome.ApplicationModel.Eventing;
using OnYourWayHome.ApplicationModel.Presentation.Input;
using OnYourWayHome.Events;
using OnYourWayHome.Models;

namespace OnYourWayHome.ViewModels
{
    partial class ShoppingListViewModel
    {
        // Checks out a cart of items
        internal class CheckoutCartCommand : Command
        {
            private readonly IEventAggregator _eventAggregator;
            private readonly ReadOnlyObservableCollection<GroceryItem> _groceryItems;

            public CheckoutCartCommand(ReadOnlyObservableCollection<GroceryItem> groceryItems, IEventAggregator eventAggregator)
            {
                Assumes.NotNull(groceryItems);
                Assumes.NotNull(eventAggregator);

                _groceryItems = groceryItems;
                _eventAggregator = eventAggregator;

                var collectionChanged = ((INotifyCollectionChanged)groceryItems);
                collectionChanged.CollectionChanged += EventServices.MakeWeak(OnCollectionChanged, h => collectionChanged.CollectionChanged -= h);
            }

            public override bool CanExecute()
            {
                return _groceryItems.Any(g => g.IsInCart);
            }

            public override void Execute()
            {
                CheckoutEvent e = new CheckoutEvent();
                foreach (GroceryItem item in _groceryItems)
                {
                    if (item.IsInCart)
                        e.Ids.Add(item.Id);
                }

                _eventAggregator.Publish(e);
            }

            public void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                FireCanExecuteChanged();
            }
        }
    }
}