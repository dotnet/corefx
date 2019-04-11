﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Text.Json.Serialization.Tests
{
    public static partial class NullTests
    {
        [Fact]
        public static void DefaultWriteOptions()
        {
            var input = new TestClassWithNull();
            string json = JsonSerializer.ToString(input);
            Assert.Equal(@"{""MyString"":null}", json);
        }

        [Fact]
        public static void OverrideWriteOnOption()
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.IgnoreNullPropertyValueOnWrite = true;

            var input = new TestClassWithNull();
            string json = JsonSerializer.ToString(input, options);
            Assert.Equal(@"{}", json);
        }

        [Fact]
        public static void NullReferences()
        {
            var obj = new ObjectWithObjectProperties();
            obj.Address = null;
            obj.Array = null;
            obj.List = null;
            obj.NullableInt = null;
            obj.NullableIntArray = null;
            obj.Object = null;

            string json = JsonSerializer.ToString(obj);
            Assert.Contains(@"""Address"":null", json);
            Assert.Contains(@"""List"":null", json);
            Assert.Contains(@"""Array"":null", json);
            Assert.Contains(@"""NullableInt"":null", json);
            Assert.Contains(@"""Object"":null", json);
            Assert.Contains(@"""NullableIntArray"":null", json);
        }

        [Fact]
        public static void NullArrayElement()
        {
            string json = JsonSerializer.ToString(new ObjectWithObjectProperties[]{ null });
            Assert.Equal("[null]", json);
        }

        [Fact]
        public static void NullArgumentFail()
        {
            Assert.Throws<ArgumentNullException>(() => JsonSerializer.ToString("", (Type)null));
        }

        [Fact]
        public static void NullObjectOutput()
        {
            {
                string output = JsonSerializer.ToString<string>(null);
                Assert.Equal("null", output);
            }

            {
                string output = JsonSerializer.ToString<string>(null, null);
                Assert.Equal("null", output);
            }
        }
    }
}
