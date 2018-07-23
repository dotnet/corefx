// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
// The .NET Foundation licenses this file to you under the MIT license.

using Xunit;

namespace System.Runtime.Serialization.Tests
{
    public class ContractNamespaceAttributeTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("contractNamespace")]
        public void Ctor_String(string contractNamespace)
        {
            var attribute = new ContractNamespaceAttribute(contractNamespace);
            Assert.Equal(contractNamespace, attribute.ContractNamespace);
            Assert.Null(attribute.ClrNamespace);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("value")]
        public void ClrNamespace_Set_GetReturnsExpected(string value)
        {
            var attribute = new ContractNamespaceAttribute("contractNamespace") { ClrNamespace = value };
            Assert.Equal(value, attribute.ClrNamespace);
        }
    }
}
