// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// SmtpClientTest.cs - NUnit Test Cases for System.Net.Mail.SmtpClient
//
// Authors:
//   John Luke (john.luke@gmail.com)
//
// (C) 2006 John Luke
//

using System.IO;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace System.Net.Mail.Tests
{
    public class SmtpClientTest : FileCleanupTestBase
    {
        private SmtpClient _smtp;

        private SmtpClient Smtp
        {
            get
            {
                return _smtp ?? (_smtp = new SmtpClient());
            }
        }

        private string TempFolder
        {
            get
            {
                return TestDirectory;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (_smtp != null)
            {
                _smtp.Dispose();
            }
            base.Dispose(disposing);
        }

        [Theory]
        [InlineData(SmtpDeliveryMethod.SpecifiedPickupDirectory)]
        [InlineData(SmtpDeliveryMethod.PickupDirectoryFromIis)]
        [InlineData(SmtpDeliveryMethod.PickupDirectoryFromIis)]
        public void DeliveryMethodTest(SmtpDeliveryMethod method)
        {
            Smtp.DeliveryMethod = method;
            Assert.Equal(method, Smtp.DeliveryMethod);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void EnableSslTest(bool value)
        {
            Smtp.EnableSsl = value;
            Assert.Equal(value, Smtp.EnableSsl);
        }

        [Theory]
        [InlineData("127.0.0.1")]
        [InlineData("smtp.ximian.com")]
        public void HostTest(string host)
        {
            Smtp.Host = host;
            Assert.Equal(host, Smtp.Host);
        }

        [Fact]
        public void InvalidHostTest()
        {
            Assert.Throws<ArgumentNullException>(() => Smtp.Host = null);
            AssertExtensions.Throws<ArgumentException>("value", () => Smtp.Host = "");
        }

        [Fact]
        public void ServicePoint_GetsCachedInstanceSpecificToHostPort()
        {
            using (var smtp1 = new SmtpClient("localhost1", 25))
            using (var smtp2 = new SmtpClient("localhost1", 25))
            using (var smtp3 = new SmtpClient("localhost2", 25))
            using (var smtp4 = new SmtpClient("localhost2", 26))
            {
                ServicePoint s1 = smtp1.ServicePoint;
                ServicePoint s2 = smtp2.ServicePoint;
                ServicePoint s3 = smtp3.ServicePoint;
                ServicePoint s4 = smtp4.ServicePoint;

                Assert.NotNull(s1);
                Assert.NotNull(s2);
                Assert.NotNull(s3);
                Assert.NotNull(s4);

                Assert.Same(s1, s2);
                Assert.NotSame(s2, s3);
                Assert.NotSame(s2, s4);
                Assert.NotSame(s3, s4);
            }
        }

        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework)]
        [Fact]
        public void ServicePoint_NetCoreApp_AddressIsAccessible()
        {
            using (var smtp = new SmtpClient("localhost", 25))
            {
                Assert.Equal("mailto", smtp.ServicePoint.Address.Scheme);
                Assert.Equal("localhost", smtp.ServicePoint.Address.Host);
                Assert.Equal(25, smtp.ServicePoint.Address.Port);
            }
        }

        [SkipOnTargetFramework(~TargetFrameworkMonikers.NetFramework)]
        [Fact]
        public void ServicePoint_NetFramework_AddressIsInaccessible()
        {
            using (var smtp = new SmtpClient("localhost", 25))
            {
                ServicePoint sp = smtp.ServicePoint;
                Assert.Throws<NotSupportedException>(() => sp.Address);
            }
        }

        [Fact]
        public void ServicePoint_ReflectsHostAndPortChange()
        {
            using (var smtp = new SmtpClient("localhost1", 25))
            {
                ServicePoint s1 = smtp.ServicePoint;

                smtp.Host = "localhost2";
                ServicePoint s2 = smtp.ServicePoint;
                smtp.Host = "localhost2";
                ServicePoint s3 = smtp.ServicePoint;

                Assert.NotSame(s1, s2);
                Assert.Same(s2, s3);

                smtp.Port = 26;
                ServicePoint s4 = smtp.ServicePoint;
                smtp.Port = 26;
                ServicePoint s5 = smtp.ServicePoint;

                Assert.NotSame(s3, s4);
                Assert.Same(s4, s5);
            }
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("shouldnotexist")]
        [InlineData("\0")]
        [InlineData("C:\\some\\path\\like\\string")]
        public void PickupDirectoryLocationTest(string folder)
        {
            Smtp.PickupDirectoryLocation = folder;
            Assert.Equal(folder, Smtp.PickupDirectoryLocation);
        }

        [Theory]
        [InlineData(25)]
        [InlineData(1)]
        [InlineData(int.MaxValue)]
        public void PortTest(int value)
        {
            Smtp.Port = value;
            Assert.Equal(value, Smtp.Port);
        }

        [Fact]
        public void TestDefaultsOnProperties()
        {
            Assert.Equal(25, Smtp.Port);
            Assert.Equal(100000, Smtp.Timeout);
            Assert.Null(Smtp.Host);
            Assert.Null(Smtp.Credentials);
            Assert.False(Smtp.EnableSsl);
            Assert.False(Smtp.UseDefaultCredentials);
            Assert.Equal(SmtpDeliveryMethod.Network, Smtp.DeliveryMethod);
            Assert.Null(Smtp.PickupDirectoryLocation);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void Port_Value_Invalid(int value)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Smtp.Port = value);
        }

        [Fact]
        public void Send_Message_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Smtp.Send(null));
        }

        [Fact]
        public void Send_Network_Host_Null()
        {
            Assert.Throws<InvalidOperationException>(() => Smtp.Send("mono@novell.com", "everyone@novell.com", "introduction", "hello"));
        }

        [Fact]
        public void Send_Network_Host_Whitespace()
        {
            Smtp.Host = " \r\n ";
            Assert.Throws<InvalidOperationException>(() => Smtp.Send("mono@novell.com", "everyone@novell.com", "introduction", "hello"));
        }

        [Fact]
        public void Send_SpecifiedPickupDirectory()
        {
            Smtp.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
            Smtp.PickupDirectoryLocation = TempFolder;
            Smtp.Send("mono@novell.com", "everyone@novell.com", "introduction", "hello");

            string[] files = Directory.GetFiles(TempFolder, "*");
            Assert.Equal(1, files.Length);
            Assert.Equal(".eml", Path.GetExtension(files[0]));
        }

        [Theory]
        [InlineData("some_path_not_exist")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("\0abc")]
        public void Send_SpecifiedPickupDirectoryInvalid(string location)
        {
            Smtp.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
            Smtp.PickupDirectoryLocation = location;
            Assert.Throws<SmtpException>(() => Smtp.Send("mono@novell.com", "everyone@novell.com", "introduction", "hello"));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(50)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        public void TestTimeout(int value)
        {
            if (value < 0)
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => Smtp.Timeout = value);
                return;
            }

            Smtp.Timeout = value;
            Assert.Equal(value, Smtp.Timeout);
        }

        [Fact]
        public void Send_ServerDoesntExist_Throws()
        {
            using (var smtp = new SmtpClient(Guid.NewGuid().ToString("N")))
            {
                Assert.Throws<SmtpException>(() => smtp.Send("anyone@anyone.com", "anyone@anyone.com", "subject", "body"));
            }
        }

        [Fact]
        public async Task SendAsync_ServerDoesntExist_Throws()
        {
            using (var smtp = new SmtpClient(Guid.NewGuid().ToString("N")))
            {
                await Assert.ThrowsAsync<SmtpException>(() => smtp.SendMailAsync("anyone@anyone.com", "anyone@anyone.com", "subject", "body"));
            }
        }

        [Fact]
        public void TestMailDelivery()
        {
            SmtpServer server = new SmtpServer();
            SmtpClient client = new SmtpClient("localhost", server.EndPoint.Port);
            client.Credentials = new NetworkCredential("user", "password");
            MailMessage msg = new MailMessage("foo@example.com", "bar@example.com", "hello", "howdydoo");
            string clientDomain = IPGlobalProperties.GetIPGlobalProperties().HostName.Trim().ToLower();

            try
            {
                Thread t = new Thread(server.Run);
                t.Start();
                client.Send(msg);
                t.Join();

                Assert.Equal("<foo@example.com>", server.MailFrom);
                Assert.Equal("<bar@example.com>", server.MailTo);
                Assert.Equal("hello", server.Subject);
                Assert.Equal("howdydoo", server.Body);
                Assert.Equal(clientDomain, server.ClientDomain);
            }
            finally
            {
                server.Stop();
            }
        }

        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, ".NET Framework has a bug and could hang in case of null or empty body")]
        [Theory]
        [InlineData("howdydoo")]
        [InlineData("")]
        [InlineData(null)]
        public async Task TestMailDeliveryAsync(string body)
        {
            SmtpServer server = new SmtpServer();
            SmtpClient client = new SmtpClient("localhost", server.EndPoint.Port);
            MailMessage msg = new MailMessage("foo@example.com", "bar@example.com", "hello", body);
            string clientDomain = IPGlobalProperties.GetIPGlobalProperties().HostName.Trim().ToLower();

            try
            {
                Thread t = new Thread(server.Run);
                t.Start();
                await client.SendMailAsync(msg).TimeoutAfter((int)TimeSpan.FromSeconds(30).TotalMilliseconds);
                t.Join();

                Assert.Equal("<foo@example.com>", server.MailFrom);
                Assert.Equal("<bar@example.com>", server.MailTo);
                Assert.Equal("hello", server.Subject);
                Assert.Equal(body ?? "", server.Body);
                Assert.Equal(clientDomain, server.ClientDomain);
            }
            finally
            {
                server.Stop();
            }
        }

        [Fact]
        public async Task TestCredentialsCopyInAsyncContext()
        {
            SmtpServer server = new SmtpServer();
            SmtpClient client = new SmtpClient("localhost", server.EndPoint.Port);
            MailMessage msg = new MailMessage("foo@example.com", "bar@example.com", "hello", "howdydoo");
            string clientDomain = IPGlobalProperties.GetIPGlobalProperties().HostName.Trim().ToLower();

            CredentialCache cache = new CredentialCache();
            cache.Add("localhost", server.EndPoint.Port, "NTLM", CredentialCache.DefaultNetworkCredentials);

            client.Credentials = cache;

            try
            {
                Thread t = new Thread(server.Run);
                t.Start();
                await client.SendMailAsync(msg);
                t.Join();

                Assert.Equal("<foo@example.com>", server.MailFrom);
                Assert.Equal("<bar@example.com>", server.MailTo);
                Assert.Equal("hello", server.Subject);
                Assert.Equal("howdydoo", server.Body);
                Assert.Equal(clientDomain, server.ClientDomain);
            }
            finally
            {
                server.Stop();
            }
        }


        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "NETFX doesn't have the fix for encoding")]
        [Theory]
        [InlineData(false, false, false)]
        [InlineData(false, false, true)] // Received subjectText.
        [InlineData(false, true, false)]
        [InlineData(false, true, true)]
        [InlineData(true, false, false)]
        [InlineData(true, false, true)] // Received subjectText.
        [InlineData(true, true, false)]
        [InlineData(true, true, true)] // Received subjectBase64. If subjectText is received, the test fails, and the results are inconsistent with those of synchronous methods.
        public void SendMail_DeliveryFormat_SubjectEncoded(bool useAsyncSend, bool useSevenBit, bool useSmtpUTF8)
        {
            // If the server support `SMTPUTF8` and use `SmtpDeliveryFormat.International`, the server should received this subject.
            const string subjectText = "Test \u6d4b\u8bd5 Contain \u5305\u542b UTF8";

            // If the server does not support `SMTPUTF8` or use `SmtpDeliveryFormat.SevenBit`, the server should received this subject.
            const string subjectBase64 = "=?utf-8?B?VGVzdCDmtYvor5UgQ29udGFpbiDljIXlkKsgVVRGOA==?=";

            SmtpServer server = new SmtpServer();

            // Setting up Server Support for `SMTPUTF8`.
            server.SupportSmtpUTF8 = useSmtpUTF8;

            SmtpClient client = new SmtpClient("localhost", server.EndPoint.Port);
            
            if (useSevenBit)
            {
                // Subject will be encoded by Base64.
                client.DeliveryFormat = SmtpDeliveryFormat.SevenBit;
            }
            else
            {
                // If the server supports `SMTPUTF8`, subject will not be encoded. Otherwise, subject will be encoded by Base64.
                client.DeliveryFormat = SmtpDeliveryFormat.International;
            }

            MailMessage msg = new MailMessage("foo@example.com", "bar@example.com", subjectText, "hello \u9ad8\u575a\u679c");
            msg.HeadersEncoding = msg.BodyEncoding = msg.SubjectEncoding = System.Text.Encoding.UTF8;

            try
            {
                Thread t = new Thread(server.Run);
                t.Start();

                if (useAsyncSend)
                {
                    client.SendMailAsync(msg).Wait(); 
                }
                else
                {
                    client.Send(msg);
                }

                if (useSevenBit || !useSmtpUTF8)
                {
                    Assert.Equal(subjectBase64, server.Subject);
                }
                else
                {
                    Assert.Equal(subjectText, server.Subject);
                }
            }
            finally
            {
                server.Stop();
            }
        }
    }
}
