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
            Assert.Null(assemblyName.GetPublicKeyToken());

            // Version
            Assert.Equal(assemblyDefinition.Version, assemblyName.Version);
            Assert.Equal(new Version(1, 2, 3, 4), assemblyName.Version);
            
            // HashAlgorithm
            Assert.Equal(Configuration.Assemblies.AssemblyHashAlgorithm.SHA1, assemblyName.HashAlgorithm);
            Assert.Equal((Configuration.Assemblies.AssemblyHashAlgorithm)assemblyDefinition.HashAlgorithm, assemblyName.HashAlgorithm);
        }

        [Fact]
        public void ValidateAssemblyNameForSingleAssemblyDefinition()
        {
            var reader = MetadataReaderTests.GetMetadataReader(WinRT.Lib, options: MetadataReaderOptions.ApplyWindowsRuntimeProjections);
            var handle = reader.AssemblyReferences.Skip(3).First();
            var assemblyDef = reader.GetAssemblyDefinition();
            var assemblyRef = reader.GetAssemblyReference(handle);

            var assemblyName = assemblyDef.GetAssemblyName();

            // Name
            Assert.Equal("Lib", assemblyName.Name);
            Assert.Equal(reader.GetString(assemblyDef.Name), assemblyName.Name);
            Assert.NotEqual(reader.GetString(assemblyRef.Name), assemblyName.Name);

            // Version
            Assert.Equal(new Version(1, 0, 0, 0), assemblyName.Version);
            Assert.Equal(assemblyDef.Version, assemblyName.Version);
            Assert.NotEqual(assemblyRef.Version, assemblyName.Version);

            // Culture
            Assert.Null(assemblyName.CultureName);

            // PublicKey
            //Assert.Equal(new byte[] { 0xB0, 0x3F, 0x5F, 0x7F, 0x11, 0xD5, 0x0A, 0x3A }, assemblyName.GetPublicKey());
            Assert.Null(assemblyName.GetPublicKey());
            Assert.Null(assemblyName.GetPublicKeyToken());
            //Assert.Equal(reader.GetBlobBytes(assemblyRef.PublicKeyOrToken), assemblyName.GetPublicKey());
            Assert.Null(assemblyName.GetPublicKey());
            Assert.Null(assemblyName.GetPublicKeyToken());


            // HashAlgorithm
            Assert.Equal(Configuration.Assemblies.AssemblyHashAlgorithm.SHA1, assemblyName.HashAlgorithm);
            Assert.Equal((Configuration.Assemblies.AssemblyHashAlgorithm)assemblyDef.HashAlgorithm, assemblyName.HashAlgorithm);

            // Flags
            Assert.Equal(AssemblyNameFlags.None, assemblyName.Flags);

            // ContentType
            Assert.Equal(AssemblyContentType.WindowsRuntime, assemblyName.ContentType);
            Assert.Equal((AssemblyContentType)(((int)assemblyDef.Flags & 0x0E00) >> 9), assemblyName.ContentType);
        }

        [Fact]
        public void ValidateAssemblyNameForMultipleAssemblyDefinition()
        {
            byte[][] expKeys = new byte[][]
            {
                new byte[] { 0xb7, 0x7a, 0x5c, 0x56, 0x19, 0x34, 0xe0, 0x89 },
                new byte[] { 0xb7, 0x7a, 0x5c, 0x56, 0x19, 0x34, 0xe0, 0x89 },
                new byte[] { 0xb7, 0x7a, 0x5c, 0x56, 0x19, 0x34, 0xe0, 0x89 },

                // VB: B0 3F 5F 7F 11 D5 0A 3A
                new byte[] { 0xb0, 0x3f, 0x5f, 0x7f, 0x11, 0xd5, 0x0a, 0x3a }
            };
            var expVers = new Version[]
            {
                new Version(4, 0, 0, 0),
                new Version(4, 0, 0, 0),
                new Version(4, 0, 0, 0),
                new Version(/*VB*/10, 0, 0, 0),
            };

            var reader = MetadataReaderTests.GetMetadataReader(NetModule.AppCS);

            int i = 0;
            foreach (var assemblyRefHandle in reader.AssemblyReferences)
            {
                var assemblyDef = reader.GetAssemblyDefinition();
                var assemblyName = assemblyDef.GetAssemblyName();

                // Name
                Assert.Equal(reader.GetString(assemblyDef.Name), assemblyName.Name);

                // Version
                Assert.Equal(assemblyDef.Version, assemblyName.Version);

                // Culture
                Assert.Null(assemblyName.CultureName);

                // PublicKey
                //Assert.Equal(expKeys[i], assemblyName.GetPublicKey());
                Assert.Null(assemblyName.GetPublicKey());
                Assert.Null(assemblyName.GetPublicKeyToken());
                //Assert.Equal(reader.GetBlobBytes(assemblyRef.PublicKeyOrToken), assemblyName.GetPublicKey());
                Assert.Null(assemblyName.GetPublicKey());
                Assert.Null(assemblyName.GetPublicKeyToken());

                // HashAlgorithm
                Assert.Equal(Configuration.Assemblies.AssemblyHashAlgorithm.SHA1, assemblyName.HashAlgorithm);
                Assert.Equal((Configuration.Assemblies.AssemblyHashAlgorithm)assemblyDef.HashAlgorithm, assemblyName.HashAlgorithm);

                // Flags
                Assert.Equal((uint)0, (uint)assemblyName.Flags);
                Assert.Equal(AssemblyNameFlags.None, assemblyName.Flags);

                // ContentType
                Assert.Equal(AssemblyContentType.Default, assemblyName.ContentType);

                i++;
            }
        }
    }
}
