// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit;

namespace System
{
    public class UriBuilderTests
    {
        [Fact]
        public static void TestQuery()
        {
            UriBuilder uriBuilder = new UriBuilder(@"http://foo/bar/baz?date=today");

            string q;

            q = uriBuilder.Query;
            Assert.Equal<String>(q, @"?date=today");

            uriBuilder.Query = @"?date=yesterday";
            q = uriBuilder.Query;
            Assert.Equal<String>(q, @"?date=yesterday");

            uriBuilder.Query = @"date=tomorrow";
            q = uriBuilder.Query;
            Assert.Equal<String>(q, @"?date=tomorrow");

            uriBuilder.Query += @"&place=redmond";
            q = uriBuilder.Query;
            Assert.Equal<String>(q, @"?date=tomorrow&place=redmond");

            uriBuilder.Query = null;
            q = uriBuilder.Query;
            Assert.Equal<String>(q, @"");

            uriBuilder.Query = "";
            q = uriBuilder.Query;
            Assert.Equal<String>(q, @"");

            uriBuilder.Query = "?";
            q = uriBuilder.Query;
            Assert.Equal<String>(q, @"?");
        }
    }
}
