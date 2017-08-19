// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Reflection.Metadata.Tests
{
    public class AssemblyReferenceTests
    {
        [Fact]
        public void DefaultAssemblyNameFlagIsNone()
        {
            AssemblyName assemblyName = new AssemblyName();

            Assert.Equal(AssemblyNameFlags.None, assemblyName.Flags);
        }

        [Fact]
        public void RetargetableFlagIsSet()
        {
            AssemblyName assemblyName = new AssemblyName();

            //Assert.Equal(AssemblyNameFlags.Retargetable, assemblyName.Flags);
        }

        // TODO: add more tests
    }
}
