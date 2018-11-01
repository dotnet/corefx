// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace System.Reflection
{
    /// <summary>
    /// A MetadataAssemblyResolver that uses paths to each assembly.
    /// </summary>
    /// <remarks>
    /// If the corresponding MetadataLoadContext does not have the CoreAssemblyName property set,
    /// default values are provided which are compatible with .NET Core and .NET Framework.
    /// </remarks>
    public class PathAssemblyResolver : MetadataAssemblyResolver
    {
        private Dictionary<string, string> _fileToPaths = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public PathAssemblyResolver(IEnumerable<string> assemblyPaths)
        {
            if (assemblyPaths == null)
                throw new ArgumentNullException(nameof(assemblyPaths));

            foreach (string path in assemblyPaths)
            {
                string file = Path.GetFileNameWithoutExtension(path);
                _fileToPaths.Add(file, path);
            }
        }

        public override Assembly Resolve(MetadataLoadContext context, AssemblyName assemblyName)
        {
            Debug.Assert(context != null);
            Debug.Assert(assemblyName != null);

            Assembly assembly = null;
            if (_fileToPaths.TryGetValue(assemblyName.Name, out string assemblyPath))
            {
                assembly = context.LoadFromAssemblyPath(assemblyPath);
            }

            return assembly;
        }
    }
}
