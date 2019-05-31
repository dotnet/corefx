// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Tests
{
    public class CLSCompliantAttributeTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Ctor_Bool(bool isCompliant)
        {
            var attribute = new CLSCompliantAttribute(isCompliant);
            Assert.Equal(isCompliant, attribute.IsCompliant);
        }
    }
}
