using System;
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
                try
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
                catch (SqlException ex)
                {
                    Console.WriteLine("Error ({0}): {1}", ex.Number, ex.Message);
                    Assert.Null(ex);
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine("Error: {0}", ex.Message);
                    Assert.Null(ex);
                }
                catch (Exception ex)
                {
                    // You might want to pass these errors
                    // back out to the caller.
                    Console.WriteLine("Error: {0}", ex.Message);
                    Assert.Null(ex);
                }
            }
        }

        [CheckConnStrSetupFact]
        public static void FailureTest()
        {
            using (SqlConnection connection = new SqlConnection(DataTestUtility.TcpConnStr))
            {
                bool caughtException = false;
                SqlCommand command = new SqlCommand(commandText, connection);
                connection.Open();

                //Try to execute a synchronous query on same command
                IAsyncResult result = command.BeginExecuteXmlReader();
                try
                {
                    command.ExecuteXmlReader();
                }
                catch (Exception ex)
                {
                    Assert.True(ex is InvalidOperationException, "FAILED: Thrown exception for BeginExecuteXmlReader was not an InvalidOperationException");
                    caughtException = true;
                }

                Assert.True(caughtException, "FAILED: No exception thrown after trying second BeginExecuteXmlReader.");
                caughtException = false;

                while (!result.IsCompleted)
                {
                    System.Threading.Thread.Sleep(100);
                }

                Assert.True(result.IsCompleted, "FAILED: ExecuteXmlReaderAsync Task did not complete successfully.");

                XmlReader reader = command.EndExecuteXmlReader(result);

                reader.ReadToDescendant("dbo.Customers");
                Assert.Equal("ALFKI", reader["CustomerID"]);
            }
        }
    }
}
