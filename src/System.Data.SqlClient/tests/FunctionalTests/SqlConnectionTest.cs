// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
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
        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))] // https://github.com/dotnet/corefx/issues/19218
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
        public void ConnectionTimeoutTestWithThread()
        {
            int timeoutSec = 5;
            string connStrNotAvailable = $"Server=tcp:fakeServer,1433;uid=fakeuser;pwd=fakepwd;Connection Timeout={timeoutSec}";

            List<ConnectionWorker> list = new List<ConnectionWorker>();
            for (int i = 0; i < 10; ++i)
            {
                list.Add(new ConnectionWorker(connStrNotAvailable));
            }

            ConnectionWorker.Start();
            ConnectionWorker.Stop();

            double theMax = 0;
            foreach (ConnectionWorker w in list)
            {
                if (theMax < w.MaxTimeElapsed)
                {
                    theMax = w.MaxTimeElapsed;
                }
            }

            int threshold = (timeoutSec + 1) * 1000;
            Assert.True(theMax < threshold);

            Console.WriteLine("theMax: "+ theMax);
        }

        public class ConnectionWorker
        {
            private static ManualResetEventSlim startEvent = new ManualResetEventSlim(false);
            private static List<ConnectionWorker> workerList = new List<ConnectionWorker>();
            private ManualResetEventSlim doneEvent = new ManualResetEventSlim(false);
            private double maxTimeElapsed;
            private Thread thread;
            private string connectionString;

            public ConnectionWorker(string connectionString)
            {
                workerList.Add(this);
                this.connectionString = connectionString;
                thread = new Thread(new ThreadStart(SqlConnectionOpen));
                thread.Start();
            }

            public double MaxTimeElapsed
            {
                get
                {
                    return maxTimeElapsed;
                }
            }

            public static void Start()
            {
                startEvent.Set();
            }

            public static void Stop()
            {
                foreach (ConnectionWorker w in workerList)
                {
                    w.doneEvent.Wait();
                }
            }

            public void SqlConnectionOpen()
            {
                startEvent.Wait();

                Stopwatch sw = new Stopwatch();
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    sw.Start();
                    try
                    {
                        con.Open();
                    }
                    catch { }
                    sw.Stop();
                }

                double elapsed = sw.Elapsed.TotalMilliseconds;
                if (maxTimeElapsed < elapsed)
                {
                    maxTimeElapsed = elapsed;
                }

                doneEvent.Set();
            }
        }
    }
}
