using System;
using System.Windows.Controls;
using System.Windows.Navigation;
using OnYourWayHome.ApplicationModel.Composition;
using OnYourWayHome.ViewModels;

namespace OnYourWayHome.ApplicationModel.Presentation.Navigation
{
    // Phone's implementation of a navigation service that
    // navigates from a ViewModel type to a Uri
    public class PhoneNavigationService : NavigationService<Uri>
    {
        public PhoneNavigationService(IResolver resolver)
            : base(resolver)
        {
            Frame.Navigated += OnFrameNavigated;

            Register(typeof(ShoppingListViewModel),     new Uri("/Views/ShoppingListView.xaml",    UriKind.Relative));
            Register(typeof(AddGroceryItemViewModel),   new Uri("/Views/AddGroceryItemView.xaml",  UriKind.Relative));
        }

        public Frame Frame
        {
            get { return (Frame)OnYourWayHomePhoneHost.Frame; }
        }

        public override bool CanGoBack
        {
            get { return Frame.CanGoBack; }
        }

        protected override void GoBackCore()
        {
            Frame.GoBack();
        }

        protected override void NavigateTo(Uri url)
        {
            Frame.Navigate(url);
        }

        private void OnFrameNavigated(object sender, NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.New)
            {
                Bind(e.Uri, (PhoneView)e.Content);
            }
        }
    }
}
