// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.Data;
using System.Data.SqlClient;
using Xunit;

namespace System.Data.SqlClient.ManualTesting.Tests
{
    public class CopyWithEvent
    {
        private static readonly long[] ExpectedRowCopiedResults = { 50, 100 };
        private static int currentRowCopyResult = 0;

        protected static void OnRowCopied(object sender, SqlRowsCopiedEventArgs e)
        {
            Assert.True(currentRowCopyResult < ExpectedRowCopiedResults.Length, "More row copies than expected!");
            DataTestUtility.AssertEqualsWithDescription(ExpectedRowCopiedResults[currentRowCopyResult++], e.RowsCopied, "Unexpected Rows Copied count.");
        }

        public static void Test(string srcConstr, string dstConstr, string dstTable)
        {
            DataSet dataset;
            SqlDataAdapter adapter;
            DataTable datatable;
            DataRow[] rows;

            using (SqlConnection dstConn = new SqlConnection(dstConstr))
            using (SqlCommand dstCmd = dstConn.CreateCommand())
            {
                dstConn.Open();

                try
                {
                    Helpers.TryExecute(dstCmd, "create table " + dstTable + " (orderid int, customerid nchar(5), rdate datetime, freight money, shipname nvarchar(40))");

                    using (SqlConnection srcConn = new SqlConnection(srcConstr))
                    using (SqlCommand srcCmd = new SqlCommand("select top 100 * from orders", srcConn))
                    {
                        srcConn.Open();

                        dataset = new DataSet("MyDataSet");
                        adapter = new SqlDataAdapter(srcCmd);
                        adapter.Fill(dataset);
                        datatable = dataset.Tables[0];
                        rows = new DataRow[datatable.Rows.Count];
                        for (int i = 0; i < rows.Length; i++)
                        {
                            rows[i] = datatable.Rows[i];
                        }
                    }

                    using (SqlBulkCopy bulkcopy = new SqlBulkCopy(dstConn))
                    {

                        bulkcopy.SqlRowsCopied += new SqlRowsCopiedEventHandler(OnRowCopied);

                        bulkcopy.DestinationTableName = dstTable;
                        bulkcopy.NotifyAfter = 50;

                        SqlBulkCopyColumnMappingCollection ColumnMappings = bulkcopy.ColumnMappings;

                        ColumnMappings.Add(0, "orderid");
                        ColumnMappings.Add(1, "customerid");
                        ColumnMappings.Add(4, "rdate");
                        ColumnMappings.Add(7, "freight");
                        ColumnMappings.Add(8, "shipname");

                        bulkcopy.WriteToServer(rows);
                        bulkcopy.SqlRowsCopied -= new SqlRowsCopiedEventHandler(OnRowCopied);
                    }
                    Helpers.VerifyResults(dstConn, dstTable, 5, 100);
                }
                finally
                {
                    Helpers.TryExecute(dstCmd, "drop table " + dstTable);
                }
            }
        }
    }
}
