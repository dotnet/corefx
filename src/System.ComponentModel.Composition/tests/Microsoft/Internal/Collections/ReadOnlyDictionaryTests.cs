// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xunit;

namespace Microsoft.Internal.Collections
{
    public class ReadOnlyDictionaryTests
    {
        [Fact]
        public void Constructor_WritableDictionaryAsDictionaryArgument_ShouldPopulateCollection()
        {
            var dictionary = GetWritableDictionaryWithData();
            var readOnlyDictionary = new ReadOnlyDictionary<string, object>(dictionary);

            Assert.Equal(dictionary, readOnlyDictionary);
        }

        [Fact]
        public void Add1_ShouldThrowNotSupported()
        {
            var dictionary = GetReadOnlyDictionaryWithData();

            Assert.Throws<NotSupportedException>(() =>
            {
                dictionary.Add(new KeyValuePair<string, object>("Key", "Value"));
            });
        }

        [Fact]
        public void Add2_ShouldThrowNotSupported()
        {
            var dictionary = GetReadOnlyDictionaryWithData();

            Assert.Throws<NotSupportedException>(() =>
            {
                dictionary.Add("Key", "Value");
            });
        }

        [Fact]
        public void Clear_ShouldThrowNotSupported()
        {
            var dictionary = GetReadOnlyDictionaryWithData();

            Assert.Throws<NotSupportedException>(() =>
            {
                dictionary.Clear();
            });
        }

        [Fact]
        public void Remove1_ShouldThrowNotSupported()
        {
            var dictionary = GetReadOnlyDictionaryWithData();

            Assert.Throws<NotSupportedException>(() =>
            {
                dictionary.Remove("Value");
            });
        }

        [Fact]
        public void Remove2_ShouldThrowNotSupported()
        {
            var dictionary = GetReadOnlyDictionaryWithData();

            Assert.Throws<NotSupportedException>(() =>
            {
                dictionary.Remove(new KeyValuePair<string, object>("Key", "Value"));
            });
        }

        [Fact]
        public void ItemSet_ShouldThrowNotSupported()
        {
            var dictionary = GetReadOnlyDictionaryWithData();

            Assert.Throws<NotSupportedException>(() =>
            {
                dictionary["Key"] = "Value";
            });
        }

        [Fact]
        public void Keys_ShouldReturnWrappedDictionaryKeys()
        {
            var dictionary = GetWritableDictionaryWithData();
            var readOnlyDictionary = GetReadOnlyDictionary(dictionary);

            Assert.Equal(readOnlyDictionary.Keys, dictionary.Keys);
        }

        [Fact]
        public void Values_ShouldReturnWrappedDictionaryValues()
        {
            var dictionary = GetWritableDictionaryWithData();
            var readOnlyDictionary = GetReadOnlyDictionary(dictionary);

            Assert.Equal(readOnlyDictionary.Values, readOnlyDictionary.Values);
        }

        [Fact]
        public void IsReadOnly_ShouldAlwaysBeTrue()
        {
            var dictionary = GetWritableDictionaryWithData();
            var readOnlyDictionary = GetReadOnlyDictionary(dictionary);
            
            Assert.False(dictionary.IsReadOnly);
            Assert.True(readOnlyDictionary.IsReadOnly);
        }

        [Fact]
        public void Count_ShouldReturnWrappedDictionaryCount()
        {
            var dictionary = GetWritableDictionaryWithData();
            var readOnlyDictionary = GetReadOnlyDictionary(dictionary);

            Assert.Equal(dictionary.Count, readOnlyDictionary.Count);
        }

        [Fact]
        public void ContainsKey()
        {
            var dictionary = GetWritableDictionaryWithData();
            var readOnlyDictionary = GetReadOnlyDictionary(dictionary);

            Assert.True(readOnlyDictionary.ContainsKey("Key1"));
            Assert.False(readOnlyDictionary.ContainsKey("InvalidKey"));
        }

        [Fact]
        public void Contains()
        {
            var dictionary = GetWritableDictionaryWithData();
            var readOnlyDictionary = GetReadOnlyDictionary(dictionary);

            Assert.True(readOnlyDictionary.Contains(new KeyValuePair<string,object>("Key1", "Value1")));
            Assert.False(readOnlyDictionary.Contains(new KeyValuePair<string,object>("InvalidKey", "Value1")));
            Assert.False(readOnlyDictionary.Contains(new KeyValuePair<string,object>("Key1", "InvalidValue")));
        }

        [Fact]
        public void CopyTo()
        {
            var dictionary = GetWritableDictionaryWithData();
            var readOnlyDictionary = GetReadOnlyDictionary(dictionary);
            KeyValuePair<string, object>[] destination = new KeyValuePair<string, object> [readOnlyDictionary.Count];
            readOnlyDictionary.CopyTo(destination, 0);
            Assert.Equal(readOnlyDictionary, destination);
        }

        [Fact]
        public void GetEnumerator()
        {
            var dictionary = GetWritableDictionaryWithData();
            var readOnlyDictionary = GetReadOnlyDictionary(dictionary);
            IEnumerable<KeyValuePair<string, object>> genericEnumerable = readOnlyDictionary;
            Assert.Equal(genericEnumerable, dictionary);
            IEnumerable weakEnumerable = (IEnumerable)readOnlyDictionary;
            Assert.Equal(weakEnumerable, dictionary);
        }

        [Fact]
        public void Item()
        {
            var dictionary = GetWritableDictionaryWithData();
            var readOnlyDictionary = GetReadOnlyDictionary(dictionary);
            Assert.Equal("Value1", readOnlyDictionary["Key1"]); // "Expecting to read wrapped value");
        }

        [Fact]
        public void TryGetValue()
        {
            var dictionary = GetWritableDictionaryWithData();
            var readOnlyDictionary = GetReadOnlyDictionary(dictionary);
            object result;
            bool ret = readOnlyDictionary.TryGetValue("Key1", out result);
            Assert.True(ret, "Expecting TryGetExportedValue to return true for wrapped key");
            Assert.Equal("Value1", result); // "Expecting TryGetExportedValue to return wrapped value");
        }

        private static IDictionary<String, object> GetReadOnlyDictionaryWithData()
        {
            return GetReadOnlyDictionary(GetWritableDictionaryWithData());
        }

        private static IDictionary<TKey, TValue> GetReadOnlyDictionary<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
        {
            return new ReadOnlyDictionary<TKey, TValue>(dictionary);
        }

        private static IDictionary<String, object> GetWritableDictionaryWithData()
        {
            IDictionary<String, object> dictionary = new Dictionary<String, object>();
            dictionary.Add("Key1", "Value1");
            dictionary.Add("Key2", 42);
            return dictionary;
        }
    }
}
