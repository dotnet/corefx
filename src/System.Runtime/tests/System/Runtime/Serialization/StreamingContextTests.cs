// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Runtime.Serialization.Tests
{
    public class StreamingContextTests
    {
        [Fact]
        public void Ctor_Default()
        {
            var context = new StreamingContext();
            Assert.Equal((StreamingContextStates)0, context.State);
            Assert.Null(context.Context);
        }

        [Theory]
        [InlineData(StreamingContextStates.All)]
        [InlineData((StreamingContextStates)0)]
        [InlineData((StreamingContextStates)(-1))]
        public void Ctor_StreamingContextStates(StreamingContextStates state)
        {
            var context = new StreamingContext(state);
            Assert.Equal(state, context.State);
            Assert.Null(context.Context);
        }

        [Theory]
        [InlineData(StreamingContextStates.All, null)]
        [InlineData((StreamingContextStates)0, "")]
        [InlineData((StreamingContextStates)(-1), "context")]
        public void Ctor_StreamingContextStates_Object(StreamingContextStates state, object additional)
        {
            var context = new StreamingContext(state, additional);
            Assert.Equal(state, context.State);
            Assert.Equal(additional, context.Context);
        }

        public static IEnumerable<object[]> Equals_TestData()
        {
            yield return new object[] { new StreamingContext(StreamingContextStates.All, null), new StreamingContext(StreamingContextStates.All, null), true };
            yield return new object[] { new StreamingContext(StreamingContextStates.All, null), new StreamingContext(StreamingContextStates.Clone, null), false };
            yield return new object[] { new StreamingContext(StreamingContextStates.All, null), new StreamingContext(StreamingContextStates.All, "additional"), false };

            yield return new object[] { new StreamingContext(StreamingContextStates.All, null), new object(), false };
            yield return new object[] { new StreamingContext(StreamingContextStates.All, null), null, false };
        }

        [Theory]
        [MemberData(nameof(Equals_TestData))]
        public void Equals_Invoke_ReturnsExpected(StreamingContext context, object other, bool expected)
        {
            Assert.Equal(expected, context.Equals(other));
        }

        [Fact]
        public void GetHashCode_Invoke_ReturnsState()
        {
            var context = new StreamingContext((StreamingContextStates)10);
            Assert.Equal(10, context.GetHashCode());
        }
    }
}