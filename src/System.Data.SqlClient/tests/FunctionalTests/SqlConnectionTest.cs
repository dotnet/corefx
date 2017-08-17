﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Data.Common;
using System.Reflection;
using Xunit;

namespace System.Data.SqlClient.Tests
{
    public class SqlConnectionBasicTests
    {

        [Fact]
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
        // https://github.com/dotnet/corefx/issues/19218 And https://github.com/dotnet/corefx/issues/21598
        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer), 
                                                    nameof(PlatformDetection.IsNotArmProcess))] 
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

        [Fact]
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
        public void SqlConnectionValidParameters()
        {
            var con = new SqlConnection("Timeout=1234;packet Size=5678 ;;; ;");
            Assert.Equal(1234, con.ConnectionTimeout);
            Assert.Equal(5678, con.PacketSize);
        }

        [Fact]
        public void SqlConnectionEmptyParameters()
        {
            var con = new SqlConnection("Timeout=;packet Size= ;");
            //default values are defined in internal class DbConnectionStringDefaults
            Assert.Equal(15, con.ConnectionTimeout);
            Assert.Equal(8000, con.PacketSize);
        }

        [Fact]
        public void SqlConnectionInvalidParameters()
        {
            Assert.Throws<ArgumentException>(() => new SqlConnection("Timeout=null;"));
            Assert.Throws<ArgumentException>(() => new SqlConnection("Timeout= null;"));
            Assert.Throws<ArgumentException>(() => new SqlConnection("Timeout=1 1;"));
            Assert.Throws<ArgumentException>(() => new SqlConnection("Timeout=1a;"));
        }
    }
}
