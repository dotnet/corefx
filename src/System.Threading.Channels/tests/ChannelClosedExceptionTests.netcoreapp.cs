// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Xunit;

namespace System.Threading.Channels.Tests
{
    public partial class ChannelClosedExceptionTests
    {
        [Fact]
        public void Serialization_Roundtrip()
        {
            var s = new MemoryStream();

            var inner = new InvalidOperationException("inner");
            var outer = new ChannelClosedException("outer", inner);

            new BinaryFormatter().Serialize(s, outer);
            s.Position = 0;

            var newOuter = (ChannelClosedException)new BinaryFormatter().Deserialize(s);
            Assert.NotSame(outer, newOuter);
            Assert.Equal(outer.Message, newOuter.Message);

            Assert.NotNull(newOuter.InnerException);
            Assert.NotSame(inner, newOuter.InnerException);
            Assert.Equal(inner.Message, newOuter.InnerException.Message);
        }
    }
}
