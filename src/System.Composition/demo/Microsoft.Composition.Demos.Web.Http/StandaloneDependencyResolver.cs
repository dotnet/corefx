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
using System.Web.Http.Dependencies;

namespace Microsoft.Composition.Demos.Web.Http
{
    /// <summary>
    /// A depdendency resolver for ASP.NET WebAPI that is independent of the
    /// rest of the web stack.
    /// </summary>
    public class StandaloneDependencyResolver : StandaloneDependencyScope, IDependencyResolver
    {
        private readonly ExportFactory<CompositionContext> _requestScopeFactory;

        /// <summary>
        /// Construct a <see cref="StandaloneDependencyResolver"/> for the provided
        /// root composition scope.
        /// </summary>
        /// <param name="rootCompositionScope">The scope to provide application-level services to
        /// the program.</param>
        public StandaloneDependencyResolver(CompositionHost rootCompositionScope)
            : base(new Export<CompositionContext>(rootCompositionScope, rootCompositionScope.Dispose))
        {
            if (rootCompositionScope == null) throw new ArgumentNullException();
            var factoryContract = new CompositionContract(typeof(ExportFactory<CompositionContext>), null, new Dictionary<string, object> {
                { "SharingBoundaryNames", new[] { Boundaries.HttpRequest, Boundaries.DataConsistency, Boundaries.UserIdentity }}
            });

            _requestScopeFactory = (ExportFactory<CompositionContext>)rootCompositionScope.GetExport(factoryContract);
        }

        /// <summary>
        /// Create a new request-specific scope.
        /// </summary>
        /// <returns>A new scope.</returns>
        public IDependencyScope BeginScope()
        {
            return new StandaloneDependencyScope(_requestScopeFactory.CreateExport());
        }
    }
}
