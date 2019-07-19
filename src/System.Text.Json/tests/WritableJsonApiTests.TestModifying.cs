// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Text.Json
{
#pragma warning disable xUnit1000
    internal static partial class WritableJsonApiTests
#pragma warning enable xUnit1000
    {
        /// <summary>
        /// Replacing Json object's primnary types
        /// </summary>
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
            Assert.Equal(person1["name"] as JsonString, "Bob");

            // Assign by using an implicit operator on primary Json type
            JsonNumber newAge = 55;
            person1["age"] = newAge;

            Assert.IsType<JsonNumber>(person1["age"]);
            Assert.Equal(person1["age"] as JsonNumber, 55);

            // Assign by explicit cast from Json primary type
            person1["is_married"] = (JsonBoolean)false;

            Assert.IsType<JsonBoolean>(person1["is_married"]);
            Assert.Equal(person1["is_married"] as JsonBoolean, false);

            // Not possible scenario (wold require implicit cast operators in JsonNode):
            // person["name"] = "Bob";

            var person2 = new JsonObject
            {
                { "name", new JsonString[]{ "Emily", "Rosalie" } },
                { "age", 33 },
                { "is_married", true }
            };

            // Copy property from another JsonObject
            person1["age"] = person2["age"];

            Assert.IsType<JsonNumber>(person1["age"]);
            Assert.Equal(person1["age"] as JsonNumber, 33);

            // Copy property of different typoe
            person1["name"] = person2["name"];

            Assert.IsType<JsonArray>(person1["name"]);
        }

        /// <summary>
        /// Modifying Json object's primnary types
        /// </summary>
        [Fact]
        public static void TestModifyingJsonObjectPrimaryTypes()
        {
            JsonString name = "previous name";
            name.Value = "new name";

            Assert.Equal("new name", name.Value);

            bool shouldBeEnabled = true;
            var isEnabled = new JsonBoolean(false);
            isEnabled.Value = shouldBeEnabled;

            Assert.True(isEnabled.Value);

            JsonNumber veryBigConstant = new JsonNumber();
            veryBigConstant.SetString("1e1000");
            string bigNumber = veryBigConstant.GetString();

            Assert.Equal("1e1000", bigNumber);

            veryBigConstant.SetInt16(123);
            short smallNumber = veryBigConstant.GetInt16();

            Assert.Equal(123, smallNumber);

            // Incrementing age:
            JsonObject employee = EmployeesDatabase.GetManager();
            int age = ((JsonNumber)employee["age"]).GetInt32();
            ((JsonNumber)employee["age"]).SetInt32(age + 1);

            Assert.Equal(46, ((JsonNumber)employee["age"]).GetInt32());
        }

        /// <summary>
        /// Adding values to JsonArray
        /// </summary>
        [Fact]
        public static void TestAddingToJsonArray()
        {
            var employeesIds = new JsonArray();

            foreach (KeyValuePair<string, JsonNode> employee in EmployeesDatabase.GetTenBestEmployees())
            {
                employeesIds.Add(employee.Key);
            }

            JsonString prevId = new JsonString();
            foreach (JsonNode employeeId in employeesIds)
            {
                Assert.IsType<JsonString>(employeeId);
                var employeeIdString = employeeId as JsonString;
                Assert.NotEqual(prevId, employeeIdString);
                prevId = employeeIdString;
            }
        }

        /// <summary>
        /// Modifying Json object key - remove and add
        /// </summary>
        [Fact]
        public static void TestModifyingJsonObjectKeyRemoveAdd()
        {
            JsonObject manager = EmployeesDatabase.GetManager();
            JsonObject reportingEmployees = manager.GetJsonObjectProperty("reporting employees");

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

        /// <summary>
        /// Modifying Json object key - modify method
        /// </summary>
        [Fact]
        public static void TestModifyingJsonObjectKeyModifyMethod()
        {
            JsonObject manager = EmployeesDatabase.GetManager();
            JsonObject reportingEmployees = manager.GetJsonObjectProperty("reporting employees");

            Assert.True(reportingEmployees.ContainsProperty("software developers"));
            JsonNode previousValue = reportingEmployees["software engineers"];

            reportingEmployees.ModifyPropertyName("software developers", "software engineers");

            Assert.False(reportingEmployees.ContainsProperty("software developers"));
            Assert.True(reportingEmployees.ContainsProperty("software engineers"));
            Assert.Equal(previousValue, reportingEmployees["software engineers"]);
        }
    }
}
