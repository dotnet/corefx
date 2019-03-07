// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Configuration.Assemblies;
using Xunit;

namespace System.Reflection.Tests
{
    public class AssemblyAlgorithmIdAttributeTests
    {
        [Theory]
        [InlineData((Configuration.Assemblies.AssemblyHashAlgorithm)(Configuration.Assemblies.AssemblyHashAlgorithm.None - 1), 4294967295)]
        [InlineData(Configuration.Assemblies.AssemblyHashAlgorithm.MD5, 32771)]
        [InlineData(Configuration.Assemblies.AssemblyHashAlgorithm.SHA1, 32772)]
        [InlineData(Configuration.Assemblies.AssemblyHashAlgorithm.SHA256, 32780)]
        [InlineData(Configuration.Assemblies.AssemblyHashAlgorithm.SHA384, 32781)]
        [InlineData(Configuration.Assemblies.AssemblyHashAlgorithm.SHA512, 32782)]
        public void Ctor_AssemblyHashAlgorithm(Configuration.Assemblies.AssemblyHashAlgorithm algorithmId, uint expectedAlgorithmId)
        {
            var attribute = new AssemblyAlgorithmIdAttribute(algorithmId);
            Assert.Equal(expectedAlgorithmId, attribute.AlgorithmId);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(32771)]
        [InlineData(32772)]
        [InlineData(32780)]
        [InlineData(32781)]
        [InlineData(32782)]
        [InlineData(uint.MaxValue)]
        public void Ctor_UInt(uint algorithmId)
        {
            var attribute = new AssemblyAlgorithmIdAttribute(algorithmId);
            Assert.Equal(algorithmId, attribute.AlgorithmId);
        }
    }
}
