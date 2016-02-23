// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using Xunit;

#pragma warning disable 0414

namespace System.Reflection.Tests
{
    public class PropertyInfoTests
    {
        [Theory]
        [InlineData(typeof(PropertyInfoSubClass), "Description", PropertyAttributes.None)]
        [InlineData(typeof(PropertyInfoSubClass), "NewProperty", PropertyAttributes.None)]
        public void Attributes(Type type, string name, PropertyAttributes expected)
        {
            PropertyInfo propertyInfo = GetProperty(type, name);
            Assert.Equal(expected, propertyInfo.Attributes);
        }

        [Theory]
        [InlineData(typeof(PropertyInfoBaseClass), "ReadWriteProperty1", true)]
        [InlineData(typeof(PropertyInfoBaseClass), "ReadOnlyProperty", true)]
        [InlineData(typeof(PropertyInfoBaseClass), "WriteOnlyProperty", false)]
        public void CanRead(Type type, string name, bool expected)
        {
            PropertyInfo propertyInfo = GetProperty(type, name);
            Assert.Equal(expected, propertyInfo.CanRead);
        }

        [Theory]
        [InlineData(typeof(PropertyInfoBaseClass), "ReadWriteProperty1", true)]
        [InlineData(typeof(PropertyInfoBaseClass), "ReadOnlyProperty", false)]
        [InlineData(typeof(PropertyInfoBaseClass), "WriteOnlyProperty", true)]
        public void CanWrite(Type type, string name, bool expected)
        {
            PropertyInfo propertyInfo = GetProperty(type, name);
            Assert.Equal(expected, propertyInfo.CanWrite);
        }

        [Theory]
        [InlineData(typeof(PropertyInfoBaseClass), "ReadWriteProperty1")]
        [InlineData(typeof(PropertyInfoSubClass), "NewProperty")]
        public void DeclaringType(Type type, string name)
        {
            PropertyInfo propertyInfo = GetProperty(type, name);
            Assert.Equal(type, propertyInfo.DeclaringType);
        }

        [Theory]
        [InlineData(typeof(PropertyInfoBaseClass), "ReadWriteProperty1", typeof(PropertyInfoBaseClass), "ReadWriteProperty1", true)]
        [InlineData(typeof(PropertyInfoBaseClass), "ReadWriteProperty1", typeof(PropertyInfoBaseClass), "ReadWriteProperty2", false)]
        public void Equals(Type type1, string name1, Type type2, string name2, bool expected)
        {
            PropertyInfo propertyInfo1 = GetProperty(type1, name1);
            PropertyInfo propertyInfo2 = GetProperty(type2, name2);
            Assert.Equal(expected, propertyInfo1.Equals(propertyInfo2));
        }

        [Theory]
        [InlineData(typeof(PropertyInfoBaseClass), "IntProperty")]
        [InlineData(typeof(PropertyInfoBaseClass), "StringProperty")]
        [InlineData(typeof(PropertyInfoBaseClass), "DoubleProperty")]
        [InlineData(typeof(PropertyInfoBaseClass), "FloatProperty")]
        [InlineData(typeof(PropertyInfoBaseClass), "EnumProperty")]
        public void GetConstantValue_Invalid(Type type, string name)
        {
            PropertyInfo propertyInfo = GetProperty(type, name);
            Assert.Throws<InvalidOperationException>(() => propertyInfo.GetConstantValue());
        }

        [Fact]
        public void GetHashCodeTest()
        {
            PropertyInfo propertyInfo = GetProperty(typeof(PropertyInfoBaseClass), "ReadWriteProperty1");
            Assert.NotEqual(0, propertyInfo.GetHashCode());
        }

        [Theory]
        [InlineData(typeof(PropertyInfoBaseClass), "Item", new string[] { "Index" })]
        [InlineData(typeof(PropertyInfoBaseClass), "ReadWriteProperty1", new string[0])]
        public void GetIndexParameters(Type type, string name, string[] expectedNames)
        {
            PropertyInfo propertyInfo = GetProperty(type, name);
            ParameterInfo[] indexParameters = propertyInfo.GetIndexParameters();

            Assert.Equal(expectedNames.Length, indexParameters.Length);
            for (int i = 0; i < indexParameters.Length; i++)
            {
                Assert.Equal(expectedNames[i], indexParameters[i].Name);
            }
        }

        [Theory]
        [InlineData(typeof(GetMethodSetMethodClass), "ReadWriteProperty", true, true)]
        [InlineData(typeof(GetMethodSetMethodClass), "ReadOnlyProperty", true, false)]
        [InlineData(typeof(GetMethodSetMethodClass), "WriteOnlyProperty", false, true)]
        [InlineData(typeof(GetMethodSetMethodClass), "Item", true, true)]
        [InlineData(typeof(GetMethodSetMethodStruct), "ReadWriteProperty", true, true)]
        [InlineData(typeof(GetMethodSetMethodStruct), "ReadOnlyProperty", true, false)]
        [InlineData(typeof(GetMethodSetMethodStruct), "WriteOnlyProperty", false, true)]
        [InlineData(typeof(GetMethodSetMethodStruct), "Item", true, false)]
        [InlineData(typeof(GetMethodSetMethodInterface), "ReadWriteProperty", true, true)]
        [InlineData(typeof(GetMethodSetMethodInterface), "ReadOnlyProperty", true, false)]
        [InlineData(typeof(GetMethodSetMethodInterface), "WriteOnlyProperty", false, true)]
        [InlineData(typeof(GetMethodSetMethodInterface), "Item", false, true)]
        public void GetMethod_SetMethod(Type type, string name, bool hasGetter, bool hasSetter)
        {
            PropertyInfo propertyInfo = GetProperty(type, name);
            Assert.True(hasGetter == (propertyInfo.GetMethod != null), string.Format("{0}.GetMethod expected existence: '{1}', but got '{2}'", propertyInfo, hasGetter, !hasGetter));
            Assert.True(hasSetter == (propertyInfo.SetMethod != null), string.Format("{0}.SetMethod expected existence: '{1}', but got '{2}'", propertyInfo, hasSetter, !hasSetter));
        }

        public static IEnumerable<object[]> GetValue_TestData()
        {
            yield return new object[] { typeof(PropertyInfoBaseClass), "ReadWriteProperty2", new PropertyInfoBaseClass(), null, -1.0 };
            yield return new object[] { typeof(PropertyInfoBaseClass), "ReadWriteProperty3", typeof(PropertyInfoBaseClass), null, -2 };
            yield return new object[] { typeof(PropertyInfoBaseClass), "Name", new PropertyInfoBaseClass(), null, "hello" };
            yield return new object[] { typeof(CustomIndexerNameClass), "BasicIndexer", new CustomIndexerNameClass(), new object[] { 1, "2" }, null };
            yield return new object[] { typeof(PropertyInfoBaseClass), "ReadOnlyProperty", new PropertyInfoBaseClass(), null, 100 };
        }

        [Theory]
        [MemberData("GetValue_TestData")]
        public void GetValue(Type type, string name, object obj, object[] index, object expected)
        {
            PropertyInfo propertyInfo = GetProperty(type, name);
            if (index == null)
            {
                Assert.Equal(expected, propertyInfo.GetValue(obj));
            }
            Assert.Equal(expected, propertyInfo.GetValue(obj, index));
        }

        public static IEnumerable<object[]> GetValue_Invalid_TestData()
        {
            yield return new object[] { typeof(CustomIndexerNameClass), "BasicIndexer", new CustomIndexerNameClass(), new object[] { 1, "2", 3 }, typeof(TargetParameterCountException) };
            yield return new object[] { typeof(CustomIndexerNameClass), "BasicIndexer", new CustomIndexerNameClass(), null, typeof(TargetParameterCountException) };
            yield return new object[] { typeof(CustomIndexerNameClass), "BasicIndexer", new CustomIndexerNameClass(), new object[] { "1", "2" }, typeof(ArgumentException) };
            yield return new object[] { typeof(PropertyInfoBaseClass), "WriteOnlyProperty", new PropertyInfoBaseClass(), null, typeof(ArgumentException) };
            yield return new object[] { typeof(CustomIndexerNameClass), "BasicIndexer", null, new object[] { "1", "2" }, typeof(Exception) }; // Wrong types, Win8p throws Exception, not TargetException
        }

        [Theory]
        [MemberData("GetValue_Invalid_TestData")]
        public void GetValue_Invalid(Type type, string name, object obj, object[] index, Type exceptionType)
        {
            PropertyInfo propertyInfo = GetProperty(type, name);
            if (exceptionType.Equals(typeof(Exception)))
            {
                Assert.ThrowsAny<Exception>(() => propertyInfo.GetValue(obj, index));
            }
            else
            {
                Assert.Throws(exceptionType, () => propertyInfo.GetValue(obj, index));
            }
        }

        [Theory]
        [InlineData(typeof(PropertyInfoBaseClass), "ReadWriteProperty1", false)]
        [InlineData(typeof(PropertyInfoBaseClass), "ReadOnlyProperty", false)]
        [InlineData(typeof(PropertyInfoBaseClass), "WriteOnlyProperty", false)]
        public void IsSpecialName(Type type, string name, bool expected)
        {
            PropertyInfo propertyInfo = GetProperty(type, name);
            Assert.Equal(expected, propertyInfo.IsSpecialName);
        }
        
        [Theory]
        [InlineData(typeof(PropertyInfoBaseClass), "ReadWriteProperty1")]
        [InlineData(typeof(PropertyInfoBaseClass), "ReadOnlyProperty")]
        [InlineData(typeof(PropertyInfoBaseClass), "WriteOnlyProperty")]
        public void Name(Type type, string name)
        {
            PropertyInfo propertyInfo = GetProperty(type, name);
            Assert.Equal(name, propertyInfo.Name);
        }

        [Theory]
        [InlineData(typeof(PropertyInfoBaseClass), "ReadWriteProperty1", typeof(short))]
        [InlineData(typeof(PropertyInfoBaseClass), "ReadWriteProperty2", typeof(double))]
        [InlineData(typeof(PropertyInfoSubClass), "NewProperty", typeof(int))]
        public void PropertyType(Type type, string name, Type expected)
        {
            PropertyInfo propertyInfo = GetProperty(type, name);
            Assert.Equal(expected, propertyInfo.PropertyType);
        }
        
        public static IEnumerable<object[]> SetValue_TestData()
        {
            yield return new object[] { typeof(PropertyInfoBaseClass), "StaticObjectArrayProperty", typeof(PropertyInfoBaseClass), new string[] { "hello" }, null, new string[] { "hello" } };
            yield return new object[] { typeof(PropertyInfoBaseClass), "ObjectArrayProperty", new PropertyInfoBaseClass(), new string[] { "hello" }, null, new string[] { "hello" } };
            yield return new object[] { typeof(PropertyInfoBaseClass), "Name", new PropertyInfoBaseClass(), "hello", null, "hello" };
            yield return new object[] { typeof(AdvancedIndexerClass), "Item", new AdvancedIndexerClass(), "hello", new object[] { 99, 2, new string[] { "hello" }, "f" }, "992f1" };
            yield return new object[] { typeof(AdvancedIndexerClass), "Item", new AdvancedIndexerClass(), "pw", new object[] { 99, 2, new string[] { "hello" }, "SOME string" }, "992SOME string1" };
        }

        [Theory]
        [MemberData("SetValue_TestData")]
        public void SetValue(Type type, string name, object obj, object value, object[] index, object expected)
        {
            PropertyInfo PropertyInfo = GetProperty(type, name);
            object originalValue;
            if (index == null)
            {
                // Use SetValue(object, object)
                originalValue = PropertyInfo.GetValue(obj);
                try
                {
                    PropertyInfo.SetValue(obj, value);
                    Assert.Equal(expected, PropertyInfo.GetValue(obj));
                }
                finally
                {
                    PropertyInfo.SetValue(obj, originalValue);
                }
            }
            // Use SetValue(object, object, object[])
            originalValue = PropertyInfo.GetValue(obj, index);
            try
            {
                PropertyInfo.SetValue(obj, value, index);
                Assert.Equal(expected, PropertyInfo.GetValue(obj, index));
            }
            finally
            {
                PropertyInfo.SetValue(obj, originalValue, index);
            }
        }

        public static IEnumerable<object[]> SetValue_Invalid_TestData()
        {
            // Incorrect type for indexer ObjectArrayProperty
            yield return new object[] { typeof(AdvancedIndexerClass), "Item", new AdvancedIndexerClass(), "value", new object[] { 99, 2, "invalid", "string" }, typeof(ArgumentException) };

            // Incorrect type for value
            yield return new object[] { typeof(AdvancedIndexerClass), "Item", new AdvancedIndexerClass(), 100, new object[] { 99, 2, new string[] { "a" }, "b" }, typeof(ArgumentException) };

            // Incorrect number of parameters
            yield return new object[] { typeof(AdvancedIndexerClass), "Item", new AdvancedIndexerClass(), "value", new object[] { 99, 2, new string[] { "a" } }, typeof(TargetParameterCountException) };

            // Obj is null (Win8p throws Exception, not TargetException)
            yield return new object[] { typeof(AdvancedIndexerClass), "Item", null, "value", new object[] { 99, 2, new string[] { "a" }, "b" }, typeof(Exception) };

            // Set instance ObjectArrayProperty with wrong type (Win8p throws Exception, not TargetException)
            yield return new object[] { typeof(PropertyInfoBaseClass), "WriteOnlyObjectArrayProperty", new PropertyInfoBaseClass(), "value", null, typeof(Exception) };

            // Obj is null (Win8p throws Exception, not TargetException)
            yield return new object[] { typeof(PropertyInfoBaseClass), "WriteOnlyObjectArrayProperty", null, null, null, typeof(Exception) };

            // ObjectArrayProperty has no setter
            yield return new object[] { typeof(PropertyInfoBaseClass), "ReadOnlyObjectArrayProperty", new PropertyInfoBaseClass(), 100, null, typeof(Exception) };
        }

        [Theory]
        [MemberData("SetValue_Invalid_TestData")]
        public void SetValue_Invalid(Type type, string name, object obj, object value, object[] index, Type exceptionType)
        {
            PropertyInfo PropertyInfo = GetProperty(type, name);
            if (exceptionType.Equals(typeof(Exception)))
            {
                Assert.ThrowsAny<Exception>(() => PropertyInfo.SetValue(obj, value, index));
            }
            else
            {
                Assert.Throws(exceptionType, () => PropertyInfo.SetValue(obj, value, index));
            }
        }

        public static PropertyInfo GetProperty(Type type, string name)
        {
            return type.GetTypeInfo().DeclaredProperties.FirstOrDefault(propertyInfo => propertyInfo.Name.Equals(name));
        }
    }

    // Metadata for reflection
    public enum MyEnum { First = 1, Second = 2, Third = 3, Fourth = 4 };

    public interface InterfaceWithPropertyDeclaration
    {
        string Name { get; set; }
    }
    
    public class PropertyInfoBaseClass : InterfaceWithPropertyDeclaration
    {
        public static object[] _staticObjectArrayProperty = new object[1];
        public static object[] StaticObjectArrayProperty
        {
            get { return _staticObjectArrayProperty; }
            set { _staticObjectArrayProperty = value; }
        }

        public object[] _objectArray;
        public object[] ObjectArrayProperty
        {
            get { return _objectArray; }
            set { _objectArray = value; }
        }

        public short _readWriteProperty1 = -2;
        public short ReadWriteProperty1
        {
            get { return _readWriteProperty1; }
            set { _readWriteProperty1 = value; }
        }

        public double _readWriteProperty2 = -1;
        public double ReadWriteProperty2
        {
            get { return _readWriteProperty2; }
            set { _readWriteProperty2 = value; }
        }

        public static int _readWriteProperty3 = -2;
        public static int ReadWriteProperty3
        {
            get { return _readWriteProperty3; }
            set { _readWriteProperty3 = value; }
        }

        public int _readOnlyProperty = 100;
        public int ReadOnlyProperty
        {
            get { return _readOnlyProperty; }
        }

        public long _writeOnlyProperty = 1;
        public int WriteOnlyProperty
        {
            set { _writeOnlyProperty = value; }
        }

        // Indexer properties
        public string[] _stringArray = { "abc", "def", "ghi", "jkl" };
        public string this[int Index]
        {
            get { return _stringArray[Index]; }
            set { _stringArray[Index] = value; }
        }

        // Interface property
        private string _name = "hello";
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        // Const fields
        private const int IntField = 100;
        private const string StringField = "hello";
        private const double DoubleField = 22.314;
        private const float FloatField = 99.99F;
        private const MyEnum EnumField = MyEnum.First;

        public int IntProperty { get { return IntField; } }
        public string StringProperty { get { return StringField; } }
        public double DoubleProperty { get { return DoubleField; } }
        public float FloatProperty { get { return FloatField; } }
        public MyEnum EnumProperty { get { return EnumField; } }
    }

    public class PropertyInfoSubClass : PropertyInfoBaseClass
    {
        private int _newProperty = 100;
        private string _description;

        public int NewProperty
        {
            get { return _newProperty; }
            set { _newProperty = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
    }

    public class CustomIndexerNameClass
    {
        private object[] _objectArray;
        [IndexerName("BasicIndexer")] // Property name will be BasicIndexer instead of Item
        public object[] this[int index, string s]
        {
            get { return _objectArray; }
            set { _objectArray = value; }
        }
    }

    public class AdvancedIndexerClass
    {
        public string _setValue = null;
        public string this[int index, int index2, string[] h, string myStr]
        {
            get
            {
                string strHashLength = "null";
                if (h != null)
                {
                    strHashLength = h.Length.ToString();
                }
                return index.ToString() + index2.ToString() + myStr + strHashLength;
            }
            set
            {
                string strHashLength = "null";
                if (h != null)
                {
                    strHashLength = h.Length.ToString();
                }
                _setValue = _setValue = index.ToString() + index2.ToString() + myStr + strHashLength + value;
            }
        }
    }

    public class GetMethodSetMethodClass
    {
        public int ReadWriteProperty { get { return 1; } set { } }
        public string ReadOnlyProperty { get { return "Test"; } }
        public char WriteOnlyProperty { set { } }
        public int this[int index] { get { return 2; } set { } }
    }

    public struct GetMethodSetMethodStruct
    {
        public int ReadWriteProperty { get { return 1; } set { } }
        public string ReadOnlyProperty { get { return "Test"; } }
        public char WriteOnlyProperty { set { } }
        public string this[int index] { get { return "name"; } }
    }

    public interface GetMethodSetMethodInterface
    {
        int ReadWriteProperty { get; set; }
        string ReadOnlyProperty { get; }
        char WriteOnlyProperty { set; }
        char this[int index] { set; }
    }
}
