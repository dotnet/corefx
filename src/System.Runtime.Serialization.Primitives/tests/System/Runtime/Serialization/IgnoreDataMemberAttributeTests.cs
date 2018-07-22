// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Runtime.Serialization.Tests
{
    public class IgnoreDataMemberAttributeTests
    {
        [Fact]
        public void Ctor_Default_Success()
        {
            new IgnoreDataMemberAttribute();
        }
    }
}
