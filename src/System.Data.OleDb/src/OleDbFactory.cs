// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Data;
using System.Data.Common;
using System.Security;
using System.Security.Permissions;

namespace System.Data.OleDb {

    public sealed class OleDbFactory : DbProviderFactory {

        public static readonly OleDbFactory Instance = new OleDbFactory();

        private OleDbFactory() {
        }

        public override DbCommand CreateCommand() {
            return new OleDbCommand();
        }

        public override DbCommandBuilder CreateCommandBuilder() {
            return new OleDbCommandBuilder();
        }

        public override DbConnection CreateConnection() {
            return new OleDbConnection();
        }

        public override DbConnectionStringBuilder CreateConnectionStringBuilder() {
            return new OleDbConnectionStringBuilder();
        }

        public override DbDataAdapter CreateDataAdapter() {
            return new OleDbDataAdapter();
        }

        public override DbParameter CreateParameter() {
            return new OleDbParameter();
        }

    }
}

