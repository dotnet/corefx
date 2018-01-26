// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;

namespace System.Data.SqlClient.ManualTesting.Tests
{
    public class ReadAsyncTest
    {
        [CheckConnStrSetupFact]
        public static void Run()
        {
            CompareSyncAndAsync();
        }

        public static async void CompareSyncAndAsync()
        {
            double elapsedAsync = await RunReadAsync();
            double elapsedSync = RunRead();
            Assert.True(elapsedAsync < (elapsedSync / 2.0d));
        }

        private const string commandText = "SET NOCOUNT ON"
                                        + " SELECT 'a'"
                                        + " DECLARE @t DATETIME = SYSDATETIME()"
                                        + " WHILE DATEDIFF(s, @t, SYSDATETIME()) < 20 BEGIN"
                                        + "   SELECT 2 x INTO #y"
                                        + "   DROP TABLE #y"
                                        + " END"
                                        + " SELECT 'b'";

        public static async Task<double> RunReadAsync()
        {
            double maxElapsedTimeMillisecond = 0;
            using (SqlConnection connection = new SqlConnection(DataTestUtility.TcpConnStr))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = commandText;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        Task<bool> t;
                        Stopwatch stopwatch = new Stopwatch();
                        do
                        {
                            do
                            {
                                stopwatch.Start();
                                t = reader.ReadAsync();
                                stopwatch.Stop();
                                double elased = stopwatch.Elapsed.TotalMilliseconds;
                                if (maxElapsedTimeMillisecond < elased)
                                {
                                    maxElapsedTimeMillisecond = elased;
                                }
                            }
                            while (await t);
                        }
                        while (reader.NextResult());
                    }
                }
            }

            return maxElapsedTimeMillisecond;
        }

        public static double RunRead()
        {
            double maxElapsedTimeMillisecond = 0;
            using (SqlConnection connection = new SqlConnection(DataTestUtility.TcpConnStr))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = commandText;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        bool result;
                        Stopwatch stopwatch = new Stopwatch();
                        do
                        {
                            do
                            {
                                stopwatch.Start();
                                result = reader.Read();
                                stopwatch.Stop();
                                double elased = stopwatch.Elapsed.TotalMilliseconds;
                                if (maxElapsedTimeMillisecond < elased)
                                {
                                    maxElapsedTimeMillisecond = elased;
                                }
                            }
                            while (result);
                        }
                        while (reader.NextResult());
                    }
                }
            }

            return maxElapsedTimeMillisecond;
        }
    }
}
