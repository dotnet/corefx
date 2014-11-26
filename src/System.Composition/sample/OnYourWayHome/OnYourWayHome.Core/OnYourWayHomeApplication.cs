using System;
using System.Linq;
using OnYourWayHome.ApplicationModel;
using OnYourWayHome.ApplicationModel.Composition;
using OnYourWayHome.ApplicationModel.Presentation.Navigation;
using OnYourWayHome.ViewModels;
using OnYourWayHome.ServiceBus;
using System.Collections.Generic;
using System.Reflection;
using System.Composition.Hosting;
using System.Composition.Convention;

namespace OnYourWayHome
{
    // Represents the shared application object between device-specific 'Application' objects
    public class OnYourWayHomeApplication : DisposableObject
    {
        private readonly CompositionHost _compositionHost;

        public OnYourWayHomeApplication(Type hostType)
        {
            Requires.NotNull(hostType, "hostType");

            var conventions = new ConventionBuilder();
            
            conventions.ForTypesMatching(t => IsAPart(t)).Export()
                                                         .ExportInterfaces()
                                                         .Shared();

            conventions.ForTypesDerivedFrom<NavigatableViewModel>().Export()
                                                                   .Shared(NavigatableViewModel.SharingBoundary);

            var assemblies = new[] { typeof(OnYourWayHomeApplication).GetTypeInfo().Assembly,
                                     hostType.GetTypeInfo().Assembly };

            var configuration = new ContainerConfiguration().WithAssemblies(assemblies, conventions);            
            _compositionHost = configuration.CreateContainer();
        }

        private bool IsAPart(Type t)
        {
            return (null != t) && (null != t.Namespace) && (t.Namespace.Contains(".Parts.") || t.Namespace.EndsWith(".Parts"));
        }
        
        public void Start()
        {
            // Create all the startup services
            _compositionHost.GetExports<IStartupService>();
            
            INavigationService navigation = _compositionHost.GetExport<INavigationService>();
            navigation.NavigateTo<ShoppingListViewModel>();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _compositionHost.Dispose();
            }
        }
    }
}
