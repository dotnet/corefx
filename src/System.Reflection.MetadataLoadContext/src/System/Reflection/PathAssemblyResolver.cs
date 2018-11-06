// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;

namespace System.Reflection
{
    /// <summary>
    /// A MetadataAssemblyResolver that uses paths to each assembly.
    /// Assemblies cannot be duplicated by simlple name.
    /// The assembly's simple name is used as a key to obtain the corresponding directory.
    /// </summary>
    /// <remarks>
    /// The assembly name does not need to include the assembly file extension (.dll). If an extension exists
    /// it is removed.
    /// </remarks>
    /// <exception cref="System.ArgumentException">Thrown when the assembly simple name already exists.</exception>
    public class PathAssemblyResolver : MetadataAssemblyResolver
    {
        private Dictionary<string, string> _fileToPaths = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Initializes a new instance of the cref="System.Reflection.PathAssemblyResolver"> class.
        /// </summary>
        /// <remarks>
        /// This method does not verify that the path or file name exists.
        /// </remarks>
        /// <exception cref="System.ArgumentNullException">Thrown when assemblyPaths is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when a path is invalid or an assembly was specified more than once.</exception>
        public PathAssemblyResolver(IEnumerable<string> assemblyPaths)
        {
            if (assemblyPaths == null)
                throw new ArgumentNullException(nameof(assemblyPaths));

            foreach (string path in assemblyPaths)
            {
                if (string.IsNullOrEmpty(path))
                    throw new ArgumentException(SR.Format(SR.Arg_InvalidPath, path), nameof(assemblyPaths));

                string file = Path.GetFileNameWithoutExtension(path);
                if (file.Length == 0)
                    throw new ArgumentException(SR.Format(SR.Arg_InvalidPath, path), nameof(assemblyPaths));

                _fileToPaths.Add(file, path);
            }
        }

        public override Assembly Resolve(MetadataLoadContext context, AssemblyName assemblyName)
        {
            Assembly assembly = null;
            if (_fileToPaths.TryGetValue(assemblyName.Name, out string assemblyPath))
                assembly = context.LoadFromAssemblyPath(assemblyPath);

            return assembly;
        }
    }
}
