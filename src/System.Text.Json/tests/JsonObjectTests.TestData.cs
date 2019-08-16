﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Text.Json.Tests
{
    public static partial class JsonObjectTests
    {
        /// <summary>
        /// Helper class simulating external library
        /// </summary>
        private static class EmployeesDatabase
        {
            private static int s_id = 0;
            public static KeyValuePair<string, JsonNode> GetNextEmployee()
            {
                var employee = new JsonObject()
                {
                    { "name", "John" } ,
                    { "surname", "Smith"},
                    { "age", 45 }
                };

                return new KeyValuePair<string, JsonNode>("employee" + s_id++, employee);
            }

            public static IEnumerable<KeyValuePair<string, JsonNode>> GetTenBestEmployees()
            {
                for (int i = 0; i < 10; i++)
                    yield return GetNextEmployee();
            }

            /// <summary>
            /// Returns following JsonObject:
            /// {
            ///     { "name" : "John" }
            ///     { "phone numbers" : { "work" :  "123-456-7890", "home": "123-456-7890"  } }
            ///     { 
            ///         "reporting employees" : 
            ///         {
            ///             "software developers" :
            ///             {
            ///                 "full time employees" : /JsonObject of 3 employees fromk database/ 
            ///                 "intern employees" : /JsonObject of 2 employees fromk database/ 
            ///             },
            ///             "HR" : /JsonObject of 10 employees fromk database/ 
            ///         }
            /// </summary>
            /// <returns></returns>
            public static JsonObject GetManager()
            {
                var manager = GetNextEmployee().Value as JsonObject;

                manager.Add
                (
                    "phone numbers",
                    new JsonObject()
                    {
                        { "work", "123-456-7890" }, { "home", "123-456-7890" }
                    }
                );

                manager.Add
                (
                    "reporting employees", new JsonObject()
                    {
                        {
                            "software developers", new JsonObject()
                            {
                                {
                                    "full time employees", new JsonObject()
                                    {
                                        EmployeesDatabase.GetNextEmployee(),
                                        EmployeesDatabase.GetNextEmployee(),
                                        EmployeesDatabase.GetNextEmployee(),
                                    }
                                },
                                {
                                    "intern employees", new JsonObject()
                                    {
                                        EmployeesDatabase.GetNextEmployee(),
                                        EmployeesDatabase.GetNextEmployee(),
                                    }
                                }
                            }
                        },
                        {
                            "HR", new JsonObject()
                            {
                                {
                                    "full time employees", new JsonObject(EmployeesDatabase.GetTenBestEmployees())
                                }
                            }
                        }
                    }
                );

                return manager;
            }
            public static void PerformHeavyOperations(JsonElement employee) { }
        }

        private static class HealthCare
        {
            public static void CreateMedicalAppointment(string personName) { }
        }

        /// <summary>
        /// Helper class simulating enum
        /// </summary>
        private enum AvailableStateCodes
        {
            WA,
            CA,
            NY,
        }
    }
}
