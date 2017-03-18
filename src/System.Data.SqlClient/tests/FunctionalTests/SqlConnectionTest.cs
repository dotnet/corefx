// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using Xunit;

namespace System.Data.SqlClient.Tests
{
    public class SqlConnectionBasicTests
    {

        [Fact]
        public void ConnectionTest()
        {
            Exception e = null;

            using (TestTdsServer server = TestTdsServer.StartTestServer())
            {
                try
                {
                    SqlConnection connection = new SqlConnection(server.ConnectionString);
                    connection.Open();
                }
                catch (Exception ce)
                {
                    e = ce;
                }
            }
            Assert.Null(e);
        }


        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]  // Integ auth on Test server is supported on Windows right now
        public void IntegratedAuthConnectionTest()
        {
            Exception e = null;

            using (TestTdsServer server = TestTdsServer.StartTestServer())
            {
                try
                {
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(server.ConnectionString);
                    builder.IntegratedSecurity = true;
                    SqlConnection connection = new SqlConnection(builder.ConnectionString);
                    connection.Open();
                }
                catch (Exception ce)
                {
                    e = ce;
                }
            }
            Assert.Null(e);
        }
    }

}
