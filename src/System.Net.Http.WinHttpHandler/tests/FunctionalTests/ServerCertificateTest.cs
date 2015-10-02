// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Net.Tests;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

using Xunit;
using Xunit.Abstractions;

namespace System.Net.Http.WinHttpHandlerFunctional.Tests
{
    public class ServerCertificateTest
    {
        private readonly ITestOutputHelper _output;
        private readonly ValidationCallbackHistory _validationCallbackHistory;

        public ServerCertificateTest(ITestOutputHelper output)
        {
            _output = output;
            _validationCallbackHistory = new ValidationCallbackHistory();
        }

        [Fact]
        public async Task NoCallback_ValidCertificate_CallbackNotCalled()
        {
            var handler = new WinHttpHandler();
            using (var client = new HttpClient(handler))
            {
                HttpResponseMessage response = await client.GetAsync(HttpTestServers.SecureRemoteGetServer);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.False(_validationCallbackHistory.WasCalled);
            }
        }

        [Fact]
        public async Task UseCallback_NotSecureConnection_CallbackNotCalled()
        {
            var handler = new WinHttpHandler();
            handler.ServerCertificateValidationCallback = CustomServerCertificateValidationCallback;
            using (var client = new HttpClient(handler))
            {
                HttpResponseMessage response = await client.GetAsync(HttpTestServers.RemoteGetServer);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.False(_validationCallbackHistory.WasCalled);
            }
        }

        [Fact]
        public async Task UseCallback_ValidCertificate_ExpectedValuesDuringCallback()
        {
            var handler = new WinHttpHandler();
            handler.ServerCertificateValidationCallback = CustomServerCertificateValidationCallback;
            using (var client = new HttpClient(handler))
            {
                HttpResponseMessage response = await client.GetAsync(HttpTestServers.SecureRemoteGetServer);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(_validationCallbackHistory.WasCalled);
                
                ConfirmValidCertificate(HttpTestServers.Host);
            }
        }

        [Fact]
        public async Task UseCallback_RedirectandValidCertificate_ExpectedValuesDuringCallback()
        {
            Uri uri = HttpTestServers.RedirectUriForDestinationUri(HttpTestServers.SecureRemoteGetServer);

            var handler = new WinHttpHandler();
            handler.ServerCertificateValidationCallback = CustomServerCertificateValidationCallback;
            using (var client = new HttpClient(handler))
            {
                HttpResponseMessage response = await client.GetAsync(uri);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(_validationCallbackHistory.WasCalled);
                
                ConfirmValidCertificate(HttpTestServers.Host);
            }
        }

        [Fact]
        public async Task UseCallback_CallbackReturnsFailure_ThrowsInnerSecurityFailureException()
        {
            const int ERROR_WINHTTP_SECURE_FAILURE = 12175;

            var handler = new WinHttpHandler();
            handler.ServerCertificateValidationCallback = CustomServerCertificateValidationCallback;
            using (var client = new HttpClient(handler))
            {
                var request = new HttpRequestMessage(HttpMethod.Get, HttpTestServers.SecureRemoteGetServer);
                _validationCallbackHistory.ReturnFailure = true;
                HttpRequestException ex = await Assert.ThrowsAsync<HttpRequestException>(() =>
                    client.GetAsync(HttpTestServers.SecureRemoteGetServer));
                var innerEx = (Win32Exception)ex.InnerException;
                Assert.Equal(ERROR_WINHTTP_SECURE_FAILURE, innerEx.NativeErrorCode);
            }
        }

        [Fact]
        public async Task UseCallback_CallbackThrowsSpecificException_ThrowsInnerSpecificException()
        {
            var handler = new WinHttpHandler();
            handler.ServerCertificateValidationCallback = CustomServerCertificateValidationCallback;
            using (var client = new HttpClient(handler))
            {
                _validationCallbackHistory.ThrowException = true;
                await Assert.ThrowsAsync<CustomException>(() => client.GetAsync(HttpTestServers.SecureRemoteGetServer));
            }
        }

        private void ConfirmValidCertificate(string expectedHostName)
        {
                Assert.Equal(SslPolicyErrors.None, _validationCallbackHistory.SslPolicyErrors);
                Assert.True(_validationCallbackHistory.CertificateChain.Count > 0);
                _output.WriteLine("Certificate.Subject: {0}", _validationCallbackHistory.Certificate.Subject);
                _output.WriteLine("Expected HostName: {0}", expectedHostName);
                Assert.Contains(expectedHostName, _validationCallbackHistory.Certificate.Subject);
        }
        
        private bool CustomServerCertificateValidationCallback(
            HttpRequestMessage sender,
            X509Certificate2 certificate,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            _validationCallbackHistory.WasCalled = true;
            _validationCallbackHistory.Certificate = certificate;
            foreach (var element in chain.ChainElements)
            {
                _validationCallbackHistory.CertificateChain.Add(element.Certificate);
            }
            _validationCallbackHistory.ChainStatus = chain.ChainStatus;
            _validationCallbackHistory.SslPolicyErrors = sslPolicyErrors;

            if (_validationCallbackHistory.ThrowException)
            {
                throw new CustomException();
            }

            if (_validationCallbackHistory.ReturnFailure)
            {
                return false;
            }

            return true;
        }

        public class CustomException : Exception
        {
            public CustomException()
            {
            }
        }
        
        public class ValidationCallbackHistory
        {
            public bool ThrowException;
            public bool ReturnFailure;
            public bool WasCalled;
            public SslPolicyErrors SslPolicyErrors;
            public X509Certificate2 Certificate;
            public X509CertificateCollection CertificateChain;
            public X509ChainStatus[] ChainStatus;

            public ValidationCallbackHistory()
            {
                ThrowException = false;
                ReturnFailure = false;
                WasCalled = false;
                SslPolicyErrors = SslPolicyErrors.None;
                Certificate = null;
                CertificateChain = new X509CertificateCollection();
                ChainStatus = null;
            }
        }
    }
}
