// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// -----------------------------------------------------------------------
// Copyright © Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.Composition.Hosting.Core;
using System.Composition.Convention;
using System.Composition.Runtime;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.Composition.Demos.Web.Mvc
{
    /// <summary>
    /// Provides composition services to ASP.NET MVC by integrating DependencyResolver with
    /// the Managed Extensibility Framework (MEF). This class is self-configuring and will be
    /// enabled by simply being present in the application's Bin directory. Most applications
    /// should not need to access this class.
    /// </summary>
    public static class CompositionProvider
    {
        private static CompositionHost _container;
        private static ExportFactory<CompositionContext> _requestScopeFactory;
        private static IList<Assembly> _partAssemblies = new List<Assembly>();

        /// <summary>
        /// Used to override the default conventions for controller/part dependency injection.
        /// Cannot be used in conjunction with any other methods on this type. Most applications
        /// should not use this method.
        /// </summary>
        /// <param name="configuration">A configuration containing the controller types and other parts that
        /// should be used by the composition provider.</param>
        public static void SetConfiguration(ContainerConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException("catalog");
            if (IsInitialized) throw new InvalidOperationException("Already initialized.");

            // We add RSF with no conventions (overriding anything set as the default in configuration)
            _container = configuration.CreateContainer();

            var factoryContract = new CompositionContract(typeof(ExportFactory<CompositionContext>), null, new Dictionary<string, object> {
                { "SharingBoundaryNames", new[] { Boundaries.HttpRequest, Boundaries.DataConsistency, Boundaries.UserIdentity }}
            });

            _requestScopeFactory = (ExportFactory<CompositionContext>)_container.GetExport(factoryContract);

            ConfigureMvc();
            ConfigureWebApi();
        }

        static void ConfigureWebApi()
        {
            System.Web.Http.GlobalConfiguration.Configuration.DependencyResolver = new CompositionScopeHttpDependencyResolver();
        }

        static void ConfigureMvc()
        {
            if (DependencyResolver.Current.GetType().Name != "DefaultDependencyResolver")
                throw new CompositionFailedException("The Composition Provider for ASP.NET MVC sets the application-wide DependencyResolver. It seems that another dependency resolver has already been set.");

            DependencyResolver.SetResolver(new CompositionScopeDependencyResolver());

            FilterProviders.Providers.Remove(FilterProviders.Providers.OfType<FilterAttributeFilterProvider>().SingleOrDefault());
            FilterProviders.Providers.Add(new CompositionScopeFilterAttributeFilterProvider());

            ModelBinderProviders.BinderProviders.Add(new CompositionScopeModelBinderProvider());
        }

        internal static CompositionContext Current
        {
            get
            {
                var current = CurrentInitialisedScope;
                if (current == null)
                {
                    current = _requestScopeFactory.CreateExport();
                    CurrentInitialisedScope = current;
                }
                return current.Value;
            }
        }

        internal static Export<CompositionContext> CurrentInitialisedScope
        {
            get { return (Export<CompositionContext>)HttpContext.Current.Items[typeof(CompositionProvider)]; }
            private set { HttpContext.Current.Items[typeof(CompositionProvider)] = value; }
        }

        /// <summary>
        /// Adds assemblies containing MEF parts to the composition provider.
        /// Part types should be in namespaces that include a ".Parts" element in
        /// their name in order to be discovered and used.
        /// </summary>
        /// <param name="assemblies">Assemblies containing parts to add.</param>
        public static void AddAssemblies(params Assembly[] assemblies)
        {
            AddAssemblies((IEnumerable<Assembly>)assemblies);
        }

        /// <summary>
        /// Adds assemblies containing MEF parts to the composition provider.
        /// Part types should be in namespaces that include a ".Parts" element in
        /// their name in order to be discovered and used.
        /// </summary>
        /// <param name="assemblies">Assemblies containing parts to add.</param>
        public static void AddAssemblies(IEnumerable<Assembly> assemblies)
        {
            if (assemblies == null) throw new ArgumentException("assemblies");

            foreach (var assembly in assemblies)
                AddAssembly(assembly);
        }

        /// <summary>
        /// Adds an assembly containing MEF parts to the composition provider.
        /// Part types should be in namespaces that include a ".Parts" element in
        /// their name in order to be discovered and used.
        /// </summary>
        /// <param name="assembly">An assembly containing parts to add.</param>
        public static void AddAssembly(Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException("assembly");

            _partAssemblies.Add(assembly);
        }

        internal static void PostStartDefaultInitialize()
        {
            if (!IsInitialized)
            {
                System.Diagnostics.Debug.WriteLine("Performing default CompositionProvider initialization.");
                SetConfiguration(new MvcContainerConfiguration(
                    _partAssemblies.Union(new[] { MvcContainerConfiguration.GuessGlobalApplicationAssembly() }).ToArray()));
            }
        }

        static bool IsInitialized
        {
            get { return _requestScopeFactory != null; }
        }
    }
}