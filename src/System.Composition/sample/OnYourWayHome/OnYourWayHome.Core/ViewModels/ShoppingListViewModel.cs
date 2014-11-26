using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using OnYourWayHome.ApplicationModel.Eventing;
using OnYourWayHome.ApplicationModel.Presentation.Input;
using OnYourWayHome.ApplicationModel.Presentation.Navigation;
using OnYourWayHome.Events;
using OnYourWayHome.Models;
using OnYourWayHome.Models.Parts;
using OnYourWayHome.ServiceBus;

namespace OnYourWayHome.ViewModels
{
    public partial class ShoppingListViewModel : NavigatableViewModel
    {
        private readonly ShoppingList _shoppingList;
        private readonly IEventAggregator _eventAggregator;
        private readonly INavigationService _navigationService;
        private ICommand _navigateToAddGroceryItemCommand;
        private ICommand _checkoutCommand;
        private ICommand _removeItemCommand;
        private ICommand _changeIsInCartCommand;

        public ShoppingListViewModel(IEventAggregator eventAggregator, INavigationService navigationService, ShoppingList shoppingList)
            : base(navigationService)
        {
            Requires.NotNull(eventAggregator, "eventAggregator");
            Requires.NotNull(navigationService, "navigationService");
            Requires.NotNull(shoppingList, "shoppingList");

            _navigationService = navigationService;
            _shoppingList = shoppingList;
            _eventAggregator = eventAggregator;
        }

        public ICommand NavigateToAddGroceryItemCommand
        {
            get { return _navigateToAddGroceryItemCommand ?? (_navigateToAddGroceryItemCommand = new ActionCommand(NavigateToAddGroceryItem)); }
        }

        public ICommand CheckoutCommand
        {
            get { return _checkoutCommand ?? (_checkoutCommand = new CheckoutCartCommand(GroceryItems, _eventAggregator)); }
        }

        public ICommand RemoveItemCommand
        {
            get { return _removeItemCommand ?? (_removeItemCommand = new ActionCommand<GroceryItem>(RemoveItem)); }
        }

        public ICommand ChangeIsInCartCommand
        {
            get { return _changeIsInCartCommand ?? (_changeIsInCartCommand = new ActionCommand<GroceryItem>(ChangeItemIsInCart)); }
        }

        public ReadOnlyObservableCollection<GroceryItem> GroceryItems
        {
            get { return _shoppingList.GroceryItems; }
        }

        private void NavigateToAddGroceryItem()
        {
            _navigationService.NavigateTo<AddGroceryItemViewModel>();
        }

        private void RemoveItem(GroceryItem item)
        {
            ItemRemovedEvent e = new ItemRemovedEvent();
            e.Id = item.Id;

            _eventAggregator.Publish(e);
        }

        private void ChangeItemIsInCart(GroceryItem item)
        {
            ItemIsInCartChangedEvent e = new ItemIsInCartChangedEvent();
            e.Id = item.Id;
            e.IsInCart = !item.IsInCart;

            _eventAggregator.Publish(e);
        }
    }
}