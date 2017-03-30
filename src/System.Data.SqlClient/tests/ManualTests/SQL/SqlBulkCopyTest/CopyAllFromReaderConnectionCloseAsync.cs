// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace System.Data.SqlClient.ManualTesting.Tests
{
    public class CopyAllFromReaderConnectionClosedAsync
    {
        public static void Test(string srcConstr, string dstConstr, string dstTable)
        {
            Task t = TestAsync(srcConstr, dstConstr, dstTable);
            DataTestUtility.AssertThrowsWrapper<AggregateException, InvalidOperationException>(() => t.Wait());
            Assert.True(t.IsCompleted, "Task did not complete! Status: " + t.Status);
        }

        private static async Task TestAsync(string srcConstr, string dstConstr, string dstTable)
        {
            string initialQueryTemplate = "create table {0} (col1 int, col2 nvarchar(20), col3 nvarchar(10))";
            string sourceQueryTemplate = "select top 5 EmployeeID, LastName, FirstName from {0}";
            string sourceTable = "employees";

            string sourceQuery = string.Format(sourceQueryTemplate, sourceTable);
            string initialQuery = string.Format(initialQueryTemplate, dstTable);

            using (SqlConnection dstConn = new SqlConnection(dstConstr))
            using (SqlCommand dstCmd = dstConn.CreateCommand())
            {
                dstConn.Open();
                try
                {
                    Helpers.TryExecute(dstCmd, initialQuery);
                    using (SqlConnection srcConn = new SqlConnection(srcConstr))
                    using (SqlCommand srcCmd = new SqlCommand(sourceQuery, srcConn))
                    {
                        srcConn.Open();

                        using (DbDataReader reader = srcCmd.ExecuteReader())
                        using (SqlBulkCopy bulkcopy = new SqlBulkCopy(dstConn))
                        {
                            bulkcopy.DestinationTableName = dstTable;
                            dstConn.Close();
                            await bulkcopy.WriteToServerAsync(reader);
                        }
                    }
                }
                finally
                {
                    Helpers.TryDropTable(dstConstr, dstTable);
                }
            }
        }
    }
}
