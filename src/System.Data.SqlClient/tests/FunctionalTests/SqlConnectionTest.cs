// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using Microsoft.Win32;
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


        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]  // Integ auth on Test server is supported on Windows right now
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

        private const string LocalDBInstalledVersionRegistryKey = "SOFTWARE\\Microsoft\\Microsoft SQL Server Local DB\\Installed Versions\\";

        [Fact]
        public void LocaldbTest()
        {
            Exception e = null;
            
            
            //AppContext.SetSwitch("System.Data.SqlClient.UseLegacyNetworkingOnWindows", true);

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = "(localdb)\\MSSQLLocalDB";
            builder.IntegratedSecurity = true;
            
        
                SqlConnection connection = new SqlConnection(builder.ConnectionString);
                connection.Open();
                connection = new SqlConnection(builder.ConnectionString);
                connection.Open();
        
            
        }

    }

}
