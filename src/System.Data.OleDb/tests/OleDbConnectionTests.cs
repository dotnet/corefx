// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Text.RegularExpressions;

namespace System.Data.OleDb.Tests
{
    public class OleDbConnectionTests 
    {
        [Fact]
        public void Ctor_ConnectionStringMissingProvider_Throws()
        {
            Assert.Throws<ArgumentException>(() => new OleDbConnection("Reason=missingProvider"));
        }

        [Fact]
        public void Ctor_LongProvider_Throws()
        {
            Assert.Throws<ArgumentException>(() => new OleDbConnection("provider=" + new string('c', 256)));
        }

        [Fact]
        public void Ctor_MSDASQLNotSupported_Throws()
        {
            Assert.Throws<ArgumentException>(() => new OleDbConnection("provider=MSDASQL"));
        }

        [Fact]
        public void Ctor_MissingUdlFile_Throws()
        {
            Assert.Throws<ArgumentException>(() => new OleDbConnection(@"file name = missing-file.udl"));
            Assert.Throws<ArgumentException>(() => new OleDbConnection(@"file name = C:\Users\maariyan\Desktop\TestConnectionInvalid.udl"));
        }

        [Fact]
        public void Ctor_AsynchronousNotSupported_Throws()
        {
            Assert.Throws<ArgumentException>(() => 
                new OleDbConnection(ConnectionStrings.WorkingConnection + ";asynchronous processing=true"));
        }

        [Fact]
        public void Ctor_InvalidConnectTimeout_Throws()
        {
            Assert.Throws<ArgumentException>(() => 
                new OleDbConnection(ConnectionStrings.WorkingConnection + ";connect timeout=-2"));
        }

        [Fact]
        public void Provider_SetProperlyFromCtor()
        {
            Match match = Regex.Match(ConnectionStrings.WorkingConnection, "Provider=([^']*);");
            string providerName = match.Groups[1].Value;
            using (var connection = new OleDbConnection(ConnectionStrings.WorkingConnection))
            {
                connection.Open();
                Assert.Equal(providerName, connection.Provider);
                var table = connection.GetSchema();
                Assert.NotNull(table);
            }
        }

        [Fact]
        public void DataSource_SetProperlyFromCtor()
        {
            using (var connection = new OleDbConnection(@"file name = C:\Users\maariyan\Desktop\TestConnectionSuccess.udl"))
            {
                connection.Open();
                Assert.NotEmpty(connection.DataSource);
            }
        }

        public static IEnumerable<object[]> ManufacturedOleDbSchemaGuids
        {
            get
            {
                yield return new object[] { OleDbSchemaGuid.DbInfoLiterals };
                yield return new object[] { OleDbSchemaGuid.SchemaGuids };
                yield return new object[] { OleDbSchemaGuid.DbInfoKeywords };
            }
        }

        [Theory, MemberData(nameof(ManufacturedOleDbSchemaGuids))]
        public void GetOleDbSchemaTable_NoRestrictions_Success(Guid oleDbSchemaGuid)
        {
            DataTable oleDbSchemaTable = null;
            using (var connection = new OleDbConnection(ConnectionStrings.WorkingConnection))
            {
                connection.Open();
                oleDbSchemaTable = connection.GetOleDbSchemaTable(oleDbSchemaGuid, restrictions: null);
            }
            Assert.NotNull(oleDbSchemaTable);
            Assert.NotNull(oleDbSchemaTable.Rows);
            foreach (DataRow dataRow in oleDbSchemaTable.Rows)
            {
                Assert.NotNull(dataRow.ItemArray);
            }
        }

        [Theory, MemberData(nameof(ManufacturedOleDbSchemaGuids))]
        public void GetOleDbSchemaTable_SomeRestrictions_Throws(Guid oleDbSchemaGuid)
        {
            object[] restrictions = new object[] { null };
            using (var connection = new OleDbConnection(ConnectionStrings.WorkingConnection))
            {
                connection.Open();
                Assert.Throws<ArgumentException>(() => 
                    connection.GetOleDbSchemaTable(oleDbSchemaGuid, restrictions));
            }
        }
    }
}