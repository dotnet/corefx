// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Drawing.Imaging.Tests
{
    public class FrameDimensionTests
    {
        [Fact]
        public void Ctor_Guid()
        {
            var guid = Guid.NewGuid();
            FrameDimension fd = new FrameDimension(guid);
            Assert.Equal(guid, fd.Guid);
        }

        [Fact]
        public void DefinedFrameDimensions_ReturnsExpected()
        {
            Assert.Equal(new Guid("{6aedbd6d-3fb5-418a-83a6-7f45229dc872}"), FrameDimension.Time.Guid);
            Assert.Equal(new Guid("{84236f7b-3bd3-428f-8dab-4ea1439ca315}"), FrameDimension.Resolution.Guid);
            Assert.Equal(new Guid("{7462dc86-6180-4c7e-8e3f-ee7333a7a483}"), FrameDimension.Page.Guid);
        }

        [Fact]
        public void Equals_Object_ReturnsExpected()
        {
            var guid = Guid.NewGuid();
            FrameDimension fd = new FrameDimension(guid);
            Assert.True(fd.Equals(new FrameDimension(guid)));
            Assert.False(fd.Equals(null));
            Assert.False(fd.Equals(new object()));
            Assert.False(fd.Equals(new FrameDimension(Guid.NewGuid())));
            Assert.Equal(guid.GetHashCode(), fd.GetHashCode());
        }
    }
}
