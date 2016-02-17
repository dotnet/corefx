// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Reflection.Tests
{
    public class ReflectionContextTests
    {
        [Fact]
        public void ReflectionContextTestsGetTypeForObject()
        {
            MyRC rc = new MyRC();
            TypeInfo typeInfo = rc.MapType(typeof(int).GetTypeInfo());

            Assembly asm = rc.MapAssembly(typeof(int).GetTypeInfo().Assembly);

            typeInfo = rc.GetTypeForObject(rc);
            Assert.Equal(typeInfo, typeof(MyRC).GetTypeInfo());
        }
    }

    public class MyRC : ReflectionContext
    {
        public override Assembly MapAssembly(Assembly asm)
        {
            return asm;
        }

        public override TypeInfo MapType(TypeInfo typeInfo)
        {
            return typeInfo;
        }
    }
}
