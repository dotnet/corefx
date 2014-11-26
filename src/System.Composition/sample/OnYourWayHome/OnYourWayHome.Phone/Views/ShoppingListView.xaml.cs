using System;
using OnYourWayHome.ViewModels;
using Microsoft.Phone.Shell;

namespace OnYourWayHome.ApplicationModel.Presentation.Views
{
    public partial class ShoppingListView : PhoneView
    {
        private ShoppingListViewModel _viewModel;

        public ShoppingListView()
        {
            InitializeComponent();
        }

        // The ApplicationBar doesn't support binding so we need to 
        // manually handle them in the code behind
        public override void Bind(object context)
        {
            base.Bind(context);

            _viewModel = (ShoppingListViewModel)context;
            _viewModel.CheckoutCommand.CanExecuteChanged += (_s, _e) => UpdateCheckoutButton();

            UpdateCheckoutButton();
        }

        private void AddClick(object sender, EventArgs e)
        {
            _viewModel.NavigateToAddGroceryItemCommand.Execute(null);
        }

        private void CheckoutClick(object sender, EventArgs e)
        {
            _viewModel.CheckoutCommand.Execute(null);
        }

        private void UpdateCheckoutButton()
        {
            ((ApplicationBarIconButton)this.ApplicationBar.Buttons[1]).IsEnabled = _viewModel.CheckoutCommand.CanExecute(null);
        }
    }
}