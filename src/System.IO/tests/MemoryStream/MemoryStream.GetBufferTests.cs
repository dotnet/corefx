﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.IO.Tests
{
    public class MemoryStream_GetBufferTests
    {
        [Fact]
        public void MemoryStream_GetBuffer_Length()
        {
            MemoryStream ms = new MemoryStream();
            byte[] buffer = ms.GetBuffer();
            Assert.Equal(0, buffer.Length);
        }

        [Fact]
        public void MemoryStream_GetBuffer_NonExposable()
        {
            MemoryStream ms = new MemoryStream(new byte[100]);
            Assert.Throws<UnauthorizedAccessException>(() => ms.GetBuffer());
        }

        [Fact]
        public void MemoryStream_GetBuffer_Exposable()
        {
            MemoryStream ms = new MemoryStream(new byte[500], 0, 100, true, true);
            byte[] buffer = ms.GetBuffer();
            Assert.Equal(500, buffer.Length);
        }

        [Fact]
        public void MemoryStream_GetBuffer()
        {
            byte[] testdata = new byte[100];
            new Random(45135).NextBytes(testdata);
            MemoryStream ms = new MemoryStream(100);
            byte[] buffer = ms.GetBuffer();
            Assert.Equal(100, buffer.Length);

            ms.Write(testdata, 0, 100);
            ms.Write(testdata, 0, 100);
            Assert.Equal(200, ms.Length);
            buffer = ms.GetBuffer();
            Assert.Equal(256, buffer.Length); // Minimun size after writing
        }
    }
}
