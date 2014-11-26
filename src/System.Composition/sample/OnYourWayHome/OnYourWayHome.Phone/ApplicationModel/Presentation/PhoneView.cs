using System;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using System.ComponentModel;

namespace OnYourWayHome.ApplicationModel.Presentation
{
    // Phone's implementation of a View
    public class PhoneView : PhoneApplicationPage, IView
    {
        public virtual void Bind(object context)
        {
            Requires.NotNull(context, "context");

            DataContext = context;
        }
    }
}
