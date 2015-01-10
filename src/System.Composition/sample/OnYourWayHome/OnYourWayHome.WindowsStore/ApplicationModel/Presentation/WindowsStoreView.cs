using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace OnYourWayHome.ApplicationModel.Presentation
{
    // WindowsStore's implementation of a View
    public class WindowsStoreView : Page, IView
    {
        public WindowsStoreView()
        {
        }

        public void Bind(object context)
        {
            Requires.NotNull(context, "context");

            DataContext = context;
        }
    }
}
