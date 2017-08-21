// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using Xunit;

namespace System.Reflection.Metadata.Tests
{
    public class AssemblyDefinitionTests
    {
        [Fact]
        public void ValidateAssemblyNameForAssemblyDefinitionExe()
        {
            var reader = MetadataReaderTests.GetMetadataReader(NetModule.AppCS);

            AssemblyDefinition assemblyDefinition = reader.GetAssemblyDefinition();
            AssemblyName assemblyName = assemblyDefinition.GetAssemblyName();

            // Name
            Assert.Equal(reader.GetString(assemblyDefinition.Name), assemblyName.Name);
            Assert.Equal("AppCS", assemblyName.Name);

            // Flags
            Assert.Equal(AssemblyNameFlags.None, assemblyName.Flags);
            Assert.Equal((uint)0, (uint)assemblyName.Flags); // AssemblyFlags

            // Locale
            Assert.Null(assemblyName.CultureName);

            // PublicKey
            Assert.Null(assemblyName.GetPublicKey());

            // Version
            Assert.Equal(assemblyDefinition.Version, assemblyName.Version);
            Assert.Equal(new Version(1, 2, 3, 4), assemblyName.Version);
            
            // HashAlgorithm
            Assert.Equal(Configuration.Assemblies.AssemblyHashAlgorithm.SHA1, assemblyName.HashAlgorithm);
            Assert.Equal((Configuration.Assemblies.AssemblyHashAlgorithm)assemblyDefinition.HashAlgorithm, assemblyName.HashAlgorithm);
        }
    }
}
