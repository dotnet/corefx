// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net.Test.Common;
using System.Security;
using System.Security.Authentication;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

using Xunit;

namespace System.Net.Security.Tests
{
    using Configuration = System.Net.Test.Common.Configuration;

    public class NegotiateStreamKerberosTest
    {
        public static bool IsServerAndDomainAvailable => 
            Capability.IsDomainAvailable() && Capability.IsNegotiateServerAvailable();
        
        public static IEnumerable<object[]> GoodCredentialsData
        {
            get
            {
                yield return new object[] { CredentialCache.DefaultNetworkCredentials };
                /*
                yield return new object[] { new NetworkCredential(
                    Configuration.Security.ActiveDirectoryUserName,
                    Configuration.Security.ActiveDirectoryUserPassword,
                    Configuration.Security.ActiveDirectoryName) };

                yield return new object[] { new NetworkCredential(
                    Configuration.Security.ActiveDirectoryUserName,
                    AsSecureString(Configuration.Security.ActiveDirectoryUserPassword),
                    Configuration.Security.ActiveDirectoryName) };
                
                
                // Anonymous (with domain name).
                yield return new object[] { new NetworkCredential(
                    Configuration.Security.ActiveDirectoryUserName,
                    (string)null,
                    Configuration.Security.ActiveDirectoryName) };

                yield return new object[] { new NetworkCredential(
                    Configuration.Security.ActiveDirectoryUserName,
                    (SecureString)null,
                    Configuration.Security.ActiveDirectoryName) };
                
                // Anonymous (without domain).
                yield return new object[] { new NetworkCredential(
                    Configuration.Security.ActiveDirectoryUserName,
                    (string)null,
                    null) };

                yield return new object[] { new NetworkCredential(
                    Configuration.Security.ActiveDirectoryUserName,
                    (SecureString)null,
                    null) };
                */
            }
        }

        public static IEnumerable<object[]> BadCredentialsData
        {
            get
            {
                yield return new object[] { new NetworkCredential(null, (string)null, Configuration.Security.ActiveDirectoryName) };
                yield return new object[] { new NetworkCredential(null, (SecureString)null, Configuration.Security.ActiveDirectoryName) };

                yield return new object[] { new NetworkCredential(null, (string)null, null) };
                yield return new object[] { new NetworkCredential(null, (SecureString)null, null) };

                yield return new object[] { new NetworkCredential(
                    "baduser", 
                    (string)null, 
                    Configuration.Security.ActiveDirectoryName) };

                yield return new object[] { new NetworkCredential(
                    "baduser", 
                    (SecureString)null, 
                    Configuration.Security.ActiveDirectoryName) };

                yield return new object[] { new NetworkCredential(
                    "baduser", 
                    AsSecureString("badpassword"), 
                    Configuration.Security.ActiveDirectoryName) };
            }
        }

        [OuterLoop]
        // [ConditionalTheory(nameof(IsServerAndDomainAvailable))]
        [MemberData(nameof(GoodCredentialsData))]
        public async Task NegotiateStream_ClientAuthenticationRemote_Success(object credentialObject)
        {
            var credential = (NetworkCredential)credentialObject;
            await VerifyClientAuthentication(credential);
        }

        [OuterLoop]
        // [ConditionalTheory(nameof(IsServerAndDomainAvailable))]
        [MemberData(nameof(BadCredentialsData))]
        public async Task NegotiateStream_ClientAuthenticationRemote_Fails(object credentialObject)
        {
            var credential = (NetworkCredential)credentialObject;
            await Assert.ThrowsAsync<AuthenticationException>(() => VerifyClientAuthentication(credential));
        }

        private async Task VerifyClientAuthentication(NetworkCredential credential)
        {
            string serverName = Configuration.Security.NegotiateServer.Host;
            int port = Configuration.Security.NegotiateServer.Port;
            string serverSPN = "HOST/" + serverName;
            bool isLocalhost = await IsLocalHost(serverName);
            
            string expectedAuthenticationType = "Kerberos";
            bool mutualAuthenitcated = true;

            if (credential == CredentialCache.DefaultNetworkCredentials && isLocalhost)
            {
                expectedAuthenticationType = "NTLM";
            }
            else if (credential != CredentialCache.DefaultNetworkCredentials && 
                (string.IsNullOrEmpty(credential.UserName) || string.IsNullOrEmpty(credential.Password)))
            {
                // Anonymous authentication.
                expectedAuthenticationType = "NTLM";
                mutualAuthenitcated = false;
            }

            using (var client = new TcpClient())
            {
                await client.ConnectAsync(serverName, port);

                NetworkStream clientStream = client.GetStream();
                using (var auth = new NegotiateStream(clientStream, leaveInnerStreamOpen:false))
                {
                    await auth.AuthenticateAsClientAsync(
                        credential,
                        serverSPN,
                        ProtectionLevel.EncryptAndSign,
                        System.Security.Principal.TokenImpersonationLevel.Identification);

                    Assert.Equal(expectedAuthenticationType, auth.RemoteIdentity.AuthenticationType);
                    Assert.Equal(serverSPN, auth.RemoteIdentity.Name);

                    Assert.Equal(true, auth.IsAuthenticated);
                    Assert.Equal(true, auth.IsEncrypted);
                    Assert.Equal(mutualAuthenitcated, auth.IsMutuallyAuthenticated);
                    Assert.Equal(true, auth.IsSigned);

                    // Send a message to the server. Encode the test data into a byte array.
                    byte[] message = Encoding.UTF8.GetBytes("Hello from the client.");
                    await auth.WriteAsync(message, 0, message.Length);
                }
            }
        }

        [OuterLoop]
        // [Theory]
        // [ConditionalTheory(nameof(IsServerAndDomainAvailable))]
        [MemberData(nameof(GoodCredentialsData))]
        public async Task NegotiateStream_LocalServerAuthentication_Success(object credentialObject)
        {
            var credential = (NetworkCredential)credentialObject;
            await VerifyLocalServerAuthentication(credential);
        }

        [OuterLoop]
        // [Theory]
        // [ConditionalTheory(nameof(IsServerAndDomainAvailable))]
        [MemberData(nameof(BadCredentialsData))]
        public async Task NegotiateStream_LocalServerAuthentication_Fails(object credentialObject)
        {
            var credential = (NetworkCredential)credentialObject;
            await Assert.ThrowsAsync<AuthenticationException>(() => VerifyLocalServerAuthentication(credential));
        }

        private async Task VerifyLocalServerAuthentication(NetworkCredential credential)
        {
            string serverName = "chrross-udesk.crkerberos.com";// Configuration.Security.NegotiateServer.Host;
            int port = 54321;// Configuration.Security.NegotiateServer.Port;
            string serverSPN = "HTTP/" + serverName;
            
            string expectedAuthenticationType = "Kerberos";
            bool mutualAuthenitcated = true;
/*
            bool isLocalhost = false; // await IsLocalHost(serverName);
            if (credential == CredentialCache.DefaultNetworkCredentials && isLocalhost)
            {
                expectedAuthenticationType = "NTLM";
            }
            else if (credential != CredentialCache.DefaultNetworkCredentials && 
                (string.IsNullOrEmpty(credential.UserName) || string.IsNullOrEmpty(credential.Password)))
            {
                // Anonymous authentication.
                expectedAuthenticationType = "NTLM";
                mutualAuthenitcated = false;
            }
*/
            using (var client = new TcpClient())
            {
                var server = new TcpListener(IPAddress.Parse("10.121.100.113"), port);
                server.Start();
                try
                {
                    var acceptTask = server.AcceptTcpClientAsync();
                    await client.ConnectAsync(IPAddress.Parse("10.121.100.113"), port);

                    var serverConnection = await acceptTask;
                    var serverStream = serverConnection.GetStream();
/*
            var loop = true;
            while (loop)
            {
                await Task.Delay(1000);
            }
*/
                    using (var serverAuth = new NegotiateStream(serverStream, leaveInnerStreamOpen: false))
                    {
                        var clientStream = client.GetStream();
                        using (var clientAuth = new NegotiateStream(clientStream, leaveInnerStreamOpen:false))
                        {
                            var serverAuthTask = serverAuth.AuthenticateAsServerAsync();
                            var clientAuthTask = clientAuth.AuthenticateAsClientAsync(
                                credential,
                                serverSPN,
                                ProtectionLevel.None,
                                System.Security.Principal.TokenImpersonationLevel.Identification);

                            await serverAuthTask;

                            Assert.Equal(expectedAuthenticationType, serverAuth.RemoteIdentity.AuthenticationType);
                            Assert.Equal(serverSPN, serverAuth.RemoteIdentity.Name);

                            Assert.Equal(true, serverAuth.IsAuthenticated);
                            Assert.Equal(true, serverAuth.IsEncrypted);
                            Assert.Equal(mutualAuthenitcated, serverAuth.IsMutuallyAuthenticated);
                            Assert.Equal(true, serverAuth.IsSigned);

                            await clientAuthTask;

                            Assert.Equal(expectedAuthenticationType, clientAuth.RemoteIdentity.AuthenticationType);
                            Assert.Equal(serverSPN, clientAuth.RemoteIdentity.Name);

                            Assert.Equal(true, clientAuth.IsAuthenticated);
                            Assert.Equal(true, clientAuth.IsEncrypted);
                            Assert.Equal(mutualAuthenitcated, clientAuth.IsMutuallyAuthenticated);
                            Assert.Equal(true, clientAuth.IsSigned);

                            // Send a message to the server. Encode the test data into a byte array.
                            byte[] message = Encoding.UTF8.GetBytes("Hello from the client.");
                            await clientAuth.WriteAsync(message, 0, message.Length);
                        }
                    }
                }
                finally
                {
                    server.Stop();
                }
            }
        }

        [OuterLoop]
        [Theory]
        // [ConditionalTheory(nameof(IsServerAndDomainAvailable))]
        [MemberData(nameof(GoodCredentialsData))]
        public async Task NegotiateStream_RemoteServerAuthentication_Success(object credentialObject)
        {
            var credential = (NetworkCredential)credentialObject;
            await VerifyRemoteServerAuthentication(credential);
        }

        [OuterLoop]
        // [Theory]
        // [ConditionalTheory(nameof(IsServerAndDomainAvailable))]
        [MemberData(nameof(BadCredentialsData))]
        public async Task NegotiateStream_RemoteServerAuthentication_Fails(object credentialObject)
        {
            var credential = (NetworkCredential)credentialObject;
            await Assert.ThrowsAsync<AuthenticationException>(() => VerifyRemoteServerAuthentication(credential));
        }

        private async Task VerifyRemoteServerAuthentication(NetworkCredential credential)
        {
            string serverName = "chrross-udesk.crkerberos.com";// Configuration.Security.NegotiateServer.Host;
            int port = 54321;// Configuration.Security.NegotiateServer.Port;
            string serverSPN = "HTTP/" + serverName;
            
            string expectedAuthenticationType = "Kerberos";
            bool mutualAuthenitcated = true;
/*
            if (credential != CredentialCache.DefaultNetworkCredentials && 
                (string.IsNullOrEmpty(credential.UserName) || string.IsNullOrEmpty(credential.Password)))
            {
                // Anonymous authentication.
                expectedAuthenticationType = "NTLM";
                mutualAuthenitcated = false;
            }
*/
            var server = new TcpListener(IPAddress.Parse("10.121.100.113"), port);
            server.Start();
            try
            {                
                var acceptTask = server.AcceptTcpClientAsync();
                // TODO: Signal client to connect

                var serverConnection = await acceptTask;
                var serverStream = serverConnection.GetStream();
/*
                var loop = true;
                while (loop)
                {
                    await Task.Delay(1000);
                }
*/
                using (var serverAuth = new NegotiateStream(serverStream, leaveInnerStreamOpen: false))
                {
                    serverAuth.AuthenticateAsServer(
                        CredentialCache.DefaultNetworkCredentials,
                        ProtectionLevel.None,
                        TokenImpersonationLevel.Identification);

                    Assert.Equal(expectedAuthenticationType, serverAuth.RemoteIdentity.AuthenticationType);
                    Assert.Equal(serverSPN, serverAuth.RemoteIdentity.Name);

                    Assert.True(serverAuth.IsAuthenticated, "IsAuthenticated");
                    Assert.True(serverAuth.IsEncrypted, "IsEncrypted");
                    Assert.True(serverAuth.IsSigned, "IsSigned");
                    Assert.Equal(mutualAuthenitcated, serverAuth.IsMutuallyAuthenticated);

                    // Send a message to the server. Encode the test data into a byte array.
                    byte[] message = Encoding.UTF8.GetBytes("Hello from the server.");
                    await serverAuth.WriteAsync(message, 0, message.Length);
                }
            }
            finally
            {
                server.Stop();
            }
        }

        private async static Task<bool> IsLocalHost(string hostname)
        {
            IPAddress[] hostAddresses = await Dns.GetHostAddressesAsync(hostname);
            IPAddress[] localHostAddresses = await Dns.GetHostAddressesAsync(Dns.GetHostName());

            // Note: This returns 127.0.0.1 and ::1 but 127.0.0.0/8 which most systems consider localhost.
            IPAddress[] loopbackAddresses = await Dns.GetHostAddressesAsync("localhost");
            int isLocalHost = hostAddresses.Intersect(localHostAddresses).Count();
            int isLoopback = hostAddresses.Intersect(loopbackAddresses).Count();

            return isLocalHost + isLoopback > 0;
        }

        private static SecureString AsSecureString(string str)
        {
            SecureString secureString = new SecureString();

            if (string.IsNullOrEmpty(str))
            {
                return secureString;
            }

            foreach (char ch in str)
            {
                secureString.AppendChar(ch);
            }

            return secureString;
        }
    }
}
