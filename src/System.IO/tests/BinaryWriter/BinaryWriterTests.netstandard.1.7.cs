// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace System.IO.Tests
{
    public class BinaryWriterTests
    {
        protected virtual Stream CreateStream()
        {
            return new MemoryStream();
        }

        [Fact]
        public void BinaryWriter_CloseTests()
        {
            // Closing multiple times should not throw an exception
            using (Stream memStream = CreateStream())
            using (BinaryWriter binaryWriter = new BinaryWriter(memStream))
            {
                binaryWriter.Close();
                binaryWriter.Close();
                binaryWriter.Close();
            }
        }

        [Fact]
        public void BinaryWriter_CloseTests_Negative()
        {
            using (Stream memStream = CreateStream())
            {
                BinaryWriter binaryWriter = new BinaryWriter(memStream);
                binaryWriter.Close();
                ValidateDisposedExceptions(binaryWriter);
            }
        }

        private void ValidateDisposedExceptions(BinaryWriter binaryWriter)
        {
            Assert.Throws<ObjectDisposedException>(() => binaryWriter.Seek(1, SeekOrigin.Begin));
            Assert.Throws<ObjectDisposedException>(() => binaryWriter.Write(new byte[2], 0, 2));
            Assert.Throws<ObjectDisposedException>(() => binaryWriter.Write(new char[2], 0, 2));
            Assert.Throws<ObjectDisposedException>(() => binaryWriter.Write(true));
            Assert.Throws<ObjectDisposedException>(() => binaryWriter.Write((byte)4));
            Assert.Throws<ObjectDisposedException>(() => binaryWriter.Write(new byte[] { 1, 2 }));
            Assert.Throws<ObjectDisposedException>(() => binaryWriter.Write('a'));
            Assert.Throws<ObjectDisposedException>(() => binaryWriter.Write(new char[] { 'a', 'b' }));
            Assert.Throws<ObjectDisposedException>(() => binaryWriter.Write(5.3));
            Assert.Throws<ObjectDisposedException>(() => binaryWriter.Write((short)3));
            Assert.Throws<ObjectDisposedException>(() => binaryWriter.Write(33));
            Assert.Throws<ObjectDisposedException>(() => binaryWriter.Write((Int64)42));
            Assert.Throws<ObjectDisposedException>(() => binaryWriter.Write((sbyte)4));
            Assert.Throws<ObjectDisposedException>(() => binaryWriter.Write("Hello There"));
            Assert.Throws<ObjectDisposedException>(() => binaryWriter.Write((float)4.3));
            Assert.Throws<ObjectDisposedException>(() => binaryWriter.Write((UInt16)3));
            Assert.Throws<ObjectDisposedException>(() => binaryWriter.Write((uint)4));
            Assert.Throws<ObjectDisposedException>(() => binaryWriter.Write((ulong)5));
            Assert.Throws<ObjectDisposedException>(() => binaryWriter.Write("Bah"));
        }
    }
}
