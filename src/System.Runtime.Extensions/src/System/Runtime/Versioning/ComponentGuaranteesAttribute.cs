// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Runtime.Versioning
{
 
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | 
                    AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Delegate |
                    AttributeTargets.Enum | AttributeTargets.Method | AttributeTargets.Property |
                    AttributeTargets.Constructor | AttributeTargets.Event, 
                    AllowMultiple = false, Inherited = false)]
    public sealed class ComponentGuaranteesAttribute : Attribute
    {
        private ComponentGuaranteesOptions _guarantees;

        public ComponentGuaranteesAttribute(ComponentGuaranteesOptions guarantees)
        {
            _guarantees = guarantees;
        }

        public ComponentGuaranteesOptions Guarantees {
            get { return _guarantees; }
        }
    }
}
