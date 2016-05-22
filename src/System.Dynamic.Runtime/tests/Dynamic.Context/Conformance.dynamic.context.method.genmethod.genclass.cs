// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.CSharp.RuntimeBinder;
using Xunit;

namespace Dynamic.GenericClass.GenericMethod.Tests
{
    public class EmptyClass { }

    public interface EmptyInterface { }

    public class GenericClass<T>
    {
        public U Method_ReturnsU<U>(U u) => u;

        public U Method_ReturnsU<U>(T t) => default(U);

        public U Method_ReturnsU<U, V>(T t, V v) => default(U);

        public T Method_ReturnsT<U>(params U[] d) => default(T);

        public T Method_ReturnsT<U>(float x, T t, U u) => default(T);

        public T Method_ReturnsT<U>(U u) => default(T);

        public T Method_ReturnsT<U, V>(out U u, ref V v)
        {
            u = default(U);
            return default(T);
        }

        public int Method_ReturnsInt(T t) => 1;

        public string Method_ReturnsString<U>(T t, U u) => "foo";

        public float Method_ReturnsFloat<U, V>(T t, dynamic d, U u) => 3.4f;

        public float Method_ReturnsFloat<U, V>(T t, dynamic d, U u, ref decimal dec)
        {
            dec = 3m;
            return 3.4f;
        }

        public dynamic Method_ReturnsDynamic(T t) => t;

        public dynamic Method_ReturnsDynamic<U>(T t, U u, dynamic d) => u;

        public dynamic Method_ReturnsDynamic<U, V>(T t, U u, V v, dynamic d) => v;
        
        public U Method_ReturnsUConstraint<U>(U u) where U : class
        {
            return null;
        }

        public U Method_ReturnsUConstraint<U>(T t) where U : new()
        {
            return new U();
        }

        public U Method_ReturnsUConstraint<U, V>(T t, V v) where U : new() where V : U
        {
            return default(V);
        }

        public void Method_WithConstraints<U, V, W>(T t, U u, V v, W w) where U : struct where V : class where W : V
        {
        }

        public dynamic Method_ReturnsDynamicConstraint<U>(T t, U u, dynamic d) where U : new()
        {
            return new U();
        }

        public dynamic Method_ReturnsDynamicConstraint<U, V>(T t, U u, V v, dynamic d) where V : class
        {
            return u;
        }

        public float Method_ReturnsFloatConstraint<U, V>(T t, dynamic d, U u, ref decimal dec) where V : U
        {
            dec = 3m;
            return 3.4f;
        }

        public string Method_ReturnsStringConstraint<U, V>(T t, dynamic d, U u) where U : class where V : U
        {
            return "";
        }

        //These are negative methods... you should not be able to call them with the dynamic type because the dynamic type would not satisfy the constraints
        //you cannot call this like: Method_ReturnsUConstraint<dynamic>(d); because object does not derive from C
        public U Method_ReturnsUNegConstraint<U>(U u) where U : EmptyClass
        {
            return null;
        }

        public T Method_ReturnsTNegConstraint<U>(U u) where U : struct
        {
            return default(T);
        }

        public float Method_ReturnsFloatNegConstraint<U, V>(T t, dynamic d, U u, ref decimal dec) where V : U where U : EmptyInterface
        {
            dec = 3m;
            return 3.4f;
        }
        
        public class NestedGenericClass<U>
        {
            public U Method_ReturnsU(T t)
            {
                return default(U);
            }

            public T Method_ReturnsT(params U[] d)
            {
                return default(T);
            }

            public T Method_ReturnsT<V>(out U u, ref V v)
            {
                u = default(U);
                return default(T);
            }

            public dynamic Method_ReturnsDynamic(T t, U u, dynamic d)
            {
                return u;
            }

            public dynamic Method_ReturnsDynamic<V>(T t, U u, V v, dynamic d)
            {
                return v;
            }

            public decimal Method_ReturnsDecimal<V>(T t, dynamic d, U u)
            {
                return 3.4m;
            }

            public byte Method_ReturnsByte<V>(T t, dynamic d, U u, ref double dec)
            {
                dec = 3;
                return 4;
            }
        }
    }

    public class GenericClassWithClassConstraint<T> where T : class
    {
        public int Method_ReturnsInt<U>() where U : T
        {
            return 1;
        }

        public T Method_ReturnsT<U, V>(decimal dec, dynamic d) where U : T where V : U
        {
            return null;
        }
    }

    public class GenericClassWithNewConstraint<T> where T : new()
    {
        public U Method_ReturnsU<U>() where U : T
        {
            return default(U);
        }

        public dynamic Method_ReturnsDynamic<V>(T u, V v) where V : T
        {
            return new T();
        }
    }

    public class GenericClassWithAnotherTypeConstraint<T, U> where T : U
    {
        public U Method_ReturnsU()
        {
            return default(U);
        }

        public dynamic Method_ReturnsDynamic<V>(int x, U u, V v) where V : U
        {
            return default(T);
        }
    }
    
    public class GenericClassWithUDClassConstraint<T> where T : EmptyClass
    {
        public U Method_ReturnsU<U>() where U : T, new()
        {
            return new U();
        }
    }

    public class GenericClassWithStructConstraint<T> where T : struct
    {
        public U Method_ReturnsU<U>() => default(U);
    }

    public class GenericClassWithInterfaceConstraint<T> where T : EmptyInterface
    {
        public dynamic Method_ReturnsDynamic<U, V>(int x, U u, V v) where V : U where U : T
        {
            return default(T);
        }
    }

    public class GenericMethodGenericClassTests
    {
        public class NestedClass : GenericMethodGenericClassTests { }

        [Fact]
        public static void StaticMethodBody1()
        {
            dynamic d = new GenericClass<int>();
            int ret = d.Method_ReturnsT<string>(string.Empty, "ab", null);
            dynamic d1 = string.Empty;
            dynamic d2 = "ab";
            ret += d.Method_ReturnsT(d1, d2, null);
            Assert.Equal(0, ret);
        }

        [Fact]
        public static void IfElseBlock()
        {
#if c1
dynamic mc = new GenericClass<int>();
return (Test)mc.Method_ReturnsUConstraint<Test>(new Test());
#else
            dynamic d = new GenericClass<int>();
            Assert.Null(d.Method_ReturnsUConstraint<string>("1"));
            Assert.Throws<RuntimeBinderException>(() => (int)d.Method_ReturnsUConstraint<string>(1));
#endif
        }

        [Fact]
        public static void ImplicitlyTypedVariableInitializer()
        {
            dynamic d = new GenericClass<string>();
        }

        [Fact]
        public static void ImplicitExplicitOperator()
        {
            dynamic dy1 = new GenericClass<int>();
            long result1 = (int)dy1.Method_ReturnsDynamic(1); // implicit
            dynamic dy2 = new GenericClass<long>();
            int result2 = (int)dy2.Method_ReturnsDynamic(1L); // explicit
            Assert.Equal(result1, result2);
        }

        [Fact]
        public static void ForeachExpressionBody()
        {
            dynamic d = new GenericClass<string>();
            List<string> list = new List<string>() { null, string.Empty, "Test" };
            List<string> list2 = new List<string>();
            foreach (dynamic s in (IEnumerable)d.Method_ReturnsDynamic<int, IEnumerable>(null, 0, list, string.Empty))
            {
                list2.Add(s);
            }

            Assert.Equal(new List<string>() { null, string.Empty, "Test" }, list2);
        }

        [Fact]
        public static void ForeachLoopBody()
        {
            dynamic mc = new GenericClass<string>();
            List<int> list1 = new List<int>() { 0, 1, 2 };
            List<int> list2 = new List<int>();
            foreach (int s in list1)
            {
                list2.Add((int)mc.Method_ReturnsU<int, string>(null, null) + s);
            }

            Assert.Equal(new List<int>() { 0, 1, 2 }, list2);
        }

        [Fact]
        public static void StaticMethodBody2()
        {
            dynamic mc = new GenericClass<string>();
            Assert.Null((string)mc.Method_ReturnsT<long>(1.11f, string.Empty, 1L));
        }

        [Fact]
        public static void StaticMethodBody3()
        {
            string u = "Test";
            int v = 10;
            dynamic d = new GenericClass<long>();
            long result = (long)d.Method_ReturnsT<string, int>(out u, ref v);

            Assert.Equal(0, result);
            Assert.Equal(10, v);
            Assert.Null(u);
        }

        [Fact]
        public static void WhileDoExpression()
        {
            dynamic d = new GenericClass<int>();
            int[] array = new int[] { 1, 2, 1, 3, 1 };
            int i = 0;
            while (i < array.Length && array[i] >= (int)d.Method_ReturnsInt(array[i]))
            {
                array[i] = (int)d.Method_ReturnsInt(array[i]);
                i++;
            }

            while (i < array.Length)
            {
                Assert.False(true);
                Assert.Equal(1, array[i]);
            }
        }

        [Fact]
        public static void StaticMethodBody4()
        {
            dynamic d = new GenericClass<string>();
            float result = (float)d.Method_ReturnsFloat<int, EmptyClass>(null, new EmptyClass(), 0);
            Assert.Equal(3.4f, result);
        }

        [Fact]
        public static void StaticMethodBody5()
        {
            decimal dec = 0M;
            dynamic d = new GenericClass<string>();
            float result = (float)d.Method_ReturnsFloat<int, EmptyClass>(null, new EmptyClass(), 0, ref dec);
            Assert.Equal(3.4f, result);
            Assert.Equal(3M, dec);
        }

        [Fact]
        public static void StaticMethodBody6()
        {
            dynamic d = new GenericClass<int>();
            Assert.Throws<RuntimeBinderException>(() => (string)d.Method_ReturnsDynamicConstraint<string>(10, null, d));
        }

        [Fact]
        public static void StaticMethodBody7()
        {
            dynamic d = new GenericClass<string>();
            dynamic mc2 = "Test2";
            dynamic mc3 = "Test3";
            string result = (string)d.Method_ReturnsDynamicConstraint<string, EmptyClass>(null, mc2, new EmptyClass(), mc3);
            Assert.Equal("Test2", result);
            Assert.Equal("Test3", (string)mc3);
        }
        
        [Fact]
        public static void StaticMethodBody_DynamicTypeDoesNotSatisfyConstraints()
        {
            dynamic d = new GenericClass<string>();
            GenericMethodGenericClassTests test = new GenericMethodGenericClassTests();
            dynamic d2 = test;
            Assert.Throws<RuntimeBinderException>(() => (string)d.Method_ReturnsTNegConstraint<GenericMethodGenericClassTests>(d2));
        }
        
        [Fact]
        public static void AnonymousMethod()
        {
            dynamic d = new GenericClass<string>.NestedGenericClass<int>();
            Func<string, int> func = delegate (string arg)
            {
                return (int)d.Method_ReturnsU(null);
            };
            Assert.Equal(0, func(null));
        }

        [Fact]
        public static void LambdaExpression()
        {
            dynamic d = new GenericClass<string>.NestedGenericClass<int>();
            Func<int, int, string> func = (int arg1, int arg2) => (string)d.Method_ReturnsT(arg1, arg1);
            Assert.Null(func(1, 2));
        }

        [Fact]
        public static void StaticMethodBody8()
        {
            dynamic mc = new GenericClass<string>.NestedGenericClass<int>();
            int u = 10;
            string v = "Test";
            string result = (string)mc.Method_ReturnsT<string>(out u, ref v);
            Assert.Equal(0, u);
            Assert.Equal("Test", v);
            Assert.Null(result);
        }
        
        [Fact]
        public static void StaticMethodBody9()
        {
            dynamic mc = new GenericClass<GenericMethodGenericClassTests>.NestedGenericClass<GenericMethodGenericClassTests>();
            GenericMethodGenericClassTests t = new GenericMethodGenericClassTests();
            dynamic dy = t;
            decimal result = (decimal)mc.Method_ReturnsDecimal<int>(t, dy, t);
            Assert.Equal(3.4M, result);
        }

        [Fact]
        public static void StaticMethodBody10()
        {
            double dec = 1.1D;
            dynamic mc = new GenericClass<GenericMethodGenericClassTests>.NestedGenericClass<GenericMethodGenericClassTests>();
            GenericMethodGenericClassTests t = new GenericMethodGenericClassTests();
            dynamic dy = t;
            byte result = (byte)mc.Method_ReturnsByte<int>(t, dy, t, ref dec);
            Assert.Equal(4, result);
            Assert.Equal(3, dec);
        }

        [Fact]
        public static void DefaultSwitchStatement()
        {
            dynamic mc = new GenericClassWithClassConstraint<string>();
            int result = -1;
            switch (result)
            {
                case 0:
                    break;
                default:
                    result = (int)mc.Method_ReturnsInt<string>();
                    break;
            }
            Assert.Equal(1, result);
        }

        [Fact]
        public static void SwitchStatement()
        {
            dynamic mc = new GenericClassWithClassConstraint<GenericMethodGenericClassTests>();
            GenericMethodGenericClassTests result = new NestedClass();
            dynamic innertest = result;
            int field = 0;
            switch (field)
            {
                case 0:
                    result = (GenericMethodGenericClassTests)mc.Method_ReturnsT<NestedClass, NestedClass>(3.2M, innertest);
                    break;
                default:
                    break;
            }
            Assert.Null(result);
        }

        [Fact]
        public static void StaticMethodBody11()
        {
            dynamic mc = new GenericClassWithNewConstraint<GenericMethodGenericClassTests>();
            NestedClass result = (NestedClass)mc.Method_ReturnsU<NestedClass>();
            Assert.Null(result);
        }

        [Fact]
        public static void StaticMethodBody12()
        {
            dynamic d = new GenericClassWithAnotherTypeConstraint<NestedClass, NestedClass>();
            GenericMethodGenericClassTests result = (GenericMethodGenericClassTests)d.Method_ReturnsU();
            Assert.Null(result);
        }

        [Fact]
        public static void ImplicitlyTypedVariableInitializer2()
        {
            dynamic d = new GenericClass<string>();
            bool threwRuntimeBinderException = false;
            try
            {
                var loc = d.Method_ReturnsU(null);
            }
            catch (RuntimeBinderException)
            {
                threwRuntimeBinderException = true;
            }
            Assert.True(threwRuntimeBinderException);
        }
        
        [Fact]
        public static void ForeachExpression()
        {
            dynamic d = new GenericClass<string>();
            List<string> list = new List<string>() { null, string.Empty, "Test" };
            List<string> list2 = new List<string>();
            foreach (string s in d.Method_ReturnsDynamic(null, 0, list, d))
            {
                list2.Add(s);
            }

            Assert.Equal(new List<string>() { null, string.Empty, "Test" }, list2);
        }

        [Fact]
        public static void StaticMethodBody13()
        {
            dynamic d = new GenericClass<string>();
            Assert.Null(d.Method_ReturnsT(1.11f, string.Empty, 1L));
        }

        [Fact]
        public static void StaticMethodBody14()
        {
            dynamic u = "Test";
            dynamic v = 10;
            dynamic d = new GenericClass<long>();
            long result = d.Method_ReturnsT(out u, ref v);
            Assert.Equal(0, result);
            Assert.Null(u);
            Assert.Equal(10, v);
        }

        [Fact]
        public static void StaticMethodBody15()
        {
            dynamic d1 = new GenericClass<string>();
            dynamic d2 = "Test2";
            dynamic d3 = "Test3";
            string result = d1.Method_ReturnsDynamicConstraint(null, d2, new EmptyClass(), d3);
            Assert.Equal("Test2", result);
            Assert.Equal("Test3", d3);
        }

        [Fact]
        public static void StaticMethodBody16()
        {
            dynamic d = new GenericClass<string>();
            GenericMethodGenericClassTests t = new GenericMethodGenericClassTests();
            Assert.Throws<RuntimeBinderException>(() => d.Method_ReturnsTNegConstraint(t));
        }

        [Fact]
        public static void StaticMethodBody17()
        {
            dynamic d = new GenericClass<string>.NestedGenericClass<int>();
            int u = 10;
            string v = "Test";
            string result = d.Method_ReturnsT(out u, ref v);
            Assert.Equal(0, u);
            Assert.Equal("Test", v);
            Assert.Null(result);
        }

        [Fact]
        public static void StaticMethodBody18()
        {
            dynamic d1 = new GenericClass<string>.NestedGenericClass<string>();
            dynamic dy = "Me";
            dynamic result = d1.Method_ReturnsDynamic(null, "Test", 10, dy);
            Assert.Equal(10, result);
        }
    }

    public class ClassWithImplicitlyTypedVariableInitializer
    {
        private static dynamic s_d = new GenericClass<string>();

        [Fact]
        public static void ImplicitlyTypedVariableInitializer()
        {
            var result = (int)s_d.Method_ReturnsU<int>(null);
            Assert.Equal(0, result);
        }
    }

    public class ClassWithImplicitOperator1
    {
        public class InnerTest1
        {
            public int field;
            public static implicit operator InnerTest2(InnerTest1 t1)
            {
                dynamic dy = new GenericClass<InnerTest1>();
                return new InnerTest2()
                {
                    field = (int)dy.Method_ReturnsU<int>(t1.field + 1)
                };
            }
        }

        public class InnerTest2
        {
            public int field;
        }

        [Fact]
        public static void ImplicitOperator()
        {
            dynamic d = new GenericClass<InnerTest2>();
            InnerTest2 result1 = (InnerTest1)d.Method_ReturnsDynamic<InnerTest1>(
                new InnerTest2() { field = 0 },
                new InnerTest1() { field = 10 },
                0);

            Assert.Equal(11, result1.field);
        }
    }

    public class ClassWithVolatileMembers
    {
        private dynamic _mc = new GenericClass<string>.NestedGenericClass<string>();
        private dynamic _dy = "Me";
        private volatile dynamic _field;
        public ClassWithVolatileMembers()
        {
            _field = _mc.Method_ReturnsDynamic(null, "Test", _dy);
        }

        [Fact]
        public static void StaticMethodBody()
        {
            ClassWithVolatileMembers t = new ClassWithVolatileMembers();
            Assert.Equal("Test", (string)t._field);
        }
    }

    public class ClassWithTwoNestedClasses
    {
        public class NestedClass1 : ClassWithTwoNestedClasses { }
        public class NestedClass2 : ClassWithTwoNestedClasses { }

        [Fact]
        public static void StaticMethodBody1()
        {
            dynamic d = new GenericClassWithAnotherTypeConstraint<NestedClass1, ClassWithTwoNestedClasses>();
            NestedClass1 result = (NestedClass1)d.Method_ReturnsDynamic<NestedClass2>(0, new ClassWithTwoNestedClasses(), new NestedClass2());
            Assert.Null(result);
        }

        [Fact]
        public static void StaticMethodBody2()
        {
            dynamic d = new GenericClassWithAnotherTypeConstraint<NestedClass1, ClassWithTwoNestedClasses>();
            NestedClass1 result = (NestedClass1)d.Method_ReturnsDynamic(0, new ClassWithTwoNestedClasses(), new NestedClass2());
            Assert.Null(result);
        }
    }

    public class ClassWithEmptyBaseClass : EmptyClass
    {
        [Fact]
        public static void StaticMethodBody()
        {
            dynamic d = new GenericClassWithUDClassConstraint<EmptyClass>();
            Assert.Throws<RuntimeBinderException>(() => d.Method_ReturnsU<dynamic>());
        }
    }

    public class ClassWithExplicitOperator2
    {
        public class InnerTest1
        {
            public int field;
            public static explicit operator InnerTest2(InnerTest1 t1)
            {
                dynamic dy = new GenericClass<InnerTest1>();
                return new InnerTest2()
                {
                    field = (int)dy.Method_ReturnsU<int>(t1.field + 1)
                };
            }
        }

        public class InnerTest2
        {
            public int field;
        }

        [Fact]
        public static void ExplicitOperator()
        {
            dynamic d = new GenericClass<InnerTest2>();
            InnerTest2 result1 = (InnerTest2)((InnerTest1)d.Method_ReturnsDynamic<InnerTest1>(
            new InnerTest2() { field = 0 },
            new InnerTest1() { field = 10 },
            0)); //explicit
            Assert.Equal(11, result1.field);
        }
    }

    public class ClassWithImplicitOperator2
    {
        public class InnerTest1
        {
            public int field;
            public static implicit operator InnerTest2(InnerTest1 t1)
            {
                dynamic dy = new GenericClass<InnerTest1>();
                return new InnerTest2() { field = dy.Method_ReturnsU(t1.field + 1) };
            }
        }

        public class InnerTest2
        {
            public int field;
        }

        [Fact]
        public static void ImplicitOperator()
        {
            dynamic d = new GenericClass<InnerTest2>();
            InnerTest2 result = d.Method_ReturnsDynamic(
                new InnerTest2() { field = 0 },
                new InnerTest1() { field = 10 },
                0); // implicit;

            Assert.Equal(11, result.field);
        }
    }

    public class ClassWithEmptyInterface : EmptyInterface
    {
        [Fact]
        public static void StaticMethodBody1()
        {
            dynamic d = new GenericClass<int>();
            Assert.Equal(0, (int)d.Method_ReturnsT<EmptyInterface>(new ClassWithEmptyInterface()));
        }

        [Fact]
        public static void StaticMethodBody2()
        {
            dynamic d1 = new GenericClass<int>();
            EmptyInterface i = new ClassWithEmptyInterface();
            dynamic d2 = i;
            Assert.Equal(0, d1.Method_ReturnsT(d2));
        }

        [Fact]
        public static void StaticMethodBody3()
        {
            dynamic mc = new GenericClass<int>();
            dynamic d = new ClassWithEmptyInterface();
            Assert.Equal(0, mc.Method_ReturnsT<EmptyInterface>(d));
        }
    }

    public class ClassWithSinglePrivateField
    {
        private int _field;
        public ClassWithSinglePrivateField()
        {
            _field = 10;
        }

        [Fact]
        public static void StaticMethodBody1()
        {
            dynamic d = new GenericClass<string>();
            Assert.Throws<RuntimeBinderException>(() => (ClassWithSinglePrivateField)d.Method_ReturnsUConstraint<ClassWithSinglePrivateField>(null));
        }

        [Fact]
        public static void StaticMethodBody2()
        {
            dynamic d = new GenericClass<string>();
            ClassWithSinglePrivateField result = null;
            result = d.Method_ReturnsUConstraint(result);
            Assert.Null(result);
        }
    }

    public class ClassWithSimpleInternalFieldAndNestedClass
    {
        internal int _field;
        public ClassWithSimpleInternalFieldAndNestedClass()
        {
            _field = 10;
        }

        public class NestedClass : ClassWithSimpleInternalFieldAndNestedClass
        {
            public NestedClass()
            {
                _field = 11;
            }
        }

        [Fact]
        public static void StaticMethodBody1()
        {
            dynamic d = new GenericClass<string>();
            ClassWithSimpleInternalFieldAndNestedClass result = (ClassWithSimpleInternalFieldAndNestedClass)d.Method_ReturnsUConstraint<ClassWithSimpleInternalFieldAndNestedClass, NestedClass>(null, new NestedClass() { _field = 0 });
            Assert.Null(result);
        }

        [Fact]
        public static void StaticMethodBody2()
        {
            dynamic d = new GenericClassWithNewConstraint<ClassWithSimpleInternalFieldAndNestedClass>();
            var t = new NestedClass() { _field = 0 };
            ClassWithSimpleInternalFieldAndNestedClass result = (ClassWithSimpleInternalFieldAndNestedClass)d.Method_ReturnsDynamic<NestedClass>(t, t);
            Assert.Equal(10, result._field);
        }

        [Fact]
        public static void StaticMethodBody3()
        {
            dynamic mc = new GenericClassWithNewConstraint<ClassWithSimpleInternalFieldAndNestedClass>();
            ClassWithSimpleInternalFieldAndNestedClass t = new NestedClass() { _field = 0 };
            ClassWithSimpleInternalFieldAndNestedClass result = mc.Method_ReturnsDynamic(t, t);
            Assert.Equal(10, result._field);
        }
    }

    public class ClassWithSimpleInternalFieldAndEmptyNestedClass
    {
        internal int Field;
        public class NestedClass : ClassWithSimpleInternalFieldAndEmptyNestedClass { }

        [Fact]
        public static void StaticMethodBody1()
        {
            decimal dec = 10M;
            dynamic d = new GenericClass<ClassWithSimpleInternalFieldAndEmptyNestedClass>();
            ClassWithSimpleInternalFieldAndEmptyNestedClass mc2 = new ClassWithSimpleInternalFieldAndEmptyNestedClass() { Field = 2 };
            dynamic dy2 = mc2;
            ClassWithSimpleInternalFieldAndEmptyNestedClass mc3 = new NestedClass() { Field = 3 };

            float result = (float)d.Method_ReturnsFloatConstraint<ClassWithSimpleInternalFieldAndEmptyNestedClass, NestedClass>(mc2, dy2, mc3, ref dec);
            Assert.Equal(3M, dec);
            Assert.Equal(3.4f, result);
        }

        [Fact]
        public static void StaticMethodBody2()
        {
            dynamic mc = new GenericClass<ClassWithSimpleInternalFieldAndEmptyNestedClass>();
            ClassWithSimpleInternalFieldAndEmptyNestedClass mc2 = new ClassWithSimpleInternalFieldAndEmptyNestedClass() { Field = 2 };
            dynamic dy2 = mc2;
            ClassWithSimpleInternalFieldAndEmptyNestedClass mc3 = new NestedClass() { Field = 3 };
            string result = (string)mc.Method_ReturnsStringConstraint<ClassWithSimpleInternalFieldAndEmptyNestedClass, NestedClass>(mc2, dy2, mc3);
            Assert.Equal(string.Empty, result);
        }
    }

    public class ClassWithSimpleInternalFieldEmptyNestedClassAndEmptyInterface : EmptyInterface
    {
        internal int Field;
        public class NestedClass : ClassWithSimpleInternalFieldEmptyNestedClassAndEmptyInterface { }

        [Fact]
        public static void StaticMethodBody()
        {
            dynamic mc = new GenericClass<ClassWithSimpleInternalFieldEmptyNestedClassAndEmptyInterface>();
            ClassWithSimpleInternalFieldEmptyNestedClassAndEmptyInterface mc2 = new ClassWithSimpleInternalFieldEmptyNestedClassAndEmptyInterface() { Field = 2 };
            dynamic dy2 = mc2;

            ClassWithSimpleInternalFieldEmptyNestedClassAndEmptyInterface mc3 = new NestedClass() { Field = 3 };
            dynamic dy3 = mc3;
            decimal dec = 3;
            decimal result = (decimal)mc.Method_ReturnsFloatNegConstraint<ClassWithSimpleInternalFieldEmptyNestedClassAndEmptyInterface, NestedClass>(dy2, dy2, dy3, ref dec);
            Assert.Equal(3.4M, result);
        }
    }
    
    public class ClasWithExplicitOperator2
    {
        public class InnerTest1
        {
            public int field;
            public static explicit operator InnerTest2(InnerTest1 t1)
            {
                dynamic dy = new GenericClass<InnerTest1>();
                return new InnerTest2() { field = dy.Method_ReturnsU(t1.field + 1) };
            }
        }

        public class InnerTest2
        {
            public int field;
        }

        [Fact]
        public static void ExplicitOperator()
        {
            dynamic dy = new GenericClass<InnerTest2>();
            InnerTest2 result = (InnerTest2)((InnerTest1)dy.Method_ReturnsDynamic(
                new InnerTest2() { field = 0 },
                new InnerTest1() { field = 10 },
                0)); //explicit
            Assert.Equal(11, result.field);
        }
    }
}
