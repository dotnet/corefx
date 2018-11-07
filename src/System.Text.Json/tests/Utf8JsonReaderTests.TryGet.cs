// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Text.Json.Tests
{
    public static partial class Utf8JsonReaderTests
    {
        [Fact]
        public static void TestingNumbers()
        {
            var random = new Random(42);

            const int numberOfItems = 1_000;

            // Make sure we have 1_005 values in each numeric list.
            var ints = new List<int>
            {
                0,
                12345,
                -12345,
                int.MaxValue,
                int.MinValue
            };
            for (int i = 0; i < numberOfItems; i++)
            {
                int value = random.Next(int.MinValue, int.MaxValue);
                ints.Add(value);
            }

            var longs = new List<long>
            {
                0,
                12345678901,
                -12345678901,
                long.MaxValue,
                long.MinValue
            };
            for (int i = 0; i < numberOfItems; i++)
            {
                long value = random.Next(int.MinValue, int.MaxValue);
                if (value < 0)
                    value += int.MinValue;
                else
                    value += int.MaxValue;
                longs.Add(value);
            }

            var doubles = new List<double>
            {
                0.000,
                1.1234e1,
                -1.1234e1,
                1.79769313486231E+308,  // double.MaxValue doesn't round trip
                -1.79769313486231E+308  // double.MinValue doesn't round trip
            };
            for (int i = 0; i < numberOfItems / 2; i++)
            {
                double value = JsonTestHelper.NextDouble(random, double.MinValue / 10, double.MaxValue / 10);
                doubles.Add(value);
            }
            for (int i = 0; i < numberOfItems / 2; i++)
            {
                double value = JsonTestHelper.NextDouble(random, 1_000_000, -1_000_000);
                doubles.Add(value);
            }

            var floats = new List<float>
            {
                0.000f,
                1.1234e1f,
                -1.1234e1f,
                float.MaxValue,
                float.MinValue
            };
            for (int i = 0; i < numberOfItems; i++)
            {
                float value = JsonTestHelper.NextFloat(random);
                floats.Add(value);
            }

            var decimals = new List<decimal>
            {
                (decimal)0.000,
                (decimal)1.1234e1,
                (decimal)-1.1234e1,
                decimal.MaxValue,
                decimal.MinValue
            };
            for (int i = 0; i < numberOfItems / 2; i++)
            {
                decimal value = JsonTestHelper.NextDecimal(random, 78E14, -78E14);
                decimals.Add(value);
            }
            for (int i = 0; i < numberOfItems / 2; i++)
            {
                decimal value = JsonTestHelper.NextDecimal(random, 1_000_000, -1_000_000);
                decimals.Add(value);
            }

            var builder = new StringBuilder();
            builder.Append("{");

            for (int i = 0; i < ints.Count; i++)
            {
                builder.Append("\"int").Append(i).Append("\": ");
                builder.Append(ints[i]).Append(", ");
            }
            for (int i = 0; i < longs.Count; i++)
            {
                builder.Append("\"long").Append(i).Append("\": ");
                builder.Append(longs[i]).Append(", ");
            }
            for (int i = 0; i < doubles.Count; i++)
            {
                builder.Append("\"double").Append(i).Append("\": ");
                builder.Append(doubles[i]).Append(", ");
            }
            for (int i = 0; i < floats.Count; i++)
            {
                builder.Append("\"float").Append(i).Append("\": ");
                builder.Append(floats[i]).Append(", ");
            }
            for (int i = 0; i < decimals.Count; i++)
            {
                builder.Append("\"decimal").Append(i).Append("\": ");
                builder.Append(decimals[i]).Append(", ");
            }

            builder.Append("\"intEnd\": 0}");

            string jsonString = builder.ToString();
            byte[] dataUtf8 = Encoding.UTF8.GetBytes(jsonString);

            var json = new Utf8JsonReader(dataUtf8, true, default);
            string key = "";
            int count = 0;
            while (json.Read())
            {
                if (json.TokenType == JsonTokenType.PropertyName)
                {
                    Assert.True(json.TryGetValueAsString(out key));
                }
                if (json.TokenType == JsonTokenType.Number)
                {
                    if (key.StartsWith("int"))
                    {
                        Assert.True(json.TryGetValueAsInt32(out int numberInt));
                        if (count >= ints.Count)
                            count = 0;
                        Assert.Equal(ints[count], numberInt);
                        count++;
                    }
                    else if (key.StartsWith("long"))
                    {
                        Assert.True(json.TryGetValueAsInt64(out long numberLong));
                        if (count >= longs.Count)
                            count = 0;
                        Assert.Equal(longs[count], numberLong);
                        count++;
                    }
                    else if (key.StartsWith("float"))
                    {
                        Assert.True(json.TryGetValueAsSingle(out float numberFloat));
                        if (count >= floats.Count)
                            count = 0;

                        float expected = float.Parse(floats[count].ToString());

                        Assert.Equal(expected, numberFloat);
                        count++;
                    }
                    else if (key.StartsWith("double"))
                    {
                        Assert.True(json.TryGetValueAsDouble(out double numberDouble));
                        if (count >= doubles.Count)
                            count = 0;

                        double expected = double.Parse(doubles[count].ToString());

                        Assert.Equal(expected, numberDouble);
                        count++;
                    }
                    else if (key.StartsWith("decimal"))
                    {
                        Assert.True(json.TryGetValueAsDecimal(out decimal numberDecimal));
                        if (count >= decimals.Count)
                            count = 0;
                        Assert.Equal(decimals[count], numberDecimal);
                        count++;
                    }
                }
            }

            Assert.Equal(dataUtf8.Length, json.BytesConsumed);
            Assert.Equal(json.BytesConsumed, json.CurrentState.BytesConsumed);
        }

        [Theory]
        [InlineData("0.000", 0)]
        [InlineData("1e1", 10)]
        [InlineData("1.1e2", 110)]
        [InlineData("12345.1", 12345.1)]
        [InlineData("12345678901", 12345678901)]
        [InlineData("123456789012345678901", 123456789012345678901d)]
        public static void TestingNumbersInvalidCastToInt32(string jsonString, double expected)
        {
            byte[] dataUtf8 = Encoding.UTF8.GetBytes(jsonString);

            var json = new Utf8JsonReader(dataUtf8, true, default);
            while (json.Read())
            {
                if (json.TokenType == JsonTokenType.Number)
                {
                    Assert.False(json.TryGetValueAsInt32(out int value));
                    Assert.True(json.TryGetValueAsDouble(out double doubleValue));
                    Assert.Equal(expected, doubleValue);
                }
            }

            Assert.Equal(dataUtf8.Length, json.BytesConsumed);
            Assert.Equal(json.BytesConsumed, json.CurrentState.BytesConsumed);
        }

        [Theory]
        [InlineData("0.000", 0)]
        [InlineData("1e1", 10)]
        [InlineData("1.1e2", 110)]
        [InlineData("12345.1", 12345.1)]
        [InlineData("123456789012345678901", 123456789012345678901d)]
        public static void TestingNumbersInvalidCastToInt64(string jsonString, double expected)
        {
            byte[] dataUtf8 = Encoding.UTF8.GetBytes(jsonString);

            var json = new Utf8JsonReader(dataUtf8, true, default);
            while (json.Read())
            {
                if (json.TokenType == JsonTokenType.Number)
                {
                    Assert.False(json.TryGetValueAsInt64(out long value));
                    Assert.True(json.TryGetValueAsDouble(out double doubleValue));
                    Assert.Equal(expected, doubleValue);
                }
            }

            Assert.Equal(dataUtf8.Length, json.BytesConsumed);
            Assert.Equal(json.BytesConsumed, json.CurrentState.BytesConsumed);
        }

        [Fact]
        public static void TryGetInvalidCast()
        {
            string jsonString = "[\"stringValue\", true, 1234]";
            byte[] dataUtf8 = Encoding.UTF8.GetBytes(jsonString);

            var json = new Utf8JsonReader(dataUtf8, true, default);
            while (json.Read())
            {
                if (json.TokenType != JsonTokenType.String)
                {
                    try
                    {
                        json.TryGetValueAsString(out string value);
                        Assert.True(false, "Expected TryGetValueAsString to throw InvalidCastException due to mismatch token type.");
                    }
                    catch (InvalidCastException)
                    { }
                }

                if (json.TokenType != JsonTokenType.True && json.TokenType != JsonTokenType.False)
                {
                    try
                    {
                        json.TryGetValueAsBoolean(out bool value);
                        Assert.True(false, "Expected TryGetValueAsBoolean to throw InvalidCastException due to mismatch token type.");
                    }
                    catch (InvalidCastException)
                    { }
                }

                if (json.TokenType != JsonTokenType.Number)
                {
                    try
                    {
                        json.TryGetValueAsInt32(out int value);
                        Assert.True(false, "Expected TryGetValueAsInt32 to throw InvalidCastException due to mismatch token type.");
                    }
                    catch (InvalidCastException)
                    { }

                    try
                    {
                        json.TryGetValueAsInt64(out long value);
                        Assert.True(false, "Expected TryGetValueAsInt64 to throw InvalidCastException due to mismatch token type.");
                    }
                    catch (InvalidCastException)
                    { }

                    try
                    {
                        json.TryGetValueAsSingle(out float value);
                        Assert.True(false, "Expected TryGetValueAsSingle to throw InvalidCastException due to mismatch token type.");
                    }
                    catch (InvalidCastException)
                    { }

                    try
                    {
                        json.TryGetValueAsDouble(out double value);
                        Assert.True(false, "Expected TryGetValueAsDouble to throw InvalidCastException due to mismatch token type.");
                    }
                    catch (InvalidCastException)
                    { }

                    try
                    {
                        json.TryGetValueAsDecimal(out decimal value);
                        Assert.True(false, "Expected TryGetValueAsDecimal to throw InvalidCastException due to mismatch token type.");
                    }
                    catch (InvalidCastException)
                    { }
                }
            }

            Assert.Equal(dataUtf8.Length, json.BytesConsumed);
            Assert.Equal(json.BytesConsumed, json.CurrentState.BytesConsumed);
        }
    }
}
