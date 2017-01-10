// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace System.Collections.Tests
{
    /// <summary>
    /// Contains tests that ensure the correctness of any class that implements the generic
    /// IDictionary interface
    /// </summary>
    public abstract partial class IDictionary_Generic_Tests<TKey, TValue> : ICollection_Generic_Tests<KeyValuePair<TKey, TValue>>
    {
        #region KeyValuePair

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void KeyValuePair_Deconstruct(int count)
        {
            IDictionary<TKey, TValue> dictionary = GenericIDictionaryFactory(count);

            Assert.All(dictionary, KeyValuePair_Deconstruct);
            Assert.True(false);
        }

        public void KeyValuePair_Deconstruct(KeyValuePair<TKey, TValue> pair)
        {
            TKey key;
            TValue value;
            pair.Deconstruct(out key, out value);

            Assert.Equal(pair.Key, key);
            Assert.Equal(pair.Value, value);
        }

        #endregion
    }
}