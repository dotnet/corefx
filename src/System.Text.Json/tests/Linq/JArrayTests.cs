// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace System.Text.Json.Linq.Tests
{
    public static class JArrayTests
    {

        private static void TestArray<T>(T value1, T value2, Func<JArray, T> getter, Func<T, JNode> nodeCtor)
        {
            JNode value1Node = nodeCtor(value1);
            JNode value2Node = nodeCtor(value2);

            var list = new List<T>() { value1 };
            JArray jsonArray = new JArray(list as dynamic);
            Assert.Equal(1, jsonArray.Count);

            Assert.True(jsonArray.Contains(value1Node));
            Assert.Equal(value1, getter(jsonArray));

            jsonArray.Insert(0, value2 as dynamic);
            Assert.Equal(2, jsonArray.Count);
            Assert.True(jsonArray.Contains(value2Node));
            Assert.Equal(value2, getter(jsonArray));

            Assert.Equal(1, jsonArray.IndexOf(value1Node));
            Assert.Equal(1, jsonArray.LastIndexOf(value1Node));

            jsonArray.RemoveAt(0);
            Assert.False(jsonArray.Contains(value2Node));

            jsonArray.Remove(value1Node);
            Assert.False(jsonArray.Contains(value1Node));

            Assert.Equal(0, jsonArray.Count);

            jsonArray.Add(value2 as dynamic);
            Assert.Equal(1, jsonArray.Count);
            Assert.True(jsonArray.Contains(value2Node));
            Assert.Equal(value2, getter(jsonArray));

            jsonArray[0] = value1Node;
            Assert.Equal(1, jsonArray.Count);
            Assert.True(jsonArray.Contains(value1Node));
            Assert.Equal(value1, getter(jsonArray));
        }

        [Fact]
        public static void TestStringArray()
        {
            TestArray(
                "value1", "value2",
                jsonArray => ((JString)jsonArray[0]).Value,
                v => new JString(v)
            );
        }

        [Fact]
        public static void TestBooleanArray()
        {
            TestArray(
                true, false,
                jsonArray => ((JBoolean)jsonArray[0]).Value,
                v => new JBoolean(v)
            );
        }

        [Fact]
        public static void TestByteArray()
        {
            TestArray<byte>(
                byte.MaxValue, byte.MaxValue - 1,
                jsonArray => ((JNumber)jsonArray[0]).GetByte(),
                v => new JNumber(v)
            );
        }

        [Fact]
        public static void TestInt16Array()
        {
            TestArray<short>(
                short.MaxValue, short.MaxValue - 1,
                jsonArray => ((JNumber)jsonArray[0]).GetInt16(),
                v => new JNumber(v)
            );
        }

        [Fact]
        public static void TestInt32rray()
        {
            TestArray(
                int.MaxValue, int.MaxValue - 1,
                jsonArray => ((JNumber)jsonArray[0]).GetInt32(),
                v => new JNumber(v)
            );
        }

        [Fact]
        public static void TestInt64rray()
        {
            TestArray(
                long.MaxValue, long.MaxValue - 1,
                jsonArray => ((JNumber)jsonArray[0]).GetInt64(),
                v => new JNumber(v)
            );
        }

        [Fact]
        public static void TestSingleArray()
        {
            TestArray(
                3.14f, 1.41f,
                jsonArray => ((JNumber)jsonArray[0]).GetSingle(),
                v => new JNumber(v)
            );
        }

        [Fact]
        public static void TestDoubleArray()
        {
            TestArray(
                3.14, 1.41,
                jsonArray => ((JNumber)jsonArray[0]).GetDouble(),
                v => new JNumber(v)
            );
        }

        [Fact]
        public static void TestSByteArray()
        {
            TestArray<sbyte>(
                sbyte.MaxValue, sbyte.MaxValue - 1,
                jsonArray => ((JNumber)jsonArray[0]).GetSByte(),
                v => new JNumber(v)
            );
        }

        [Fact]
        public static void TestUInt16Array()
        {
            TestArray<ushort>(
                ushort.MaxValue, ushort.MaxValue - 1,
                jsonArray => ((JNumber)jsonArray[0]).GetUInt16(),
                v => new JNumber(v)
            );
        }

        [Fact]
        public static void TestUInt32Array()
        {
            TestArray(
                uint.MaxValue, uint.MaxValue - 1,
                jsonArray => ((JNumber)jsonArray[0]).GetUInt32(),
                v => new JNumber(v)
            );
        }

        [Fact]
        public static void TestUInt64Array()
        {
            TestArray(
                ulong.MaxValue, ulong.MaxValue - 1,
                jsonArray => ((JNumber)jsonArray[0]).GetUInt64(),
                v => new JNumber(v)
            );
        }

        [Fact]
        public static void TestDecimalArray()
        {
            TestArray(
                decimal.One, decimal.Zero,
                jsonArray => ((JNumber)jsonArray[0]).GetDecimal(),
                v => new JNumber(v)
            );
        }

        [Fact]
        public static void TestCreatingJArrayFromStringArray()
        {
            string[] expected = { "sushi", "pasta", "cucumber soup" };

            var dishesJArray = new JArray(expected);
            Assert.Equal(3, dishesJArray.Count);

            for (int i = 0; i < dishesJArray.Count; i++)
            {
                Assert.Equal(expected[i], ((JString)dishesJArray[i]).Value);
            }
        }

        [Fact]
        public static void TestCreatingJArrayFromIEnumerableOfStrings()
        {
            var sportsExperienceYears = new JObject()
            {
                { "skiing", 5 },
                { "cycling", 8 },
                { "hiking", 6 },
                { "chess", 2 },
                { "skating", 1 },
            };

            // choose only sports with > 2 experience years
            IEnumerable<string> sports = sportsExperienceYears.Where(sport => ((JNumber)sport.Value).GetInt32() > 2).Select(sport => sport.Key);

            var sportsJArray = new JArray(sports);
            Assert.Equal(3, sportsJArray.Count);

            for (int i = 0; i < sportsJArray.Count; i++)
            {
                Assert.Equal(sports.ElementAt(i), ((JString)sportsJArray[i]).Value);
            }
        }

        [Fact]
        public static void TestCreatingJArrayFromIEnumerableOfJNodes()
        {
            var strangeWords = new JArray()
            {
                "supercalifragilisticexpialidocious",
                "gladiolus",
                "albumen",
                "smaragdine"
            };

            var strangeWordsJArray = new JArray(strangeWords.Where(word => ((JString)word).Value.Length < 10));
            Assert.Equal(2, strangeWordsJArray.Count);

            string[] expected = { "gladiolus", "albumen" };

            for (int i = 0; i < strangeWordsJArray.Count; i++)
            {
                Assert.Equal(expected[i], ((JString)strangeWordsJArray[i]).Value);
            }
        }

        [Fact]
        public static void TestCreatingNestedJArray()
        {
            var vertices = new JArray()
            {
                new JArray
                {
                    new JArray
                    {
                        new JArray { 0, 0, 0 },
                        new JArray { 0, 0, 1 }
                    },
                    new JArray
                    {
                        new JArray { 0, 1, 0 },
                        new JArray { 0, 1, 1 }
                    }
                },
                new JArray
                {
                    new JArray
                    {
                        new JArray { 1, 0, 0 },
                        new JArray { 1, 0, 1 }
                    },
                    new JArray
                    {
                        new JArray { 1, 1, 0 },
                        new JArray { 1, 1, 1 }
                    }
                },
            };

            var jsonArray = (JArray)vertices[0];
            Assert.Equal(2, jsonArray.Count());
            jsonArray = (JArray)jsonArray[1];
            Assert.Equal(2, jsonArray.Count());
            jsonArray = (JArray)jsonArray[0];
            Assert.Equal(3, jsonArray.Count());

            Assert.Equal(0, jsonArray[0]);
            Assert.Equal(1, jsonArray[1]);
            Assert.Equal(0, jsonArray[2]);
        }

        [Fact]
        public static void TestCreatingJArrayFromCollection()
        {
            var employeesIds = new JArray(EmployeesDatabase.GetTenBestEmployees().Select(employee => new JString(employee.Key)));

            JString prevId = new JString();
            foreach (JNode employeeId in employeesIds)
            {
                var employeeIdString = (JString)employeeId;
                Assert.NotEqual(prevId, employeeIdString);
                prevId = employeeIdString;
            }
        }

        [Fact]
        public static void TestCreatingJArrayFromCollectionOfString()
        {
            var employeesIds = new JArray(EmployeesDatabase.GetTenBestEmployees().Select(employee => employee.Key));

            JString prevId = new JString();
            foreach (JNode employeeId in employeesIds)
            {
                var employeeIdString = (JString)employeeId;
                Assert.NotEqual(prevId, employeeIdString);
                prevId = employeeIdString;
            }
        }

        [Fact]
        public static void TestAddingToJArray()
        {
            var employeesIds = new JArray();

            foreach (KeyValuePair<string, JNode> employee in EmployeesDatabase.GetTenBestEmployees())
            {
                employeesIds.Add(employee.Key);
            }

            JString prevId = new JString();
            foreach (JNode employeeId in employeesIds)
            {
                var employeeIdString = (JString)employeeId;
                Assert.NotEqual(prevId, employeeIdString);
                prevId = employeeIdString;
            }
        }

        [Fact]
        public static void TestHandlingNulls()
        {
            var jsonArray = new JArray() { "to be replaced" };

            jsonArray[0] = null;
            Assert.Equal(1, jsonArray.Count());
            Assert.IsType<JNull>(jsonArray[0]);

            jsonArray.Add(null);
            Assert.Equal(2, jsonArray.Count());
            Assert.IsType<JNull>(jsonArray[1]);

            jsonArray.Add(new JNull());
            Assert.Equal(3, jsonArray.Count());
            Assert.IsType<JNull>(jsonArray[2]);

            jsonArray.Insert(3, null);
            Assert.Equal(4, jsonArray.Count());
            Assert.IsType<JNull>(jsonArray[3]);

            jsonArray.Insert(4, new JNull());
            Assert.Equal(5, jsonArray.Count());
            Assert.IsType<JNull>(jsonArray[4]);

            Assert.True(jsonArray.Contains(null));

            Assert.Equal(0, jsonArray.IndexOf(null));
            Assert.Equal(4, jsonArray.LastIndexOf(null));

            jsonArray.Remove(null);
            Assert.Equal(4, jsonArray.Count());
        }

        [Fact]
        public static void TestAccesingNestedJArrayGetPropertyMethod()
        {
            var issues = new JObject()
            {
                { "features", new JArray { "new functionality 1", "new functionality 2" } },
                { "bugs", new JArray { "bug 123", "bug 4566", "bug 821" } },
                { "tests", new JArray { "code coverage" } },
            };

            issues.GetJsonArrayPropertyValue("bugs").Add("bug 12356");
            ((JString)issues.GetJsonArrayPropertyValue("features")[0]).Value = "feature 1569";
            ((JString)issues.GetJsonArrayPropertyValue("features")[1]).Value = "feature 56134";

            Assert.Equal("bug 12356", ((JString)((JArray)issues["bugs"])[3]).Value);
            Assert.Equal("feature 1569", ((JString)((JArray)issues["features"])[0]).Value);
            Assert.Equal("feature 56134", ((JString)((JArray)issues["features"])[1]).Value);
        }

        [Fact]
        public static void TestAccesingNestedJArrayTryGetPropertyMethod()
        {
            var issues = new JObject()
            {
                { "features", new JArray { "new functionality 1", "new functionality 2" } },
            };

            Assert.True(issues.TryGetJsonArrayPropertyValue("features", out JArray featuresArray));
            Assert.Equal("new functionality 1", ((JString)featuresArray[0]).Value);
            Assert.Equal("new functionality 2", ((JString)featuresArray[1]).Value);
        }

        [Fact]
        public static void TestInsert()
        {
            var jsonArray = new JArray() { 1 };
            Assert.Equal(1, jsonArray.Count);

            jsonArray.Insert(0, 0);

            Assert.Equal(2, jsonArray.Count);
            Assert.Equal(0, jsonArray[0]);
            Assert.Equal(1, jsonArray[1]);

            jsonArray.Insert(2, 3);

            Assert.Equal(3, jsonArray.Count);
            Assert.Equal(0, jsonArray[0]);
            Assert.Equal(1, jsonArray[1]);
            Assert.Equal(3, jsonArray[2]);

            jsonArray.Insert(2, 2);

            Assert.Equal(4, jsonArray.Count);
            Assert.Equal(0, jsonArray[0]);
            Assert.Equal(1, jsonArray[1]);
            Assert.Equal(2, jsonArray[2]);
            Assert.Equal(3, jsonArray[3]);
        }

        [Fact]
        public static void TestHeterogeneousArray()
        {
            var mixedTypesArray = new JArray { 1, "value", true };

            Assert.Equal(1, mixedTypesArray[0]);
            Assert.Equal("value", mixedTypesArray[1]);
            Assert.Equal(true, mixedTypesArray[2]);

            mixedTypesArray.Add(17);
            mixedTypesArray.Insert(4, "another");
            mixedTypesArray.Add(new JNull());

            Assert.Equal(17, mixedTypesArray[3]);
            Assert.Equal("another", mixedTypesArray[4]);
            Assert.IsType<JNull>(mixedTypesArray[5]);

        }

        [Fact]
        public static void TestOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new JArray()[-1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => new JArray()[-1] = new JString());
            Assert.Throws<ArgumentOutOfRangeException>(() => new JArray()[0]);
            Assert.Throws<ArgumentOutOfRangeException>(() => new JArray()[0] = new JString());
            Assert.Throws<ArgumentOutOfRangeException>(() => new JArray()[1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => new JArray()[1] = new JString());
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var jsonArray = new JArray { 1, 2, 3 };
                jsonArray.Insert(4, 17);
            });
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var jsonArray = new JArray { 1, 2, 3 };
                jsonArray.Insert(-1, 17);
            });
        }

        [Fact]
        public static void TestIsReadOnly()
        {
            Assert.False(new JArray().IsReadOnly);
        }

        [Fact]
        public static void TestClear()
        {
            var jsonArray = new JArray { 1, 2, 3 };

            Assert.Equal(3, jsonArray.Count);
            Assert.Equal(1, ((JNumber)jsonArray[0]).GetInt32());
            Assert.Equal(2, ((JNumber)jsonArray[1]).GetInt32());
            Assert.Equal(3, ((JNumber)jsonArray[2]).GetInt32());

            jsonArray.Clear();

            Assert.Equal(0, jsonArray.Count);
        }

        [Fact]
        public static void TestRemoveAll()
        {
            var jsonArray = new JArray { 1, 2, 3 };

            Assert.Equal(3, jsonArray.Count);
            Assert.Equal(1, ((JNumber)jsonArray[0]).GetInt32());
            Assert.Equal(2, ((JNumber)jsonArray[1]).GetInt32());
            Assert.Equal(3, ((JNumber)jsonArray[2]).GetInt32());

            jsonArray.RemoveAll(v => ((JNumber)v).GetInt32() <= 2);

            Assert.Equal(1, jsonArray.Count);
            Assert.Equal(3, ((JNumber)jsonArray[0]).GetInt32());
        }

        [Fact]
        public static void TestValueKind()
        {
            Assert.Equal(JsonValueKind.Array, new JArray().ValueKind);
        }

        [Fact]
        public static void TestJArrayIEnumerator()
        {
            var jsonArray = new JArray() { 1, "value" };

            // Test generic IEnumerator:
            IEnumerator<JNode> jsonArrayEnumerator = new JArrayEnumerator(jsonArray);

            Assert.Null(jsonArrayEnumerator.Current);

            jsonArrayEnumerator.MoveNext();
            Assert.Equal(1, jsonArrayEnumerator.Current);
            jsonArrayEnumerator.MoveNext();
            Assert.Equal("value", jsonArrayEnumerator.Current);

            jsonArrayEnumerator.Reset();

            jsonArrayEnumerator.MoveNext();
            Assert.Equal(1, jsonArrayEnumerator.Current);
            jsonArrayEnumerator.MoveNext();
            Assert.Equal("value", jsonArrayEnumerator.Current);

            // Test non-generic IEnumerator:
            IEnumerator jsonArrayEnumerator2 = new JArrayEnumerator(jsonArray);

            Assert.Null(jsonArrayEnumerator2.Current);

            jsonArrayEnumerator2.MoveNext();
            Assert.Equal((JNumber)1, jsonArrayEnumerator2.Current);
            jsonArrayEnumerator2.MoveNext();
            Assert.Equal((JString)"value", jsonArrayEnumerator2.Current);

            jsonArrayEnumerator2.Reset();

            jsonArrayEnumerator2.MoveNext();
            Assert.Equal((JNumber)1, jsonArrayEnumerator2.Current);
            jsonArrayEnumerator2.MoveNext();
            Assert.Equal((JString)"value", jsonArrayEnumerator2.Current);
        }

        [Fact]
        public static void TestGetJArrayIEnumerable()
        {
            IEnumerable jsonArray = new JArray() { 1, "value" };
            IEnumerator jsonArrayEnumerator = jsonArray.GetEnumerator();
            
            Assert.Null(jsonArrayEnumerator.Current);

            jsonArrayEnumerator.MoveNext();
            Assert.Equal((JNumber)1, jsonArrayEnumerator.Current);
            jsonArrayEnumerator.MoveNext();
            Assert.Equal((JString)"value", jsonArrayEnumerator.Current);
        }

        [Fact]
        public static void TestJArrayEmptyArrayEnumerator()
        {
            var jsonArray = new JArray();
            var jsonArrayEnumerator = new JArrayEnumerator(jsonArray);

            Assert.Null(jsonArrayEnumerator.Current);
            Assert.False(jsonArrayEnumerator.MoveNext());
        }
    }
}
