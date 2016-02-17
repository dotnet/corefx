// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;

using Xunit;

#pragma warning disable 0414

namespace System.Reflection.Tests
{
    public partial class FieldInfoTests
    {
        [Theory]
        [InlineData(typeof(FieldInfoTests), "intField", FieldAttributes.Public)]
        [InlineData(typeof(FieldInfoTests), "s_intField", FieldAttributes.Public | FieldAttributes.Static)]
        [InlineData(typeof(FieldInfoTests), "privateIntField", FieldAttributes.Private)]
        [InlineData(typeof(FieldInfoTests), "readonlyIntField", FieldAttributes.Public | FieldAttributes.InitOnly)]
        public void Attributes(Type type, string name, FieldAttributes expected)
        {
            FieldInfo fieldInfo = GetField(type, name);
            Assert.Equal(expected, fieldInfo.Attributes);
        }

        [InlineData(typeof(Int32Attr))]
        [InlineData(typeof(Int64Attr))]
        [InlineData(typeof(StringAttr))]
        [InlineData(typeof(EnumAttr))]
        [InlineData(typeof(TypeAttr))]
        [InlineData(typeof(Attr))]
        private static void CustomAttributes(Type type)
        {
            FieldInfo fieldInfo = GetField(typeof(FieldInfoTests), "fieldWithAttributes");
            IEnumerable<CustomAttributeData> customAttrs = fieldInfo.CustomAttributes;
            bool result = customAttrs.Any(customAttribute => customAttribute.AttributeType.Equals(type));
            Assert.True(result, string.Format("Did not find custom attribute of type {0}.", type));
        }

        [Theory]
        [InlineData(typeof(FieldInfoTests), "intField", typeof(int))]
        [InlineData(typeof(FieldInfoTests), "stringField", typeof(string))]
        [InlineData(typeof(FieldInfoTests), "s_assemblyField1", typeof(object))]
        public void FieldType(Type type, string name, Type expected)
        {
            FieldInfo fieldInfo = GetField(type, name);
            Assert.Equal(expected, fieldInfo.FieldType);
        }

        public static IEnumerable<object[]> GetValue_TestData()
        {
            yield return new object[] { typeof(FieldInfoTests), "s_intField", new FieldInfoTests(), 100 };
            yield return new object[] { typeof(FieldInfoTests), "s_intField", null, 100 };
            yield return new object[] { typeof(FieldInfoTests), "intField", new FieldInfoTests(), 101 };
            yield return new object[] { typeof(FieldInfoTests), "s_stringField", new FieldInfoTests(), "static" };
            yield return new object[] { typeof(FieldInfoTests), "stringField", new FieldInfoTests(), "non static" };
        }

        [Theory]
        [MemberData("GetValue_TestData")]
        public void GetValue(Type type, string name, object obj, object expected)
        {
            FieldInfo fieldInfo = GetField(type, name);
            Assert.Equal(expected, fieldInfo.GetValue(obj));
        }

        public static IEnumerable<object[]> GetValue_Invalid_TestData()
        {
            yield return new object[] { typeof(FieldInfoTests), "stringField", null, typeof(Exception) }; // Win8p throws Exception, not TargetException
            yield return new object[] { typeof(FieldInfoTests), "stringField", new object(), typeof(ArgumentException) }; // Invalid object
        }

        [Theory]
        [MemberData("GetValue_Invalid_TestData")]
        public void GetValue_Invalid(Type type, string name, object obj, Type exceptionType)
        {
            FieldInfo fieldInfo = GetField(type, name);
            if (exceptionType.Equals(typeof(Exception)))
            {
                Assert.ThrowsAny<Exception>(() => fieldInfo.GetValue(obj));
            }
            else
            {
                Assert.Throws(exceptionType, () => fieldInfo.GetValue(obj));
            }
        }

        [Theory]
        [InlineData(typeof(FieldInfoTests), "s_assemblyField1", false)]
        [InlineData(typeof(FieldInfoTests), "s_assemblyField2", false)]
        [InlineData(typeof(FieldInfoTests), "s_assemblyField3", false)]
        [InlineData(typeof(FieldInfoTests), "s_assemblyField4", false)]
        [InlineData(typeof(FieldInfoTests), "s_assemblyField5", true)]
        public void IsAssembly(Type type, string name, bool expected)
        {
            FieldInfo fieldInfo = GetField(type, name);
            Assert.Equal(expected, fieldInfo.IsAssembly);
        }

        [Theory]
        [InlineData(typeof(FieldInfoTests), "s_assemblyField1", false)]
        [InlineData(typeof(FieldInfoTests), "s_assemblyField2", false)]
        [InlineData(typeof(FieldInfoTests), "s_assemblyField3", true)]
        [InlineData(typeof(FieldInfoTests), "s_assemblyField4", false)]
        [InlineData(typeof(FieldInfoTests), "s_assemblyField5", false)]
        public void IsFamily(Type type, string name, bool expected)
        {
            FieldInfo fieldInfo = GetField(type, name);
            Assert.Equal(expected, fieldInfo.IsFamily);
        }

        [Theory]
        [InlineData(typeof(FieldInfoTests), "s_assemblyField1", false)]
        [InlineData(typeof(FieldInfoTests), "s_assemblyField2", false)]
        [InlineData(typeof(FieldInfoTests), "s_assemblyField3", false)]
        [InlineData(typeof(FieldInfoTests), "s_assemblyField4", false)]
        [InlineData(typeof(FieldInfoTests), "s_assemblyField5", false)]
        public void IsFamilyAndAssembly(Type type, string name, bool expected)
        {
            FieldInfo fieldInfo = GetField(type, name);
            Assert.Equal(expected, fieldInfo.IsFamilyAndAssembly);
        }

        [Theory]
        [InlineData(typeof(FieldInfoTests), "s_assemblyField1", false)]
        [InlineData(typeof(FieldInfoTests), "s_assemblyField2", false)]
        [InlineData(typeof(FieldInfoTests), "s_assemblyField3", false)]
        [InlineData(typeof(FieldInfoTests), "s_assemblyField4", false)]
        [InlineData(typeof(FieldInfoTests), "s_assemblyField5", false)]
        public void IsFamilyOrAssembly(Type type, string name, bool expected)
        {
            FieldInfo fieldInfo = GetField(type, name);
            Assert.Equal(expected, fieldInfo.IsFamilyOrAssembly);
        }

        [Theory]
        [InlineData(typeof(FieldInfoTests), "readonlyIntField", true)]
        [InlineData(typeof(FieldInfoTests), "intField", false)]
        public void IsInitOnly(Type type, string name, bool expected)
        {
            FieldInfo fieldInfo = GetField(type, name);
            Assert.Equal(expected, fieldInfo.IsInitOnly);
        }

        [Theory]
        [InlineData(typeof(FieldInfoTests), "ConstIntField", true)]
        [InlineData(typeof(FieldInfoTests), "intField", false)]
        public void IsLiteral(Type type, string name, bool expected)
        {
            FieldInfo fieldInfo = GetField(type, name);
            Assert.Equal(expected, fieldInfo.IsLiteral);
        }

        [Theory]
        [InlineData(typeof(FieldInfoTests), "stringField", true)]
        [InlineData(typeof(FieldInfoTests), "s_intField", true)]
        [InlineData(typeof(FieldInfoTests), "privateIntField", false)]
        [InlineData(typeof(FieldInfoTests), "privateStringField", false)]
        public void IsPublic(Type type, string name, bool expected)
        {
            FieldInfo fieldInfo = GetField(type, name);
            Assert.Equal(expected, fieldInfo.IsPublic);
        }

        [Theory]
        [InlineData(typeof(FieldInfoTests), "privateIntField", true)]
        [InlineData(typeof(FieldInfoTests), "privateStringField", true)]
        [InlineData(typeof(FieldInfoTests), "stringField", false)]
        [InlineData(typeof(FieldInfoTests), "s_intField", false)]
        public void IsPrivate(Type type, string name, bool expected)
        {
            FieldInfo fieldInfo = GetField(type, name);
            Assert.Equal(expected, fieldInfo.IsPrivate);
        }

        [Theory]
        [InlineData(typeof(FieldInfoTests), "intField", false)]
        public void IsSpecialName(Type type, string name, bool expected)
        {
            FieldInfo fieldInfo = GetField(type, name);
            Assert.Equal(expected, fieldInfo.IsSpecialName);
        }

        [Theory]
        [InlineData(typeof(FieldInfoTests), "s_intField", true)]
        [InlineData(typeof(FieldInfoTests), "intField", false)]
        [InlineData(typeof(FieldInfoTests), "s_stringField", true)]
        [InlineData(typeof(FieldInfoTests), "stringField", false)]
        [InlineData(typeof(FieldInfoTests), "privateIntField", false)]
        public void IsStatic(Type type, string name, bool expected)
        {
            FieldInfo fieldInfo = GetField(type, name);
            Assert.Equal(expected, fieldInfo.IsStatic);
        }

        public static IEnumerable<object[]> SetValue_TestData()
        {
            yield return new object[] { typeof(FieldInfoTests), "s_intField", new FieldInfoTests(), 1000 };
            yield return new object[] { typeof(FieldInfoTests), "s_intField", null, 1000 };
            yield return new object[] { typeof(FieldInfoTests), "intField", new FieldInfoTests(), 1000 };
            yield return new object[] { typeof(FieldInfoTests), "s_stringField", new FieldInfoTests(), "new" };
            yield return new object[] { typeof(FieldInfoTests), "stringField", new FieldInfoTests(), "new" };
        }

        [Theory]
        [MemberData("SetValue_TestData")]
        public void SetValue(Type type, string name, object obj, object value)
        {
            FieldInfo fieldInfo = GetField(type, name);
            object original = fieldInfo.GetValue(obj);
            try
            {
                fieldInfo.SetValue(obj, value);
                Assert.Equal(value, fieldInfo.GetValue(obj));
            }
            finally
            {
                fieldInfo.SetValue(obj, original);
            }
        }

        public static IEnumerable<object[]> SetValue_Invalid_TestData()
        {
            yield return new object[] { typeof(FieldInfoTests), "stringField", null, "new", typeof(Exception) }; // Win8p throws Exception, not TargetException
            yield return new object[] { typeof(FieldInfoTests), "stringField", new object(), "new", typeof(ArgumentException) }; // Invalid object
            yield return new object[] { typeof(FieldInfoTests), "stringField", new FieldInfoTests(), 100, typeof(ArgumentException) }; // Invalid value
        }

        [Theory]
        [MemberData("SetValue_Invalid_TestData")]
        public void SetValue_Invalid(Type type, string name, object obj, object value, Type exceptionType)
        {
            FieldInfo fieldInfo = GetField(type, name);
            if (exceptionType.Equals(typeof(Exception)))
            {
                Assert.ThrowsAny<Exception>(() => fieldInfo.SetValue(obj, value));
            }
            else
            {
                Assert.Throws(exceptionType, () => fieldInfo.SetValue(obj, value));
            }
        }

        [Theory]
        [InlineData(typeof(FieldInfoTests), "stringField", typeof(FieldInfoTests), "stringField", true)]
        [InlineData(typeof(FieldInfoTests), "stringField", typeof(FieldInfoTests), "s_intField", false)]
        public void Equals(Type type1, string name1, Type type2, string name2, bool expected)
        {
            FieldInfo fieldInfo1 = GetField(type1, name1);
            FieldInfo fieldInfo2 = GetField(type2, name2);
            Assert.Equal(expected, fieldInfo1.Equals(fieldInfo2));
        }
        
        [Fact]
        public void GetHashCodeTest()
        {
            FieldInfo fieldInfo = GetField(typeof(FieldInfoTests), "stringField");
            Assert.NotEqual(0, fieldInfo.GetHashCode());
        }

        [Theory]
        [InlineData("ConstIntField", 222)]
        [InlineData("ConstStringField", "new value")]
        [InlineData("ConstCharField", 'A')]
        [InlineData("ConstBoolField", false)]
        [InlineData("ConstFloatField", 4.56f)]
        [InlineData("ConstDoubleField", double.MaxValue)]
        [InlineData("ConstInt64Field", long.MaxValue)]
        [InlineData("ConstByteField", byte.MaxValue)]
        public void SetValue_ConstantField(string field, object value)
        {
            FieldInfo fieldInfo = GetField(typeof(FieldInfoTests), field);
            Assert.Throws<FieldAccessException>(() => fieldInfo.SetValue(new FieldInfoTests(), value));
        }

        [Fact]
        public void SetValue_ReadonlyField()
        {
            FieldInfo fieldInfo = typeof(FieldInfoTests).GetTypeInfo().GetDeclaredField("readonlyIntField");
            FieldInfoTests myInstance = new FieldInfoTests();

            object current = fieldInfo.GetValue(myInstance);
            Assert.Equal(1, current);

            fieldInfo.SetValue(myInstance, int.MinValue);
            Assert.Equal(int.MinValue, fieldInfo.GetValue(myInstance));
        }

        [Fact]
        public void SetValue_MixedArrayTypes_CommonBaseClass()
        {
            A[] ATypeWithMixedAB = new A[] { new A(), new B() };
            A[] ATypeWithAllA = new A[] { new A(), new A() };
            A[] ATypeWithAllB = new A[] { new B(), new B() };
            B[] BTypeWithAllB = new B[] { new B(), new B() };
            A[] BTypeWithAllB_Contra = new B[] { new B(), new B() };

            Type type = typeof(ArrayAsField);
            object obj = Activator.CreateInstance(type);
            FieldInfo fieldInfo = GetField(type, "aArray");

            fieldInfo.SetValue(obj, ATypeWithMixedAB);
            Assert.Equal(ATypeWithMixedAB, fieldInfo.GetValue(obj));

            fieldInfo.SetValue(obj, ATypeWithAllA);
            Assert.Equal(ATypeWithAllA, fieldInfo.GetValue(obj));

            fieldInfo.SetValue(obj, ATypeWithAllB);
            Assert.Equal(ATypeWithAllB, fieldInfo.GetValue(obj));

            fieldInfo.SetValue(obj, BTypeWithAllB);
            Assert.Equal(BTypeWithAllB, fieldInfo.GetValue(obj));

            fieldInfo.SetValue(obj, BTypeWithAllB_Contra);
            Assert.Equal(BTypeWithAllB_Contra, fieldInfo.GetValue(obj));
        }

        [Fact]
        public void SetValue_MixedArrayTypes_SubClass()
        {
            A[] ATypeWithMixedAB = new A[] { new A(), new B() };
            A[] ATypeWithAllA = new A[] { new A(), new A() };
            A[] ATypeWithAllB = new A[] { new B(), new B() };
            B[] BTypeWithAllB = new B[] { new B(), new B() };
            A[] BTypeWithAllB_Contra = new B[] { new B(), new B() };

            Type type = typeof(ArrayAsField);
            object obj = Activator.CreateInstance(type);
            FieldInfo fieldInfo = GetField(type, "bArray");

            Assert.Throws<ArgumentException>(() => fieldInfo.SetValue(obj, ATypeWithMixedAB));
            Assert.Throws<ArgumentException>(() => fieldInfo.SetValue(obj, ATypeWithAllA));
            Assert.Throws<ArgumentException>(() => fieldInfo.SetValue(obj, ATypeWithAllB));

            fieldInfo.SetValue(obj, BTypeWithAllB);
            Assert.Equal(BTypeWithAllB, fieldInfo.GetValue(obj));

            fieldInfo.SetValue(obj, BTypeWithAllB_Contra);
            Assert.Equal(BTypeWithAllB_Contra, fieldInfo.GetValue(obj));
        }

        [Fact]
        public void SetValue_MixedArrayTypes_Interface()
        {
            Type type = typeof(ArrayAsField);
            object obj = Activator.CreateInstance(type);
            FieldInfo fieldInfo = GetField(type, "iArray");

            I[] mixedMN = new I[] { new M(), new N() };
            fieldInfo.SetValue(obj, mixedMN);
            Assert.Equal(mixedMN, fieldInfo.GetValue(obj));
        }

        [Fact]
        public void SetValue_MixedArrayTypes_IntByte()
        {
            Type type = typeof(ArrayAsField);
            object obj = Activator.CreateInstance(type);
            FieldInfo fieldInfo = GetField(type, "intArray");

            int[] intArray = new int[] { 200, -200, 30, 2 };
            fieldInfo.SetValue(obj, intArray);
            Assert.Equal(intArray, fieldInfo.GetValue(obj));

            Assert.Throws<ArgumentException>(() => fieldInfo.SetValue(obj, new byte[] { 2, 3, 4 }));
        }

        [Fact]
        public void SetValue_MixedArrayTypes_ObjectArray()
        {
            Type type = typeof(ArrayAsField);
            object obj = Activator.CreateInstance(type);
            FieldInfo fieldInfo = GetField(type, "objectArray");

            I[] mixedMN = new I[] { new M(), new N() };
            fieldInfo.SetValue(obj, mixedMN);
            Assert.Equal(mixedMN, fieldInfo.GetValue(obj));

            A[] BTypeWithAllB_Contra = new B[] { new B(), new B() };
            fieldInfo.SetValue(obj, BTypeWithAllB_Contra);
            Assert.Equal(BTypeWithAllB_Contra, fieldInfo.GetValue(obj));

            Assert.Throws<ArgumentException>(() => fieldInfo.SetValue(obj, new int[] { 1, -1, 2, -2 }));
            Assert.Throws<ArgumentException>(() => fieldInfo.SetValue(obj, new byte[] { 2, 3, 4 }));
        }

        private static FieldInfo GetField(Type type, string name)
        {
            return type.GetTypeInfo().DeclaredFields.FirstOrDefault(fieldInfo => fieldInfo.Name.Equals(name));
        }

        public readonly int readonlyIntField = 1;
        public const int ConstIntField = 1222;
        public const string ConstStringField = "Hello";
        public const char ConstCharField = 'c';
        public const bool ConstBoolField = true;
        public const float ConstFloatField = (float)22 / 7;
        public const double ConstDoubleField = 22.33;
        public const long ConstInt64Field = 1000;
        public const byte ConstByteField = 0;

        public static int s_intField = 100;
        public static string s_stringField = "static";

        public int intField = 101;
        public string stringField = "non static";

        private int privateIntField = 1;
        private string privateStringField = "privateStringField";

        [Attr(77, name = "AttrSimple"),
        Int32Attr(77, name = "Int32AttrSimple"),
        Int64Attr(77, name = "Int64AttrSimple"),
        StringAttr("hello", name = "StringAttrSimple"),
        EnumAttr(MyEnum.First, name = "EnumAttrSimple"),
        TypeAttr(typeof(object), name = "TypeAttrSimple")]
        public string fieldWithAttributes = "";

        private static object s_assemblyField1 = null; // Without keyword
        private static object s_assemblyField2 = null; // With private keyword
        protected static object s_assemblyField3 = null; // With protected keyword
        public static object s_assemblyField4 = null; // With public keyword
        internal static object s_assemblyField5 = null; // With internal keyword
    }

    public class A { }
    public class B : A { }

    public interface I { }
    public class M : I { }
    public class N : I { }

    public class ArrayAsField
    {
        public A[] aArray;
        public B[] bArray;

        public I[] iArray;

        public int[] intArray;
        public object[] objectArray;
    }
}
