﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Collections.Tests
{
    public class CollectionExtensionsTests
    {
        [Fact]
        public void GetValueOrDefault_KeyExistsInIDictionary_ReturnsValue()
        {
            IDictionary<string, string> dictionary = new SortedDictionary<string, string>() { { "key", "value" } };
            Assert.Equal("value", dictionary.GetValueOrDefault("key"));
            Assert.Equal("value", dictionary.GetValueOrDefault("key", null));
        }

        [Fact]
        public void GetValueOrDefault_KeyDoesntExistInIDictionary_ReturnsDefaultValue()
        {
            IDictionary<string, string> dictionary = new SortedDictionary<string, string>() { { "key", "value" } };
            Assert.Null(dictionary.GetValueOrDefault("anotherKey"));
            Assert.Equal("anotherValue", dictionary.GetValueOrDefault("anotherKey", "anotherValue"));
        }

        [Fact]
        public void GetValueOrDefault_NullKeyIDictionary_ThrowsArgumentNullException()
        {
            IDictionary<string, string> dictionary = new SortedDictionary<string, string>() { { "key", "value" } };
            Assert.Throws<ArgumentNullException>("key", () => dictionary.GetValueOrDefault(null));
            Assert.Throws<ArgumentNullException>("key", () => dictionary.GetValueOrDefault(null, "anotherValue"));
        }

        [Fact]
        public void GetValueOrDefault_NullIDictionary_ThrowsArgumentNullException()
        {
            IDictionary<string, string> dictionary = null;
            Assert.Throws<ArgumentNullException>("dictionary", () => dictionary.GetValueOrDefault("key"));
            Assert.Throws<ArgumentNullException>("dictionary", () => dictionary.GetValueOrDefault("key", "value"));
        }

        [Fact]
        public void GetValueOrDefault_KeyExistsInIReadOnlyDictionary_ReturnsValue()
        {
            IReadOnlyDictionary<string, string> dictionary = new SortedDictionary<string, string>() { { "key", "value" } };
            Assert.Equal("value", dictionary.GetValueOrDefault("key"));
            Assert.Equal("value", dictionary.GetValueOrDefault("key", null));
        }

        [Fact]
        public void GetValueOrDefault_KeyDoesntExistInIReadOnlyDictionary_ReturnsDefaultValue()
        {
            IReadOnlyDictionary<string, string> dictionary = new SortedDictionary<string, string>() { { "key", "value" } };
            Assert.Null(dictionary.GetValueOrDefault("anotherKey"));
            Assert.Equal("anotherValue", dictionary.GetValueOrDefault("anotherKey", "anotherValue"));
        }

        [Fact]
        public void GetValueOrDefault_NullKeyIReadOnlyDictionary_ThrowsArgumentNullException()
        {
            IReadOnlyDictionary<string, string> dictionary = new SortedDictionary<string, string>() { { "key", "value" } };
            Assert.Throws<ArgumentNullException>("key", () => dictionary.GetValueOrDefault(null));
            Assert.Throws<ArgumentNullException>("key", () => dictionary.GetValueOrDefault(null, "anotherValue"));
        }

        [Fact]
        public void GetValueOrDefault_NullIReadOnlyDictionary_ThrowsArgumentNullException()
        {
            IReadOnlyDictionary<string, string> dictionary = null;
            Assert.Throws<ArgumentNullException>("dictionary", () => dictionary.GetValueOrDefault("key"));
            Assert.Throws<ArgumentNullException>("dictionary", () => dictionary.GetValueOrDefault("key", "value"));
        }

        [Fact]
        public void TryAdd_NullIDictionary_ThrowsArgumentNullException()
        {
            IDictionary<string, string> dictionary = null;
            Assert.Throws<ArgumentNullException>("dictionary", () => dictionary.TryAdd("key", "value"));
        }

        [Fact]
        public void TryAdd_NullKeyIDictionary_ThrowsArgumenNullException()
        {
            IDictionary<string, string> dictionary = new SortedDictionary<string, string>() { { "key", "value" } };
            Assert.Throws<ArgumentNullException>("key", () => dictionary.TryAdd(null, "anotherValue"));
        }

        [Fact]
        public void TryAdd_KeyDoesntExistInIDictionary_ReturnsTrue()
        {
            IDictionary<string, string> dictionary = new SortedDictionary<string, string>();
            Assert.Equal(true, dictionary.TryAdd("key", "value"));
            Assert.Equal("value", dictionary["key"]);
        }

        [Fact]
        public void TryAdd_KeyExistsInIDictionary_ReturnsFalse()
        {
            IDictionary<string, string> dictionary = new SortedDictionary<string, string>() { ["key"] = "value" };
            Assert.Equal(false, dictionary.TryAdd("key", "value2"));
            Assert.Equal("value", dictionary["key"]);
        }

        [Fact]
        public void Remove_NullIDictionary_ThrowsArgumentNullException()
        {
            IDictionary<string, string> dictionary = null;
            Assert.Throws<ArgumentNullException>("dictionary", () => dictionary.Remove("key", out var value));
        }

        [Fact]
        public void Remove_NullKeyIDictionary_ThrowsArgumenNullException()
        {
            IDictionary<string, string> dictionary = new SortedDictionary<string, string>() { { "key", "value" } };
            Assert.Throws<ArgumentNullException>("key", () => dictionary.Remove(null, out var value));
        }

        [Fact]
        public void Remove_KeyExistsInIDictionary_ReturnsTrue()
        {
            IDictionary<string, string> dictionary = new SortedDictionary<string, string>() { ["key"] = "value" };
            Assert.Equal(true, dictionary.Remove("key", out var value));
            Assert.Equal("value", value);
            Assert.Throws<KeyNotFoundException>(() => dictionary["key"]);
        }

        [Fact]
        public void Remove_KeyDoesntExistInIDictionary_ReturnsFalse()
        {
            IDictionary<string, string> dictionary = new SortedDictionary<string, string>();
            Assert.Equal(false, dictionary.Remove("key", out var value));
            Assert.Equal(default(string), value);
        }
    }
}
