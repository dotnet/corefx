// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

#pragma warning disable 0618

namespace System.Reflection.Tests
{
    public class AssemblyFlagsAttributeTests
    {
        [Theory]
        [InlineData(AssemblyNameFlags.None, 0)]
        [InlineData(AssemblyNameFlags.EnableJITcompileOptimizer, 16384)]
        [InlineData(AssemblyNameFlags.EnableJITcompileTracking, 32768)]
        [InlineData(AssemblyNameFlags.PublicKey, 1)]
        [InlineData(AssemblyNameFlags.Retargetable, 256)]
        [InlineData(int.MinValue, 2147483648)]
        [InlineData((AssemblyNameFlags)(AssemblyNameFlags.None - 1), uint.MaxValue)]
        [InlineData((AssemblyNameFlags)int.MaxValue, 2147483647)]
        public void Ctor_AssemblyNameFlags(AssemblyNameFlags assemblyFlags, uint expectedFlags)
        {
            var attribute = new AssemblyFlagsAttribute(assemblyFlags);
            Assert.Equal(expectedFlags, attribute.Flags);
            Assert.Equal((int)assemblyFlags, attribute.AssemblyFlags);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(16384, 16384)]
        [InlineData(32768, 32768)]
        [InlineData(1, 1)]
        [InlineData(256, 256)]
        [InlineData(uint.MaxValue, -1)]
        public void Ctor_UInt(uint flags, int expectedAssemblyFlags)
        {
            var attribute = new AssemblyFlagsAttribute(flags);
            Assert.Equal(flags, attribute.Flags);
            Assert.Equal(expectedAssemblyFlags, attribute.AssemblyFlags);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(16384, 16384)]
        [InlineData(32768, 32768)]
        [InlineData(1, 1)]
        [InlineData(256, 256)]
        [InlineData(int.MinValue, 2147483648)]
        [InlineData(-1, uint.MaxValue)]
        [InlineData(int.MaxValue, 2147483647)]
        public void Ctor_Int(int assemblyFlags, uint expectedFlags)
        {
            var attribute = new AssemblyFlagsAttribute(assemblyFlags);
            Assert.Equal(expectedFlags, attribute.Flags);
            Assert.Equal(assemblyFlags, attribute.AssemblyFlags);
        }
    }
}

#pragma warning restore 0618
