namespace NCLTest.Utilities
{
    using CoreFXTestLibrary;
    using System;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Threading.Tasks;
    using NCLTest.Common;

    [TestClass]
    public class PingTests
    {
        [TestMethod]
        public async Task SendPingAsyncWithIPAddress()
        {
            IPAddress localIpAddress = await TestSettings.GetLocalIPAddress();

            SendBatchPingAsync(
                (ping) => ping.SendPingAsync(localIpAddress),
                (pingReply) =>
                {
                    Assert.IsTrue(pingReply.Status == IPStatus.Success);
                    Assert.IsTrue(pingReply.Address.Equals(localIpAddress));
                });
        }

        [TestMethod]
        public async Task SendPingAsyncWithIPAddressAndTimeout()
        {
            IPAddress localIpAddress = await TestSettings.GetLocalIPAddress();

            SendBatchPingAsync(
                (ping) => ping.SendPingAsync(localIpAddress, TestSettings.PingTimeout),
                (pingReply) =>
                {
                    Assert.IsTrue(pingReply.Status == IPStatus.Success);
                    Assert.IsTrue(pingReply.Address.Equals(localIpAddress));
                });
        }

        [TestMethod]
        public async Task SendPingAsyncWithIPAddressAndTimeoutAndBuffer()
        {
            byte[] buffer = TestSettings.PayloadAsBytes;
            IPAddress localIpAddress = await TestSettings.GetLocalIPAddress();
            
            SendBatchPingAsync(
                (ping) => ping.SendPingAsync(localIpAddress, TestSettings.PingTimeout, buffer),
                (pingReply) =>
                {
                    Assert.IsTrue(pingReply.Status == IPStatus.Success);
                    Assert.IsTrue(pingReply.Address.Equals(localIpAddress));

                    buffer.AssertEquals(pingReply.Buffer);
                });
        }

        [TestMethod]
        public async Task SendPingAsyncWithIPAddressAndTimeoutAndBufferAndPingOptions()
        {
            IPAddress localIpAddress = await TestSettings.GetLocalIPAddress();

            byte[] buffer = TestSettings.PayloadAsBytes;
            SendBatchPingAsync(
                (ping) => ping.SendPingAsync(localIpAddress, TestSettings.PingTimeout, buffer, new PingOptions()),
                (pingReply) =>
                {
                    Assert.IsTrue(pingReply.Status == IPStatus.Success);
                    Assert.IsTrue(pingReply.Address.Equals(localIpAddress));

                    buffer.AssertEquals(pingReply.Buffer);
                });
        }

        [TestMethod]
        public async Task SendPingAsyncWithHost()
        {
            IPAddress localIpAddress = await TestSettings.GetLocalIPAddress();

            SendBatchPingAsync(
                (ping) => ping.SendPingAsync(TestSettings.LocalHost),
                (pingReply) =>
                {
                    Assert.IsTrue(pingReply.Status == IPStatus.Success);
                    Assert.IsTrue(pingReply.Address.Equals(localIpAddress));
                });
        }

        [TestMethod]
        public async Task SendPingAsyncWithHostAndTimeout()
        {
            IPAddress localIpAddress = await TestSettings.GetLocalIPAddress();

            SendBatchPingAsync(
                (ping) => ping.SendPingAsync(TestSettings.LocalHost, TestSettings.PingTimeout),
                (pingReply) =>
                {
                    Assert.IsTrue(pingReply.Status == IPStatus.Success);
                    Assert.IsTrue(pingReply.Address.Equals(localIpAddress));
                });
        }

        [TestMethod]
        public async Task SendPingAsyncWithHostAndTimeoutAndBuffer()
        {
            IPAddress localIpAddress = await TestSettings.GetLocalIPAddress();

            byte[] buffer = TestSettings.PayloadAsBytes;
            SendBatchPingAsync(
                (ping) => ping.SendPingAsync(TestSettings.LocalHost, TestSettings.PingTimeout, buffer),
                (pingReply) =>
                {
                    Assert.IsTrue(pingReply.Status == IPStatus.Success);
                    Assert.IsTrue(pingReply.Address.Equals(localIpAddress));

                    buffer.AssertEquals(pingReply.Buffer);
                });
        }

        [TestMethod]
        public async Task SendPingAsyncWithHostAndTimeoutAndBufferAndPingOptions()
        {
            IPAddress localIpAddress = await TestSettings.GetLocalIPAddress();

            byte[] buffer = TestSettings.PayloadAsBytes;
            SendBatchPingAsync(
                (ping) => ping.SendPingAsync(TestSettings.LocalHost, TestSettings.PingTimeout, buffer, new PingOptions()),
                (pingReply) =>
                {
                    Assert.IsTrue(pingReply.Status == IPStatus.Success);
                    Assert.IsTrue(pingReply.Address.Equals(localIpAddress));

                    buffer.AssertEquals(pingReply.Buffer);
                });
        }

        private const int PingCount = 4;
        private static void SendBatchPingAsync(Func<Ping, Task<PingReply>> sendPing, Action<PingReply> pingResultValidator)
        {
            Logger.LogInformation("Creating concurrent pings...");
            // create several concurrent pings
            Task[] pingTasks = new Task[PingCount];
            for (int i = 0; i < PingCount; i++)
            {
                pingTasks[i] = SendPingAsync(sendPing, pingResultValidator);
            }
            Task.WaitAll(pingTasks);
        }

        private static async Task SendPingAsync(Func<Ping, Task<PingReply>> sendPing, Action<PingReply> pingResultValidator)
        {
            var pingResult = await sendPing(new Ping());
            pingResultValidator(pingResult);
        }
    }
}
