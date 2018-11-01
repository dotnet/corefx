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
        private static readonly string[] CoreNames = { "mscorlib", "System.Private.CoreLib", "System.Runtime", "netstandard" };

        private Dictionary<string, string> _fileToPaths = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public PathAssemblyResolver(IEnumerable<string> assemblyPath)
        {
            if (assemblyPath == null)
                throw new ArgumentNullException(nameof(assemblyPath));

            foreach (string path in assemblyPath)
            {
                string file = Path.GetFileNameWithoutExtension(path);
                _fileToPaths.Add(file, path);
            }
        }

        public override Assembly Resolve(MetadataLoadContext context, AssemblyName assemblyName)
        {
            string assemblyPath = null;

            if (assemblyName != null)
            {
                _fileToPaths.TryGetValue(assemblyName.Name, out assemblyPath);
                if (assemblyPath != null)
                {
                    return context.LoadFromAssemblyPath(assemblyPath);
                }
            }
            else
            {
                Debug.Assert(context.CoreAssemblyName == null);

                // Try loading the first core assembly that has a path specified
                foreach (string coreName in CoreNames)
                {
                    if (_fileToPaths.TryGetValue(coreName, out assemblyPath))
                    {
                        Assembly assembly = context.LoadFromAssemblyPath(assemblyPath);
                        if (assembly != null)
                        {
                            context.CoreAssemblyName = coreName;
                        }

                        return assembly;
                    }
                }
            }

            return null;
        }
    }
}
