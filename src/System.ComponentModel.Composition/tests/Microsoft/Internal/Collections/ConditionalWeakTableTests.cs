// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using System.Text;
using Xunit;

namespace Microsoft.Internal.Collections
{
    public class ConditionalWeakTableTests
    {
        [Fact]
        [ActiveIssue(123456789)]
        public void Add_KeyShouldBeCollected()
        {
            var obj = new object();
            var cwt = new ConditionalWeakTable<object, object>();

            cwt.Add(obj, new object());

            var wr = new WeakReference(obj);

            obj = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Assert.Null(wr.Target);

            GC.KeepAlive(cwt);
        }

        [Fact]
        public void Add_KeyHeld_ValueShouldNotBeCollected()
        {
            var obj = new object();
            var str = new StringBuilder();
            var cwt = new ConditionalWeakTable<object, StringBuilder>();

            var wrKey = new WeakReference(obj);
            var wrValue = new WeakReference(str);

            cwt.Add(obj, str);

            str = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            // Should still have both references
            Assert.NotNull(wrKey.Target);
            Assert.NotNull(wrValue.Target);

            GC.KeepAlive(obj);
            GC.KeepAlive(cwt);
        }

        [Fact]
        [ActiveIssue(123456789)]
        public void Add_KeyCollected_ValueShouldBeCollected()
        {
            var obj = new object();
            var str = new StringBuilder();
            var cwt = new ConditionalWeakTable<object, StringBuilder>();

            cwt.Add(obj, str);

            var wrKey = new WeakReference(obj);
            var wrValue = new WeakReference(str);
            str = null;
            obj = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Assert.Null(wrKey.Target);
            Assert.Null(wrValue.Target);

            GC.KeepAlive(cwt);
        }

        [Fact]
        public void Remove_ValidKey_ShouldReturnTrue()
        {
            var obj = new object();
            var obj2 = new object();
            var cwt = new ConditionalWeakTable<object, object>();

            cwt.Add(obj, obj2);

            Assert.True(cwt.Remove(obj));
        }

        [Fact]
        public void Remove_InvalidKey_ShouldReturnFalse()
        {
            var obj = new object();
            var obj2 = new object();
            var cwt = new ConditionalWeakTable<object, object>();

            cwt.Add(obj, obj2);

            Assert.False(cwt.Remove(obj2));
        }

        [Fact]
        public void TryGetValue_ValidKey_ShouldReturnTrueAndValue()
        {
            var obj = new object();
            var obj2 = new object();
            var cwt = new ConditionalWeakTable<object, object>();

            cwt.Add(obj, obj2);

            object obj3;
            Assert.True(cwt.TryGetValue(obj, out obj3), "Should find a value with the key!");
            Assert.Equal(obj2, obj3);
        }

        [Fact]
        public void TryGetValue_InvalidKey_ShouldReturnFalseAndNull()
        {
            var obj = new object();
            var obj2 = new object();
            var cwt = new ConditionalWeakTable<object, object>();

            cwt.Add(obj, obj2);

            object obj3;
            Assert.False(cwt.TryGetValue(obj2, out obj3), "Should NOT find a value with the key!");
            Assert.Null(obj3); 
        }
    }
}
