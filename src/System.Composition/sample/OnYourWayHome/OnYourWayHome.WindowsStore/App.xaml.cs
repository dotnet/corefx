using System;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace OnYourWayHome
{
    public sealed partial class OnYourWayHomeWindowsStoreAppHost : Application
    {
        private readonly OnYourWayHomeApplication _application = new OnYourWayHomeApplication(typeof(OnYourWayHomeWindowsStoreAppHost));

        public OnYourWayHomeWindowsStoreAppHost()
        {
            this.InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            Window.Current.Content = new Frame();
            Window.Current.Activate();

            _application.Start();
        }
    }
}
