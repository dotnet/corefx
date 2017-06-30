// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Data.Common;
using System.Reflection;
using Xunit;
using System.Net;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Threading;
using System.Collections;
using System.Transactions;
using System.Data.SqlClient;

namespace System.Data.SqlClient.Tests
{
    public class SqlConnectionBasicTests
    {
        //[Fact]
        public void ConnectionTest()
        {
            using (TestTdsServer server = TestTdsServer.StartTestServer())
            {
                using (SqlConnection connection = new SqlConnection(server.ConnectionString))
                {
                    connection.Open();
                }
            }
        }

        [PlatformSpecific(TestPlatforms.Windows)]  // Integ auth on Test server is supported on Windows right now
        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer), nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotArmProcess))] // https://github.com/dotnet/corefx/issues/19218 And https://github.com/dotnet/corefx/issues/21598
        [ActiveIssue("https://github.com/dotnet/corefx/issues/20718", TargetFrameworkMonikers.UapAot)]
        public void IntegratedAuthConnectionTest()
        {
            using (TestTdsServer server = TestTdsServer.StartTestServer())
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(server.ConnectionString);
                builder.IntegratedSecurity = true;
                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                }
            }
        }

        //[Fact]
        public void SqlConnectionDbProviderFactoryTest()
        {
            SqlConnection con = new SqlConnection();
            PropertyInfo dbProviderFactoryProperty = con.GetType().GetProperty("DbProviderFactory", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(dbProviderFactoryProperty);
            DbProviderFactory factory = dbProviderFactoryProperty.GetValue(con) as DbProviderFactory;
            Assert.NotNull(factory);
            Assert.Same(typeof(SqlClientFactory), factory.GetType());
            Assert.Same(SqlClientFactory.Instance, factory);
        }

        [Fact]
        public static void Test8()
        {
            Console.WriteLine("Test8");
            string connString = "Server=tcp:GELEE-VM-WIN10B,1433;User ID=testuser;Password=test1234;pooling=false;Enlist=true";
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connString);
            string connectionString = builder.ConnectionString;

            using (TransactionScope txScope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.MaxValue))
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    /*
                    try
                    {
                        using (SqlCommand command = connection.CreateCommand())
                        {
                            command.CommandText = "drop table mytable";
                            command.ExecuteNonQuery();
                        }
                    }
                    catch { }
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "create table mytable (col1 text, col2 text)";
                        command.ExecuteNonQuery();
                    }
                    
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "INSERT INTO mytable VALUES ('11', '22')";
                        command.ExecuteNonQuery();
                    }
                    */
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "INSERT INTO mytable VALUES ('31', '44')";
                        command.ExecuteNonQuery();
                    }
                    /*
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT @@SPID";
                        Console.WriteLine(command.ExecuteScalar());
                    }
                    */
                }
                txScope.Complete();
            }
        }
    }
}