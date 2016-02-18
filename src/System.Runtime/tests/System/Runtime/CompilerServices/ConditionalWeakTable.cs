// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Runtime.CompilerServices.Tests
{
    public class ConditionalWeakTableTests
    {
        [Fact]
        public static void AddTest()
        {
            var cwt = new ConditionalWeakTable<object, object>();
            var key = new object();
            var value = new object();
            object obj = null;

            cwt.Add(key, value);

            Assert.True(cwt.TryGetValue(key, out value));
            Assert.Equal(value, cwt.GetOrCreateValue(key));
            Assert.Equal(value, cwt.GetValue(key, k => new object()));

            var wrValue = new WeakReference<object>(value, false);
            var wrkey = new WeakReference<object>(key, false);
            key = null;
            value = null;

            GC.Collect();

            // key and value must be collected
            Assert.False(wrValue.TryGetTarget(out obj));
            Assert.False(wrkey.TryGetTarget(out obj));
        }

        [Fact]
        public static void GetOrCreateValueTest()
        {
            var cwt = new ConditionalWeakTable<object, object>();
            var key = new object();
            object obj = null;

            object value = cwt.GetOrCreateValue(key);

            Assert.True(cwt.TryGetValue(key, out value));
            Assert.Equal(value, cwt.GetValue(key, k => new object()));

            var wrValue = new WeakReference<object>(value, false);
            var wrkey = new WeakReference<object>(key, false);
            key = null;
            value = null;

            GC.Collect();

            // key and value must be collected
            Assert.False(wrValue.TryGetTarget(out obj));
            Assert.False(wrkey.TryGetTarget(out obj));
        }

        [Fact]
        public static void GetValueTest()
        {
            var cwt = new ConditionalWeakTable<object, object>();
            var key = new object();
            object obj = null;

            object value = cwt.GetValue(key, k => new object());

            Assert.True(cwt.TryGetValue(key, out value));
            Assert.Equal(value, cwt.GetOrCreateValue(key));

            var wrValue = new WeakReference<object>(value, false);
            var wrkey = new WeakReference<object>(key, false);
            key = null;
            value = null;

            GC.Collect();

            // key and value must be collected
            Assert.False(wrValue.TryGetTarget(out obj));
            Assert.False(wrkey.TryGetTarget(out obj));
        }
    }
}
