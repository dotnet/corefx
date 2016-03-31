// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
