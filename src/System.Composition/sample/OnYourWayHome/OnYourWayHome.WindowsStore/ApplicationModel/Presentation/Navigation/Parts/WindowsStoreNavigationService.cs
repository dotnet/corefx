using System;
using System.Composition;
using OnYourWayHome.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace OnYourWayHome.ApplicationModel.Presentation.Navigation.Parts
{
    // WindowsStore's implementation of a navigation service that 
    // navigates from a ViewModel type to a View type
    internal class WindowsStoreNavigationService : NavigationService<Type>
    {
        public WindowsStoreNavigationService(
            [SharingBoundary(NavigatableViewModel.SharingBoundary)]
            ExportFactory<CompositionContext> contextFactory)
            : base(contextFactory)
        {
            Frame.Navigated += OnFrameNavigated;

            Register(typeof(ShoppingListViewModel),     typeof(ShoppingListView));
            Register(typeof(AddGroceryItemViewModel),   typeof(AddGroceryItemView));
        }

        public Frame Frame
        {
            get { return (Frame)Window.Current.Content; }
        }

        public override bool CanGoBack
        {
            get { return Frame.CanGoBack; }
        }

        protected override void NavigateTo(Type viewType)
        {
            Frame.Navigate(viewType);
        }

        protected override void GoBackCore()
        {
            Frame.GoBack();
        }

        private void OnFrameNavigated(object sender, NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.New)
            {
                // We use OnFrameNavigated with NavigationMode.New to identify when a new page is created
                // and bind the ViewModel implmentation to the view (Page).
                Bind(e.SourcePageType, (WindowsStoreView)e.Content);
            }
        }
    }
}
