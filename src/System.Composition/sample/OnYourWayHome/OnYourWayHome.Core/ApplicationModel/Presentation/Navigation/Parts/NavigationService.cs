using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;
using OnYourWayHome.ApplicationModel;

namespace OnYourWayHome.ApplicationModel.Presentation.Navigation.Parts
{
    public abstract class NavigationService<TViewId> : INavigationService
    {
        private readonly Dictionary<Type, TViewId> _map = new Dictionary<Type, TViewId>();
        private readonly ExportFactory<CompositionContext> _contextFactory;

        protected NavigationService(ExportFactory<CompositionContext> contextFactory)
        {
            Requires.NotNull(contextFactory, "compositionContext");

            _contextFactory = contextFactory;
        }

        public event EventHandler CanGoBackChanged;

        public abstract bool CanGoBack
        {
            get;
        }

        public void GoBack()
        {
            GoBackCore();
            OnCanGoBackChanged(EventArgs.Empty);
        }

        public void Register(Type type, TViewId id)
        {
            _map.Add(type, id);
        }

        public void NavigateTo<TViewModel>()
            where TViewModel : NavigatableViewModel
        {
            // Get the matching view for this view model
            TViewId id;
            if (!_map.TryGetValue(typeof(TViewModel), out id))
                throw new ArgumentException();

            // Call the platform-specific override
            NavigateTo(id);

            OnCanGoBackChanged(EventArgs.Empty);
        }

        protected void Bind(TViewId id, IView view)
        {
            Type viewModelType = FindViewModelType(id);
            var viewModelContext = _contextFactory.CreateExport();    
            NavigatableViewModel viewModel = (NavigatableViewModel)viewModelContext.Value.GetExport(viewModelType);            
            view.Bind(viewModel);
            // Note: we do not dispose the viewModelContext because we do not want to dispose the parts it created
        }

        protected abstract void NavigateTo(TViewId id);

        protected abstract void GoBackCore();

        protected virtual void OnCanGoBackChanged(EventArgs e)
        {
            var handler = CanGoBackChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private Type FindViewModelType(TViewId locator)
        {
            foreach (var pair in _map)
            {
                if (pair.Value.Equals(locator))
                {
                    return pair.Key;
                }
            }

            return null;
        }
    }
}
