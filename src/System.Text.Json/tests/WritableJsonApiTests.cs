// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using Xunit;


namespace System.Text.Json
{
#pragma warning disable xUnit1000
    internal static class WritableJsonApiTests
#pragma warning enable xUnit1000
    {
        /// <summary>
        /// Helper class simulating external library
        /// </summary>
        private static class EmployeesDatabase
        {
            private static int s_id = 0;
            public static KeyValuePair<string, JsonNode> GetNextEmployee()
            {
                return new KeyValuePair<string, JsonNode>("employee" + s_id++, new JsonObject());
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
                return new JsonObject
                {
                    { "name", "John" },
                    {
                        "phone numbers", new JsonObject()
                        {
                            { "work", "123-456-7890" }, { "home", "123-456-7890" }
                        }
                    },
                    {
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
                    }
                };
            }

            public static bool CheckSSN(string ssnNumber) => true;
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

        /// <summary>
        /// Creating simple Json object
        /// </summary>
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
        }

        /// <summary>
        /// Creating simple Json object by new methods on primary types
        /// </summary>
        [Fact]
        public static void TestCreatingJsonObjectNewMethods()
        {
            var developer = new JsonObject
            {
                { "name", new JsonString("Kasia") },
                { "age", new JsonNumber(22) },
                { "is developer", new JsonBoolean(true) }
            };
        }

        /// <summary>
        /// Creating and retriving different numeric values
        /// </summary>
        [Fact]
        public static void TestNumerics()
        {
            double PI = 3.14159265359;
            var circle = new JsonObject
            {
                { "radius", 1 },
                { "length", 2*PI },
                { "area", PI }
            };

            JsonNumber bigConstantBoxed = Int64.MaxValue;
            long bigConstant = bigConstantBoxed.GetInt64();

            var smallValueBoxed = new JsonNumber(17);
            smallValueBoxed.TryGetInt16(out short smallValue);
        }

        /// <summary>
        /// Creating nested Json object
        /// </summary>
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
        }

        /// <summary>
        /// Defining as KeyValuePair value
        /// </summary>
        [Fact]
        public static void TestAssignmentDefinition()
        {
            JsonNode employee = EmployeesDatabase.GetNextEmployee().Value;
        }

        /// <summary>
        /// Adding KeyValuePair from external library
        /// </summary>
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
        }

        /// <summary>
        /// Adding KeyValuePair from external library after initialization
        /// </summary>
        [Fact]
        public static void TestAddingKeyValuePairAfterInitialization()
        {
            var employees = new JsonObject();
            foreach (KeyValuePair<string, JsonNode> employee in EmployeesDatabase.GetTenBestEmployees())
            {
                employees.Add(employee);
            }
        }

        /// <summary>
        /// Adding KeyValuePairs collection from external library
        /// </summary>
        [Fact]
        public static void TestAddingKeyValuePairsCollection()
        {
            var employees = new JsonObject(EmployeesDatabase.GetTenBestEmployees());
        }

        /// <summary>
        /// Adding KeyValuePairs collection from external library after initialization
        /// </summary>
        [Fact]
        public static void TestAddingKeyValuePairsCollectionAfterInitialization()
        {
            var employees = new JsonObject();
            employees.AddRange(EmployeesDatabase.GetTenBestEmployees());
        }

        /// <summary>
        /// Creating Json array
        /// </summary>
        [Fact]
        public static void TestCreatingJsonArray()
        {
            string[] dishes = { "sushi", "pasta", "cucumber soup" };

            var sportsExperienceYears = new JsonObject()
            {
                { "skiing", 5 },
                { "cycling", 8 },
                { "hiking", 6 },
                { "chess", 2 },
                { "skating", 1 },
            };

            // choose only sports with > 2 experience years
            IEnumerable<string> sports = sportsExperienceYears.Where(sport => ((JsonNumber)sport.Value).GetInt32() > 2).Select(sport => sport.Key);

            var strangeWords = new JsonArray()
            {
                "supercalifragilisticexpialidocious",
                "gladiolus",
                "albumen",
                "smaragdine"
            };

            var preferences = new JsonObject()
            {
                { "colours", new JsonArray { "red", "green", "purple" } },
                { "numbers", new JsonArray { 4, 123, 88 } },
                { "prime numbers", new JsonNumber[] { 19, 37 } },
                { "varia", new JsonArray { 17, "green", true } },
                { "dishes", new JsonArray(dishes) },
                { "sports", new JsonArray(sports) },
                { "strange words", strangeWords.Where(word => ((JsonString)word).Value.Length < 10) },
            };
        }

        /// <summary>
        /// Creating nested Json array
        /// </summary>
        [Fact]
        public static void TestCreatingNestedJsonArray()
        {
            var vertices = new JsonArray()
            {
                new JsonArray
                {
                    new JsonArray
                    {
                        new JsonArray { 0, 0, 0 },
                        new JsonArray { 0, 0, 1 }
                    },
                    new JsonArray
                    {
                        new JsonArray { 0, 1, 0 },
                        new JsonArray { 0, 1, 1 }
                    }
                },
                new JsonArray
                {
                    new JsonArray
                    {
                        new JsonArray { 1, 0, 0 },
                        new JsonArray { 1, 0, 1 }
                    },
                    new JsonArray
                    {
                        new JsonArray { 1, 1, 0 },
                        new JsonArray { 1, 1, 1 }
                    }
                },
            };
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
        }

        /// <summary>
        /// Creating Json array from collection
        /// </summary>
        [Fact]
        public static void TestCreatingJsonArrayFromCollection()
        {
            var employeesIds = new JsonArray(EmployeesDatabase.GetTenBestEmployees().Select(employee => new JsonString(employee.Key)));
        }

        /// <summary>
        /// Creating Json array from collection of strings
        /// </summary>
        [Fact]
        public static void TestCreatingJsonArrayFromCollectionOfString()
        {
            var employeesIds = new JsonArray(EmployeesDatabase.GetTenBestEmployees().Select(employee => employee.Key));
        }

        /// <summary>
        /// Contains checks
        /// </summary>
        [Fact]
        public static void TestContains()
        {
            var person = new JsonObject
            {
                { "name", "John" },
                { "ssn", "123456789" },
            };

            if (person.ContainsProperty("ssn"))
            {
                EmployeesDatabase.CheckSSN(((JsonString)person["ssn"]).Value);
            }

            var enabledOptions = new JsonArray
            {
                "readonly",
                "no cache",
                "continue on failure"
            };

            if (enabledOptions.Contains("no cache"))
            {
                // do sth without using caching
            }

            var requiredOptions = new JsonArray
            {
                "readonly",
                "continue on failure"
            };

            // if all required options are enabled
            if (!requiredOptions.Select(option => !enabledOptions.Contains(option)).Any())
            {
                // do sth without using caching
            }
        }

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

            // Assign by using an implicit operator on primary Json type
            JsonNumber newAge = 55;
            person1["age"] = newAge;

            // Assign by explicit cast from Json primary type
            person1["is_married"] = (JsonBoolean)true;

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

            // Copy property of different typoe
            person1["name"] = person2["name"];         
        }

        /// <summary>
        /// Modifying Json object's primnary types
        /// </summary>
        [Fact]
        public static void TestModifyingJsonObjectPrimaryTypes()
        {
            JsonString name = "previous name";
            name.Value = "new name";

            bool shouldBeEnabled = true;
            var isEnabled = new JsonBoolean(false);
            isEnabled.Value = shouldBeEnabled;

            JsonNumber veryBigConstant = new JsonNumber();
            veryBigConstant.SetString("1e1000");
            string bigNumber = veryBigConstant.GetString();
            veryBigConstant.SetInt16(123);
            short smallNumber = veryBigConstant.GetInt16();
        }

        /// <summary>
        /// Accesing nested Json object - casting with as operator
        /// </summary>
        [Fact]
        public static void TestAccesingNestedJsonObjectCastWithAs()
        {
            // Casting with as operator
            JsonObject manager = EmployeesDatabase.GetManager();

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

            if (manager["reporting employees"] is JsonObject reportingEmployees)
            {
                if (reportingEmployees["software developers"] is JsonObject softwareDevelopers)
                {
                    if (softwareDevelopers["full time employees"] is JsonObject fullTimeEmployees)
                    {
                        fullTimeEmployees.Add(EmployeesDatabase.GetNextEmployee());
                    }
                }
            }
        }

        /// <summary>
        /// Accesing nested Json object - explicit casting
        /// </summary>
        [Fact]
        public static void TestAccesingNestedJsonObjectExplicitCast()
        {
            JsonObject manager = EmployeesDatabase.GetManager();

            ((JsonObject)((JsonObject)manager["reporting employees"])["HR"]).Add(EmployeesDatabase.GetNextEmployee());
        }

        /// <summary>
        /// Accesing nested Json object - GetObjectProperty method
        /// </summary>
        [Fact]
        public static void TestAccesingNestedJsonObjectGetPropertyMethod()
        {
            JsonObject manager = EmployeesDatabase.GetManager();
            JsonObject internDevelopers = manager.GetObjectProperty("reporting employees")
                                          .GetObjectProperty("software developers")
                                          .GetObjectProperty("intern employees");
            internDevelopers.Add(EmployeesDatabase.GetNextEmployee());
        }

        /// <summary>
        /// Accesing nested Json object - TryGetObjectProperty method
        /// </summary>
        [Fact]
        public static void TestAccesingNestedJsonObjectTryGetPropertyMethod()
        {
            JsonObject manager = EmployeesDatabase.GetManager();
            if (manager.TryGetObjectProperty("reporting employees", out JsonObject reportingEmployees))
            {
                if (reportingEmployees.TryGetObjectProperty("software developers", out JsonObject softwareDevelopers))
                {
                    if (softwareDevelopers.TryGetObjectProperty("full time employees", out JsonObject fullTimeEmployees))
                    {
                        fullTimeEmployees.Add(EmployeesDatabase.GetNextEmployee());
                    }
                }
            }
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

            issues.GetArrayProperty("bugs").Add("bug 12356");
            ((JsonString)issues.GetArrayProperty("features")[0]).Value = "feature 1569";
            ((JsonString)issues.GetArrayProperty("features")[1]).Value = "feature 56134";
        }

        /// <summary>
        /// Modifying Json object key - remove and add
        /// </summary>
        [Fact]
        public static void TestModifyingJsonObjectKeyRemoveAdd()
        {
            JsonObject manager = EmployeesDatabase.GetManager();
            JsonObject reportingEmployees = manager.GetObjectProperty("reporting employees");

            JsonNode softwareDevelopers = reportingEmployees["software developers"];
            reportingEmployees.Remove("software developers");
            reportingEmployees.Add("software engineers", softwareDevelopers);
        }

        /// <summary>
        /// Modifying Json object key - modify method
        /// </summary>
        [Fact]
        public static void TestModifyingJsonObjectKeyModifyMethod()
        {
            JsonObject manager = EmployeesDatabase.GetManager();
            JsonObject reportingEmployees = manager.GetObjectProperty("reporting employees");

            reportingEmployees.ModifyPropertyName("software developers", "software engineers");
        }

        /// <summary>
        /// Aquiring all values
        /// </summary>
        [Fact]
        public static void TestAquiringAllValues()
        {
            var employees = new JsonObject(EmployeesDatabase.GetTenBestEmployees());
            ICollection<JsonNode> employeesWithoutId = employees.Values;
        }

        /// <summary>
        /// Aquiring all properties
        /// </summary>
        [Fact]
        public static void TestAquiringAllProperties()
        {
            var employees = new JsonObject()
            {
                { "FTE", "John Smith" },
                { "FTE", "Ann Predictable" },
                { "Intern", "Zoe Coder" },
                { "FTE", "Byron Shadow" },
            };

            IEnumerable<JsonNode> fullTimeEmployees = employees.GetAllProperties("FTE");
        }
    }
}
