// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

using Xunit;

public static unsafe class UIntPtrTests
{
    [Fact]
    public static unsafe void TestBasics()
    {
        UIntPtr p;
        uint i;
        ulong l;

        if (sizeof(void*) == 4)
        {
            // Skip UIntPtr tests on 32-bit platforms
            return;
        }

        int size = UIntPtr.Size;
        Assert.Equal(size, sizeof(void*));

        TestPointer(UIntPtr.Zero, 0);

        i = 42;
        TestPointer(new UIntPtr(i), i);
        TestPointer((UIntPtr)i, i);

        i = 42;
        TestPointer(new UIntPtr(i), i);

        l = 0x0fffffffffffffff;
        TestPointer(new UIntPtr(l), l);
        TestPointer((UIntPtr)l, l);

        void* pv = new UIntPtr(42).ToPointer();
        TestPointer(new UIntPtr(pv), 42);
        TestPointer((UIntPtr)pv, 42);

        p = UIntPtr.Add(new UIntPtr(42), 5);
        TestPointer(p, 42 + 5);

        // Add is spected NOT to generate an OverflowException
        p = UIntPtr.Add(new UIntPtr(0xffffffffffffffff), 5);
        unchecked
        {
            TestPointer(p, (long)0x0000000000000004);
        }

        p = UIntPtr.Subtract(new UIntPtr(42), 5);
        TestPointer(p, 42 - 5);

        bool b;
        p = new UIntPtr(42);
        b = p.Equals(null);
        Assert.False(b);
        b = p.Equals((object)42);
        Assert.False(b);
        b = p.Equals((object)(new UIntPtr(42)));
        Assert.True(b);

        int h = p.GetHashCode();
        int h2 = p.GetHashCode();
        Assert.Equal(h, h2);

        p = new UIntPtr(42);
        i = (uint)p;
        Assert.Equal(i, 42u);
        l = (ulong)p;
        Assert.Equal(l, 42u);
        UIntPtr p2;
        p2 = (UIntPtr)i;
        Assert.Equal(p, p2);
        p2 = (UIntPtr)l;
        Assert.Equal(p, p2);
        p2 = (UIntPtr)(p.ToPointer());
        Assert.Equal(p, p2);
        p2 = new UIntPtr(40) + 2;
        Assert.Equal(p, p2);
        p2 = new UIntPtr(44) - 2;
        Assert.Equal(p, p2);

        p = new UIntPtr(0x7fffffffffffffff);
        Assert.Throws<OverflowException>(() => (uint)p);
    }

    private static void TestPointer(UIntPtr p, ulong expected)
    {
        ulong l = p.ToUInt64();
        Assert.Equal(expected, l);

        uint expected32 = (uint)expected;
        if (expected32 != expected)
        {
            Assert.Throws<OverflowException>(() => p.ToUInt32());
            return;
        }

        {
            uint i = p.ToUInt32();
            Assert.Equal(expected32, i);
        }

        string s = p.ToString();
        string sExpected = expected.ToString();
        Assert.Equal(s, sExpected);

        Assert.True(p == new UIntPtr(expected));
        Assert.Equal(p, new UIntPtr(expected));
        Assert.False(p == new UIntPtr(expected + 1));
        Assert.NotEqual(p, new UIntPtr(expected + 1));
        Assert.False(p != new UIntPtr(expected));
        Assert.True(p != new UIntPtr(expected + 1));
    }
}
