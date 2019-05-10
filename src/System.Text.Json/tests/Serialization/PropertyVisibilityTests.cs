// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Text.Json.Serialization.Tests
{
    public static class PropertyVisibilityTests
    {
        [Fact]
        public static void NoSetter()
        {
            var obj = new ClassWithNoSetter();

            string json = JsonSerializer.ToString(obj);
            Assert.Contains(@"""MyString"":""DefaultValue""", json);
            Assert.Contains(@"""MyInts"":[1,2]", json);
            Assert.Contains(@"""MyProperty"":""MyComplexProperty""", json);
            Assert.Contains(@"""MyInt"":42", json);

            obj = JsonSerializer.Parse<ClassWithNoSetter>(@"{""MyInt"": ""0"",""MyString"":""IgnoreMe"",""MyInts"":[0],""MyComplexProperty"":{""MyProperty"":""IgnoreMe""}}");
            Assert.Equal("DefaultValue", obj.MyString);
            Assert.Equal(2, obj.MyInts.Length);
            Assert.Equal("MyComplexProperty", obj.MyComplexProperty.MyProperty);
            Assert.Equal(42, obj.MyInt);
        }

        [Fact]
        public static void IgnoreReadOnlyProperties()
        {
            var options = new JsonSerializerOptions();
            options.IgnoreReadOnlyProperties = true;

            var obj = new ClassWithNoSetter();

            string json = JsonSerializer.ToString(obj, options);
            Assert.Equal(@"{}", json);
        }

        [Fact]
        public static void NoGetter()
        {
            ClassWithNoGetter objWithNoGetter = JsonSerializer.Parse<ClassWithNoGetter>(
                @"{""MyString"":""Hello"",""MyIntArray"":[0],""MyIntList"":[0]}");

            Assert.Equal("Hello", objWithNoGetter.GetMyString());

            // Currently we don't support setters without getters.
            Assert.Equal(0, objWithNoGetter.GetMyIntArray().Length);
            Assert.Equal(0, objWithNoGetter.GetMyIntList().Count);
        }

        [Fact]
        public static void PrivateGetter()
        {
            var obj = new ClassWithPrivateSetterAndGetter();
            obj.SetMyString("Hello");

            string json = JsonSerializer.ToString(obj);
            Assert.Equal(@"{}", json);
        }

        [Fact]
        public static void PrivateSetter()
        {
            string json = @"{""MyString"":""Hello""}";

            ClassWithPrivateSetterAndGetter objCopy = JsonSerializer.Parse<ClassWithPrivateSetterAndGetter>(json);
            Assert.Null(objCopy.GetMyString());
        }

        [Fact]
        public static void JsonIgnoreAttribute()
        {
            // Verify default state.
            var obj = new ClassWithIgnoreAttributeProperty();
            Assert.Equal(@"MyString", obj.MyString);
            Assert.Equal(@"MyStringWithIgnore", obj.MyStringWithIgnore);
            Assert.Equal(2, obj.MyStringsWithIgnore.Length);
            Assert.Equal(@"MyComplexPropertyWithIgnore", obj.MyComplexPropertyWithIgnore.MyProperty);

            // Verify serialize.
            string json = JsonSerializer.ToString(obj);
            Assert.Contains(@"""MyString""", json);
            Assert.DoesNotContain(@"MyStringWithIgnore", json);
            Assert.DoesNotContain(@"MyStringsWithIgnore", json);
            Assert.DoesNotContain(@"MyComplexPropertyWithIgnore", json);

            // Verify deserialize default.
            obj = JsonSerializer.Parse<ClassWithIgnoreAttributeProperty>(@"{}");
            Assert.Equal(@"MyString", obj.MyString);
            Assert.Equal(@"MyStringWithIgnore", obj.MyStringWithIgnore);
            Assert.Equal(2, obj.MyStringsWithIgnore.Length);
            Assert.Equal(@"MyComplexPropertyWithIgnore", obj.MyComplexPropertyWithIgnore.MyProperty);

            // Verify deserialize ignores the json for MyStringWithIgnore, MyStringsWithIgnore and MyComplexPropertyWithIgnore.
            obj = JsonSerializer.Parse<ClassWithIgnoreAttributeProperty>(
                @"{""MyString"":""Hello"", ""MyStringWithIgnore"":""IgnoreMe"", ""MyStringsWithIgnore"":[""IgnoreMe""], ""MyComplexPropertyWithIgnore"":{""MyProperty"":""IgnoreMe""}}");
            Assert.Contains(@"Hello", obj.MyString);
            Assert.Equal(@"MyStringWithIgnore", obj.MyStringWithIgnore);
            Assert.Equal(2, obj.MyStringsWithIgnore.Length);
            Assert.Equal(@"MyComplexPropertyWithIgnore", obj.MyComplexPropertyWithIgnore.MyProperty);
        }

        [Fact]
        public static void JsonIgnoreAttribute_NestedObject()
        {
            // Verify default state.
            var obj = new NestedObjectWithIgnoreAttributeProperty();
            Assert.Equal(@"MyString", obj.MyString);
            Assert.Equal(@"MyStringNested", obj.NestedObject.MyStringNested);
            Assert.Equal(@"MyStringNested2", obj.NestedObject.NestedObject2.MyStringNested2);

            // Verify serialize.
            string json = JsonSerializer.ToString(obj);
            Assert.Contains(@"""MyString""", json);
            Assert.Contains(@"""MyStringNested""", json);
            Assert.DoesNotContain(@"MyStringNested2", json);

            // Verify deserialize default.
            obj = JsonSerializer.Parse<NestedObjectWithIgnoreAttributeProperty>(@"{}");
            Assert.Equal(@"MyString", obj.MyString);
            Assert.Equal(@"MyStringNested", obj.NestedObject.MyStringNested);
            Assert.Equal(@"MyStringNested2", obj.NestedObject.NestedObject2.MyStringNested2);

            // Verify deserialize ignores the json for NestedObject2.
            obj = JsonSerializer.Parse<NestedObjectWithIgnoreAttributeProperty>(
                @"{""MyString"":""Hello"",""NestedObject"":{""MyStringNested"":""HelloMyStringNested"",""NestedObject2"":{""MyStringNested2"":""IgnoreMe""}}}");
            Assert.Contains(@"Hello", obj.MyString);
            Assert.Contains(@"HelloMyStringNested", obj.NestedObject.MyStringNested);
            Assert.Null(obj.NestedObject.NestedObject2);
        }

        // Todo: add tests with missing object property and missing collection property.

        public class ClassWithPrivateSetterAndGetter
        {
            private string MyString { get; set; }

            public string GetMyString()
            {
                return MyString;
            }

            public void SetMyString(string value)
            {
                MyString = value;
            }
        }

        public class ClassWithNoSetter
        {
            public ClassWithNoSetter()
            {
                MyInt = 42;
                MyString = "DefaultValue";
                MyInts = new int[] { 1, 2 };
                MyComplexProperty = new ComplexType() { MyProperty = "MyComplexProperty" };
            }

            public int MyInt { get; }
            public string MyString { get; }
            public int[] MyInts { get; }
            public ComplexType MyComplexProperty { get; }
        }

        public class ClassWithNoGetter
        {
            string _myString = "";
            int[] _myIntArray = new int[] { };
            List<int> _myIntList = new List<int> { };

            public string MyString
            {
                set
                {
                    _myString = value;
                }
            }

            public int[] MyIntArray
            {
                set
                {
                    _myIntArray = value;
                }
            }

            public List<int> MyList
            {
                set
                {
                    _myIntList = value;
                }
            }

            public string GetMyString()
            {
                return _myString;
            }

            public int[] GetMyIntArray()
            {
                return _myIntArray;
            }

            public List<int> GetMyIntList()
            {
                return _myIntList;
            }
        }

        public class ClassWithIgnoreAttributeProperty
        {
            public ClassWithIgnoreAttributeProperty()
            {
                MyString = "MyString";
                MyStringWithIgnore = "MyStringWithIgnore";
                MyStringsWithIgnore = new string[] { "1", "2" };
                MyComplexPropertyWithIgnore = new ComplexType() { MyProperty = "MyComplexPropertyWithIgnore" };
            }

            [JsonIgnore]
            public string MyStringWithIgnore { get; set; }

            public string MyString { get; set; }

            [JsonIgnore]
            public string[] MyStringsWithIgnore { get; set; }

            [JsonIgnore]
            public ComplexType MyComplexPropertyWithIgnore { get; set; }
        }

        public class ComplexType
        {
            public string MyProperty { get; set; }
        }

        public class NestedObjectWithIgnoreAttributeProperty
        {
            public NestedObjectWithIgnoreAttributeProperty()
            {
                MyString = "MyString";
                NestedObject = new NestedObject()
                {
                    MyStringNested = "MyStringNested",
                    NestedObject2 = new NestedObject2()
                    {
                        MyStringNested2 = "MyStringNested2"
                    }
                };
            }

            public string MyString { get; set; }

            public NestedObject NestedObject { get; set; }
        }

        public class NestedObject
        {
            public string MyStringNested { get; set; }

            [JsonIgnore]
            public NestedObject2 NestedObject2 { get; set; }
        }

        public class NestedObject2
        {
            public string MyStringNested2 { get; set; }
        }
    }
}
