namespace NCLTest.NameResolution
{
    using CoreFXTestLibrary;
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using NCLTest.Common;

    [TestClass]
    public class DnsTests
    {
        [TestMethod]
        public void GetHostAddressesAsync()
        {
            var hostAddressesTask1 = Dns.GetHostAddressesAsync(TestSettings.LocalHost);
            var hostAddressesTask2 = Dns.GetHostAddressesAsync(TestSettings.LocalHost);

            Task.WaitAll(hostAddressesTask1, hostAddressesTask2);
            Assert.AreEqual(hostAddressesTask1.Result.Length, hostAddressesTask2.Result.Length);
            for(int i = 0; i < hostAddressesTask1.Result.Length; i++)
            {
                Assert.AreEqual(hostAddressesTask1.Result[i], hostAddressesTask2.Result[i]);
            }
        }

        [TestMethod]
        public async Task GetHostEntryAsyncWithAddress()
        {
            IPAddress localIPAddress = await TestSettings.GetLocalIPAddress();

            GetHostEntryAsync(() => Dns.GetHostEntryAsync(localIPAddress));
        }

        [TestMethod]
        public void GetHostEntryAsyncWithHost()
        {
            GetHostEntryAsync(() => Dns.GetHostEntryAsync(TestSettings.LocalHost));
        }


        private static void GetHostEntryAsync(Func<Task<IPHostEntry>> getHostEntryFunc)
        {
            var hostEntryTask1 = getHostEntryFunc();
            var hostEntryTask2 = getHostEntryFunc();

            Task.WaitAll(hostEntryTask1, hostEntryTask2);
            hostEntryTask1.Result.AddressList.AssertEquals(hostEntryTask2.Result.AddressList);
        }
    }
}
