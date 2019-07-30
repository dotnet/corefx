// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace System.Text.Json
{
    public static partial class WritableJsonApiTests
    {
        /// <summary>
        /// Transforming JsoneNode to JsonElement
        /// </summary>
        [Fact]
        public static void TestTransformingJsonNodeToJsonElement()
        {
            // Send Json through network, should not throw any exceptions:
            JsonNode employeeDataToSend = EmployeesDatabase.GetNextEmployee().Value;
            Mailbox.SendEmployeeData(employeeDataToSend.AsJsonElement());
        }

        /// <summary>
        /// Transforming JsonElement to JsonNode
        /// </summary>
        [Fact]
        public static void TestTransformingJsonElementToJsonNode()
        {
            // Retrieve Json from network, should not throw any exceptions:
            JsonNode receivedEmployeeData = JsonNode.DeepCopy(Mailbox.RetrieveMutableEmployeeData());
            if (receivedEmployeeData is JsonObject employee)
            {
                employee["name"] = new JsonString("Bob");
                Mailbox.SendEmployeeData(employee.AsJsonElement());
            }
        }

        /// <summary>
        /// Transforming JsonDocument to JsonNode and vice versa
        /// </summary>
        [Fact]
        public static void TestTransformingToFromJsonDocument()
        {
            string jsonString = @"
            {
                ""employee1"" : 
                {
                    ""name"" : ""Ann"",
                    ""surname"" : ""Predictable"",
                    ""age"" : 30,                
                },
                ""employee2"" : 
                {
                    ""name"" : ""Zoe"",
                    ""surname"" : ""Coder"",
                    ""age"" : 24,                
                }
            }";

            using (JsonDocument employeesToSend = JsonDocument.Parse(jsonString))
            {
                // regular send:
                Mailbox.SendAllEmployeesData(employeesToSend.RootElement);

                // modified elements send:
                JsonObject employees = JsonNode.DeepCopy(employeesToSend) as JsonObject;
                Assert.Equal(2, employees.Count());

                employees.Add(EmployeesDatabase.GetNextEmployee());
                Assert.Equal(3, employees.Count());

                Mailbox.SendAllEmployeesData(employees.AsJsonElement());
            }
        }

        /// <summary>
        /// Parsing right to JsonNode if user knows data will be modified
        /// </summary>
        [Fact]
        public static void TestParsingToJsonNode()
        {
            string jsonString = @"
            {
                ""employee1"" : 
                {
                    ""name"" : ""Ann"",
                    ""surname"" : ""Predictable"",
                    ""age"" : 30,                
                },
                ""employee2"" : 
                {
                    ""name"" : ""Zoe"",
                    ""surname"" : ""Coder"",
                    ""age"" : 24,                
                }
            }";

            JsonObject employees = JsonNode.Parse(jsonString) as JsonObject;
            employees.Add(EmployeesDatabase.GetNextEmployee());
            Mailbox.SendAllEmployeesData(employees.AsJsonElement());

            Assert.Equal(2, employees.Count());
            Assert.True(employees.ContainsProperty("employee1"));
            Assert.True(employees.ContainsProperty("employee2"));

            JsonObject employee2 = employees.GetJsonObjectProperty("employee2");
            Assert.IsType<JsonString>(employee2["name"]);
            Assert.Equal("Zoe", (JsonString)employee2["name"]);
            Assert.IsType<JsonString>(employee2["surname"]);
            Assert.Equal("Coder", (JsonString)employee2["surname"]);
            Assert.IsType<JsonNumber>(employee2["age"]);
            Assert.Equal(24, (JsonNumber)employee2["age"]);
        }

        /// <summary>
        /// Copying JsoneNode
        /// </summary>
        [Fact]
        public static void TestCopyingJsonNode()
        {
            JsonObject employee = EmployeesDatabase.GetManager();
            JsonNode employeeCopy = JsonNode.DeepCopy(employee);

            static bool RecursiveEquals(JsonNode left, JsonNode right)
            {
                if (left == null && right == null)
                {
                    return true;
                }

                if (right.GetType() != left.GetType())
                {
                    return false;
                }

                switch (left)
                {
                    case JsonObject leftJsonObject:
                        var rightJsonObject = right as JsonObject;
                        if (leftJsonObject.Count() != rightJsonObject.Count())
                        {
                            return false;
                        }

                        for (int idx = 0; idx < leftJsonObject.Count(); idx++)
                        {
                            KeyValuePair<string, JsonNode> leftElement = leftJsonObject.ElementAt(idx);
                            KeyValuePair<string, JsonNode> rightElement = rightJsonObject.ElementAt(idx);

                            if (leftElement.Key != rightElement.Key || !RecursiveEquals(leftElement.Value, rightElement.Value))
                            {
                                return false;
                            }
                        }

                        return true;
                    case JsonArray leftJsonArray:
                        var rightJsonArray = right as JsonArray;
                        if (leftJsonArray.Count() != rightJsonArray.Count())
                        {
                            return false;
                        }
                        for (int idx = 0; idx < leftJsonArray.Count(); idx++)
                        {
                            JsonNode leftElement = leftJsonArray.ElementAt(idx);
                            JsonNode rightElement = rightJsonArray.ElementAt(idx);

                            if (!RecursiveEquals(leftElement, rightElement))
                            {
                                return false;
                            }
                        }

                        return true;
                    case JsonString leftJsonString:
                        return leftJsonString.Equals(right as JsonString);
                    case JsonNumber leftJsonNumber:
                        return leftJsonNumber.Equals(right as JsonNumber);
                    case JsonBoolean leftJsonBoolean:
                        return leftJsonBoolean.Equals(right as JsonBoolean);
                    default:
                        return false;
                }
            }

            Assert.True(RecursiveEquals(employee, employeeCopy));
        }

        /// <summary>
        /// Checking IsImmutable property
        /// </summary>
        [Fact]
        public static void TestIsImmutable()
        {
            JsonElement employee = Mailbox.RetrieveMutableEmployeeData();
            Assert.False(employee.IsImmutable);

            if (!employee.IsImmutable)
            {
                JsonObject employeeNode = JsonNode.GetNode(employee) as JsonObject;
                employeeNode["received as node"] = (JsonBoolean)true;
            }

            employee = Mailbox.RetrieveImmutableEmployeeData();
            Assert.True(employee.IsImmutable);
        }

        /// <summary>
        /// Retrieving JsonNode from JsonElement
        /// </summary>
        [Fact]
        public static void TestTryGetNode()
        {
            JsonElement employee = Mailbox.RetrieveMutableEmployeeData();

            static bool CheckIfReceivedAsNode(JsonElement employee)
            {
                if (JsonNode.TryGetNode(employee, out JsonNode employeeNode))
                {
                    ((JsonObject)employeeNode)["received as node"] = (JsonBoolean)true;
                    return true;
                }
                return false;
            }

            Assert.True(CheckIfReceivedAsNode(employee));
        }
    }
}
