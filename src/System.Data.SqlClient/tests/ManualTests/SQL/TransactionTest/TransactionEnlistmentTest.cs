// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Transactions;
using Xunit;

namespace System.Data.SqlClient.ManualTesting.Tests
{
    public class TransactionEnlistmentTest
    {
        [CheckConnStrSetupFact]
        public static void Test8__()
        {
            Console.WriteLine("Test8__");

            const int inputCol1 = 1;
            const string inputCol2 = "one";

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(DataTestUtility.TcpConnStr);
            string connectionString = builder.ConnectionString;
            string testTableName = GenerateTableName();

            RunNonQuery(connectionString, string.Format("create table {0} (col1 int, col2 text)", testTableName));

            try
            {
                using (TransactionScope txScope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.MaxValue))
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        using (SqlCommand command = connection.CreateCommand())
                        {
                            command.CommandText = string.Format("INSERT INTO {0} VALUES ({1}, '{2}')", testTableName, inputCol1, inputCol2);
                            command.ExecuteNonQuery();
                        }
                    }
                    txScope.Complete();
                }

                DataTable result = RunQuery(connectionString, string.Format("select col2 from {0} where col1 = {1}", testTableName, inputCol1));
                Assert.True(result.Rows.Count > 0);
                Assert.True(string.Equals(result.Rows[0][0], inputCol2));
            }
            finally
            {
                RunNonQuery(connectionString, string.Format("drop table {0}", testTableName));
            }
        }

        private static void RunNonQuery(string connectionString, string sql)
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

        private static DataTable RunQuery(string connectionString, string sql)
        {
            DataTable result = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        /*
                        result = new List<Object[]>();
                        while(reader.Read())
                        {
                            Object[] row = new Object[reader.FieldCount];
                            for (int j = 0; j < reader.FieldCount; ++j)
                            {
                                row[j] = reader.GetValue(j);
                            }
                            result.Add(row);
                        }
                        */
                        result = new DataTable();
                        result.Load(reader);
                    }
                }
            }
            return result;
        }

        private static string GenerateTableName()
        {
            return string.Format("TEST_{0}{1}{2}", Environment.GetEnvironmentVariable("ComputerName"), Environment.TickCount, Guid.NewGuid()).Replace('-', '_');
        }
    }
}
