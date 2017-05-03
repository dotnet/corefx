using System;
using System.Data.SqlClient;
using System.Data.SqlClient.ManualTesting.Tests;
using Xunit;

namespace SQL.TransactionTest
{
    public class IsolationLevelCleanUpTest
    {
        [CheckConnStrSetupFact]
        public static void TestIsolationLevelCleanUpOnClose()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(DataTestUtility.TcpConnStr);
            builder.Pooling = true;

            SqlConnection conn = new SqlConnection(builder.ConnectionString);
            conn.Open();
            conn.BeginTransaction(System.Data.IsolationLevel.Serializable).Dispose();
            conn.Close();
            conn.Open();
            string currentIsolationLevel = GetCurrentIsolationLevel(conn);
            conn.Dispose();

            Assert.True("ReadCommitted".Equals(currentIsolationLevel));
        }

        private static string GetCurrentIsolationLevel(SqlConnection conn, SqlTransaction transaction = null)
        {
            var cmd = conn.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandText = @"SELECT CASE transaction_isolation_level 
						WHEN 0 THEN 'Unspecified' 
						WHEN 1 THEN 'ReadUncommitted' 
						WHEN 2 THEN 'ReadCommitted' 
						WHEN 3 THEN 'Repeatable' 
						WHEN 4 THEN 'Serializable' 
						WHEN 5 THEN 'Snapshot' END AS TRANSACTION_ISOLATION_LEVEL 
						FROM sys.dm_exec_sessions 
						where session_id = @@SPID";
            string isolationLevel = cmd.ExecuteScalar().ToString();
            return isolationLevel;
        }
    }
}
