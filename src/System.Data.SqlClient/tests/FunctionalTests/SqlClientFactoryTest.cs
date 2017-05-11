// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Data.SqlClient.Tests
{
    public class SqlClientFactoryTest
    {
        [Fact]
        public void Instance_NotNullSame()
        {
            SqlClientFactory instance = SqlClientFactory.Instance;
            Assert.NotNull(instance);
            Assert.Same(instance, SqlClientFactory.Instance);
        }

        [Fact]
        public void CreateCommand_NotNull()
        {
            Assert.NotNull(SqlClientFactory.Instance.CreateCommand());
        }

        [Fact]
        public void CreateConnection_NotNull()
        {
            Assert.NotNull(SqlClientFactory.Instance.CreateConnection());
        }

        [Fact]
        public void CreateConnectionStringBuilder_NotNull()
        {
            Assert.NotNull(SqlClientFactory.Instance.CreateConnectionStringBuilder());
        }

        [Fact]
        public void CreateDataAdapter_NotNull()
        {
            Assert.NotNull(SqlClientFactory.Instance.CreateDataAdapter());
        }

        [Fact]
        public void CreateParameter_NotNull()
        {
            Assert.NotNull(SqlClientFactory.Instance.CreateParameter());
        }
    }
}
