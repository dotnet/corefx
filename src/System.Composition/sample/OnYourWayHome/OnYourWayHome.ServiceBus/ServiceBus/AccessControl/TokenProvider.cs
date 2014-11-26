//---------------------------------------------------------------------------------
// Copyright (c), Microsoft Corporation
//---------------------------------------------------------------------------------

namespace OnYourWayHome.AccessControl
{
    using System;
    using OnYourWayHome.ServiceBus;

    public sealed class TokenProvider
    {
        private static readonly TimeSpan minimumTokenLifetime = TimeSpan.FromMinutes(1);

        private readonly TokenRequest tokenRequest;
        private readonly string serviceNamespace;

        private SimpleWebToken cachedToken;

        private TokenProvider(string serviceNamespace, TokenRequest tokenRequest)
        {
            if (tokenRequest == null)
            {
                throw new ArgumentNullException("tokenRequest");
            }

            this.tokenRequest = tokenRequest;

            if (string.IsNullOrEmpty(serviceNamespace))
            {
                throw new ArgumentNullException("serviceNamespace");
            }

            this.serviceNamespace = serviceNamespace;
        }

        public string ServiceNamespace
        {
            get { return this.serviceNamespace; }
        }

        private bool CachedTokenHasExpired
        {
            get
            {
                return this.cachedToken == null
                    || this.cachedToken.ExpiresAt < DateTime.UtcNow.Subtract(minimumTokenLifetime);
            }
        }

        public static TokenProvider CreateUsernameAndPasswordTokenProvider(string serviceNamespace, string username, string password)
        {
            return CreateUsernameAndPasswordTokenProvider(serviceNamespace, username, password, TokenRequestFormat.Wrap);
        }

        public static TokenProvider CreateUsernameAndPasswordTokenProvider(string serviceNamespace, string username, string password, TokenRequestFormat format)
        {
            var relyingPartyAddress = ServiceBusEnvironment.CreateDefaultServiceRealmUri(serviceNamespace);

            switch (format)
            {
                case TokenRequestFormat.Wrap:
                    return new TokenProvider(serviceNamespace, TokenRequest.CreateWrapUsernameAndPasswordRequest(username, password, relyingPartyAddress));

                case TokenRequestFormat.OAuth2:
                    return new TokenProvider(serviceNamespace, TokenRequest.CreateOAuth2UsernameAndPasswordRequest(username, password, relyingPartyAddress));

                default:
                    throw new NotSupportedException("The token format '" + format.ToString() + "' is unknown.");
            }
        }

        public static TokenProvider CreateSharedSecretTokenProvider(string serviceNamespace, string issuerName, string issuerSecret)
        {
            return CreateSharedSecretTokenProvider(serviceNamespace, issuerName, issuerSecret, TokenRequestFormat.Wrap);
        }

        public static TokenProvider CreateSharedSecretTokenProvider(string serviceNamespace, string issuerName, string issuerSecret, TokenRequestFormat format)
        {
            var relyingPartyAddress = ServiceBusEnvironment.CreateDefaultServiceRealmUri(serviceNamespace);

            switch (format)
            {
                case TokenRequestFormat.Wrap:
                    return new TokenProvider(serviceNamespace, TokenRequest.CreateWrapSharedSecretRequest(issuerName, issuerSecret, relyingPartyAddress));

                case TokenRequestFormat.OAuth2:
                    return new TokenProvider(serviceNamespace, TokenRequest.CreateOAuth2SharedSecretRequest(issuerName, issuerSecret, relyingPartyAddress));

                default:
                    throw new NotSupportedException("The token format '" + format.ToString() + "' is unknown.");
            }
        }

        public IAsyncResult BeginGetToken(bool bypassCache, AsyncCallback callback, object state)
        {
            AsyncResult<SimpleWebToken> getTokenAsyncResult;

            if (bypassCache || this.CachedTokenHasExpired)
            {
                getTokenAsyncResult = new AccessControlServiceTokenRequestAsyncResult(this.serviceNamespace, this.tokenRequest);
                getTokenAsyncResult.BeginInvoke(callback, state);
            }
            else
            {
                getTokenAsyncResult = new GetCachedTokenAsyncResult(this.cachedToken, callback, state);
            }

            return getTokenAsyncResult;
        }

        public SimpleWebToken EndGetToken(IAsyncResult ar)
        {
            var asyncResult = (AsyncResult<SimpleWebToken>)ar;

            this.cachedToken = asyncResult.EndInvoke();
            return this.cachedToken;
        }

        private class GetCachedTokenAsyncResult : AsyncResult<SimpleWebToken>
        {
            public GetCachedTokenAsyncResult(SimpleWebToken token, AsyncCallback callback, object state)
            {
                this.Callback = callback;
                this.AsyncState = state;
                this.Result = token;

                this.SetCompleted(null, true);
            }
        }
    }
}
