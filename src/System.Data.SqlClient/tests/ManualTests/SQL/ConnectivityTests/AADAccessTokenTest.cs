﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.RegularExpressions;
using Xunit;

namespace System.Data.SqlClient.ManualTesting.Tests
{
    public class AADAccessTokenTest
    {
        private static bool IsAccessTokenSetup() => DataTestUtility.IsAccessTokenSetup();
        private static bool IsAzureServer() => DataTestUtility.IsAzureSqlServer(DataTestUtility.GetDataSource(DataTestUtility.TcpConnStr));

        [ConditionalFact(nameof(IsAccessTokenSetup), nameof(IsAzureServer))]
        public static void AccessTokenTest()
        {
            using (SqlConnection connection = new SqlConnection(DataTestUtility.TcpConnStr))
            {
                connection.AccessToken = DataTestUtility.getAccessToken();
                connection.Open();
            }
        }
    }
}
