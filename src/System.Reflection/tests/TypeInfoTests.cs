// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Xunit;

#pragma warning disable 0414
#pragma warning disable 0067
#pragma warning disable 3026

namespace System.Reflection.Tests
{
    public class TypeInfoTests
    {
        [Theory]
        [InlineData(typeof(BaseClass))]
        [InlineData(typeof(BaseClass.PublicNestedClass1))]
        public void AsType(Type type)
        {
            TypeInfo typeInfo = type.GetTypeInfo();
            Assert.Equal(type, typeInfo.AsType());
        }

        [Theory]
        [InlineData(typeof(SubClass), typeof(BaseClass))]
        [InlineData(typeof(BaseClass), typeof(object))]
        public void BaseType(Type type, Type expected)
        {
            Assert.Equal(expected, type.GetTypeInfo().BaseType);
        }

        [Theory]
        [InlineData(typeof(ClassWithConstraints<,>), true)]
        [InlineData(typeof(BaseClass), false)]
        public void ContainsGenericParameters(Type type, bool expected)
        {
            Assert.Equal(expected, type.GetTypeInfo().ContainsGenericParameters);
        }

        [Theory]
        [InlineData(typeof(BaseClass), "EventPublic", true)]
        [InlineData(typeof(BaseClass), "EventPublicStatic", true)]
        [InlineData(typeof(BaseClass), "NoSuchEvent", false)]
        [InlineData(typeof(BaseClass), "", false)]
        [InlineData(typeof(SubClass), "EventPublicNew", true)]
        [InlineData(typeof(SubClass), "EventPublic", true)]
        [InlineData(typeof(SubClass), "EventPublicStatic", false)]
        public void DeclaredEvents(Type type, string name, bool exists)
        {
            IEnumerable<EventInfo> events = type.GetTypeInfo().DeclaredEvents;
            Assert.Equal(exists, events.Any(eventInfo => eventInfo.Name.Equals(name)));

            EventInfo declaredEventInfo = type.GetTypeInfo().GetDeclaredEvent(name);
            if (exists)
            {
                Assert.Equal(name, declaredEventInfo.Name);
            }
            else
            {
                Assert.Null(declaredEventInfo);
            }
        }

        [Theory]
        [InlineData(typeof(BaseClass), 5)]
        [InlineData(typeof(SubClass), 2)]
        [InlineData(typeof(StaticClass), 0)]
        public void DeclaredConstructors(Type type, int expectedCount)
        {
            ConstructorInfo[] constructors = type.GetTypeInfo().DeclaredConstructors.Where(ctorInfo => !ctorInfo.IsStatic).ToArray();
            Assert.Equal(expectedCount, constructors.Length);
            foreach (ConstructorInfo constructorInfo in constructors)
            {
                Assert.NotNull(constructorInfo);
            }
        }

        [Theory]
        [InlineData(typeof(BaseClass), "staticInt", true)]
        [InlineData(typeof(BaseClass), "subPublicField1", true)]
        [InlineData(typeof(BaseClass), "publicField1", true)]
        [InlineData(typeof(BaseClass), "publicField2", true)]
        [InlineData(typeof(BaseClass), "publicField3", true)]
        [InlineData(typeof(BaseClass), "publicField4", true)]
        [InlineData(typeof(BaseClass), "publicField5", true)]
        [InlineData(typeof(BaseClass), "publicField6", true)]
        [InlineData(typeof(BaseClass), "NoSuchField", false)]
        [InlineData(typeof(BaseClass), "", false)]
        [InlineData(typeof(SubClass), "staticInt", true)]
        [InlineData(typeof(SubClass), "stringArray", true)]
        [InlineData(typeof(SubClass), "publicField1", true)]
        [InlineData(typeof(SubClass), "publicField2", true)]
        [InlineData(typeof(SubClass), "publicField3", true)]
        [InlineData(typeof(SubClass), "publicField4", true)]
        [InlineData(typeof(SubClass), "publicField5", true)]
        [InlineData(typeof(SubClass), "publicField6", true)]
        public void DeclaredFields(Type type, string name, bool exists)
        {
            IEnumerable<FieldInfo> fields = type.GetTypeInfo().DeclaredFields;
            bool result = fields.Any(fieldInfo => fieldInfo.Name.Equals(name));
            Assert.True(exists == result, string.Format("Expected existence of field {0} in type {1} was '{2}' but got '{3}'", name, type, exists, result));

            FieldInfo declaredFieldInfo = type.GetTypeInfo().GetDeclaredField(name);
            if (exists)
            {
                Assert.Equal(name, declaredFieldInfo.Name);
            }
            else
            {
                Assert.Null(declaredFieldInfo);
            }
        }

        [Theory]
        [InlineData(typeof(BaseClass), "PublicNestedClass1", true)]
        [InlineData(typeof(BaseClass), "PublicNestedClass2", true)]
        [InlineData(typeof(BaseClass), "", false)]
        [InlineData(typeof(BaseClass), "NoSuchType", false)]
        [InlineData(typeof(SubClass), "ProtectedNestedClass", false)]
        [InlineData(typeof(SubClass), "InternalNestedClass", false)]
        [InlineData(typeof(SubClass), "PrivateNestedClass", false)]
        [InlineData(typeof(SubClass), "PublicNestedClass1", true)]
        [InlineData(typeof(SubClass), "PublicNestedClass2", false)]
        [InlineData(typeof(SubClass), "PublicNestedClass3", true)]
        [InlineData(typeof(MultiNestClass), "Nest1", true)]
        [InlineData(typeof(MultiNestClass.Nest1), "Nest2", true)]
        [InlineData(typeof(MultiNestClass.Nest1.Nest2), "Nest3", true)]
        private static void DeclaredNestedTypes(Type type, string name, bool exists)
        {
            IEnumerable<TypeInfo> nestedTypes = type.GetTypeInfo().DeclaredNestedTypes;
            Assert.True(exists == nestedTypes.Any(nestedType => nestedType.Name.Equals(name)));

            TypeInfo typeInfo = type.GetTypeInfo().GetDeclaredNestedType(name);
            if (exists)
            {
                Assert.Equal(name, typeInfo.Name);
            }
            else
            {
                Assert.Null(typeInfo);
            }
        }

        [Theory]
        [InlineData(typeof(BaseClass), new string[] { "staticInt", "subPublicField1", "publicField1", "publicField2", "publicField3", "publicField4", "publicField5", "publicField6" })]
        [InlineData(typeof(SubClass), new string[] { "staticInt", "stringArray", "publicField1", "publicField2", "publicField3", "publicField4", "publicField5", "publicField6" })]
        public void DeclaredMembers(Type type, string[] expected)
        {
            IEnumerable<MemberInfo> members = type.GetTypeInfo().DeclaredMembers;
            foreach (string memberName in expected)
            {
                Assert.True(members.Any(memberInfo => memberInfo.Name.Equals(memberName)), string.Format("Did not find member {0} in type {1}", memberName, type));
            }
        }

        [Theory]
        [InlineData(typeof(BaseClass), "PublicBaseMethod1", true)]
        [InlineData(typeof(BaseClass), "PublicMethod1", true)]
        [InlineData(typeof(BaseClass), "PublicMethod2", true)]
        [InlineData(typeof(BaseClass), "PublicMethod3", true)]
        [InlineData(typeof(BaseClass), "PublicMethod2ToOverride", true)]
        [InlineData(typeof(BaseClass), "NoSuchMethod", false)]
        [InlineData(typeof(BaseClass), "", false)]
        [InlineData(typeof(SubClass), "PublicMethod1", true)]
        [InlineData(typeof(SubClass), "PublicMethod2", true)]
        [InlineData(typeof(SubClass), "PublicMethod3", true)]
        [InlineData(typeof(SubClass), "PublicMethod2ToOverride", true)]
        public void DeclaredMethods(Type type, string name, bool exists)
        {
            IEnumerable<MethodInfo> methods = type.GetTypeInfo().DeclaredMethods;
            bool result = methods.Any(methodInfo => methodInfo.Name.Equals(name));
            Assert.True(exists == result, string.Format("Expected existence of method {0} in type {1} was '{2}' but got '{3}'", name, type, exists, result));

            MethodInfo declaredMethodInfo = type.GetTypeInfo().GetDeclaredMethod(name);
            if (exists)
            {
                Assert.Equal(name, declaredMethodInfo.Name);
            }
            else
            {
                Assert.Null(declaredMethodInfo);
            }
        }

        [Theory]
        [InlineData(typeof(BaseClass), "PublicProperty1")]
        [InlineData(typeof(BaseClass), "SubPublicProperty1")]
        [InlineData(typeof(BaseClass), "PublicProperty2")]
        [InlineData(typeof(BaseClass), "PublicProperty3")]
        [InlineData(typeof(SubClass), "PublicProperty1")]
        [InlineData(typeof(SubClass), "PublicProperty2")]
        [InlineData(typeof(SubClass), "PublicProperty3")]
        private static void DeclaredProperties(Type type, string name)
        {
            IEnumerable<PropertyInfo> properties = type.GetTypeInfo().DeclaredProperties;
            Assert.True(properties.Any(property => property.Name.Equals(name)), string.Format("Did not find property {0} in type {1}", name, type));
        }

        [Theory]
        [InlineData(typeof(int), "System.Int32")]
        [InlineData(typeof(string), "System.String")]
        public void FullName(Type type, string expected)
        {
            Assert.Equal(expected, type.GetTypeInfo().FullName);
        }

        [Theory]
        // Enums
        [InlineData(typeof(NonGenericEnum), new string[0], null)]
        // Interfaces
        [InlineData(typeof(NonGenericInterface1), new string[0], null)]
        [InlineData(typeof(GenericInterface1<>), new string[0], null)]
        [InlineData(typeof(GenericInterface1<int>), new string[] { "Int32" }, null)]
        [InlineData(typeof(GenericInterface2<,>), new string[0], null)]
        [InlineData(typeof(GenericInterface2<int, string>), new string[] { "Int32", "String" }, null)]
        [InlineData(typeof(NonGenericStruct), new string[0], null)]
        [InlineData(typeof(GenericStruct1<>), new string[0], null)]
        [InlineData(typeof(GenericStruct1<int>), new string[] { "Int32" }, null)]
        [InlineData(typeof(GenericStruct2<,>), new string[0], null)]
        [InlineData(typeof(GenericStruct2<int, string>), new string[] { "Int32", "String" }, null)]
        // Structs
        [InlineData(typeof(NonGenericStructWithNonGenericInterface1), new string[0], new string[0])]
        [InlineData(typeof(NonGenericStructWithGenericInterface1), new string[0], new string[] { "Int32" })]
        [InlineData(typeof(NonGenericStructWithGenericInterface2), new string[0], new string[] { "Int32", "Int32" })]
        [InlineData(typeof(GenericStructWithGenericInterface1<>), new string[0], new string[] { "T" })]
        [InlineData(typeof(GenericStructWithGenericInterface1<int>), new string[] { "Int32" }, new string[] { "Int32" })]
        [InlineData(typeof(GenericStructWithGenericInterface2<,>), new string[0], new string[] { "T", "U" })]
        [InlineData(typeof(GenericStructWithGenericInterface2<int, string>), new string[] { "Int32", "String" }, new string[] { "Int32", "String" })]
        [InlineData(typeof(GenericStructWithGenericInterface3<>), new string[0], new string[] { "T", "Int32" })]
        [InlineData(typeof(GenericStructWithGenericInterface3<string>), new string[] { "String" }, new string[] { "String", "Int32" })]
        // Classes
        [InlineData(typeof(NonGenericClass), new string[0], null)]
        [InlineData(typeof(GenericClass1<>), new string[0], null)]
        [InlineData(typeof(GenericClass1<int>), new string[] { "Int32" }, null)]
        [InlineData(typeof(GenericClass2<,>), new string[0], null)]
        [InlineData(typeof(GenericClass2<int, string>), new string[] { "Int32", "String" }, null)]
        [InlineData(typeof(NonGenericClassWithNonGenericInterface1), new string[0], new string[0])]
        [InlineData(typeof(NonGenericClassWithGenericInterface), new string[0], new string[] { "Int32" })]
        [InlineData(typeof(NonGenericClassWithGenericSuperClass), new string[0], new string[] { "Int32", "Int32" })]
        [InlineData(typeof(GenericClassWithGenericInterface1<>), new string[0], new string[] { "T" })]
        [InlineData(typeof(GenericClassWithGenericInterface1<int>), new string[] { "Int32" }, new string[] { "Int32" })]
        [InlineData(typeof(GenericClassWithGenericInterface2<,>), new string[0], new string[] { "T", "U" })]
        [InlineData(typeof(GenericClassWithGenericInterface2<int, string>), new string[] { "Int32", "String" }, new string[] { "Int32", "String" })]
        [InlineData(typeof(GenericClassWithGenericSuperClass<>), new string[0], new string[] { "T", "Int32" })]
        [InlineData(typeof(GenericClassWithGenericSuperClass<string>), new string[] { "String" }, new string[] { "String", "Int32" })]
        public void GenericTypeArguments(Type type, string[] expected, string[] baseExpected)
        {
            TypeInfo typeInfo = type.GetTypeInfo();
            Type[] genericTypeArguments = typeInfo.GenericTypeArguments;

            Assert.Equal(expected.Length, genericTypeArguments.Length);
            for (int i = 0; i < genericTypeArguments.Length; i++)
            {
                Assert.Equal(expected[i], genericTypeArguments[i].Name);
            }

            Type baseType = typeInfo.BaseType;
            if (baseType == null || typeInfo.IsEnum)
                return;

            if (baseType == typeof(ValueType) || baseType == typeof(object))
            {
                Type[] interfaces = typeInfo.ImplementedInterfaces.ToArray();
                if (interfaces.Length == 0)
                    return;
                baseType = interfaces[0];
            }

            TypeInfo typeInfoBase = baseType.GetTypeInfo();
            genericTypeArguments = typeInfoBase.GenericTypeArguments;

            Assert.Equal(baseExpected.Length, genericTypeArguments.Length);
            for (int i = 0; i < genericTypeArguments.Length; i++)
            {
                Assert.Equal(baseExpected[i], genericTypeArguments[i].Name);
            }
        }

        [Theory]
        // Enums
        [InlineData(typeof(NonGenericEnum), new string[0], null)]
        // Interfaces
        [InlineData(typeof(NonGenericInterface1), new string[0], null)]
        [InlineData(typeof(GenericInterface1<>), new string[] { "T" }, null)]
        [InlineData(typeof(GenericInterface1<int>), new string[0], null)]
        [InlineData(typeof(GenericInterface2<,>), new string[] { "T", "U" }, null)]
        [InlineData(typeof(GenericInterface2<int, string>), new string[0], null)]
        //Structs
        [InlineData(typeof(NonGenericStruct), new string[0], null)]
        [InlineData(typeof(GenericStruct1<>), new string[] { "T" }, null)]
        [InlineData(typeof(GenericStruct1<int>), new string[0], null)]
        [InlineData(typeof(GenericStruct2<,>), new string[] { "T", "U" }, null)]
        [InlineData(typeof(GenericStruct2<int, string>), new string[0], null)]
        [InlineData(typeof(NonGenericStructWithNonGenericInterface1), new string[0], new string[0])]
        [InlineData(typeof(NonGenericStructWithGenericInterface1), new string[0], new string[0])]
        [InlineData(typeof(NonGenericStructWithGenericInterface2), new string[0], new string[0])]
        [InlineData(typeof(GenericStructWithGenericInterface1<>), new string[] { "T" }, new string[0])]
        [InlineData(typeof(GenericStructWithGenericInterface1<int>), new string[0], new string[0])]
        [InlineData(typeof(GenericStructWithGenericInterface2<,>), new string[] { "T", "U" }, new string[0])]
        [InlineData(typeof(GenericStructWithGenericInterface2<int, string>), new string[0], new string[0])]
        [InlineData(typeof(GenericStructWithGenericInterface3<>), new string[] { "T" }, new string[0])]
        [InlineData(typeof(GenericStructWithGenericInterface3<int>), new string[0], new string[0])]
        // Classes
        [InlineData(typeof(NonGenericClass), new string[0], null)]
        [InlineData(typeof(GenericClass1<>), new string[] { "T" }, null)]
        [InlineData(typeof(GenericClass1<int>), new string[0], null)]
        [InlineData(typeof(GenericClass2<,>), new string[] { "T", "U" }, null)]
        [InlineData(typeof(GenericClass2<int, string>), new string[0], null)]
        [InlineData(typeof(NonGenericClassWithNonGenericInterface1), new string[0], new string[0])]
        [InlineData(typeof(NonGenericClassWithGenericInterface), new string[0], new string[0])]
        [InlineData(typeof(NonGenericClassWithGenericSuperClass), new string[0], new string[0])]
        [InlineData(typeof(GenericClassWithGenericInterface1<>), new string[] { "T" }, new string[0])]
        [InlineData(typeof(GenericClassWithGenericInterface1<int>), new string[0], new string[0])]
        [InlineData(typeof(GenericClassWithGenericInterface2<,>), new string[] { "T", "U" }, new string[0])]
        [InlineData(typeof(GenericClassWithGenericInterface2<int, string>), new string[0], new string[0])]
        [InlineData(typeof(GenericClassWithGenericSuperClass<>), new string[] { "T" }, new string[0])]
        [InlineData(typeof(GenericClassWithGenericSuperClass<int>), new string[0], new string[0])]
        public void GenericTypeParameters(Type type, string[] expected, string[] baseExpected)
        {
            TypeInfo typeInfo = type.GetTypeInfo();
            Type[] genericTypeParameters = typeInfo.GenericTypeParameters;

            Assert.Equal(expected.Length, genericTypeParameters.Length);
            for (int i = 0; i < genericTypeParameters.Length; i++)
            {
                Assert.Equal(expected[i], genericTypeParameters[i].Name);
            }

            Type baseType = typeInfo.BaseType;
            if (baseType == null || typeInfo.IsEnum)
                return;

            if (baseType == typeof(ValueType) || baseType == typeof(object))
            {
                Type[] interfaces = typeInfo.ImplementedInterfaces.ToArray();
                if (interfaces.Length == 0)
                    return;
                baseType = interfaces[0];
            }

            TypeInfo typeInfoBase = baseType.GetTypeInfo();
            genericTypeParameters = typeInfoBase.GenericTypeParameters;

            Assert.Equal(baseExpected.Length, genericTypeParameters.Length);
            for (int i = 0; i < genericTypeParameters.Length; i++)
            {
                Assert.Equal(baseExpected[i], genericTypeParameters[i].Name);
            }
        }

        [Theory]
        [InlineData(new int[] { 1, 2, 3 }, 1)]
        [InlineData(new string[] { "hello" }, 1)]
        public void GetArrayRank(object obj, int expected)
        {
            Assert.Equal(expected, obj.GetType().GetTypeInfo().GetArrayRank());
        }

        [Fact]
        public void GetDeclaredEvent_Invalid()
        {
            Assert.Throws<ArgumentNullException>(() => typeof(BaseClass).GetTypeInfo().GetDeclaredEvent(null)); // Name is null
        }

        [Fact]
        public void GetDeclaredField_Invalid()
        {
            Assert.Throws<ArgumentNullException>(() => typeof(BaseClass).GetTypeInfo().GetDeclaredField(null)); // Name is null
        }

        [Fact]
        public void GetDeclaredMethod_Invalid()
        {
            Assert.Throws<ArgumentNullException>(() => typeof(BaseClass).GetTypeInfo().GetDeclaredMethod(null)); // Name is null
        }

        [Theory]
        [InlineData(typeof(BaseClass), "OverloadedMethod", 4)]
        [InlineData(typeof(BaseClass), "NoSuchMethod", 0)]
        [InlineData(typeof(BaseClass), "", 0)]
        [InlineData(typeof(BaseClass), null, 0)]
        public void GetDeclaredMethods(Type type, string name, int numberOfMethods)
        {
            IEnumerable<MethodInfo> declaredMethods = type.GetTypeInfo().GetDeclaredMethods(name);
            int count = 0;
            foreach (MethodInfo methodInfo in declaredMethods)
            {
                Assert.Equal(name, methodInfo.Name);
                count++;
            }
            Assert.Equal(numberOfMethods, count);
        }

        [Fact]
        public void GetDeclaredNestedType_Invalid()
        {
            Assert.Throws<ArgumentNullException>(() => typeof(BaseClass).GetTypeInfo().GetDeclaredNestedType(null)); // Name is null
        }

        [Theory]
        [InlineData(typeof(BaseClass), null)]
        [InlineData(typeof(int[]), typeof(int))]
        [InlineData(typeof(int[,]), typeof(int))]
        [InlineData(typeof(int[][]), typeof(int[]))]
        [InlineData(typeof(string[]), typeof(string))]
        [InlineData(typeof(int*), typeof(int))]
        public void GetElementType(Type type, Type expected)
        {
            Assert.Equal(expected, type.GetTypeInfo().GetElementType());
        }

        public static IEnumerable<object[]> GetGenericParameterConstraints_TestData()
        {
            Type[][] parameterConstraints = new Type[][]
            {
                new Type[] { typeof(BaseClass), typeof(NonGenericInterface1) },
                new Type[0]
            };
            yield return new object[] { typeof(ClassWithConstraints<,>), parameterConstraints };
        }

        [Theory]
        [MemberData("GetGenericParameterConstraints_TestData")]
        public void GetGenericParameterConstraints1(Type type, Type[][] expected)
        {
            Type[] genericTypeParameters = type.GetTypeInfo().GenericTypeParameters;

            Assert.Equal(expected.GetLength(0), genericTypeParameters.Length);
            for (int i = 0; i < expected.GetLength(0); i++)
            {
                Assert.Equal(expected[i], genericTypeParameters[i].GetTypeInfo().GetGenericParameterConstraints());
            }
        }

        [Theory]
        [InlineData(typeof(GenericClass1<int>), typeof(GenericClass1<>))]
        public void GetGenericTypeDefinition(Type type, Type expected)
        {
            Assert.Equal(expected, type.GetTypeInfo().GetGenericTypeDefinition());
        }

        [Theory]
        [InlineData(typeof(ClassWithGuid))]
        public void Guid(Type type)
        {
            Assert.NotNull(type.GetTypeInfo().GUID);
        }

        [Theory]
        [InlineData(typeof(int), false)]
        [InlineData(typeof(int*), true)]
        [InlineData(typeof(int[]), true)]
        [InlineData(typeof(int[,]), true)]
        [InlineData(typeof(int[][]), true)]
        public void HasElementType(Type type, bool expected)
        {
            Assert.Equal(expected, type.GetTypeInfo().HasElementType);
        }

        [Theory]
        // Interfaces
        [InlineData(typeof(NonGenericInheritedInterface), new Type[] { typeof(NonGenericInterface1) })]
        // Structs
        [InlineData(typeof(NonGenericStructWithNonGenericInterface1), new Type[] { typeof(NonGenericInterface1), typeof(NonGenericInheritedInterface) })]
        // Classes
        [InlineData(typeof(NonGenericClassWithNonGenericInterface1), new Type[] { typeof(NonGenericInterface1) })]
        [InlineData(typeof(CompoundClass1), new Type[] { typeof(NonGenericInterface1), typeof(NonGenericInterface2), typeof(NonGenericInheritedInterface) })]
        [InlineData(typeof(CompoundClass2<>), new Type[] { typeof(NonGenericInterface1), typeof(NonGenericInterface2), typeof(NonGenericInheritedInterface) })]
        [InlineData(typeof(CompoundClass2<int>), new Type[] { typeof(NonGenericInterface1), typeof(NonGenericInterface2), typeof(NonGenericInheritedInterface) })]
        [InlineData(typeof(CompoundClass3<NonGenericInheritedInterface>), new Type[] { typeof(GenericInterface1<NonGenericInheritedInterface>), typeof(NonGenericInterface1) })]
        [InlineData(typeof(CompoundClass4<>), new Type[] { typeof(GenericInterface1<string>), typeof(NonGenericInterface1) })]
        [InlineData(typeof(CompoundClass4<string>), new Type[] { typeof(GenericInterface1<string>), typeof(NonGenericInterface1) })]
        public void ImplementedInterfaces(Type type, Type[] expected)
        {
            TypeInfo typeInfo = type.GetTypeInfo();
            Type[] implementedInterfaces = type.GetTypeInfo().ImplementedInterfaces.ToArray();

            Array.Sort(implementedInterfaces, delegate (Type a, Type b) { return a.GetHashCode() - b.GetHashCode(); });
            Array.Sort(expected, delegate (Type a, Type b) { return a.GetHashCode() - b.GetHashCode(); });

            Assert.Equal(expected.Length, implementedInterfaces.Length);
            for (int i = 0; i < implementedInterfaces.Length; i++)
            {
                Assert.Equal(expected[i], implementedInterfaces[i]);
                Assert.True(expected[i].GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()));
            }
        }

        [Theory]
        [InlineData(typeof(BaseClass), typeof(SubClass), true)]
        [InlineData(typeof(SubClass), typeof(BaseClass), false)]
        [InlineData(typeof(BaseClass), typeof(BaseClass), true)]
        [InlineData(typeof(NonGenericInterface1), typeof(NonGenericClassWithNonGenericInterface1), true)]
        [InlineData(typeof(NonGenericClassWithNonGenericInterface1), typeof(NonGenericInterface1), false)]
        // Arrays and lists
        [InlineData(typeof(IList<object>), typeof(object[]), true)]
        [InlineData(typeof(object[]), typeof(IList<object>), false)]
        [InlineData(typeof(BaseClass), typeof(SubClass), true)]
        [InlineData(typeof(BaseClass[]), typeof(SubClass[]), true)]
        [InlineData(typeof(IList<object>), typeof(BaseClass[]), true)]
        [InlineData(typeof(IList<BaseClass>), typeof(BaseClass[]), true)]
        [InlineData(typeof(IList<BaseClass>), typeof(SubClass[]), true)]
        [InlineData(typeof(IList<SubClass>), typeof(SubClass[]), true)]
        // Strings and objects
        [InlineData(typeof(GenericInterface1<object>), typeof(GenericClassWithGenericInterface1Subclass<object>), true)]
        [InlineData(typeof(GenericClassWithGenericInterface1<string>), typeof(GenericClassWithGenericInterface1Subclass<string>), true)]
        [InlineData(typeof(GenericClassWithGenericInterface1<string>), typeof(GenericClassWithGenericInterface1<string>), true)]
        [InlineData(typeof(GenericClassWithGenericInterface1<string>), typeof(GenericClassWithGenericInterface1<object>), false)]
        [InlineData(typeof(GenericClassWithGenericInterface1<object>), typeof(GenericClassWithGenericInterface1<string>), false)]
        [InlineData(typeof(GenericClassWithGenericInterface1Subclass<object>), typeof(GenericClassWithGenericInterface1<string>), false)]
        [InlineData(typeof(GenericClassWithGenericInterface1<string>), typeof(GenericInterface1<string>), false)]
        // Interfaces
        [InlineData(typeof(NonGenericInterface1), typeof(NonGenericInterface1), true)]
        [InlineData(typeof(NonGenericInterface1), typeof(NonGenericClassWithNonGenericInterface1), true)]
        [InlineData(typeof(NonGenericInterface2), typeof(CompoundClass1), true)]
        [InlineData(typeof(NonGenericInterface1), typeof(GenericClassWithNonGenericInterface1<>), true)]
        [InlineData(typeof(NonGenericInterface1), typeof(GenericClassWithNonGenericInterface1<string>), true)]
        [InlineData(typeof(NonGenericClassWithNonGenericInterface1), typeof(NonGenericInterface1), false)]
        // Namespaces
        [InlineData(typeof(IsAssignableNamespace.BaseClass1), typeof(IsAssignableNamespace.BaseClass2), true)]
        [InlineData(typeof(IsAssignableNamespace.BaseClass1), typeof(IsAssignableNamespace.SubClass), true)]
        [InlineData(typeof(IsAssignableNamespace.BaseClass2), typeof(IsAssignableNamespace.SubClass), true)]
        // A T[] is assignable to IList<U> iff T[] is assignable to U[]
        [InlineData(typeof(NonGenericInterface1[]), typeof(NonGenericStructWithNonGenericInterface1[]), false)]
        [InlineData(typeof(NonGenericInterface1[]), typeof(NonGenericClassWithNonGenericInterface1[]), true)]
        [InlineData(typeof(IList<NonGenericInterface1>), typeof(NonGenericStructWithNonGenericInterface1[]), false)]
        [InlineData(typeof(IList<NonGenericInterface1>), typeof(NonGenericClassWithNonGenericInterface1[]), true)]
        [InlineData(typeof(int[]), typeof(uint[]), true)]
        [InlineData(typeof(uint[]), typeof(int[]), true)]
        [InlineData(typeof(IList<int>), typeof(uint[]), true)]
        [InlineData(typeof(IList<uint>), typeof(int[]), true)]
        public void IsAssignableFrom(Type type1, Type type2, bool expected)
        {
            Assert.Equal(expected, type1.GetTypeInfo().IsAssignableFrom(type2.GetTypeInfo()));
        }

        [Fact]
        public void IsAssignable_Null()
        {
            Assert.False(typeof(BaseClass).GetTypeInfo().IsAssignableFrom(null));
        }

        [Theory]
        [InlineData(typeof(string), typeof(string), true)]
        [InlineData(typeof(object), typeof(string), false)]
        [InlineData(typeof(BaseClass), typeof(BaseClass), true)]
        public void IsEquivalentTo(Type type, Type other, bool expected)
        {
            Assert.Equal(expected, type.GetTypeInfo().IsEquivalentTo(other));
        }

        [Theory]
        [InlineData(typeof(SubClass), typeof(BaseClass), true)]
        [InlineData(typeof(BaseClass), typeof(SubClass), false)]
        public void IsSubClassOf(Type type1, Type type2, bool expected)
        {
            Assert.Equal(expected, type1.GetTypeInfo().IsSubclassOf(type2));
        }

        [Theory]
        [InlineData(typeof(string), typeof(string[]))]
        [InlineData(typeof(int), typeof(int[]))]
        public void MakeArrayType(Type type, Type expected)
        {
            Type arrayType = type.GetTypeInfo().MakeArrayType();
            Assert.Equal(expected, arrayType);
            Assert.True(arrayType.IsArray, "MakeArrayType() returned a type for which IsArray is false.");
        }

        [Fact]
        public void MakeArrayType_Invalid()
        {
            Assert.Throws<TypeLoadException>(() => typeof(int).MakeByRefType().GetTypeInfo().MakeArrayType()); // Type is a by ref type
        }

        [Theory]
        [InlineData(typeof(int), 1, typeof(int[]))]
        [InlineData(typeof(int), 2, typeof(int[,]))]
        [InlineData(typeof(int), 3, typeof(int[,,]))]
        [InlineData(typeof(char*), 3, typeof(char*[,,]))]
        public void MakeArrayType_Int(Type type, int rank, Type expected)
        {
            Type arrayType = type.GetTypeInfo().MakeArrayType(rank);
            // For some reason, typeof(int[]).Equals(typeof(int[]) returns false here
            if (expected != typeof(int[]))
            {
                Assert.Equal(expected, arrayType);
            }
            Assert.Equal(type, arrayType.GetElementType());
            Assert.Equal(rank, arrayType.GetArrayRank());
            Assert.True(arrayType.IsArray, "MakeArrayType() returned a type for which IsArray is false.");
        }

        [Fact]
        public void MakeArrayType_Int_Invalid()
        {
            TypeInfo typeInfo = typeof(int).GetTypeInfo();
            Assert.Throws<IndexOutOfRangeException>(() => typeInfo.MakeArrayType(0)); // Rank < 1
            Assert.Throws<IndexOutOfRangeException>(() => typeInfo.MakeArrayType(-1)); // Rank < 1
            Assert.Throws<TypeLoadException>(() => typeInfo.MakeArrayType(33)); // Rank > 32

            Assert.Throws<TypeLoadException>(() => typeof(int).MakeByRefType().GetTypeInfo().MakeArrayType(1)); // Type is a by ref type
        }

        [Theory]
        [InlineData(typeof(int))]
        [InlineData(typeof(string))]
        public void MakeByRefType(Type type)
        {
            Type byRefType = type.GetTypeInfo().MakeByRefType();
            Assert.NotNull(byRefType);
            Assert.True(byRefType.IsByRef, "MakeByRefType() returned a type for which IsByRef is false.");
        }

        [Fact]
        public void MakeByRefType_Invalid()
        {
            Type alreadyByRefType = typeof(string).GetTypeInfo().MakeByRefType();
            Assert.Throws<TypeLoadException>(() => alreadyByRefType.MakeByRefType()); // Type is aready byRef
        }

        [Theory]
        [InlineData(typeof(int))]
        [InlineData(typeof(string))]
        public void MakePointerType(Type type)
        {
            Type pointerType = type.GetTypeInfo().MakePointerType();
            Assert.NotNull(pointerType);
            Assert.True(pointerType.IsPointer, "MakePointerType() returned a type for which IsPointer is false.");
        }

        [Fact]
        public void MakePointerType_Invalid()
        {
            Type byRefType = typeof(string).GetTypeInfo().MakeByRefType();
            Assert.Throws<TypeLoadException>(() => byRefType.MakePointerType()); // Type is a by ref type
        }

        [Theory]
        [InlineData(typeof(List<>), new Type[] { typeof(string) })]
        public void MakeGenericType(Type type, Type[] typeArguments)
        {
            Type genericType = type.GetTypeInfo().MakeGenericType(typeArguments);
            Assert.NotNull(genericType);
            Assert.True(genericType.GetTypeInfo().IsGenericType, "MakeGenericType() returned a type for which IsGenericType is false.");
        }

        [Fact]
        public void MakeGenericType_Invalid()
        {
            TypeInfo typeInfo = typeof(List<>).GetTypeInfo();
            Assert.Throws<ArgumentNullException>(() => typeInfo.MakeGenericType(null)); // TypeArguments is null
            Assert.Throws<ArgumentNullException>(() => typeInfo.MakeGenericType(new Type[] { null })); // TypeArguments has a null element
            Assert.Throws<ArgumentException>(() => typeInfo.MakeGenericType(new Type[] { typeof(int*) })); // TypeArguments has a pointer type
            Assert.Throws<ArgumentException>(() => typeInfo.MakeGenericType(new Type[] { typeof(void) })); // TypeArguments has a void type
            Assert.Throws<ArgumentException>(() => typeInfo.MakeGenericType(new Type[] { typeof(int).MakeByRefType() })); // TypeArguments has a by ref type

            Assert.Throws<InvalidOperationException>(() => typeof(int).GetTypeInfo().MakeGenericType(new Type[] { typeof(int) })); // Type has no generic type definition
        }

        [Theory]
        [InlineData(typeof(BaseClass), "System.Reflection.Tests")]
        [InlineData(typeof(NonGenericInterface1), "System.Reflection.Tests")]
        [InlineData(typeof(NonGenericEnum), "System.Reflection.Tests")]
        [InlineData(typeof(BaseClass.PublicNestedClass1), "System.Reflection.Tests")]
        [InlineData(typeof(int), "System")]
        public void Namespace(Type type, string expected)
        {
            Assert.Equal(expected, type.GetTypeInfo().Namespace);
        }

        [Theory]
        [InlineData(typeof(string), "System.String")]
        [InlineData(typeof(int), "System.Int32")]
        public void ToString(Type type, string expected)
        {
            Assert.Equal(expected, type.GetTypeInfo().ToString());
        }
        
        [Theory]
        [InlineData(typeof(AbstractClass), true)]
        [InlineData(typeof(BaseClass), false)]
        public void IsAbstract(Type type, bool expected)
        {
            Assert.Equal(expected, type.GetTypeInfo().IsAbstract);
        }

        [Theory]
        [InlineData(typeof(string), true)]
        public void IsAnsiClass(Type type, bool expected)
        {
            Assert.Equal(expected, type.GetTypeInfo().IsAnsiClass);
        }

        [Theory]
        [InlineData(typeof(int), false)]
        [InlineData(typeof(int*), false)]
        [InlineData(typeof(int[]), true)]
        [InlineData(typeof(int[,]), true)]
        [InlineData(typeof(int[][]), true)]
        public void IsArray(Type type, bool expected)
        {
            Assert.Equal(expected, type.GetTypeInfo().IsArray);
        }

        [Theory]
        [InlineData(typeof(string), false)]
        public void IsAutoClass(Type type, bool expected)
        {
            Assert.Equal(expected, type.GetTypeInfo().IsAutoClass);
            Assert.Equal(expected, type.MakeByRefType().GetTypeInfo().IsAutoClass);
        }

        public static IEnumerable<object[]> IsByRef_TestData()
        {
            yield return new object[] { typeof(int).MakeByRefType(), true };
            yield return new object[] { typeof(int), false };
        }

        [Theory]
        [MemberData("IsByRef_TestData")]
        public void IsByRef(Type type, bool expected)
        {
            Assert.Equal(expected, type.GetTypeInfo().IsByRef);
        }

        [Theory]
        [InlineData(typeof(BaseClass), true)]
        [InlineData(typeof(NonGenericEnum), false)]
        public void IsClass(Type type, bool expected)
        {
            Assert.Equal(expected, type.GetTypeInfo().IsClass);
        }

        [Theory]
        [InlineData(typeof(BaseClass), false)]
        [InlineData(typeof(NonGenericEnum), true)]
        public void IsEnum(Type type, bool expected)
        {
            Assert.Equal(expected, type.GetTypeInfo().IsEnum);
        }

        [Theory]
        [InlineData(typeof(BaseClass), false)]
        [InlineData(typeof(NonGenericEnum), false)]
        public void IsImport(Type type, bool expected)
        {
            Assert.Equal(expected, type.GetTypeInfo().IsImport);
        }

        [Theory]
        [InlineData(typeof(BaseClass), false)]
        [InlineData(typeof(NonGenericInterface1), true)]
        public void IsInterface(Type type, bool expected)
        {
            Assert.Equal(expected, type.GetTypeInfo().IsInterface);
        }

        [Theory]
        [InlineData(typeof(string), false)]
        public void IsMarshalByRef(Type type, bool expected)
        {
            Assert.Equal(expected, type.GetTypeInfo().IsMarshalByRef);
            Assert.Equal(expected, type.MakeByRefType().GetTypeInfo().IsMarshalByRef);
            Assert.Equal(expected, type.MakePointerType().GetTypeInfo().IsMarshalByRef);
        }

        [Theory]
        [InlineData(typeof(BaseClass), false)]
        [InlineData(typeof(BaseClass.PublicNestedClass1), true)]
        public void IsNested(Type type, bool expected)
        {
            Assert.Equal(expected, type.GetTypeInfo().IsNested);
        }

        [Theory]
        [InlineData(typeof(BaseClass), false)]
        public void IsNestedAssembly(Type type, bool expected)
        {
            Assert.Equal(expected, type.GetTypeInfo().IsNestedAssembly);
        }

        [Theory]
        [InlineData(typeof(BaseClass), false)]
        public void IsNestedFamily(Type type, bool expected)
        {
            Assert.Equal(expected, type.GetTypeInfo().IsNestedFamily);
        }

        [Theory]
        [InlineData(typeof(BaseClass), false)]
        public void IsNestedFamANDAssem(Type type, bool expected)
        {
            Assert.Equal(expected, type.GetTypeInfo().IsNestedFamANDAssem);
        }

        [Theory]
        [InlineData(typeof(BaseClass), false)]
        public void IsNestedFamORAssem(Type type, bool expected)
        {
            Assert.Equal(expected, type.GetTypeInfo().IsNestedFamORAssem);
        }

        [Theory]
        [InlineData(typeof(BaseClass), false)]
        [InlineData(typeof(BaseClass.PublicNestedClass1), false)]
        public void IsNestedPrivate(Type type, bool expected)
        {
            Assert.Equal(expected, type.GetTypeInfo().IsNestedPrivate);
        }

        [Theory]
        [InlineData(typeof(BaseClass), false)]
        [InlineData(typeof(BaseClass.PublicNestedClass1), true)]
        public void IsNestedPublic(Type type, bool expected)
        {
            Assert.Equal(expected, type.GetTypeInfo().IsNestedPublic);
        }

        public static IEnumerable<object[]> IsPointer_TestData()
        {
            yield return new object[] { typeof(int).MakePointerType(), true };
            yield return new object[] { typeof(int), false };
        }

        [Theory]
        [MemberData("IsPointer_TestData")]
        public void IsPointer(Type type, bool expected)
        {
            Assert.Equal(expected, type.GetTypeInfo().IsPointer);
        }

        [Theory]
        [InlineData(typeof(BaseClass), false)]
        [InlineData(typeof(int), true)]
        [InlineData(typeof(char), true)]
        public void IsPrimitive(Type type, bool expected)
        {
            Assert.Equal(expected, type.GetTypeInfo().IsPrimitive);
        }

        [Theory]
        [InlineData(typeof(BaseClass), true)]
        [InlineData(typeof(SubClass), true)]
        [InlineData(typeof(NonGenericInterface1), true)]
        public void IsPublic(Type type, bool expected)
        {
            // Public and IsNotPublic should be mutually exclusive.
            Assert.Equal(expected, type.GetTypeInfo().IsPublic);
            Assert.Equal(!expected, type.GetTypeInfo().IsNotPublic);
        }

        [Theory]
        [InlineData(typeof(BaseClass), false)]
        [InlineData(typeof(SealedClass), true)]
        public void IsSealed(Type type, bool expected)
        {
            Assert.Equal(expected, type.GetTypeInfo().IsSealed);
        }

        [Theory]
        [InlineData(typeof(BaseClass), false)]
        public void IsSerializable(Type type, bool expected)
        {
            Assert.Equal(expected, type.GetTypeInfo().IsSerializable);
        }

        [Theory]
        [InlineData(typeof(string), false)]
        public void IsUnicodeClass(Type type, bool expected)
        {
            Assert.Equal(expected, type.GetTypeInfo().IsUnicodeClass);
            Assert.Equal(expected, type.MakeByRefType().GetTypeInfo().IsUnicodeClass);
        }

        [Theory]
        [InlineData(typeof(BaseClass), false)]
        [InlineData(typeof(NonGenericEnum), true)]
        public void IsValueType(Type type, bool expected)
        {
            Assert.Equal(expected, type.GetTypeInfo().IsValueType);
        }

        [Theory]
        [InlineData(typeof(BaseClass), true)]
        [InlineData(typeof(NonGenericInterface1), true)]
        [InlineData(typeof(NonGenericEnum), true)]
        [InlineData(typeof(BaseClass.PublicNestedClass1), true)]
        public void IsVisible(Type type, bool expected)
        {
            Assert.Equal(expected, type.GetTypeInfo().IsVisible);
        }        
    }

    // Metadata for Reflection
    public class BaseClass
    {
        public static int staticInt = 9;
        public string subPublicField1 = "";
        public string publicField1 = "";
        public readonly string publicField2 = "";
        public volatile string publicField3 = "";
        public static string publicField4 = "";
        public static readonly string publicField5 = "";
        public static volatile string publicField6 = "";

        static BaseClass() { }

        public BaseClass() { }
        public BaseClass(short i) { }
        public BaseClass(TimeSpan ts) { }
        public BaseClass(object object1, object object2) { }
        public BaseClass(object obj0, int i) { }

        public string PublicProperty1 { get { return ""; } set { } }
        public string SubPublicProperty1 { get { return ""; } set { } }
        public virtual string PublicProperty2 { get { return ""; } set { } }
        public static string PublicProperty3 { get { return ""; } set { } }

        public void PublicBaseMethod1() { }
        public void PublicMethod1() { }
        public virtual void PublicMethod2() { }
        public virtual void PublicMethod2ToOverride() { }
        public static void PublicMethod3() { }

        public void OverloadedMethod() { }
        public void OverloadedMethod(string s) { }
        public void OverloadedMethod(string s, int i) { }
        public int OverloadedMethod(DateTime dt) { return 2; }

        public event EventHandler EventPublic;
        public static event EventHandler EventPublicStatic;

        public class PublicNestedClass1 { }
        public class PublicNestedClass2 { }
        private class PrivateNestedClass { } // Private, so not inherited
        internal class InternalNestedClass { } // Internal members are not inherited
        protected class ProtectedNestedClass { }
    }

    public class SubClass : BaseClass
    {
        public new static int staticInt = 10;
        public static string[] stringArray = new string[] { "string" };
        public new string publicField1 = "";
        public new readonly string publicField2 = "";
        public new volatile string publicField3 = "";
        public new static string publicField4 = "";
        public new static readonly string publicField5 = "";
        public new static volatile string publicField6 = "";

        public SubClass(string s) { }
        public SubClass(short i) { }

        public new string PublicProperty1 { get { return ""; } set { } }
        public new virtual string PublicProperty2 { get { return ""; } set { } }
        public new static string PublicProperty3 { get { return ""; } set { } }

        public new void PublicMethod1() { }
        public new virtual void PublicMethod2() { }
        public override void PublicMethod2ToOverride() { }
        public new static void PublicMethod3() { }

        public new event EventHandler EventPublic; // Overrides event				
        public event EventHandler EventPublicNew; // New event

        public new class PublicNestedClass1 { }
        public class PublicNestedClass3 { }
        private class PrivateNestedClass2 { }
    }

    public static class StaticClass
    {
        static StaticClass() { }
    }

    [Guid("FD80F123-BEDD-4492-B50A-5D46AE94DD4E")]
    public static class ClassWithGuid { }

    public sealed class SealedClass { }
    public abstract class AbstractClass { }

    public class MultiNestClass
    {
        public class Nest1
        {
            public class Nest2
            {
                public class Nest3 { }
            }
        }
    }

    public class ClassWithConstraints<T, U> where T : BaseClass, NonGenericInterface1 where U : class, new() { }

    public enum NonGenericEnum { One, Two, Three }

    public interface NonGenericInterface1 { }
    public interface NonGenericInterface2 { }
    public interface NonGenericInheritedInterface : NonGenericInterface1 { }
    public interface GenericInterface1<T> { }
    public interface GenericInterface2<T, U> { }

    public struct NonGenericStruct { }
    public struct GenericStruct1<T> { }
    public struct GenericStruct2<T, U> { }

    public struct NonGenericStructWithNonGenericInterface1 : NonGenericInheritedInterface { }
    public struct NonGenericStructWithGenericInterface1 : GenericInterface1<int> { }
    public struct NonGenericStructWithGenericInterface2 : GenericInterface2<int, int> { }

    public struct GenericStructWithGenericInterface1<T> : GenericInterface1<T> { }
    public struct GenericStructWithGenericInterface2<T, U> : GenericInterface2<T, U> { }
    public struct GenericStructWithGenericInterface3<T> : GenericInterface2<T, int> { }

    public class NonGenericClass { }
    public class GenericClass1<T> { }
    public class GenericClass2<T, U> { }

    public class NonGenericClassWithNonGenericInterface1 : NonGenericInterface1 { }
    public class NonGenericClassWithNonGenericInterfaceSubclass: NonGenericClassWithNonGenericInterface1 { }
    public class NonGenericClassWithGenericInterface : GenericInterface1<int> { }
    public class NonGenericClassWithGenericSuperClass : GenericClass2<int, int> { }

    public class GenericClassWithNonGenericInterface1<T> : NonGenericInterface1 { }
    public class GenericClassWithGenericInterface1<T> : GenericInterface1<T> { }
    public class GenericClassWithGenericInterface1Subclass<T> : GenericClassWithGenericInterface1<T> { }
    public class GenericClassWithGenericInterface2<T, U> : GenericInterface2<T, U> { }
    public class GenericClassWithGenericSuperClass<T> : GenericClass2<T, int> { }

    public class CompoundClass1 : NonGenericClassWithNonGenericInterface1, NonGenericInheritedInterface, NonGenericInterface2 { }
    public class CompoundClass2<T> : NonGenericClassWithNonGenericInterface1, NonGenericInheritedInterface, NonGenericInterface2 { }
    public class CompoundClass3<T> : NonGenericClassWithNonGenericInterface1, GenericInterface1<T> { }
    public class CompoundClass4<T> : NonGenericClassWithNonGenericInterface1, GenericInterface1<string> { }

    namespace IsAssignableNamespace
    {
        public abstract class BaseClass1 { }
        public abstract class BaseClass2 : BaseClass1 { }
        public class SubClass : BaseClass2 { }
    }
}
