// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

using Xunit;

namespace System.Runtime.Tests
{
    public static class ObjectTests
    {
        public static IEnumerable<object[]> EqualsTestData()
        {
            var o1 = new object();
            var o2 = new object();

            yield return new object[] { o1, o1, true };
            yield return new object[] { o1, null, false };
            yield return new object[] { o1, o2, false };

            yield return new object[] { null, null, true };
            yield return new object[] { null, o1, false };
        }

        [Theory, MemberData("EqualsTestData")]
        public static void TestEquals(object o1, object o2, bool expected)
        {
            if (o1 != null)
            {
                Assert.Equal(expected, o1.Equals(o2));
                Assert.True(o1.Equals(o1));
                Assert.Equal(o1.GetHashCode(), o1.GetHashCode());
                if (o2 != null)
                {
                    Assert.Equal(expected, o1.GetHashCode().Equals(o2.GetHashCode()));
                }
            }
            Assert.Equal(expected, Equals(o1, o2));
        }

        [Fact]
        public static void TestReferenceEquals()
        {
            var e1 = new EOverrider(7);
            var e2 = new EOverrider(8);

            EOverrider.s_EqualsCalled = false;
            Assert.True(Equals(e1, e1));
            Assert.False(EOverrider.s_EqualsCalled);

            // ReferenceEquals should not call Equals
            EOverrider.s_EqualsCalled = false;
            Assert.False(ReferenceEquals(e1, e2));
            Assert.False(EOverrider.s_EqualsCalled);

            EOverrider.s_EqualsCalled = false;
            Assert.True(ReferenceEquals(e1, e1));
            Assert.False(EOverrider.s_EqualsCalled);

            EOverrider.s_EqualsCalled = false;
            Assert.True(ReferenceEquals(e1, e1));
            Assert.False(EOverrider.s_EqualsCalled);

            EOverrider.s_EqualsCalled = false;
            Assert.False(ReferenceEquals(e1, null));
            Assert.False(EOverrider.s_EqualsCalled);

            EOverrider.s_EqualsCalled = false;
            Assert.False(ReferenceEquals(null, e1));
            Assert.False(EOverrider.s_EqualsCalled);
        }

        public static IEnumerable<object[]> GetTypeTestData()
        {
            yield return new object[] { new object(), typeof(object) };
            yield return new object[] { new C("Hello", 7, 9), typeof(C) };
            yield return new object[] { new Generic<string>(), typeof(Generic<string>) };
            yield return new object[] { new int[3], typeof(int[]) };
        }

        [Theory, MemberData("GetTypeTestData")]
        public static void TestGetType(object obj, Type expected)
        {
            Assert.Equal(expected, obj.GetType());
        }
        
        [Fact]
        public static void TestToString()
        {
            var o = new object();
            Assert.Equal("System.Object", o.ToString());
            Assert.Equal(o.ToString(), o.GetType().ToString());
        }

        [Fact]
        public static void TestMemberwiseClone()
        {
            var c1 = new C("Hello", 7, 8);
            var c2 = c1.CallMemberwiseClone();
            Assert.Equal("Hello", c2.s);
            Assert.Equal(7, c2.x);
            Assert.Equal(8, c2.y);
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
                return (C)MemberwiseClone();
            }
        }

        private class Generic<T>
        {
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
                return eo.X == X;
            }

            public override int GetHashCode()
            {
                return 42;
            }

            public int X;

            public static bool s_EqualsCalled = false;
        }
    }
}
