﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.DirectoryServices.Protocols.Tests
{
    public class DirSyncRequestControlTests
    {
        [Fact]
        public void Ctor_Default()
        {
            var control = new DirSyncRequestControl();
            Assert.Equal(1048576, control.AttributeCount);
            Assert.Empty(control.Cookie);
            Assert.Equal(DirectorySynchronizationOptions.None, control.Option);

            Assert.True(control.IsCritical);
            Assert.True(control.ServerSide);
            Assert.Equal("1.2.840.113556.1.4.841", control.Type);

            Assert.Equal(new byte[] { 48, 132, 0, 0, 0, 10, 2, 1, 0, 2, 3, 16, 0, 0, 4, 0 }, control.GetValue());
        }

        [Theory]
        [InlineData(null, new byte[] { 48, 132, 0, 0, 0, 10, 2, 1, 0, 2, 3, 16, 0, 0, 4, 0 })]
        [InlineData(new byte[0], new byte[] { 48, 132, 0, 0, 0, 10, 2, 1, 0, 2, 3, 16, 0, 0, 4, 0 })]
        [InlineData(new byte[] { 97, 98, 99 }, new byte[] { 48, 132, 0, 0, 0, 13, 2, 1, 0, 2, 3, 16, 0, 0, 4, 3, 97, 98, 99 })]
        public void Ctor_Cookie(byte[] cookie, byte[] expectedValue)
        {
            var control = new DirSyncRequestControl(cookie);
            Assert.Equal(1048576, control.AttributeCount);
            Assert.Equal(cookie ?? Array.Empty<byte>(), control.Cookie);
            Assert.Equal(DirectorySynchronizationOptions.None, control.Option);

            Assert.True(control.IsCritical);
            Assert.True(control.ServerSide);
            Assert.Equal("1.2.840.113556.1.4.841", control.Type);

            Assert.Equal(expectedValue, control.GetValue());
        }

        [Theory]
        [InlineData(null, DirectorySynchronizationOptions.None, new byte[] { 48, 132, 0, 0, 0, 10, 2, 1, 0, 2, 3, 16, 0, 0, 4, 0 })]
        [InlineData(new byte[0], DirectorySynchronizationOptions.None - 1, new byte[] { 48, 132, 0, 0, 0, 13, 2, 4, 255, 255, 255, 255, 2, 3, 16, 0, 0, 4, 0 })]
        [InlineData(new byte[] { 97, 98, 99 }, DirectorySynchronizationOptions.ObjectSecurity, new byte[] { 48, 132, 0, 0, 0, 13, 2, 1, 1, 2, 3, 16, 0, 0, 4, 3, 97, 98, 99 })]
        public void Ctor_Cookie_Options(byte[] cookie, DirectorySynchronizationOptions option, byte[] expectedValue)
        {
            var control = new DirSyncRequestControl(cookie, option);
            Assert.Equal(1048576, control.AttributeCount);
            Assert.Equal(cookie ?? Array.Empty<byte>(), control.Cookie);
            Assert.Equal(option, control.Option);

            Assert.True(control.IsCritical);
            Assert.True(control.ServerSide);
            Assert.Equal("1.2.840.113556.1.4.841", control.Type);
            
            Assert.Equal(expectedValue, control.GetValue());
        }

        [Theory]
        [InlineData(null, DirectorySynchronizationOptions.None, 1048576, new byte[] { 48, 132, 0, 0, 0, 10, 2, 1, 0, 2, 3, 16, 0, 0, 4, 0 })]
        [InlineData(new byte[0], DirectorySynchronizationOptions.None - 1, 0, new byte[] { 48, 132, 0, 0, 0, 11, 2, 4, 255, 255, 255, 255, 2, 1, 0, 4, 0 })]
        [InlineData(new byte[] { 97, 98, 99 }, DirectorySynchronizationOptions.ObjectSecurity, 10, new byte[] { 48, 132, 0, 0, 0, 11, 2, 1, 1, 2, 1, 10, 4, 3, 97, 98, 99 })]
        public void Ctor_Cookie_Options_AttributeCount(byte[] cookie, DirectorySynchronizationOptions option, int attributeCount , byte[] expectedValue)
        {
            var control = new DirSyncRequestControl(cookie, option, attributeCount);
            Assert.Equal(attributeCount, control.AttributeCount);
            Assert.Equal(cookie ?? Array.Empty<byte>(), control.Cookie);
            Assert.Equal(option, control.Option);

            Assert.True(control.IsCritical);
            Assert.True(control.ServerSide);
            Assert.Equal("1.2.840.113556.1.4.841", control.Type);

            Assert.Equal(expectedValue, control.GetValue());
        }

        [Fact]
        public void Ctor_NegativeAttributeCount_ThrowsArgumentException()
        {
            AssertExtensions.Throws<ArgumentException>("value", () => new DirSyncRequestControl(new byte[0], DirectorySynchronizationOptions.None, -1));
        }

        [Fact]
        public void AttributeCount_SetValid_GetReturnsExpected()
        {
            var control = new DirSyncRequestControl { AttributeCount = 0 };
            Assert.Equal(0, control.AttributeCount);
        }

        [Fact]
        public void AttributeCount_SetNegative_ThrowsArgumentException()
        {
            var control = new DirSyncRequestControl();
            AssertExtensions.Throws<ArgumentException>("value", () => control.AttributeCount = -1);
        }

        [Fact]
        public void Cookie_Set_GetReturnsExpected()
        {
            byte[] cookie = new byte[] { 1, 2, 3 };
            var control = new DirSyncRequestControl { Cookie = cookie };
            Assert.NotSame(cookie, control.Cookie);
            Assert.Equal(cookie, control.Cookie);
        }
    }
}
