// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Reflection
{
    public class PathAssemblyResolver : MetadataAssemblyResolver
    {
        public PathAssemblyResolver(params string[] fileNames)
        {
            return;
        }

        public PathAssemblyResolver(IEnumerable<string> fileNames)
        {
            return;
        }

        public override Assembly Resolve(TypeLoader context, AssemblyName assemblyName)
        {
            return null;
        }
    }
}
