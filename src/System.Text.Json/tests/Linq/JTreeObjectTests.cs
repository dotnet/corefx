// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json.Tests;
using Xunit;

namespace System.Text.Json.Linq.Tests
{
    public static class JTreeObjectTests
    {
        [Fact]
        public static void TestDefaultConstructor()
        {
            var jsonObject = new JTreeObject();
            Assert.Equal(0, jsonObject.GetPropertyNames().Count);
            Assert.Equal(0, jsonObject.GetPropertyValues().Count);
        }

        [Fact]
        public static void TestIEnumerableKVPConstructor()
        {
            var jsonProperties = new List<KeyValuePair<string, JTreeNode>>();
            jsonProperties.Add(new KeyValuePair<string, JTreeNode>("number", new JTreeNumber(17)));
            jsonProperties.Add(new KeyValuePair<string, JTreeNode>("string", new JTreeString("property value")));
            jsonProperties.Add(new KeyValuePair<string, JTreeNode>("boolean", new JTreeBoolean(true)));

            var jsonObject = new JTreeObject(jsonProperties);

            Assert.Equal(17, ((JTreeNumber)jsonObject["number"]).GetInt32());
            Assert.Equal("property value", ((JTreeString)jsonObject["string"]).Value);
            Assert.True(((JTreeBoolean)jsonObject["boolean"]).Value);
        }

        [Fact]
        public static void TestDuplicates()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                JTreeObject jsonObject = new JTreeObject();
                jsonObject.Add("property", "value1");
                jsonObject.Add("property", "value2");
            });

            JTreeObject jsonObject = new JTreeObject() { { "property", "value" } };
            Assert.Equal("value", jsonObject["property"]);

            jsonObject["property"] = "indexer value";
            Assert.Equal("indexer value", jsonObject["property"]);
        }

        [Fact]
        public static void TestNumerics()
        {
            var jsonObject = new JTreeObject();

            jsonObject.Add("byte", byte.MaxValue);
            Assert.Equal(byte.MaxValue, ((JTreeNumber)jsonObject["byte"]).GetByte());

            jsonObject.Add("short", short.MaxValue);
            Assert.Equal(short.MaxValue, ((JTreeNumber)jsonObject["short"]).GetInt16());

            jsonObject.Add("int", int.MaxValue);
            Assert.Equal(int.MaxValue, ((JTreeNumber)jsonObject["int"]).GetInt32());

            jsonObject.Add("long", long.MaxValue);
            Assert.Equal(long.MaxValue, ((JTreeNumber)jsonObject["long"]).GetInt64());

            jsonObject.Add("float", 3.14f);
            Assert.Equal(3.14f, ((JTreeNumber)jsonObject["float"]).GetSingle());

            jsonObject.Add("double", 3.14);
            Assert.Equal(3.14, ((JTreeNumber)jsonObject["double"]).GetDouble());

            jsonObject.Add("sbyte", sbyte.MaxValue);
            Assert.Equal(sbyte.MaxValue, ((JTreeNumber)jsonObject["sbyte"]).GetSByte());

            jsonObject.Add("ushort", ushort.MaxValue);
            Assert.Equal(ushort.MaxValue, ((JTreeNumber)jsonObject["ushort"]).GetUInt16());

            jsonObject.Add("uint", uint.MaxValue);
            Assert.Equal(uint.MaxValue, ((JTreeNumber)jsonObject["uint"]).GetUInt32());

            jsonObject.Add("ulong", ulong.MaxValue);
            Assert.Equal(ulong.MaxValue, ((JTreeNumber)jsonObject["ulong"]).GetUInt64());

            jsonObject.Add("decimal", decimal.One);
            Assert.Equal(decimal.One, ((JTreeNumber)jsonObject["decimal"]).GetDecimal());
        }

        [Fact]
        public static void TestGuid()
        {
            var guidString = "ca761232-ed42-11ce-bacd-00aa0057b223";
            Guid guid = new Guid(guidString);
            var jsonObject = new JTreeObject { { "guid", guid } };
            Assert.Equal(guidString, ((JTreeString)jsonObject["guid"]).Value);
        }

        [Theory]
        [MemberData(nameof(JsonDateTimeTestData.DateTimeFractionTrimBaseTests), MemberType = typeof(JsonDateTimeTestData))]
        [MemberData(nameof(JsonDateTimeTestData.DateTimeFractionTrimUtcOffsetTests), MemberType = typeof(JsonDateTimeTestData))]
        public static void TestDateTime(string testStr, string expectedStr)
        {
            var dateTime = DateTime.ParseExact(testStr, "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

            var jsonObject = new JTreeObject { { "dateTime", dateTime } };

            Assert.Equal(expectedStr, ((JTreeString)jsonObject["dateTime"]).Value);
        }

        [Theory]
        [MemberData(nameof(JsonDateTimeTestData.DateTimeOffsetFractionTrimTests), MemberType = typeof(JsonDateTimeTestData))]
        public static void TestDateTimeOffset(string testStr, string expectedStr)
        {
            var dateTimeOffset = DateTimeOffset.ParseExact(testStr, "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

            var jsonObject = new JTreeObject { { "dateTimeOffset", dateTimeOffset } };

            Assert.Equal(expectedStr, ((JTreeString)jsonObject["dateTimeOffset"]).Value);
        }

        [Fact]
        public static void TestCreatingJTreeObject()
        {
            var developer = new JTreeObject
            {
                { "name", "Kasia" },
                { "n\\u0061me", "Kasia" }, // different property name than above one
                { "age", 22 },
                { "is developer", true },
                { "null property", (JTreeNode) null }
            };

            Assert.Equal("Kasia", ((JTreeString)developer["name"]).Value);
            Assert.Equal("Kasia", ((JTreeString)developer["n\\u0061me"]).Value);
            Assert.Equal(22, ((JTreeNumber)developer["age"]).GetInt32());
            Assert.True(((JTreeBoolean)developer["is developer"]).Value);
            Assert.IsType<JTreeNull>(developer["null property"]);
        }

        [Fact]
        public static void TestCreatingJTreeObjectNewMethods()
        {
            var developer = new JTreeObject
            {
                { "name", new JTreeString("Kasia") },
                { "age", new JTreeNumber(22) },
                { "is developer", new JTreeBoolean(true) },
                { "null property", new JTreeNull() }
            };

            Assert.Equal("Kasia", ((JTreeString)developer["name"]).Value);
            Assert.Equal(22, ((JTreeNumber)developer["age"]).GetInt32());
            Assert.True(((JTreeBoolean)developer["is developer"]).Value);
            Assert.IsType<JTreeNull>(developer["null property"]);
        }

        [Fact]
        public static void TestCreatingJTreeObjectDictionaryInitializerSyntax()
        {
            var developer = new JTreeObject
            {
                ["name"] = "Kasia",
                ["age"] = 22,
                ["is developer"] = true,
                ["null property"] = null
            };

            Assert.Equal("Kasia", ((JTreeString)developer["name"]).Value);
            Assert.Equal(22, ((JTreeNumber)developer["age"]).GetInt32());
            Assert.True(((JTreeBoolean)developer["is developer"]).Value);
            Assert.IsType<JTreeNull>(developer["null property"]);
        }

        [Fact]
        public static void TestCreatingNestedJTreeObject()
        {
            var person = new JTreeObject
            {
                { "name", "John" },
                { "surname", "Smith" },
                {
                    "phone numbers", new JTreeObject()
                    {
                        { "work", "123-456-7890" },
                        { "home", "123-456-7890" }
                    }
                },
                {
                    "addresses", new JTreeObject()
                    {
                        {
                            "office", new JTreeObject()
                            {
                                {  "address line 1", "One Microsoft Way" },
                                {  "city" , "Redmond" } ,
                                {  "zip code" , 98052 } ,
                                {  "state" , (int) AvailableStateCodes.WA }
                            }
                        },
                        {
                            "home", new JTreeObject()
                            {
                                {  "address line 1", "Pear Ave" },
                                {  "address line 2", "1288" },
                                {  "city" , "Mountain View" } ,
                                {  "zip code" , 94043 } ,
                                {  "state" , (int) AvailableStateCodes.CA }
                            }
                        }
                    }
                }
            };

            Assert.IsType<JTreeObject>(person["phone numbers"]);
            var phoneNumbers = person["phone numbers"] as JTreeObject;
            Assert.IsType<JTreeString>(phoneNumbers["work"]);
            Assert.IsType<JTreeString>(phoneNumbers["home"]);

            Assert.IsType<JTreeObject>(person["addresses"]);
            var addresses = person["addresses"] as JTreeObject;
            Assert.IsType<JTreeObject>(addresses["office"]);
            Assert.IsType<JTreeObject>(addresses["home"]);
        }

        [Fact]
        public static void TestAssignmentDefinition()
        {
            JTreeNode employee = EmployeesDatabase.GetNextEmployee().Value;
            Assert.IsType<JTreeObject>(employee);
        }

        private static void CheckEmployeesAreDifferent(JTreeObject employees)
        {
            string prevId = "";
            foreach (KeyValuePair<string, JTreeNode> employee in employees)
            {
                Assert.NotEqual(prevId, employee.Key);
                prevId = employee.Key;

                Assert.IsType<JTreeObject>(employee.Value);
            }
        }

        [Fact]
        public static void TestAddingKeyValuePair()
        {
            var employees = new JTreeObject
            {
                EmployeesDatabase.GetNextEmployee(),
                EmployeesDatabase.GetNextEmployee(),
                EmployeesDatabase.GetNextEmployee(),
                EmployeesDatabase.GetNextEmployee(),
            };

            CheckEmployeesAreDifferent(employees);
        }

        [Fact]
        public static void TestAddingKeyValuePairAfterInitialization()
        {
            var employees = new JTreeObject();
            foreach (KeyValuePair<string, JTreeNode> employee in EmployeesDatabase.GetTenBestEmployees())
            {
                employees.Add(employee);
            }

            CheckEmployeesAreDifferent(employees);
        }

        [Fact]
        public static void TestAddingKeyValuePairsCollection()
        {
            var employees = new JTreeObject(EmployeesDatabase.GetTenBestEmployees());

            CheckEmployeesAreDifferent(employees);
        }

        [Fact]
        public static void TestAddingKeyValuePairsCollectionAfterInitialization()
        {
            var employees = new JTreeObject();
            employees.AddRange(EmployeesDatabase.GetTenBestEmployees());

            CheckEmployeesAreDifferent(employees);
        }

        [Fact]
        public static void TestAddingJTreeArray()
        {
            var preferences = new JTreeObject()
            {
                { "prime numbers", new JTreeArray { 19, 37 } }
            };

            var primeNumbers = (JTreeArray)preferences["prime numbers"];
            Assert.Equal(2, primeNumbers.Count);

            int[] expected = { 19, 37 };

            for (int i = 0; i < primeNumbers.Count; i++)
            {
                Assert.Equal(expected[i], ((JTreeNumber)primeNumbers[i]).GetInt32());
            }
        }

        [Fact]
        public static void TestGetJsonArrayPropertyValue()
        {
            var jsonObject = new JTreeObject()
            {
                { "array", new JTreeArray() { 1, 2 } }
            };

            JTreeArray jsonArray = jsonObject.GetJsonArrayPropertyValue("array");
            Assert.Equal(2, jsonArray.Count);
            Assert.Equal(1, jsonArray[0]);
            Assert.Equal(2, jsonArray[1]);
        }

        [Fact]
        public static void TestAddingNull()
        {
            var jsonObject = new JTreeObject
            {
                { "null1", null },
                { "null2", (JTreeNode)null },
                { "null3", (JTreeNull)null },
                { "null4", new JTreeNull() },
                { "null5", (string)null },
            };

            Assert.IsType<JTreeNull>(jsonObject["null1"]);
            Assert.IsType<JTreeNull>(jsonObject["null2"]);
            Assert.IsType<JTreeNull>(jsonObject["null3"]);
            Assert.IsType<JTreeNull>(jsonObject["null4"]);
            Assert.IsType<JTreeNull>(jsonObject["null5"]);

            jsonObject["null1"] = null;
            Assert.IsType<JTreeNull>(jsonObject["null1"]);
        }

        [Fact]
        public static void TestContains()
        {
            var person = new JTreeObject
            {
                { "name", "John" },
                { "ssn", "123456789" },
            };

            Assert.True(person.ContainsProperty("ssn"));
            Assert.Equal("123456789", ((JTreeString)person["ssn"]).Value);
            Assert.False(person.ContainsProperty("surname"));
        }

        [Fact]
        public static void TestAquiringAllValues()
        {
            var employees = new JTreeObject(EmployeesDatabase.GetTenBestEmployees());
            IReadOnlyCollection<JTreeNode> employeesWithoutId = employees.GetPropertyValues();

            Assert.Equal(10, employees.GetPropertyNames().Count);
            Assert.Equal(10, employees.GetPropertyValues().Count);

            foreach (JTreeNode employee in employeesWithoutId)
            {
                Assert.IsType<JTreeObject>(employee);
            }
        }

        [Fact]
        public static void TestReplacingsonObjectPrimaryTypes()
        {
            var person1 = new JTreeObject
            {
                { "name", "John" },
                { "age", 45 },
                { "is_married", true }
            };


            person1["name"] = new JTreeString("Bob");
            Assert.Equal("Bob", ((JTreeString)person1["name"]).Value);

            person1["age"] = new JTreeNumber(55);
            Assert.Equal(55, ((JTreeNumber)person1["age"]).GetInt32());

            person1["is_married"] = new JTreeBoolean(false);
            Assert.False(((JTreeBoolean)person1["is_married"]).Value);

            var person2 = new JTreeObject
            {
                { "name", "Bob" },
                { "age", 33 },
                { "is_married", true }
            };

            // Copy property from another JTreeObject
            person1["age"] = person2["age"];

            Assert.Equal(33, ((JTreeNumber)person1["age"]).GetInt32());

            // Copy property of different typoe
            person1["name"] = person2["name"];
        }

        [Fact]
        public static void TestModifyingJTreeObjectKeyRemoveAdd()
        {
            JTreeObject manager = EmployeesDatabase.GetManager();
            JTreeObject reportingEmployees = manager.GetJsonObjectPropertyValue("reporting employees");

            static void ModifyProperty(JTreeObject jsonObject, string previousName, string newName)
            {
                JTreeNode previousValue = jsonObject[previousName];
                jsonObject.Remove(previousName);
                jsonObject.Add(newName, previousValue);
            }

            string previousName = "software developers";
            string newName = "software engineers";

            Assert.True(reportingEmployees.ContainsProperty(previousName));
            JTreeNode previousValue = reportingEmployees[previousName];

            ModifyProperty(reportingEmployees, previousName, newName);

            Assert.False(reportingEmployees.ContainsProperty(previousName));
            Assert.True(reportingEmployees.ContainsProperty(newName));
            Assert.Equal(previousValue, reportingEmployees[newName]);
        }

        [Fact]
        public static void TestAccessingNestedJTreeObjectCastWithAs()
        {
            JTreeObject manager = EmployeesDatabase.GetManager();

            var reportingEmployees = manager["reporting employees"] as JTreeObject;
            Assert.NotNull(reportingEmployees);

            var softwareDevelopers = reportingEmployees["software developers"] as JTreeObject;
            Assert.NotNull(softwareDevelopers);

            var internDevelopers = softwareDevelopers["intern employees"] as JTreeObject;
            Assert.NotNull(internDevelopers);

            internDevelopers.Add(EmployeesDatabase.GetNextEmployee());
        }

        [Fact]
        public static void TestAccessingNestedJTreeObjectCastWithIs()
        {
            JTreeObject manager = EmployeesDatabase.GetManager();

            static bool AddEmployee(JTreeObject manager)
            {

                if (manager["reporting employees"] is JTreeObject reportingEmployees)
                {
                    if (reportingEmployees["software developers"] is JTreeObject softwareDevelopers)
                    {
                        if (softwareDevelopers["full time employees"] is JTreeObject fullTimeEmployees)
                        {
                            fullTimeEmployees.Add(EmployeesDatabase.GetNextEmployee());
                            return true;
                        }
                    }
                }
                return false;
            }

            bool success = AddEmployee(manager);
            Assert.True(success);
        }

        [Fact]
        public static void TestAccessingNestedJTreeObjectExplicitCast()
        {
            JTreeObject manager = EmployeesDatabase.GetManager();

            // Should not throw any exceptions:
            ((JTreeObject)((JTreeObject)manager["reporting employees"])["HR"]).Add(EmployeesDatabase.GetNextEmployee());
        }

        [Fact]
        public static void TestAccessingNestedJTreeObjectGetPropertyMethod()
        {
            JTreeObject manager = EmployeesDatabase.GetManager();

            // Should not throw any exceptions:

            JTreeObject internDevelopers = manager.GetJsonObjectPropertyValue("reporting employees")
                                          .GetJsonObjectPropertyValue("software developers")
                                          .GetJsonObjectPropertyValue("intern employees");
            internDevelopers.Add(EmployeesDatabase.GetNextEmployee());
        }

        [Fact]
        public static void TestAccessingNestedJTreeObjectTryGetPropertyMethod()
        {
            JTreeObject manager = EmployeesDatabase.GetManager();

            static bool AddEmployee(JTreeObject manager)
            {
                if (manager.TryGetJsonObjectPropertyValue("reporting employees", out JTreeObject reportingEmployees))
                {
                    if (reportingEmployees.TryGetJsonObjectPropertyValue("software developers", out JTreeObject softwareDevelopers))
                    {
                        if (softwareDevelopers.TryGetJsonObjectPropertyValue("full time employees", out JTreeObject fullTimeEmployees))
                        {
                            fullTimeEmployees.Add(EmployeesDatabase.GetNextEmployee());
                            return true;
                        }
                    }
                }

                return false;
            }

            bool success = AddEmployee(manager);
            Assert.True(success);
        }

        [Fact]
        public static void TestGetPropertyThrows()
        {
            Assert.Throws<KeyNotFoundException>(() =>
            {
                var jsonObject = new JTreeObject()
                {
                    { "name", "value" }
                };

                jsonObject.GetPropertyValue("different name");
            });
        }

        [Fact]
        public static void TestTryGetProperty()
        {
            var jsonObject = new JTreeObject()
            {
                { "name", "value" }
            };

            Assert.True(jsonObject.TryGetPropertyValue("name", out JTreeNode property));
            Assert.Equal("value", ((JTreeString)property).Value);
            Assert.False(jsonObject.TryGetPropertyValue("other", out property));
            Assert.Null(property);
        }

        [Fact]
        public static void TestGetJTreeObjectPropertyThrows()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var jsonObject = new JTreeObject()
                {
                    { "name", "value" }
                };

                jsonObject.GetJsonObjectPropertyValue("name");
            });
        }

        [Fact]
        public static void TestTryGetObjectPropertyFails()
        {
            var jsonObject = new JTreeObject()
            {
                { "name", "value" }
            };

            Assert.False(jsonObject.TryGetJsonObjectPropertyValue("name", out JTreeObject property));
            Assert.Null(property);

            Assert.False(jsonObject.TryGetJsonObjectPropertyValue("other", out property));
            Assert.Null(property);
        }

        [Fact]
        public static void TestGetJTreeArrayPropertyThrows()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var jsonObject = new JTreeObject()
                {
                    { "name", "value" }
                };

                jsonObject.GetJsonArrayPropertyValue("name");
            });
        }

        [Fact]
        public static void TestTryGetArrayPropertyFails()
        {
            var jsonObject = new JTreeObject()
            {
                { "name", "value" }
            };

            Assert.False(jsonObject.TryGetJsonArrayPropertyValue("name", out JTreeArray property));
            Assert.Null(property);

            Assert.False(jsonObject.TryGetJsonArrayPropertyValue("other", out property));
            Assert.Null(property);
        }

        [Fact]
        public static void TestArgumentNullValidation()
        {
            Assert.Throws<ArgumentNullException>(() => new JTreeObject().Add(null, ""));
            Assert.Throws<ArgumentNullException>(() => new JTreeObject().Add(null, new JTreeObject()));
            Assert.Throws<ArgumentNullException>(() => new JTreeObject().Add(null, (byte)17));
            Assert.Throws<ArgumentNullException>(() => new JTreeObject().Add(null, (short)17));
            Assert.Throws<ArgumentNullException>(() => new JTreeObject().Add(null, 17));
            Assert.Throws<ArgumentNullException>(() => new JTreeObject().Add(null, (long)17));
            Assert.Throws<ArgumentNullException>(() => new JTreeObject().Add(null, 3.14f));
            Assert.Throws<ArgumentNullException>(() => new JTreeObject().Add(null, 3.14));
            Assert.Throws<ArgumentNullException>(() => new JTreeObject().Add(null, decimal.One));
            Assert.Throws<ArgumentNullException>(() => new JTreeObject().Add(null, (sbyte)17));
            Assert.Throws<ArgumentNullException>(() => new JTreeObject().Add(null, (ushort)17));
            Assert.Throws<ArgumentNullException>(() => new JTreeObject().Add(null, (uint)17));
            Assert.Throws<ArgumentNullException>(() => new JTreeObject().Add(null, (ulong)17));
            Assert.Throws<ArgumentNullException>(() => new JTreeObject().Add(new KeyValuePair<string, JTreeNode>(null, new JTreeObject())));
            Assert.Throws<ArgumentNullException>(() =>
            {
                var property = new KeyValuePair<string, JTreeNode>(null, new JTreeObject());
                new JTreeObject().AddRange(new List<KeyValuePair<string, JTreeNode>>() { property });
            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                var property1 = new KeyValuePair<string, JTreeNode>("regular property", new JTreeObject());
                var property2 = new KeyValuePair<string, JTreeNode>("regular property2", new JTreeObject());
                var nullProperty = new KeyValuePair<string, JTreeNode>(null, new JTreeObject());
                var propertyList = new List<KeyValuePair<string, JTreeNode>>() { property1, nullProperty, property2 };
                var jsonObject = new JTreeObject(propertyList);
            });
            Assert.Throws<ArgumentNullException>(() => new JTreeObject()[null] = new JTreeString());
            Assert.Throws<ArgumentNullException>(() =>
            {
                var jsonObject = new JTreeObject();
                JTreeNode x = jsonObject[null];
            });
            Assert.Throws<ArgumentNullException>(() => new JTreeObject().Remove(null));
            Assert.Throws<ArgumentNullException>(() => new JTreeObject().ContainsProperty(null));
        }

        [Fact]
        public static void TestStringComparisonEnum()
        {
            var jsonObject = new JTreeObject()
            {
                { "not encyclopaedia", "value1" },
                { "Encyclopaedia", "value2" },
                { "NOT encyclopaedia", "value3" },
                { "encyclopaedia", "value4" }
            };

            Assert.Equal(4, jsonObject.Count());

            Assert.False(jsonObject.ContainsProperty("ENCYCLOPAEDIA"));
            Assert.False(jsonObject.TryGetPropertyValue("ENCYCLOPAEDIA", out JTreeNode jsonNode));
            Assert.Null(jsonNode);
            Assert.Throws<KeyNotFoundException>(() => jsonObject.GetPropertyValue("ENCYCLOPAEDIA"));
            jsonObject.Remove("ENCYCLOPAEDIA");
            Assert.Equal(4, jsonObject.Count());

            Assert.False(jsonObject.ContainsProperty("ENCYCLOPAEDIA", StringComparison.CurrentCulture));
            Assert.False(jsonObject.TryGetPropertyValue("ENCYCLOPAEDIA", StringComparison.CurrentCulture, out jsonNode));
            Assert.Null(jsonNode);
            Assert.Throws<KeyNotFoundException>(() => jsonObject.GetPropertyValue("ENCYCLOPAEDIA", StringComparison.CurrentCulture));
            jsonObject.Remove("ENCYCLOPAEDIA", StringComparison.CurrentCulture);
            Assert.Equal(4, jsonObject.Count());

            Assert.True(jsonObject.ContainsProperty("ENCYCLOPAEDIA", StringComparison.InvariantCultureIgnoreCase));
            Assert.True(jsonObject.TryGetPropertyValue("ENCYCLOPAEDIA", StringComparison.InvariantCultureIgnoreCase, out jsonNode));
            Assert.Equal("value2", jsonNode);
            Assert.Equal("value2", jsonObject.GetPropertyValue("ENCYCLOPAEDIA", StringComparison.InvariantCultureIgnoreCase));
            jsonObject.Remove("ENCYCLOPAEDIA", StringComparison.InvariantCultureIgnoreCase);
            Assert.Equal(3, jsonObject.Count());

            IReadOnlyCollection<JTreeNode> values = jsonObject.GetPropertyValues();
            Assert.False(values.Contains("value2"));
            Assert.True(values.Contains("value1"));
            Assert.True(values.Contains("value3"));
            Assert.True(values.Contains("value4"));

            jsonObject = new JTreeObject()
            {
                { "object first", new JTreeObject() },
                { "object FIRST", new JTreeArray() },
                { "array first", new JTreeArray() },
                { "array FIRST", new JTreeObject() }
            };

            Assert.Equal(0, jsonObject.GetJsonObjectPropertyValue("OBJECT first",
                StringComparison.InvariantCultureIgnoreCase).GetPropertyNames().Count);
            Assert.True(jsonObject.TryGetJsonObjectPropertyValue("OBJECT first", StringComparison.InvariantCultureIgnoreCase,
                out JTreeObject objectProperty));
            Assert.Equal(0, objectProperty.GetPropertyNames().Count);

            Assert.Throws<ArgumentException>(() =>
                jsonObject.GetJsonArrayPropertyValue("OBJECT first", StringComparison.InvariantCultureIgnoreCase));
            Assert.False(jsonObject.TryGetJsonArrayPropertyValue("OBJECT first", StringComparison.InvariantCultureIgnoreCase,
                out JTreeArray arrayProperty));
            Assert.False(jsonObject.TryGetJsonArrayPropertyValue("something different", StringComparison.InvariantCultureIgnoreCase,
                out arrayProperty));

            Assert.Equal(0, jsonObject.GetJsonArrayPropertyValue("ARRAY first",
                StringComparison.InvariantCultureIgnoreCase).Count);
            Assert.True(jsonObject.TryGetJsonArrayPropertyValue("ARRAY first", StringComparison.InvariantCultureIgnoreCase,
                out arrayProperty));
            Assert.Equal(0, arrayProperty.Count);

            Assert.Throws<ArgumentException>(() =>
                jsonObject.GetJsonObjectPropertyValue("ARRAY first", StringComparison.InvariantCultureIgnoreCase));
            Assert.False(jsonObject.TryGetJsonObjectPropertyValue("ARRAY first", StringComparison.InvariantCultureIgnoreCase,
                out objectProperty));
            Assert.False(jsonObject.TryGetJsonObjectPropertyValue("something different", StringComparison.InvariantCultureIgnoreCase,
                out objectProperty));

            Assert.Throws<ArgumentNullException>(() =>
               jsonObject.Remove(null, StringComparison.InvariantCultureIgnoreCase));
        }

        [Fact]
        public static void TestValueKind()
        {
            Assert.Equal(JsonValueKind.Object, new JTreeObject().ValueKind);
        }

        [Fact]
        public static void TestRemoveLastProperty()
        {
            var jsonObject = new JTreeObject()
            {
                { "first", "value1" },
                { "middle", "value2" },
                { "last", "" }
            };

            jsonObject.Remove("last");
            Assert.Equal(2, jsonObject.GetPropertyNames().Count());
            Assert.Equal(2, jsonObject.GetPropertyValues().Count());
            Assert.Equal("value1", jsonObject["first"]);
            Assert.Equal("value2", jsonObject["middle"]);
        }

        [Fact]
        public static void TestJTreeObjectIEnumerator()
        {
            var jsonObject = new JTreeObject()
            {
                ["first"] = 17,
                ["second"] = "value"
            };

            // Test generic IEnumerator:
            IEnumerator<KeyValuePair<string, JTreeNode>> jsonObjectEnumerator = new JTreeObject.Enumerator(jsonObject);

            Assert.Null(jsonObjectEnumerator.Current.Key);
            Assert.Null(jsonObjectEnumerator.Current.Value);

            jsonObjectEnumerator.MoveNext();
            Assert.Equal("first", jsonObjectEnumerator.Current.Key);
            Assert.Equal(17, jsonObjectEnumerator.Current.Value);
            jsonObjectEnumerator.MoveNext();
            Assert.Equal("second", jsonObjectEnumerator.Current.Key);
            Assert.Equal("value", jsonObjectEnumerator.Current.Value);

            jsonObjectEnumerator.Reset();

            jsonObjectEnumerator.MoveNext();
            Assert.Equal("first", jsonObjectEnumerator.Current.Key);
            Assert.Equal(17, jsonObjectEnumerator.Current.Value);
            jsonObjectEnumerator.MoveNext();
            Assert.Equal("second", jsonObjectEnumerator.Current.Key);
            Assert.Equal("value", jsonObjectEnumerator.Current.Value);
        }

        [Fact]
        public static void TestJTreeObjectEmptyObjectEnumerator()
        {
            var jsonObject = new JTreeObject();
            var jsonObjectEnumerator = new JTreeObject.Enumerator(jsonObject);

            Assert.Null(jsonObjectEnumerator.Current.Key);
            Assert.Null(jsonObjectEnumerator.Current.Value);
            Assert.False(jsonObjectEnumerator.MoveNext());
        }
    }
}
