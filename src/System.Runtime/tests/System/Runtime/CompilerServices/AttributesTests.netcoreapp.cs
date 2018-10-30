// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Runtime.CompilerServices.Tests
{
    public static partial class AttributesTests
    {
        [Fact]
        public static void IsByRefLikeAttributeTests()
        {
            new IsByRefLikeAttribute();
        }

        [Fact]
        public static void IsReadOnlyAttributeTests()
        {
            new IsReadOnlyAttribute();
        }
    }
}
