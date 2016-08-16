// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace System.Reflection.Tests
{
    public class TypeTests
    {
        [Theory]
        [InlineData(typeof(TI_Class), nameof(TI_Class), MemberTypes.TypeInfo)]
        [InlineData(typeof(TI_Class.PublicNestedType), nameof(TI_Class.PublicNestedType), MemberTypes.NestedType)]
        [InlineData(typeof(string), nameof(String), MemberTypes.TypeInfo)]
        public void Properties(Type type, string name, MemberTypes memberType)
        {
            Assert.Equal(name, type.Name);
            Assert.Equal(memberType, type.GetTypeInfo().MemberType);

            Assert.Equal(memberType == MemberTypes.NestedType, type.IsNested);
        }

        [Theory]
        [InlineData(typeof(GenericArrayWrapperClass<string>), new string[] { "System.String Item [Int32]" })]
        [InlineData(typeof(GenericArrayWrapperClass<>), new string[] { "T Item [Int32]" })]
        [InlineData(typeof(GenericClass<>), new string[] { "T ReturnAndSetField(T)" })]
        [InlineData(typeof(GenericClass<int>), new string[] { "Int32 ReturnAndSetField(Int32)" })]
        public void GetDefaultMembers(Type type, string[] expectedToString)
        {
            string[] memberNames = type.GetDefaultMembers().Select(member => member.Name).ToArray();
            Assert.All(expectedToString, toString => memberNames.Contains(toString));
        }

        public static IEnumerable<object[]> GetEvents_TestData()
        {
            string[] expectedPublic = new string[] { "WeightChanged" };
            yield return new object[] { BindingFlags.Default, expectedPublic };

            yield return new object[] { BindingFlags.IgnoreCase, new string[0] };
            yield return new object[] { BindingFlags.DeclaredOnly, new string[0] };
            yield return new object[] { BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly, new string[0] };

            yield return new object[] { BindingFlags.Instance, new string[0] };
            yield return new object[] { BindingFlags.IgnoreCase | BindingFlags.Instance, new string[0] };
            yield return new object[] { BindingFlags.Instance | BindingFlags.DeclaredOnly, new string[0] };
            yield return new object[] { BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.DeclaredOnly, new string[0] };

            yield return new object[] { BindingFlags.IgnoreCase | BindingFlags.Static, new string[0] };
            yield return new object[] { BindingFlags.Static, new string[0] };
            yield return new object[] { BindingFlags.DeclaredOnly | BindingFlags.Static, new string[0] };
            yield return new object[] { BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Static, new string[0] };

            yield return new object[] { BindingFlags.Instance | BindingFlags.Static, new string[0] };
            yield return new object[] { BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static, new string[0] };
            yield return new object[] { BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static, new string[0] };
            yield return new object[] { BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static, new string[0] };

            yield return new object[] { BindingFlags.Public, new string[0] };
            yield return new object[] { BindingFlags.IgnoreCase | BindingFlags.Public, new string[0] };
            yield return new object[] { BindingFlags.DeclaredOnly | BindingFlags.Public, new string[0] };
            yield return new object[] { BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Public, new string[0] };

            yield return new object[] { BindingFlags.Instance | BindingFlags.Public, expectedPublic };
            yield return new object[] { BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public, expectedPublic };
            yield return new object[] { BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public, expectedPublic };
            yield return new object[] { BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public, expectedPublic };

            yield return new object[] { BindingFlags.Public | BindingFlags.Static, new string[0] };
            yield return new object[] { BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Static, new string[0] };
            yield return new object[] { BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static, new string[0] };
            yield return new object[] { BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static, new string[0] };

            yield return new object[] { BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, expectedPublic };
            yield return new object[] { BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, expectedPublic };
            yield return new object[] { BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, expectedPublic };
            yield return new object[] { BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, expectedPublic };

            yield return new object[] { BindingFlags.NonPublic, new string[0] };
            yield return new object[] { BindingFlags.IgnoreCase | BindingFlags.NonPublic, new string[0] };
            yield return new object[] { BindingFlags.DeclaredOnly | BindingFlags.NonPublic, new string[0] };
            yield return new object[] { BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.NonPublic, new string[0] };

            string[] expectedNonPublic = new string[] { "WeightStayedTheSame" };
            yield return new object[] { BindingFlags.Instance | BindingFlags.NonPublic, expectedNonPublic };
            yield return new object[] { BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic, expectedNonPublic };
            yield return new object[] { BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic, expectedNonPublic };
            yield return new object[] { BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic, expectedNonPublic };

            yield return new object[] { BindingFlags.Static | BindingFlags.NonPublic, new string[0] };
            yield return new object[] { BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.NonPublic, new string[0] };
            yield return new object[] { BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic, new string[0] };
            yield return new object[] { BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic, new string[0] };

            yield return new object[] { BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic, expectedNonPublic };
            yield return new object[] { BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic, expectedNonPublic };
            yield return new object[] { BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic, expectedNonPublic };
            yield return new object[] { BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic, expectedNonPublic };

            yield return new object[] { BindingFlags.Public | BindingFlags.NonPublic, new string[0] };
            yield return new object[] { BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic, new string[0] };
            yield return new object[] { BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic, new string[0] };
            yield return new object[] { BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic, new string[0] };

            string[] expectedAll = new string[] { "WeightChanged", "WeightStayedTheSame" };
            yield return new object[] { BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, expectedAll };
            yield return new object[] { BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, expectedAll };
            yield return new object[] { BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, expectedAll };
            yield return new object[] { BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, expectedAll };

            yield return new object[] { BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, new string[0] };
            yield return new object[] { BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, new string[0] };
            yield return new object[] { BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, new string[0] };
            yield return new object[] { BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, new string[0] };

            yield return new object[] { BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, expectedAll };
            yield return new object[] { BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, expectedAll };
            yield return new object[] { BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, expectedAll };
            yield return new object[] { BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, expectedAll };

            yield return new object[] { BindingFlags.FlattenHierarchy, new string[0] };
        }

        [Theory]
        [MemberData(nameof(GetEvents_TestData))]
        public void GetEvents(BindingFlags bindingAttributes, string[] expectedNames)
        {
            if (bindingAttributes == BindingFlags.Default)
            {
                string[] eventNames = typeof(Cat<int>).GetEvents().Select(eventInfo => eventInfo.Name).ToArray();
                Assert.All(expectedNames, name => eventNames.Contains(name));
            }
            else
            {
                string[] eventNames = typeof(Cat<int>).GetEvents(bindingAttributes).Select(eventInfo => eventInfo.Name).ToArray();
                Assert.All(expectedNames, name => eventNames.Contains(name));
            }
        }

        public static IEnumerable<object[]> GetFields_TestData()
        {
            yield return new object[] { typeof(GenericClassUsingNestedInterfaces<string, int>), new string[] { "FieldZero", "FieldOne", "FieldTwo", "FieldThree" } };
            yield return new object[] { typeof(GenericStructWithInterface<string>), new string[] { "field", "field2" } };
            yield return new object[] { typeof(NonGenericClassWithGenericInterface), new string[] { "field" } };
            yield return new object[] { typeof(GenericStruct2TP<int, string>), new string[] { "field", "field2" } };
            yield return new object[] { typeof(GenericClassWithVarArgMethod<>), new string[] { "field" } };
        }

        [Theory]
        [MemberData(nameof(GetFields_TestData))]
        public void GetFields(Type type, string[] expectedNames)
        {
            string[] fieldNames = type.GetFields().Select(field => field.Name).ToArray();
            Assert.All(expectedNames, name => fieldNames.Contains(name));
        }

        [Theory]
        [InlineData(typeof(GenericClass<string>), new Type[0])]
        [InlineData(typeof(IGenericInterface<string>), new Type[0])]
        [InlineData(typeof(Cat<string>), new Type[] { typeof(IConsume) })]
        [InlineData(typeof(Cat<>), new Type[] { typeof(IConsume) })]
        [InlineData(typeof(PackOfCarnivores<Cat<string>>), new Type[0])]
        [InlineData(typeof(IGenericInterfaceInherits<int, string>), new Type[] { typeof(IGenericInterface<int>), typeof(IGenericInterface2<string, int>) })]
        [InlineData(typeof(GenericClassUsingNestedInterfaces<int, string>), new Type[] { typeof(IGenericInterfaceInherits<int, string>), typeof(IGenericInterface<int>), typeof(IGenericInterface2<string, int>) })]
        [InlineData(typeof(NonGenericClassWithGenericInterface), new Type[] { typeof(IGenericInterface<int>) })]
        public void GetInterfaces(Type type, Type[] expected)
        {
            Assert.Equal(expected, type.GetInterfaces());
        }

        [Fact]
        public void GetInterfaces_GenericInterfaceWithTypeParameter_ReturnsExpectedToString()
        {
            Type[] interfaces = typeof(GenericClassWithInterface<>).GetInterfaces();
            Assert.Equal(1, interfaces.Length);
            Assert.Equal("System.Reflection.Tests.IGenericInterface`1[T]", interfaces[0].ToString());
        }

        public static IEnumerable<object[]> GetMember_TestData()
        {
            yield return new object[] { typeof(GenericClassUsingNestedInterfaces<string, int>), "Field*", BindingFlags.Default, new string[] { "FieldZero", "FieldOne", "FieldTwo", "FieldThree" } };
            yield return new object[] { typeof(GenericClassUsingNestedInterfaces<string, int>), "Return*", BindingFlags.Default, new string[] { "ReturnAndSetFieldZero", "ReturnAndSetFieldThree" } };
            yield return new object[] { typeof(GenericClassWithInterface<int>), "*", BindingFlags.Default, new string[] { "ReturnAndSetFieldZero", "GenericMethod", "ToString", "Equals", "GetHashCode", "GetType", ".ctor", "field" } };
            yield return new object[] { typeof(IGenericInterface<>), "ReturnAndSetFieldZero", BindingFlags.Default, new string[] { "ReturnAndSetFieldZero" } };

            yield return new object[] { typeof(GenericArrayWrapperClass<>), "*", BindingFlags.Default, new string[] { "get_myProperty", "set_myProperty", "get_Item", "set_Item", "ToString", "Equals", "GetHashCode", "GetType", ".ctor", "myProperty", "Item" } };

            yield return new object[] { typeof(Cat<int>), "*", BindingFlags.Default, new string[] { "add_WeightChanged", "remove_WeightChanged", "get_StuffConsumed", "Eat", "Puke", "ToString", "Equals", "GetHashCode", "GetType", ".ctor", "StuffConsumed", "WeightChanged" } };

            yield return new object[] { typeof(GenericArrayWrapperClass<int>), "*", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly, new string[] { "get_myProperty", "set_myProperty", "get_Item", "set_Item", ".ctor", "myProperty", "Item", "_field", "_field1" } };
        }

        [Theory]
        [MemberData(nameof(GetMember_TestData))]
        public void GetMember(Type type, string name, BindingFlags bindingAttributes, string[] expectedNames)
        {
            if (bindingAttributes == BindingFlags.Default)
            {
                string[] memberNames = type.GetMember(name).Select(member => member.Name).ToArray();
                Assert.All(expectedNames, expectedName => memberNames.Contains(expectedName));
                if (name == "*")
                {
                    Assert.Equal(type.GetMembers(), type.GetMember(name));
                }
            }
            else
            {
                string[] memberNames = type.GetMember(name, bindingAttributes).Select(member => member.Name).ToArray();
                Assert.All(expectedNames, expectedName => memberNames.Contains(expectedName));
                if (name == "*")
                {
                    Assert.Equal(type.GetMembers(bindingAttributes), type.GetMember(name, bindingAttributes));
                }
            }
        }
        public static IEnumerable<object[]> GetMethods_TestData()
        {
            yield return new object[] { typeof(GenericClass<string>), BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance, new string[] { "ReturnAndSetField" } };
            yield return new object[] { typeof(GenericClassUsingNestedInterfaces<string, int>), BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance, new string[] { "ReturnAndSetFieldZero", "SetFieldOne", "SetFieldTwo", "ReturnAndSetFieldThree" } };
            yield return new object[] { typeof(GenericClassWithInterface<>), BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance, new string[] { "ReturnAndSetFieldZero", "GenericMethod" } };
            yield return new object[] { typeof(GenericClassWithInterface<int>), BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance, new string[] { "ReturnAndSetFieldZero", "GenericMethod" } };
            yield return new object[] { typeof(GenericClassWithVarArgMethod<int>), BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance, new string[] { "get_publicField", "set_publicField", "ReturnAndSetField" } };

            yield return new object[] { typeof(int), BindingFlags.IgnoreCase, new string[0] };
            yield return new object[] { typeof(int), BindingFlags.DeclaredOnly, new string[0] };
            yield return new object[] { typeof(int), BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly, new string[0] };

            yield return new object[] { typeof(int), BindingFlags.Instance, new string[0] };
            yield return new object[] { typeof(int), BindingFlags.IgnoreCase | BindingFlags.Instance, new string[0] };
            yield return new object[] { typeof(int), BindingFlags.DeclaredOnly | BindingFlags.Instance, new string[0] };
            yield return new object[] { typeof(int), BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Instance, new string[0] };

            yield return new object[] { typeof(int), BindingFlags.Static, new string[0] };
            yield return new object[] { typeof(int), BindingFlags.IgnoreCase | BindingFlags.Static, new string[0] };
            yield return new object[] { typeof(int), BindingFlags.DeclaredOnly | BindingFlags.Static, new string[0] };
            yield return new object[] { typeof(int), BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Static, new string[0] };

            yield return new object[] { typeof(int), BindingFlags.Instance | BindingFlags.Static, new string[0] };
            yield return new object[] { typeof(int), BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static, new string[0] };
            yield return new object[] { typeof(int), BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static, new string[0] };
            yield return new object[] { typeof(int), BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static, new string[0] };

            yield return new object[] { typeof(int), BindingFlags.Public, new string[0] };
            yield return new object[] { typeof(int), BindingFlags.IgnoreCase | BindingFlags.Public, new string[0] };
            yield return new object[] { typeof(int), BindingFlags.DeclaredOnly | BindingFlags.Public, new string[0] };
            yield return new object[] { typeof(int), BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Public, new string[0] };

            yield return new object[] { typeof(int), BindingFlags.Instance | BindingFlags.Public, new string[] { "CompareTo", "CompareTo", "Equals", "Equals", "GetHashCode", "ToString", "ToString", "ToString", "ToString", "GetTypeCode", "GetType" } };
            yield return new object[] { typeof(int), BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public, new string[] { "CompareTo", "CompareTo", "Equals", "Equals", "GetHashCode", "ToString", "ToString", "ToString", "ToString", "GetTypeCode", "GetType" } };
            yield return new object[] { typeof(int), BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public, new string[] { "CompareTo", "CompareTo", "Equals", "Equals", "GetHashCode", "ToString", "ToString", "ToString", "ToString", "GetTypeCode" } };
            yield return new object[] { typeof(int), BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public, new string[] { "CompareTo", "CompareTo", "Equals", "Equals", "GetHashCode", "ToString", "ToString", "ToString", "ToString", "GetTypeCode" } };

            yield return new object[] { typeof(int), BindingFlags.Static | BindingFlags.Public, new string[] { "Parse", "Parse", "Parse", "Parse", "TryParse", "TryParse" } };
            yield return new object[] { typeof(int), BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.Public, new string[] { "Parse", "Parse", "Parse", "Parse", "TryParse", "TryParse" } };
            yield return new object[] { typeof(int), BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public, new string[] { "Parse", "Parse", "Parse", "Parse", "TryParse", "TryParse" } };
            yield return new object[] { typeof(int), BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public, new string[] { "Parse", "Parse", "Parse", "Parse", "TryParse", "TryParse" } };

            yield return new object[] { typeof(int), BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, new string[] { "CompareTo", "CompareTo", "Equals", "Equals", "GetHashCode", "ToString", "ToString", "ToString", "ToString", "Parse", "Parse", "Parse", "Parse", "TryParse", "TryParse", "GetTypeCode", "GetType" } };
            yield return new object[] { typeof(int), BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, new string[] { "CompareTo", "CompareTo", "Equals", "Equals", "GetHashCode", "ToString", "ToString", "ToString", "ToString", "Parse", "Parse", "Parse", "Parse", "TryParse", "TryParse", "GetTypeCode", "GetType" } };
            yield return new object[] { typeof(int), BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, new string[] { "CompareTo", "CompareTo", "Equals", "Equals", "GetHashCode", "ToString", "ToString", "ToString", "ToString", "Parse", "Parse", "Parse", "Parse", "TryParse", "TryParse", "GetTypeCode" } };
            yield return new object[] { typeof(int), BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, new string[] { "CompareTo", "CompareTo", "Equals", "Equals", "GetHashCode", "ToString", "ToString", "ToString", "ToString", "Parse", "Parse", "Parse", "Parse", "TryParse", "TryParse", "GetTypeCode" } };

            yield return new object[] { typeof(int), BindingFlags.NonPublic, new string[0] };
            yield return new object[] { typeof(int), BindingFlags.IgnoreCase | BindingFlags.NonPublic, new string[0] };
            yield return new object[] { typeof(int), BindingFlags.DeclaredOnly | BindingFlags.NonPublic, new string[0] };
            yield return new object[] { typeof(int), BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.NonPublic, new string[0] };

            yield return new object[] { typeof(int), BindingFlags.Instance | BindingFlags.NonPublic, new string[] { "System.IConvertible.ToBoolean", "System.IConvertible.ToChar", "System.IConvertible.ToSByte", "System.IConvertible.ToByte", "System.IConvertible.ToInt16", "System.IConvertible.ToUInt16", "System.IConvertible.ToInt32", "System.IConvertible.ToUInt32", "System.IConvertible.ToInt64", "System.IConvertible.ToUInt64", "System.IConvertible.ToSingle", "System.IConvertible.ToDouble", "System.IConvertible.ToDecimal", "System.IConvertible.ToDateTime", "System.IConvertible.ToType", "Finalize", "MemberwiseClone" } };
            yield return new object[] { typeof(int), BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic, new string[] { "System.IConvertible.ToBoolean", "System.IConvertible.ToChar", "System.IConvertible.ToSByte", "System.IConvertible.ToByte", "System.IConvertible.ToInt16", "System.IConvertible.ToUInt16", "System.IConvertible.ToInt32", "System.IConvertible.ToUInt32", "System.IConvertible.ToInt64", "System.IConvertible.ToUInt64", "System.IConvertible.ToSingle", "System.IConvertible.ToDouble", "System.IConvertible.ToDecimal", "System.IConvertible.ToDateTime", "System.IConvertible.ToType", "Finalize", "MemberwiseClone" } };
            yield return new object[] { typeof(int), BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic, new string[] { "System.IConvertible.ToBoolean", "System.IConvertible.ToChar", "System.IConvertible.ToSByte", "System.IConvertible.ToByte", "System.IConvertible.ToInt16", "System.IConvertible.ToUInt16", "System.IConvertible.ToInt32", "System.IConvertible.ToUInt32", "System.IConvertible.ToInt64", "System.IConvertible.ToUInt64", "System.IConvertible.ToSingle", "System.IConvertible.ToDouble", "System.IConvertible.ToDecimal", "System.IConvertible.ToDateTime", "System.IConvertible.ToType" } };
            yield return new object[] { typeof(int), BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic, new string[] { "System.IConvertible.ToBoolean", "System.IConvertible.ToChar", "System.IConvertible.ToSByte", "System.IConvertible.ToByte", "System.IConvertible.ToInt16", "System.IConvertible.ToUInt16", "System.IConvertible.ToInt32", "System.IConvertible.ToUInt32", "System.IConvertible.ToInt64", "System.IConvertible.ToUInt64", "System.IConvertible.ToSingle", "System.IConvertible.ToDouble", "System.IConvertible.ToDecimal", "System.IConvertible.ToDateTime", "System.IConvertible.ToType" } };

            yield return new object[] { typeof(int), BindingFlags.Static | BindingFlags.NonPublic, new string[0] };
            yield return new object[] { typeof(int), BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.NonPublic, new string[0] };
            yield return new object[] { typeof(int), BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic, new string[0] };
            yield return new object[] { typeof(int), BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.NonPublic, new string[0] };

            yield return new object[] { typeof(int), BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic, new string[] { "System.IConvertible.ToBoolean", "System.IConvertible.ToChar", "System.IConvertible.ToSByte", "System.IConvertible.ToByte", "System.IConvertible.ToInt16", "System.IConvertible.ToUInt16", "System.IConvertible.ToInt32", "System.IConvertible.ToUInt32", "System.IConvertible.ToInt64", "System.IConvertible.ToUInt64", "System.IConvertible.ToSingle", "System.IConvertible.ToDouble", "System.IConvertible.ToDecimal", "System.IConvertible.ToDateTime", "System.IConvertible.ToType", "Finalize", "MemberwiseClone" } };
            yield return new object[] { typeof(int), BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic, new string[] { "System.IConvertible.ToBoolean", "System.IConvertible.ToChar", "System.IConvertible.ToSByte", "System.IConvertible.ToByte", "System.IConvertible.ToInt16", "System.IConvertible.ToUInt16", "System.IConvertible.ToInt32", "System.IConvertible.ToUInt32", "System.IConvertible.ToInt64", "System.IConvertible.ToUInt64", "System.IConvertible.ToSingle", "System.IConvertible.ToDouble", "System.IConvertible.ToDecimal", "System.IConvertible.ToDateTime", "System.IConvertible.ToType", "Finalize", "MemberwiseClone" } };
            yield return new object[] { typeof(int), BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic, new string[] { "System.IConvertible.ToBoolean", "System.IConvertible.ToChar", "System.IConvertible.ToSByte", "System.IConvertible.ToByte", "System.IConvertible.ToInt16", "System.IConvertible.ToUInt16", "System.IConvertible.ToInt32", "System.IConvertible.ToUInt32", "System.IConvertible.ToInt64", "System.IConvertible.ToUInt64", "System.IConvertible.ToSingle", "System.IConvertible.ToDouble", "System.IConvertible.ToDecimal", "System.IConvertible.ToDateTime", "System.IConvertible.ToType" } };
            yield return new object[] { typeof(int), BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic, new string[] { "System.IConvertible.ToBoolean", "System.IConvertible.ToChar", "System.IConvertible.ToSByte", "System.IConvertible.ToByte", "System.IConvertible.ToInt16", "System.IConvertible.ToUInt16", "System.IConvertible.ToInt32", "System.IConvertible.ToUInt32", "System.IConvertible.ToInt64", "System.IConvertible.ToUInt64", "System.IConvertible.ToSingle", "System.IConvertible.ToDouble", "System.IConvertible.ToDecimal", "System.IConvertible.ToDateTime", "System.IConvertible.ToType" } };

            yield return new object[] { typeof(int), BindingFlags.Public | BindingFlags.NonPublic, new string[0] };
            yield return new object[] { typeof(int), BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic, new string[0] };
            yield return new object[] { typeof(int), BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic, new string[0] };
            yield return new object[] { typeof(int), BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.NonPublic, new string[0] };

            yield return new object[] { typeof(int), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, new string[] { "CompareTo", "CompareTo", "Equals", "Equals", "GetHashCode", "ToString", "ToString", "ToString", "ToString", "GetTypeCode", "System.IConvertible.ToBoolean", "System.IConvertible.ToChar", "System.IConvertible.ToSByte", "System.IConvertible.ToByte", "System.IConvertible.ToInt16", "System.IConvertible.ToUInt16", "System.IConvertible.ToInt32", "System.IConvertible.ToUInt32", "System.IConvertible.ToInt64", "System.IConvertible.ToUInt64", "System.IConvertible.ToSingle", "System.IConvertible.ToDouble", "System.IConvertible.ToDecimal", "System.IConvertible.ToDateTime", "System.IConvertible.ToType", "GetType", "Finalize", "MemberwiseClone" } };
            yield return new object[] { typeof(int), BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, new string[] { "CompareTo", "CompareTo", "Equals", "Equals", "GetHashCode", "ToString", "ToString", "ToString", "ToString", "GetTypeCode", "System.IConvertible.ToBoolean", "System.IConvertible.ToChar", "System.IConvertible.ToSByte", "System.IConvertible.ToByte", "System.IConvertible.ToInt16", "System.IConvertible.ToUInt16", "System.IConvertible.ToInt32", "System.IConvertible.ToUInt32", "System.IConvertible.ToInt64", "System.IConvertible.ToUInt64", "System.IConvertible.ToSingle", "System.IConvertible.ToDouble", "System.IConvertible.ToDecimal", "System.IConvertible.ToDateTime", "System.IConvertible.ToType", "GetType", "Finalize", "MemberwiseClone" } };
            yield return new object[] { typeof(int), BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, new string[] { "CompareTo", "CompareTo", "Equals", "Equals", "GetHashCode", "ToString", "ToString", "ToString", "ToString", "GetTypeCode", "System.IConvertible.ToBoolean", "System.IConvertible.ToChar", "System.IConvertible.ToSByte", "System.IConvertible.ToByte", "System.IConvertible.ToInt16", "System.IConvertible.ToUInt16", "System.IConvertible.ToInt32", "System.IConvertible.ToUInt32", "System.IConvertible.ToInt64", "System.IConvertible.ToUInt64", "System.IConvertible.ToSingle", "System.IConvertible.ToDouble", "System.IConvertible.ToDecimal", "System.IConvertible.ToDateTime", "System.IConvertible.ToType" } };
            yield return new object[] { typeof(int), BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, new string[] { "CompareTo", "CompareTo", "Equals", "Equals", "GetHashCode", "ToString", "ToString", "ToString", "ToString", "System.IConvertible.ToBoolean", "System.IConvertible.ToChar", "System.IConvertible.ToSByte", "System.IConvertible.ToByte", "System.IConvertible.ToInt16", "System.IConvertible.ToUInt16", "System.IConvertible.ToInt32", "System.IConvertible.ToUInt32", "System.IConvertible.ToInt64", "System.IConvertible.ToUInt64", "System.IConvertible.ToSingle", "System.IConvertible.ToDouble", "System.IConvertible.ToDecimal", "System.IConvertible.ToDateTime", "System.IConvertible.ToType" } };

            yield return new object[] { typeof(int), BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, new string[] { "Parse", "Parse", "Parse", "Parse", "TryParse", "TryParse" } };
            yield return new object[] { typeof(int), BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, new string[] { "Parse", "Parse", "Parse", "Parse", "TryParse", "TryParse" } };
            yield return new object[] { typeof(int), BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, new string[] { "Parse", "Parse", "Parse", "Parse", "TryParse", "TryParse" } };
            yield return new object[] { typeof(int), BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, new string[] { "Parse", "Parse", "Parse", "Parse", "TryParse", "TryParse" } };

            yield return new object[] { typeof(int), BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, new string[] { "CompareTo", "CompareTo", "Equals", "Equals", "GetHashCode", "ToString", "ToString", "ToString", "ToString", "Parse", "Parse", "Parse", "Parse", "TryParse", "TryParse", "GetTypeCode", "System.IConvertible.ToBoolean", "System.IConvertible.ToChar", "System.IConvertible.ToSByte", "System.IConvertible.ToByte", "System.IConvertible.ToInt16", "System.IConvertible.ToUInt16", "System.IConvertible.ToInt32", "System.IConvertible.ToUInt32", "System.IConvertible.ToInt64", "System.IConvertible.ToUInt64", "System.IConvertible.ToSingle", "System.IConvertible.ToDouble", "System.IConvertible.ToDecimal", "System.IConvertible.ToDateTime", "System.IConvertible.ToType", "GetType", "Finalize", "MemberwiseClone" } };
            yield return new object[] { typeof(int), BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, new string[] { "CompareTo", "CompareTo", "Equals", "Equals", "GetHashCode", "ToString", "ToString", "ToString", "ToString", "Parse", "Parse", "Parse", "Parse", "TryParse", "TryParse", "GetTypeCode", "System.IConvertible.ToBoolean", "System.IConvertible.ToChar", "System.IConvertible.ToSByte", "System.IConvertible.ToByte", "System.IConvertible.ToInt16", "System.IConvertible.ToUInt16", "System.IConvertible.ToInt32", "System.IConvertible.ToUInt32", "System.IConvertible.ToInt64", "System.IConvertible.ToUInt64", "System.IConvertible.ToSingle", "System.IConvertible.ToDouble", "System.IConvertible.ToDecimal", "System.IConvertible.ToDateTime", "System.IConvertible.ToType", "GetType", "Finalize", "MemberwiseClone" } };
            yield return new object[] { typeof(int), BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, new string[] { "CompareTo", "CompareTo", "Equals", "Equals", "GetHashCode", "ToString", "ToString", "ToString", "ToString", "Parse", "Parse", "Parse", "Parse", "TryParse", "TryParse", "GetTypeCode", "System.IConvertible.ToBoolean", "System.IConvertible.ToChar", "System.IConvertible.ToSByte", "System.IConvertible.ToByte", "System.IConvertible.ToInt16", "System.IConvertible.ToUInt16", "System.IConvertible.ToInt32", "System.IConvertible.ToUInt32", "System.IConvertible.ToInt64", "System.IConvertible.ToUInt64", "System.IConvertible.ToSingle", "System.IConvertible.ToDouble", "System.IConvertible.ToDecimal", "System.IConvertible.ToDateTime", "System.IConvertible.ToType" } };
            yield return new object[] { typeof(int), BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, new string[] { "CompareTo", "CompareTo", "Equals", "Equals", "GetHashCode", "ToString", "ToString", "ToString", "ToString", "Parse", "Parse", "Parse", "Parse", "TryParse", "TryParse", "System.IConvertible.ToBoolean", "System.IConvertible.ToChar", "System.IConvertible.ToSByte", "System.IConvertible.ToByte", "System.IConvertible.ToInt16", "System.IConvertible.ToUInt16", "System.IConvertible.ToInt32", "System.IConvertible.ToUInt32", "System.IConvertible.ToInt64", "System.IConvertible.ToUInt64", "System.IConvertible.ToSingle", "System.IConvertible.ToDouble", "System.IConvertible.ToDecimal", "System.IConvertible.ToDateTime", "System.IConvertible.ToType" } };

            yield return new object[] { typeof(int), BindingFlags.FlattenHierarchy, new string[0] };
        }

        [Theory]
        [MemberData(nameof(GetMethods_TestData))]
        public void GetMethods(Type type, BindingFlags bindingAttributes, string[] expectedNames)
        {
            if (bindingAttributes == BindingFlags.Default)
            {
                string[] methodNames = type.GetMethods().Select(method => method.Name).ToArray();
                Assert.All(expectedNames, name => methodNames.Contains(name));
            }
            else
            {
                string[] methodNames = type.GetMethods(bindingAttributes).Select(method => method.Name).ToArray();
                Assert.All(expectedNames, name => methodNames.Contains(name));
            }
        }

        public static IEnumerable<object[]> GetProperties_TestData()
        {
            yield return new object[] { typeof(GenericClassWithVarArgMethod<string>), BindingFlags.Default, new string[] { "publicField" } };
            yield return new object[] { typeof(Cat<int>), BindingFlags.Default, new string[] { "StuffConsumed" } };
            yield return new object[] { typeof(GenericClassWithVarArgMethod<int>), BindingFlags.Default, new string[] { "publicField" } };
            yield return new object[] { typeof(GenericClassWithVarArgMethod<>), BindingFlags.Default, new string[] { "publicField" } };
            yield return new object[] { typeof(ClassWithVarArgMethod), BindingFlags.Default, new string[] { "publicField" } };

            yield return new object[] { typeof(string), BindingFlags.IgnoreCase, new string[0] };
            yield return new object[] { typeof(string), BindingFlags.DeclaredOnly, new string[0] };
            yield return new object[] { typeof(string), BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly, new string[0] };

            yield return new object[] { typeof(string), BindingFlags.Instance, new string[0] };
            yield return new object[] { typeof(string), BindingFlags.IgnoreCase | BindingFlags.Instance, new string[0] };
            yield return new object[] { typeof(string), BindingFlags.DeclaredOnly | BindingFlags.Instance, new string[0] };
            yield return new object[] { typeof(string), BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Instance, new string[0] };

            yield return new object[] { typeof(string), BindingFlags.Static, new string[0] };
            yield return new object[] { typeof(string), BindingFlags.IgnoreCase | BindingFlags.Static, new string[0] };
            yield return new object[] { typeof(string), BindingFlags.DeclaredOnly | BindingFlags.Static, new string[0] };
            yield return new object[] { typeof(string), BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Static, new string[0] };

            yield return new object[] { typeof(string), BindingFlags.Instance | BindingFlags.Static, new string[0] };
            yield return new object[] { typeof(string), BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static, new string[0] };
            yield return new object[] { typeof(string), BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static, new string[0] };
            yield return new object[] { typeof(string), BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static, new string[0] };

            yield return new object[] { typeof(string), BindingFlags.Public, new string[0] };
            yield return new object[] { typeof(string), BindingFlags.IgnoreCase | BindingFlags.Public, new string[0] };
            yield return new object[] { typeof(string), BindingFlags.DeclaredOnly | BindingFlags.Public, new string[0] };
            yield return new object[] { typeof(string), BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Public, new string[0] };

            yield return new object[] { typeof(string), BindingFlags.Instance | BindingFlags.Public, new string[] { "Chars", "Length" } };
            yield return new object[] { typeof(string), BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public, new string[] { "Chars", "Length" } };
            yield return new object[] { typeof(string), BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public, new string[] { "Chars", "Length" } };
            yield return new object[] { typeof(string), BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public, new string[] { "Chars", "Length" } };

            yield return new object[] { typeof(string), BindingFlags.Public | BindingFlags.Static, new string[0] };
            yield return new object[] { typeof(string), BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Static, new string[0] };
            yield return new object[] { typeof(string), BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static, new string[0] };
            yield return new object[] { typeof(string), BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static, new string[0] };

            yield return new object[] { typeof(string), BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static, new string[] { "Chars", "Length" } };
            yield return new object[] { typeof(string), BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static, new string[] { "Chars", "Length" } };
            yield return new object[] { typeof(string), BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static, new string[] { "Chars", "Length" } };
            yield return new object[] { typeof(string), BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static, new string[] { "Chars", "Length" } };

            yield return new object[] { typeof(string), BindingFlags.NonPublic, new string[0] };
            yield return new object[] { typeof(string), BindingFlags.IgnoreCase | BindingFlags.NonPublic, new string[0] };
            yield return new object[] { typeof(string), BindingFlags.DeclaredOnly | BindingFlags.NonPublic, new string[0] };
            yield return new object[] { typeof(string), BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.NonPublic, new string[0] };

            yield return new object[] { typeof(string), BindingFlags.Instance | BindingFlags.NonPublic, new string[] { "FirstChar" } };
            yield return new object[] { typeof(string), BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic, new string[] { "FirstChar" } };
            yield return new object[] { typeof(string), BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic, new string[] { "FirstChar" } };
            yield return new object[] { typeof(string), BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic, new string[] { "FirstChar" } };

            yield return new object[] { typeof(string), BindingFlags.Public | BindingFlags.NonPublic, new string[0] };
            yield return new object[] { typeof(string), BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic, new string[0] };
            yield return new object[] { typeof(string), BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic, new string[0] };
            yield return new object[] { typeof(string), BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic, new string[0] };

            yield return new object[] { typeof(string), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, new string[] { "FirstChar", "Chars", "Length" } };
            yield return new object[] { typeof(string), BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, new string[] { "FirstChar", "Chars", "Length" } };
            yield return new object[] { typeof(string), BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, new string[] { "FirstChar", "Chars", "Length" } };
            yield return new object[] { typeof(string), BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, new string[] { "FirstChar", "Chars", "Length" } };

            yield return new object[] { typeof(string), BindingFlags.FlattenHierarchy, new string[0] };
        }

        [Theory]
        [MemberData(nameof(GetProperties_TestData))]
        public void GetProperties(Type type, BindingFlags bindingAttributes, string[] expectedNames)
        {
            if (bindingAttributes == BindingFlags.Default)
            {
                string[] propertyNames = type.GetProperties().Select(method => method.Name).ToArray();
                Assert.All(expectedNames, name => propertyNames.Contains(name));
            }
            else
            {
                string[] propertyNames = type.GetProperties(bindingAttributes).Select(method => method.Name).ToArray();
                Assert.All(expectedNames, name => propertyNames.Contains(name));
            }
        }

        public static IEnumerable<object[]> GetProperty_TestData()
        {
            yield return new object[] { typeof(GenericClassWithVarArgMethod<string>), "publicField", BindingFlags.Default, typeof(string), new Type[0] };
            yield return new object[] { typeof(Cat<int>), "StuffConsumed", BindingFlags.Default, null, new Type[0] };
            yield return new object[] { typeof(GenericClassWithVarArgMethod<int>), "publicField", BindingFlags.Default, typeof(int), new Type[0] };
            yield return new object[] { typeof(GenericClassWithVarArgMethod<>), "publicField", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, new Type[0] };
            yield return new object[] { typeof(GenericClassWithVarArgMethod<>), "publicField", BindingFlags.Default, null, new Type[0] };
            yield return new object[] { typeof(ClassWithVarArgMethod), "publicField", BindingFlags.Default, null, new Type[0] };
        }

        [Theory]
        [MemberData(nameof(GetProperty_TestData))]
        public void GetProperty(Type type, string name, BindingFlags bindingAttributes, Type returnType, Type[] types)
        {
            if (returnType == null)
            {
                if (bindingAttributes == BindingFlags.Default)
                {
                    Assert.Equal(name, type.GetProperty(name).Name);
                }
                else
                {
                    Assert.Equal(name, type.GetProperty(name, bindingAttributes).Name);
                }
            }
            else
            {
                if (types.Length == 0)
                {
                    Assert.Equal(name, type.GetProperty(name, returnType).Name);
                }
                Assert.Equal(name, type.GetProperty(name, returnType, types).Name);
            }
        }

        public static IEnumerable<object[]> IsAssignableFrom_TestData()
        {
            yield return new object[] { typeof(TI_BaseClassWithInterface), null, false };
            yield return new object[] { typeof(IList<object>), typeof(object[]), true };
            yield return new object[] { typeof(object[]), typeof(IList<object>), false };

            yield return new object[] { typeof(TI_BaseClassWithInterface), typeof(TI_SubClassWithInterface), true };
            yield return new object[] { typeof(TI_BaseClassWithInterface[]), typeof(TI_SubClassWithInterface[]), true };
            yield return new object[] { typeof(IList<object>), typeof(TI_BaseClassWithInterface[]), true };
            yield return new object[] { typeof(IList<TI_BaseClassWithInterface>), typeof(TI_BaseClassWithInterface[]), true };
            yield return new object[] { typeof(IList<TI_BaseClassWithInterface>), typeof(TI_SubClassWithInterface[]), true };
            yield return new object[] { typeof(IList<TI_SubClassWithInterface>), typeof(TI_SubClassWithInterface[]), true };

            yield return new object[] { typeof(TI_GenericBaseClass<object>), typeof(TI_GenericSubSubClass<object>), true };
            yield return new object[] { typeof(TI_GenericSubClass<string>), typeof(TI_GenericSubSubClass<string>), true };
            yield return new object[] { typeof(TI_GenericSubClass<string>), typeof(TI_GenericSubClass<string>), true };
            yield return new object[] { typeof(TI_GenericSubClass<string>), typeof(TI_GenericSubClass<object>), false };
            yield return new object[] { typeof(TI_GenericSubClass<object>), typeof(TI_GenericSubClass<string>), false };
            yield return new object[] { typeof(TI_GenericSubSubClass<object>), typeof(TI_GenericSubClass<object>), false };
            yield return new object[] { typeof(TI_GenericSubClass<string>), typeof(TI_GenericBaseClass<string>), false };

            yield return new object[] { typeof(TI_Interface2), typeof(TI_Interface2), true };
            yield return new object[] { typeof(TI_Interface2), typeof(TI_BaseClassWithInterface), true };
            yield return new object[] { typeof(TI_Interface2), typeof(TI_SubClassWithInterface), true };
            yield return new object[] { typeof(TI_Interface2), typeof(TI_GenericSubClassWithInterface<>), true };
            yield return new object[] { typeof(TI_Interface2), typeof(TI_GenericSubClassWithInterface<string>), true };
            yield return new object[] { typeof(TI_SubClassWithInterface), typeof(TI_Interface1), false };

            yield return new object[] { typeof(TI_Interface1), typeof(TI_GenericSubClassWithConstraints<>).GetGenericArguments()[0], true };
            yield return new object[] { typeof(TI_Interface2), typeof(TI_GenericSubClassWithConstraints<>).GetGenericArguments()[0], true };
            yield return new object[] { typeof(TI_GenericSubClassWithInterface<>), typeof(TI_GenericSubClassWithConstraints<>).GetGenericArguments()[0], false };

            yield return new object[] { typeof(CustomNamespace.TI_AbstractBaseClass), typeof(CustomNamespace.TI_AbstractSubClass), true };
            yield return new object[] { typeof(CustomNamespace.TI_AbstractBaseClass), typeof(CustomNamespace.TI_SubClass), true };
            yield return new object[] { typeof(CustomNamespace.TI_AbstractSubClass), typeof(CustomNamespace.TI_SubClass), true };

            yield return new object[] { typeof(TI_GenericClassWithConstraints<>).GetGenericArguments()[0], typeof(TI_Interface1), false };
            yield return new object[] { typeof(TI_GenericClassWithConstraints<>).GetGenericArguments()[0], typeof(TI_BaseClassWithInterface), false };
            yield return new object[] { typeof(TI_Interface1), typeof(TI_GenericClassWithConstraints<>).GetGenericArguments()[0], true };
            yield return new object[] { typeof(TI_BaseClassWithInterface), typeof(TI_GenericClassWithConstraints<>).GetGenericArguments()[0], false };
            yield return new object[] { typeof(TI_Interface1), typeof(TI_GenericSubClassWithConstraints<>).GetGenericArguments()[0], true };
            yield return new object[] { typeof(TI_Interface2), typeof(TI_GenericSubClassWithConstraints<>).GetGenericArguments()[0], true };

            // A T[] is assignable to IList<U> iff T[] is assignable to U[]
            yield return new object[] { typeof(TI_Interface1[]), typeof(TI_StructWithInterface[]), false };
            yield return new object[] { typeof(TI_Interface1[]), typeof(TI_SubClassWithInterface[]), true };
            yield return new object[] { typeof(IList<TI_Interface1>), typeof(TI_StructWithInterface[]), false };
            yield return new object[] { typeof(IList<TI_Interface1>), typeof(TI_SubClassWithInterface[]), true };

            yield return new object[] { typeof(int[]), typeof(uint[]), true };
            yield return new object[] { typeof(uint[]), typeof(int[]), true };
            yield return new object[] { typeof(IList<int>), typeof(uint[]), true };
            yield return new object[] { typeof(IList<uint>), typeof(int[]), true };
        }

        [Theory]
        [MemberData(nameof(IsAssignableFrom_TestData))]
        public void IsAssignableFrom(Type type, Type type2, bool expected)
        {
            TypeInfo typeInfo = type.GetTypeInfo();
            Assert.Equal(expected, typeInfo.IsAssignableFrom(type2?.GetTypeInfo()));
            Assert.True(typeInfo.IsAssignableFrom(typeInfo));
        }

        [Theory]
        [InlineData(typeof(TI_Class))]
        public void Properties_SameAsUnderlyingType(Type type)
        {
            TypeInfo typeInfo = type.GetTypeInfo();
            Assert.Equal(type.Name, typeInfo.Name);
            Assert.Equal(type.FullName, typeInfo.FullName);
        }

        [Theory]
        [InlineData(typeof(TI_Class))]
        public void Methods_SameAsUnderlyingType(Type type)
        {
            TypeInfo typeInfo = type.GetTypeInfo();
            BindingFlags declaredFlags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

            Assert.Equal(type.GetMethod("ProtectedMethod", declaredFlags), typeInfo.GetDeclaredMethods("ProtectedMethod").First());
            Assert.Equal(type.GetNestedType("PublicNestedType", declaredFlags), typeInfo.GetDeclaredNestedType("PublicNestedType").AsType());
            Assert.Equal(type.GetProperty("PrivateProperty", declaredFlags), typeInfo.GetDeclaredProperty("PrivateProperty"));

            Assert.All(type.GetFields(declaredFlags), field => typeInfo.DeclaredFields.Contains(field));
            Assert.All(type.GetMethods(declaredFlags), method => typeInfo.DeclaredMethods.Contains(method));
            Assert.Equal(type.GetNestedTypes(declaredFlags), typeInfo.DeclaredNestedTypes.Select(aTypeInfo => aTypeInfo.AsType()));
            Assert.All(type.GetProperties(declaredFlags), method => typeInfo.DeclaredProperties.Contains(method));
            Assert.All(type.GetEvents(declaredFlags), method => typeInfo.DeclaredEvents.Contains(method));
            Assert.All(type.GetConstructors(declaredFlags), method => typeInfo.DeclaredConstructors.Contains(method));

            Assert.Equal(type.GetEvents(), typeInfo.AsType().GetEvents());
            Assert.Equal(type.GetFields(), typeInfo.AsType().GetFields());
            Assert.Equal(type.GetMethods(), typeInfo.AsType().GetMethods());
            Assert.Equal(type.GetProperties(), typeInfo.AsType().GetProperties());
            Assert.Equal(type.GetType(), typeInfo.GetType());
            Assert.True(type.Equals(typeInfo));

            BindingFlags allFlags = BindingFlags.DeclaredOnly | BindingFlags.FlattenHierarchy | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
            Assert.Equal(type.GetNestedTypes(allFlags), typeInfo.AsType().GetNestedTypes(allFlags));
        }
    }

#pragma warning disable 0169, 0067, 0649
    public class TI_Class
    {
        public int PublicField;
        protected int ProtectedField;
        private int _privateField;
        internal int InternalField;

        public static int PublicStaticField;
        protected static int ProtectedStaticField;
        private static int s_privateStaticField;
        internal static int InternalStaticField;

        public TI_Class() { }
        protected TI_Class(int i) { }
        private TI_Class(int i, int j) { }
        internal TI_Class(int i, int j, int k) { }

        public void PublicMethod() { }
        protected void ProtectedMethod() { }
        private void PrivateMethod() { }
        internal void InternalMethod() { }

        public static void PublicStaticMethod() { }
        protected static void ProtectedStaticMethod() { }
        private static void PrivateStaticMethod() { }
        internal static void InternalStaticMethod() { }

        public class PublicNestedType { }
        protected class ProtectedNestedType { }
        private class PrivateNestedType { }
        internal class InternalNestedType { }

        public int PublicProperty { get { return default(int); } set { } }
        protected int ProtectedProperty { get { return default(int); } set { } }
        private int PrivateProperty { get { return default(int); } set { } }
        internal int InternalProperty { get { return default(int); } set { } }
    }

    [DefaultMember("ReturnAndSetField")]
    public class GenericClass<T>
    {
        public T field;

        public GenericClass(T a) { field = a; }

        public T ReturnAndSetField(T newFieldValue)
        {
            field = newFieldValue;
            return field;
        }
    }

    public struct GenericStruct2TP<T, W>
    {
        public T field;
        public W field2;

        public T ReturnAndSetField1(T newFieldValue)
        {
            field = newFieldValue;
            return field;
        }

        public W ReturnAndSetField2(W newFieldValue)
        {
            field2 = newFieldValue;
            return field2;
        }
    }

    public class GenericClassWithInterface<T> : IGenericInterface<T>
    {
        public T field;

        public GenericClassWithInterface(T a)
        {
            field = a;
        }

        public T ReturnAndSetFieldZero(T newFieldValue)
        {
            field = newFieldValue;
            return field;
        }

        public W GenericMethod<W>(W a) => a;
    }

    public class NonGenericClassWithGenericInterface : IGenericInterface<int>
    {
        public int field;

        public int ReturnAndSetFieldZero(int newFieldValue)
        {
            field = newFieldValue;
            return field;
        }
    }

    public struct GenericStructWithInterface<T> : IGenericInterface<T>
    {
        public T field;
        public int field2;

        public GenericStructWithInterface(T a)
        {
            field = a;
            field2 = 0;
        }

        public GenericStructWithInterface(T a, int b)
        {
            field = a;
            field2 = b;
        }

        public T ReturnAndSetFieldZero(T newFieldValue)
        {
            field = newFieldValue;
            return field;
        }
    }

    public interface IGenericInterface<T>
    {
        T ReturnAndSetFieldZero(T newFieldValue);
    }

    public interface IGenericInterface2<T, W>
    {
        void SetFieldOne(T newFieldValue);
        void SetFieldTwo(W newFieldValue);
    }

    public interface IGenericInterfaceInherits<U, V> : IGenericInterface<U>, IGenericInterface2<V, U>
    {
        V ReturnAndSetFieldThree(V newFieldValue);
    }

    public class GenericClassUsingNestedInterfaces<X, Y> : IGenericInterfaceInherits<X, Y>
    {
        public X FieldZero;
        public X FieldOne;
        public Y FieldTwo;
        public Y FieldThree;

        public GenericClassUsingNestedInterfaces(X a, X b, Y c, Y d)
        {
            FieldZero = a;
            FieldOne = b;
            FieldTwo = c;
            FieldThree = d;
        }

        public X ReturnAndSetFieldZero(X newFieldValue)
        {
            FieldZero = newFieldValue;
            return FieldZero;
        }

        public void SetFieldOne(Y newFieldValue) => FieldTwo = newFieldValue;

        public void SetFieldTwo(X newFieldValue) => FieldOne = newFieldValue;

        public Y ReturnAndSetFieldThree(Y newFieldValue)
        {
            FieldThree = newFieldValue;
            return FieldThree;
        }
    }

    public class GenericClassWithVarArgMethod<T>
    {
        public T field;

        public T publicField
        {
            get { return field; }
            set { field = value; }
        }

        public T ReturnAndSetField(T newFieldValue, params T[] moreFieldValues)
        {
            field = newFieldValue;

            for (int i = 0; i <= moreFieldValues.Length - 1; i++)
            {
                field = moreFieldValues[i];
            }

            return field;
        }
    }

    public class ClassWithVarArgMethod
    {
        public int field;

        public int publicField
        {
            get { return field; }
            set { field = value; }
        }

        public int ReturnAndSetField(int newFieldValue, params int[] moreFieldValues)
        {
            field = newFieldValue;

            for (int i = 0; i <= moreFieldValues.Length - 1; i++)
            {
                field = moreFieldValues[i];
            }

            return field;
        }
    }

    public interface IConsume
    {
        object[] StuffConsumed { get; }

        void Eat(object ThingEaten);

        object[] Puke(int Amount);
    }

    public class PackOfCarnivores<T> where T : IConsume
    {
        public T[] pPack;
    }

    public class Cat<C> : IConsume
    {
        private List<object> _pStuffConsumed = new List<object>();

        public event EventHandler WeightChanged;

        private event EventHandler WeightStayedTheSame;

        private static EventHandler s_catDisappeared;

        public object[] StuffConsumed { get { return PStuffConsumed.ToArray(); } }

        public List<object> PStuffConsumed
        {
            get
            {
                return _pStuffConsumed;
            }

            set
            {
                _pStuffConsumed = value;
            }
        }

        public void Eat(object ThingEaten) => PStuffConsumed.Add(ThingEaten);

        public object[] Puke(int Amount)
        {
            object[] vomit;
            if (PStuffConsumed.Count < Amount)
            {
                Amount = PStuffConsumed.Count;
            }
            vomit = PStuffConsumed.GetRange(PStuffConsumed.Count - Amount, Amount).ToArray();
            PStuffConsumed.RemoveRange(PStuffConsumed.Count - Amount, Amount);
            return vomit;
        }
    }

    public class GenericArrayWrapperClass<T>
    {
        private T[] _field;
        private int _field1;

        public int myProperty
        {
            get { return 0; }
            set { _field1 = value; }
        }

        public GenericArrayWrapperClass(T[] fieldValues)
        {
            int size = fieldValues.Length;
            _field = new T[size];
            for (int i = 0; i < _field.Length; i++)
            {
                _field[i] = fieldValues[i];
            }
        }

        public T this[int index]
        {
            get { return _field[index]; }
            set { _field[index] = value; }
        }
    }

    public class GenericOuterClass<T>
    {
        public T field;

        public class GenericNestedClass<W>
        {
            public T field;
            public W field2;
        }

        public class NestedClass
        {
            public T field;
            public int field2;
        }
    }

    public class OuterClass
    {
        public int field;

        public class GenericNestedClass<W>
        {
            public int field;
            public W field2;
        }

        public class NestedClass
        {
            public string field;
            public int field2;
        }
    }

    internal interface TI_Interface1 { }
    internal interface TI_Interface2 { }

    internal struct TI_StructWithInterface : TI_Interface1 { }

    internal class TI_BaseClassWithInterface : TI_Interface1, TI_Interface2 { }
    internal class TI_SubClassWithInterface : TI_BaseClassWithInterface { }
    internal class TI_GenericSubClassWithInterface<T> : TI_SubClassWithInterface { }

    internal class TI_GenericBaseClass<T> { }
    internal class TI_GenericSubClass<T> : TI_GenericBaseClass<T> { }
    internal class TI_GenericSubSubClass<T> : TI_GenericSubClass<T> { }

    internal class TI_GenericSubClassWithConstraints<T> where T : TI_GenericSubClassWithInterface<T>, TI_Interface1, TI_Interface2 { }

    namespace CustomNamespace
    {
        internal abstract class TI_AbstractBaseClass { }
        internal abstract class TI_AbstractSubClass : TI_AbstractBaseClass { }
        internal class TI_SubClass : TI_AbstractSubClass { }
    }

    internal class TI_GenericClassWithConstraints<T> where T : TI_Interface1 { }

#pragma warning restore 0169, 0067, 0649
}
