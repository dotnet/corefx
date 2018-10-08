// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Reflection.Tests
{
    internal class MockDerivedType : BaseMockType
    {
        public override Type UnderlyingSystemType => null;
        protected override TypeAttributes GetAttributeFlagsImpl() => new TypeAttributes();
    }

    public class TypeDerivedTests
    {
        [Fact]
        public void IsAssignableFrom_NullUnderlyingSystemType()
        {
            var testType = new MockDerivedType();
            Assert.Null(testType.UnderlyingSystemType);
            Assert.True(testType.IsAssignableFrom(testType));

            Type compareType = typeof(int);
            Assert.False(testType.IsAssignableFrom(compareType));
            Assert.False(compareType.IsAssignableFrom(testType));
        }
    }
}
