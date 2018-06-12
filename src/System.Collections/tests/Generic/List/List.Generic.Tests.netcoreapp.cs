// Licensed to the .NET Foundation under one or more agreements.
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
        #region ItemRef

        protected void VerifyList(List<T> list, List<T> expectedItems)
        {
            Assert.Equal(expectedItems.Count, list.Count);

            //Only verify the indexer. List should be in a good enough state that we
            //do not have to verify consistency with any other method.
            for (int i = 0; i < list.Count; ++i)
            {
                Assert.True(list[i] == null ? expectedItems[i] == null : list[i].Equals(expectedItems[i]));
                Assert.True(list[i] == null ? expectedItems.ItemRef(i) == null : list[i].Equals(expectedItems.ItemRef(i)));
                Assert.True(list.ItemRef(i) == null ? expectedItems[i] == null : list.ItemRef(i).Equals(expectedItems[i]));
            }
        }

        #endregion
    }
}
