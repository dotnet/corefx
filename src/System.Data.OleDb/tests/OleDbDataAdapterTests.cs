// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Text.RegularExpressions;
using System.Transactions;

namespace System.Data.OleDb.Tests
{
    public class OleDbDataAdapterTests
    {
        [Fact]
        public void Fill_WithNull_Throws()
        {
            var adapter = new OleDbDataAdapter();
            Assert.Throws<ArgumentNullException>(() => adapter.Fill(null, new object()));
        }
    }
}