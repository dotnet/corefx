// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;
using Xunit;

namespace System.SpanTests
{
    public static partial class ReadOnlySpanTests
    {
        [Fact]
        public static void ZeroLengthLastIndexOf_Byte()
        {
            ReadOnlySpan<byte> sp = new ReadOnlySpan<byte>(Array.Empty<byte>());
            int idx = sp.LastIndexOf<byte>(0);
            Assert.Equal(-1, idx);
        }

        [Fact]
        public static void DefaultFilledLastIndexOf_Byte()
        {
            for (int length = 0; length <= byte.MaxValue; length++)
            {
                byte[] a = new byte[length];
                ReadOnlySpan<byte> span = new ReadOnlySpan<byte>(a);

                for (int i = 0; i < length; i++)
                {
                    byte target0 = default;
                    int idx = span.LastIndexOf(target0);
                    Assert.Equal(length - 1, idx);
                }
            }
        }

        [Fact]
        public static void TestMatchLastIndexOf_Byte()
        {
            for (int length = 0; length <= byte.MaxValue; length++)
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
                    int idx = span.LastIndexOf(target);
                    Assert.Equal(targetIndex, idx);
                }
            }
        }

        [Fact]
        public static void TestNoMatchLastIndexOf_Byte()
        {
            var rnd = new Random(42);
            for (int length = 0; length <= byte.MaxValue; length++)
            {
                byte[] a = new byte[length];
                byte target = (byte)rnd.Next(0, 256);
                for (int i = 0; i < length; i++)
                {
                    byte val = (byte)(i + 1);
                    a[i] = val == target ? (byte)(target + 1) : val;
                }
                ReadOnlySpan<byte> span = new ReadOnlySpan<byte>(a);

                int idx = span.LastIndexOf(target);
                Assert.Equal(-1, idx);
            }
        }

        [Fact]
        public static void TestAllignmentNoMatchLastIndexOf_Byte()
        {
            byte[] array = new byte[4 * Vector<byte>.Count];
            for (var i = 0; i < Vector<byte>.Count; i++)
            {
                var span = new ReadOnlySpan<byte>(array, i, 3 * Vector<byte>.Count);
                int idx = span.LastIndexOf<byte>(5);
                Assert.Equal(-1, idx);

                span = new ReadOnlySpan<byte>(array, i, 3 * Vector<byte>.Count - 3);
                idx = span.LastIndexOf<byte>(5);
                Assert.Equal(-1, idx);
            }
        }

        [Fact]
        public static void TestAllignmentMatchLastIndexOf_Byte()
        {
            byte[] array = new byte[4 * Vector<byte>.Count];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = 5;
            }
            for (var i = 0; i < Vector<byte>.Count; i++)
            {
                var span = new ReadOnlySpan<byte>(array, i, 3 * Vector<byte>.Count);
                int idx = span.LastIndexOf<byte>(5);
                Assert.Equal(span.Length - 1, idx);

                span = new ReadOnlySpan<byte>(array, i, 3 * Vector<byte>.Count - 3);
                idx = span.LastIndexOf<byte>(5);
                Assert.Equal(span.Length - 1, idx);
            }
        }

        [Fact]
        public static void TestMultipleMatchLastIndexOf_Byte()
        {
            for (int length = 2; length <= byte.MaxValue; length++)
            {
                byte[] a = new byte[length];
                for (int i = 0; i < length; i++)
                {
                    byte val = (byte)(i + 1);
                    a[i] = val == 200 ? (byte)201 : val;
                }

                a[length - 1] = 200;
                a[length - 2] = 200;

                ReadOnlySpan<byte> span = new ReadOnlySpan<byte>(a);
                int idx = span.LastIndexOf<byte>(200);
                Assert.Equal(length - 1, idx);
            }
        }

        [Fact]
        public static void MakeSureNoChecksGoOutOfRangeLastIndexOf_Byte()
        {
            for (int length = 0; length <= byte.MaxValue; length++)
            {
                byte[] a = new byte[length + 2];
                a[0] = 99;
                a[length + 1] = 99;
                ReadOnlySpan<byte> span = new ReadOnlySpan<byte>(a, 1, length);
                int index = span.LastIndexOf<byte>(99);
                Assert.Equal(-1, index);
            }
        }
    }
}
