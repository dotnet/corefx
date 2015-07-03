// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

using System.Reflection;
using System.Reflection.Emit;
using Xunit;

namespace System.Reflection.Emit.Tests
{
    public class AssemblyBuilderGetManifestResourceNames
    {
        [Fact]
        public void TestThrowsExceptionOnDynamicAssembly()
        {
            AssemblyName name = new AssemblyName("NegTest1Assembly");
            AssemblyBuilder builder = AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
            Assert.Throws<NotSupportedException>(() => { string[] myName = builder.GetManifestResourceNames(); });
        }
    }
}
