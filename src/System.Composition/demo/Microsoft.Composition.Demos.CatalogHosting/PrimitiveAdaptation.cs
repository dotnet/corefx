// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Composition.Demos.CatalogHosting
{
    static class PrimitiveAdaptation
    {
        public static string ParseTypeIdentity(string typeIdentity)
        {
            var numberOfCommas = typeIdentity.Count(c => c == ',');
            var numberOfGenericArgs = numberOfCommas + 1;
            return typeIdentity.Replace("(", string.Format("`{0}[", numberOfGenericArgs)).Replace(")", "]");
        }

        public static Tuple<bool, string> DefaultCreationPolicyMapping(CreationPolicy creationPolicy)
        {
            if (creationPolicy == CreationPolicy.NonShared)
                return Tuple.Create(false, (string)null);

            return Tuple.Create(true, (string)null);
        }

        public static Type DefaultTypeNameResolver(string typeName)
        {
            return AssemblyTypeNameResolver(typeName, AppDomain.CurrentDomain.GetAssemblies());
        }

        public static Type AssemblyTypeNameResolver(string typeName, IEnumerable<Assembly> assemblies)
        {
            return assemblies.Select(a => a.GetType(typeName)).SingleOrDefault(a => a != null);
        }
    }
}
