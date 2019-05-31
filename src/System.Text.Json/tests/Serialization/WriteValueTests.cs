// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using Xunit;

namespace System.Text.Json.Serialization.Tests
{
    public static partial class WriteValueTests
    {
        [Fact]
        public static void WriteInvalid()
        {
            {
                var abw = new ArrayBufferWriter<byte>();
                var writer = new Utf8JsonWriter(abw);

                JsonSerializer.WriteValue(writer, 1, typeof(int));
                Assert.Throws<InvalidOperationException>(() => JsonSerializer.WriteValue(writer, 1, typeof(int)));
                Assert.Throws<InvalidOperationException>(() => JsonSerializer.WriteValue(writer, 1));
            }

            {
                var abw = new ArrayBufferWriter<byte>();
                var writer = new Utf8JsonWriter(abw);

                writer.WriteStartObject();
                Assert.Throws<InvalidOperationException>(() => JsonSerializer.WriteValue(writer, 1, typeof(int)));
                Assert.Throws<InvalidOperationException>(() => JsonSerializer.WriteValue(writer, 1));
            }

            {
                var abw = new ArrayBufferWriter<byte>();
                var writer = new Utf8JsonWriter(abw);

                writer.WriteStartArray();
                writer.WriteEndArray();
                Assert.Throws<InvalidOperationException>(() => JsonSerializer.WriteValue(writer, 1, typeof(int)));
                Assert.Throws<InvalidOperationException>(() => JsonSerializer.WriteValue(writer, 1));
            }

            {
                var abw = new ArrayBufferWriter<byte>();
                var writer = new Utf8JsonWriter(abw);

                writer.WriteStartObject();
                writer.WriteEndObject();
                Assert.Throws<InvalidOperationException>(() => JsonSerializer.WriteValue(writer, 1, typeof(int)));
                Assert.Throws<InvalidOperationException>(() => JsonSerializer.WriteValue(writer, 1));
            }
        }
    }
}
