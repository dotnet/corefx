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

            string plainConnString = builder.ConnectionString;
            builder.DataSource = "np:" + GetHostFromDataSource(builder.DataSource);
            string serverNameOnlyConnString = builder.ConnectionString;

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                OpenBadConnection<PlatformNotSupportedException>(plainConnString);
                OpenBadConnection<PlatformNotSupportedException>(serverNameOnlyConnString);
            }
            else
            {
                OpenGoodConnection(plainConnString);
                OpenGoodConnection(serverNameOnlyConnString);
            }
        }

#if MANAGED_SNI
        [CheckConnStrSetupFact]
        public static void InvalidConnStringTest()
        {
            const string invalidConnStringError = "A network-related or instance-specific error occurred while establishing a connection to SQL Server. The server was not found or was not accessible. Verify that the instance name is correct and that SQL Server is configured to allow remote connections. (provider: Named Pipes Provider, error: 25 - Connection string is not valid)";

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(DataTestUtility.NpConnStr);
            builder.ConnectTimeout = 2;

            string host = GetHostFromDataSource(builder.DataSource);

            // Using forward slashes
            builder.DataSource = "np://" + host + "/pipe/sql/query";
            OpenBadConnection<SqlException>(builder.ConnectionString, invalidConnStringError);

            // Without pipe token
            builder.DataSource = @"np:\\" + host + @"\sql\query";
            OpenBadConnection<SqlException>(builder.ConnectionString, invalidConnStringError);

            // Without a pipe name
            builder.DataSource = @"np:\\" + host + @"\pipe";
            OpenBadConnection<SqlException>(builder.ConnectionString, invalidConnStringError);

            // Nothing after server
            builder.DataSource = @"np:\\" + host;
            OpenBadConnection<SqlException>(builder.ConnectionString, invalidConnStringError);

            // No leading slashes
            builder.DataSource = @"np:" + host + @"\pipe\sql\query";
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

        private static string GetHostFromDataSource(string dataSource)
        {
            // NP Data Source can be prefixed with "np:" and then a path/hostname, or can just be the hostname
            int colonIndex = dataSource.IndexOf(':');
            if (colonIndex != -1)
            {
                if("np" != dataSource.Substring(0, colonIndex))
                {
                    throw new InvalidOperationException("Connection string did not contain expected NP token in Server Name string!");
                }
                return dataSource.Contains(@"\") ? (new Uri(dataSource.Substring(colonIndex + 1))).Host : dataSource.Substring(colonIndex+1);
            }
            else
            {
                return dataSource;
            }
        }
    }
}
