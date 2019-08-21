﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace System.Text.Json.Tests
{
    public static class JsonArrayTests
    {

        private static void TestArray<T>(T value1, T value2, Func<JsonArray, T> getter, Func<T, JsonNode> nodeCtor)
        {
            var value1Casted = value1 as dynamic;
            var value2Casted = value2 as dynamic;

            var list = new List<T>() { value1 };
            JsonArray jsonArray = new JsonArray(list as dynamic);
            Assert.Equal(1, jsonArray.Count);

            Assert.True(jsonArray.Contains(value1Casted));
            Assert.Equal(value1, getter(jsonArray));

            jsonArray.Insert(0, value2 as dynamic);
            Assert.Equal(2, jsonArray.Count);
            Assert.True(jsonArray.Contains(value2Casted));
            Assert.Equal(value2, getter(jsonArray));

            JsonNode value1Node = nodeCtor(value1);
            Assert.Equal(1, jsonArray.IndexOf(value1Node));
            Assert.Equal(1, jsonArray.LastIndexOf(value1Node));

            jsonArray.RemoveAt(0);
            Assert.False(jsonArray.Contains(value2Casted));

            jsonArray.Remove(value1Node);
            Assert.False(jsonArray.Contains(value1Casted));

            Assert.Equal(0, jsonArray.Count);

            jsonArray.Add(value2Casted);
            Assert.Equal(1, jsonArray.Count);
            Assert.True(jsonArray.Contains(value2Casted));
            Assert.Equal(value2, getter(jsonArray));

            jsonArray[0] = value1Node;
            Assert.Equal(1, jsonArray.Count);
            Assert.True(jsonArray.Contains(value1Casted));
            Assert.Equal(value1, getter(jsonArray));
        }

        [Fact]
        public static void TestStringArray()
        {
            TestArray(
                "value1", "value2",
                jsonArray => ((JsonString)jsonArray[0]).Value,
                v => new JsonString(v)
            );
        }

        [Fact]
        public static void TestBooleanArray()
        {
            TestArray(
                true, false,
                jsonArray => ((JsonBoolean)jsonArray[0]).Value,
                v => new JsonBoolean(v)
            );
        }

        [Fact]
        public static void TestByteArray()
        {
            TestArray<byte>(
                byte.MaxValue, byte.MaxValue - 1,
                jsonArray => ((JsonNumber)jsonArray[0]).GetByte(),
                v => new JsonNumber(v)
            );
        }

        [Fact]
        public static void TestInt16Array()
        {
            TestArray<short>(
                short.MaxValue, short.MaxValue - 1,
                jsonArray => ((JsonNumber)jsonArray[0]).GetInt16(),
                v => new JsonNumber(v)
            );
        }

        [Fact]
        public static void TestInt32rray()
        {
            TestArray(
                int.MaxValue, int.MaxValue - 1,
                jsonArray => ((JsonNumber)jsonArray[0]).GetInt32(),
                v => new JsonNumber(v)
            );
        }

        [Fact]
        public static void TestInt64rray()
        {
            TestArray(
                long.MaxValue, long.MaxValue - 1,
                jsonArray => ((JsonNumber)jsonArray[0]).GetInt64(),
                v => new JsonNumber(v)
            );
        }

        [Fact]
        public static void TestSingleArray()
        {
            TestArray(
                3.14f, 1.41f,
                jsonArray => ((JsonNumber)jsonArray[0]).GetSingle(),
                v => new JsonNumber(v)
            );
        }

        [Fact]
        public static void TestDoubleArray()
        {
            TestArray(
                3.14, 1.41,
                jsonArray => ((JsonNumber)jsonArray[0]).GetDouble(),
                v => new JsonNumber(v)
            );
        }

        [Fact]
        public static void TestSByteArray()
        {
            TestArray<sbyte>(
                sbyte.MaxValue, sbyte.MaxValue - 1,
                jsonArray => ((JsonNumber)jsonArray[0]).GetSByte(),
                v => new JsonNumber(v)
            );
        }

        [Fact]
        public static void TestUInt16Array()
        {
            TestArray<ushort>(
                ushort.MaxValue, ushort.MaxValue - 1,
                jsonArray => ((JsonNumber)jsonArray[0]).GetUInt16(),
                v => new JsonNumber(v)
            );
        }

        [Fact]
        public static void TestUInt32Array()
        {
            TestArray(
                uint.MaxValue, uint.MaxValue - 1,
                jsonArray => ((JsonNumber)jsonArray[0]).GetUInt32(),
                v => new JsonNumber(v)
            );
        }

        [Fact]
        public static void TestUInt64Array()
        {
            TestArray(
                ulong.MaxValue, ulong.MaxValue - 1,
                jsonArray => ((JsonNumber)jsonArray[0]).GetUInt64(),
                v => new JsonNumber(v)
            );
        }

        [Fact]
        public static void TestDecimalArray()
        {
            TestArray(
                decimal.One, decimal.Zero,
                jsonArray => ((JsonNumber)jsonArray[0]).GetDecimal(),
                v => new JsonNumber(v)
            );
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

            issues.GetJsonArrayPropertyValue("bugs").Add("bug 12356");
            ((JsonString)issues.GetJsonArrayPropertyValue("features")[0]).Value = "feature 1569";
            ((JsonString)issues.GetJsonArrayPropertyValue("features")[1]).Value = "feature 56134";

            Assert.True(((JsonArray)issues["bugs"]).Contains("bug 12356"));
            Assert.Equal("feature 1569", (JsonString)((JsonArray)issues["features"])[0]);
            Assert.Equal("feature 56134", (JsonString)((JsonArray)issues["features"])[1]);
        }

        [Fact]
        public static void TestAccesingNestedJsonArrayTryGetPropertyMethod()
        {
            var issues = new JsonObject()
            {
                { "features", new JsonString [] { "new functionality 1", "new functionality 2" } },
            };

            Assert.True(issues.TryGetJsonArrayPropertyValue("features", out JsonArray featuresArray));
            Assert.Equal("new functionality 1", (JsonString)featuresArray[0]);
            Assert.Equal("new functionality 2", (JsonString)featuresArray[1]);
        }

        [Fact]
        public static void TestOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new JsonArray()[-1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => new JsonArray()[-1] = new JsonString());
            Assert.Throws<ArgumentOutOfRangeException>(() => new JsonArray()[0]);
            Assert.Throws<ArgumentOutOfRangeException>(() => new JsonArray()[0] = new JsonString());
            Assert.Throws<ArgumentOutOfRangeException>(() => new JsonArray()[1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => new JsonArray()[1] = new JsonString());
        }

        [Fact]
        public static void TestIsReadOnly()
        {
            Assert.False(new JsonArray().IsReadOnly);
        }

        [Fact]
        public static void TestClean()
        {
            var jsonArray = new JsonArray { 1, 2, 3 };
            
            Assert.Equal(3, jsonArray.Count);
            Assert.True(jsonArray.Contains(1));
            Assert.True(jsonArray.Contains(2));
            Assert.True(jsonArray.Contains(3));

            jsonArray.Clear();
            
            Assert.Equal(0, jsonArray.Count);
            Assert.False(jsonArray.Contains(1));
            Assert.False(jsonArray.Contains(2));
            Assert.False(jsonArray.Contains(3));
        }

        [Fact]
        public static void TestRemoveAll()
        {
            var jsonArray = new JsonArray { 1, 2, 3 };

            Assert.Equal(3, jsonArray.Count);
            Assert.True(jsonArray.Contains(1));
            Assert.True(jsonArray.Contains(2));
            Assert.True(jsonArray.Contains(3));

            jsonArray.RemoveAll(v => ((JsonNumber)v).GetInt32() <= 2);

            Assert.Equal(1, jsonArray.Count);
            Assert.False(jsonArray.Contains(1));
            Assert.False(jsonArray.Contains(2));
            Assert.True(jsonArray.Contains(3));
        }

       
    }
}
