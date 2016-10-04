// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Runtime.Versioning
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Constructor, Inherited = false)]
    [Conditional("RESOURCE_ANNOTATION_WORK")]
    public sealed class ResourceConsumptionAttribute : Attribute
    {
        private ResourceScope _consumptionScope;
        private ResourceScope _resourceScope;

        public ResourceConsumptionAttribute(ResourceScope resourceScope)
        {
            _resourceScope = resourceScope;
            _consumptionScope = _resourceScope;
        }

        public ResourceConsumptionAttribute(ResourceScope resourceScope, ResourceScope consumptionScope)
        {
            _resourceScope = resourceScope;
            _consumptionScope = consumptionScope;
        }

        public ResourceScope ResourceScope {
            get { return _resourceScope; }
        }

        public ResourceScope ConsumptionScope {
            get { return _consumptionScope; }
        }
    }
}
