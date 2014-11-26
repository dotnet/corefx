// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// -----------------------------------------------------------------------
// Copyright © Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.Composition.Demos.Web.Mvc
{
    class CompositionScopeDependencyResolver : IDependencyResolver
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
    }
}