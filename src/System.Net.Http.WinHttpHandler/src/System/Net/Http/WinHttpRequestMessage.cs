// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace System.Net.Http
{
    public class WinHttpRequestMessage : HttpRequestMessage
    {
        private DecompressionMethods _automaticDecompression = HttpHandlerDefaults.DefaultAutomaticDecompression;
        private Func<
            HttpRequestMessage,
            X509Certificate2,
            X509Chain,
            SslPolicyErrors,
            bool> _serverCertificateValidationCallback = null;
        private bool _checkCertificateRevocationList = false;
        private ClientCertificateOption _clientCertificateOption = ClientCertificateOption.Manual;
        private X509Certificate2Collection _clientCertificates = null; // Only create collection when required.

        public WinHttpRequestMessage() : base()
        {
        }
        
        public WinHttpRequestMessage(HttpMethod method, Uri requestUri) : base(method, requestUri)
        {
        }
        
        public WinHttpRequestMessage(HttpMethod method, string requestUri) : base(method, requestUri)
        {
        }

        public DecompressionMethods AutomaticDecompression
        {
            get
            {
                return _automaticDecompression;
            }

            set
            {
                _automaticDecompression = value;
            }
        }

        public Func<
            HttpRequestMessage,
            X509Certificate2,
            X509Chain,
            SslPolicyErrors,
            bool> ServerCertificateValidationCallback
        {
            get
            {
                return _serverCertificateValidationCallback;
            }

            set
            {
                _serverCertificateValidationCallback = value;
            }
        }

        public bool CheckCertificateRevocationList
        {
            get
            {
                return _checkCertificateRevocationList;
            }

            set
            {
                _checkCertificateRevocationList = value;
            }
        }

        public ClientCertificateOption ClientCertificateOption
        {
            get
            {
                return _clientCertificateOption;
            }

            set
            {
                if (value != ClientCertificateOption.Manual
                    && value != ClientCertificateOption.Automatic)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                _clientCertificateOption = value;
            }
        }

        public X509Certificate2Collection ClientCertificates
        {
            get
            {
                if (_clientCertificateOption != ClientCertificateOption.Manual)
                {
                    throw new InvalidOperationException(SR.Format(SR.net_http_invalid_enable_first, "ClientCertificateOptions", "Manual"));
                }

                if (_clientCertificates == null)
                {
                    _clientCertificates = new X509Certificate2Collection();
                }

                return _clientCertificates;
            }
        }
    }
}
