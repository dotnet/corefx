// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using Xunit;

namespace System.Data.SqlClient.ManualTesting.Tests
{
    public class CommandCancelTest
    {
        // Shrink the packet size - this should make timeouts more likely
        private static string s_constr = (new SqlConnectionStringBuilder(DataTestClass.SQL2008_Northwind) { PacketSize = 512 }).ConnectionString;

        [Fact]
        public void MultiThreadedCancel_NonAsync()
        {
            MultiThreadedCancel(s_constr, false);
        }

        [Fact]
        public void TimeoutCancel()
        {
            TimeoutCancel(s_constr);
        }

        [Fact]
        public void CancelAndDisposePreparedCommand()
        {
            CancelAndDisposePreparedCommand(s_constr);
        }

        [Fact]
        public void TimeOutDuringRead()
        {
            TimeOutDuringRead(s_constr);
        }

        public void MultiThreadedCancel(string constr, bool async)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                var command = con.CreateCommand();
                command.CommandText = "select * from orders; waitfor delay '00:00:08'; select * from customers";

                Thread rThread1 = new Thread(ExecuteCommandCancelExpected);
                Thread rThread2 = new Thread(CancelSharedCommand);
                Barrier threadsReady = new Barrier(2);
                object state = new Tuple<bool, SqlCommand, Barrier>(async, command, threadsReady);

                rThread1.Start(state);
                rThread2.Start(state);
                rThread1.Join();
                rThread2.Join();

                CommandCancelTest.VerifyConnection(command);
            }
        }

        private void TimeoutCancel(string constr)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandTimeout = 1;
                cmd.CommandText = "WAITFOR DELAY '00:00:30';select * from Customers";

                string errorMessage = SystemDataResourceManager.Instance.SQL_Timeout;
                DataTestClass.ExpectFailure<SqlException>(() => cmd.ExecuteReader(), errorMessage);

                VerifyConnection(cmd);
            }
        }

        //InvalidOperationException from conenction.Dispose if that connection has prepared command cancelled during reading of data
        public static void CancelAndDisposePreparedCommand(string constr)
        {
            int expectedValue = 1;
            using (var connection = new SqlConnection(constr))
            {
                try
                {
                    // Generate a query with a large number of results.
                    using (var command = new SqlCommand("select @P from sysobjects a cross join sysobjects b cross join sysobjects c cross join sysobjects d cross join sysobjects e cross join sysobjects f"
                        , connection))
                    {
                        command.Parameters.Add(new SqlParameter("@P", SqlDbType.Int) { Value = expectedValue });
                        connection.Open();
                        // Prepare the query.
                        // Currently this does nothing until command.ExecuteReader is called.
                        // Ideally this shoudl call sp_prepare up-front.
                        command.Prepare();
                        using (var reader = command.ExecuteReader(CommandBehavior.SingleResult))
                        {
                            if (reader.Read())
                            {
                                int actualValue = reader.GetInt32(0);
                                Assert.True(actualValue == expectedValue, string.Format("Got incorrect value. Expected: {0}, Actual: {1}", expectedValue, actualValue));
                            }
                            // Abandon reading the results.
                            command.Cancel();
                        }
                    }
                }
                finally
                {
                    connection.Dispose(); // before the fix, InvalidOperationException happened here
                }
            }
        }


        public static void VerifyConnection(SqlCommand cmd)
        {
            Assert.True(cmd.Connection.State == ConnectionState.Open, "FAILURE: - unexpected non-open state after Execute!");

            cmd.CommandText = "select 'ABC'"; // Verify Connection
            string value = (string)cmd.ExecuteScalar();
            Assert.True(value == "ABC", "FAILURE: upon validation execute on connection: '" + value + "'");
        }

        public void ExecuteCommandCancelExpected(object state)
        {
            var stateTuple = (Tuple<bool, SqlCommand, Barrier>)state;
            bool async = stateTuple.Item1;
            SqlCommand command = stateTuple.Item2;
            Barrier threadsReady = stateTuple.Item3;

            string errorMessage = SystemDataResourceManager.Instance.SQL_OperationCancelled;
            DataTestClass.ExpectFailure<SqlException>(() =>
            {
                threadsReady.SignalAndWait();
                using (SqlDataReader r = command.ExecuteReader())
                {
                    do
                    {
                        while (r.Read())
                        {
                        }
                    } while (r.NextResult());
                }
            }, errorMessage);
        }

        public static void CancelSharedCommand(object state)
        {
            var stateTuple = (Tuple<bool, SqlCommand, Barrier>)state;

            // sleep 1 seconds before cancel to ensure ExecuteReader starts and ensure it does not end before Cancel is called (command is running WAITFOR 8 seconds)
            stateTuple.Item3.SignalAndWait();
            Thread.Sleep(TimeSpan.FromSeconds(1));
            stateTuple.Item2.Cancel();
        }

        public void TimeOutDuringRead(string constr)
        {
            // Create the proxy
            ProxyServer proxy = ProxyServer.CreateAndStartProxy(constr, out constr);
            proxy.SimulatedPacketDelay = 100;
            proxy.SimulatedOutDelay = true;

            try
            {
                using (SqlConnection conn = new SqlConnection(constr))
                {
                    // Start the command
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT @p", conn);
                    cmd.Parameters.AddWithValue("p", new byte[20000]);
                    SqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();

                    // Tweak the timeout to 1ms, stop the proxy from proxying and then try GetValue (which should timeout)
                    reader.SetDefaultTimeout(1);
                    proxy.PauseCopying();
                    string errorMessage = SystemDataResourceManager.Instance.SQL_Timeout;
                    Exception exception = Assert.Throws<SqlException>(() => reader.GetValue(0));
                    Assert.True(exception.Message.Contains(errorMessage));

                    // Return everything to normal and close
                    proxy.ResumeCopying();
                    reader.SetDefaultTimeout(30000);
                    reader.Dispose();
                }

                proxy.Stop();
            }
            catch
            {
                // In case of error, stop the proxy and dump its logs (hopefully this will help with debugging
                proxy.Stop();
                Console.WriteLine(proxy.GetServerEventLog());
                Assert.True(false, "Error while reading through proxy");
                throw;
            }
        }
    }
}
