// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace Microsoft.Internal.Collections
{
    public class WeakReferenceCollectionTests
    {
        [Fact]
        [ActiveIssue(123456789)]
        public void Add_ObjectShouldGetCollected()
        {
            var obj = new object();
            var wrc = new WeakReferenceCollection<object>();

            wrc.Add(obj);

            var wr = new WeakReference(obj);
            obj = null;

            Assert.NotNull(wr.Target);

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Assert.Null(wr.Target);

            GC.KeepAlive(wrc);
        }

        [Fact]
        public void Remove_ObjectShouldGetRemoved()
        {
            var obj = new object();
            var wrc = new WeakReferenceCollection<object>();

            wrc.Add(obj);

            Assert.Equal(1, wrc.AliveItemsToList().Count);
            
            wrc.Remove(obj);

            Assert.Equal(0, wrc.AliveItemsToList().Count);
        }

        [Fact]
        public void AliveItemsToList_ShouldReturnAllItems()
        {
            var list = new object[] {new object(), new object(), new object()};
            var wrc = new WeakReferenceCollection<object>();

            foreach (object obj in list)
            {
                wrc.Add(obj);
            }

            Assert.Equal(list.Length, wrc.AliveItemsToList().Count);
        }

        [Fact]
        [ActiveIssue(123456789)]
        public void AliveItemsToList_ShouldReturnAllAliveItems()
        {
            var list = new object[] { new object(), new object(), new object() };
            var wrc = new WeakReferenceCollection<object>();

            var obj1 = new object();
            wrc.Add(obj1);

            foreach (object obj in list)
            {
                wrc.Add(obj);
            }

            var obj2 = new object();
            wrc.Add(obj2);

            Assert.Equal(list.Length + 2, wrc.AliveItemsToList().Count);

            obj1 = obj2 = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            var aliveItems = wrc.AliveItemsToList();
            Assert.Equal(list.Length, aliveItems.Count);

            Assert.Equal(list[0], aliveItems[0]);
            Assert.Equal(list[1], aliveItems[1]);
            Assert.Equal(list[2], aliveItems[2]);
        }

        [Fact]
        public void AliveItemsToList_ShouldReturnEmpty()
        {
            var wrc = new WeakReferenceCollection<object>();
            Assert.Equal(0, wrc.AliveItemsToList().Count);
        }
    }
}
