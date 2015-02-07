﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Xunit;

namespace System.IO.Tests
{
    public class UmsReadTests
    {
        [Fact]
        public static void EmptyStreamRead()
        {
            using (var manager = new UmsManager(FileAccess.Read, 0))
            {
                UnmanagedMemoryStream stream = manager.Stream;
                UmsTests.ReadUmsInvariants(stream);

                var position = stream.Position;
                Assert.Equal(manager.Stream.ReadByte(), -1); // end of stream
                Assert.Equal(stream.Position, position);
            }
        }

        [Fact]
        public static void OneByteStreamRead()
        {
            using (var manager = new UmsManager(FileAccess.Read, new byte[] { 100 }))
            {
                UnmanagedMemoryStream stream = manager.Stream;
                UmsTests.ReadUmsInvariants(stream);

                var position = stream.Position;
                Assert.Equal(stream.ReadByte(), 100);
                Assert.Equal(stream.Position, position + 1);

                position = stream.Position;
                Assert.Equal(stream.ReadByte(), -1); // end of stream
                Assert.Equal(stream.Position, position);
            }
        }

        [Fact]
        public static void CannotReadFromWriteStream()
        {
            using (var manager = new UmsManager(FileAccess.Write, 100))
            {
                Stream stream = manager.Stream;
                Assert.Throws<NotSupportedException>(() => stream.ReadByte());
            }
        }

        void ReadToEnd(UmsManager manager)
        {
            Stream stream = manager.Stream;
            if (stream.CanRead)
            {
                byte[] read = ReadAllBytes(stream);
                Assert.Equal(stream.Position, read.Length);
                Assert.True(ArrayHelpers.Comparer<byte>().Equals(read, manager.ToArray()));
            }
            else
            {
                Assert.Throws<NotSupportedException>(() => stream.ReadByte());
            }
        }

        [Fact]
        public static void InvalidReadWrite()
        {
            var length = 1000;
            using (var manager = new UmsManager(FileAccess.Read, length))
            {
                UnmanagedMemoryStream stream = manager.Stream;

                //case#3: call Read with null, ArgumentNullException should be thrown.
                Assert.Throws<ArgumentNullException>(() => stream.Read(null, 0, 3));
                Assert.Throws<ArgumentNullException>(() => stream.Write(null, 0, 7));

                //case#4: call Read with start<0, ArgumentOutOfRangeException should be thrown.
                Assert.Throws<ArgumentOutOfRangeException>(() => stream.Read(new byte[] { }, SByte.MinValue, 9));
                Assert.Throws<ArgumentOutOfRangeException>(() => stream.Write(new byte[] { }, -1, 6));

                //case#5: call Read with count<0, ArgumentOutOfRangeException should be thrown.
                Assert.Throws<ArgumentOutOfRangeException>(() => stream.Read(new byte[] { }, 0, -1));
                Assert.Throws<ArgumentOutOfRangeException>(() => stream.Write(new byte[] { }, 1, -2));

                //case#6: call Read with count > ums.Length-startIndex, ArgumentOutOfRangeException should be thrown.
                Assert.Throws<ArgumentException>(() => stream.Read(new byte[10], 0, 11)); // "Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection."
                Assert.Throws<ArgumentException>(() => stream.Write(new byte[3], 0, 4));

                //case#10: Call Read on a n length stream, (Capacity is implicitly n), position is set to end, call it,  should throw ArgumentException.
                Assert.Throws<ArgumentException>(() => stream.Read(new byte[] { }, 0, 1));
                Assert.Throws<ArgumentException>(() => stream.Write(new byte[] { }, 0, 1));
            }
        }

        public static byte[] ReadAllBytes(Stream stream)
        {
            List<byte> read = new List<byte>();
            while (true)
            {
                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0) { break; }
                read.AddRange(new ArraySegment<byte>(buffer, 0, bytesRead));
            }
            return read.ToArray();
        }
    }
}
