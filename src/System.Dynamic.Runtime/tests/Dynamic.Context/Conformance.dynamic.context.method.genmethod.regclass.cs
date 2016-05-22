// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;
using Xunit;

namespace Dynamic.RegularClass.GenericMethod.Tests
{
    public class EmptyClass { }

    public interface EmptyInterface { }

    public class NonGenericClass
    {
        public T Method_ReturnsT<T>() => default(T);

        public T Method_ReturnsT<T>(T t) => t;

        public T Method_ReturnsT<T, U>(out T t)
        {
            t = default(T);
            return t;
        }

        public T Method_ReturnsT<T>(ref T t)
        {
            t = default(T);
            return t;
        }

        public T Method_ReturnsT<T>(params T[] t) => default(T);

        public int M<T>(T t) where T : IEnumerable<T> => 3;

        public T Method_ReturnsT<T, U>(params U[] d) => default(T);

        public T Method_ReturnsT<T, U>(float x, T t, U u) => default(T);

        public T Method_ReturnsT<T, U>(U u) => default(T);

        public T Method_ReturnsT<T, U, V>(out U u, ref V v)
        {
            u = default(U);
            return default(T);
        }

        public int Method_ReturnsInt<T>(T t) => 1;

        public string Method_ReturnsString<T, U>(T t, U u) => "foo";

        public float Method_ReturnsFloat<T, U, V>(T t, dynamic d, U u) => 3.4f;

        public float Method_ReturnsFloat<T, U, V>(T t, dynamic d, U u, ref decimal dec)
        {
            dec = 3m;
            return 3.4f;
        }

        public dynamic Method_ReturnsDynamic<T>(T t) => t;

        public dynamic Method_ReturnsDynamic<T, U>(T t, U u, dynamic d) => u;

        public dynamic Method_ReturnsDynamic<T, U, V>(T t, U u, V v, dynamic d) => v;
        
        public U Method_ReturnsUConstraint<U>(U u) where U : class
        {
            return null;
        }

        public U Method_ReturnsUConstraint<T, U>(T t) where U : new()
        {
            return new U();
        }

        public U Method_ReturnsUConstraint<T, U, V>(T t, V v) where U : new() where V : U, new()
        {
            return new V();
        }

        public dynamic Method_ReturnsDynamicConstraint<T, U>(T t, U u, dynamic d) where U : new()
        {
            return new U();
        }

        public dynamic Method_ReturnsDynamicConstraint<T, U, V>(T t, U u, V v, dynamic d) where V : class
        {
            return u;
        }

        public float Method_ReturnsFloatConstraint<T, U, V>(T t, dynamic d, U u, ref decimal dec) where V : U
        {
            dec = 3m;
            return 3.4f;
        }

        public string Method_ReturnsStringConstraint<T, U, V>(T t, dynamic d, U u) where U : class where V : U
        {
            return "";
        }

        //These are negative methods... you should not be able to call them with the dynamic type because the dynamic type would not satisfy the constraints
        //you cannot call this like: Method_ReturnsUConstraint<dynamic>(d); because object does not derive from C
        public U Method_ReturnsUNegConstraint<U>(U u) where U : EmptyClass
        {
            return null;
        }

        public T Method_ReturnsTNegConstraint<T, U>(U u) where U : struct
        {
            return default(T);
        }

        public float Method_ReturnsFloatNegConstraint<T, U, V>(T t, dynamic d, U u, ref decimal dec) where V : U where U : EmptyInterface
        {
            dec = 3m;
            return 3.4f;
        }
        
        public class NestedGenericClass<U>
        {
            public U Method_ReturnsU(U t) => default(U);

            public U Method_ReturnsT(params U[] d) => default(U);

            public U Method_ReturnsT<V>(out U u, ref V v)
            {
                u = default(U);
                return default(U);
            }

            public dynamic Method_ReturnsDynamic(U t, U u, dynamic d) => u;

            public dynamic Method_ReturnsDynamic<V>(U t, U u, V v, dynamic d) => v;

            public decimal Method_ReturnsDecimal<V>(U t, dynamic d, U u) => 3.4m;

            public byte Method_ReturnsByte<V>(U t, dynamic d, U u, ref double dec)
            {
                dec = 3;
                return 4;
            }
        }
    }

    public class RegularClassGenericMethodTests
    {
        [Fact]
        public void RegularMethodBody()
        {
            dynamic d = new NonGenericClass();
            Assert.False((bool)d.Method_ReturnsT<bool>());
        }

        private static dynamic s_mc = new NonGenericClass();
        public int _loc = (int)s_mc.Method_ReturnsT<int, string>(string.Empty, null, "abc");

        [Fact]
        public static void VariableInitializer()
        {
            RegularClassGenericMethodTests t = new RegularClassGenericMethodTests();
            Assert.Equal(0, t._loc);
        }

        [Fact]
        public static void VariableNamedDynamic()
        {
            int i = 1;
            dynamic dynamic = new NonGenericClass();
            int result = dynamic.Method_ReturnsT<int>(ref i);
            Assert.Equal(0, i);
            Assert.Equal(0, result);
        }

        public T TestMethod<T>(T t)
        {
            dynamic d = new NonGenericClass();
            return (T)d.Method_ReturnsT<T>(t, t, t);
        }

        [Fact]
        public void RegularMethodBody2()
        {
            RegularClassGenericMethodTests t1 = new RegularClassGenericMethodTests();
            Assert.Equal(0, t1.TestMethod(1L));
        }

        [Fact]
        public static void ImplicitlyTypedVariableInitializer1()
        {
            dynamic d = new NonGenericClass();
            string u = string.Empty;
            string v = string.Empty;
            var result = (int)d.Method_ReturnsT<int, string, string>(out u, ref v);
            Assert.Equal(0, result);
            Assert.Null(u);
            Assert.Equal(string.Empty, v);
        }

        [Fact]
        public static void ImplicitlyTypedVariableIntializer2()
        {
            dynamic mc = new NonGenericClass();
            var tc = new
            {
                A1 = (int)mc.Method_ReturnsInt<int>(0),
                A2 = (string)mc.Method_ReturnsString<int, int>(1, 2)
            };
            Assert.Equal(1, tc.A1);
            Assert.Equal("foo", tc.A2);
        }

        [Fact]
        public static void Operator()
        {
            dynamic d = new NonGenericClass();
            int result = (int)d.Method_ReturnsDynamic<int>(7) % (int)d.Method_ReturnsDynamic<int>(5);
            Assert.Equal(99, (int)d.Method_ReturnsDynamic<int>(99));
            Assert.Equal(2, result);
        }

        [Fact]
        public static void NullCoalescingOperator()
        {
            dynamic d = new NonGenericClass();
            string result = (string)d.Method_ReturnsDynamic<int, string>(10, null, d) ?? string.Empty;
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public static void QueryExpression()
        {
            var list = new List<string>() { null, "b", null, "a" };
            dynamic d = new NonGenericClass();
            string a = "a";
            dynamic da = a;
            var result = list.Where(p => p == (string)d.Method_ReturnsDynamic<string, int, string>(null, 10, a, da)).ToList();
            Assert.Equal(new List<string>() { "a" }, result);
        }

        [Fact]
        public static void ShortCircuitBooleanExpression()
        {
            dynamic d = new NonGenericClass.NestedGenericClass<string>();
            int loopCount = 0;
            while ((int)d.Method_ReturnsDynamic<int>(null, null, loopCount, d) < 10)
            {
                loopCount++;
            }
            Assert.Equal(10, loopCount);

        }

        [Fact]
        public static void StaticMethodBody1()
        {
            dynamic d = new NonGenericClass();
            var result = (object)d.Method_ReturnsUConstraint<RegularClassGenericMethodTests>(new RegularClassGenericMethodTests());
            Assert.Null(result);
        }

        [Fact]
        public static void StaticMethodBody2()
        {
            dynamic d = new NonGenericClass();
            dynamic t = new RegularClassGenericMethodTests();
            Assert.Throws<RuntimeBinderException>(() => d.Method_ReturnsUNegConstraint<RegularClassGenericMethodTests>(t));
        }

        [Fact]
        public static void StaticMethodBody3()
        {
            dynamic dy = new NonGenericClass();
            EmptyBaseClass mt = new EmptyBaseClass();
            dynamic d = mt;
            decimal dec = 0M;
            Assert.Throws<RuntimeBinderException>(() => (float)dy.Method_ReturnsFloatNegConstraint<RegularClassGenericMethodTests, dynamic, EmptySubClass>(null, d, d, ref dec));
        }

        [Fact]
        public static void StaticMethodBody4()
        {
            dynamic d = new NonGenericClass.NestedGenericClass<string>();
            string result = (string)d.Method_ReturnsU("0");
            Assert.Null(result);
        }
    }

    public class ClassWithIndexer
    {
        private Dictionary<int, string> _dic = new Dictionary<int, string>();
        private NonGenericClass _mc = new NonGenericClass();

        public string this[int i]
        {
            set
            {
                dynamic d = _mc;
                _dic.Add((int)d.Method_ReturnsT(i), value);
            }

            get
            {
                dynamic d = _mc;
                return _dic[(int)d.Method_ReturnsT(i)];
            }
        }

        [Fact]
        public void Indexer_Get()
        {
            ClassWithIndexer t = new ClassWithIndexer();
            t._dic[2] = "Test0";
            t._dic[1] = null;
            t._dic[0] = "Test2";
            Assert.Equal("Test0", t[2]);
            Assert.Null(t[1]);
            Assert.Equal("Test2", t[0]);
        }

        [Fact]
        public static void Indexer_Set()
        {
            ClassWithIndexer t = new ClassWithIndexer();
            t[0] = "Test0";
            t[1] = null;
            t[2] = "Test2";
            Assert.Equal("Test0", t._dic[0]);
            Assert.Null(t._dic[1]);
            Assert.Equal("Test2", t._dic[2]);
        }
    }

    public class ClassWithStaticFields
    {
        private static int s_field = 10;
        private static int s_result = -1;
        public ClassWithStaticFields()
        {
            dynamic d = new NonGenericClass();
            string s = "Foo";
            s_result = d.Method_ReturnsT<int, int, string>(out s_field, ref s);
        }

        [Fact]
        public static void StaticConstructor()
        {
            ClassWithStaticFields t = new ClassWithStaticFields();
            Assert.Equal(0, s_field);
            Assert.Equal(0, s_result);
        }
    }

    public class ClassWithConditionalAttribute
    {
        private static int s_result = 0;

        [Fact]
        public void ConditionalAttribute()
        {
            ClassWithConditionalAttribute c = new ClassWithConditionalAttribute();
            c.TestMethod();
            Assert.Equal(0, s_result);
        }

        [Conditional("c1")]
        public void TestMethod()
        {
            dynamic d = new NonGenericClass();
            s_result = (int)d.Method_ReturnsT<int, string>(null) + 1;
        }
    }

    public class ClassWithNestedClasses
    {
        public class NestedClass : IEnumerable<NestedClass>
        {
            public int Field;
            public IEnumerator<NestedClass> GetEnumerator()
            {
                return new NestedEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new NestedEnumerator();
            }
        }

        public class NestedEnumerator : IEnumerator<NestedClass>, IEnumerator
        {
            private NestedClass[] _list = new NestedClass[]
            {
                new NestedClass() { Field = 0 },
                new NestedClass() { Field = 1 },
                null
            };

            private int _index = -1;

            public NestedClass Current => _list[_index];

            public void Dispose() { }

            object IEnumerator.Current => _list[_index];

            public bool MoveNext()
            {
                _index++;
                return _index >= _list.Length ? false : true;
            }

            public void Reset() => _index = -1;
        }

        [Fact]
        public static void ArgumentsToMethodInvocation()
        {
            ClassWithNestedClasses t = new ClassWithNestedClasses();
            dynamic mc = new NonGenericClass();
            int result = t.Method((int)mc.M<NestedClass>(new NestedClass()));
            Assert.Equal(3, result);
        }

        public int Method(int value) => value;
    }

    public class ClassWithSetProperty
    {
        private float _field;
        public float TestProperty
        {
            set
            {
                dynamic mc = new NonGenericClass();
                dynamic mv = value;
                _field = (float)mc.Method_ReturnsFloat<float, string, string>(value, mv, null);
            }
            get { return _field; }
        }

        [Fact]
        public static void PropertySetBody()
        {
            ClassWithSetProperty t = new ClassWithSetProperty();
            t.TestProperty = 1.1f;
            Assert.Equal(3.4f, t.TestProperty);
        }
    }

    public class ClassWithConstructor
    {
        private float _filed;
        private decimal _dec;
        public ClassWithConstructor()
        {
            dynamic mc = new NonGenericClass();
            _filed = mc.Method_ReturnsFloat<string, long, int>(null, mc, 1L, ref _dec);
        }

        [Fact]
        public static void Constructor()
        {
            ClassWithConstructor t = new ClassWithConstructor();
            Assert.Equal(3.4f, t._filed);
            Assert.Equal(3M, t._dec);
        }
    }

    public class ClassWithDeconstructor
    {
        private static string s_field;
        public static object locker = new object();
        ~ClassWithDeconstructor()
        {
            lock (locker)
            {
                dynamic d = new NonGenericClass.NestedGenericClass<ClassWithDeconstructor>();
                s_field = d.Method_ReturnsDynamic<string>(null, null, "Test", d);
            }
        }

        public void Foo() { }

        private static void Verify()
        {
            lock (locker)
            {
                Assert.Equal("Test", s_field);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void RequireLifetimesEnded()
        {
            ClassWithDeconstructor t = new ClassWithDeconstructor();
            s_field = "Field";
            t.Foo();
        }

        [Fact]
        public static void Deconstructor()
        {
            RequireLifetimesEnded();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            // If move the code in Verify() to here, the finalizer will only be executed after exited Main
            Verify();
        }
    }

    public class ClassWithRegularField
    {
        private int _field;
        public ClassWithRegularField()
        {
            _field = 10;
        }

        [Fact]
        public static void StaticMethodBody1()
        {
            dynamic d = new NonGenericClass();
            var result = (ClassWithRegularField)d.Method_ReturnsUConstraint<int, ClassWithRegularField>(4);
            Assert.Equal(10, result._field);
        }

        [Fact]
        public static void StaticMethodBody2()
        {
            dynamic d = new NonGenericClass();
            dynamic result = d.Method_ReturnsDynamicConstraint<int, ClassWithRegularField>(1, new ClassWithRegularField() { _field = 0 }, d);

            Assert.Equal(10, ((ClassWithRegularField)result)._field);
        }

        [Fact]
        public static void StaticMethodBody3()
        {
            dynamic d = new NonGenericClass();
            dynamic s = "Test";
            dynamic result = d.Method_ReturnsDynamicConstraint<string, string, string>((string)s, (string)s, (string)s, s);
            Assert.Equal("Test", (string)result);
        }
    }

    public class ClassWithNestedClass
    {
        private int _field;
        public ClassWithNestedClass()
        {
            _field = 10;
        }

        public class NestedClass : ClassWithNestedClass
        {
            public NestedClass()
            {
                _field = 11;
            }
        }

        [Fact]
        public static void StaticMethodBody1()
        {
            dynamic d = new NonGenericClass();
            var result = (ClassWithNestedClass)d.Method_ReturnsUConstraint<string, ClassWithNestedClass, NestedClass>(null, new NestedClass() { _field = 0 });

            Assert.IsType<NestedClass>(result);
            Assert.Equal(11, result._field);
        }

        [Fact]
        public static void StaticMethodBody2()
        {
            dynamic d = new NonGenericClass();
            ClassWithNestedClass t = new ClassWithNestedClass();
            dynamic it = new NestedClass();
            decimal dec = 0M;
            float result = (float)d.Method_ReturnsFloatConstraint<ClassWithNestedClass, ClassWithNestedClass, NestedClass>(t, it, t, ref dec);
            Assert.Equal(3M, dec);
            Assert.Equal(3.4f, result);
        }
    }

    public class EmptyBaseClass : EmptyInterface { }

    public class EmptySubClass : EmptyBaseClass { }
}
