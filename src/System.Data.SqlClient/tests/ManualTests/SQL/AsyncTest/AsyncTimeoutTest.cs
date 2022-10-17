// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Xml;
using System.Data.SqlClient.ManualTesting.Tests.SystemDataInternals;
using Xunit;

namespace System.Data.SqlClient.ManualTesting.Tests
{
    public static class AsyncTimeoutTest
    {
        static string delayQuery2s = "WAITFOR DELAY '00:00:02'";
        static string delayQuery10s = "WAITFOR DELAY '00:00:10'";

        public enum AsyncAPI
        {
            ExecuteReaderAsync,
            ExecuteScalarAsync,
            ExecuteXmlReaderAsync
        }

        [ConditionalTheory(typeof(DataTestUtility), nameof(DataTestUtility.AreConnStringsSetup))]
        [ClassData(typeof(AsyncTimeoutTestVariations))]
        public static void TestDelayedAsyncTimeout(AsyncAPI api, string commonObj, int delayPeriod, bool marsEnabled) =>
            RunTest(api, commonObj, delayPeriod, marsEnabled);

        public class AsyncTimeoutTestVariations : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { AsyncAPI.ExecuteReaderAsync, "Connection", 8000, true };
                yield return new object[] { AsyncAPI.ExecuteReaderAsync, "Connection", 5000, true };
                yield return new object[] { AsyncAPI.ExecuteReaderAsync, "Connection", 0, true };
                yield return new object[] { AsyncAPI.ExecuteReaderAsync, "Connection", 8000, false };
                yield return new object[] { AsyncAPI.ExecuteReaderAsync, "Connection", 5000, false };
                yield return new object[] { AsyncAPI.ExecuteReaderAsync, "Connection", 0, false };

                yield return new object[] { AsyncAPI.ExecuteScalarAsync, "Connection", 8000, true };
                yield return new object[] { AsyncAPI.ExecuteScalarAsync, "Connection", 5000, true };
                yield return new object[] { AsyncAPI.ExecuteScalarAsync, "Connection", 0, true };
                yield return new object[] { AsyncAPI.ExecuteScalarAsync, "Connection", 8000, false };
                yield return new object[] { AsyncAPI.ExecuteScalarAsync, "Connection", 5000, false };
                yield return new object[] { AsyncAPI.ExecuteScalarAsync, "Connection", 0, false };

                yield return new object[] { AsyncAPI.ExecuteXmlReaderAsync, "Connection", 8000, true };
                yield return new object[] { AsyncAPI.ExecuteXmlReaderAsync, "Connection", 5000, true };
                yield return new object[] { AsyncAPI.ExecuteXmlReaderAsync, "Connection", 0, true };
                yield return new object[] { AsyncAPI.ExecuteXmlReaderAsync, "Connection", 8000, false };
                yield return new object[] { AsyncAPI.ExecuteXmlReaderAsync, "Connection", 5000, false };
                yield return new object[] { AsyncAPI.ExecuteXmlReaderAsync, "Connection", 0, false };

                yield return new object[] { AsyncAPI.ExecuteReaderAsync, "Command", 8000, true };
                yield return new object[] { AsyncAPI.ExecuteReaderAsync, "Command", 5000, true };
                yield return new object[] { AsyncAPI.ExecuteReaderAsync, "Command", 0, true };
                yield return new object[] { AsyncAPI.ExecuteReaderAsync, "Command", 8000, false };
                yield return new object[] { AsyncAPI.ExecuteReaderAsync, "Command", 5000, false };
                yield return new object[] { AsyncAPI.ExecuteReaderAsync, "Command", 0, false };

                yield return new object[] { AsyncAPI.ExecuteScalarAsync, "Command", 8000, true };
                yield return new object[] { AsyncAPI.ExecuteScalarAsync, "Command", 5000, true };
                yield return new object[] { AsyncAPI.ExecuteScalarAsync, "Command", 0, true };
                yield return new object[] { AsyncAPI.ExecuteScalarAsync, "Command", 8000, false };
                yield return new object[] { AsyncAPI.ExecuteScalarAsync, "Command", 5000, false };
                yield return new object[] { AsyncAPI.ExecuteScalarAsync, "Command", 0, false };

                yield return new object[] { AsyncAPI.ExecuteXmlReaderAsync, "Command", 8000, true };
                yield return new object[] { AsyncAPI.ExecuteXmlReaderAsync, "Command", 5000, true };
                yield return new object[] { AsyncAPI.ExecuteXmlReaderAsync, "Command", 0, true };
                yield return new object[] { AsyncAPI.ExecuteXmlReaderAsync, "Command", 8000, false };
                yield return new object[] { AsyncAPI.ExecuteXmlReaderAsync, "Command", 5000, false };
                yield return new object[] { AsyncAPI.ExecuteXmlReaderAsync, "Command", 0, false };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private static void RunTest(AsyncAPI api, string commonObj, int timeoutDelay, bool marsEnabled)
        {
            string connString = new SqlConnectionStringBuilder(DataTestUtility.TcpConnStr)
            {
                MultipleActiveResultSets = marsEnabled
            }.ConnectionString;

            using (SqlConnection sqlConnection = new SqlConnection(connString))
            {
                sqlConnection.Open();
                if (timeoutDelay != 0)
                {
                    ConnectionHelper.SetEnforcedTimeout(sqlConnection, true, timeoutDelay);
                }
                switch (commonObj)
                {
                    case "Connection":
                        QueryAndValidate(api, 1, delayQuery2s, 1, true, true, sqlConnection).Wait();
                        QueryAndValidate(api, 2, delayQuery2s, 5, false, true, sqlConnection).Wait();
                        QueryAndValidate(api, 3, delayQuery10s, 1, true, true, sqlConnection).Wait();
                        QueryAndValidate(api, 4, delayQuery2s, 10, false, true, sqlConnection).Wait();
                        break;
                    case "Command":
                        using (SqlCommand cmd = sqlConnection.CreateCommand())
                        {
                            QueryAndValidate(api, 1, delayQuery2s, 1, true, false, sqlConnection, cmd).Wait();
                            QueryAndValidate(api, 2, delayQuery2s, 5, false, false, sqlConnection, cmd).Wait();
                            QueryAndValidate(api, 3, delayQuery10s, 1, true, false, sqlConnection, cmd).Wait();
                            QueryAndValidate(api, 4, delayQuery2s, 10, false, false, sqlConnection, cmd).Wait();
                        }
                        break;
                }
            }
        }

        private static async Task QueryAndValidate(AsyncAPI api, int index, string delayQuery, int timeout,
            bool timeoutExExpected = false, bool useTransaction = false, SqlConnection cn = null, SqlCommand cmd = null)
        {
            SqlTransaction tx = null;
            try
            {
                if (cn != null)
                {
                    if (cn.State != ConnectionState.Open)
                    {
                        await cn.OpenAsync();
                    }
                    cmd = cn.CreateCommand();
                    if (useTransaction)
                    {
                        tx = cn.BeginTransaction(IsolationLevel.ReadCommitted);
                        cmd.Transaction = tx;
                    }
                }

                cmd.CommandTimeout = timeout;
                if (api != AsyncAPI.ExecuteXmlReaderAsync)
                {
                    cmd.CommandText = delayQuery + $";select {index} as Id;";
                }
                else
                {
                    cmd.CommandText = delayQuery + $";select {index} as Id FOR XML PATH;";
                }

                var result = -1;
                switch (api)
                {
                    case AsyncAPI.ExecuteReaderAsync:
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                var columnIndex = reader.GetOrdinal("Id");
                                result = reader.GetInt32(columnIndex);
                                break;
                            }
                        }
                        break;
                    case AsyncAPI.ExecuteScalarAsync:
                        result = (int)await cmd.ExecuteScalarAsync().ConfigureAwait(false);
                        break;
                    case AsyncAPI.ExecuteXmlReaderAsync:
                        using (XmlReader reader = await cmd.ExecuteXmlReaderAsync().ConfigureAwait(false))
                        {
                            try
                            {
                                Assert.True(reader.Settings.Async);
                                reader.ReadToDescendant("Id");
                                result = reader.ReadElementContentAsInt();
                            }
                            catch (Exception ex)
                            {
                                Assert.False(true, "Exception occurred: " + ex.Message);
                            }
                        }
                        break;
                }

                if (result != index)
                {
                    throw new Exception("High Alert! Wrong data received for index: " + index);
                }
                else
                {
                    Assert.True(!timeoutExExpected && result == index);
                }
            }
            catch (SqlException e)
            {
                if (!timeoutExExpected)
                    throw new Exception("Index " + index + " failed with: " + e.Message);
                else
                    Assert.True(timeoutExExpected && e.Class == 11 && e.Number == -2);
            }
            finally
            {
                if (cn != null)
                {
                    if (useTransaction)
                        tx.Commit();
                    cn.Close();
                }
            }
        }
    }
}
