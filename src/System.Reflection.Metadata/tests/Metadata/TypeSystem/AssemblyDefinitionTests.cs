// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace System.Reflection.Metadata.Tests
{
    public class AssemblyDefinitionTests
    {
        [Fact]
        public void ValidateAssemblyNameForAssemblyDefinition()
        {
            var assemblyItems = new[] {
                new { Assembly = WinRT.Lib, Name = "Lib", Version = new Version(1, 0, 0, 0), ContentType = AssemblyContentType.WindowsRuntime },
                new { Assembly = NetModule.AppCS, Name = "AppCS", Version = new Version(1, 2, 3, 4), ContentType = AssemblyContentType.Default },
                new { Assembly = Namespace.NamespaceTests, Name = "NamespaceTests", Version = new Version(0, 0, 0, 0), ContentType = AssemblyContentType.Default },
                new { Assembly = PortablePdbs.DocumentsDll, Name = "Documents", Version = new Version(0, 0, 0, 0), ContentType = AssemblyContentType.Default }
            };

            foreach (var item in assemblyItems)
            {
                var reader = MetadataReaderTests.GetMetadataReader(item.Assembly, options: MetadataReaderOptions.ApplyWindowsRuntimeProjections);
                
                foreach (var assemblyRefHandle in reader.AssemblyReferences)
                {
                    var assemblyDef = reader.GetAssemblyDefinition();
                    var assemblyRef = reader.GetAssemblyReference(assemblyRefHandle);
                    var assemblyName = assemblyDef.GetAssemblyName();

                    Assert.Equal(item.Version, assemblyName.Version);
                    Assert.Equal(item.Name, assemblyName.Name);
                    Assert.Equal(item.ContentType, assemblyName.ContentType);

                    ValidateDefinitionAssemblyNameDefaults(assemblyName);
                    ValidateDefinitionAssemblyNameAgainst(assemblyName, reader, assemblyDef);

                    Assert.NotEqual(reader.GetString(assemblyRef.Name), assemblyName.Name);
                }
            }
        }

        private void ValidateDefinitionAssemblyNameDefaults(AssemblyName assemblyName)
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

        private void ValidateDefinitionAssemblyNameAgainst(AssemblyName assemblyName, MetadataReader reader, AssemblyDefinition assemblyDef)
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
    }
}
