// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Reflection.Tests
{
    public class TypeDerivedTests
    {
        [Fact]
        public void IsAssignableFrom_NullUnderlyingSystemType()
        {
            Type compareType = typeof(TypeDerivedTests);
            var testType = new MockDerivedType();
            Assert.Null(testType.UnderlyingSystemType);

            // Add this test once coreclr has IsAssignableFrom fix
            // Assert.False(testType.IsAssignableFrom(compareType));

            Assert.False(compareType.IsAssignableFrom(testType));
        }
    }
}
