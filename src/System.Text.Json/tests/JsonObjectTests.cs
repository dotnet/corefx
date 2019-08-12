// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Text.Json.Tests
{
    public static partial class JsonObjectTests
    {
        [Fact]
        public static void TestDefaultConstructor()
        {
            var jsonObject = new JsonObject();
            Assert.Equal(0, jsonObject.PropertyNames.Count);
            Assert.Equal(0, jsonObject.PropertyValues.Count);
        }

        [Fact]
        public static void TestIEnumerableKVPConstructor()
        {
            var jsonProperties = new List<KeyValuePair<string, JsonNode>>();
            jsonProperties.Add(new KeyValuePair<string, JsonNode>("number", new JsonNumber(17)));
            jsonProperties.Add(new KeyValuePair<string, JsonNode>("string", new JsonString("property value")));
            jsonProperties.Add(new KeyValuePair<string, JsonNode>("boolean", new JsonBoolean(true)));

            var jsonObject = new JsonObject(jsonProperties);

            Assert.IsType<JsonNumber>(jsonObject["number"]);
            Assert.Equal(17, (jsonObject["number"] as JsonNumber).GetInt32());

            Assert.IsType<JsonString>(jsonObject["string"]);
            Assert.Equal("property value", (jsonObject["string"] as JsonString).Value);

            Assert.IsType<JsonBoolean>(jsonObject["boolean"]);
            Assert.True((jsonObject["boolean"] as JsonBoolean).Value);
        }

        public enum ExpectedValue
        {
            Previous,
            New
        }

        private static void TestDuplicates(DuplicatePropertyNameHandling duplicatePropertyNameHandling, ExpectedValue expected = default, bool valueConstructor = true)
        {
            string previousValue = "value1";
            string newValue = "value1";

            JsonObject jsonObject = valueConstructor ? new JsonObject(duplicatePropertyNameHandling) : new JsonObject();
            jsonObject.Add("property", new JsonString(previousValue));

            Assert.IsType<JsonString>(jsonObject["property"]);
            Assert.Equal(previousValue, (jsonObject["property"] as JsonString).Value);

            jsonObject.Add("property", new JsonString(newValue));
            Assert.IsType<JsonString>(jsonObject["property"]);

            string expectedString = expected == ExpectedValue.Previous ? previousValue : newValue;

            Assert.Equal(expectedString, (jsonObject["property"] as JsonString).Value);

            // with indexer, property should change no matter which duplicates handling option is chosen:
            jsonObject["property"] = (JsonString)"indexer value";
            Assert.Equal("indexer value", (JsonString)jsonObject["property"]);
        }


        [Theory]
        [InlineData(DuplicatePropertyNameHandling.Replace, ExpectedValue.New)]
        [InlineData(DuplicatePropertyNameHandling.Replace, ExpectedValue.New, false)]
        [InlineData(DuplicatePropertyNameHandling.Ignore, ExpectedValue.Previous)]
        public static void TestDuplicatesReplaceAndIgnore(DuplicatePropertyNameHandling duplicatePropertyNameHandling, ExpectedValue expected = default, bool valueConstructor = true)
        {
            TestDuplicates(duplicatePropertyNameHandling, expected, valueConstructor);
        }

        [Fact]
        public static void TestDuplicatesError()
        {
            Assert.Throws<ArgumentException>(() => TestDuplicates(DuplicatePropertyNameHandling.Error));

            JsonObject jsonObject = new JsonObject(DuplicatePropertyNameHandling.Error) { { "property", "" } };
            jsonObject["property"] = (JsonString) "indexer value";
            Assert.Equal("indexer value", (JsonString) jsonObject["property"]);
        }

        [Fact]
        public static void TestNumerics()
        {
            var jsonObject = new JsonObject();

            jsonObject.Add("byte", byte.MaxValue);
            Assert.IsType<JsonNumber>(jsonObject["byte"]);
            Assert.Equal(byte.MaxValue, ((JsonNumber)jsonObject["byte"]).GetByte());

            jsonObject.Add("short", short.MaxValue);
            Assert.IsType<JsonNumber>(jsonObject["short"]);
            Assert.Equal(short.MaxValue, ((JsonNumber)jsonObject["short"]).GetInt16());

            jsonObject.Add("int", int.MaxValue);
            Assert.IsType<JsonNumber>(jsonObject["int"]);
            Assert.Equal(int.MaxValue, ((JsonNumber)jsonObject["int"]).GetInt32());

            jsonObject.Add("long", long.MaxValue);
            Assert.IsType<JsonNumber>(jsonObject["long"]);
            Assert.Equal(long.MaxValue, ((JsonNumber)jsonObject["long"]).GetInt64());

            jsonObject.Add("float", 3.14f);
            Assert.IsType<JsonNumber>(jsonObject["float"]);
            Assert.Equal(3.14f, ((JsonNumber)jsonObject["float"]).GetSingle());

            jsonObject.Add("double", 3.14);
            Assert.IsType<JsonNumber>(jsonObject["double"]);
            Assert.Equal(3.14, ((JsonNumber)jsonObject["double"]).GetDouble());

            jsonObject.Add("sbyte", sbyte.MaxValue);
            Assert.IsType<JsonNumber>(jsonObject["sbyte"]);
            Assert.Equal(sbyte.MaxValue, ((JsonNumber)jsonObject["sbyte"]).GetSByte());

            jsonObject.Add("ushort", ushort.MaxValue);
            Assert.IsType<JsonNumber>(jsonObject["ushort"]);
            Assert.Equal(ushort.MaxValue, ((JsonNumber)jsonObject["ushort"]).GetUInt16());

            jsonObject.Add("uint", uint.MaxValue);
            Assert.IsType<JsonNumber>(jsonObject["uint"]);
            Assert.Equal(uint.MaxValue, ((JsonNumber)jsonObject["uint"]).GetUInt32());

            jsonObject.Add("ulong", ulong.MaxValue);
            Assert.IsType<JsonNumber>(jsonObject["ulong"]);
            Assert.Equal(ulong.MaxValue, ((JsonNumber)jsonObject["ulong"]).GetUInt64());

            jsonObject.Add("decimal", decimal.One);
            Assert.IsType<JsonNumber>(jsonObject["decimal"]);
            Assert.Equal(decimal.One, ((JsonNumber)jsonObject["decimal"]).GetDecimal());
        }

        [Fact]
        public static void TestCreatingJsonObject()
        {
            var developer = new JsonObject
            {
                { "name", "Kasia" },
                { "age", 22 },
                { "is developer", true },
                { "null property", (JsonNode) null }
            };

            Assert.IsType<JsonString>(developer["name"]);
            var developerNameCasted = developer["name"] as JsonString;
            Assert.Equal("Kasia", developerNameCasted.Value);

            Assert.IsType<JsonNumber>(developer["age"]);
            var developerAgeCasted = developer["age"] as JsonNumber;
            Assert.Equal(22, developerAgeCasted.GetInt32());

            Assert.IsType<JsonBoolean>(developer["is developer"]);
            var isDeveloperCasted = developer["is developer"] as JsonBoolean;
            Assert.True(isDeveloperCasted.Value);

            Assert.Null(developer["null property"]);
        }

        [Fact]
        public static void TestCreatingJsonObjectNewMethods()
        {
            var developer = new JsonObject
            {
                { "name", new JsonString("Kasia") },
                { "age", new JsonNumber(22) },
                { "is developer", new JsonBoolean(true) }
            };

            Assert.IsType<JsonString>(developer["name"]);
            var developerNameCasted = developer["name"] as JsonString;
            Assert.Equal("Kasia", developerNameCasted.Value);

            Assert.IsType<JsonNumber>(developer["age"]);
            var developerAgeCasted = developer["age"] as JsonNumber;
            Assert.Equal(22, developerAgeCasted.GetInt32());

            Assert.IsType<JsonBoolean>(developer["is developer"]);
            var isDeveloperCasted = developer["is developer"] as JsonBoolean;
            Assert.True(isDeveloperCasted.Value);
        }

        [Fact]
        public static void TestCreatingNestedJsonObject()
        {
            var person = new JsonObject
            {
                { "name", "John" },
                { "surname", "Smith" },
                {
                    "phone numbers", new JsonObject()
                    {
                        { "work", "123-456-7890" },
                        { "home", "123-456-7890" }
                    }
                },
                {
                    "addresses", new JsonObject()
                    {
                        {
                            "office", new JsonObject()
                            {
                                {  "address line 1", "One Microsoft Way" },
                                {  "city" , "Redmond" } ,
                                {  "zip code" , 98052 } ,
                                {  "state" , (int) AvailableStateCodes.WA }
                            }
                        },
                        {
                            "home", new JsonObject()
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

            Assert.IsType<JsonObject>(person["phone numbers"]);
            var phoneNumbers = person["phone numbers"] as JsonObject;
            Assert.IsType<JsonString>(phoneNumbers["work"]);
            Assert.IsType<JsonString>(phoneNumbers["home"]);

            Assert.IsType<JsonObject>(person["addresses"]);
            var addresses = person["addresses"] as JsonObject;
            Assert.IsType<JsonObject>(addresses["office"]);
            Assert.IsType<JsonObject>(addresses["home"]);
        }

        [Fact]
        public static void TestAssignmentDefinition()
        {
            JsonNode employee = EmployeesDatabase.GetNextEmployee().Value;
            Assert.IsType<JsonObject>(employee);
        }

        [Fact]
        public static void TestAddingKeyValuePair()
        {
            var employees = new JsonObject
            {
                EmployeesDatabase.GetNextEmployee(),
                EmployeesDatabase.GetNextEmployee(),
                EmployeesDatabase.GetNextEmployee(),
                EmployeesDatabase.GetNextEmployee(),
            };

            string prevId = "";
            foreach (KeyValuePair <string,JsonNode> employee in employees)
            {
                Assert.NotEqual(prevId, employee.Key);
                prevId = employee.Key;

                Assert.IsType<JsonObject>(employee.Value);
            }
        }

        [Fact]
        public static void TestAddingKeyValuePairAfterInitialization()
        {
            var employees = new JsonObject();
            foreach (KeyValuePair<string, JsonNode> employee in EmployeesDatabase.GetTenBestEmployees())
            {
                employees.Add(employee);
            }

            string prevId = "";
            foreach (KeyValuePair<string, JsonNode> employee in employees)
            {
                Assert.NotEqual(prevId, employee.Key);
                prevId = employee.Key;

                Assert.IsType<JsonObject>(employee.Value);
            }
        }

        [Fact]
        public static void TestAddingKeyValuePairsCollection()
        {
            var employees = new JsonObject(EmployeesDatabase.GetTenBestEmployees());

            string prevId = "";
            foreach (KeyValuePair<string, JsonNode> employee in employees)
            {
                Assert.NotEqual(prevId, employee.Key);
                prevId = employee.Key;

                Assert.IsType<JsonObject>(employee.Value);
            }
        }

        [Fact]
        public static void TestAddingKeyValuePairsCollectionAfterInitialization()
        {
            var employees = new JsonObject();
            employees.AddRange(EmployeesDatabase.GetTenBestEmployees());

            string prevId = "";
            foreach (KeyValuePair<string, JsonNode> employee in employees)
            {
                Assert.NotEqual(prevId, employee.Key);
                prevId = employee.Key;

                Assert.IsType<JsonObject>(employee.Value);
            }
        }

        [Fact]
        public static void TestContains()
        {
            var person = new JsonObject
            {
                { "name", "John" },
                { "ssn", "123456789" },
            };

            Assert.True(person.ContainsProperty("ssn"));
            Assert.Equal("123456789", (JsonString)person["ssn"]);
            Assert.False(person.ContainsProperty("surname"));
        }

        [Fact]
        public static void TestAquiringAllValues()
        {
            var employees = new JsonObject(EmployeesDatabase.GetTenBestEmployees());
            ICollection<JsonNode> employeesWithoutId = employees.PropertyValues;

            Assert.Equal(10, employees.PropertyNames.Count);
            Assert.Equal(10, employees.PropertyValues.Count);

            foreach (JsonNode employee in employeesWithoutId)
            {
                Assert.IsType<JsonObject>(employee);
            }
        }

        [Fact]
        public static void TestReplacingsonObjectPrimaryTypes()
        {
            var person1 = new JsonObject
            {
                { "name", "John" },
                { "age", 45 },
                { "is_married", true }
            };

            // Assign by creating a new instance of primary Json type
            person1["name"] = new JsonString("Bob");

            Assert.IsType<JsonString>(person1["name"]);
            Assert.Equal("Bob", person1["name"] as JsonString);

            // Assign by using an implicit operator on primary Json type
            JsonNumber newAge = 55;
            person1["age"] = newAge;

            Assert.IsType<JsonNumber>(person1["age"]);
            Assert.Equal(55, person1["age"] as JsonNumber);

            // Assign by explicit cast from Json primary type
            person1["is_married"] = (JsonBoolean)false;

            Assert.IsType<JsonBoolean>(person1["is_married"]);
            Assert.Equal(false, person1["is_married"] as JsonBoolean);

            var person2 = new JsonObject
            {
                { "name", "Bob" },
                { "age", 33 },
                { "is_married", true }
            };

            // Copy property from another JsonObject
            person1["age"] = person2["age"];

            Assert.IsType<JsonNumber>(person1["age"]);
            Assert.Equal(33, person1["age"] as JsonNumber);

            // Copy property of different typoe
            person1["name"] = person2["name"];
        }

        [Fact]
        public static void TestModifyingJsonObjectKeyRemoveAdd()
        {
            JsonObject manager = EmployeesDatabase.GetManager();
            JsonObject reportingEmployees = manager.GetJsonObjectPropertyValue("reporting employees");

            static void ModifyProperty(JsonObject jsonObject, string previousName, string newName)
            {
                JsonNode previousValue = jsonObject[previousName];
                jsonObject.Remove(previousName);
                jsonObject.Add(newName, previousValue);
            }

            string previousName = "software developers";
            string newName = "software engineers";

            Assert.True(reportingEmployees.ContainsProperty(previousName));
            JsonNode previousValue = reportingEmployees[previousName];

            ModifyProperty(reportingEmployees, previousName, newName);

            Assert.False(reportingEmployees.ContainsProperty(previousName));
            Assert.True(reportingEmployees.ContainsProperty(newName));
            Assert.Equal(previousValue, reportingEmployees[newName]);
        }

        [Fact]
        public static void TestModifyingJsonObjectKeyModifyMethod()
        {
            JsonObject manager = EmployeesDatabase.GetManager();
            JsonObject reportingEmployees = manager.GetJsonObjectPropertyValue("reporting employees");

            Assert.True(reportingEmployees.ContainsProperty("software developers"));
            JsonNode previousValue = reportingEmployees["software developers"];

            reportingEmployees.ModifyPropertyName("software developers", "software engineers");

            Assert.False(reportingEmployees.ContainsProperty("software developers"));
            Assert.True(reportingEmployees.ContainsProperty("software engineers"));
            Assert.Equal(previousValue, reportingEmployees["software engineers"]);
        }

        [Fact]
        public static void TestAccessingNestedJsonObjectCastWithAs()
        {
            JsonObject manager = EmployeesDatabase.GetManager();

            var reportingEmployees = manager["reporting employees"] as JsonObject;
            Assert.NotNull(reportingEmployees);

            var softwareDevelopers = reportingEmployees["software developers"] as JsonObject;
            Assert.NotNull(softwareDevelopers);

            var internDevelopers = softwareDevelopers["intern employees"] as JsonObject;
            Assert.NotNull(internDevelopers);

            internDevelopers.Add(EmployeesDatabase.GetNextEmployee());
        }

        [Fact]
        public static void TestAccessingNestedJsonObjectCastWithIs()
        {
            JsonObject manager = EmployeesDatabase.GetManager();

            static bool AddEmployee(JsonObject manager)
            {

                if (manager["reporting employees"] is JsonObject reportingEmployees)
                {
                    if (reportingEmployees["software developers"] is JsonObject softwareDevelopers)
                    {
                        if (softwareDevelopers["full time employees"] is JsonObject fullTimeEmployees)
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
        public static void TestAccessingNestedJsonObjectExplicitCast()
        {
            JsonObject manager = EmployeesDatabase.GetManager();

            // Should not throw any exceptions:
            ((JsonObject)((JsonObject)manager["reporting employees"])["HR"]).Add(EmployeesDatabase.GetNextEmployee());
        }

        [Fact]
        public static void TestAccessingNestedJsonObjectGetPropertyMethod()
        {
            JsonObject manager = EmployeesDatabase.GetManager();

            // Should not throw any exceptions:

            JsonObject internDevelopers = manager.GetJsonObjectPropertyValue("reporting employees")
                                          .GetJsonObjectPropertyValue("software developers")
                                          .GetJsonObjectPropertyValue("intern employees");
            internDevelopers.Add(EmployeesDatabase.GetNextEmployee());
        }

        [Fact]
        public static void TestAccessingNestedJsonObjectTryGetPropertyMethod()
        {
            JsonObject manager = EmployeesDatabase.GetManager();

            static bool AddEmployee(JsonObject manager)
            {
                if (manager.TryGetJsonObjectPropertyValue("reporting employees", out JsonObject reportingEmployees))
                {
                    if (reportingEmployees.TryGetJsonObjectPropertyValue("software developers", out JsonObject softwareDevelopers))
                    {
                        if (softwareDevelopers.TryGetJsonObjectPropertyValue("full time employees", out JsonObject fullTimeEmployees))
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
        public static void TestModifyPropertyNameThrows()
        {
            Assert.Throws<KeyNotFoundException>(() => new JsonObject().ModifyPropertyName("", ""));
            Assert.Throws<ArgumentException>(() =>
            {
                var jsonObject = new JsonObject()
                {
                    { "oldName", "value" },
                    { "newName", "value" }
                };

                jsonObject.ModifyPropertyName("oldName", "newName");
            });
        }

        [Fact]
        public static void TestGetPropertyThrows()
        {
            Assert.Throws<KeyNotFoundException>(() =>
            {
                var jsonObject = new JsonObject()
                {
                    { "name", "value" }
                };

                jsonObject.GetPropertyValue("different name");
            });
        }

        [Fact]
        public static void TestTryGetProperty ()
        {
            var jsonObject = new JsonObject()
            {
                { "name", "value" }
            };

            Assert.True(jsonObject.TryGetPropertyValue("name", out JsonNode property));
            Assert.IsType<JsonString>(property);
            Assert.Equal("value", property as JsonString);
            Assert.False(jsonObject.TryGetPropertyValue("other", out property));
            Assert.Null(property);
        }

        [Fact]
        public static void TestGetJsonObjectPropertyThrows()
        {
            Assert.Throws<InvalidCastException>(() =>
            {
                var jsonObject = new JsonObject()
                {
                    { "name", "value" }
                };

                jsonObject.GetJsonObjectPropertyValue("name");
            });
        }

        [Fact]
        public static void TestTryGetObjectPropertyFails()
        {
            var jsonObject = new JsonObject()
            {
                { "name", "value" }
            };

            Assert.False(jsonObject.TryGetJsonObjectPropertyValue("name", out JsonObject property));
            Assert.Null(property);

            Assert.False(jsonObject.TryGetJsonObjectPropertyValue("other", out property));
            Assert.Null(property);
        }

        [Fact]
        public static void TestArgumentNullValidation()
        {
            Assert.Throws<ArgumentNullException>(() => new JsonObject().Add(null, ""));
            Assert.Throws<ArgumentNullException>(() => new JsonObject().Add(null, new JsonObject()));
            Assert.Throws<ArgumentNullException>(() => new JsonObject().Add(null, (byte)17));
            Assert.Throws<ArgumentNullException>(() => new JsonObject().Add(null, (short)17));
            Assert.Throws<ArgumentNullException>(() => new JsonObject().Add(null, 17));
            Assert.Throws<ArgumentNullException>(() => new JsonObject().Add(null, (long)17));
            Assert.Throws<ArgumentNullException>(() => new JsonObject().Add(null, 3.14f));
            Assert.Throws<ArgumentNullException>(() => new JsonObject().Add(null, 3.14));
            Assert.Throws<ArgumentNullException>(() => new JsonObject().Add(null, decimal.One));
            Assert.Throws<ArgumentNullException>(() => new JsonObject().Add(null, (sbyte)17));
            Assert.Throws<ArgumentNullException>(() => new JsonObject().Add(null, (ushort)17));
            Assert.Throws<ArgumentNullException>(() => new JsonObject().Add(null, (uint)17));
            Assert.Throws<ArgumentNullException>(() => new JsonObject().Add(null, (ulong)17));
            Assert.Throws<ArgumentNullException>(() => new JsonObject().Add(new KeyValuePair<string, JsonNode>(null, new JsonObject())));
            Assert.Throws<ArgumentNullException>(() =>
            {
                var property = new KeyValuePair<string, JsonNode>(null, new JsonObject());
                new JsonObject().AddRange(new List<KeyValuePair<string, JsonNode>>() { property });
            });
            Assert.Throws<ArgumentNullException>(() => new JsonObject()[null] = new JsonString());
            Assert.Throws<ArgumentNullException>(() =>
            {
                var jsonObject = new JsonObject();
                JsonNode x = jsonObject[null];
            });
            Assert.Throws<ArgumentNullException>(() => new JsonObject().Remove(null));
            Assert.Throws<ArgumentNullException>(() => new JsonObject().ContainsProperty(null));
        }

        [Fact]
        public static void TestDuplicatesEnumOutOfRange()
        {
            Assert.Throws<ArgumentException>(() => new JsonObject((DuplicatePropertyNameHandling)123));
        }
    }
}
