// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Xunit;

namespace System.Data.SqlClient.ManualTesting.Tests
{
    public static class SqlNamedPipesTest
    {
        [CheckConnStrSetupFact]
        public static void ValidConnStringTest()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(DataTestUtility.NpConnStr);
            builder.ConnectTimeout = 5;

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                OpenBadConnection<PlatformNotSupportedException>(builder.ConnectionString);
            }
            else
            {
                OpenGoodConnection(builder.ConnectionString);
            }
        }

#if MANAGED_SNI
        [CheckConnStrSetupFact]
        public static void InvalidConnStringTest()
        {
            const string invalidConnStringError = "A network-related or instance-specific error occurred while establishing a connection to SQL Server. The server was not found or was not accessible. Verify that the instance name is correct and that SQL Server is configured to allow remote connections. (provider: Named Pipes Provider, error: 25 - Connection string is not valid)";

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(DataTestUtility.NpConnStr);
            builder.ConnectTimeout = 2;

            // Using forward slashes
            builder.DataSource = "np://NotARealServer/pipe/sql/query";
            OpenBadConnection<SqlException>(builder.ConnectionString, invalidConnStringError);

            // Without pipe token
            builder.DataSource = @"np:\\NotARealServer\sql\query";
            OpenBadConnection<SqlException>(builder.ConnectionString, invalidConnStringError);

            // Without a pipe name
            builder.DataSource = @"np:\\NotARealServer\pipe";
            OpenBadConnection<SqlException>(builder.ConnectionString, invalidConnStringError);

            // Nothing after server
            builder.DataSource = @"np:\\NotARealServer";
            OpenBadConnection<SqlException>(builder.ConnectionString, invalidConnStringError);

            // No leading slashes
            builder.DataSource = @"np:NotARealServer\pipe\sql\query";
            OpenBadConnection<SqlException>(builder.ConnectionString, invalidConnStringError);

            // No server name
            builder.DataSource = @"np:\\\pipe\sql\query";
            OpenBadConnection<SqlException>(builder.ConnectionString, invalidConnStringError);

            // Nothing but slashes
            builder.DataSource = @"np:\\\\\";
            OpenBadConnection<SqlException>(builder.ConnectionString, invalidConnStringError);
        }
#endif

        private static void OpenBadConnection<T>(string connectionString, string errorMessage = null) where T : Exception
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                DataTestUtility.AssertThrowsWrapper<T>(() => conn.Open(), errorMessage);
            }
        }

        private static void OpenGoodConnection(string connectionString)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                DataTestUtility.AssertEqualsWithDescription(ConnectionState.Open, conn.State, "FAILED: Connection should be in open state");
            }
        }
    }
}
