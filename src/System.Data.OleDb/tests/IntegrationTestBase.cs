// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using System.Data.OleDb;

namespace System.Data.OleDb.Tests
{
    public abstract class IntegrationTestBase : IDisposable
    {
        protected readonly OleDbConnection connection;
        protected readonly OleDbTransaction transaction;
        protected readonly OleDbCommand command;

        public IntegrationTestBase()
        {
            connection = new OleDbConnection(ConnectionStrings.WorkingConnection);
            connection.Open();
            transaction = connection.BeginTransaction();
            command = connection.CreateCommand();
            command.Transaction = transaction;
        }

        public void Dispose()
        {
            command.Dispose();
            transaction.Dispose();
            connection.Dispose();
        }
    }
}
