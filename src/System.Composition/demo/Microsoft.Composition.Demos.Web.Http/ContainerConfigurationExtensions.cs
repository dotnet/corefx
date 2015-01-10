// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// -----------------------------------------------------------------------
// Copyright © Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------

using System;
using System.Composition.Hosting;
using System.Composition.Convention;
using System.Reflection;

namespace Microsoft.Composition.Demos.Web.Http
{
    /// <summary>
    /// Adds convenience extension methods for ASP.NET Web API to the <see cref="ContainerConfiguration"/> class.
    /// </summary>
    public static class ContainerConfigurationExtensions
    {
        /// <summary>
        /// Adds API controllers to the container configuration.
        /// </summary>
        /// <param name="containerConfiguration">The container configuration.</param>
        /// <param name="assemblies">Assemblies containing API controllers.</param>
        public static void WithApiControllers(this ContainerConfiguration containerConfiguration, params Assembly[] assemblies)
        {
            throw new NotImplementedException();
        }
    }
}
