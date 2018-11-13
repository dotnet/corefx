// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Reflection.Tests
{
    /// <summary>
    /// Resolves the core assembly by creating a simulated core assembly and other assemblies by match
    /// on Name, Version, PublicKeyToken and CultureName.
    /// </summary>
    public class SimpleAssemblyResolver : CoreMetadataAssemblyResolver
    {
        private static readonly Version s_Version0000 = new Version(0, 0, 0, 0);
        private static readonly byte[] s_EmptyArray = Array.Empty<byte>();

        public SimpleAssemblyResolver() { }

        public override Assembly Resolve(MetadataLoadContext context, AssemblyName assemblyName)
        {
            Assembly core = base.Resolve(context, assemblyName);
            if (core != null)
                return core;

            foreach(Assembly a in context.GetAssemblies())
            {
                if (assemblyName.Name.Equals(a.GetName().Name, StringComparison.OrdinalIgnoreCase) &&
                    NormalizeVersion(assemblyName.Version).Equals(NormalizeVersion(a.GetName().Version)) &&
                    NormalizePublicKeyToken(assemblyName.GetPublicKeyToken()).SequenceEqual(NormalizePublicKeyToken(a.GetName().GetPublicKeyToken())) &&
                    NormalizeCultureName(assemblyName.CultureName).Equals(NormalizeCultureName(a.GetName().CultureName)))
                    return a;
            }

            return null;
        }

        private ReadOnlySpan<byte> NormalizePublicKeyToken(ReadOnlySpan<byte> publicKeyToken)
        {
            if (publicKeyToken == null)
                return s_EmptyArray;

            return publicKeyToken;
        }

        private Version NormalizeVersion(Version v)
        {
            if (v == null)
                return s_Version0000;

            return v;
        }

        private string NormalizeCultureName(string cultureName)
        {
            if (cultureName == null)
                return string.Empty;

            return cultureName;
        }
    }
}
