// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Transactions;
using Xunit;

namespace System.Data.SqlClient.Tests
{
    public class AmbientTransactionFailureTest
    {

        private static string servername = Guid.NewGuid().ToString();
        private static string connectionStringWithEnlistAsDefault = $"Data Source={servername}; Integrated Security=true; Connect Timeout=1;";
        private static string connectionStringWithEnlistOff = $"Data Source={servername}; Integrated Security=true; Connect Timeout=1;Enlist=False";

        private static Action<string> ConnectToServer = (connectionString) =>
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
            }
        };

        private static Func<string, Task> ConnectToServerTask = (connectionString) =>
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                return connection.OpenAsync();
            }
        };

        private static Func<string, Task> ConnectToServerInTransactionScopeTask = (connectionString) =>
        {
            using (TransactionScope scope = new TransactionScope())
            {
                return ConnectToServerTask(connectionString);
            }
        };

        private static Action<string> ConnectToServerInTransactionScope = (connectionString) =>
        {
            using (TransactionScope scope = new TransactionScope())
            {
                ConnectToServer(connectionString);
            }
        };

        private static Action<string> EnlistConnectionInTransaction = (connectionString) =>
        {
            using (TransactionScope scope = new TransactionScope())
            {
                SqlConnection connection = new SqlConnection(connectionString);
                connection.EnlistTransaction(Transaction.Current);
            }
        };

        public static readonly object[][] ExceptionTestDataForSqlException =
        {
            new object[] { ConnectToServerInTransactionScope, connectionStringWithEnlistOff },
            new object[] { ConnectToServer, connectionStringWithEnlistAsDefault }
        };

        public static readonly object[][] ExceptionTestDataForNotSupportedException =
        {
            new object[] { ConnectToServerInTransactionScope, connectionStringWithEnlistAsDefault },
            new object[] { EnlistConnectionInTransaction, connectionStringWithEnlistAsDefault },
            new object[] { EnlistConnectionInTransaction, connectionStringWithEnlistOff }
        };

        [Theory]
        [MemberData(nameof(ExceptionTestDataForSqlException))]
        public void TestSqlException(Action<string> connectAction, string connectionString)
        {
            Assert.Throws<SqlException>(() =>
            {
                connectAction(connectionString);
            });
        }

        [Theory]
        [MemberData(nameof(ExceptionTestDataForNotSupportedException))]
        public void TestNotException(Action<string> connectAction, string connectionString)
        {
            Assert.Throws<NotSupportedException>(() =>
            {
                connectAction(connectionString);
            });
        }

        [Fact]
        public void TestNotSupportExceptionForTransactionScopeAsync()
        {
            Assert.ThrowsAsync<NotSupportedException>(() => ConnectToServerInTransactionScopeTask(connectionStringWithEnlistAsDefault));
        }
    }
}
