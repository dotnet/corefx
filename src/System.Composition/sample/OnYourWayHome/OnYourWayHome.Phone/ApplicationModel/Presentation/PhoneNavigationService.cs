using System;
using System.Windows.Controls;
using System.Windows;
using System.Collections.Generic;
using OnYourWayHome.Presentation.ViewModels;

namespace OnYourWayHome.Presentation
{
    public class PhoneNavigationService : INavigationService
    {
        // TODO: Have a better way to doing this, rather a hard-coded list
        private static readonly Dictionary<Type, string> _map = new Dictionary<Type, string>() { { typeof(ShoppingListViewModel),   "/Presentation/Views/ShoppingListView.xaml"},
                                                                                                 { typeof(AddGroceryItemViewModel), "/Presentation/Views/AddGroceryItemView.xaml"} };

        public PhoneNavigationService()
        {   
        }

        public Frame Frame
        {
            get { return ((OnYourWayHomePhoneApp)Application.Current).Frame; }
        }

        public bool CanGoBack
        {
            get { return Frame.CanGoBack; }
        }

        public void GoBack()
        {
            Frame.GoBack();
        }

        public void NavigateTo(Type type)
        {
            string uri;
            if (!_map.TryGetValue(type, out uri))
                throw new ArgumentException();
            
            Frame.Navigate(new Uri(uri, UriKind.Relative));
        }
    }
}
