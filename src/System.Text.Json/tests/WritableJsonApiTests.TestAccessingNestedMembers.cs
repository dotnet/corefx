// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Text.Json
{
#pragma warning disable xUnit1000
    internal static partial class WritableJsonApiTests
#pragma warning enable xUnit1000
    {
        /// <summary>
        /// Accesing nested Json object - casting with as operator
        /// </summary>
        [Fact]
        public static void TestAccesingNestedJsonObjectCastWithAs()
        {
            JsonObject manager = EmployeesDatabase.GetManager();

            // Should not throw any exceptions:

            var reportingEmployees = manager["reporting employees"] as JsonObject;
            if (reportingEmployees == null)
                throw new InvalidCastException();

            var softwareDevelopers = reportingEmployees["software developers"] as JsonObject;
            if (softwareDevelopers == null)
                throw new InvalidCastException();

            var internDevelopers = softwareDevelopers["intern employees"] as JsonObject;
            if (internDevelopers == null)
                throw new InvalidCastException();

            internDevelopers.Add(EmployeesDatabase.GetNextEmployee());
        }

        /// <summary>
        /// Accesing nested Json object - casting with is operator
        /// </summary>
        [Fact]
        public static void TestAccesingNestedJsonObjectCastWithIs()
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

        /// <summary>
        /// Accesing nested Json object - explicit casting
        /// </summary>
        [Fact]
        public static void TestAccesingNestedJsonObjectExplicitCast()
        {
            JsonObject manager = EmployeesDatabase.GetManager();

            // Should not throw any exceptions:
            ((JsonObject)((JsonObject)manager["reporting employees"])["HR"]).Add(EmployeesDatabase.GetNextEmployee());
        }

        /// <summary>
        /// Accesing nested Json object - GetObjectProperty method
        /// </summary>
        [Fact]
        public static void TestAccesingNestedJsonObjectGetPropertyMethod()
        {
            JsonObject manager = EmployeesDatabase.GetManager();

            // Should not throw any exceptions:

            JsonObject internDevelopers = manager.GetJsonObjectProperty("reporting employees")
                                          .GetJsonObjectProperty("software developers")
                                          .GetJsonObjectProperty("intern employees");
            internDevelopers.Add(EmployeesDatabase.GetNextEmployee());
        }

        /// <summary>
        /// Accesing nested Json object - TryGetObjectProperty method
        /// </summary>
        [Fact]
        public static void TestAccesingNestedJsonObjectTryGetPropertyMethod()
        {
            JsonObject manager = EmployeesDatabase.GetManager();

            static bool AddEmployee(JsonObject manager)
            {
                if (manager.TryGetObjectProperty("reporting employees", out JsonObject reportingEmployees))
                {
                    if (reportingEmployees.TryGetObjectProperty("software developers", out JsonObject softwareDevelopers))
                    {
                        if (softwareDevelopers.TryGetObjectProperty("full time employees", out JsonObject fullTimeEmployees))
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

        /// <summary>
        /// Accesing nested Json array - GetArrayProperty method
        /// </summary>
        [Fact]
        public static void TestAccesingNestedJsonArrayGetPropertyMethod()
        {
            var issues = new JsonObject()
            {
                { "features", new JsonArray{ "new functionality 1", "new functionality 2" } },
                { "bugs", new JsonArray{ "bug 123", "bug 4566", "bug 821" } },
                { "tests", new JsonArray{ "code coverage" } },
            };

            issues.GetJsonArrayProperty("bugs").Add("bug 12356");
            ((JsonString)issues.GetJsonArrayProperty("features")[0]).Value = "feature 1569";
            ((JsonString)issues.GetJsonArrayProperty("features")[1]).Value = "feature 56134";

            Assert.True(((JsonArray)issues["bugs"]).Contains("bug 12356"));
            Assert.Equal((JsonString)((JsonArray)issues["features"])[0], "feature 1569");
            Assert.Equal((JsonString)((JsonArray)issues["features"])[1], "feature 56134");
        }
    }
}
