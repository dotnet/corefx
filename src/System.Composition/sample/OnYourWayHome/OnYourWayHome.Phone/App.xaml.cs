using System;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;

namespace OnYourWayHome
{
    public partial class OnYourWayHomePhoneHost : Application
    {
        private readonly OnYourWayHomeApplication _application = new OnYourWayHomeApplication(typeof(OnYourWayHomePhoneHost));

        public OnYourWayHomePhoneHost()
        {
            InitializeComponent();
            InitializePhoneApplication();
        }

        public static PhoneApplicationFrame Frame 
        { 
            get; 
            private set; 
        }

        private void InitializePhoneApplication()
        {
            Frame = new PhoneApplicationFrame();
            Frame.Navigated += OnFrameNavigated;

            _application.Start();
        }

        private void OnFrameNavigated(object sender, NavigationEventArgs e)
        {
            if (RootVisual != Frame)
                RootVisual = Frame;

            Frame.Navigated -= OnFrameNavigated;
        }
    }
}