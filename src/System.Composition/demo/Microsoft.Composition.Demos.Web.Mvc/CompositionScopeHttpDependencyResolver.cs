// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// -----------------------------------------------------------------------
// Copyright © Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http.Dependencies;

namespace Microsoft.Composition.Demos.Web.Mvc
{
    class CompositionScopeHttpDependencyResolver : IDependencyResolver
    {
        public object GetService(Type serviceType)
        {
            object export;
            if (!CompositionProvider.Current.TryGetExport(serviceType, null, out export))
                return null;

            return export;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return CompositionProvider.Current.GetExports(serviceType);
        }

        public IDependencyScope BeginScope()
        {
            return this;
        }

        public void Dispose()
        {
        }
    }
}
