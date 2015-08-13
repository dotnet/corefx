namespace NCLTest.Sockets
{
    using CoreFXTestLibrary;
    using NCLTest.Common;
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Net.NetworkInformation;
    
    /// <summary>
    /// Summary description for AgnosticListenerTest
    /// </summary>
    [TestClass]
    public class AgnosticListenerTest
    {
        private static int TestPortBase = 8010;

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize()
        {
            TestRequirements.CheckIPv4Support();
            TestRequirements.CheckIPv6Support();

            // Make sure my listening port is not already in use.
            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] endPoints = properties.GetActiveTcpListeners();
            foreach (IPEndPoint e in endPoints)
            {
                Assert.AreNotEqual(TestPortBase, e.Port, "Port is already in use");
                Assert.AreNotEqual(TestPortBase + 1, e.Port, "Port is already in use");
                Assert.AreNotEqual(TestPortBase + 2, e.Port, "Port is already in use");
                Assert.AreNotEqual(TestPortBase + 3, e.Port, "Port is already in use");
            }
        }

        public AgnosticListenerTest()
        {
            // Workaround for 916993
            MyTestInitialize();
        }

        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void Create_Success()
        {
            TcpListener listener = TcpListener.Create(TestPortBase);
            listener.Start();
            listener.Stop();
        }

        [TestMethod]
        public void ConnectWithV4_Success()
        {
            TcpListener listener = TcpListener.Create(TestPortBase + 1);
            listener.Start();
            IAsyncResult asyncResult = listener.BeginAcceptTcpClient(null, null);

            TcpClient client = new TcpClient(AddressFamily.InterNetwork);
            client.Connect(new IPEndPoint(IPAddress.Loopback, TestPortBase + 1));

            TcpClient acceptedClient = listener.EndAcceptTcpClient(asyncResult);
            client.Dispose();
            acceptedClient.Dispose();
            listener.Stop();
        }

        [TestMethod]
        public void ConnectWithV6_Success()
        {
            TcpListener listener = TcpListener.Create(TestPortBase + 2);
            listener.Start();
            IAsyncResult asyncResult = listener.BeginAcceptTcpClient(null, null);

            TcpClient client = new TcpClient(AddressFamily.InterNetworkV6);
            client.Connect(new IPEndPoint(IPAddress.IPv6Loopback, TestPortBase + 2));

            TcpClient acceptedClient = listener.EndAcceptTcpClient(asyncResult);
            client.Dispose();
            acceptedClient.Dispose();
            listener.Stop();
        }
        
        [TestMethod]
        public void ConnectWithV4AndV6_Success()
        {
            TcpListener listener = TcpListener.Create(TestPortBase + 3);
            listener.Start();
            IAsyncResult asyncResult = listener.BeginAcceptTcpClient(null, null);

            TcpClient v6Client = new TcpClient(AddressFamily.InterNetworkV6);
            v6Client.Connect(new IPEndPoint(IPAddress.IPv6Loopback, TestPortBase + 3));

            TcpClient acceptedV6Client = listener.EndAcceptTcpClient(asyncResult);
            Assert.AreEqual(AddressFamily.InterNetworkV6, acceptedV6Client.Client.RemoteEndPoint.AddressFamily, "v6 server side AddressFamily");
            Assert.AreEqual(AddressFamily.InterNetworkV6, v6Client.Client.RemoteEndPoint.AddressFamily, "v6 client side AddressFamily");

            asyncResult = listener.BeginAcceptTcpClient(null, null);

            TcpClient v4Client = new TcpClient(AddressFamily.InterNetwork);
            v4Client.Connect(new IPEndPoint(IPAddress.Loopback, TestPortBase + 3));

            TcpClient acceptedV4Client = listener.EndAcceptTcpClient(asyncResult);
            Assert.AreEqual(AddressFamily.InterNetworkV6, acceptedV4Client.Client.RemoteEndPoint.AddressFamily, "v4 server side AddressFamily");
            Assert.AreEqual(AddressFamily.InterNetwork, v4Client.Client.RemoteEndPoint.AddressFamily, "v4 client side AddressFamily");
            
            v6Client.Dispose();
            acceptedV6Client.Dispose();

            v4Client.Dispose();
            acceptedV4Client.Dispose();

            listener.Stop();
        }

        #region GC Finalizer test
        // This test assumes sequential execution of tests and that it is going to be executed after other tests
        // that used Sockets. 
        [TestMethod]
        public void TestFinalizers()
        {
            // Making several passes through the FReachable list.
            for (int i = 0; i < 3; i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
        #endregion 
    }
}
