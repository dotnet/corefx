// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Runtime.Tests
{
    public static class MulticastDelegateTests
    {
        [Fact]
        public static void TestGetInvocationList()
        {
            DFoo dfoo = new C().Foo;
            Delegate[] delegates = dfoo.GetInvocationList();
            Assert.NotNull(delegates);
            Assert.Equal(delegates.Length, 1);
            Assert.True(dfoo.Equals(delegates[0]));
        }

        [Fact]
        public static void TestEquals()
        {
            var c = new C();
            DFoo d1 = c.Foo;
            DFoo d2 = c.Foo;
            Assert.False(ReferenceEquals(d1, d2));
            Assert.True(d1.Equals(d2));
            Assert.True(d1 == d2);
            Assert.False(d1 != d2);

            d1 = c.Foo;
            d2 = c.Goo;
            Assert.False(d1.Equals(d2));
            Assert.False(d1 == d2);
            Assert.True(d1 != d2);

            d1 = new C().Foo;
            d2 = new C().Foo;
            Assert.False(d1.Equals(d2));
            Assert.False(d1 == d2);
            Assert.True(d1 != d2);

            DGoo dgoo = c.Foo;
            d1 = c.Foo;
            Assert.False(d1.Equals(dgoo));
            
            Assert.Equal(d1.GetHashCode(), d1.GetHashCode());
        }

        [Fact]
        public static void TestCombineReturn()
        {
            var tracker = new Tracker();
            var dret1 = new DRet(tracker.ARet);
            var dret2 = new DRet(tracker.BRet);
            var dret12 = (DRet)Delegate.Combine(dret1, dret2);
            string s = dret12(4);
            Assert.Equal("BRet4", s);
            Assert.Equal("ARet4BRet4", tracker.S);
        }

        [Fact]
        public static void TestCombine()
        {
            var tracker = new Tracker();
            var a = new D(tracker.A);
            var b = new D(tracker.B);
            var c = new D(tracker.C);
            var d = new D(tracker.D);

            D nothing = (D)(Delegate.Combine());
            Assert.Null(nothing);

            D one = (D)(Delegate.Combine(a));
            tracker.Clear();
            one(5);
            Assert.Equal("A5", tracker.S);
            CheckInvokeList(new D[] { a }, one, tracker);

            D ab = (D)(Delegate.Combine(a, b));
            tracker.Clear();
            ab(5);
            Assert.Equal("A5B5", tracker.S);
            CheckInvokeList(new D[] { a, b }, ab, tracker);

            D abc = (D)(Delegate.Combine(a, b, c));
            tracker.Clear();
            abc(5);
            Assert.Equal("A5B5C5", tracker.S);
            CheckInvokeList(new D[] { a, b, c }, abc, tracker);

            D abcdabc = (D)(Delegate.Combine(abc, d, abc));
            tracker.Clear();
            abcdabc(9);
            Assert.Equal("A9B9C9D9A9B9C9", tracker.S);
            CheckInvokeList(new D[] { a, b, c, d, a, b, c }, abcdabc, tracker);
        }

        [Fact]
        public static void TestRemove()
        {
            var tracker = new Tracker();
            var a = new D(tracker.A);
            var b = new D(tracker.B);
            var c = new D(tracker.C);
            var d = new D(tracker.D);
            var e = new D(tracker.E);

            D abc = (D)(Delegate.Combine(a, b, c));
            D abcdabc = (D)(Delegate.Combine(abc, d, abc));
            D ab = (D)(Delegate.Combine(a, b));
            D dab = (D)(Delegate.Combine(d, ab));
            D abcc = (D)(Delegate.Remove(abcdabc, dab));
            tracker.Clear();
            abcc(9);
            Assert.Equal("A9B9C9C9", tracker.S);
            CheckInvokeList(new D[] { a, b, c, c }, abcc, tracker);

            // Pattern-match is based on structural equivalence, not reference equality.
            D acopy = new D(tracker.A);
            D bbba = (D)(Delegate.Combine(b, b, b, acopy));
            D bbb = (D)(Delegate.Remove(bbba, a));
            tracker.Clear();
            bbb(9);
            Assert.Equal("B9B9B9", tracker.S);
            CheckInvokeList(new D[] { b, b, b }, bbb, tracker);

            // In the case of multiple occurrences, Remove() must remove the last one.
            D abcd = (D)(Delegate.Remove(abcdabc, abc));
            tracker.Clear();
            abcd(9);
            Assert.Equal("A9B9C9D9", tracker.S);
            CheckInvokeList(new D[] { a, b, c, d }, abcd, tracker);

            D d1 = (D)(Delegate.RemoveAll(abcdabc, abc));
            tracker.Clear();
            d1(9);
            Assert.Equal("D9", tracker.S);
            CheckInvokeList(new D[] { d }, d1, tracker);

            D nothing = (D)(Delegate.Remove(d1, d1));
            Assert.Null(nothing);

            // The pattern-not-found case.
            D abcd1 = (D)(Delegate.Remove(abcd, null));
            tracker.Clear();
            abcd1(9);
            Assert.Equal("A9B9C9D9", tracker.S);
            CheckInvokeList(new D[] { a, b, c, d }, abcd1, tracker);

            // The pattern-not-found case.
            D abcd2 = (D)(Delegate.Remove(abcd, e));
            tracker.Clear();
            abcd2(9);
            Assert.Equal("A9B9C9D9", tracker.S);
            CheckInvokeList(new D[] { a, b, c, d }, abcd2, tracker);

            // The pattern-not-found case.
            D abcd3 = (D)(Delegate.RemoveAll(abcd, null));
            tracker.Clear();
            abcd3(9);
            Assert.Equal("A9B9C9D9", tracker.S);
            CheckInvokeList(new D[] { a, b, c, d }, abcd3, tracker);

            // The pattern-not-found case.
            D abcd4 = (D)(Delegate.RemoveAll(abcd, e));
            tracker.Clear();
            abcd4(9);
            Assert.Equal("A9B9C9D9", tracker.S);
            CheckInvokeList(new D[] { a, b, c, d }, abcd4, tracker);
        }

        private static void CheckInvokeList(D[] expected, D combo, Tracker target)
        {
            Delegate[] invokeList = combo.GetInvocationList();
            Assert.Equal(invokeList.Length, expected.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                CheckIsSingletonDelegate(expected[i], (D)(invokeList[i]), target);
            }
            Assert.True(ReferenceEquals(combo.Target, expected[expected.Length - 1].Target));
            Assert.True(ReferenceEquals(combo.Target, target));
        }

        private static void CheckIsSingletonDelegate(D expected, D actual, Tracker target)
        {
            Assert.True(expected.Equals(actual));
            Delegate[] invokeList = actual.GetInvocationList();
            Assert.Equal(invokeList.Length, 1);
            Assert.True(actual.Equals(invokeList[0]));

            target.Clear();
            expected(9);
            string sExpected = target.S;

            target.Clear();
            actual(9);
            Assert.Equal(sExpected, target.S);

            Assert.True(ReferenceEquals(target, actual.Target));
        }

        private class Tracker
        {
            public Tracker()
            {
                S = "";
            }

            public string S;

            public void Clear()
            {
                S = "";
            }

            public void A(int x)
            {
                S = S + "A" + x;
            }

            public string ARet(int x)
            {
                S = S + "ARet" + x;
                return "ARet" + x;
            }

            public void B(int x)
            {
                S = S + "B" + x;
            }

            public string BRet(int x)
            {
                S = S + "BRet" + x;
                return "BRet" + x;
            }

            public void C(int x)
            {
                S = S + "C" + x;
            }

            public void D(int x)
            {
                S = S + "D" + x;
            }

            public void E(int x)
            {
                S = S + "E" + x;
            }

            public void F(int x)
            {
                S = S + "F" + x;
            }
        }

        private delegate void D(int x);
        private delegate string DRet(int x);

        private delegate string DFoo(int x);
        private delegate string DGoo(int x);

        private class C
        {
            public string Foo(int x)
            {
                return new string('A', x);
            }

            public string Goo(int x)
            {
                return new string('A', x);
            }
        }
    }
}
