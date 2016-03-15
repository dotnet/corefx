// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Test.Common;
using System.Security.Authentication;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

using Xunit;
using Xunit.Abstractions;

namespace System.Net.Security.Tests
{
    public class KDCSetup : IDisposable
    {
        private const string Krb5ConfigFile = "/etc/krb5.conf";
        private const string KDestroyCmd = "kdestroy";
        private const string SudoCommand = "sudo";
        private const string ScriptName = "setup-kdc.sh";
        private const string ScriptUninstallArgs = "--uninstall --yes";
        private readonly bool _isKrbInstalled ;
        private const string NtlmUserFile = "NTLM_USER_FILE";

        public KDCSetup()
        {
            _isKrbInstalled = File.Exists(Krb5ConfigFile);
            if (!_isKrbInstalled)
            {
                int exitCode = RunSetupScript();
                if (exitCode != 0)
                {
                    Dispose();
                    Assert.True(false, "KDC setup failure");
                }
            }
        }

        public void Dispose()
        {
            if (!_isKrbInstalled)
            {
                RunSetupScript(ScriptUninstallArgs);
            }
        }

        // checks for avilability of Kerberos related infrastructure
        // on the host. Returns true available, false otherwise
        public bool CheckAndInitializeKerberos()
        {
            // Clear the credentials
            var startInfo = new ProcessStartInfo(KDestroyCmd);
            startInfo.UseShellExecute = true;
            startInfo.CreateNoWindow = true;
            startInfo.Arguments = "-A";
            using (Process clearCreds = Process.Start(startInfo))
            {
                clearCreds.WaitForExit();
                return (clearCreds.ExitCode == 0);
            }
        }

        public bool CheckAndInitializeNtlm(bool isKrbAvailable)
        {
            if (!isKrbAvailable)
            {
                return false;
            }

            if (File.Exists(TestConfiguration.NtlmUserFilePath))
            {
                Environment.SetEnvironmentVariable(NtlmUserFile, TestConfiguration.NtlmUserFilePath);
                return true;
            }

            return false;
        }

        private static int RunSetupScript(string args = null)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();

            // since ProcessStartInfo does not support Verb, we use sudo as
            // the program to be run
            startInfo.FileName = SudoCommand;
            startInfo.Arguments = string.Format("bash {0} {1}", ScriptName, args);
            using (Process kdcSetup = Process.Start(startInfo))
            {
                kdcSetup.WaitForExit();
                return kdcSetup.ExitCode;
            }
        }
    }

    public class NegotiateStreamTest : IDisposable, IClassFixture<KDCSetup>
    {
        private readonly byte[] _firstMessage = Encoding.UTF8.GetBytes("Sample First Message");
        private readonly byte[] _secondMessage = Encoding.UTF8.GetBytes("Sample Second Message");
        private readonly bool _isKrbAvailable; // tests are no-op if kerberos is not available on the host machine
        private readonly bool _isNtlmAvailable; // tests are no-op if ntlm is not available on the host machine
        private readonly KDCSetup _fixture;
        private readonly ITestOutputHelper _output;
        private readonly string _emptyTarget = string.Empty;
        private readonly string _testTarget = "TestTarget";
        private readonly static string _ntlmPassword = TestConfiguration.NtlmPassword;

        public NegotiateStreamTest(KDCSetup fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _output = output;
            _isKrbAvailable = _fixture.CheckAndInitializeKerberos();
            _isNtlmAvailable = _fixture.CheckAndInitializeNtlm(_isKrbAvailable);
        }

        [Fact, OuterLoop]
        [PlatformSpecific(PlatformID.Linux)]
        public void NegotiateStream_StreamToStream_KerberosAuthentication_Success()
        {
            if (!_isKrbAvailable)
            {
                return;
            }

            VirtualNetwork network = new VirtualNetwork();

            using (var clientStream = new VirtualNetworkStream(network, isServer: false))
            using (var serverStream = new VirtualNetworkStream(network, isServer: true))
            using (var client = new NegotiateStream(clientStream))
            using (var server = new UnixGssFakeNegotiateStream(serverStream))
            {
                Assert.False(client.IsAuthenticated, "client is not authenticated");

                Task[] auth = new Task[2];
                string user = string.Format("{0}@{1}", TestConfiguration.KerberosUser, TestConfiguration.Realm);
                string target = string.Format("{0}@{1}", TestConfiguration.HostTarget, TestConfiguration.Realm);
                NetworkCredential credential = new NetworkCredential(user, TestConfiguration.Password);
                auth[0] = client.AuthenticateAsClientAsync(credential, target);
                auth[1] = server.AuthenticateAsServerAsync();

                bool finished = Task.WaitAll(auth, TestConfiguration.PassingTestTimeoutMilliseconds);
                Assert.True(finished, "Handshake completed in the allotted time");

                // Expected Client property values:
                Assert.True(client.IsAuthenticated, "client is now authenticated");
                Assert.Equal(TokenImpersonationLevel.Identification, client.ImpersonationLevel);
                Assert.True(client.IsEncrypted, "client is encrypted");
                Assert.True(client.IsMutuallyAuthenticated, "client is mutually authenticated");
                Assert.False(client.IsServer, "client is not server");
                Assert.True(client.IsSigned, "client is signed");
                Assert.False(client.LeaveInnerStreamOpen, "inner stream remains open");

                IIdentity serverIdentity = client.RemoteIdentity;
                Assert.Equal("Kerberos", serverIdentity.AuthenticationType);
                Assert.True(serverIdentity.IsAuthenticated, "server identity is authenticated");
                IdentityValidator.AssertHasName(serverIdentity, target);
            }
        }

        [Fact, OuterLoop]
        [PlatformSpecific(PlatformID.Linux)]
        public void NegotiateStream_StreamToStream_AuthToHttpTarget_Success()
        {
            if (!_isKrbAvailable)
            {
                return;
            }

            VirtualNetwork network = new VirtualNetwork();

            using (var clientStream = new VirtualNetworkStream(network, isServer: false))
            using (var serverStream = new VirtualNetworkStream(network, isServer: true))
            using (var client = new NegotiateStream(clientStream))
            using (var server = new UnixGssFakeNegotiateStream(serverStream))
            {
                Assert.False(client.IsAuthenticated);

                Task[] auth = new Task[2];
                string user = string.Format("{0}@{1}", TestConfiguration.KerberosUser, TestConfiguration.Realm);
                NetworkCredential credential = new NetworkCredential(user, TestConfiguration.Password);
                auth[0] = client.AuthenticateAsClientAsync(credential, TestConfiguration.HttpTarget);
                auth[1] = server.AuthenticateAsServerAsync();

                bool finished = Task.WaitAll(auth, TestConfiguration.PassingTestTimeoutMilliseconds);
                Assert.True(finished, "Handshake completed in the allotted time");

                // Expected Client property values:
                Assert.True(client.IsAuthenticated, "client is authenticated");
                Assert.Equal(TokenImpersonationLevel.Identification, client.ImpersonationLevel);
                Assert.True(client.IsEncrypted, "client is encrypted");
                Assert.True(client.IsMutuallyAuthenticated, "mutually authentication is true");
                Assert.False(client.IsServer, "client is not a server");
                Assert.True(client.IsSigned, "clientStream is signed");
                Assert.False(client.LeaveInnerStreamOpen, "Inner stream is open");

                IIdentity serverIdentity = client.RemoteIdentity;
                Assert.Equal("Kerberos", serverIdentity.AuthenticationType);
                Assert.True(serverIdentity.IsAuthenticated, "remote identity of client is authenticated");
                IdentityValidator.AssertHasName(serverIdentity, TestConfiguration.HttpTarget);
            }
        }

        [Fact, OuterLoop]
        [PlatformSpecific(PlatformID.Linux)]
        public void NegotiateStream_StreamToStream_KerberosAuthWithoutRealm_Success()
        {
            if (!_isKrbAvailable)
            {
                return;
            }

            VirtualNetwork network = new VirtualNetwork();

            using (var clientStream = new VirtualNetworkStream(network, isServer: false))
            using (var serverStream = new VirtualNetworkStream(network, isServer: true))
            using (var client = new NegotiateStream(clientStream))
            using (var server = new UnixGssFakeNegotiateStream(serverStream))
            {
                Assert.False(client.IsAuthenticated);

                Task[] auth = new Task[2];
                NetworkCredential credential = new NetworkCredential(TestConfiguration.KerberosUser, TestConfiguration.Password);
                auth[0] = client.AuthenticateAsClientAsync(credential, TestConfiguration.HostTarget);
                auth[1] = server.AuthenticateAsServerAsync();

                bool finished = Task.WaitAll(auth, TestConfiguration.PassingTestTimeoutMilliseconds);
                Assert.True(finished, "Handshake completed in the allotted time");

                // Expected Client property values:
                Assert.True(client.IsAuthenticated, "client is authenticated");
                Assert.Equal(TokenImpersonationLevel.Identification, client.ImpersonationLevel);
                Assert.True(client.IsEncrypted, "client stream is encrypted");
                Assert.True(client.IsMutuallyAuthenticated, "mutual authentication is true");
                Assert.False(client.IsServer, "client is not server");
                Assert.True(client.IsSigned, "client stream is signed");
                Assert.False(client.LeaveInnerStreamOpen, "inner stream is open");

                IIdentity serverIdentity = client.RemoteIdentity;
                Assert.Equal("Kerberos", serverIdentity.AuthenticationType);
                Assert.True(serverIdentity.IsAuthenticated, "remote identity is authenticated");
                IdentityValidator.AssertHasName(serverIdentity, TestConfiguration.HostTarget);
            }
        }

        [Fact, OuterLoop]
        [PlatformSpecific(PlatformID.Linux)]
        public void NegotiateStream_StreamToStream_KerberosAuthDefaultCredentials_Success()
        {
            if (!_isKrbAvailable)
            {
                return;
            }

            VirtualNetwork network = new VirtualNetwork();

            using (var clientStream = new VirtualNetworkStream(network, isServer: false))
            using (var serverStream = new VirtualNetworkStream(network, isServer: true))
            using (var client = new NegotiateStream(clientStream))
            using (var server = new UnixGssFakeNegotiateStream(serverStream))
            {
                Assert.False(client.IsAuthenticated, "client is not authenticated before AuthenticateAsClient call");

                Task[] auth = new Task[2];
                string user = string.Format("{0}@{1}", TestConfiguration.KerberosUser, TestConfiguration.Realm);
                string target = string.Format("{0}@{1}", TestConfiguration.HostTarget, TestConfiguration.Realm);
                // Seed the default Kerberos cache with the TGT
                UnixGssFakeNegotiateStream.GetDefaultKerberosCredentials(user, TestConfiguration.Password);
                auth[0] = client.AuthenticateAsClientAsync(CredentialCache.DefaultNetworkCredentials, target);
                auth[1] = server.AuthenticateAsServerAsync();

                bool finished = Task.WaitAll(auth, TestConfiguration.PassingTestTimeoutMilliseconds);
                Assert.True(finished, "Handshake completed in the allotted time");

                // Expected Client property values:
                Assert.True(client.IsAuthenticated, "client is now authenticated");
                Assert.Equal(TokenImpersonationLevel.Identification, client.ImpersonationLevel);
                Assert.True(client.IsEncrypted, "client stream is encrypted");
                Assert.True(client.IsMutuallyAuthenticated, "mutual authentication is true");
                Assert.False(client.IsServer, "client is not server");
                Assert.True(client.IsSigned, "client stream is signed");
                Assert.False(client.LeaveInnerStreamOpen, "inner stream is open");

                IIdentity serverIdentity = client.RemoteIdentity;
                Assert.Equal("Kerberos", serverIdentity.AuthenticationType);
                Assert.True(serverIdentity.IsAuthenticated,"server identity is authenticated");
                IdentityValidator.AssertHasName(serverIdentity, target);
            }
        }

        [Fact, OuterLoop]
        [PlatformSpecific(PlatformID.Linux)]
        public void NegotiateStream_EchoServer_ClientWriteRead_Successive_Sync_Success()
        {
            if (!_isKrbAvailable)
            {
                return;
            }

            VirtualNetwork network = new VirtualNetwork();
            byte[] firstRecvBuffer = new byte[_firstMessage.Length];
            byte[] secondRecvBuffer = new byte[_secondMessage.Length];

            using (var clientStream = new VirtualNetworkStream(network, isServer: false))
            using (var serverStream = new VirtualNetworkStream(network, isServer: true))
            using (var client = new NegotiateStream(clientStream))
            using (var server = new UnixGssFakeNegotiateStream(serverStream))
            {
                Assert.False(client.IsAuthenticated, "client is not authenticated before AuthenticateAsClient call");

                Task[] auth = new Task[2];
                string user = string.Format("{0}@{1}", TestConfiguration.KerberosUser, TestConfiguration.Realm);
                string target = string.Format("{0}@{1}", TestConfiguration.HostTarget, TestConfiguration.Realm);
                NetworkCredential credential = new NetworkCredential(user, TestConfiguration.Password);
                auth[0] = client.AuthenticateAsClientAsync(credential, target);
                auth[1] = server.AuthenticateAsServerAsync();
                bool finished = Task.WaitAll(auth, TestConfiguration.PassingTestTimeoutMilliseconds);
                Assert.True(finished, "Handshake completed in the allotted time");

                Task svrMsgTask = server.PollMessageAsync(2);

                client.Write(_firstMessage, 0, _firstMessage.Length);
                client.Write(_secondMessage, 0, _secondMessage.Length);
                client.Read(firstRecvBuffer, 0, firstRecvBuffer.Length);
                client.Read(secondRecvBuffer, 0, secondRecvBuffer.Length);
                Assert.True(_firstMessage.SequenceEqual(firstRecvBuffer), "first message received is as expected");
                Assert.True(_secondMessage.SequenceEqual(secondRecvBuffer), "second message received is as expected");
                finished = svrMsgTask.Wait(TestConfiguration.PassingTestTimeoutMilliseconds);
                Assert.True(finished, "Message roundtrip completed in the allotted time");
            }
        }

        [Fact, OuterLoop]
        [PlatformSpecific(PlatformID.Linux)]
        public void NegotiateStream_EchoServer_ClientWriteRead_Successive_Async_Success()
        {
            if (!_isKrbAvailable)
            {
                return;
            }

            VirtualNetwork network = new VirtualNetwork();
            byte[] firstRecvBuffer = new byte[_firstMessage.Length];
            byte[] secondRecvBuffer = new byte[_secondMessage.Length];

            using (var clientStream = new VirtualNetworkStream(network, isServer: false))
            using (var serverStream = new VirtualNetworkStream(network, isServer: true))
            using (var client = new NegotiateStream(clientStream))
            using (var server = new UnixGssFakeNegotiateStream(serverStream))
            {
                Assert.False(client.IsAuthenticated, "client is not authenticated before AuthenticateAsClient call");

                Task[] auth = new Task[2];
                string user = string.Format("{0}@{1}", TestConfiguration.KerberosUser, TestConfiguration.Realm);
                string target = string.Format("{0}@{1}", TestConfiguration.HostTarget, TestConfiguration.Realm);
                NetworkCredential credential = new NetworkCredential(user, TestConfiguration.Password);
                auth[0] = client.AuthenticateAsClientAsync(credential, target);
                auth[1] = server.AuthenticateAsServerAsync();
                bool finished = Task.WaitAll(auth, TestConfiguration.PassingTestTimeoutMilliseconds);
                Assert.True(finished, "Handshake completed in the allotted time");

                Task serverTask = server.PollMessageAsync(2);
                Task[] msgTasks = new Task[3];
                msgTasks[0] = client.WriteAsync(_firstMessage, 0, _firstMessage.Length).ContinueWith((t) =>
                client.WriteAsync(_secondMessage, 0, _secondMessage.Length));

                msgTasks[1] = client.ReadAsync(firstRecvBuffer, 0, firstRecvBuffer.Length).ContinueWith((t) =>
                   client.ReadAsync(secondRecvBuffer, 0, secondRecvBuffer.Length));

                msgTasks[2] = serverTask;
                finished = Task.WaitAll(msgTasks, TestConfiguration.PassingTestTimeoutMilliseconds);
                Assert.True(finished, "Messages sent and received in the allotted time");
                Assert.True(_firstMessage.SequenceEqual(firstRecvBuffer), "The first message received is as expected");
                Assert.True(_secondMessage.SequenceEqual(secondRecvBuffer), "The second message received is as expected");
            }
        }

        [Fact, OuterLoop]
        [PlatformSpecific(PlatformID.Linux)]
        public void NegotiateStream_StreamToStream_KerberosAuthDefaultCredentialsNoSeed_Failure()
        {
            if (!_isKrbAvailable)
            {
                return;
            }

            VirtualNetwork network = new VirtualNetwork();

            using (var clientStream = new VirtualNetworkStream(network, isServer: false))
            using (var client = new NegotiateStream(clientStream))
            {
                Assert.False(client.IsAuthenticated, "client is not authenticated before AuthenticateAsClient call");

                string user = string.Format("{0}@{1}", TestConfiguration.KerberosUser, TestConfiguration.Realm);
                string target = string.Format("{0}@{1}", TestConfiguration.HostTarget, TestConfiguration.Realm);
                Assert.ThrowsAsync<AuthenticationException>(() => client.AuthenticateAsClientAsync(CredentialCache.DefaultNetworkCredentials, target));
            }
        }

        [Fact, OuterLoop]
        [PlatformSpecific(PlatformID.Linux)]
        public void NegotiateStream_StreamToStream_KerberosAuthInvalidUser_Failure()
        {
            if (!_isKrbAvailable)
            {
                return;
            }

            VirtualNetwork network = new VirtualNetwork();

            using (var clientStream = new VirtualNetworkStream(network, isServer: false))
            using (var serverStream = new VirtualNetworkStream(network, isServer: true))
            using (var client = new NegotiateStream(clientStream))
            using (var server = new UnixGssFakeNegotiateStream(serverStream))
            {
                Assert.False(client.IsAuthenticated, "client is not authenticated by default");
                string user = string.Format("{0}@{1}", TestConfiguration.KerberosUser, TestConfiguration.Realm);
                string target = string.Format("{0}@{1}", TestConfiguration.HostTarget, TestConfiguration.Realm);
                NetworkCredential credential = new NetworkCredential(user.Substring(1), TestConfiguration.Password);
                Assert.ThrowsAsync<AuthenticationException>(() =>client.AuthenticateAsClientAsync(credential, target));
                Assert.ThrowsAsync<AuthenticationException>(() => server.AuthenticateAsServerAsync());
            }
        }

        [Fact, OuterLoop]
        [PlatformSpecific(PlatformID.Linux)]
        public void NegotiateStream_StreamToStream_KerberosAuthInvalidPassword_Failure()
        {
            if (!_isKrbAvailable)
            {
                return;
            }

            VirtualNetwork network = new VirtualNetwork();

            using (var clientStream = new VirtualNetworkStream(network, isServer: false))
            using (var serverStream = new VirtualNetworkStream(network, isServer: true))
            using (var client = new NegotiateStream(clientStream))
            using (var server = new UnixGssFakeNegotiateStream(serverStream))
            {
                Assert.False(client.IsAuthenticated, "client stream is not authenticated by default");

                string user = string.Format("{0}@{1}", TestConfiguration.KerberosUser, TestConfiguration.Realm);
                string target = string.Format("{0}@{1}", TestConfiguration.HostTarget, TestConfiguration.Realm);
                NetworkCredential credential = new NetworkCredential(user, TestConfiguration.Password.Substring(1));
                Task serverAuth = server.AuthenticateAsServerAsync();
                Assert.ThrowsAsync<AuthenticationException>(() => client.AuthenticateAsClientAsync(credential, target));
                Assert.ThrowsAsync<AuthenticationException>(() => server.AuthenticateAsServerAsync());
            }
        }

        [Fact, OuterLoop]
        [PlatformSpecific(PlatformID.Linux)]
        public void NegotiateStream_StreamToStream_KerberosAuthInvalidTarget_Failure()
        {
            if (!_isKrbAvailable)
            {
                return;
            }

            VirtualNetwork network = new VirtualNetwork();

            using (var clientStream = new VirtualNetworkStream(network, isServer: false))
            using (var client = new NegotiateStream(clientStream))
            {
                Assert.False(client.IsAuthenticated, "client stream is not authenticated by default");

                string user = string.Format("{0}@{1}", TestConfiguration.KerberosUser, TestConfiguration.Realm);
                string target = string.Format("{0}@{1}", TestConfiguration.HostTarget, TestConfiguration.Realm);
                NetworkCredential credential = new NetworkCredential(user, TestConfiguration.Password);
                Assert.ThrowsAsync<AuthenticationException>(() => client.AuthenticateAsClientAsync(credential, target.Substring(1)));
            }
        }

        public static IEnumerable<object[]> ValidNtlmCredentials()
        {
         
                yield return new object[]{new NetworkCredential(TestConfiguration.NtlmUser, _ntlmPassword, TestConfiguration.Domain)};
                yield return new object[] {new NetworkCredential(TestConfiguration.NtlmUser, _ntlmPassword)};
                yield return new object[]
                {
                    new NetworkCredential(
                        string.Format(@"{0}\{1}", TestConfiguration.Domain, TestConfiguration.NtlmUser), _ntlmPassword)
                };
                yield return new object[]
                {
                    new NetworkCredential(
                        string.Format("{0}@{1}", TestConfiguration.NtlmUser, TestConfiguration.Domain), _ntlmPassword)
                };
        }

        public static IEnumerable<object[]> InvalidNtlmCredentials
        {
            get
            {
                yield return new object[] { new NetworkCredential(TestConfiguration.NtlmUser, _ntlmPassword, TestConfiguration.Domain.Substring(1))};
                yield return new object[] { new NetworkCredential(TestConfiguration.NtlmUser.Substring(1), _ntlmPassword, TestConfiguration.Domain) };
                yield return new object[] { new NetworkCredential(TestConfiguration.NtlmUser, _ntlmPassword.Substring(1), TestConfiguration.Domain) };
                yield return new object[] { new NetworkCredential(string.Format(@"{0}\{1}", TestConfiguration.Domain, TestConfiguration.NtlmUser), _ntlmPassword, TestConfiguration.Domain.Substring(1))};
                yield return new object[] { new NetworkCredential(string.Format("{0}@{1}", TestConfiguration.NtlmUser, TestConfiguration.Domain.Substring(1)), _ntlmPassword)};
                yield return new object[] { new NetworkCredential(string.Format(@"{0}\{1}", TestConfiguration.Domain.Substring(1), TestConfiguration.NtlmUser), _ntlmPassword, TestConfiguration.Domain)};
                yield return new object[] { new NetworkCredential(TestConfiguration.NtlmUser, _ntlmPassword, TestConfiguration.Realm)};

            }
        }


        [Theory, OuterLoop]
        [MemberData("ValidNtlmCredentials")]
        [PlatformSpecific(PlatformID.Linux)]
        public void NegotiateStream_StreamToStream_NtlmAuthentication_ValidCredentials_Success(NetworkCredential credential)
        {
            if (!_isNtlmAvailable)
            {
                _output.WriteLine("skipping NegotiateStream_StreamToStream_NtlmAuthentication_ValidCredentials_Success");
                return;
            }

            VirtualNetwork network = new VirtualNetwork();

            using (var clientStream = new VirtualNetworkStream(network, isServer: false))
            using (var serverStream = new VirtualNetworkStream(network, isServer: true))
            using (var client = new NegotiateStream(clientStream))
            using (var server = new UnixGssFakeNegotiateStream(serverStream))
            {
                Assert.False(client.IsAuthenticated, "client.IsAuthenticated");
                Assert.False(server.IsAuthenticated, "server.IsAuthenticated");

                Task[] auth = new Task[2];
                auth[0] = client.AuthenticateAsClientAsync(credential, _testTarget, ProtectionLevel.None, TokenImpersonationLevel.Identification);
                auth[1] = server.AuthenticateAsServerAsync();

                bool finished = Task.WaitAll(auth, TestConfiguration.PassingTestTimeoutMilliseconds);
                Assert.True(finished, "Handshake completed in the allotted time");

                // Expected Client property values:
                Assert.True(client.IsAuthenticated, "client.IsAuthenticated");
                Assert.Equal(TokenImpersonationLevel.Identification, client.ImpersonationLevel);
                Assert.False(client.IsEncrypted, "client.IsEncrypted");
                Assert.False(client.IsMutuallyAuthenticated, "client.IsMutuallyAuthenticated");
                Assert.False(client.IsServer, "client.IsServer");
                Assert.True(client.IsSigned, "client.IsSigned");
                Assert.False(client.LeaveInnerStreamOpen, "client.LeaveInnerStreamOpen");

                IIdentity serverIdentity = client.RemoteIdentity;
                Assert.Equal("NTLM", serverIdentity.AuthenticationType);
                Assert.True(serverIdentity.IsAuthenticated, "serverIdentity.IsAuthenticated");
                IdentityValidator.AssertHasName(serverIdentity, _testTarget);
            }
        }

     
        [Fact, OuterLoop]
        [PlatformSpecific(PlatformID.Linux)]
        public void NegotiateStream_StreamToStream_NtlmAuthentication_EmptyTarget_KerberosUser_Fallback_Success()
        {
            if (!_isNtlmAvailable)
            {
                _output.WriteLine("skipping NegotiateStream_StreamToStream_NtlmAuthentication_EmptyTarget_KerberosUser_Fallback_Success");
                return;
            }

            VirtualNetwork network = new VirtualNetwork();

            using (var clientStream = new VirtualNetworkStream(network, isServer: false))
            using (var serverStream = new VirtualNetworkStream(network, isServer: true))
            using (var client = new NegotiateStream(clientStream))
            using (var server = new UnixGssFakeNegotiateStream(serverStream))
            {
                Assert.False(client.IsAuthenticated, "client.IsAuthenticated");
                Assert.False(server.IsAuthenticated, "server.IsAuthenticated");

                Task[] auth = new Task[2];
                string user = string.Format("{0}@{1}", TestConfiguration.KerberosUser, TestConfiguration.Realm);
                NetworkCredential credential = new NetworkCredential(user, TestConfiguration.Password);
                auth[0] = client.AuthenticateAsClientAsync(credential, _emptyTarget);
                auth[1] = server.AuthenticateAsServerAsync();

                bool finished = Task.WaitAll(auth, TestConfiguration.PassingTestTimeoutMilliseconds);
                Assert.True(finished, "Handshake completed in the allotted time");

                // Expected Client property values:
                Assert.True(client.IsAuthenticated, "client.IsAuthenticated");
                Assert.Equal(TokenImpersonationLevel.Identification, client.ImpersonationLevel);
                Assert.True(client.IsEncrypted, "client.IsEncrypted");
                Assert.False(client.IsMutuallyAuthenticated, "client.IsMutuallyAuthenticated");
                Assert.False(client.IsServer, "client.IsServer");
                Assert.True(client.IsSigned, "client.IsSigned");
                Assert.False(client.LeaveInnerStreamOpen, "client.LeaveInnerStreamOpen");

                IIdentity serverIdentity = client.RemoteIdentity;
                Assert.Equal("NTLM", serverIdentity.AuthenticationType);
                Assert.False(serverIdentity.IsAuthenticated, "serverIdentity.IsAuthenticated");
            }
        }

        [Fact, OuterLoop]
        [PlatformSpecific(PlatformID.Linux)]
        public void NegotiateStream_StreamToStream_NtlmAuthentication_KerberosCreds_Success()
        {
            if (!_isNtlmAvailable)
            {
                _output.WriteLine("skipping NegotiateStream_StreamToStream_NtlmAuthentication_KerberosCreds_Success");
                return;
            }

            VirtualNetwork network = new VirtualNetwork();

            using (var clientStream = new VirtualNetworkStream(network, isServer: false))
            using (var serverStream = new VirtualNetworkStream(network, isServer: true))
            using (var client = new NegotiateStream(clientStream))
            using (var server = new UnixGssFakeNegotiateStream(serverStream))
            {
                Assert.False(client.IsAuthenticated, "client.IsAuthenticated");
                Assert.False(server.IsAuthenticated, "server.IsAuthenticated");

                Task[] auth = new Task[2];
                string user = string.Format("{0}@{1}", TestConfiguration.KerberosUser, TestConfiguration.Realm);
                NetworkCredential credential = new NetworkCredential(user, TestConfiguration.Password);
                auth[0] = client.AuthenticateAsClientAsync(credential, TestConfiguration.HttpTarget, ProtectionLevel.None, TokenImpersonationLevel.Identification);
                auth[1] = server.AuthenticateAsServerAsync();

                bool finished = Task.WaitAll(auth, TestConfiguration.PassingTestTimeoutMilliseconds);
                Assert.True(finished, "Handshake completed in the allotted time");

                // Expected Client property values:
                Assert.True(client.IsAuthenticated, "client.IsAuthenticated");
                Assert.Equal(TokenImpersonationLevel.Identification, client.ImpersonationLevel);
                Assert.False(client.IsEncrypted, "client.IsEncrypted");
                Assert.False(client.IsMutuallyAuthenticated, "client.IsMutuallyAuthenticated");
                Assert.False(client.IsServer, "client.IsServer");
                Assert.True(client.IsSigned, "client.IsSigned");
                Assert.False(client.LeaveInnerStreamOpen, "client.LeaveInnerStreamOpen");

                IIdentity serverIdentity = client.RemoteIdentity;
                Assert.Equal("NTLM", serverIdentity.AuthenticationType);
                Assert.True(serverIdentity.IsAuthenticated, "server identity is authenticated");
                IdentityValidator.AssertHasName(serverIdentity, TestConfiguration.HttpTarget);
            }
        }

        [Fact, OuterLoop]
        [PlatformSpecific(PlatformID.Linux)]
        public void NegotiateStream_EchoServer_NTLM_ClientWriteRead_Successive_Sync_Success()
        {
            if (!_isNtlmAvailable)
            {
                _output.WriteLine("skipping NegotiateStream_EchoServer_NTLM_ClientWriteRead_Successive_Sync_Success");
                return;
            }

            byte[] firstRecvBuffer = new byte[_firstMessage.Length];
            byte[] secondRecvBuffer = new byte[_secondMessage.Length];

            VirtualNetwork network = new VirtualNetwork();

            using (var clientStream = new VirtualNetworkStream(network, isServer: false))
            using (var serverStream = new VirtualNetworkStream(network, isServer: true))
            using (var client = new NegotiateStream(clientStream))
            using (var server = new UnixGssFakeNegotiateStream(serverStream))
            {
                Assert.False(client.IsAuthenticated, "client.IsAuthenticated");
                Assert.False(server.IsAuthenticated, "server.IsAuthenticated");

                Task[] auth = new Task[2];
                string user = string.Format("{0}@{1}", TestConfiguration.KerberosUser, TestConfiguration.Realm);
                NetworkCredential credential = new NetworkCredential(user, TestConfiguration.Password);
                auth[0] = client.AuthenticateAsClientAsync(credential, _emptyTarget);
                auth[1] = server.AuthenticateAsServerAsync();

                bool finished = Task.WaitAll(auth, TestConfiguration.PassingTestTimeoutMilliseconds);
                Assert.True(finished, "Handshake completed in the allotted time");

                //clearing message queue
                byte[] junkBytes = new byte[5];
                int j = clientStream.Read(junkBytes, 0, 5);
                Task svrMsgTask = server.PollMessageAsync(2);

                client.Write(_firstMessage, 0, _firstMessage.Length);
                client.Write(_secondMessage, 0, _secondMessage.Length);
                client.Read(firstRecvBuffer, 0, firstRecvBuffer.Length);
                client.Read(secondRecvBuffer, 0, secondRecvBuffer.Length);
                Assert.True(_firstMessage.SequenceEqual(firstRecvBuffer), "first message received is as expected");
                Assert.True(_secondMessage.SequenceEqual(secondRecvBuffer), "second message received is as expected");
                finished = svrMsgTask.Wait(TestConfiguration.PassingTestTimeoutMilliseconds);
                Assert.True(finished, "Message roundtrip completed in the allotted time");
            }
        }

        [Fact, OuterLoop]
        [PlatformSpecific(PlatformID.Linux)]
        public void NegotiateStream_EchoServer_NTLM_ClientWriteRead_Successive_Async_Success()
        {
            if (!_isNtlmAvailable)
            {
                _output.WriteLine("skipping NegotiateStream_EchoServer_NTLM_ClientWriteRead_Successive_Async_Success");
                return;
            }

            byte[] firstRecvBuffer = new byte[_firstMessage.Length];
            byte[] secondRecvBuffer = new byte[_secondMessage.Length];

            VirtualNetwork network = new VirtualNetwork();

            using (var clientStream = new VirtualNetworkStream(network, isServer: false))
            using (var serverStream = new VirtualNetworkStream(network, isServer: true))
            using (var client = new NegotiateStream(clientStream))
            using (var server = new UnixGssFakeNegotiateStream(serverStream))
            {
                Assert.False(client.IsAuthenticated, "client.IsAuthenticated");
                Assert.False(server.IsAuthenticated, "server.IsAuthenticated");

                Task[] auth = new Task[2];
                string user = string.Format("{0}@{1}", TestConfiguration.KerberosUser, TestConfiguration.Realm);
                NetworkCredential credential = new NetworkCredential(user, TestConfiguration.Password);
                auth[0] = client.AuthenticateAsClientAsync(credential, _emptyTarget);
                auth[1] = server.AuthenticateAsServerAsync();

                bool finished = Task.WaitAll(auth, TestConfiguration.PassingTestTimeoutMilliseconds);
                Assert.True(finished, "Handshake completed in the allotted time");

                //clearing message queue
                byte[] junkBytes = new byte[5];
                int j = clientStream.Read(junkBytes, 0, 5);
                Task serverTask = server.PollMessageAsync(2);
                Task[] msgTasks = new Task[3];
                msgTasks[0] = client.WriteAsync(_firstMessage, 0, _firstMessage.Length).ContinueWith((t) =>
                client.WriteAsync(_secondMessage, 0, _secondMessage.Length));

                msgTasks[1] = client.ReadAsync(firstRecvBuffer, 0, firstRecvBuffer.Length).ContinueWith((t) =>
                   client.ReadAsync(secondRecvBuffer, 0, secondRecvBuffer.Length));

                msgTasks[2] = serverTask;
                finished = Task.WaitAll(msgTasks, TestConfiguration.PassingTestTimeoutMilliseconds);
                Assert.True(finished, "Messages sent and received in the allotted time");
                Assert.True(_firstMessage.SequenceEqual(firstRecvBuffer), "The first message received is as expected");
                Assert.True(_secondMessage.SequenceEqual(secondRecvBuffer), "The second message received is as expected");
            }
        }

        [Theory, OuterLoop]
        [MemberData("InvalidNtlmCredentials")]
        [PlatformSpecific(PlatformID.Linux)]
        public void NegotiateStream_StreamToStream_NtlmAuthentication_NtlmUser_InvalidCredentials_Fail(NetworkCredential credential)
        {
            if (!_isNtlmAvailable)
            {
                _output.WriteLine("skipping NegotiateStream_StreamToStream_NtlmAuthentication_NtlmUser_InvalidCredentials_Fail");
                return;
            }

            VirtualNetwork network = new VirtualNetwork();
            using (var clientStream = new VirtualNetworkStream(network, isServer: false))
            using (var serverStream = new VirtualNetworkStream(network, isServer: true))
            using (var client = new NegotiateStream(clientStream))
            using (var server = new UnixGssFakeNegotiateStream(serverStream))
            {
                Assert.False(client.IsAuthenticated, "client.IsAuthenticated");
                Assert.ThrowsAsync<AuthenticationException>(() => server.AuthenticateAsServerAsync());
                Assert.ThrowsAsync<AuthenticationException>(() => client.AuthenticateAsClientAsync(credential, _testTarget, ProtectionLevel.None, TokenImpersonationLevel.Identification));
            }
        }

        public void Dispose()
        {
            try
            {
                _fixture.CheckAndInitializeKerberos();
            }
            catch
            {
            }
        }
    }
}
