﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Xunit;

namespace System.Data.SqlClient.ManualTesting.Tests
{
    public static class XmlReaderAsyncTest
    {
        private static string commandText =
            "SELECT * from dbo.Customers FOR XML AUTO, XMLDATA;";

        [CheckConnStrSetupFact]
        public static void ExecuteTest()
        {
            using (SqlConnection connection = new SqlConnection(DataTestUtility.TcpConnStr))
            {
                SqlCommand command = new SqlCommand(commandText, connection);
                connection.Open();

                IAsyncResult result = command.BeginExecuteXmlReader();
                while (!result.IsCompleted)
                {
                    System.Threading.Thread.Sleep(100);
                }

                XmlReader reader = command.EndExecuteXmlReader(result);

                reader.ReadToDescendant("dbo.Customers");
                Assert.Equal("ALFKI", reader["CustomerID"]);
            }
        }

        [CheckConnStrSetupFact]
        public static void ExceptionTest()
        {
            using (SqlConnection connection = new SqlConnection(DataTestUtility.TcpConnStr))
            {
                SqlCommand command = new SqlCommand(commandText, connection);
                connection.Open();

                //Try to execute a synchronous query on same command
                IAsyncResult result = command.BeginExecuteXmlReader();

                Assert.Throws<InvalidOperationException>( delegate { command.ExecuteXmlReader(); });

                while (!result.IsCompleted)
                {
                    System.Threading.Thread.Sleep(100);
                }

                XmlReader reader = command.EndExecuteXmlReader(result);

                reader.ReadToDescendant("dbo.Customers");
                Assert.Equal("ALFKI", reader["CustomerID"]);
            }
        }
    }
}
