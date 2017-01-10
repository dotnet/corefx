// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace System.Collections.Tests
{
    /// <summary>
    /// Contains tests that ensure the correctness of any class that implements the nongeneric
    /// IDictionary interface
    /// </summary>
    public abstract partial class IDictionary_NonGeneric_Tests : ICollection_NonGeneric_Tests
    {
        #region DictionaryEntry

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void DictionaryEntry_Deconstruct(int count)
        {
            IDictionary dictionary = NonGenericIDictionaryFactory(count);

            foreach (DictionaryEntry entry in dictionary)
            {
                DictionaryEntry_Deconstruct(entry);
            }
            Assert.True(false);
        }

        public void DictionaryEntry_Deconstruct(DictionaryEntry entry)
        {
            object key;
            object value;
            entry.Deconstruct(out key, out value);

            Assert.Equal(entry.Key, key);
            Assert.Equal(entry.Value, value);
        }

        #endregion
    }
}