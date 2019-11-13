// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace System.Text.Json.Linq.Tests
{
    public static class JTreeArrayTests
    {

        private static void TestArray<T>(T value1, T value2, Func<JTreeArray, T> getter, Func<T, JTreeNode> nodeCtor)
        {
            JTreeNode value1Node = nodeCtor(value1);
            JTreeNode value2Node = nodeCtor(value2);

            var list = new List<T>() { value1 };
            JTreeArray jsonArray = new JTreeArray(list as dynamic);
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
                jsonArray => ((JTreeString)jsonArray[0]).Value,
                v => new JTreeString(v)
            );
        }

        [Fact]
        public static void TestBooleanArray()
        {
            TestArray(
                true, false,
                jsonArray => ((JTreeBoolean)jsonArray[0]).Value,
                v => new JTreeBoolean(v)
            );
        }

        [Fact]
        public static void TestByteArray()
        {
            TestArray<byte>(
                byte.MaxValue, byte.MaxValue - 1,
                jsonArray => ((JTreeNumber)jsonArray[0]).GetByte(),
                v => new JTreeNumber(v)
            );
        }

        [Fact]
        public static void TestInt16Array()
        {
            TestArray<short>(
                short.MaxValue, short.MaxValue - 1,
                jsonArray => ((JTreeNumber)jsonArray[0]).GetInt16(),
                v => new JTreeNumber(v)
            );
        }

        [Fact]
        public static void TestInt32rray()
        {
            TestArray(
                int.MaxValue, int.MaxValue - 1,
                jsonArray => ((JTreeNumber)jsonArray[0]).GetInt32(),
                v => new JTreeNumber(v)
            );
        }

        [Fact]
        public static void TestInt64rray()
        {
            TestArray(
                long.MaxValue, long.MaxValue - 1,
                jsonArray => ((JTreeNumber)jsonArray[0]).GetInt64(),
                v => new JTreeNumber(v)
            );
        }

        [Fact]
        public static void TestSingleArray()
        {
            TestArray(
                3.14f, 1.41f,
                jsonArray => ((JTreeNumber)jsonArray[0]).GetSingle(),
                v => new JTreeNumber(v)
            );
        }

        [Fact]
        public static void TestDoubleArray()
        {
            TestArray(
                3.14, 1.41,
                jsonArray => ((JTreeNumber)jsonArray[0]).GetDouble(),
                v => new JTreeNumber(v)
            );
        }

        [Fact]
        public static void TestSByteArray()
        {
            TestArray<sbyte>(
                sbyte.MaxValue, sbyte.MaxValue - 1,
                jsonArray => ((JTreeNumber)jsonArray[0]).GetSByte(),
                v => new JTreeNumber(v)
            );
        }

        [Fact]
        public static void TestUInt16Array()
        {
            TestArray<ushort>(
                ushort.MaxValue, ushort.MaxValue - 1,
                jsonArray => ((JTreeNumber)jsonArray[0]).GetUInt16(),
                v => new JTreeNumber(v)
            );
        }

        [Fact]
        public static void TestUInt32Array()
        {
            TestArray(
                uint.MaxValue, uint.MaxValue - 1,
                jsonArray => ((JTreeNumber)jsonArray[0]).GetUInt32(),
                v => new JTreeNumber(v)
            );
        }

        [Fact]
        public static void TestUInt64Array()
        {
            TestArray(
                ulong.MaxValue, ulong.MaxValue - 1,
                jsonArray => ((JTreeNumber)jsonArray[0]).GetUInt64(),
                v => new JTreeNumber(v)
            );
        }

        [Fact]
        public static void TestDecimalArray()
        {
            TestArray(
                decimal.One, decimal.Zero,
                jsonArray => ((JTreeNumber)jsonArray[0]).GetDecimal(),
                v => new JTreeNumber(v)
            );
        }

        [Fact]
        public static void TestCreatingJTreeArrayFromStringArray()
        {
            string[] expected = { "sushi", "pasta", "cucumber soup" };

            var dishesJTreeArray = new JTreeArray(expected);
            Assert.Equal(3, dishesJTreeArray.Count);

            for (int i = 0; i < dishesJTreeArray.Count; i++)
            {
                Assert.Equal(expected[i], ((JTreeString)dishesJTreeArray[i]).Value);
            }
        }

        [Fact]
        public static void TestCreatingJTreeArrayFromIEnumerableOfStrings()
        {
            var sportsExperienceYears = new JTreeObject()
            {
                { "skiing", 5 },
                { "cycling", 8 },
                { "hiking", 6 },
                { "chess", 2 },
                { "skating", 1 },
            };

            // choose only sports with > 2 experience years
            IEnumerable<string> sports = sportsExperienceYears.Where(sport => ((JTreeNumber)sport.Value).GetInt32() > 2).Select(sport => sport.Key);

            var sportsJTreeArray = new JTreeArray(sports);
            Assert.Equal(3, sportsJTreeArray.Count);

            for (int i = 0; i < sportsJTreeArray.Count; i++)
            {
                Assert.Equal(sports.ElementAt(i), ((JTreeString)sportsJTreeArray[i]).Value);
            }
        }

        [Fact]
        public static void TestCreatingJTreeArrayFromIEnumerableOfJTreeNodes()
        {
            var strangeWords = new JTreeArray()
            {
                "supercalifragilisticexpialidocious",
                "gladiolus",
                "albumen",
                "smaragdine"
            };

            var strangeWordsJTreeArray = new JTreeArray(strangeWords.Where(word => ((JTreeString)word).Value.Length < 10));
            Assert.Equal(2, strangeWordsJTreeArray.Count);

            string[] expected = { "gladiolus", "albumen" };

            for (int i = 0; i < strangeWordsJTreeArray.Count; i++)
            {
                Assert.Equal(expected[i], ((JTreeString)strangeWordsJTreeArray[i]).Value);
            }
        }

        [Fact]
        public static void TestCreatingNestedJTreeArray()
        {
            var vertices = new JTreeArray()
            {
                new JTreeArray
                {
                    new JTreeArray
                    {
                        new JTreeArray { 0, 0, 0 },
                        new JTreeArray { 0, 0, 1 }
                    },
                    new JTreeArray
                    {
                        new JTreeArray { 0, 1, 0 },
                        new JTreeArray { 0, 1, 1 }
                    }
                },
                new JTreeArray
                {
                    new JTreeArray
                    {
                        new JTreeArray { 1, 0, 0 },
                        new JTreeArray { 1, 0, 1 }
                    },
                    new JTreeArray
                    {
                        new JTreeArray { 1, 1, 0 },
                        new JTreeArray { 1, 1, 1 }
                    }
                },
            };

            var jsonArray = (JTreeArray)vertices[0];
            Assert.Equal(2, jsonArray.Count());
            jsonArray = (JTreeArray)jsonArray[1];
            Assert.Equal(2, jsonArray.Count());
            jsonArray = (JTreeArray)jsonArray[0];
            Assert.Equal(3, jsonArray.Count());

            Assert.Equal(0, jsonArray[0]);
            Assert.Equal(1, jsonArray[1]);
            Assert.Equal(0, jsonArray[2]);
        }

        [Fact]
        public static void TestCreatingJTreeArrayFromCollection()
        {
            var employeesIds = new JTreeArray(EmployeesDatabase.GetTenBestEmployees().Select(employee => new JTreeString(employee.Key)));

            JTreeString prevId = new JTreeString();
            foreach (JTreeNode employeeId in employeesIds)
            {
                var employeeIdString = (JTreeString)employeeId;
                Assert.NotEqual(prevId, employeeIdString);
                prevId = employeeIdString;
            }
        }

        [Fact]
        public static void TestCreatingJTreeArrayFromCollectionOfString()
        {
            var employeesIds = new JTreeArray(EmployeesDatabase.GetTenBestEmployees().Select(employee => employee.Key));

            JTreeString prevId = new JTreeString();
            foreach (JTreeNode employeeId in employeesIds)
            {
                var employeeIdString = (JTreeString)employeeId;
                Assert.NotEqual(prevId, employeeIdString);
                prevId = employeeIdString;
            }
        }

        [Fact]
        public static void TestAddingToJTreeArray()
        {
            var employeesIds = new JTreeArray();

            foreach (KeyValuePair<string, JTreeNode> employee in EmployeesDatabase.GetTenBestEmployees())
            {
                employeesIds.Add(employee.Key);
            }

            JTreeString prevId = new JTreeString();
            foreach (JTreeNode employeeId in employeesIds)
            {
                var employeeIdString = (JTreeString)employeeId;
                Assert.NotEqual(prevId, employeeIdString);
                prevId = employeeIdString;
            }
        }

        [Fact]
        public static void TestHandlingNulls()
        {
            var jsonArray = new JTreeArray() { "to be replaced" };

            jsonArray[0] = null;
            Assert.Equal(1, jsonArray.Count());
            Assert.IsType<JTreeNull>(jsonArray[0]);

            jsonArray.Add(null);
            Assert.Equal(2, jsonArray.Count());
            Assert.IsType<JTreeNull>(jsonArray[1]);

            jsonArray.Add(new JTreeNull());
            Assert.Equal(3, jsonArray.Count());
            Assert.IsType<JTreeNull>(jsonArray[2]);

            jsonArray.Insert(3, null);
            Assert.Equal(4, jsonArray.Count());
            Assert.IsType<JTreeNull>(jsonArray[3]);

            jsonArray.Insert(4, new JTreeNull());
            Assert.Equal(5, jsonArray.Count());
            Assert.IsType<JTreeNull>(jsonArray[4]);

            Assert.True(jsonArray.Contains(null));

            Assert.Equal(0, jsonArray.IndexOf(null));
            Assert.Equal(4, jsonArray.LastIndexOf(null));

            jsonArray.Remove(null);
            Assert.Equal(4, jsonArray.Count());
        }

        [Fact]
        public static void TestAccesingNestedJTreeArrayGetPropertyMethod()
        {
            var issues = new JTreeObject()
            {
                { "features", new JTreeArray { "new functionality 1", "new functionality 2" } },
                { "bugs", new JTreeArray { "bug 123", "bug 4566", "bug 821" } },
                { "tests", new JTreeArray { "code coverage" } },
            };

            issues.GetJsonArrayPropertyValue("bugs").Add("bug 12356");
            ((JTreeString)issues.GetJsonArrayPropertyValue("features")[0]).Value = "feature 1569";
            ((JTreeString)issues.GetJsonArrayPropertyValue("features")[1]).Value = "feature 56134";

            Assert.Equal("bug 12356", ((JTreeString)((JTreeArray)issues["bugs"])[3]).Value);
            Assert.Equal("feature 1569", ((JTreeString)((JTreeArray)issues["features"])[0]).Value);
            Assert.Equal("feature 56134", ((JTreeString)((JTreeArray)issues["features"])[1]).Value);
        }

        [Fact]
        public static void TestAccesingNestedJTreeArrayTryGetPropertyMethod()
        {
            var issues = new JTreeObject()
            {
                { "features", new JTreeArray { "new functionality 1", "new functionality 2" } },
            };

            Assert.True(issues.TryGetJsonArrayPropertyValue("features", out JTreeArray featuresArray));
            Assert.Equal("new functionality 1", ((JTreeString)featuresArray[0]).Value);
            Assert.Equal("new functionality 2", ((JTreeString)featuresArray[1]).Value);
        }

        [Fact]
        public static void TestInsert()
        {
            var jsonArray = new JTreeArray() { 1 };
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
            var mixedTypesArray = new JTreeArray { 1, "value", true };

            Assert.Equal(1, mixedTypesArray[0]);
            Assert.Equal("value", mixedTypesArray[1]);
            Assert.Equal(true, mixedTypesArray[2]);

            mixedTypesArray.Add(17);
            mixedTypesArray.Insert(4, "another");
            mixedTypesArray.Add(new JTreeNull());

            Assert.Equal(17, mixedTypesArray[3]);
            Assert.Equal("another", mixedTypesArray[4]);
            Assert.IsType<JTreeNull>(mixedTypesArray[5]);

        }

        [Fact]
        public static void TestOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new JTreeArray()[-1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => new JTreeArray()[-1] = new JTreeString());
            Assert.Throws<ArgumentOutOfRangeException>(() => new JTreeArray()[0]);
            Assert.Throws<ArgumentOutOfRangeException>(() => new JTreeArray()[0] = new JTreeString());
            Assert.Throws<ArgumentOutOfRangeException>(() => new JTreeArray()[1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => new JTreeArray()[1] = new JTreeString());
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var jsonArray = new JTreeArray { 1, 2, 3 };
                jsonArray.Insert(4, 17);
            });
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var jsonArray = new JTreeArray { 1, 2, 3 };
                jsonArray.Insert(-1, 17);
            });
        }

        [Fact]
        public static void TestIsReadOnly()
        {
            Assert.False(new JTreeArray().IsReadOnly);
        }

        [Fact]
        public static void TestClear()
        {
            var jsonArray = new JTreeArray { 1, 2, 3 };

            Assert.Equal(3, jsonArray.Count);
            Assert.Equal(1, ((JTreeNumber)jsonArray[0]).GetInt32());
            Assert.Equal(2, ((JTreeNumber)jsonArray[1]).GetInt32());
            Assert.Equal(3, ((JTreeNumber)jsonArray[2]).GetInt32());

            jsonArray.Clear();

            Assert.Equal(0, jsonArray.Count);
        }

        [Fact]
        public static void TestRemoveAll()
        {
            var jsonArray = new JTreeArray { 1, 2, 3 };

            Assert.Equal(3, jsonArray.Count);
            Assert.Equal(1, ((JTreeNumber)jsonArray[0]).GetInt32());
            Assert.Equal(2, ((JTreeNumber)jsonArray[1]).GetInt32());
            Assert.Equal(3, ((JTreeNumber)jsonArray[2]).GetInt32());

            jsonArray.RemoveAll(v => ((JTreeNumber)v).GetInt32() <= 2);

            Assert.Equal(1, jsonArray.Count);
            Assert.Equal(3, ((JTreeNumber)jsonArray[0]).GetInt32());
        }

        [Fact]
        public static void TestValueKind()
        {
            Assert.Equal(JsonValueKind.Array, new JTreeArray().ValueKind);
        }

        [Fact]
        public static void TestJTreeArrayIEnumerator()
        {
            var jsonArray = new JTreeArray() { 1, "value" };

            // Test generic IEnumerator:
            IEnumerator<JTreeNode> jsonArrayEnumerator = new JTreeArray.Enumerator(jsonArray);

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
            IEnumerator jsonArrayEnumerator2 = new JTreeArray.Enumerator(jsonArray);

            Assert.Null(jsonArrayEnumerator2.Current);

            jsonArrayEnumerator2.MoveNext();
            Assert.Equal((JTreeNumber)1, jsonArrayEnumerator2.Current);
            jsonArrayEnumerator2.MoveNext();
            Assert.Equal((JTreeString)"value", jsonArrayEnumerator2.Current);

            jsonArrayEnumerator2.Reset();

            jsonArrayEnumerator2.MoveNext();
            Assert.Equal((JTreeNumber)1, jsonArrayEnumerator2.Current);
            jsonArrayEnumerator2.MoveNext();
            Assert.Equal((JTreeString)"value", jsonArrayEnumerator2.Current);
        }

        [Fact]
        public static void TestGetJTreeArrayIEnumerable()
        {
            IEnumerable jsonArray = new JTreeArray() { 1, "value" };
            IEnumerator jsonArrayEnumerator = jsonArray.GetEnumerator();
            
            Assert.Null(jsonArrayEnumerator.Current);

            jsonArrayEnumerator.MoveNext();
            Assert.Equal((JTreeNumber)1, jsonArrayEnumerator.Current);
            jsonArrayEnumerator.MoveNext();
            Assert.Equal((JTreeString)"value", jsonArrayEnumerator.Current);
        }

        [Fact]
        public static void TestJTreeArrayEmptyArrayEnumerator()
        {
            var jsonArray = new JTreeArray();
            var jsonArrayEnumerator = new JTreeArray.Enumerator(jsonArray);

            Assert.Null(jsonArrayEnumerator.Current);
            Assert.False(jsonArrayEnumerator.MoveNext());
        }
    }
}
