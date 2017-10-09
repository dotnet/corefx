// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Data.SqlClient;

namespace System.Data.Odbc.Tests
{
    public static class DataTestUtility
    {
        public static readonly string OdbcConnStr = null;
        public static readonly string TcpConnStr = null;

        static DataTestUtility()
        {
            OdbcConnStr = Environment.GetEnvironmentVariable("TEST_ODBC_CONN_STR");
            TcpConnStr = Environment.GetEnvironmentVariable("TEST_TCP_CONN_STR");
        }

        public static bool AreConnStringsSetup()
        {
            return !string.IsNullOrEmpty(OdbcConnStr) && !string.IsNullOrEmpty(TcpConnStr);
        }

        public static void RunNonQuery(string connectionString, string sql)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
