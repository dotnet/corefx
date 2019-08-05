// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.VisualBasic.MyServices.Internal;
using System;
using System.Threading;
using Xunit;

namespace Microsoft.VisualBasic.MyServices.Internal.Tests
{
    public class ContextValueTests
    {
        [Fact]
        public void NoValue()
        {
            Assert.Null((new ContextValue<string>()).Value);
            Assert.Throws<NullReferenceException>(() => (new ContextValue<int>()).Value);
        }

        [Fact]
        public void MultipleInstances()
        {
            var context1 = new ContextValue<int>();
            context1.Value = 1;
            var context2 = new ContextValue<int>();
            context2.Value = 2;
            Assert.Equal(1, context1.Value);
            Assert.Equal(2, context2.Value);
        }

        [Fact]
        public void MultipleThreads()
        {
            var context = new ContextValue<string>();
            context.Value = "Hello";
            var thread = new Thread(() =>
            {
                Assert.Null(context.Value);
                context.Value = "World";
                Assert.Equal("World", context.Value);
            });
            thread.Start();
            thread.Join();
            Assert.Equal("Hello", context.Value);
        }
    }
}
