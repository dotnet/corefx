﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Xunit;

namespace System.Data.SqlClient.ManualTesting.Tests
{
    public static class LegacyAsyncTest
    {
        [CheckConnStrSetupFact]
        public static void ExecuteTest()
        {
            string commandText =
                "INSERT INTO[dbo].[Shippers] " +
                "([CompanyName] " +
                ",[Phone]) " +
                "VALUES " +
                "('Acme Inc.' " +
                ",'555-1212'); " +
                "WAITFOR DELAY '0:0:3';" +
                "DELETE FROM dbo.Shippers WHERE ShipperID > 3;";

            SqlConnection connection = new SqlConnection(DataTestUtility.TcpConnStr);

            using (connection)
            {
                try
                {                    
                    SqlCommand command = new SqlCommand(commandText, connection);
                    connection.Open();

                    IAsyncResult result = command.BeginExecuteNonQuery();
                    while (!result.IsCompleted)
                    {
                        System.Threading.Thread.Sleep(100);
                    }

                    Assert.True(command.EndExecuteNonQuery(result) > 0, "FAILED: BeginExecuteNonQuery did not complete successfully.");
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("Error ({0}): {1}", ex.Number, ex.Message);
                    Assert.Null(ex);
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine("Error: {0}", ex.Message);
                    Assert.Null(ex);
                }
                catch (Exception ex)
                {
                    // You might want to pass these errors
                    // back out to the caller.
                    Console.WriteLine("Error: {0}", ex.Message);
                    Assert.Null(ex);
                }
            }
        }

        [CheckConnStrSetupFact]
        public static void FailureTest()
        {

            string commandText =
                "INSERT INTO[dbo].[Shippers] " +
                "([CompanyName] " +
                ",[Phone]) " +
                "VALUES " +
                "('Acme Inc.' " +
                ",'555-1212'); " +
                "WAITFOR DELAY '0:0:3';" +
                "DELETE FROM dbo.Shippers WHERE ShipperID > 3;";

            SqlConnection connection = new SqlConnection(DataTestUtility.TcpConnStr);

            using (connection)
            {
                bool expectedException = false;
                SqlCommand command = new SqlCommand(commandText, connection);
                connection.Open();

                IAsyncResult result = command.BeginExecuteNonQuery();
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Assert.True(ex is InvalidOperationException, "FAILED: Thrown exception for BeginExecuteNonQuery was not an InvalidOperationException");
                    expectedException = true;
                }

                Assert.True(expectedException, "FAILED: No exception thrown after trying second BeginExecuteNonQuery.");
                expectedException = false;

                while (!result.IsCompleted)
                {
                    System.Threading.Thread.Sleep(100);
                }

                Assert.True(result.IsCompleted, "FAILED: ExecuteNonQueryAsync Task did not complete successfully.");
                Assert.True(command.EndExecuteNonQuery(result) > 0, "FAILED: No rows affected");
            }
        }
    }
}
