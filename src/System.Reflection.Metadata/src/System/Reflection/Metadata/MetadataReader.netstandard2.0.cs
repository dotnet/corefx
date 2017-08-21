// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Reflection.Metadata
{
    public sealed partial class MetadataReader
    {
        internal AssemblyName GetAssemblyName(StringHandle nameHandle, Version version, StringHandle cultureHandle, BlobHandle publicKeyHandle, AssemblyHashAlgorithm assemblyHashAlgorithm, AssemblyFlags flags)
        {
            string name = (!nameHandle.IsNil) ? GetString(nameHandle) : null;
            string cultureName = (!cultureHandle.IsNil) ? GetString(cultureHandle) : null;
            var hashAlgorithm = (Configuration.Assemblies.AssemblyHashAlgorithm)assemblyHashAlgorithm;
            byte[] publicKeyOrToken = !publicKeyHandle.IsNil ? GetBlobBytes(publicKeyHandle) : null;

            var assemblyName = new AssemblyName(name)
            {
                Version = version,
                CultureName = cultureName,
                HashAlgorithm = hashAlgorithm
            };

            assemblyName.SetPublicKey(publicKeyOrToken);
            assemblyName.Flags = GetAssemblyNameFlags(flags);
            assemblyName.ContentType = GetContentTypeFromAssemblyFlags(flags);

            return assemblyName;
        }

        private AssemblyNameFlags GetAssemblyNameFlags(AssemblyFlags flags)
        {
            AssemblyNameFlags assemblyNameFlags = AssemblyNameFlags.None;

            if ((flags & AssemblyFlags.PublicKey) != 0)
                assemblyNameFlags |= AssemblyNameFlags.PublicKey;
            else
                assemblyNameFlags &= ~AssemblyNameFlags.PublicKey;

            if ((flags & AssemblyFlags.Retargetable) != 0)
                assemblyNameFlags |= AssemblyNameFlags.Retargetable;
            else
                assemblyNameFlags &= ~AssemblyNameFlags.Retargetable;

            if ((flags & AssemblyFlags.EnableJitCompileTracking) != 0)
                assemblyNameFlags |= AssemblyNameFlags.EnableJITcompileTracking;
            else
                assemblyNameFlags &= ~AssemblyNameFlags.EnableJITcompileTracking;

            if ((flags & AssemblyFlags.DisableJitCompileOptimizer) != 0)
                assemblyNameFlags |= AssemblyNameFlags.EnableJITcompileOptimizer;
            else
                assemblyNameFlags &= ~AssemblyNameFlags.EnableJITcompileOptimizer;

            return assemblyNameFlags;
        }

        private AssemblyContentType GetContentTypeFromAssemblyFlags(AssemblyFlags flags)
        {
            return (AssemblyContentType)(((int)flags & 0x0E00) >> 9);
        }
    }
}
