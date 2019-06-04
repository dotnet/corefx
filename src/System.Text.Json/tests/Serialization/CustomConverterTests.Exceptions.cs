// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Text.Json.Serialization.Tests
{
    public static partial class CustomConverterTests
    {
        public static void ConverterFailJsonException<TException>() where TException: Exception, new()
        {
            var options = new JsonSerializerOptions();
            JsonConverter converter = new FailConverter<TException>();
            options.Converters.Add(converter);

            try
            {
                JsonSerializer.Parse<int>("0", options);
                Assert.True(false, "Expected exception");
            }
            catch (JsonException ex)
            {
                Assert.NotNull(ex.InnerException);
                Assert.IsType<TException>(ex.InnerException);
            }

            try
            {
                JsonSerializer.Parse<int[]>("[0]", options);
                Assert.True(false, "Expected exception");
            }
            catch (JsonException ex)
            {
                Assert.NotNull(ex.InnerException);
                Assert.IsType<TException>(ex.InnerException);
            }

            try
            {
                JsonSerializer.ToString(0, options);
                Assert.True(false, "Expected exception");
            }
            catch (JsonException ex)
            {
                Assert.NotNull(ex.InnerException);
                Assert.IsType<TException>(ex.InnerException);
            }

            try
            {
                JsonSerializer.ToString(new int[] { 0 }, options);
                Assert.True(false, "Expected exception");
            }
            catch (JsonException ex)
            {
                Assert.NotNull(ex.InnerException);
                Assert.IsType<TException>(ex.InnerException);
            }

            try
            {
                var obj = new Dictionary<string, int>();
                obj["key"] = 0;

                JsonSerializer.ToString(obj, options);
                Assert.True(false, "Expected exception");
            }
            catch (JsonException ex)
            {
                Assert.NotNull(ex.InnerException);
                Assert.IsType<TException>(ex.InnerException);
            }
        }

        private class FailConverter<TException> : JsonConverter<int> where TException: Exception, new()
        {
            public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                throw new TException();
            }

            public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
            {
                throw new TException();
            }

            public override void Write(Utf8JsonWriter writer, int value, JsonEncodedText propertyName, JsonSerializerOptions options)
            {
                throw new TException();
            }
        }

        [Fact]
        public static void ConverterExceptionsRethrownAsJsonExceptionFail()
        {
            ConverterFailJsonException<ArgumentException>();
            ConverterFailJsonException<FormatException>();
            ConverterFailJsonException<InvalidOperationException>();
            ConverterFailJsonException<OverflowException>();
        }

        public static void ConverterFailNoRethrow<TException>() where TException : Exception, new()
        {
            var options = new JsonSerializerOptions();
            JsonConverter converter = new FailConverter<TException>();
            options.Converters.Add(converter);

            Assert.Throws<TException>(() => JsonSerializer.Parse<int>("0", options));
            Assert.Throws<TException>(() => JsonSerializer.Parse<int[]>("[0]", options));
            Assert.Throws<TException>(() => JsonSerializer.ToString(0, options));
            Assert.Throws<TException>(() => JsonSerializer.ToString(new int[] { 0 }, options));

            var obj = new Dictionary<string, int>();
            obj["key"] = 0;

            Assert.Throws<TException>(() => JsonSerializer.ToString(obj, options));
        }

        [Fact]
        public static void ConverterExceptionsNotRethrownFail()
        {
            ConverterFailNoRethrow<Exception>();

            // Some misc exceptions not re-thrown as JsonException:
            ConverterFailNoRethrow<SystemException>();
            ConverterFailNoRethrow<StackOverflowException>();
            ConverterFailNoRethrow<IndexOutOfRangeException>();
            ConverterFailNoRethrow<NotSupportedException>();
        }
    }
}
