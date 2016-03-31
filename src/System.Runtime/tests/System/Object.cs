// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Xunit;

public static unsafe class ObjectTests
{
    [Fact]
    public static void TestEqualsAndHashCode()
    {
        object o1 = new object();
        int h1 = o1.GetHashCode();
        int h2 = o1.GetHashCode();
        Assert.Equal(h1, h2);

        bool b;

        b = o1.Equals(o1);
        Assert.True(b);
        b = o1.Equals(null);
        Assert.False(b);

        object o2 = new object();
        b = o1.Equals(o2);
        Assert.False(b);

        b = Object.Equals(o1, o1);
        Assert.True(b);

        b = Object.Equals(null, null);
        Assert.True(b);

        b = Object.Equals(o1, null);
        Assert.False(b);

        b = Object.Equals(null, o1);
        Assert.False(b);

        b = Object.Equals(o1, o2);
        Assert.False(b);

        EOverrider e1 = new EOverrider(7);
        EOverrider e2 = new EOverrider(8);

        EOverrider.s_EqualsCalled = false;
        b = Object.Equals(e1, e2);
        Assert.True(EOverrider.s_EqualsCalled);
        Assert.False(b);

        EOverrider.s_EqualsCalled = false;
        b = Object.ReferenceEquals(e1, e2);
        Assert.False(EOverrider.s_EqualsCalled);
        Assert.False(b);

        EOverrider.s_EqualsCalled = false;
        b = Object.ReferenceEquals(e1, e1);
        Assert.False(EOverrider.s_EqualsCalled);
        Assert.True(b);

        EOverrider.s_EqualsCalled = false;
        b = Object.ReferenceEquals(e1, e1);
        Assert.False(EOverrider.s_EqualsCalled);
        Assert.True(b);

        EOverrider.s_EqualsCalled = false;
        b = Object.ReferenceEquals(e1, null);
        Assert.False(EOverrider.s_EqualsCalled);
        Assert.False(b);

        EOverrider.s_EqualsCalled = false;
        b = Object.ReferenceEquals(null, e1);
        Assert.False(EOverrider.s_EqualsCalled);
        Assert.False(b);
    }

    [Fact]
    public static void TestGetType()
    {
        object o1 = new object();
        object o2 = new object();
        Type t1 = o1.GetType();
        Type t2 = o2.GetType();
        Assert.Equal(t1, typeof(object));
        Assert.Equal(t1, t2);
        Assert.Equal(t1.ToString(), o1.ToString());

        C c = new C("Hello", 7, 9);
        Type t3 = c.GetType();
        Assert.Equal(t3, typeof(C));

        Generic<string> l = new Generic<string>();
        Type t4 = l.GetType();
        Assert.Equal(t4, typeof(Generic<string>));

        int[] i = new int[3];
        Type t5 = i.GetType();
        Assert.Equal(t5, typeof(int[]));
    }

    private class Generic<T>
    {
    }

    [Fact]
    public static void TestToString()
    {
        object o = new object();
        string s = o.ToString();
        Assert.Equal(s, "System.Object");
        string s1 = o.GetType().ToString();
        Assert.Equal(s, s1);
    }

    [Fact]
    public static void TestMemberwiseClone()
    {
        C c1 = new C("Hello", 7, 8);
        C c2 = c1.CallMemberwiseClone();
        Assert.Equal(c2.s, "Hello");
        Assert.Equal(c2.x, 7);
        Assert.Equal(c2.y, 8);
    }

    private class C
    {
        public C(string s, int x, int y)
        {
            this.s = s;
            this.x = x;
            this.y = y;
        }
        public string s;
        public int x;
        public int y;

        public C CallMemberwiseClone()
        {
            return (C)(this.MemberwiseClone());
        }
    }

    private class EOverrider
    {
        public EOverrider(int x)
        {
            X = x;
        }

        public override bool Equals(object obj)
        {
            s_EqualsCalled = true;

            EOverrider eo = obj as EOverrider;
            if (eo == null)
                return false;
            return eo.X == this.X;
        }

        public override int GetHashCode()
        {
            return 42;
        }

        public int X;

        public static bool s_EqualsCalled = false;
    }
}
