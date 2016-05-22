// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Dynamic.GenericClass.RegularMethod.Tests
{
    public class EmptyClass { }

    public interface EmptyInterface { }

    public class MyClass
    {
        public int Field = 0;
    }

    public struct MyStruct
    {
        public int Number;
    }

    public enum MyEnum
    {
        First = 1,
        Second = 2,
        Third = 3
    }

    public class GenericClass<T>
    {
        public bool Method_ReturnBool() => false;

        public T Method_ReturnsT() => default(T);

        public T Method_ReturnsT(T t) => t;

        public T Method_ReturnsT(out T t)
        {
            t = default(T);
            return t;
        }

        public T Method_ReturnsT(ref T t, T tt)
        {
            t = default(T);
            return t;
        }

        public T Method_ReturnsT(params T[] t) => default(T);

        public T Method_ReturnsT(float x, T t) => default(T);

        public int Method_ReturnsInt(T t) => 1;

        public float Method_ReturnsFloat(T t, dynamic d) => 3.4f;

        public float Method_ReturnsFloat(T t, dynamic d, ref decimal dec)
        {
            dec = 3m;
            return 3.4f;
        }

        public dynamic Method_ReturnsDynamic(T t) => t;

        public dynamic Method_ReturnsDynamic(T t, dynamic d) => t;

        public dynamic Method_ReturnsDynamic(T t, int x, dynamic d) => t;

        public static bool? StaticMethod_ReturnBoolNullable() => false;
    }

    public class GenericClassMultipleParams<T, U, V>
    {
        public T Method_ReturnsT(V v, U u) => default(T);
    }

    public class GenericClassWithClassConstraint<T> where T : class
    {
        public int Method_ReturnsInt() => 1;

        public T Method_ReturnsT(decimal dec, dynamic d) => null;
    }

    public class GenericClassWithNewConstraint<T> where T : new()
    {
        public T Method_ReturnsT() => new T();

        public dynamic Method_ReturnsDynamic(T t) => new T();
    }

    public class GenericClassWithAnotherTypeConstraint<T, U> where T : U
    {
        public U Method_ReturnsU(dynamic d) => default(U);

        public dynamic Method_ReturnsDynamic(int x, U u, dynamic d) => default(T);
    }

    public class GenericClassWithUDClassConstraint<T> where T : EmptyClass, new()
    {
        public EmptyClass Method_ReturnsC() => new T();
    }

    public class GenericClassWithStructConstraint<T> where T : struct
    {
        public dynamic Method_ReturnsDynamic(int x) => x;
    }

    public class GenericClassWithInterfaceConstraint<T> where T : EmptyInterface
    {
        public dynamic Method_ReturnsDynamic(int x, T v) => default(T);
    }

    public class GenericClassRegularMethodTests
    {
        [Fact]
        public void RegularMethodBody()
        {
            dynamic mc = new GenericClass<string>();
            Assert.False((bool)mc.Method_ReturnBool());
        }

        [Fact]
        public static void ForLoop1()
        {
            dynamic d = new GenericClass<MyEnum>();
            int result = 0;
            for (int i = (int)d.Method_ReturnsInt(MyEnum.Third); i < 10; i++)
            {
                result += (int)d.Method_ReturnsInt((MyEnum)(i % 3 + 1));
            }
            Assert.Equal(9, result);
        }

        [Fact]
        public static void ForLoop2()
        {
            dynamic d = new GenericClass<MyEnum>();
            int result = 0;
            for (int i = d.Method_ReturnsInt(MyEnum.Third); i < 10; i++)
            {
                result += d.Method_ReturnsInt((MyEnum)(i % 3 + 1));
            }
            Assert.Equal(9, result);
        }

        [Fact]
        public static void Lambda()
        {
            dynamic d = new GenericClass<string>();
            dynamic d1 = 3;
            dynamic d2 = "Test";
            Func<string, string, string> fun = (x, y) => (y + x.ToString());
            string result = fun((string)d.Method_ReturnsDynamic("foo", d1), (string)d.Method_ReturnsDynamic("bar", d2));
            Assert.Equal("barfoo", result);
        }

        [Fact]
        public static void ArrayInitializer()
        {
            dynamic d = new GenericClass<string>();
            string[] array = new string[]
            {
                (string)d.Method_ReturnsDynamic(string.Empty, 1, d), (string)d.Method_ReturnsDynamic(null, 0, d), (string)d.Method_ReturnsDynamic("a", -1, d)
            };

            Assert.Equal(new string[] { string.Empty, null, "a" }, array);

        }

        [Fact]
        public static void NullCoalescingOperator()
        {
            dynamic dy = GenericClass<string>.StaticMethod_ReturnBoolNullable();
            Assert.False((bool?)dy ?? false);
        }

        [Fact]
        public static void StaticMethodBody()
        {
            dynamic d = new GenericClassMultipleParams<MyEnum, MyStruct, MyClass>();
            MyEnum me = d.Method_ReturnsT(null, new MyStruct());
            Assert.Equal(0, (int)me);
        }

        [Fact]
        public static void TernaryOperatorExpression()
        {
            dynamic d = new GenericClassWithClassConstraint<string>();
            string result = (string)d.Method_ReturnsT(decimal.MaxValue, d) == null ? string.Empty : (string)d.Method_ReturnsT(decimal.MaxValue, d);
            int ternaryResult = result == string.Empty ? 0 : 1;
            Assert.Equal(0, ternaryResult);
        }

        [Fact]
        public static void LockExpression()
        {
            dynamic d = new GenericClassWithNewConstraint<EmptyClass>();
            int result = 1;
            lock (d.Method_ReturnsT())
            {
                result = 0;
            }
            Assert.Equal(0, result);
        }

        [Fact]
        public static void SwitchStatementList()
        {
            dynamic d1 = new GenericClassWithNewConstraint<EmptyClass>();
            dynamic d2 = new GenericClassWithAnotherTypeConstraint<int, int>();
            EmptyClass result = new EmptyClass();
            int result2 = -1;
            switch ((int)d2.Method_ReturnsU(d1)) // 0
            {
                case 0:
                    result = (EmptyClass)d1.Method_ReturnsDynamic(result); // called
                    break;
                default:
                    result2 = (int)d2.Method_ReturnsDynamic(0, 0, d2); // not called
                    break;
            }

            Assert.IsType<EmptyClass>(result);
            Assert.Equal(-1, result2);
        }
    }

    public class ClassWithRegularAndStaticMethods
    {
        public int TestMethod(int i) => i == default(int) ? 0 : 1;
        public static int TestMethod(string s) => s == null ? 0 : 1;

        [Fact]
        public static void ArgumentsOfMethodInvocation()
        {
            ClassWithRegularAndStaticMethods t = new ClassWithRegularAndStaticMethods();
            dynamic mc1 = new GenericClass<int>();
            int result1 = t.TestMethod((int)mc1.Method_ReturnsT());
            dynamic mc2 = new GenericClass<string>();
            int result2 = TestMethod((string)mc2.Method_ReturnsT());
            Assert.Equal(0, result1);
            Assert.Equal(0, result2);
        }
    }

    public class ClassWithStaticVariables
    {
        private static dynamic s_mc = new GenericClass<string>();
        private static float s_loc = (float)s_mc.Method_ReturnsFloat(null, 1);

        [Fact]
        public static void StaticVariableInitialization()
        {
            Assert.Equal(3.4f, s_loc);
        }
    }

    public class ClassWithGenericMethod
    {
        public T TestMethod<T>(ref T t1, T t2)
        {
            dynamic d = new GenericClass<T>();
            return (T)d.Method_ReturnsT(ref t1, t2);
        }

        [Fact]
        public static void GenericMethodBody()
        {
            ClassWithGenericMethod t = new ClassWithGenericMethod();
            string s1 = "";
            string s2 = "";
            string result = t.TestMethod(ref s1, s2);
            Assert.Null(result);
            Assert.Null(s1);
        }
    }

    public static class Extension
    {
        public static ClassWithExtensionMethod ExReturnTest(this int t)
        {
            dynamic d = new GenericClass<int>();
            float x = 3.3f;
            int i = -1;
            return new ClassWithExtensionMethod() { Field = t + (int)d.Method_ReturnsT(x, i) };
        }
    }

    public class ClassWithExtensionMethod
    {
        public int Field = 10;

        [Fact]
        public static void ExtensionMethodBody()
        {
            ClassWithExtensionMethod t = 1.ExReturnTest();
            Assert.Equal(1, t.Field);
        }
    }

    public class ClassWithStaticConstructor : EmptyInterface
    {
        private static decimal s_dec = 0M;
        private static float s_result = 0f;
        static ClassWithStaticConstructor()
        {
            dynamic d = new GenericClass<EmptyInterface>();
            s_result = (float)d.Method_ReturnsFloat(new ClassWithStaticConstructor(), d, ref s_dec);
        }

        [Fact]
        public static void StaticConstructor()
        {
            Assert.Equal(3M, s_dec);
            Assert.Equal(3.4f, s_result);
        }
    }

    public class ClassWithNestedClass
    {
        private class NestedClass
        {
            public int Field1;
            public int Field2;
        }

        [Fact]
        public void QueryExpression()
        {
            dynamic dy = new GenericClassWithClassConstraint<MyClass>();
            var list = new List<NestedClass>()
            {
                new NestedClass() { Field1 = 1, Field2 = 2 },
                new NestedClass() { Field1 = 2, Field2 = 1 }
            };
            Assert.True(list.Any(p => p.Field1 == (int)dy.Method_ReturnsInt()));
        }
    }

    public class ClassWithStaticField
    {
        public static int i = 2;
    }

    public class GenericClassWithImplicitOperator<T>
    {
        public static implicit operator GenericClassWithImplicitOperator<List<string>>(GenericClassWithImplicitOperator<T> x) => new GenericClassWithImplicitOperator<List<string>>();
    }

    public class ClassWithStaticMethodsWithImplicitOperator
    {
        public static void Foo<T>(GenericClassWithImplicitOperator<List<T>> x, string y) => ClassWithStaticField.i--;

        public static void Foo<T>(object x, string y) => ClassWithStaticField.i++;

        [Fact]
        public static void ImplicitOperator()
        {
            var x = new GenericClassWithImplicitOperator<Action<object>>();
            Foo<string>(x, "");
            Foo<string>(x, (dynamic)"");
            Assert.Equal(0, ClassWithStaticField.i);
        }
    }
}
