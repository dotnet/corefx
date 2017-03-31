// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using Xunit;

namespace System.Runtime.Serialization.Formatters.Tests
{
    public class FormatterServicesTests
    {
        [Fact]
        public void CheckTypeSecurity_Nop()
        {
            FormatterServices.CheckTypeSecurity(typeof(int), TypeFilterLevel.Full);
            FormatterServices.CheckTypeSecurity(typeof(int), TypeFilterLevel.Low);
        }

        [Fact]
        public void GetSerializableMembers_InvalidArguments_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>("type", () => FormatterServices.GetSerializableMembers(null));
        }

        [Fact]
        public void GetSerializableMembers_Interface()
        {
            Assert.Equal<MemberInfo>(new MemberInfo[0], FormatterServices.GetSerializableMembers(typeof(IDisposable)));
        }

        [Fact]
        public void GetUninitializedObject_NullType_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("type", () => FormatterServices.GetUninitializedObject(null));
            Assert.Throws<ArgumentNullException>("type", () => FormatterServices.GetSafeUninitializedObject(null));
        }

        [Fact]
        public void GetUninitializedObject_NonRuntimeType_ThrowsSerializationException()
        {
            Assert.Throws<SerializationException>(() => FormatterServices.GetUninitializedObject(new NonRuntimeType()));
            Assert.Throws<SerializationException>(() => FormatterServices.GetSafeUninitializedObject(new NonRuntimeType()));
        }

        public static IEnumerable<object[]> GetUninitializedObject_NotSupportedType_TestData()
        {
            yield return new object[] { typeof(int[]) };
            yield return new object[] { typeof(int[,]) };
            yield return new object[] { typeof(int).MakePointerType() };
            yield return new object[] { typeof(int).MakeByRefType() };
        }

        [Theory]
        [MemberData(nameof(GetUninitializedObject_NotSupportedType_TestData))]
        public void GetUninitializedObject_NotSupportedType_ThrowsArgumentException(Type type)
        {
            Assert.Throws<ArgumentException>(null, () => FormatterServices.GetUninitializedObject(type));
            Assert.Throws<ArgumentException>(null, () => FormatterServices.GetSafeUninitializedObject(type));
        }

        [Theory]
        [InlineData(typeof(AbstractClass))]
        [InlineData(typeof(StaticClass))]
        [InlineData(typeof(Interface))]
        [InlineData(typeof(Array))]
        public void GetUninitializedObject_AbstractClass_ThrowsMemberAccessException(Type type)
        {
            Assert.Throws<MemberAccessException>(() => FormatterServices.GetUninitializedObject(type));
            Assert.Throws<MemberAccessException>(() => FormatterServices.GetSafeUninitializedObject(type));
        }

        private abstract class AbstractClass { }
        private static class StaticClass { }
        private interface Interface { }

        public static IEnumerable<object[]> GetUninitializedObject_OpenGenericClass_TestData()
        {
            yield return new object[] { typeof(GenericClass<>) };
            yield return new object[] { typeof(GenericClass<>).MakeGenericType(typeof(GenericClass<>)) };
        }

        [Theory]
        [MemberData(nameof(GetUninitializedObject_OpenGenericClass_TestData))]
        public void GetUninitializedObject_OpenGenericClass_ThrowsMemberAccessException(Type type)
        {
            Assert.Throws<MemberAccessException>(() => FormatterServices.GetUninitializedObject(type));
            Assert.Throws<MemberAccessException>(() => FormatterServices.GetSafeUninitializedObject(type));
        }

        public interface IGenericClass
        {
            int Value { get; set; }
        }

        public class GenericClass<T> : IGenericClass
        {
            public int Value { get; set; }
        }

        public static IEnumerable<object[]> GetUninitializedObject_ByRefLikeType_TestData()
        {
            yield return new object[] { typeof(ArgIterator) };
            yield return new object[] { typeof(RuntimeArgumentHandle) };
            yield return new object[] { typeof(TypedReference) };

            yield return new object[] { typeof(Span<int>) };
            yield return new object[] { typeof(ReadOnlySpan<int>) };
        }

        public static IEnumerable<object[]> GetUninitializedObject_ByRefLikeType_NetCore_TestData()
        {
            yield return new object[] { Type.GetType("System.ByReference`1[System.Int32]") };
        }

        [Theory]
        [MemberData(nameof(GetUninitializedObject_ByRefLikeType_TestData))]
        [MemberData(nameof(GetUninitializedObject_ByRefLikeType_NetCore_TestData))]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "The full .NET framework supports GetUninitializedObject for by ref like types")]
        public void GetUninitializedObject_ByRefLikeType_NetCore_ThrowsNotSupportedException(Type type)
        {
            Assert.Throws<NotSupportedException>(() => FormatterServices.GetUninitializedObject(type));
            Assert.Throws<NotSupportedException>(() => FormatterServices.GetSafeUninitializedObject(type));
        }

        [Theory]
        [MemberData(nameof(GetUninitializedObject_ByRefLikeType_TestData))]
        [SkipOnTargetFramework(~TargetFrameworkMonikers.NetFramework, "The coreclr doesn't support GetUninitializedObject for by ref like types")]
        public void GetUninitializedObject_ByRefLikeType_Netfx_ThrowsNotSupportedException(Type type)
        {
            Assert.NotNull(FormatterServices.GetUninitializedObject(type));
            Assert.NotNull(FormatterServices.GetSafeUninitializedObject(type));
        }

        [Fact]
        [SkipOnTargetFramework(~TargetFrameworkMonikers.Netcoreapp, "The coreclr doesn't support GetUninitializedObject for shared generic instances")]
        public void GetUninitializedObject_SharedGenericInstance_NetCore_ThrowsNotSupportedException()
        {
            Type canonType = Type.GetType("System.__Canon");
            Assert.NotNull(canonType);
            Type sharedGenericInstance = typeof(GenericClass<>).MakeGenericType(canonType);
            Assert.Throws<NotSupportedException>(() => FormatterServices.GetUninitializedObject(sharedGenericInstance));
            Assert.Throws<NotSupportedException>(() => FormatterServices.GetSafeUninitializedObject(sharedGenericInstance));
        }

        [Fact]
        [SkipOnTargetFramework(~TargetFrameworkMonikers.NetFramework, "The full .NET framework partially supports GetUninitializedObject for shared generic instances")]
        public void GetUninitializedObject_SharedGenericInstance_Netfx_InitializesValue()
        {
            Type canonType = Type.GetType("System.__Canon");
            Assert.NotNull(canonType);
            Type sharedGenericInstance = typeof(GenericClass<>).MakeGenericType(canonType);
            Assert.Throws<NotSupportedException>(() => FormatterServices.GetUninitializedObject(sharedGenericInstance));
            Assert.Equal(0, ((IGenericClass)FormatterServices.GetSafeUninitializedObject(sharedGenericInstance)).Value);
        }

        [Fact]
        public void GetUninitializedObject_NullableType_InitializesValue()
        {
            int? nullableUnsafe = (int?)FormatterServices.GetUninitializedObject(typeof(int?));
            Assert.True(nullableUnsafe.HasValue);
            Assert.Equal(0, nullableUnsafe.Value);

            int? nullableSafe = (int?)FormatterServices.GetSafeUninitializedObject(typeof(int?));
            Assert.True(nullableSafe.HasValue);
            Assert.Equal(0, nullableSafe.Value);
        }

        [Fact]
        public void GetUninitializedObject_COMObject_ThrowsNotSupportedException()
        {
            Type comObjectType = typeof(COMObject);
            Assert.True(comObjectType.IsCOMObject);

            Assert.Throws<NotSupportedException>(() => FormatterServices.GetUninitializedObject(typeof(COMObject)));
            Assert.Throws<NotSupportedException>(() => FormatterServices.GetSafeUninitializedObject(typeof(COMObject)));
        }

        [ComImport]
        [Guid("00000000-0000-0000-0000-000000000000")]
        public class COMObject { }

        [Theory]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "The full .NET framework doesn't support GetUninitializedObject for subclasses of ContextBoundObject")]
        public void GetUninitializedObject_ContextBoundObjectSubclass_NetCore_InitializesValue()
        {
            Assert.Equal(0, ((ContextBoundSubclass)FormatterServices.GetUninitializedObject(typeof(ContextBoundSubclass))).Value);
            Assert.Equal(0, ((ContextBoundSubclass)FormatterServices.GetSafeUninitializedObject(typeof(ContextBoundSubclass))).Value);
        }

        [Theory]
        [SkipOnTargetFramework(~TargetFrameworkMonikers.NetFramework, "The coreclr supports GetUninitializedObject for subclasses of ContextBoundObject")]
        public void GetUninitializedObject_ContextBoundObjectSubclass_Netfx_ThrowsNotSupportedException()
        {
            Assert.Throws<NotSupportedException>(() => FormatterServices.GetUninitializedObject(typeof(ContextBoundSubclass)));
            Assert.Throws<NotSupportedException>(() => FormatterServices.GetSafeUninitializedObject(typeof(ContextBoundSubclass)));
        }

        public class ContextBoundSubclass : ContextBoundObject
        {
            public int Value { get; set; }
        }

        [Fact]
        public void GetUninitializedObject_TypeHasDefaultConstructor_DoesNotRunConstructor()
        {
            Assert.Equal(42, new ObjectWithDefaultConstructor().Value);
            Assert.Equal(0, ((ObjectWithDefaultConstructor)FormatterServices.GetUninitializedObject(typeof(ObjectWithDefaultConstructor))).Value);
            Assert.Equal(0, ((ObjectWithDefaultConstructor)FormatterServices.GetSafeUninitializedObject(typeof(ObjectWithDefaultConstructor))).Value);
        }

        private class ObjectWithDefaultConstructor
        {
            public ObjectWithDefaultConstructor()
            {
                Value = 42;
            }

            public int Value;
        }

        [Fact]
        public void GetUninitializedObject_StaticConstructor_CallsStaticConstructor()
        {
            Assert.Equal(2, ((ObjectWithStaticConstructor)FormatterServices.GetUninitializedObject(typeof(ObjectWithStaticConstructor))).GetValue());
        }

        private class ObjectWithStaticConstructor
        {
            private static int s_value = 1;

            static ObjectWithStaticConstructor()
            {
                s_value = 2;
            }

            public int GetValue() => s_value;
        }

        [Fact]
        public void GetUninitializedObject_StaticField_InitializesStaticFields()
        {
            Assert.Equal(1, ((ObjectWithStaticField)FormatterServices.GetUninitializedObject(typeof(ObjectWithStaticField))).GetValue());
        }

        private class ObjectWithStaticField
        {
            private static int s_value = 1;

            public int GetValue() => s_value;
        }

        [Fact]
        public void GetUninitializedObject_StaticConstructorThrows_ThrowsTypeInitializationException()
        {
            TypeInitializationException ex = Assert.Throws<TypeInitializationException>(() => FormatterServices.GetUninitializedObject(typeof(StaticConstructorThrows)));
            Assert.IsType<DivideByZeroException>(ex.InnerException);
        }

        private class StaticConstructorThrows
        {
            static StaticConstructorThrows()
            {
                throw new DivideByZeroException();
            }
        }

        [Fact]
        public void GetUninitializedObject_ClassFieldWithDefaultValue_DefaultValueIgnored()
        {
            Assert.Equal(42, new ObjectWithStructDefaultField().Value);
            Assert.Null(((ObjectWithClassDefaultField)FormatterServices.GetUninitializedObject(typeof(ObjectWithClassDefaultField))).Value);
            Assert.Null(((ObjectWithClassDefaultField)FormatterServices.GetSafeUninitializedObject(typeof(ObjectWithClassDefaultField))).Value);
        }

        private class ObjectWithClassDefaultField
        {
            public string Value = "abc";
        }

        [Fact]
        public void GetUninitializedObject_StructFieldWithDefaultValue_DefaultValueIgnored()
        {
            Assert.Equal(42, new ObjectWithStructDefaultField().Value);
            Assert.Equal(0, ((ObjectWithStructDefaultField)FormatterServices.GetUninitializedObject(typeof(ObjectWithStructDefaultField))).Value);
            Assert.Equal(0, ((ObjectWithStructDefaultField)FormatterServices.GetSafeUninitializedObject(typeof(ObjectWithStructDefaultField))).Value);
        }

        private class ObjectWithStructDefaultField
        {
            public int Value = 42;
        }

        [Fact]
        public void PopulateObjectMembers_InvalidArguments_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>("obj", () => FormatterServices.PopulateObjectMembers(null, new MemberInfo[0], new object[0]));
            Assert.Throws<ArgumentNullException>("members", () => FormatterServices.PopulateObjectMembers(new object(), null, new object[0]));
            Assert.Throws<ArgumentNullException>("data", () => FormatterServices.PopulateObjectMembers(new object(), new MemberInfo[0], null));
            Assert.Throws<ArgumentException>(() => FormatterServices.PopulateObjectMembers(new object(), new MemberInfo[1], new object[2]));
            Assert.Throws<ArgumentNullException>("members", () => FormatterServices.PopulateObjectMembers(new object(), new MemberInfo[1], new object[1]));
            Assert.Throws<SerializationException>(() => FormatterServices.PopulateObjectMembers(new object(), new MemberInfo[] { typeof(object).GetMethod("GetHashCode") }, new object[] { new object() }));
        }

        [Fact]
        public void GetObjectData_InvalidArguments_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>("obj", () => FormatterServices.GetObjectData(null, new MemberInfo[0]));
            Assert.Throws<ArgumentNullException>("members", () => FormatterServices.GetObjectData(new object(), null));
            Assert.Throws<ArgumentNullException>("members", () => FormatterServices.GetObjectData(new object(), new MemberInfo[1]));
            Assert.Throws<SerializationException>(() => FormatterServices.GetObjectData(new object(), new MethodInfo[] { typeof(object).GetMethod("GetHashCode") }));
        }

        [Fact]
        public void GetSurrogateForCyclicalReference_InvalidArguments_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>("innerSurrogate", () => FormatterServices.GetSurrogateForCyclicalReference(null));
        }

        [Fact]
        public void GetSurrogateForCyclicalReference_ValidSurrogate_GetsObject()
        {
            var surrogate = new NonSerializablePairSurrogate();
            ISerializationSurrogate newSurrogate = FormatterServices.GetSurrogateForCyclicalReference(surrogate);
            Assert.NotNull(newSurrogate);
            Assert.NotSame(surrogate, newSurrogate);
        }

        [Fact]
        public void GetTypeFromAssembly_InvalidArguments_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>("assem", () => FormatterServices.GetTypeFromAssembly(null, "name"));
            Assert.Null(FormatterServices.GetTypeFromAssembly(GetType().Assembly, Guid.NewGuid().ToString("N"))); // non-existing type doesn't throw
        }
    }
}
