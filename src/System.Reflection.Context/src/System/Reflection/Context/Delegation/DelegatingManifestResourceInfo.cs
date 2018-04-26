// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.Contracts;

namespace System.Reflection.Context.Delegation
{
    internal class DelegatingManifestResourceInfo : ManifestResourceInfo
	{
        public DelegatingManifestResourceInfo(ManifestResourceInfo resource)
            : base(resource.ReferencedAssembly, resource.FileName, resource.ResourceLocation)
        {
            Contract.Requires(null != resource);

            UnderlyingResource = resource;
        }

        public ManifestResourceInfo UnderlyingResource { get; }
    }
}
