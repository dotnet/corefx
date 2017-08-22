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
        public void ValidateAssemblyNameForSingleAssemblyDefinitionExe()
        {
            var reader = MetadataReaderTests.GetMetadataReader(NetModule.AppCS);
            var assemblyDef = reader.GetAssemblyDefinition();
            var assemblyName = assemblyDef.GetAssemblyName();

            Assert.Equal(AssemblyContentType.Default, assemblyName.ContentType);
            Assert.Equal("AppCS", assemblyName.Name);
            Assert.Equal(new Version(1, 2, 3, 4), assemblyName.Version);

            ValidateDefinitionAssemblyNameDefaults(assemblyName);
            ValidateDefinitionAssemblyNameAgainst(assemblyName, reader, assemblyDef);
        }

        [Fact]
        public void ValidateAssemblyNameForSingleAssemblyDefinition()
        {
            var reader = MetadataReaderTests.GetMetadataReader(WinRT.Lib, options: MetadataReaderOptions.ApplyWindowsRuntimeProjections);
            var handle = reader.AssemblyReferences.Skip(3).First();
            var assemblyDef = reader.GetAssemblyDefinition();
            var assemblyRef = reader.GetAssemblyReference(handle);
            var assemblyName = assemblyDef.GetAssemblyName();

            Assert.Equal(new Version(1, 0, 0, 0), assemblyName.Version);
            Assert.Equal("Lib", assemblyName.Name);
            Assert.Equal(AssemblyContentType.WindowsRuntime, assemblyName.ContentType);

            ValidateDefinitionAssemblyNameDefaults(assemblyName);
            ValidateDefinitionAssemblyNameAgainst(assemblyName, reader, assemblyDef);
            ValidateDefinitionAssemblyNameAgainst(assemblyName, reader, assemblyRef);
        }

        [Fact]
        public void ValidateAssemblyNameForMultipleAssemblyDefinitionExe()
        {
            var reader = MetadataReaderTests.GetMetadataReader(NetModule.AppCS);

            foreach (var assemblyRefHandle in reader.AssemblyReferences)
            {
                var assemblyDef = reader.GetAssemblyDefinition();
                var assemblyRef = reader.GetAssemblyReference(assemblyRefHandle);
                var assemblyName = assemblyDef.GetAssemblyName();

                ValidateDefinitionAssemblyNameDefaults(assemblyName);
                ValidateDefinitionAssemblyNameAgainst(assemblyName, reader, assemblyDef);
                ValidateDefinitionAssemblyNameAgainst(assemblyName, reader, assemblyRef);

                Assert.Equal(AssemblyContentType.Default, assemblyName.ContentType);
            }
        }

        public void ValidateDefinitionAssemblyNameDefaults(AssemblyName assemblyName)
        {
            // Culture
            Assert.Null(assemblyName.CultureName);

            // HashAlgorithm
            Assert.Equal(Configuration.Assemblies.AssemblyHashAlgorithm.SHA1, assemblyName.HashAlgorithm);

            // PublicKey
            Assert.Null(assemblyName.GetPublicKey());
            Assert.Null(assemblyName.GetPublicKeyToken());

            // Flags
            Assert.Equal(AssemblyNameFlags.None, assemblyName.Flags);
        }

        public void ValidateDefinitionAssemblyNameAgainst(AssemblyName assemblyName, MetadataReader reader, AssemblyDefinition assemblyDef)
        {
            // Name
            Assert.Equal(reader.GetString(assemblyDef.Name), assemblyName.Name);

            // Version
            Assert.Equal(assemblyDef.Version, assemblyName.Version);

            // HashAlgorithm
            Assert.Equal((Configuration.Assemblies.AssemblyHashAlgorithm)assemblyDef.HashAlgorithm, assemblyName.HashAlgorithm);

            // ContentType
            Assert.Equal((AssemblyContentType)(((int)assemblyDef.Flags & (int)AssemblyFlags.ContentTypeMask) >> 9), assemblyName.ContentType);
        }

        public void ValidateDefinitionAssemblyNameAgainst(AssemblyName assemblyName, MetadataReader reader, AssemblyReference assemblyRef)
        {
            // Name
            Assert.NotEqual(reader.GetString(assemblyRef.Name), assemblyName.Name);

            // Version
            Assert.NotEqual(assemblyRef.Version, assemblyName.Version);
        }
    }
}
