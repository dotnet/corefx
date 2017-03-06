// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.SpanTests
{
    public static partial class ReadOnlySpanTests
    {
        [Fact]
        public static void ZeroLengthIndexOf_Byte()
        {
            ReadOnlySpan<byte> sp = new ReadOnlySpan<byte>(Array.Empty<byte>());
            int idx = sp.IndexOf(0);
            Assert.Equal(-1, idx);
        }

        [Fact]
        public static void TestMatch_Byte()
        {
            for (int length = 0; length < 32; length++)
            {
                byte[] a = new byte[length];
                for (int i = 0; i < length; i++)
                {
                    a[i] = (byte)(i + 1);
                }
                ReadOnlySpan<byte> span = new ReadOnlySpan<byte>(a);
                
                for (int targetIndex = 0; targetIndex < length; targetIndex++)
                {
                    byte target = a[targetIndex];
                    int idx = span.IndexOf(target);
                    Assert.Equal(targetIndex, idx);
                }
            }
        }

        [Fact]
        public static void TestMultipleMatch_Byte()
        {
            for (int length = 2; length < 32; length++)
            {
                byte[] a = new byte[length];
                for (int i = 0; i < length; i++)
                {
                    a[i] = (byte)(i + 1);
                }

                a[length - 1] = 200;
                a[length - 2] = 200;

                ReadOnlySpan<byte> span = new ReadOnlySpan<byte>(a);
                int idx = span.IndexOf(200);
                Assert.Equal(length - 2, idx);
            }
        }

        [Fact]
        public static void MakeSureNoChecksGoOutOfRange_Byte()
        {
            for (int length = 0; length < 100; length++)
            {
                byte[] a = new byte[length + 2];
                a[0] = 99;
                a[length + 1] = 99;
                ReadOnlySpan<byte> span = new ReadOnlySpan<byte>(a, 1, length);
                int index = span.IndexOf(99);
                Assert.Equal(-1, index);
            }
        }

        [Fact]
        public static void TestMatchWithIndexAndCount_Byte()
        {
            int length = 100;
            byte[] a = new byte[length];
            a[50] = 99;
            ReadOnlySpan<byte> span = new ReadOnlySpan<byte>(a);

            int idx = span.IndexOf(45, 10, 99);
            Assert.Equal(50, idx);
        }

        [Fact]
        public static void TestMatchWithIndexAndCountGreaterThanLength_Byte()
        {
            int length = 100;
            byte[] a = new byte[length];
            a[50] = 99;
            ReadOnlySpan<byte> span = new ReadOnlySpan<byte>(a);

            int idx = span.IndexOf(45, 75, 99);
            Assert.Equal(50, idx);
        }

        [Fact]
        public static void TestMatchWithCountGreaterThanLength_Byte()
        {
            int length = 100;
            byte[] a = new byte[length];
            a[50] = 99;
            ReadOnlySpan<byte> span = new ReadOnlySpan<byte>(a);

            int idx = span.IndexOf(0, 105, 99);
            Assert.Equal(50, idx);
        }

        [Fact]
        public static void TestNoMatchWithCountGreaterThanLength_Byte()
        {
            int length = 100;
            byte[] a = new byte[length];
            a[50] = 99;
            ReadOnlySpan<byte> span = new ReadOnlySpan<byte>(a);

            int idx = span.IndexOf(0, 105, 5);
            Assert.Equal(-1, idx);
        }

        [Fact]
        public static void TestNoMatchWithIndexAndCount_Byte()
        {
            int length = 100;
            byte[] a = new byte[length];
            a[50] = 99;
            ReadOnlySpan<byte> span = new ReadOnlySpan<byte>(a);

            int idx = span.IndexOf(45, 3, 99);
            Assert.Equal(-1, idx);
        }

        [Fact]
        public static void StartIndexTooLargeIndexOf_Byte()
        {
            int length = 100;
            byte[] a = new byte[length];
            a[50] = 99;
            ReadOnlySpan<byte> span = new ReadOnlySpan<byte>(a);
            int idx = span.IndexOf(length + 1, 10, 99);
            Assert.Equal(-1, idx);
        }

        public static void ZeroCountIndexOf_Byte()
        {
            int length = 100;
            byte[] a = new byte[length];
            a[50] = 99;
            ReadOnlySpan<byte> span = new ReadOnlySpan<byte>(a);
            int idx = span.IndexOf(0, 0, 99);
            Assert.Equal(-1, idx);
        }
    }
}
