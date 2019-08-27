// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Text.Json.Tests
{
    public static class JsonNullTests
    {
        [Fact]
        public static void TestJsonNull()
        {
            var jsonObject = new JsonObject
            {
                { "null1", null },
                { "null2", (JsonNode)null },
                { "null3", (JsonNull)null },
                { "null4", new JsonNull() },
                { "null5", (string)null },
            };

            Assert.IsType<JsonNull>(jsonObject["null1"]);
            Assert.IsType<JsonNull>(jsonObject["null2"]);
            Assert.IsType<JsonNull>(jsonObject["null3"]);
            Assert.IsType<JsonNull>(jsonObject["null4"]);
            Assert.IsType<JsonNull>(jsonObject["null5"]);
        }
    }
}
