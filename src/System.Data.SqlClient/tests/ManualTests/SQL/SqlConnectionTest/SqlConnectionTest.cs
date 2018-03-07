﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlClient.ManualTesting.Tests;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace System.Data.SqlClient.ManualTesting.Tests
{
    public static class SqlConnectionTest
    {

        [CheckConnStrSetupFact]
        public static void CreateSqlConnectionWithCredential()
        {
            var user = "u" + Guid.NewGuid().ToString().Replace("-", "");
            var passStr = "pass";

            try
            {
                createTestUser(user, passStr);

                var csb = new SqlConnectionStringBuilder(DataTestUtility.TcpConnStr);
                csb.Remove("User ID");
                csb.Remove("Password");
                csb.IntegratedSecurity = false;

                var password = new SecureString();
                passStr.ToCharArray().ToList().ForEach(x => password.AppendChar(x));
                password.MakeReadOnly();

                using (var conn = new SqlConnection(csb.ConnectionString, new SqlCredential(user, password)))
                using (var cmd = new SqlCommand("SELECT 1;", conn))
                {
                    conn.Open();
                    Assert.Equal(1, cmd.ExecuteScalar());
                }
            }
            finally
            {
                dropTestUser(user);
            }
        }

        [CheckConnStrSetupFact]
        public static void SqlConnectionChangePasswordPlaintext()
        {
            var user = "u" + Guid.NewGuid().ToString().Replace("-", "");
            var pass = "pass";
            var newPass = "newPass";

            try
            {
                createTestUser(user, pass);

                var csb = new SqlConnectionStringBuilder(DataTestUtility.TcpConnStr);
                csb.UserID = user;
                csb.Password = pass;
                csb.IntegratedSecurity = false;

                // Change password and try opening connection.
                SqlConnection.ChangePassword(csb.ConnectionString, newPass);
                csb.Password = newPass;

                using (var conn = new SqlConnection(csb.ConnectionString))
                using (var cmd = new SqlCommand("SELECT 1;", conn))
                {
                    conn.Open();
                    Assert.Equal(1, cmd.ExecuteScalar());
                }
            }
            finally
            {
                dropTestUser(user);
            }
        }

        [CheckConnStrSetupFact]
        public static void SqlConnectionChangePasswordSecureString()
        {
            var user = "u" + Guid.NewGuid().ToString().Replace("-", "");
            var passStr = "pass";
            var newPassStr = "newPass";

            try
            {
                createTestUser(user, passStr);

                var csb = new SqlConnectionStringBuilder(DataTestUtility.TcpConnStr);
                csb.Remove("User ID");
                csb.Remove("Password");
                csb.IntegratedSecurity = false;

                var password = new SecureString();
                passStr.ToCharArray().ToList().ForEach(x => password.AppendChar(x));
                password.MakeReadOnly();

                var newPassword = new SecureString();
                newPassStr.ToCharArray().ToList().ForEach(x => newPassword.AppendChar(x));
                newPassword.MakeReadOnly();

                // Change password and try opening connection.
                SqlConnection.ChangePassword(csb.ConnectionString, new SqlCredential(user, password), newPassword);

                using (var conn = new SqlConnection(csb.ConnectionString, new SqlCredential(user, newPassword)))
                using (var cmd = new SqlCommand("SELECT 1;", conn))
                {
                    conn.Open();
                    Assert.Equal(1, cmd.ExecuteScalar());
                }
            }
            finally
            {
                dropTestUser(user);
            }
        }

        private static void createTestUser(string username, string password)
        {
            // Creates a test user with read permissions.
            string createUserCmd = $"CREATE LOGIN {username} WITH PASSWORD = '{password}';"
                                    + $"EXEC sp_adduser '{username}', '{username}', 'db_datareader';";

            using (var conn = new SqlConnection(DataTestUtility.TcpConnStr))
            using (var cmd = new SqlCommand(createUserCmd, conn))
            {
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private static void dropTestUser(string username)
        {
            // Removes a created test user.
            string dropUserCmd = $"DROP SCHEMA IF EXISTS {username};" 
                                    + $"DROP USER IF EXISTS {username};"
                                    + $"DROP LOGIN {username}";

            // Pool must be cleared to prevent DROP LOGIN failure.
            SqlConnection.ClearAllPools();

            using (var conn = new SqlConnection(DataTestUtility.TcpConnStr))
            using (var cmd = new SqlCommand(dropUserCmd, conn))
            {
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
