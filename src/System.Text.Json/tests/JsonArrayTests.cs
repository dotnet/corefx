// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace System.Text.Json.Tests
{
    public static partial class JsonArrayTests
    {
        [Fact]
        public static void TestAdd()
        {
            var jsonArray = new JsonArray();
            int idx = 0;

            jsonArray.Add("value");
            Assert.Equal("value", (JsonString)jsonArray[idx++]);
            Assert.True(jsonArray.Contains("value"));

            jsonArray.Add(true);
            Assert.True(((JsonBoolean)jsonArray[idx++]).Value);
            Assert.True(jsonArray.Contains(true));

            jsonArray.Add(byte.MaxValue);
            Assert.Equal(byte.MaxValue, ((JsonNumber)jsonArray[idx++]).GetByte());
            Assert.True(jsonArray.Contains(byte.MaxValue));

            jsonArray.Add(short.MaxValue);
            Assert.Equal(short.MaxValue, ((JsonNumber)jsonArray[idx++]).GetInt16());
            Assert.True(jsonArray.Contains(short.MaxValue));

            jsonArray.Add(int.MaxValue);
            Assert.Equal(int.MaxValue, ((JsonNumber)jsonArray[idx++]).GetInt32());
            Assert.True(jsonArray.Contains(int.MaxValue));

            jsonArray.Add(long.MaxValue);
            Assert.Equal(long.MaxValue, ((JsonNumber)jsonArray[idx++]).GetInt64());
            Assert.True(jsonArray.Contains(long.MaxValue));

            jsonArray.Add(3.14f);
            Assert.Equal(3.14f, ((JsonNumber)jsonArray[idx++]).GetSingle());

            jsonArray.Add(3.14);
            Assert.Equal(3.14, ((JsonNumber)jsonArray[idx++]).GetDouble());

            jsonArray.Add(sbyte.MaxValue);
            Assert.Equal(sbyte.MaxValue, ((JsonNumber)jsonArray[idx++]).GetSByte());

            jsonArray.Add(ushort.MaxValue);
            Assert.Equal(ushort.MaxValue, ((JsonNumber)jsonArray[idx++]).GetUInt16());

            jsonArray.Add(uint.MaxValue);
            Assert.Equal(uint.MaxValue, ((JsonNumber)jsonArray[idx++]).GetUInt32());

            jsonArray.Add(ulong.MaxValue);
            Assert.Equal(ulong.MaxValue, ((JsonNumber)jsonArray[idx++]).GetUInt64());

            jsonArray.Add(decimal.One);
            Assert.Equal(decimal.One, ((JsonNumber)jsonArray[idx++]).GetDecimal());
        }

        [Fact]
        public static void TestInsert()
        {
            var jsonArray = new JsonArray();

            jsonArray.Insert(0, "value");
            Assert.Equal("value", (JsonString)jsonArray[0]);

            jsonArray.Insert(0, true);
            Assert.True(((JsonBoolean)jsonArray[0]).Value);

            jsonArray.Insert(0, byte.MaxValue);
            Assert.Equal(byte.MaxValue, ((JsonNumber)jsonArray[0]).GetByte());

            jsonArray.Insert(0, short.MaxValue);
            Assert.Equal(short.MaxValue, ((JsonNumber)jsonArray[0]).GetInt16());

            jsonArray.Insert(0, int.MaxValue);
            Assert.Equal(int.MaxValue, ((JsonNumber)jsonArray[0]).GetInt32());

            jsonArray.Insert(0, long.MaxValue);
            Assert.Equal(long.MaxValue, ((JsonNumber)jsonArray[0]).GetInt64());

            jsonArray.Insert(0, 3.14f);
            Assert.Equal(3.14f, ((JsonNumber)jsonArray[0]).GetSingle());

            jsonArray.Insert(0, 3.14);
            Assert.Equal(3.14, ((JsonNumber)jsonArray[0]).GetDouble());

            jsonArray.Insert(0, sbyte.MaxValue);
            Assert.Equal(sbyte.MaxValue, ((JsonNumber)jsonArray[0]).GetSByte());

            jsonArray.Insert(0, ushort.MaxValue);
            Assert.Equal(ushort.MaxValue, ((JsonNumber)jsonArray[0]).GetUInt16());

            jsonArray.Insert(0, uint.MaxValue);
            Assert.Equal(uint.MaxValue, ((JsonNumber)jsonArray[0]).GetUInt32());

            jsonArray.Insert(0, ulong.MaxValue);
            Assert.Equal(ulong.MaxValue, ((JsonNumber)jsonArray[0]).GetUInt64());

            jsonArray.Insert(0, decimal.One);
            Assert.Equal(decimal.One, ((JsonNumber)jsonArray[0]).GetDecimal());
        }

        [Fact]
        public static void TestAddingJsonArray()
        {
            var preferences = new JsonObject()
            {
                { "colours", (JsonNode) new JsonArray{ "red", "green", "blue" } }
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

        [Fact]
        public static void TestCretingJsonArrayFromStringArray()
        {
            string[] expected = { "sushi", "pasta", "cucumber soup" };
            var preferences = new JsonObject()
            {
                { "dishes", (JsonNode) new JsonArray(expected) }
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
                { "sports", (JsonNode) new JsonArray(sports) }
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

            string[] expected = { "gladiolus", "albumen" };

            for (int i = 0; i < strangeWordsJsonArray.Count; i++)
            {
                Assert.IsType<JsonString>(strangeWordsJsonArray[i]);
                Assert.Equal(expected[i], strangeWordsJsonArray[i] as JsonString);
            }
        }

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

        [Fact]
        public static void TestAccesingNestedJsonArrayGetPropertyMethod()
        {
            var issues = new JsonObject()
            {
                { "features", new JsonString [] { "new functionality 1", "new functionality 2" } },
                { "bugs", new JsonString [] { "bug 123", "bug 4566", "bug 821" } },
                { "tests", new JsonString [] { "code coverage" } },
            };

            issues.GetJsonArrayProperty("bugs").Add("bug 12356");
            ((JsonString)issues.GetJsonArrayProperty("features")[0]).Value = "feature 1569";
            ((JsonString)issues.GetJsonArrayProperty("features")[1]).Value = "feature 56134";

            Assert.True(((JsonArray)issues["bugs"]).Contains("bug 12356"));
            Assert.Equal("feature 1569", (JsonString)((JsonArray)issues["features"])[0]);
            Assert.Equal("feature 56134", (JsonString)((JsonArray)issues["features"])[1]);
        }
    }
}
