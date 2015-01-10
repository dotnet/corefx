using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnYourWayHome.ApplicationModel.Presentation.Navigation
{
    public interface INavigationService
    {
        event EventHandler CanGoBackChanged;

        bool CanGoBack
        {
            get;
        }

        void GoBack();

        void NavigateTo<TViewModel>()
            where TViewModel : NavigatableViewModel;
    }
}
