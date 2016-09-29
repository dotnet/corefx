﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Collections.Tests
{
    /// <summary>
    /// Contains tests that ensure the correctness of the List class.
    /// </summary>
    public abstract partial class List_Generic_Tests<T> : IList_Generic_Tests<T>
    {
        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void ForEach_Verify(int count)
        {
            List<T> list = GenericListFactory(count);
            List<T> visitedItems = new List<T>();
            Action<T> action = delegate (T item) { visitedItems.Add(item); };

            //[] Verify ForEach looks at every item
            visitedItems.Clear();
            list.ForEach(action);
            VerifyList(list, visitedItems);
        }

        [Fact]
        public void ForEach_NullAction_ThrowsArgumentNullException()
        {
            List<T> list = GenericListFactory();
            Assert.Throws<ArgumentNullException>(() => list.ForEach(null));
        }
    }
}
