// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using System.Data.Common;
using Xunit;

namespace System.Data.Common
{
    public class DbProviderFactoriesTests
    {
        [Fact]
        [Trait("Issue", "I19826")]
        public void InitializationTest()
        {
            DataTable initializedTable = DbProviderFactories.GetFactoryClasses();
            Assert.NotNull(initializedTable);
            Assert.Equal(4, initializedTable.Columns.Count);
            Assert.Equal("Name", initializedTable.Columns[0].ColumnName);
            Assert.Equal("Description", initializedTable.Columns[1].ColumnName);
            Assert.Equal("InvariantName", initializedTable.Columns[2].ColumnName);
            Assert.Equal("AssemblyQualifiedName", initializedTable.Columns[3].ColumnName);
        }


        [Fact]
        [Trait("Issue", "I19826")]
        public void GetFactoryEmptyTableTest()
        {
            ClearDbProviderFactoriesTable();
            Assert.Throws<ArgumentException>(() => DbProviderFactories.GetFactory("System.Data.SqlClient"));
        }


        [Fact]
        [Trait("Issue", "I19826")]
        public void ConfigureFactoryWithTypeTest()
        {
            ClearDbProviderFactoriesTable();
            Assert.Throws<ArgumentException>(() => DbProviderFactories.GetFactory("System.Data.SqlClient"));
            DbProviderFactories.ConfigureFactory(typeof(System.Data.SqlClient.SqlClientFactory), "System.Data.SqlClient");
            DataTable providerTable = DbProviderFactories.GetFactoryClasses();
            Assert.Equal(1, providerTable.Rows.Count);
            DbProviderFactory factory = DbProviderFactories.GetFactory("System.Data.SqlClient");
            Assert.NotNull(factory);
            Assert.Equal(typeof(System.Data.SqlClient.SqlClientFactory), factory.GetType());
        }
        

        [Fact]
        [Trait("Issue", "I19826")]
        public void ConfigureFactoryWithDbConnectionTest()
        {
            ClearDbProviderFactoriesTable();
            Assert.Throws<ArgumentException>(() => DbProviderFactories.GetFactory("System.Data.SqlClient"));
            DbProviderFactories.ConfigureFactory(new System.Data.SqlClient.SqlConnection(), "System.Data.SqlClient");
            DataTable providerTable = DbProviderFactories.GetFactoryClasses();
            Assert.Equal(1, providerTable.Rows.Count);
            DbProviderFactory factory = DbProviderFactories.GetFactory("System.Data.SqlClient");
            Assert.NotNull(factory);
            Assert.Equal(typeof(System.Data.SqlClient.SqlClientFactory), factory.GetType());
        }


        private void ClearDbProviderFactoriesTable()
        {
            // as the DbProviderFactories table is shared, for tests we need a clean one before a test starts to make sure the tests always succeed. 
            Type type = typeof(DbProviderFactories);
            FieldInfo info = type.GetField("_providerTable", BindingFlags.NonPublic | BindingFlags.Static);
            DataTable providerTable = info.GetValue(null) as DataTable;
            Assert.NotNull(providerTable);
            providerTable.Clear();
            Assert.Equal(0, providerTable.Rows.Count);
        }
    }
}
