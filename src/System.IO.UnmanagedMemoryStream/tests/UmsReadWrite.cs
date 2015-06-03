﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit;

namespace System.IO.Tests
{
    public class UmsReadWriteTests
    {
        // TODO: add tests for different offsets and lengths
        [Fact]
        public static void ReadWrite()
        {
            const int length = 1000;
            using (var manager = new UmsManager(FileAccess.ReadWrite, length))
            {
                UnmanagedMemoryStream stream = manager.Stream;
                UmsTests.ReadWriteUmsInvariants(stream);
                Assert.Equal(stream.Length, length);

                var bytes = ArrayHelpers.CreateByteArray(length);
                var copy = bytes.Copy();
                stream.Write(copy, 0, length);

                var memory = manager.ToArray();
                Assert.Equal(bytes, memory, ArrayHelpers.Comparer<byte>());

                stream.Seek(0, SeekOrigin.Begin);
                byte[] read = UmsReadTests.ReadAllBytes(stream);
                Assert.Equal(stream.Position, read.Length);

                byte[] current = manager.ToArray();
                Assert.Equal(current, read, ArrayHelpers.Comparer<byte>());
                Assert.Equal(bytes, read, ArrayHelpers.Comparer<byte>());

                stream.Write(new byte[0], 0, 0);
            }
        }

        [Fact]
        public static void ReadWriteByte()
        {
            const int length = 1000;
            using (var manager = new UmsManager(FileAccess.ReadWrite, length))
            {
                UnmanagedMemoryStream stream = manager.Stream;
                UmsTests.ReadWriteUmsInvariants(stream);

                var bytes = ArrayHelpers.CreateByteArray(length);

                for (int index = 0; index < bytes.Length; index++)
                {
                    byte byteToWrite = bytes[index];
                    stream.WriteByte(byteToWrite);
                    stream.Position--;
                    int read = stream.ReadByte();
                    Assert.Equal((byte)read, byteToWrite);
                }

                var memory = manager.ToArray();

                Assert.Equal(bytes, memory, ArrayHelpers.Comparer<byte>());
            }
        }
    }
}
