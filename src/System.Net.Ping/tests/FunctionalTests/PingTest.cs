// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net.Test.Common;
using System.Threading;
using System.Threading.Tasks;

using Xunit;

namespace System.Net.NetworkInformation.Tests
{
    public class PingTest
    {
        private class FinalizingPing : Ping
        {
            public static volatile bool WasFinalized;

            public static void CreateAndRelease()
            {
                new FinalizingPing();
            }

            protected override void Dispose(bool disposing)
            {
                if (!disposing)
                {
                    WasFinalized = true;
                }

                base.Dispose(disposing);
            }
        }


        [Fact]
        public async Task SendPingAsync_InvalidArgs()
        {
            IPAddress localIpAddress = await TestSettings.GetLocalIPAddress();
            Ping p = new Ping();

            // Null address
            AssertExtensions.Throws<ArgumentNullException>("address", () => { p.SendPingAsync((IPAddress)null); });
            AssertExtensions.Throws<ArgumentNullException>("hostNameOrAddress", () => { p.SendPingAsync((string)null); });
            AssertExtensions.Throws<ArgumentNullException>("address", () => { p.SendAsync((IPAddress)null, null); });
            AssertExtensions.Throws<ArgumentNullException>("hostNameOrAddress", () => { p.SendAsync((string)null, null); });
            AssertExtensions.Throws<ArgumentNullException>("address", () => { p.Send((IPAddress)null); });
            AssertExtensions.Throws<ArgumentNullException>("hostNameOrAddress", () => { p.Send((string)null); });

            // Invalid address
            AssertExtensions.Throws<ArgumentException>("address", () => { p.SendPingAsync(IPAddress.Any); });
            AssertExtensions.Throws<ArgumentException>("address", () => { p.SendPingAsync(IPAddress.IPv6Any); });
            AssertExtensions.Throws<ArgumentException>("address", () => { p.SendAsync(IPAddress.Any, null); });
            AssertExtensions.Throws<ArgumentException>("address", () => { p.SendAsync(IPAddress.IPv6Any, null); });
            AssertExtensions.Throws<ArgumentException>("address", () => { p.Send(IPAddress.Any); });
            AssertExtensions.Throws<ArgumentException>("address", () => { p.Send(IPAddress.IPv6Any); });

            // Negative timeout
            AssertExtensions.Throws<ArgumentOutOfRangeException>("timeout", () => { p.SendPingAsync(localIpAddress, -1); });
            AssertExtensions.Throws<ArgumentOutOfRangeException>("timeout", () => { p.SendPingAsync(TestSettings.LocalHost, -1); });
            AssertExtensions.Throws<ArgumentOutOfRangeException>("timeout", () => { p.SendAsync(localIpAddress, -1, null); });
            AssertExtensions.Throws<ArgumentOutOfRangeException>("timeout", () => { p.SendAsync(TestSettings.LocalHost, -1, null); });
            AssertExtensions.Throws<ArgumentOutOfRangeException>("timeout", () => { p.Send(localIpAddress, -1); });
            AssertExtensions.Throws<ArgumentOutOfRangeException>("timeout", () => { p.Send(TestSettings.LocalHost, -1); });

            // Null byte[]
            AssertExtensions.Throws<ArgumentNullException>("buffer", () => { p.SendPingAsync(localIpAddress, 0, null); });
            AssertExtensions.Throws<ArgumentNullException>("buffer", () => { p.SendPingAsync(TestSettings.LocalHost, 0, null); });
            AssertExtensions.Throws<ArgumentNullException>("buffer", () => { p.SendAsync(localIpAddress, 0, null, null); });
            AssertExtensions.Throws<ArgumentNullException>("buffer", () => { p.SendAsync(TestSettings.LocalHost, 0, null, null); });
            AssertExtensions.Throws<ArgumentNullException>("buffer", () => { p.Send(localIpAddress, 0, null); });
            AssertExtensions.Throws<ArgumentNullException>("buffer", () => { p.Send(TestSettings.LocalHost, 0, null); });

            // Too large byte[]
            AssertExtensions.Throws<ArgumentException>("buffer", () => { p.SendPingAsync(localIpAddress, 1, new byte[65501]); });
            AssertExtensions.Throws<ArgumentException>("buffer", () => { p.SendPingAsync(TestSettings.LocalHost, 1, new byte[65501]); });
            AssertExtensions.Throws<ArgumentException>("buffer", () => { p.SendAsync(localIpAddress, 1, new byte[65501], null); });
            AssertExtensions.Throws<ArgumentException>("buffer", () => { p.SendAsync(TestSettings.LocalHost, 1, new byte[65501], null); });
            AssertExtensions.Throws<ArgumentException>("buffer", () => { p.Send(localIpAddress, 1, new byte[65501]); });
            AssertExtensions.Throws<ArgumentException>("buffer", () => { p.Send(TestSettings.LocalHost, 1, new byte[65501]); });
        }

        [Fact]
        public async Task SendPingAsyncWithIPAddress()
        {
            IPAddress localIpAddress = await TestSettings.GetLocalIPAddress();

            await SendBatchPingAsync(
                (ping) => ping.SendPingAsync(localIpAddress),
                (pingReply) =>
                {
                    Assert.Equal(IPStatus.Success, pingReply.Status);
                    Assert.True(pingReply.Address.Equals(localIpAddress));
                });
        }

        [Fact]
        public async Task SendPingAsyncWithIPAddress_AddressAsString()
        {
            IPAddress localIpAddress = await TestSettings.GetLocalIPAddress();

            await SendBatchPingAsync(
                (ping) => ping.SendPingAsync(localIpAddress.ToString()),
                (pingReply) =>
                {
                    Assert.Equal(IPStatus.Success, pingReply.Status);
                    Assert.True(pingReply.Address.Equals(localIpAddress));
                });
        }

        [Fact]
        public async Task SendPingAsyncWithIPAddressAndTimeout()
        {
            IPAddress localIpAddress = await TestSettings.GetLocalIPAddress();

            await SendBatchPingAsync(
                (ping) => ping.SendPingAsync(localIpAddress, TestSettings.PingTimeout),
                (pingReply) =>
                {
                    Assert.Equal(IPStatus.Success, pingReply.Status);
                    Assert.True(pingReply.Address.Equals(localIpAddress));
                });
        }

        [PlatformSpecific(TestPlatforms.Windows)]  // On Unix, Non-root pings cannot send arbitrary data in the buffer, and do not receive it back in the PingReply.
        [Fact]
        public async Task SendPingAsyncWithIPAddressAndTimeoutAndBuffer()
        {
            byte[] buffer = TestSettings.PayloadAsBytes;
            IPAddress localIpAddress = await TestSettings.GetLocalIPAddress();

            await SendBatchPingAsync(
                (ping) => ping.SendPingAsync(localIpAddress, TestSettings.PingTimeout, buffer),
                (pingReply) =>
                {
                    Assert.Equal(IPStatus.Success, pingReply.Status);
                    Assert.True(pingReply.Address.Equals(localIpAddress));

                    Assert.Equal(buffer, pingReply.Buffer);
                });
        }

        [PlatformSpecific(TestPlatforms.AnyUnix)]  // On Unix, Non-root pings cannot send arbitrary data in the buffer, and do not receive it back in the PingReply.
        [Fact]
        public async Task SendPingAsyncWithIPAddressAndTimeoutAndBuffer_Unix()
        {
            byte[] buffer = TestSettings.PayloadAsBytes;
            IPAddress localIpAddress = await TestSettings.GetLocalIPAddress();

            await SendBatchPingAsync(
                (ping) => ping.SendPingAsync(localIpAddress, TestSettings.PingTimeout, buffer),
                (pingReply) =>
                {
                    Assert.Equal(IPStatus.Success, pingReply.Status);
                    Assert.True(pingReply.Address.Equals(localIpAddress));

                    // Non-root pings cannot send arbitrary data in the buffer, and do not receive it back in the PingReply.
                    if (Capability.CanUseRawSockets(localIpAddress.AddressFamily))
                    {
                        Assert.Equal(buffer, pingReply.Buffer);
                    }
                    else
                    {
                        Assert.Equal(Array.Empty<byte>(), pingReply.Buffer);
                    }
                });
        }

        [PlatformSpecific(TestPlatforms.Windows)]  // On Unix, Non-root pings cannot send arbitrary data in the buffer, and do not receive it back in the PingReply.
        [Fact]
        public async Task SendPingAsyncWithIPAddressAndTimeoutAndBufferAndPingOptions()
        {
            IPAddress localIpAddress = await TestSettings.GetLocalIPAddress();

            var options = new PingOptions();
            byte[] buffer = TestSettings.PayloadAsBytes;
            await SendBatchPingAsync(
                (ping) => ping.SendPingAsync(localIpAddress, TestSettings.PingTimeout, buffer, options),
                (pingReply) =>
                {
                    Assert.Equal(IPStatus.Success, pingReply.Status);
                    Assert.True(pingReply.Address.Equals(localIpAddress));
                    Assert.Equal(buffer, pingReply.Buffer);
                    Assert.InRange(pingReply.RoundtripTime, 0, long.MaxValue);
                });
        }

        [PlatformSpecific(TestPlatforms.AnyUnix)]  // On Unix, Non-root pings cannot send arbitrary data in the buffer, and do not receive it back in the PingReply.
        [Fact]
        public async Task SendPingAsyncWithIPAddressAndTimeoutAndBufferAndPingOptions_Unix()
        {
            IPAddress localIpAddress = await TestSettings.GetLocalIPAddress();

            byte[] buffer = TestSettings.PayloadAsBytes;
            await SendBatchPingAsync(
                (ping) => ping.SendPingAsync(localIpAddress, TestSettings.PingTimeout, buffer, new PingOptions()),
                (pingReply) =>
                {
                    Assert.Equal(IPStatus.Success, pingReply.Status);
                    Assert.True(pingReply.Address.Equals(localIpAddress));

                    // Non-root pings cannot send arbitrary data in the buffer, and do not receive it back in the PingReply.
                    if (Capability.CanUseRawSockets(localIpAddress.AddressFamily))
                    {
                        Assert.Equal(buffer, pingReply.Buffer);
                    }
                    else
                    {
                        Assert.Equal(Array.Empty<byte>(), pingReply.Buffer);
                    }
                });
        }

        [Fact]
        public async Task SendPingAsyncWithHost()
        {
            IPAddress localIpAddress = await TestSettings.GetLocalIPAddress();

            await SendBatchPingAsync(
                (ping) => ping.SendPingAsync(TestSettings.LocalHost),
                (pingReply) =>
                {
                    Assert.Equal(IPStatus.Success, pingReply.Status);
                    Assert.True(pingReply.Address.Equals(localIpAddress));
                });
        }

        [Fact]
        public async Task SendPingAsyncWithHostAndTimeout()
        {
            IPAddress localIpAddress = await TestSettings.GetLocalIPAddress();

            await SendBatchPingAsync(
                (ping) => ping.SendPingAsync(TestSettings.LocalHost, TestSettings.PingTimeout),
                (pingReply) =>
                {
                    Assert.Equal(IPStatus.Success, pingReply.Status);
                    Assert.True(pingReply.Address.Equals(localIpAddress));
                });
        }

        [PlatformSpecific(TestPlatforms.Windows)]  // On Unix, Non-root pings cannot send arbitrary data in the buffer, and do not receive it back in the PingReply.
        [Fact]
        public async Task SendPingAsyncWithHostAndTimeoutAndBuffer()
        {
            IPAddress localIpAddress = await TestSettings.GetLocalIPAddress();

            byte[] buffer = TestSettings.PayloadAsBytes;
            await SendBatchPingAsync(
                (ping) => ping.SendPingAsync(TestSettings.LocalHost, TestSettings.PingTimeout, buffer),
                (pingReply) =>
                {
                    Assert.Equal(IPStatus.Success, pingReply.Status);
                    Assert.True(pingReply.Address.Equals(localIpAddress));

                    Assert.Equal(buffer, pingReply.Buffer);
                });
        }

        [PlatformSpecific(TestPlatforms.AnyUnix)]  // On Unix, Non-root pings cannot send arbitrary data in the buffer, and do not receive it back in the PingReply.
        [Fact]
        public async Task SendPingAsyncWithHostAndTimeoutAndBuffer_Unix()
        {
            IPAddress localIpAddress = await TestSettings.GetLocalIPAddress();

            byte[] buffer = TestSettings.PayloadAsBytes;
            await SendBatchPingAsync(
                (ping) => ping.SendPingAsync(TestSettings.LocalHost, TestSettings.PingTimeout, buffer),
                (pingReply) =>
                {
                    Assert.Equal(IPStatus.Success, pingReply.Status);
                    Assert.True(pingReply.Address.Equals(localIpAddress));

                    // Non-root pings cannot send arbitrary data in the buffer, and do not receive it back in the PingReply.
                    if (Capability.CanUseRawSockets(localIpAddress.AddressFamily))
                    {
                        Assert.Equal(buffer, pingReply.Buffer);
                    }
                    else
                    {
                        Assert.Equal(Array.Empty<byte>(), pingReply.Buffer);
                    }
                });
        }

        [PlatformSpecific(TestPlatforms.Windows)]  // On Unix, Non-root pings cannot send arbitrary data in the buffer, and do not receive it back in the PingReply.
        [Fact]
        public async Task SendPingAsyncWithHostAndTimeoutAndBufferAndPingOptions()
        {
            IPAddress localIpAddress = await TestSettings.GetLocalIPAddress();

            byte[] buffer = TestSettings.PayloadAsBytes;
            await SendBatchPingAsync(
                (ping) => ping.SendPingAsync(TestSettings.LocalHost, TestSettings.PingTimeout, buffer, new PingOptions()),
                (pingReply) =>
                {
                    Assert.Equal(IPStatus.Success, pingReply.Status);
                    Assert.True(pingReply.Address.Equals(localIpAddress));

                    Assert.Equal(buffer, pingReply.Buffer);
                });
        }

        [PlatformSpecific(TestPlatforms.AnyUnix)]  // On Unix, Non-root pings cannot send arbitrary data in the buffer, and do not receive it back in the PingReply.
        [Fact]
        public async Task SendPingAsyncWithHostAndTimeoutAndBufferAndPingOptions_Unix()
        {
            IPAddress localIpAddress = await TestSettings.GetLocalIPAddress();

            byte[] buffer = TestSettings.PayloadAsBytes;
            await SendBatchPingAsync(
                (ping) => ping.SendPingAsync(TestSettings.LocalHost, TestSettings.PingTimeout, buffer, new PingOptions()),
                (pingReply) =>
                {
                    Assert.Equal(IPStatus.Success, pingReply.Status);
                    Assert.True(pingReply.Address.Equals(localIpAddress));

                    // Non-root pings cannot send arbitrary data in the buffer, and do not receive it back in the PingReply.
                    if (Capability.CanUseRawSockets(localIpAddress.AddressFamily))
                    {
                        Assert.Equal(buffer, pingReply.Buffer);
                    }
                    else
                    {
                        Assert.Equal(Array.Empty<byte>(), pingReply.Buffer);
                    }
                });
        }

        [Fact]
        public static async Task SendPings_ReuseInstance_Hostname()
        {
            IPAddress localIpAddress = await TestSettings.GetLocalIPAddress();

            using (Ping p = new Ping())
            {
                for (int i = 0; i < 3; i++)
                {
                    PingReply pingReply = await p.SendPingAsync(TestSettings.LocalHost);
                    Assert.Equal(IPStatus.Success, pingReply.Status);
                    Assert.True(pingReply.Address.Equals(localIpAddress));
                }
            }
        }

        [Fact]
        public static async Task Sends_ReuseInstance_Hostname()
        {
            IPAddress localIpAddress = await TestSettings.GetLocalIPAddress();

            using (Ping p = new Ping())
            {
                for (int i = 0; i < 3; i++)
                {
                    PingReply pingReply = p.Send(TestSettings.LocalHost);
                    Assert.Equal(IPStatus.Success, pingReply.Status);
                    Assert.True(pingReply.Address.Equals(localIpAddress));
                }
            }
        }

        [Fact]
        public static async Task SendAsyncs_ReuseInstance_Hostname()
        {
            IPAddress localIpAddress = await TestSettings.GetLocalIPAddress();

            using (Ping p = new Ping())
            {
                TaskCompletionSource<bool> tcs = null;
                PingCompletedEventArgs ea = null;
                p.PingCompleted += (s, e) =>
                {
                    ea = e;
                    tcs.TrySetResult(true);
                };
                Action reset = () =>
                {
                    ea = null;
                    tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                };

                // Several normal iterations
                for (int i = 0; i < 3; i++)
                {
                    reset();
                    p.SendAsync(TestSettings.LocalHost, null);
                    await tcs.Task;

                    Assert.NotNull(ea);
                    Assert.Equal(IPStatus.Success, ea.Reply.Status);
                    Assert.True(ea.Reply.Address.Equals(localIpAddress));
                }

                // Several canceled iterations
                for (int i = 0; i < 3; i++)
                {
                    reset();
                    p.SendAsync(TestSettings.LocalHost, null);
                    p.SendAsyncCancel(); // will block until operation can be started again
                    await tcs.Task;

                    bool cancelled = ea.Cancelled;
                    Exception error = ea.Error;
                    PingReply reply = ea.Reply;
                    Assert.True(cancelled ^ (error != null) ^ (reply != null),
                        "Cancelled: " + cancelled +
                        (error == null ? "" : (Environment.NewLine + "Error Message: " + error.Message + Environment.NewLine + "Error Inner Exception: " + error.InnerException)) +
                        (reply == null ? "" : (Environment.NewLine + "Reply Address: " + reply.Address + Environment.NewLine + "Reply Status: " + reply.Status)));
                }
            }
        }

        [Fact]
        public static async Task Ping_DisposeAfterSend_Success()
        {
            Ping p = new Ping();
            await p.SendPingAsync(TestSettings.LocalHost);
            p.Dispose();
        }

        [Fact]
        public static void Ping_DisposeMultipletimes_Success()
        {
            Ping p = new Ping();
            p.Dispose();
            p.Dispose();
        }

        [Fact]
        public static void Ping_SendAfterDispose_ThrowsSynchronously()
        {
            Ping p = new Ping();
            p.Dispose();
            Assert.Throws<ObjectDisposedException>(() => { p.SendPingAsync(TestSettings.LocalHost); });
        }

        private static readonly int s_pingcount = 4;

        private static Task SendBatchPingAsync(Func<Ping, Task<PingReply>> sendPing, Action<PingReply> pingResultValidator)
        {
            // create several concurrent pings
            Task[] pingTasks = new Task[s_pingcount];
            for (int i = 0; i < s_pingcount; i++)
            {
                pingTasks[i] = SendPingAsync(sendPing, pingResultValidator);
            }
            return Task.WhenAll(pingTasks);
        }

        private static async Task SendPingAsync(Func<Ping, Task<PingReply>> sendPing, Action<PingReply> pingResultValidator)
        {
            var pingResult = await sendPing(new Ping());
            pingResultValidator(pingResult);
        }

        [Fact]
        public void CanBeFinalized()
        {
            FinalizingPing.CreateAndRelease();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Assert.True(FinalizingPing.WasFinalized);
        }
    }
}
