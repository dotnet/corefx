using System;
using System.Windows.Input;
using OnYourWayHome.Events;
using OnYourWayHome.ApplicationModel.Eventing;
using OnYourWayHome.ApplicationModel.Presentation.Input;
using OnYourWayHome.ApplicationModel.Presentation.Navigation;

namespace OnYourWayHome.ViewModels
{
    public class AddGroceryItemViewModel : NavigatableViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private string _name;
        private string _notificationText;
        private ICommand _addCommand;

        public AddGroceryItemViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
            : base(navigationService)
        {
            Requires.NotNull(eventAggregator, "eventAggregator");

            _eventAggregator = eventAggregator;
        }

        public ICommand AddCommand
        {
            get { return _addCommand ?? (_addCommand = new ActionCommand(Add)); }
        }

        public string Name
        {
            get { return _name ?? String.Empty; }
            set { base.SetProperty(ref _name, value, "Name"); }
        }

        public string NotificationText
        {
            get { return _notificationText ?? string.Empty; }
            set { base.SetProperty(ref _notificationText, value, "NotificationText"); }
        }

        // Adds an item to the shopping list
        private void Add()
        {
            ItemAddedEvent e = new ItemAddedEvent();
            e.Id = Guid.NewGuid();
            e.Name = Name;

            _eventAggregator.Publish(e);

            NotificationText = String.Format("{0} was added to the shopping list.", Name);
            Name = string.Empty;
        }
    }
}