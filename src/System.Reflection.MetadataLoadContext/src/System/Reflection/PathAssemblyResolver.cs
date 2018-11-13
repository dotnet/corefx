// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace System.Reflection
{
    /// <summary>
    /// An assembly resolver that uses paths to every assembly that may be loaded.
    /// The file name is expected to be the same as the assembly's simple name.
    /// Multiple assemblies can exist on disk with the same name but in different directories.
    /// </summary>
    /// <remarks>
    /// In order for an AssemblyName to match to a loaded assembly, AssemblyName.Name must be equal (casing ignored).
    /// - If AssemblyName.PublicKeyToken is specified, it must be equal.
    /// - If AssemblyName.PublicKeyToken is not specified, the first assembly matching by Name will be returned.
    /// - If there is more than one assembly with the same Name and PublicKeyToken, the assembly with the highest Version is returned.
    /// - CultureName is ignored.
    /// </remarks>
    public class PathAssemblyResolver : MetadataAssemblyResolver
    {
        private static readonly Version s_Version0000 = new Version(0, 0, 0, 0);
        private static readonly byte[] s_EmptyArray = Array.Empty<byte>();

        private ConcurrentDictionary<string, List<string>> _fileToPaths = new ConcurrentDictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Initializes a new instance of the cref="System.Reflection.PathAssemblyResolver"> class.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Thrown when assemblyPaths is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when a path is invalid or an Assembly simple name was specified more than once.</exception>
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

                List<string> paths;
                if (!_fileToPaths.TryGetValue(file, out paths))
                {
                    _fileToPaths.TryAdd(file, paths = new List<string>());
                }
                paths.Add(path);
            }
        }

        public override Assembly Resolve(MetadataLoadContext context, AssemblyName assemblyName)
        {
            // On the first access to this assembly name, load each assembly from the provided path(s).
            LoadMatchingAssembliesFromPaths(context, assemblyName.Name);

            Assembly candidate = null;

            foreach (Assembly a in context.GetAssemblies())
            {
                if (assemblyName.Name.Equals(a.GetName().Name, StringComparison.OrdinalIgnoreCase) && IsPublicKeyTokenCompatible(assemblyName, a))
                {
                    // Pick the highest version.
                    if (candidate == null || NormalizeVersion(a.GetName().Version) > NormalizeVersion(candidate.GetName().Version))
                    {
                        candidate = a;
                    }
                }
            }

            return candidate;
        }

        private void LoadMatchingAssembliesFromPaths(MetadataLoadContext context, string simpleName)
        {
            List<string> paths;
            if (_fileToPaths.TryGetValue(simpleName, out paths))
            {
                foreach (string path in paths)
                {
                    context.LoadFromAssemblyPath(path);
                }

                // Remove the path so we don't process it again.
                _fileToPaths.TryRemove(simpleName, out paths);
            }
        }

        private bool IsPublicKeyTokenCompatible(AssemblyName name, Assembly a)
        {
            if (name.GetPublicKeyToken() == null)
                return true;

            if (a.GetName().GetPublicKeyToken() == null)
                return ((ReadOnlySpan<byte>)name.GetPublicKeyToken()).SequenceEqual(s_EmptyArray);

            if (((ReadOnlySpan<byte>)name.GetPublicKeyToken()).SequenceEqual(s_EmptyArray))
                return true;

            return ((ReadOnlySpan<byte>)name.GetPublicKeyToken()).SequenceEqual(a.GetName().GetPublicKeyToken());
        }

        private Version NormalizeVersion(Version v)
        {
            if (v == null)
                return s_Version0000;

            return v;
        }
    }
}
