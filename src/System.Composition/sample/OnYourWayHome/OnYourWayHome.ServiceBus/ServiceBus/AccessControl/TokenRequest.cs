//---------------------------------------------------------------------------------
// Copyright (c), Microsoft Corporation
//---------------------------------------------------------------------------------

namespace OnYourWayHome.AccessControl
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using OnYourWayHome.ServiceBus;

    internal sealed class TokenRequest
    {
        private readonly Dictionary<string, string> parameters;

        public TokenRequest()
        {
            this.parameters = new Dictionary<string, string>();
        }

        public TokenRequestFormat Format { get; private set; }

        public string Path { get; private set; }

        public string Body
        {
            get { return this.parameters.EncodeAsHttpForm(); }
        }

        private Dictionary<string, string> Parameters
        {
            get { return this.parameters; }
        }

        public static TokenRequest CreateOAuth2UsernameAndPasswordRequest(string issuerName, string issuerPassword, Uri relyingPartyAddress)
        {
            return new TokenRequest
            {
                Format = TokenRequestFormat.OAuth2,
                Path = "v2/OAuth2-13",
                Parameters = 
                    {
                        { "grant_type", "client_credentials" },
                        { "client_id", issuerName },
                        { "client_secret", issuerPassword },
                        { "scope", relyingPartyAddress.ToString() },
                    }
            };
        }

        public static TokenRequest CreateOAuth2SharedSecretRequest(string issuerName, string issuerSecret, Uri relyingPartyAddress)
        {
            var token = GetSharedSecretToken(issuerName, issuerSecret);

            return new TokenRequest
            {
                Format = TokenRequestFormat.OAuth2,
                Path = "v2/OAuth2-13",
                Parameters = 
                    {
                        { "grant_type", "http://schemas.xmlsoap.org/ws/2009/11/swt-token-profile-1.0" },
                        { "assertion", token },
                        { "scope", relyingPartyAddress.ToString() },
                    }
            };
        }

        public static TokenRequest CreateWrapUsernameAndPasswordRequest(string issuerName, string issuerPassword, Uri relyingPartyAddress)
        {
            return new TokenRequest
            {
                Format = TokenRequestFormat.Wrap,
                Path = "WRAPv0.9/",
                Parameters = 
                    {
                        { "wrap_scope", relyingPartyAddress.ToString() },
                        { "wrap_name", issuerName },
                        { "wrap_password", issuerPassword },
                    }
            };
        }

        public static TokenRequest CreateWrapSharedSecretRequest(string issuerName, string issuerSecret, Uri relyingPartyAddress)
        {
            var token = GetSharedSecretToken(issuerName, issuerSecret);

            return new TokenRequest
            {
                Format = TokenRequestFormat.Wrap,
                Path = "WRAPv0.9/",
                Parameters = 
                    {
                        { "wrap_scope", relyingPartyAddress.ToString() },
                        { "wrap_assertion_format", "SWT" },
                        { "wrap_assertion", token },
                    }
            };
        }

        private static string GetSharedSecretToken(string issuerName, string issuerSecret)
        {
            byte[] issuerSecretBytes = Convert.FromBase64String(issuerSecret);
            string token = "Issuer=" + Uri.EscapeDataString(issuerName);
            string signature;

            var adapter = ServiceBusAdapter.Current;
            byte[] signatureBytes = adapter.ComputeHmacSha256(issuerSecretBytes, Encoding.UTF8.GetBytes(token));

                signature = Convert.ToBase64String(signatureBytes);

            var signedToken = token + "&HMACSHA256=" + Uri.EscapeDataString(signature);
            return signedToken;
        }
    }
}
