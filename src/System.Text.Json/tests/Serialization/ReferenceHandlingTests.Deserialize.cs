// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Dynamic;
using Newtonsoft.Json;
using Xunit;

namespace System.Text.Json.Tests
{
    public static partial class ReferenceHandlingTests
    {
        private static JsonSerializerOptions _deserializeOptions = new JsonSerializerOptions { ReferenceHandlingOnDeserialize = ReferenceHandlingOnDeserialize.PreserveDuplicates };

        private class EmployeeWithContacts
        {
            public string Name { get; set; }
            public EmployeeWithContacts Manager { get; set; }
            public List<EmployeeWithContacts> Subordinates { get; set; }
            public Dictionary<string, EmployeeWithContacts> Contacts { get; set; }
        }

        private class Employee
        {
            public static readonly List<string> SubordinatesDefault = new List<string> { "Bob" }; //how can I make these immutables?
            public static readonly Dictionary<string, string> ContactsDefault = new Dictionary<string, string>() { { "Bob", "555-5555" } };

            public string Name { get; set; }
            public Employee Manager { get; set; }
            public List<Employee> Subordinates { get; set; }
            public Dictionary<string, Employee> Contacts { get; set; }

            public List<string> SubordinatesString { get; set; } = SubordinatesDefault;

            public Dictionary<string, string> ContactsString { get; set; } = ContactsDefault;
        }

        #region Root Object
        [Fact] //Employee list as a property and then use reference to itself on nested Employee.
        public static void ObjectReferenceLoop()
        {
            string json =
            @"{
                ""$id"": ""1"",
                ""Name"": ""Angela"",
                ""Manager"": {
                    ""$ref"": ""1""
                }
            }";

            Employee angela = JsonSerializer.Deserialize<Employee>(json, _deserializeOptions);
            Assert.Same(angela, angela.Manager);
        }

        [Fact] // Employee whose subordinates is a preserved list. EmployeeListEmployee
        public static void ObjectReferenceLoopInList()
        {
            string json =
            @"{
                ""$id"": ""1"",
                ""Subordinates"": {
                    ""$id"": ""2"",
                    ""$values"": [
                        {
                            ""$ref"": ""1""
                        }
                    ]
                }
            }";

            Employee employee = JsonSerializer.Deserialize<Employee>(json, _deserializeOptions);
            Assert.Equal(1, employee.Subordinates.Count);
            Assert.Same(employee, employee.Subordinates[0]);
        }

        [Fact] // Employee whose subordinates is a preserved list. EmployeeListEmployee
        public static void ObjectReferenceLoopInDictionary()
        {
            string json =
            @"{
                ""$id"": ""1"",
                ""Contacts"":{
                    ""$id"": ""2"",
                    ""Angela"":{
                        ""$ref"": ""1""
                    }
                }
            }";

            EmployeeWithContacts employee = JsonSerializer.Deserialize<EmployeeWithContacts>(json, _deserializeOptions);
            Assert.Same(employee, employee.Contacts["Angela"]);
        }

        [Fact] //Employee list as a property and then use reference to itself on nested Employee.
        public static void ObjectWithArrayReferenceDeeper()
        {
            string json =
            @"{
                ""$id"": ""1"",
                ""Subordinates"": {
                    ""$id"": ""2"",
                    ""$values"": [
                        {
                            ""$id"": ""3"",
                            ""Name"": ""Angela"",
                            ""Subordinates"":{
                                ""$ref"": ""2""
                            }
                        }
                    ]
                }
            }";

            Employee employee = JsonSerializer.Deserialize<Employee>(json, _deserializeOptions);
            Assert.Same(employee.Subordinates, employee.Subordinates[0].Subordinates);
        }

        [Fact] //Employee Dictionary as a property and then use reference to itself on nested Employee.MissingMethodException: Method not found: 'Void System.Text.Json.JsonSerializerOptions.set_ReferenceHandlingOnDeserialize(System.Text.Json.ReferenceHandlingOnDeserialize)'.

        public static void ObjectWithDictionaryReferenceDeeper()
        {
            string json =
            @"{
                ""$id"": ""1"",
                ""Contacts"": {
                    ""$id"": ""2"",
                    ""Angela"": {
                        ""$id"": ""3"",
                        ""Name"": ""Angela"",
                        ""Contacts"": {
                            ""$ref"": ""2""
                        }
                    }
                }
            }";

            EmployeeWithContacts employee = JsonSerializer.Deserialize<EmployeeWithContacts>(json, _deserializeOptions);
            Assert.Same(employee.Contacts, employee.Contacts["Angela"].Contacts);
        }

        private class MyClass
        {
            public List<int> MyList { get; set; }
            public List<int> MyListCopy { get; set; }
        }

        [Fact]
        public static void PreservedArrayIntoArrayProperty()
        {
            string json = @"
            {
                ""MyList"": {
                    ""$id"": ""1"",
                    ""$values"": [
                        10,
                        20,
                        30,
                        40
                    ]
                },
                ""MyListCopy"": { ""$ref"": ""1"" }
            }";

            MyClass instance = JsonSerializer.Deserialize<MyClass>(json, _deserializeOptions);
            Assert.Equal(4, instance.MyList.Count);
            Assert.Same(instance.MyList, instance.MyListCopy);
        }
        #endregion

        #region Root Dictionary
        [Fact] //Employee list as a property and then use reference to itself on nested Employee.
        public static void DictionaryReferenceLoop()
        {
            string json =
            @"{
                ""$id"": ""1"",
                ""Angela"": {
                    ""$id"": ""2"",
                    ""Name"": ""Angela"",
                    ""Contacts"": {
                        ""$ref"": ""1""
                    }
                }
            }";

            Dictionary<string, EmployeeWithContacts> dictionary = JsonSerializer.Deserialize<Dictionary<string, EmployeeWithContacts>>(json, _deserializeOptions);

            Assert.Same(dictionary, dictionary["Angela"].Contacts);
        }

        [Fact]
        public static void DictionaryReferenceLoopInList()
        {
            string json =
            @"{
                ""$id"": ""1"",
                ""Angela"": {
                    ""$id"": ""2"",
                    ""Name"": ""Angela"",
                    ""Subordinates"": {
                        ""$id"": ""3"",
                        ""$values"": [
                            {
                                ""$id"": ""4"",
                                ""Name"": ""Bob"",
                                ""Contacts"": {
                                    ""$ref"": ""1""
                                }
                            }
                        ]
                    }
                }
            }";

            Dictionary<string, EmployeeWithContacts> dictionary = JsonSerializer.Deserialize<Dictionary<string, EmployeeWithContacts>>(json, _deserializeOptions);
            Assert.Same(dictionary, dictionary["Angela"].Subordinates[0].Contacts);
        }

        [Fact]
        public static void DicitionaryDuplicatedObject()
        {
            string json =
            @"{
              ""555"": { ""$id"": ""1"", ""Name"": ""Angela"" },
              ""556"": { ""Name"": ""Bob"" },
              ""557"": { ""$ref"": ""1"" }
            }";

            Dictionary<string, Employee> directory = JsonSerializer.Deserialize<Dictionary<string, Employee>>(json, _deserializeOptions);
            Assert.Same(directory["555"], directory["557"]);
        }

        [Fact] //This should not throw, since the references are in nested objects, not in the immutable dictionary itself.
        public static void ImmutableDictionaryPreserveNestedObjects()
        {
            string json =
            @"{
                ""Angela"": {
                    ""$id"": ""1"",
                    ""Name"": ""Angela"",
                    ""Subordinates"": {
                        ""$id"": ""2"",
                        ""$values"": [
                            {
                                ""$id"": ""3"",
                                ""Name"": ""Carlos"",
                                ""Manager"": {
                                    ""$ref"": ""1""
                                }
                            }
                        ]
                    }
                },
                ""Bob"": {
                    ""$id"": ""4"",
                    ""Name"": ""Bob""
                },
                ""Carlos"": {
                    ""$ref"": ""3""
                }
            }";

            ImmutableDictionary<string, Employee> dictionary = JsonSerializer.Deserialize<ImmutableDictionary<string, Employee>>(json, _deserializeOptions);
            Assert.Same(dictionary["Angela"], dictionary["Angela"].Subordinates[0].Manager);
            Assert.Same(dictionary["Carlos"], dictionary["Angela"].Subordinates[0]);
        }
        #endregion

        #region Root Array
        [Fact]
        public static void PreservedArrayIntoRootArray()
        {
            string json = @"
            {
                ""$id"": ""1"",
                ""$values"": [
                    10,
                    20,
                    30,
                    40
                ]
            }";

            List<int> myList = JsonSerializer.Deserialize<List<int>>(json, _deserializeOptions);
            Assert.Equal(4, myList.Count);
        }

        [Fact] // Preserved list that contains an employee whose subordinates is a reference to the root list.
        public static void ArrayNestedArray()
        {
            string json =
            @"{
                ""$id"": ""1"",
                ""$values"":[
                    {
                        ""$id"":""2"",
                        ""Name"": ""Angela"",
                        ""Subordinates"": {
                            ""$ref"": ""1""
                        }
                    }
                ]
            }";

            List<Employee> employees = JsonSerializer.Deserialize<List<Employee>>(json, _deserializeOptions);

            Assert.Same(employees, employees[0].Subordinates);
        }

        [Fact]
        public static void EmptyArray()
        {
            string json =
            @"{
              ""$id"": ""1"",
              ""Subordinates"": {
                ""$id"": ""2"",
                ""$values"": []
              },
              ""Name"": ""Angela""
            }";

            Employee angela = JsonSerializer.Deserialize<Employee>(json, _deserializeOptions);

            Assert.NotNull(angela);
            Assert.NotNull(angela.Subordinates);
            Assert.Equal(0, angela.Subordinates.Count);

        }

        [Fact]
        public static void ArrayWithDuplicates() //Make sure the serializer can understand lists that were wrapped in braces.
        {
            string json =
            @"{
                ""$id"": ""1"",
                ""$values"":[
                    {
                        ""$id"": ""2"",
                        ""Name"": ""Angela""
                    },
                    {
                        ""$id"": ""3"",
                        ""Name"": ""Bob""
                    },
                    {
                        ""$ref"": ""2""
                    },
                    {
                        ""$ref"": ""3""
                    },
                    {
                        ""$id"": ""4""
                    },
                    {
                        ""$ref"": ""4""
                    }
                ]
            }";

            List<Employee> employees = JsonSerializer.Deserialize<List<Employee>>(json, _deserializeOptions);
            Assert.Equal(6, employees.Count);
            Assert.Same(employees[0], employees[2]);
            Assert.Same(employees[1], employees[3]);
            Assert.Same(employees[4], employees[5]);

        }

        [Fact]
        public static void ArrayNotPreservedWithDuplicates() //Make sure the serializer can understand lists that were wrapped in braces.
        {
            string json =
            @"[
                {
                    ""$id"": ""2"",
                    ""Name"": ""Angela""
                },
                {
                    ""$id"": ""3"",
                    ""Name"": ""Bob""
                },
                {
                    ""$ref"": ""2""
                },
                {
                    ""$ref"": ""3""
                },
                {
                    ""$id"": ""4""
                },
                {
                    ""$ref"": ""4""
                }
            ]";

            Employee[] employees = JsonSerializer.Deserialize<Employee[]>(json, _deserializeOptions);
            Assert.Equal(6, employees.Length);
            Assert.Same(employees[0], employees[2]);
            Assert.Same(employees[1], employees[3]);
            Assert.Same(employees[4], employees[5]);
        }
        #endregion

        #region Converter
        [Fact] //This only demonstrates that behavior with converters remain the same.
        public static void DeserializeWithListConverter()
        {
            string json =
            @"{
                ""$id"": ""1"",
                ""Subordinates"": {
                    ""$id"": ""2"",
                    ""$values"": [
                        {
                            ""$ref"": ""1""
                        }
                    ]
                },
                ""Name"": ""Angela"",
                ""Manager"": {
                    ""Subordinates"": {
                        ""$ref"": ""2""
                    }
                }
            }";

            var options = new JsonSerializerOptions();
            options.ReferenceHandlingOnDeserialize = ReferenceHandlingOnDeserialize.PreserveDuplicates;
            options.Converters.Add(new MyConverter());

            Employee angela = JsonSerializer.Deserialize<Employee>(json, options);
        }

        //NOTE: If you implement a converter, you are on your own when handling metadata properties and therefore references.Newtonsoft does the same.
        //However; is there a way to recall preserved references previously found in the payload and to store new ones found in the converter's payload? that would be a cool enhancement.
        private class MyConverter : Serialization.JsonConverter<List<Employee>>
        {
            public override List<Employee> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                int startObjectCount = 0;
                int endObjectCount = 0;

                while (true)
                {
                    switch (reader.TokenType)
                    {
                        case JsonTokenType.StartObject:
                            startObjectCount++; break;
                        case JsonTokenType.EndObject:
                            endObjectCount++; break;
                    }

                    if (startObjectCount == endObjectCount)
                    {
                        break;
                    }

                    reader.Read();
                }

                return new List<Employee>();
            }

            public override void Write(Utf8JsonWriter writer, List<Employee> value, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        #region Null/non-existent reference
        [Fact]
        public static void NormalDictionaryPropertyNull() //Make sure the serializer can understand lists that were wrapped in braces.
        {
            string json =
            @"{
                ""ContactsString"": null
            }";

            var opts = new JsonSerializerOptions { IgnoreNullValues = true };
            var bob = new Employee();
            Employee angela = JsonSerializer.Deserialize<Employee>(json, opts);
            Assert.NotNull(angela.ContactsString);
        }

        [Fact]
        public static void ObjectNull()
        {
            string json =
            @"{
                ""$ref"": ""1""
            }";

            Employee employee = JsonSerializer.Deserialize<Employee>(json, _deserializeOptions);
            Assert.Null(employee);
        }

        [Fact()]
        public static void ArrayNull()
        {
            string json =
            @"{
                ""$ref"": ""1""
            }";

            List<Employee> array = JsonSerializer.Deserialize<List<Employee>>(json, _deserializeOptions);
            Assert.Null(array);
        }

        [Fact]
        public static void DictionaryNull()
        {
            string json =
            @"{
                ""$ref"": ""1""
            }";

            Dictionary<string, Employee> dictionary = JsonSerializer.Deserialize<Dictionary<string, Employee>>(json, _deserializeOptions);
            Assert.Null(dictionary);
        }

        [Fact]
        public static void ObjectPropertyNull()
        {
            string json =
            @"{
                ""Manager"": {
                    ""$ref"": ""1""
                }
            }";

            Employee angela = JsonSerializer.Deserialize<Employee>(json, _deserializeOptions);
            Assert.Null(angela.Manager);
        }

        [Fact]
        public static void ArrayPropertyNull()
        {
            string json =
            @"{
                ""Subordinates"": {
                    ""$ref"": ""1""
                }
            }";

            Employee angela = JsonSerializer.Deserialize<Employee>(json, _deserializeOptions);
            Assert.Null(angela.Subordinates);
        }

        [Fact]
        public static void DictionaryPropertyNull()
        {
            string json =
            @"{
                ""Contacts"": {
                    ""$ref"": ""1""
                }
            }";

            Employee angela = JsonSerializer.Deserialize<Employee>(json, _deserializeOptions);
            Assert.Null(angela.Contacts);
        }

        #region IgnoreNullValues
        private static JsonSerializerOptions _deserializeMetadataIgnoreNull = new JsonSerializerOptions { ReferenceHandlingOnDeserialize = ReferenceHandlingOnDeserialize.PreserveDuplicates, IgnoreNullValues = true };

        [Fact] //TODO
        public static void ObjectPropertyIgnoreNull()
        {
            string json =
            @"{
                ""Manager"": {
                    ""$ref"": ""1""
                }
            }";

            Employee angela = JsonSerializer.Deserialize<Employee>(json, _deserializeMetadataIgnoreNull);
            Assert.Null(angela.Manager);
        }

        [Fact]
        public static void ArrayPropertyIgnoreNull()
        {
            string json =
            @"{
                ""SubordinatesString"": {
                    ""$ref"": ""1""
                }
            }";

            Employee angela = JsonSerializer.Deserialize<Employee>(json, _deserializeMetadataIgnoreNull);
            Assert.Equal(1, angela.SubordinatesString.Count);
            Assert.NotNull(angela.SubordinatesString);
            Assert.Same(Employee.SubordinatesDefault, angela.SubordinatesString);

        }

        [Fact]
        public static void DictionaryPropertyIgnoreNull()
        {
            string json =
            @"{
                ""ContactsString"": {
                    ""$ref"": ""1""
                }
            }";

            Employee angela = JsonSerializer.Deserialize<Employee>(json, _deserializeMetadataIgnoreNull);
            Assert.NotNull(angela.ContactsString); // ContactsString has a default value, therefore it shall not be null.
            Assert.Equal(1, angela.ContactsString.Count); // ContactsString default value contains one KVP, since JsonIgnoreNull is set, this should still contain only the default value.
            Assert.Same(Employee.ContactsDefault, angela.ContactsString);
        }
        #endregion

        // TODO: Add struct case where reference would evaluate as null; also, what does Json.Net does for that?
        #endregion

        #region Throw cases
        [Fact]
        public static void VerifyReferenceHandlingInJsonSerializerOptions()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new JsonSerializerOptions { ReferenceHandlingOnDeserialize = (ReferenceHandlingOnDeserialize)(-1) });
        }

        [Fact]
        public static void JsonPath()
        {
            string json = @"[0";
            JsonException ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<List<int>>(json, _deserializeOptions));

            Assert.Equal("$[0]", ex.Path);
        }

        [Fact]
        public static void JsonPathObject()
        {
            string json = @"{ ""Name"": ""A";
            JsonException ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Employee>(json, _deserializeOptions));

            Assert.Equal("$.Name", ex.Path);
        }

        [Fact]
        public static void JsonPathImcompletePropertyAfterMetadata()
        {
            string json =
            @"{
                ""$id"": ""1"",
                ""Nam";
            JsonException ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Employee>(json, _deserializeOptions));

            Assert.Equal("$.$id", ex.Path);
        }

        [Fact]
        public static void JsonPathImcompleteMetadataAfterProperty()
        {
            string json =
            @"{
                ""Name"": ""Angela"",
                ""$i";
            JsonException ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Employee>(json, _deserializeOptions));

            Assert.Equal("$.Name", ex.Path);
        }

        [Fact]
        public static void JsonPathCompleteMetadataButNotValue()
        {
            string json =
            @"{
                ""Name"": ""Angela"",
                ""$id"":";
            JsonException ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Employee>(json, _deserializeOptions));

            Assert.Equal("$.$id", ex.Path);
        }

        [Fact]
        public static void JsonPathIncompleteMetadataValue()
        {
            string json =
            @"{
                ""Name"": ""Angela"",
                ""$id"": ""1";
            JsonException ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Employee>(json, _deserializeOptions));

            Assert.Equal("$.$id", ex.Path);
        }

        [Fact]
        public static void JsonPathNestedObject()
        {
            string json = @"{ ""Name"": ""A"", ""Manager"": { ""Name"": ""B";
            JsonException ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Employee>(json, _deserializeOptions));

            Assert.Equal("$.Manager.Name", ex.Path);
        }

        [Fact]
        public static void JsonPathNestedArray()
        {
            string json = @"{ ""Subordinates"":";
            JsonException ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Employee>(json, _deserializeOptions));

            Assert.Equal("$.Subordinates", ex.Path);
        }

        [Fact]
        public static void JsonPathPreservedArray()
        {
            string json =
            @"{
                ""$id"": ""1"",
                ""$values"":[
                    1";
            JsonException ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<List<int>>(json, _deserializeOptions));

            Assert.Equal("$.$values[0]", ex.Path);
        }

        [Fact]
        public static void JsonPathImcompleteArrayId()
        {
            string json =
            @"{
                ""$id"": ""1";
            JsonException ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<List<int>>(json, _deserializeOptions));

            Assert.Equal("$.$id", ex.Path);
        }

        [Fact]
        public static void JsonPathImcompleteArrayValues()
        {
            string json =
            @"{
                ""$id"": ""1"",
                ""$values"":";
            JsonException ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<List<int>>(json, _deserializeOptions));

            Assert.Equal("$.$values", ex.Path);
        }

        [Fact]
        public static void JsonPathCurlyBraceOnArray()
        {
            string json = "{";
            JsonException ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<List<int>>(json, _deserializeOptions));

            Assert.Equal("$", ex.Path);
        }
        #endregion

        #region Throw on immutables
        private class EmployeeWithImmutable
        {
            public ImmutableList<EmployeeWithImmutable> Subordinates { get; set; }
            public EmployeeWithImmutable[] SubordinatesArray { get; set; }
            public ImmutableDictionary<string, EmployeeWithImmutable> Contacts { get; set; }
        }

        [Fact]
        public static void ImmutableEnumerableAsRoot()
        {
            string json =
            @"{
                ""$id"": ""1"",
                ""$values"": []
            }";

            JsonException ex;

            ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ImmutableList<EmployeeWithImmutable>>(json, _deserializeOptions));
            Assert.Equal("$", ex.Path);

            ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<EmployeeWithImmutable[]>(json, _deserializeOptions));
            Assert.Equal("$", ex.Path);
        }

        [Fact]
        public static void ImmutableDictionaryAsRoot()
        {
            string json =
            @"{
                ""$id"": ""1"",
                ""Employee1"": {}
            }";

            JsonException ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ImmutableDictionary<string, EmployeeWithImmutable>>(json, _deserializeOptions));
            Assert.Equal("$", ex.Path);
        }

        [Fact]
        public static void ImmutableEnumerableAsProperty()
        {
            string json =
            @"{
                ""$id"": ""1"",
                ""Subordinates"": {
                    ""$id"": ""2"",
                    ""$values"": []
                }
            }";

            JsonException ex;

            ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<EmployeeWithImmutable>(json, _deserializeOptions));
            Assert.Equal("$.Subordinates", ex.Path);

            json =
            @"{
                ""$id"": ""1"",
                ""SubordinatesArray"": {
                    ""$id"": ""2"",
                    ""$values"": []
                }
            }";

            ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<EmployeeWithImmutable>(json, _deserializeOptions));
            Assert.Equal("$.SubordinatesArray", ex.Path);
        }

        [Fact]
        public static void ImmutableDictionaryAsProperty()
        {
            string json =
            @"{
                ""$id"": ""1"",
                ""Contacts"": {
                    ""$id"": ""2"",
                    ""Employee1"": {}
                }
            }";

            JsonException ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<EmployeeWithImmutable>(json, _deserializeOptions));
            Assert.Equal("$.Contacts", ex.Path);
        }
        #endregion

        #region Ground Rules/Corner cases
        #region Reference objects ($ref)
        [Fact]
        public static void ReferenceObjectsShouldNotContainMoreProperties()
        {
            //Regular property before $ref
            string json = @"{
                ""$id"": ""1"",
                ""Name"": ""Angela"",
                ""Manager"": {
                    ""Name"": ""Bob"",
                    ""$ref"": ""1""
                }
            }";

            JsonException ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Employee>(json, _deserializeOptions));
            Assert.Equal("Reference objects cannot contain other properties.", ex.Message);

            //Regular property after $ref
            json = @"{
                ""$id"": ""1"",
                ""Name"": ""Angela"",
                ""Manager"": {
                    ""$ref"": ""1"",
                    ""Name"": ""Bob""
                }
            }";

            ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Employee>(json, _deserializeOptions));
            Assert.Equal("Reference objects cannot contain other properties.", ex.Message);

            //Metadata property before $ref
            json = @"{
                ""$id"": ""1"",
                ""Name"": ""Angela"",
                ""Manager"": {
                    ""$id"": ""2"",
                    ""$ref"": ""1""
                }
            }";

            ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Employee>(json, _deserializeOptions));
            Assert.Equal("Reference objects cannot contain other properties.", ex.Message);

            //Metadata property after $ref
            json = @"{
                ""$id"": ""1"",
                ""Name"": ""Angela"",
                ""Manager"": {
                    ""$ref"": ""1"",
                    ""$id"": ""2""
                }
            }";

            ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Employee>(json, _deserializeOptions));
            Assert.Equal("Reference objects cannot contain other properties.", ex.Message);
        }

        [Fact]
        public static void ReferenceObjectBeforePreservedObject()
        {
            string json = @"[
                {
                    ""$ref"": ""1""
                },
                {
                    ""$id"": ""1"",
                    ""Name"": ""Angela""
                }
            ]";

            List<Employee> root = JsonSerializer.Deserialize<List<Employee>>(json, _deserializeOptions);
            Assert.Null(root[0]);
            Assert.Equal("Angela", root[1].Name);
        }
        #endregion

        #region Preserved objects ($id)
        [Fact]
        public static void MoreThanOneId()
        {
            string json = @"{
                ""$id"": ""1"",
                ""$id"": ""2"",
                ""Name"": ""Angela"",
                ""Manager"": {
                    ""$ref"": ""1""
                }
            }";

            JsonException ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Employee>(json, _deserializeOptions));

            Assert.Equal("$.$id", ex.Path);
            Assert.Equal("Object already defines a reference identifier.", ex.Message);
        }

        [Fact]
        public static void IdIsNotFirstProperty()
        {
            string json = @"{
                ""Name"": ""Angela"",
                ""$id"": ""1"",
                ""Manager"": {
                    ""$ref"": ""1""
                }
            }";

            Employee angela = JsonSerializer.Deserialize<Employee>(json, _deserializeOptions);
            Assert.Same(angela, angela.Manager);
        }

        [Fact]
        public static void DuplicatedId()
        {
            string json = @"[
                {
                    ""$id"": ""1"",
                    ""Name"": ""Angela""
                },
                {
                    ""$id"": ""1"",
                    ""Name"": ""Bob""
                }
            ]";

            JsonException ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<List<Employee>>(json, _deserializeOptions));

            Assert.Equal("$[1].$id", ex.Path);
            Assert.Equal("Duplicated id found while preserving reference.", ex.Message);
        }
        #endregion

        #region Preserved arrays ($id and $values)
        [Fact]
        public static void PreservedArrayWithoutMetadata()
        {
            string json = "{}";
            JsonException ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<List<int>>(json, _deserializeOptions));

            Assert.Equal("$", ex.Path);
            Assert.Equal("Preserved array $values property was not present or its value is not an array.", ex.Message);
        }

        [Fact]
        public static void PreservedArrayWithoutValues()
        {
            string json = @"{
                ""$id"": ""1""
            }";
            JsonException ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<List<int>>(json, _deserializeOptions));

            Assert.Equal("$.$id", ex.Path); // Not sure if is ok for the Path to have this value.
            Assert.Equal("Preserved array $values property was not present or its value is not an array.", ex.Message);
        }

        [Fact]
        public static void PreservedArrayWithoutId()
        {
            string json = @"{
                ""$values"": []
            }";

            JsonException ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<List<int>>(json, _deserializeOptions));

            Assert.Equal("$.$values", ex.Path);
            Assert.Equal("Preserved arrays canot lack an identifier.", ex.Message);
        }

        [Fact]
        public static void PreservedArrayValuesContainsNull()
        {
            string json = @"{
                ""$id"": ""1"",
                ""$values"": null
            }";

            JsonException ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<List<int>>(json, _deserializeOptions));

            //TODO: JsonPath lacks $values due HandleNull token calls EndProperty. either add special case or find a way to make $values not-nullable.
            //Assert.Equal("$.$values", ex.Path); 
            Assert.Equal("Preserved array $values property was not present or its value is not an array.", ex.Message);
        }

        [Fact]
        public static void PreservedArrayValuesContainsValue()
        {
            string json = @"{
                ""$id"": ""1"",
                ""$values"": 1
            }";

            JsonException ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<List<Employee>>(json, _deserializeOptions));

            Assert.Equal("$.$values", ex.Path);
        }

        [Fact]
        public static void PreservedArrayValuesContainsObject()
        {
            string json = @"{
                ""$id"": ""1"",
                ""$values"": {}
            }";

            JsonException ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<List<Employee>>(json, _deserializeOptions));

            Assert.Equal("$.$values", ex.Path);
            Assert.Equal("The property is already part of a preserved array object, cannot be read as a preserved array.", ex.Message);
        }
        #endregion
        //TODO: Add more tests for dictionaries.
        #endregion
    }
}
