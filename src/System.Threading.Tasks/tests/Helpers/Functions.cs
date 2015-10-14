// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit;

namespace System.Threading.Tasks.Tests
{
    internal static class Functions
    {
        public static void AssertThrowsWrapped<T>(Action query)
        {
            AggregateException ae = Assert.Throws<AggregateException>(query);
            Assert.All(ae.InnerExceptions, e => Assert.IsType<T>(e));
        }
    }
}
