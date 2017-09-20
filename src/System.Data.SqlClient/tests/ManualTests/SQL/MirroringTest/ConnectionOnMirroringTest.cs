// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace System.Data.SqlClient.ManualTesting.Tests
{
    public static class ConnectionOnMirroringTest
    {
        [CheckConnStrSetupFact]
        public static void TestMultipleConnectionToMirroredServer()
        {
            string connectionString = DataTestUtility.TcpConnStr;
            int mirroringState;
            string failoverPartnerName;
            bool isMirroring = GetMirroringInfo(connectionString, out mirroringState, out failoverPartnerName);
            List<SqlConnection> list = new List<SqlConnection>();
            if (isMirroring && mirroringState == 4 && !string.IsNullOrEmpty(failoverPartnerName))
            {
                Stopwatch stopWatch = new Stopwatch();
                TestWorker worker = new TestWorker(connectionString);
                Thread childThread = new Thread(() => worker.TestMultipleConnection());

                stopWatch.Start();
                childThread.Start();
                while (!worker.IsDone && stopWatch.ElapsedMilliseconds <= 10000);
                stopWatch.Stop();

                if (worker.IsDone)
                {
                    childThread.Join();
                }
                else
                {
                    //thread.Abort() is not implemented yet in CoreFx.
                    childThread.Interrupt();
                    throw new Exception();
                }
            }
        }

        private static bool GetMirroringInfo(string connectionString, out int mirroringState, out string failoverPartnerName)
        {
            mirroringState = -1;
            failoverPartnerName = null;

            SqlConnectionStringBuilder existingConnStrBuilder = new SqlConnectionStringBuilder(connectionString);
            SqlConnectionStringBuilder newConnStrBuilder = new SqlConnectionStringBuilder();
            newConnStrBuilder.DataSource = existingConnStrBuilder.DataSource;
            if (!string.IsNullOrEmpty(existingConnStrBuilder.UserID))
            {
                newConnStrBuilder.UserID = existingConnStrBuilder.UserID;
            }
            if (!string.IsNullOrEmpty(existingConnStrBuilder.Password))
            {
                newConnStrBuilder.Password = existingConnStrBuilder.Password;
            }
            if (existingConnStrBuilder.IntegratedSecurity)
            {
                newConnStrBuilder.IntegratedSecurity = true;
            }

            string dbname = existingConnStrBuilder.InitialCatalog;
            DataTable dt = DataTestUtility.RunQuery(newConnStrBuilder.ConnectionString, $"select mirroring_state from sys.database_mirroring where database_id = DB_ID('{dbname}')");
            bool isMirroring = Int32.TryParse(dt.Rows[0][0].ToString(), out mirroringState);

            if (isMirroring)
            {
                dt = DataTestUtility.RunQuery(newConnStrBuilder.ConnectionString, $"select mirroring_partner_name from sys.database_mirroring where database_id = DB_ID('{dbname}')");
                failoverPartnerName = dt.Rows[0][0].ToString();
            }

            return isMirroring;
        }

        private class TestWorker
        {
            private string _connectionString;
            private bool _isDone;

            public TestWorker(string connectionString)
            {
                _connectionString = connectionString;
                _isDone = false;
            }

            public bool IsDone { get => _isDone; }

            public void TestMultipleConnection()
            {
                List<SqlConnection> list = new List<SqlConnection>();

                for (int i = 0; i < 10; ++i)
                {
                    SqlConnection conn = new SqlConnection(_connectionString);
                    list.Add(conn);
                    conn.Open();
                }

                foreach (SqlConnection conn in list)
                {
                    conn.Dispose();
                }

                _isDone = true;
            }
        }
    }
}
