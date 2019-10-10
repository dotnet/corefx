// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System.Collections.Generic;
using System.Collections.Immutable;
using Xunit;

namespace System.Text.Json.Tests
{
    public static partial class ReferenceHandlingTests
    {
        private static JsonSerializerOptions _deserializeOptions = new JsonSerializerOptions { ReadReferenceHandling = ReferenceHandlingOnDeserialize.PreserveDuplicates };

        private class EmployeeWithContacts
        {
            public string Name { get; set; }
            public EmployeeWithContacts Manager { get; set; }
            public List<EmployeeWithContacts> Subordinates { get; set; }
            public Dictionary<string, EmployeeWithContacts> Contacts { get; set; }
        }

        private class Employee
        {
            public string Name { get; set; }
            public Employee Manager { get; set; }
            public List<Employee> Subordinates { get; set; }
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
        public static void ObjectWithArrayReferencedDeeper()
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

        [Fact] //Employee Dictionary as a property and then use reference to itself on nested Employee.
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
        public static void EmptyArray() //Make sure the serializer can understand lists that were wrapped in braces.
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
            options.ReadReferenceHandling = ReferenceHandlingOnDeserialize.PreserveDuplicates;
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
        public static void ObjectNull() //Make sure the serializer can understand lists that were wrapped in braces.
        {
            string json =
            @"{
                ""$ref"": ""1""
            }";

            Employee employee = JsonSerializer.Deserialize<Employee>(json, _deserializeOptions);
            Assert.Null(employee);
        }

        [Fact]
        public static void ArrayNull() //Make sure the serializer can understand lists that were wrapped in braces.
        {
            string json =
            @"{
                ""$ref"": ""1""
            }";

            Employee[] array = JsonSerializer.Deserialize<Employee[]>(json, _deserializeOptions);
            Assert.Null(array);
        }

        [Fact]
        public static void DictionaryNull() //Make sure the serializer can understand lists that were wrapped in braces.
        {
            string json =
            @"{
                ""$ref"": ""1""
            }";

            Dictionary<string, Employee> dictionary = JsonSerializer.Deserialize<Dictionary<string, Employee>>(json, _deserializeOptions);
            Assert.Null(dictionary);
        }

        [Fact]
        public static void ArrayPropertyNull() //Make sure the serializer can understand lists that were wrapped in braces.
        {
            string json =
            @"{
                ""$id"": ""1"",
                ""Name"": ""Angela"",
                ""Manager"": {
                    ""$ref"": ""1""
                },
                ""Subordinates"": {
                    ""$ref"": ""2""
                }
            }";

            Employee angela = JsonSerializer.Deserialize<Employee>(json, _deserializeOptions);
            Assert.Null(angela.Subordinates);
        }

        // TODO: Add struct case where reference would evaluate as null; also, what does Json.Net does for that?
        #endregion

        #region Throw cases
        [Fact]
        public static void VerifyReferenceHandlingInJsonSerializerOptions()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new JsonSerializerOptions { ReadReferenceHandling = (ReferenceHandlingOnDeserialize) (-1) });
        }

        [Fact]
        public static void PropertyAfterRef()
        {
            string json =
            @"{
              ""$id"": ""1"",
              ""Name"": ""Angela"",
              ""Manager"": {
                    ""$ref"": ""1"",
                    ""Name"": ""Bob""
                }
            }";

            Exception ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Employee>(json, _deserializeOptions));
            Assert.Equal("Properties other than '$ref' are not allowed in reference objects.", ex.Message);
        }

        [Fact] 
        public static void PropertyBeforeRef()
        {
            string json =
            @"{
              ""$id"": ""1"",
              ""Name"": ""Angela"",
              ""Manager"": {
                    ""Name"": ""Bob"",
                    ""$ref"": ""1""
                }
            }";

            Exception ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Employee>(json, _deserializeOptions));
            Assert.Equal("Properties other than '$ref' are not allowed in reference objects.", ex.Message);
        }

        //Immutables
        [Fact] 
        public static void ImmutableListTryPreserve()
        {
            string json =
            @"{
                ""$id"": ""1"",
                ""$values"": []
            }";

            Exception ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ImmutableList<Employee>>(json, _deserializeOptions));
            Assert.Equal("Immutable enumerable types are not supported.", ex.Message);
        }

        [Fact] 
        public static void ImmutableDictionaryTryPreserve()
        {
            string json =
            @"{
                ""$id"": ""1"",
                ""Angela"": {}
            }";

            Exception ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ImmutableDictionary<string, Employee>>(json, _deserializeOptions));
            Assert.Equal("Immutable dictionary types are not supported.", ex.Message);
        }

        private class EmployeeWithImmutables
        {
            public string Name { get; set; }
            public EmployeeWithImmutables Manager { get; set; }
            public ImmutableList<EmployeeWithImmutables> Subordinates { get; set; }
            public ImmutableDictionary<string, EmployeeWithImmutables> Contacts { get; set; }
        }

        [Fact]
        public static void ImmutableListAsPropertyTryPreserve()
        {
            string json =
            @"{
                ""$id"": ""1"",
                ""Name"": ""Angela"",
                ""Subordinates"": {
                    ""$id"": ""2"",
                    ""$values"": []
                }
            }";

            Exception ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<EmployeeWithImmutables>(json, _deserializeOptions));
            Assert.Equal("Immutable enumerable types are not supported.", ex.Message);
        }

        [Fact]
        public static void ImmutableDictionaryAsPropertyTryPreserve()
        {
            string json =
            @"{
                ""$id"": ""1"",
                ""Name"": ""Angela"",
                ""Contacts"": {
                    ""$id"": ""2""
                }
            }";

            Exception ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<EmployeeWithImmutables>(json, _deserializeOptions));
            Assert.Equal("Immutable dictionary types are not supported.", ex.Message);
        }
        #endregion
    }
}
