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

            Assert.IsType<JsonNumber>(circle["radius"]);
            var radius = circle["radius"] as JsonNumber;
            Assert.Equal(1, radius);

            Assert.IsType<JsonNumber>(circle["length"]);
            var length = circle["length"] as JsonNumber;
            Assert.Equal(length, 2 * PI);

            Assert.IsType<JsonNumber>(circle["area"]);
            var area = circle["area"] as JsonNumber;
            Assert.Equal(area, PI);

            JsonNumber bigConstantBoxed = long.MaxValue;
            long bigConstant = bigConstantBoxed.GetInt64();

            Assert.Equal(long.MaxValue, bigConstant);

            var smallValueBoxed = new JsonNumber(17);
            smallValueBoxed.TryGetInt16(out short smallValue);

            Assert.Equal(17, smallValue);
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

            Assert.IsType<JsonObject>(person["phone numbers"]);
            var phoneNumbers = person["phone numbers"] as JsonObject;
            Assert.IsType<JsonString>(phoneNumbers["work"]);
            Assert.IsType<JsonString>(phoneNumbers["home"]);

            Assert.IsType<JsonObject>(person["addresses"]);
            var addresses = person["office"] as JsonObject;
            Assert.IsType<JsonObject>(addresses["office"]);
            Assert.IsType<JsonObject>(addresses["home"]);
        }

        /// <summary>
        /// Defining as KeyValuePair value
        /// </summary>
        [Fact]
        public static void TestAssignmentDefinition()
        {
            JsonNode employee = EmployeesDatabase.GetNextEmployee().Value;
            Assert.IsType<JsonObject>(employee);
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

            string prevId = "";
            foreach((string id, JsonNode employee) in employees)
            {
                Assert.NotEqual(prevId, id);
                prevId = id;

                Assert.IsType<JsonObject>(employee);
            }
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

            string prevId = "";
            foreach ((string id, JsonNode employee) in employees)
            {
                Assert.NotEqual(prevId, id);
                prevId = id;

                Assert.IsType<JsonObject>(employee);
            }
        }

        /// <summary>
        /// Adding KeyValuePairs collection from external library
        /// </summary>
        [Fact]
        public static void TestAddingKeyValuePairsCollection()
        {
            var employees = new JsonObject(EmployeesDatabase.GetTenBestEmployees());

            string prevId = "";
            foreach ((string id, JsonNode employee) in employees)
            {
                Assert.NotEqual(prevId, id);
                prevId = id;

                Assert.IsType<JsonObject>(employee);
            }
        }

        /// <summary>
        /// Adding KeyValuePairs collection from external library after initialization
        /// </summary>
        [Fact]
        public static void TestAddingKeyValuePairsCollectionAfterInitialization()
        {
            var employees = new JsonObject();
            employees.AddRange(EmployeesDatabase.GetTenBestEmployees());

            string prevId = "";
            foreach ((string id, JsonNode employee) in employees)
            {
                Assert.NotEqual(prevId, id);
                prevId = id;

                Assert.IsType<JsonObject>(employee);
            }
        }

        /// <summary>
        /// Adding JsonArray to JsonObject by creating it in initializing collection
        /// </summary>
        [Fact]
        public static void TestAddingJsonArray()
        {
            var preferences = new JsonObject()
            {
                { "colours", new JsonArray{ "red", "green", "purple" } }       
            };

            Assert.IsType<JsonArray>(preferences["colours"]);
            var colours = preferences["colours"] as JsonArray;
            Assert.Equal(3, colours.Count);

            string[] expected = { "red", "green", "blue" };

            for (int i = 0; i < colours.Count; i++)
            {
                Assert.IsType<JsonString>(colours[i]);
                Assert.Equal(expected[i], colours[i] as JsonString);
            }
        }

        /// <summary>
        /// Adding JsonArray to JsonObject by creating it from string array
        /// </summary>
        [Fact]
        public static void TestCretingJsonArrayFromStringArray()
        {
            string[] expected = { "sushi", "pasta", "cucumber soup" };
            var preferences = new JsonObject()
            {
                { "dishes", new JsonArray(expected) }
            };

            Assert.IsType<JsonArray>(preferences["dishes"]);
            var dishesJson = preferences["dishes"] as JsonArray;
            Assert.Equal(3, dishesJson.Count);

            for (int i = 0; i < dishesJson.Count; i++)
            {
                Assert.IsType<JsonString>(dishesJson[i]);
                Assert.Equal(expected[i], dishesJson[i] as JsonString);
            }
        }

        /// <summary>
        /// Adding JsonArray to JsonObject by passing JsonNumber array
        /// </summary>
        [Fact]
        public static void TestAddingJsonArrayFromJsonNumberArray()
        {
            var preferences = new JsonObject()
            {
                { "prime numbers", new JsonNumber[] { 19, 37 } }
            };

            Assert.IsType<JsonArray>(preferences["prime numbers"]);
            var primeNumbers = preferences["prime numbers"] as JsonArray;
            Assert.Equal(2, primeNumbers.Count);

            int[] expected = { 19, 37 };

            for (int i = 0; i < primeNumbers.Count; i++)
            {
                Assert.IsType<JsonNumber>(primeNumbers[i]);
                Assert.Equal(expected[i], primeNumbers[i] as JsonNumber);
            }
        }

        /// <summary>
        /// Adding JsonArray to JsonObject by passing IEnumerable of strings
        /// </summary>
        [Fact]
        public static void TestAddingJsonArrayFromIEnumerableOfStrings()
        {
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

            var preferences = new JsonObject()
            {
                { "sports", new JsonArray(sports) }
            };

            Assert.IsType<JsonArray>(preferences["sports"]);
            var sportsJsonArray = preferences["sports"] as JsonArray;
            Assert.Equal(3, sportsJsonArray.Count);

            for (int i = 0; i < sportsJsonArray.Count; i++)
            {
                Assert.IsType<JsonString>(sportsJsonArray[i]);
                Assert.Equal(sports.ElementAt(i), sportsJsonArray[i] as JsonString);
            }
        }

        /// <summary>
        /// Adding JsonArray to JsonObject by passing IEnumerable of JsonNodes
        /// </summary>
        [Fact]
        public static void TestAddingJsonArrayFromIEnumerableOfJsonNodes()
        {
            var strangeWords = new JsonArray()
            {
                "supercalifragilisticexpialidocious",
                "gladiolus",
                "albumen",
                "smaragdine"
            };

            var preferences = new JsonObject()
            {
                { "strange words", strangeWords.Where(word => ((JsonString)word).Value.Length < 10) }
            };

            Assert.IsType<JsonArray>(preferences["strange words"]);
            var strangeWordsJsonArray = preferences["strange words"] as JsonArray;
            Assert.Equal(2, strangeWordsJsonArray.Count);

            string [] expected = { "gladiolus", "albumen" };

            for (int i = 0; i < strangeWordsJsonArray.Count; i++)
            {
                Assert.IsType<JsonString>(strangeWordsJsonArray[i]);
                Assert.Equal(expected[i], strangeWordsJsonArray[i] as JsonString);
            }
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

            Assert.IsType<JsonArray>(vertices[0]);
            var innerJsonArray = vertices[0] as JsonArray;
            Assert.IsType<JsonArray>(innerJsonArray[0]);
            innerJsonArray = innerJsonArray[0] as JsonArray;
            Assert.IsType<JsonArray>(innerJsonArray[0]);
        }


        /// <summary>
        /// Creating Json array from collection
        /// </summary>
        [Fact]
        public static void TestCreatingJsonArrayFromCollection()
        {
            var employeesIds = new JsonArray(EmployeesDatabase.GetTenBestEmployees().Select(employee => new JsonString(employee.Key)));

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
        /// Creating Json array from collection of strings
        /// </summary>
        [Fact]
        public static void TestCreatingJsonArrayFromCollectionOfString()
        {
            var employeesIds = new JsonArray(EmployeesDatabase.GetTenBestEmployees().Select(employee => employee.Key));

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

            Assert.True(person.ContainsProperty("ssn"));
            Assert.False(person.ContainsProperty("surname"));

            // Different scenario:

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
        /// Aquiring all values
        /// </summary>
        [Fact]
        public static void TestAquiringAllValues()
        {
            var employees = new JsonObject(EmployeesDatabase.GetTenBestEmployees());
            ICollection<JsonNode> employeesWithoutId = employees.Values;

            foreach(JsonNode employee in employeesWithoutId)
            {
                Assert.IsType<JsonObject>(employee);
            }
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

            Assert.Equal(3, fullTimeEmployees.Count());
            Assert.True(fullTimeEmployees.Contains((JsonString)"John Smith"));
            Assert.True(fullTimeEmployees.Contains((JsonString)"Ann Predictable"));
            Assert.True(fullTimeEmployees.Contains((JsonString)"Byron Shadow"));
        }
    }
}
