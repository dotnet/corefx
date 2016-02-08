// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Runtime.Tests
{
    public static class DelegateTests
    {
        [Fact]
        public static void TestClosedStaticDelegate()
        {
            var foo = new TestClass();
            foo.structField.o1 = new object();
            foo.structField.o2 = new object();
            StructReturningDelegate testDelegate = foo.TestFunc;
            TestStruct returnedStruct = testDelegate();
            Assert.True(ReferenceEquals(foo.structField.o1, returnedStruct.o1));
            Assert.True(ReferenceEquals(foo.structField.o2, returnedStruct.o2));
        }
        
        [Fact]
        public static void TestDynamicInvoke()
        {
            var a1 = new A();
            var a2 = new A();
            var b1 = new B();
            var b2 = new B();

            DynamicInvokeDelegate testDelegate = DynamicInvokeTestFunction;

            // Check that the delegate behaves as expected
            A refParam = b2;
            B outParam = null;
            A returnValue = testDelegate(a1, b1, ref refParam, out outParam);
            Assert.True(ReferenceEquals(returnValue, a1));
            Assert.True(ReferenceEquals(refParam, b1));
            Assert.True(ReferenceEquals(outParam, b2));

            // Check dynamic invoke behavior
            var parameters = new object[] { a1, b1, b2, null };

            object retVal = testDelegate.DynamicInvoke(parameters);
            Assert.True(ReferenceEquals(retVal, a1));
            Assert.True(ReferenceEquals(parameters[2], b1));
            Assert.True(ReferenceEquals(parameters[3], b2));

            // Check invoke on a delegate that takes no parameters.
            Action emptyDelegate = EmptyFunc;
            emptyDelegate.DynamicInvoke(new object[] { });
            emptyDelegate.DynamicInvoke(null);
        }

        [Fact]
        public static void TestDynamicInvoke_MethodWithDefaultValues()
        {
            // Passing Type.Missing without providing default.
            Delegate d = new DFoo1(Foo1);
            Assert.Throws<ArgumentException>("parameters", () => d.DynamicInvoke(7, Type.Missing));

            // Passing Type.Missing with default.
            d = new DFoo1WithDefault(Foo1);
            d.DynamicInvoke(7, Type.Missing);
        }

        [Fact]
        public static void TestDynamicInvoke_MethodWithRefParameter()
        {
            Delegate d = new DFoo2(Foo2);
            var args = new object[] { 7 };
            d.DynamicInvoke(args);
            Assert.Equal(args[0], 8);

            d = new DFoo2(Foo2);
            args = new object[] { null };
            d.DynamicInvoke(args);
            Assert.Equal(args[0], 1);

            // For "byref ValueType" arguments, the incoming is allowed to be null. The target will receive default(ValueType).
            d = new DFoo3(Foo3);
            args = new object[] { null };
            d.DynamicInvoke(args);
            MyStruct s = (MyStruct)(args[0]);
            Assert.Equal(s.x, 7);
            Assert.Equal(s.y, 8);

            // For "byref ValueType" arguments, the type must match exactly.
            d = new DFoo2(Foo2);
            args = new object[] { (uint)7 };
            Assert.Throws<ArgumentException>(null, () => d.DynamicInvoke(args));

            d = new DFoo2(Foo2);
            args = new object[] { E4.One };
            Assert.Throws<ArgumentException>(null, () => d.DynamicInvoke(args));
        }

        [Fact]
        public static void TestDynamicInvoke_AllowsPrimitiveWidening()
        {
            // For primitives, value-preserving widenings allowed.
            Delegate d = new DFoo1(Foo1);
            d.DynamicInvoke(new object[] { 7, (short)7 });
            
            // For primitives, conversion of enum to underlying integral prior to value-preserving widening allowed.
            d = new DFoo1(Foo1);
            d.DynamicInvoke(new object[] { 7, E4.Seven });

            // For primitives, conversion of enum to underlying integral prior to value-preserving widening allowed.
            d = new DFoo1(Foo1);
            d.DynamicInvoke(new object[] { 7, E2.Seven });

            // For primitives, conversion to enum after value-preserving widening allowed.
            d = new DFoo4(Foo4);
            d.DynamicInvoke(new object[] { E4.Seven, 7 });

            // For primitives, conversion to enum after value-preserving widening allowed.
            d = new DFoo4(Foo4);
            d.DynamicInvoke(new object[] { E4.Seven, (short)7 });

            // Size-preserving but non-value-preserving conversions NOT allowed.
            d = new DFoo1(Foo1);
            Assert.Throws<ArgumentException>(null, () => d.DynamicInvoke(new object[] { 7, (uint)7 }));
            
            d = new DFoo1(Foo1);
            Assert.Throws<ArgumentException>(null, () => d.DynamicInvoke(new object[] { 7, U4.Seven }));
        }

        [Fact]
        public static void TestDynamicInvoke_AllowsNullForValueTypes()
        {
            Delegate d = new DFoo5(Foo5);
            d.DynamicInvoke(new object[] { null });
        }

        [Fact]
        public static void TestDynamicInvoke_AllowsNullableConversion()
        {
            // DynamicInvoke allows conversion of T to Nullable<T>
            Delegate d = new DFoo6(Foo6);
            d.DynamicInvoke(new object[] { 7 });
            
            // DynamicInvoke allows conversion of T to Nullable<T> but T must match exactly.
            d = new DFoo6(Foo6);
            Assert.Throws<ArgumentException>(null, () => d.DynamicInvoke(new object[] { (short)7 }));

            d = new DFoo6(Foo6);
            Assert.Throws<ArgumentException>(null, () => d.DynamicInvoke(new object[] { E4.Seven }));
        }

        public struct TestStruct
        {
            public object o1;
            public object o2;
        }

        public class TestClass
        {
            public TestStruct structField;
        }
        
        public class A { }
        public class B : A { }
        public delegate A DynamicInvokeDelegate(A nonRefParam1, B nonRefParam2, ref A refParam, out B outParam);

        public static A DynamicInvokeTestFunction(A nonRefParam1, B nonRefParam2, ref A refParam, out B outParam)
        {
            outParam = (B)refParam;
            refParam = nonRefParam2;
            return nonRefParam1;
        }

        public delegate int SomeDelegate(int x);

        private static int SquareNumber(int x)
        {
            return x * x;
        }

        private static void EmptyFunc()
        {
        }

        public delegate TestStruct StructReturningDelegate();

        private static void Foo1(int expected, int actual)
        {
            Assert.Equal(expected, actual);
        }

        private delegate void DFoo1(int expected, int actual);
        private delegate void DFoo1WithDefault(int expected, int actual = 7);

        private static void Foo2(ref int i)
        {
            i++;
        }

        private delegate void DFoo2(ref int i);

        private struct MyStruct
        {
            public int x;
            public int y;
        }

        private static void Foo3(ref MyStruct s)
        {
            s.x += 7;
            s.y += 8;
        }

        private delegate void DFoo3(ref MyStruct s);

        private static void Foo4(E4 expected, E4 actual)
        {
            Assert.Equal(expected, actual);
        }

        private delegate void DFoo4(E4 expected, E4 actual);

        private static void Foo5(MyStruct s)
        {
            Assert.Equal(s.x, 0);
            Assert.Equal(s.y, 0);
        }

        private delegate void DFoo5(MyStruct s);

        private static void Foo6(int? n)
        {
            Assert.True(n.HasValue);
            Assert.Equal(n.Value, 7);
        }

        private delegate void DFoo6(int? s);

        private enum E2 : short
        {
            One = 1,
            Seven = 7,
        }

        private enum E4 : int
        {
            One = 1,
            Seven = 7,
        }

        private enum U4 : uint
        {
            One = 1,
            Seven = 7,
        }
    }

    public static class TestExtensionMethod
    {
        public static DelegateTests.TestStruct TestFunc(this DelegateTests.TestClass testClass)
        {
            return testClass.structField;
        }
    }
}
