// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Reflection.Tests;
using Xunit;

#pragma warning disable 0414

namespace System.Reflection.Tests1
{
    public class MethodInfoTests
    {
        [Fact]
        public void Attributes()
        {
            MethodInfo methodInfo = GetMethod(typeof(SubClass), "ReturnVoidMethod");
            MethodAttributes attributes = methodInfo.Attributes;
            Assert.NotNull(attributes);
        }

        [Fact]
        public void CallingConvention()
        {
            MethodInfo methodInfo = GetMethod(typeof(SubClass), "ReturnVoidMethod");
            CallingConventions callingConvention = methodInfo.CallingConvention;
            Assert.NotNull(callingConvention);
        }

        [Theory]
        [InlineData(typeof(SubClass), "ReturnVoidMethod", false)]
        [InlineData(typeof(SubClass), "GenericMethod1", true)]
        public void ContainsGenericParameters(Type type, string name, bool expected)
        {
            Assert.Equal(expected, GetMethod(type, name).ContainsGenericParameters);
        }

        [Fact]
        public void CreateDelegate_PublicMethod()
        {
            Type typeTestClass = typeof(BaseClass);
            RunBasicTestsHelper(typeTestClass);
        }

        [Fact]
        public void CreateDelegate_InheritedMethod()
        {
            Type typeTestClass = typeof(BaseClass);
            Type TestSubClassType = typeof(SubClass);
            RunInheritanceTestsHelper(typeTestClass, TestSubClassType);
        }

        [Fact]
        public void CreateDelegate_GenericMethod()
        {
            Type typeGenericClassString = typeof(GenericClass<string>);
            RunGenericTestsHelper(typeGenericClassString);
        }

        [Fact]
        public void CreateDelegate_ValueTypeParameters()
        {
            MethodInfo miPublicStructMethod = GetMethod(typeof(BaseClass), "PublicStructMethod");
            BaseClass testClass = new BaseClass();

            Delegate methodDelegate = miPublicStructMethod.CreateDelegate(typeof(Delegate_DateTime_Str));
            object returnValue = ((Delegate_DateTime_Str)methodDelegate).DynamicInvoke(new object[] { testClass, null });
            Assert.Equal(testClass.PublicStructMethod(new DateTime()), returnValue);
        }

        public void RunBasicTestsHelper(Type typeTestClass)
        {
            BaseClass baseClass = (BaseClass)Activator.CreateInstance(typeTestClass);
            MethodInfo virtualMethodInfo = GetMethod(typeTestClass, "VirtualMethod");
            MethodInfo privateInstanceMethodInfo = GetMethod(typeTestClass, "PrivateInstanceMethod");
            MethodInfo publicStaticMethodInfo = GetMethod(typeTestClass, "PublicStaticMethod");

            Delegate methodDelegate = virtualMethodInfo.CreateDelegate(typeof(Delegate_TC_Int));
            object returnValue = ((Delegate_TC_Int)methodDelegate).DynamicInvoke(new object[] { baseClass });
            Assert.Equal(baseClass.VirtualMethod(), returnValue);

            methodDelegate = privateInstanceMethodInfo.CreateDelegate(typeof(Delegate_TC_Int));
            returnValue = ((Delegate_TC_Int)methodDelegate).DynamicInvoke(new object[] { baseClass });
            Assert.Equal(21, returnValue);

            methodDelegate = virtualMethodInfo.CreateDelegate(typeof(Delegate_Void_Int), baseClass);
            returnValue = ((Delegate_Void_Int)methodDelegate).DynamicInvoke(null);
            Assert.Equal(baseClass.VirtualMethod(), returnValue);

            methodDelegate = publicStaticMethodInfo.CreateDelegate(typeof(Delegate_Str_Str));
            returnValue = ((Delegate_Str_Str)methodDelegate).DynamicInvoke(new object[] { "85" });
            Assert.Equal("85", returnValue);

            methodDelegate = publicStaticMethodInfo.CreateDelegate(typeof(Delegate_Void_Str), "93");
            returnValue = ((Delegate_Void_Str)methodDelegate).DynamicInvoke(null);
            Assert.Equal("93", returnValue);
        }

        public void RunInheritanceTestsHelper(Type typeTestClass, Type TestSubClassType)
        {
            SubClass testSubClass = (SubClass)Activator.CreateInstance(TestSubClassType);
            BaseClass testClass = (BaseClass)Activator.CreateInstance(typeTestClass);
            MethodInfo virtualMethodInfo = GetMethod(typeTestClass, "VirtualMethod");

            Delegate methodDelegate = virtualMethodInfo.CreateDelegate(typeof(Delegate_TC_Int));
            object returnValue = ((Delegate_TC_Int)methodDelegate).DynamicInvoke(new object[] { testSubClass });
            Assert.Equal(testSubClass.VirtualMethod(), returnValue);

            methodDelegate = virtualMethodInfo.CreateDelegate(typeof(Delegate_Void_Int), testSubClass);
            returnValue = ((Delegate_Void_Int)methodDelegate).DynamicInvoke();
            Assert.Equal(testSubClass.VirtualMethod(), returnValue);
        }

        public void RunGenericTestsHelper(Type typeGenericClassString)
        {
            GenericClass<string> genericClass = (GenericClass<string>)Activator.CreateInstance(typeGenericClassString);

            MethodInfo miMethod1String = GetMethod(typeGenericClassString, "GenericMethod1");
            MethodInfo miMethod2String = GetMethod(typeGenericClassString, "GenericMethod3");
            MethodInfo miMethod2IntGeneric = miMethod2String.MakeGenericMethod(new Type[] { typeof(int) });
            MethodInfo miMethod2StringGeneric = miMethod2String.MakeGenericMethod(new Type[] { typeof(string) });

            Delegate methodDelegate = miMethod1String.CreateDelegate(typeof(Delegate_GC_T_T<string>));
            object returnValue = ((Delegate_GC_T_T<string>)methodDelegate).DynamicInvoke(new object[] { genericClass, "TestGeneric" });
            Assert.Equal(genericClass.GenericMethod1("TestGeneric"), returnValue);

            methodDelegate = miMethod1String.CreateDelegate(typeof(Delegate_T_T<string>), genericClass);
            returnValue = ((Delegate_T_T<string>)methodDelegate).DynamicInvoke(new object[] { "TestGeneric" });
            Assert.Equal(genericClass.GenericMethod1("TestGeneric"), returnValue);

            methodDelegate = miMethod2IntGeneric.CreateDelegate(typeof(Delegate_T_T<int>));
            returnValue = ((Delegate_T_T<int>)methodDelegate).DynamicInvoke(new object[] { 58 });
            Assert.Equal(58, returnValue);

            methodDelegate = miMethod2StringGeneric.CreateDelegate(typeof(Delegate_Void_T<string>), "firstArg");
            returnValue = ((Delegate_Void_T<string>)methodDelegate).DynamicInvoke();
            Assert.Equal("firstArg", returnValue);
        }

        public static IEnumerable<object[]> CreateDelegate_Invalid_TestData()
        {
            yield return new object[] { typeof(BaseClass), "VirtualMethod", null, typeof(ArgumentNullException) }; // DelegateType is null
            yield return new object[] { typeof(BaseClass), "VirtualMethod", typeof(Delegate_Void_Int), typeof(ArgumentException) }; // DelegateType is incorrect
        }

        [Theory]
        [MemberData("CreateDelegate_Invalid_TestData")]
        public void CreateDelegate_Invalid(Type type, string name, Type delegateType, Type exceptionType)
        {
            MethodInfo methodInfo = GetMethod(type, name);
            Assert.Throws(exceptionType, () => methodInfo.CreateDelegate(delegateType));
        }

        public static IEnumerable<object[]> CreateDelegate_Target_Invalid_TestData()
        {
            yield return new object[] { typeof(BaseClass), "VirtualMethod", null, new BaseClass(), typeof(ArgumentNullException) }; // DelegateType is null
            yield return new object[] { typeof(BaseClass), "VirtualMethod", typeof(Delegate_TC_Int), new BaseClass(), typeof(ArgumentException) }; // DelegateType is incorrect
            yield return new object[] { typeof(BaseClass), "VirtualMethod", typeof(Delegate_Void_Int), new DummyClass(), typeof(ArgumentException) }; // Target is incorrect
            yield return new object[] { typeof(BaseClass), "VirtualMethod", typeof(Delegate_Void_Str), new DummyClass(), typeof(ArgumentException) }; // Target is incorrect
        }

        [Theory]
        [MemberData("CreateDelegate_Target_Invalid_TestData")]
        public void CreateDelegate_Target_Invalid(Type type, string name, Type delegateType, object target, Type exceptionType)
        {
            MethodInfo methodInfo = GetMethod(type, name);
            Assert.Throws(exceptionType, () => methodInfo.CreateDelegate(delegateType, target));
        }

        [Theory]
        [InlineData(typeof(Int32Attr))]
        [InlineData(typeof(Int64Attr))]
        [InlineData(typeof(StringAttr))]
        [InlineData(typeof(EnumAttr))]
        [InlineData(typeof(TypeAttr))]
        [InlineData(typeof(Attr))]
        public void CustomAttributes(Type type)
        {
            MethodInfo methodInfo = GetMethod(typeof(SubClass), "MethodWithAttributes");
            IEnumerable<CustomAttributeData> customAttrs = methodInfo.CustomAttributes;
            bool result = customAttrs.Any(customAttribute => customAttribute.AttributeType.Equals(type));
            Assert.True(result, string.Format("Did not find custom attribute of type {0}.", type));
        }

        [Theory]
        [InlineData(typeof(SubClass), "ReturnStringMethod", typeof(SubClass), "ReturnStringMethod", true)]
        [InlineData(typeof(SubClass), "ReturnStringMethod", typeof(SubClass), "ReturnIntMethod", false)]
        [InlineData(typeof(SubClass), "GenericMethod1", typeof(GenericClass<>), "GenericMethod1", false)]
        [InlineData(typeof(SubClass), "GenericMethod2", typeof(GenericClass<string>), "GenericMethod2", false)]
        public void Equals(Type type1, string name1, Type type2, string name2, bool expected)
        {
            MethodInfo methodInfo1 = GetMethod(type1, name1);
            MethodInfo methodInfo2 = GetMethod(type2, name2);
            Assert.Equal(expected, methodInfo1.Equals(methodInfo2));
        }

        [Theory]
        [InlineData(typeof(SubClass), "GenericMethod1", 1)]
        [InlineData(typeof(SubClass), "GenericMethod2", 2)]
        [InlineData(typeof(SubClass), "ReturnLongMethod1", 0)]
        public void GetGenericArguments(Type type, string name, int expectedCount)
        {
            MethodInfo methodInfo = GetMethod(type, name);
            Type[] genericArguments = methodInfo.GetGenericArguments();
            Assert.Equal(expectedCount, genericArguments.Length);
        }

        [Fact]
        public void GetGenericMethodDefinition_Invalid()
        {
            Assert.Throws<InvalidOperationException>(() => GetMethod(typeof(SubClass), "ReturnIntMethod").GetGenericMethodDefinition()); // Method is non generic
        }

        [Fact]
        public void GetHashCodeTest()
        {
            MethodInfo methodInfo = GetMethod(typeof(SubClass), "ReturnIntMethod");
            Assert.NotEqual(0, methodInfo.GetHashCode());
        }

        [Theory]
        [InlineData(typeof(SubClass), "ReturnLongMethod2", new string[] { "i", "l" })]
        [InlineData(typeof(SubClass), "StringArrayMethod", new string[] { "strArray" })]
        [InlineData(typeof(Interlocked2), "Increment", new string[] { "location" })]
        [InlineData(typeof(Interlocked2), "Decrement", new string[] { "location" })]
        [InlineData(typeof(Interlocked2), "Exchange", new string[] { "location1", "value" })]
        [InlineData(typeof(Interlocked2), "CompareExchange", new string[] { "location1", "value", "comparand" })]
        public void GetParameters(Type type, string name, string[] expectedParameterNames)
        {
            MethodInfo method = GetMethod(type, name);
            ParameterInfo[] parameters = method.GetParameters();

            Assert.Equal(expectedParameterNames.Length, parameters.Length);
            for (int i = 0; i < parameters.Length; i++)
            {
                Assert.Equal(parameters[i].Name, expectedParameterNames[i]);
            }
        }

        [Fact]
        public void GetParameters_IsDeepCopy()
        {
            MethodInfo method = GetMethod(typeof(SubClass), "ReturnLongMethod2");
            ParameterInfo[] parameters = method.GetParameters();

            if (parameters.Length > 1)
                parameters[0] = null;

            // If GetParameters is a deep copy, then this change
            // should not affect another call to GetParameters()
            ParameterInfo[] parameters2 = method.GetParameters();
            for (int i = 0; i < parameters2.Length; i++)
            {
                Assert.NotNull(parameters2[i]);
            }
        }

        public static IEnumerable<object[]> Invoke_TestData()
        {
            yield return new object[] { typeof(BaseClass), "VirtualMethod", new BaseClass(), null, 0 };
            yield return new object[] { typeof(BaseClass), "VirtualMethod", new SubClass(), null, 1 };
            yield return new object[] { typeof(SubClass), "ReturnIntMethod", new SubClass(), null, 3 };
            yield return new object[] { typeof(SubClass), "ReturnLongMethod1", new SubClass(), null, long.MaxValue };

            // Invoke a method that requires Reflection to box a primitive integer for the invoked method's param
            yield return new object[] { typeof(SubClass), "ReturnStringMethod", new SubClass(), new object[] { 42 }, "42" };
            yield return new object[] { typeof(SubClass), "ReturnLongMethod2", new SubClass(), new object[] { 200, 100000 }, 100200L };

            yield return new object[] { typeof(SubClass), "StaticReturnIntMethod", null, new object[] { 10, 100 }, 110 }; // Static (non null obj)
            yield return new object[] { typeof(SubClass), "StaticReturnIntMethod", new SubClass(), new object[] { 10, 100 }, 110 }; // Static (null obj)
            yield return new object[] { typeof(SubClass), "ReturnEnumMethod", new SubClass(), new object[] { MyEnum.First }, MyEnum.Second }; // Enum
            yield return new object[] { typeof(Interface), "IMethod", new SubClass(), new object[0], 10 }; // Interface method
            yield return new object[] { typeof(Interface), "IMethodNew", new SubClass(), new object[0], 20 }; // Interface method (marked as new)

            yield return new object[] { typeof(SubClass), "ReturnIntPtrMethod", new SubClass(), null, (IntPtr)200 }; // Returns IntPtr
        }

        [Theory]
        [MemberData("Invoke_TestData")]
        public void Invoke(Type type, string name, object obj, object[] parameters, object expectedResult)
        {
            MethodInfo methodInfo = GetMethod(type, name);
            object result = methodInfo.Invoke(obj, parameters);
            Assert.True(result.Equals(expectedResult), string.Format("{0}.Invoke did not retun the correct result. Expected {0}, got {1})", methodInfo, expectedResult, result));
        }
        
        [Fact]
        public void Invoke_ArrayReturnType()
        {
            MethodInfo mi = GetMethod(typeof(SubClass), "ReturnArrayMethod");
            int[] result = (int[])mi.Invoke(new SubClass(), null);
            Assert.Equal(new int[] { 2, 3, 5, 7, 11 }, result);
        }

        [Theory]
        [InlineData(typeof(SubClass), "ReturnVoidMethod", false)]
        [InlineData(typeof(AbstractClass), "AbstractMethod", true)]
        public void IsAbstract(Type type, string name, bool expected)
        {
            Assert.Equal(expected, GetMethod(type, name).IsAbstract);
        }

        [Theory]
        [InlineData(typeof(SubClass), "ReturnVoidMethod", false)]
        public void IsAssembly(Type type, string name, bool expected)
        {
            Assert.Equal(expected, GetMethod(type, name).IsAssembly);
        }

        [Theory]
        [InlineData(typeof(SubClass), "ReturnVoidMethod", false)]
        public void IsConstructor(Type type, string name, bool expected)
        {
            Assert.Equal(expected, GetMethod(type, name).IsConstructor);
        }

        [Theory]
        [InlineData(typeof(SubClass), "ReturnVoidMethod", false)]
        public void IsFamily(Type type, string name, bool expected)
        {
            Assert.Equal(expected, GetMethod(type, name).IsFamily);
        }

        [Theory]
        [InlineData(typeof(SubClass), "ReturnVoidMethod", false)]
        public void IsFamilyAndAssembly(Type type, string name, bool expected)
        {
            Assert.Equal(expected, GetMethod(type, name).IsFamilyAndAssembly);
        }

        [Theory]
        [InlineData(typeof(SubClass), "ReturnVoidMethod", false)]
        public void IsFamilyOrAssembly(Type type, string name, bool expected)
        {
            Assert.Equal(expected, GetMethod(type, name).IsFamilyOrAssembly);
        }

        [Theory]
        [InlineData(typeof(SubClass), "ReturnVoidMethod", false)]
        [InlineData(typeof(SubClassAbstract), "VirtualMethod", true)]
        public void IsFinal(Type type, string name, bool expected)
        {
            Assert.Equal(expected, GetMethod(type, name).IsFinal);
        }

        [Theory]
        [InlineData(typeof(SubClass), "StaticReturnIntMethod", false)]
        public void IsGenericMethodDefinition(Type type, string name, bool expected)
        {
            Assert.Equal(expected, GetMethod(type, name).IsGenericMethodDefinition);
        }

        [Theory]
        [InlineData(typeof(SubClassAbstract), "AbstractMethod", true)]
        public void IsHideBySig(Type type, string name, bool expected)
        {
            Assert.Equal(expected, GetMethod(type, name).IsHideBySig);
        }

        [Theory]
        [InlineData(typeof(SubClass), "ReturnVoidMethod", false)]
        public void IsPrivate(Type type, string name, bool expected)
        {
            Assert.Equal(expected, GetMethod(type, name).IsPrivate);
        }

        [Theory]
        [InlineData(typeof(SubClass), "ReturnVoidMethod", true)]
        public void IsPublic(Type type, string name, bool expected)
        {
            Assert.Equal(expected, GetMethod(type, name).IsPublic);
        }

        [Theory]
        [InlineData(typeof(SubClass), "ReturnVoidMethod", false)]
        public void IsSpecialName(Type type, string name, bool expected)
        {
            Assert.Equal(expected, GetMethod(type, name).IsSpecialName);
        }

        [Theory]
        [InlineData(typeof(SubClass), "ReturnVoidMethod", false)]
        [InlineData(typeof(SubClass), "StaticReturnIntMethod", true)]
        public void IsStatic(Type type, string name, bool expected)
        {
            Assert.Equal(expected, GetMethod(type, name).IsStatic);
        }

        [Theory]
        [InlineData(typeof(SubClass), "ReturnVoidMethod", false)]
        [InlineData(typeof(SubClass), "VirtualReturnBoolMethod", true)]
        public void IsVirtual(Type type, string name, bool expected)
        {
            Assert.Equal(expected, GetMethod(type, name).IsVirtual);
        }

        [Theory]
        [InlineData(typeof(SubClass), "GenericMethod1", new Type[] { typeof(int) })]
        [InlineData(typeof(SubClass), "GenericMethod2", new Type[] { typeof(string), typeof(int) })]
        public void MakeGenericMethod(Type type, string name, Type[] typeArguments)
        {
            MethodInfo methodInfo = GetMethod(type, name);
            MethodInfo genericMethodInfo = methodInfo.MakeGenericMethod(typeArguments);
            Assert.True(genericMethodInfo.IsGenericMethod);
            Assert.False(genericMethodInfo.IsGenericMethodDefinition);

            MethodInfo genericMethodDefinition = genericMethodInfo.GetGenericMethodDefinition();
            Assert.Equal(methodInfo, genericMethodDefinition);
            Assert.True(genericMethodDefinition.IsGenericMethod);
            Assert.True(genericMethodDefinition.IsGenericMethodDefinition);
        }

        [Fact]
        public void MakeGenericMethod_Invalid()
        {
            Assert.Throws<ArgumentNullException>(() => GetMethod(typeof(SubClass), "GenericMethod1").MakeGenericMethod(null)); // Type arguments is null
            Assert.Throws<ArgumentNullException>(() => GetMethod(typeof(SubClass), "GenericMethod2").MakeGenericMethod(new Type[] { typeof(string), null })); // Type arguments has null Type
            Assert.Throws<InvalidOperationException>(() => GetMethod(typeof(SubClass), "ReturnIntMethod").MakeGenericMethod(new Type[] { typeof(int) })); // Method is non generic

            // Number of typeArguments does not match
            Assert.Throws<ArgumentException>(() => GetMethod(typeof(SubClass), "GenericMethod1").MakeGenericMethod(new Type[0]));
            Assert.Throws<ArgumentException>(() => GetMethod(typeof(SubClass), "GenericMethod1").MakeGenericMethod(new Type[] { typeof(string), typeof(int) }));
            Assert.Throws<ArgumentException>(() => GetMethod(typeof(SubClass), "GenericMethod2").MakeGenericMethod(new Type[] { typeof(int) }));
        }

        [Theory]
        [InlineData(typeof(SubClass), "ReturnLongMethod1")]
        [InlineData(typeof(SubClass), "ReturnLongMethod2")]
        [InlineData(typeof(SubClass), "StringArrayMethod")]
        [InlineData(typeof(Interlocked2), "Increment")]
        [InlineData(typeof(Interlocked2), "Decrement")]
        [InlineData(typeof(Interlocked2), "Exchange")]
        [InlineData(typeof(Interlocked2), "CompareExchange")]
        public void Name(Type type, string name)
        {
            MethodInfo mi = GetMethod(type, name);
            Assert.Equal(name, mi.Name);
        }

        [Theory]
        [InlineData(typeof(SubClass), "StaticReturnIntMethod", typeof(int))]
        [InlineData(typeof(SubClass), "ReturnVoidMethod", typeof(void))]
        [InlineData(typeof(SubClass), "ReturnIntMethod", typeof(int))]
        [InlineData(typeof(SubClass), "ReturnStringMethod", typeof(string))]
        [InlineData(typeof(SubClass), "VirtualReturnStringArrayMethod", typeof(string[]))]
        [InlineData(typeof(SubClass), "VirtualReturnBoolMethod", typeof(bool))]
        public void ReturnType_ReturnParameter(Type type, string name, Type expected)
        {
            MethodInfo methodInfo = GetMethod(type, name);
            Assert.Equal(expected, methodInfo.ReturnType);

            Assert.Equal(methodInfo.ReturnType, methodInfo.ReturnParameter.ParameterType);
            Assert.Null(methodInfo.ReturnParameter.Name);
            Assert.Equal(-1, methodInfo.ReturnParameter.Position);
        }

        [Theory]
        [InlineData(typeof(SubClass), "ReturnLongMethod1", "Int64 ReturnLongMethod1()")]
        [InlineData(typeof(SubClass), "ReturnLongMethod2", "Int64 ReturnLongMethod2(Int32, Int64)")]
        [InlineData(typeof(SubClass), "StringArrayMethod", "Void StringArrayMethod(System.String[])")]
        [InlineData(typeof(SubClass), "ReturnVoidMethod", "Void ReturnVoidMethod(System.DateTime)")]
        [InlineData(typeof(SubClass), "GenericMethod2", "Void GenericMethod2[T,U](T, U)")]
        [InlineData(typeof(Interlocked2), "Increment", "Int32 Increment(Int32 ByRef)")]
        [InlineData(typeof(Interlocked2), "Decrement", "Int32 Decrement(Int32 ByRef)")]
        [InlineData(typeof(Interlocked2), "Exchange", "Int32 Exchange(Int32 ByRef, Int32)")]
        [InlineData(typeof(Interlocked2), "CompareExchange", "Int32 CompareExchange(Int32 ByRef, Int32, Int32)")]
        [InlineData(typeof(GenericClass<>), "GenericMethod1", "T GenericMethod1(T)")]
        [InlineData(typeof(GenericClass<>), "GenericMethod2", "T GenericMethod2[S](S, T, System.String)")]
        [InlineData(typeof(GenericClass<string>), "GenericMethod1", "System.String GenericMethod1(System.String)")]
        [InlineData(typeof(GenericClass<string>), "GenericMethod2", "System.String GenericMethod2[S](S, System.String, System.String)")]
        public void ToString(Type type, string name, string expected)
        {
            MethodInfo methodInfo = GetMethod(type, name);
            Assert.Equal(expected, methodInfo.ToString());
        }

        public static IEnumerable<object[]> ToString_TestData()
        {
            MethodInfo genericMethodInfo = GetMethod(typeof(GenericClass<string>), "GenericMethod2").MakeGenericMethod(new Type[] { typeof(DateTime) });
            yield return new object[] { genericMethodInfo, "System.String GenericMethod2[DateTime](System.DateTime, System.String, System.String)" };
        }

        [Theory]
        [MemberData("ToString_TestData")]
        public void ToString(MethodInfo methodInfo, string expected)
        {
            Assert.Equal(expected, methodInfo.ToString());
        }

        private static MethodInfo GetMethod(Type type, string name)
        {
            return type.GetTypeInfo().DeclaredMethods.FirstOrDefault(methodInfo => methodInfo.Name.Equals(name));
        }
    }

    public interface Interface
    {
        int IMethod();
        int IMethodNew();
    }
    
    public class BaseClass : Interface
    {
        public int IMethod() { return 10; }
        public int IMethodNew() { return 20; }

        public virtual int VirtualMethod() { return 0; }
        private int PrivateInstanceMethod() { return 21; }
        public static string PublicStaticMethod(string x) { return x; }
        public string PublicStructMethod(DateTime dt) { return dt.ToString(); }
    }

    public class SubClass : BaseClass
    {
        public static int StaticReturnIntMethod(int i1, int i2) { return i1 + i2; }
        public static void StaticGenericMethod<T>(T t) { }

        public new int IMethodNew() { return 200; }

        public override int VirtualMethod() { return 1; }

        public MyEnum ReturnEnumMethod(MyEnum myenum) { return myenum == MyEnum.First ? MyEnum.Second : MyEnum.First; }
        
        public string ReturnStringMethod(object obj) { return obj.ToString(); }
        public void ReturnVoidMethod(DateTime dt) { }
        public int ReturnIntMethod() { return 3; }
        public long ReturnLongMethod1() { return long.MaxValue; }
        public long ReturnLongMethod2(int i, long l) { return i + l; }
        public virtual string[] VirtualReturnStringArrayMethod() { return new string[0]; }
        public virtual bool VirtualReturnBoolMethod() { return true; }

        public string Method2<T, S>(string t2, T t1, S t3) { return ""; }

        public IntPtr ReturnIntPtrMethod() { return new IntPtr(200); }
        public int[] ReturnArrayMethod() { return new int[] { 2, 3, 5, 7, 11 }; }

        public void GenericMethod1<T>(T t) { }
        public void GenericMethod2<T, U>(T t, U u) { }

        public void StringArrayMethod(string[] strArray) { }

        [Attr(77, name = "AttrSimple"),
        Int32Attr(77, name = "Int32AttrSimple"),
        Int64Attr(77, name = "Int64AttrSimple"),
        StringAttr("hello", name = "StringAttrSimple"),
        EnumAttr(MyEnum.First, name = "EnumAttrSimple"),
        TypeAttr(typeof(object), name = "TypeAttrSimple")]
        public void MethodWithAttributes() { }
    }

    // Class For Reflection Metadata
    public class Interlocked2
    {
        public static int Increment(ref int location) { return 0; }
        public static int Decrement(ref int location) { return 0; }
        public static int Exchange(ref int location1, int value) { return 0; }
        public static int CompareExchange(ref int location1, int value, int comparand) { return 0; }

        public static float Exchange(ref float location1, float value) { return 0; }
        public static float CompareExchange(ref float location1, float value, float comparand) { return 0; }

        public static object Exchange(ref object location1, object value) { return null; }
        public static object CompareExchange(ref object location1, object value, object comparand) { return null; }
    }

    public class GenericClass<T>
    {
        public T GenericMethod1(T t) { return t; }
        public T GenericMethod2<S>(S s1, T t, string s2) { return t; }
        public static S GenericMethod3<S>(S s) { return s; }
    }

    public abstract class AbstractClass
    {
        public abstract void AbstractMethod();
        public virtual void VirtualMethod() { }
    }

    public class SubClassAbstract : AbstractClass
    {
        public sealed override void VirtualMethod() { }
        public override void AbstractMethod() { }
    }

    public delegate int Delegate_TC_Int(BaseClass tc);
    public delegate int Delegate_Void_Int();
    public delegate string Delegate_Str_Str(string x);
    public delegate string Delegate_Void_Str();
    public delegate string Delegate_DateTime_Str(BaseClass tc, DateTime dt);

    public delegate T Delegate_GC_T_T<T>(GenericClass<T> gc, T x);
    public delegate T Delegate_T_T<T>(T x);
    public delegate T Delegate_Void_T<T>();

    public class DummyClass { }
}
