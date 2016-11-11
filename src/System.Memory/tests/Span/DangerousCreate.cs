// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace System.SpanTests
{
    public static partial class SpanTests
    {
        [Fact]
        public static void DangerousCreateNullObject()
        {
            Assert.Throws<ArgumentNullException>(
                delegate ()
                {
                    int dummy = 4;
                    Span<int>.DangerousCreate(null, ref dummy, 0);
                });
        }

        [Fact]
        public static void DangerousCreateNoArrays()
        {
            Assert.Throws<ArgumentException>(
                delegate ()
                {
                    int[] a = new int[4];
                    Span<int>.DangerousCreate(a, ref a[0], 0);
                });
        }

        [Fact]
        public static void DangerousCreateNoStrings()
        {
            Assert.Throws<ArgumentException>(
                delegate ()
                {
                    char dummy = 'A';
                    Span<char>.DangerousCreate("Hello", ref dummy, 0);
                });
        }


        [Fact]
        public static void DangerousCreateBadLength()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                delegate ()
                {
                    TestClass testClass = new TestClass();
                    Span<char> span = Span<char>.DangerousCreate(testClass, ref testClass.C1, -1);
                });
        }

        [Fact]
        public static void DangerousCreate1()
        {
            TestClass testClass = new TestClass();
            testClass.C0 = 'a';
            testClass.C1 = 'b';
            testClass.C2 = 'c';
            testClass.C3 = 'd';
            testClass.C4 = 'e';
            Span<char> span = Span<char>.DangerousCreate(testClass, ref testClass.C1, 3);
            span.Validate<char>('b', 'c', 'd');

            ref char pc1 = ref span[0];
            Assert.True(Unsafe.AreSame<char>(ref testClass.C1, ref pc1));
        }
    }
}
