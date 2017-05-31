// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Data.Common;

namespace System.Data.SqlClient.ManualTesting.Tests
{
    public static class SqlSchemaInfoTest
    {
        [CheckConnStrSetupFact]
        public static void TestSqlSchemaInfo()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(DataTestUtility.TcpConnStr);

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();

                TestGetSchema(connection);
                TestCommandBuilder(connection);
                TestInitialCatalogStandardValues(connection);

                connection.Close();
            }
        }

        public static void TestGetSchema(SqlConnection connection)
        {
            DataTable dataTable = connection.GetSchema("DATABASES");

            if (dataTable.Rows.Count > 0)
            {
                DataTable metaDataCollections = connection.GetSchema(DbMetaDataCollectionNames.MetaDataCollections);
                DataTable metaDataSourceInfo = connection.GetSchema(DbMetaDataCollectionNames.DataSourceInformation);
                DataTable metaDataTypes = connection.GetSchema(DbMetaDataCollectionNames.DataTypes);
            }
        }

        public static void TestCommandBuilder(SqlConnection connection)
        {
            // CommandBuilder is not supported yet in .NET Core.
        }

        public static void TestInitialCatalogStandardValues(SqlConnection connection)
        {
            // PropertyDescriptor is not supported yet in .NET Core.
        }
    }
}
