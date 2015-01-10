//---------------------------------------------------------------------------------
// Copyright (c), Microsoft Corporation
//---------------------------------------------------------------------------------

namespace OnYourWayHome.AccessControl
{
    using System;
    using System.Collections.Generic;
    using OnYourWayHome.ServiceBus;

    public sealed class SimpleWebToken
    {
        private const string ExpiresOnClaim = "ExpiresOn";
        private const string AudienceClaim = "Audience";
        private const string IssuerClaim = "Issuer";
        private const string NameIdentifierClaim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
        private const string IdentityProviderClaim = "http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider";

        private readonly IDictionary<string, string> claims = new Dictionary<string, string>();

        private static DateTime unixEpochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);

        private string tokenString;
        private DateTime expires;

        public SimpleWebToken(string tokenString)
        {
            this.Update(tokenString);
        }

        public string TokenString
        {
            get { return this.tokenString; }
        }

        public DateTime ExpiresAt
        {
            get { return this.expires; }
        }

        public string Audience
        {
            get { return this[AudienceClaim]; }
        }

        public string Issuer
        {
            get { return this[IssuerClaim]; }
        }

        public string NameIdentifier
        {
            get { return this[NameIdentifierClaim]; }
        }

        public string IdentityProvider
        {
            get { return this[IdentityProviderClaim]; }
        }

        public IEnumerable<string> ClaimNames
        {
            get { return this.claims.Keys; }
        }

        public string this[string claim]
        {
            get { return this.claims.ContainsKey(claim) ? this.claims[claim] : null; }
        }

        public override string ToString()
        {
            return this.tokenString;
        }

        public void Update(string tokenString)
        {
            this.tokenString = tokenString;

            this.claims.Clear();
            foreach (var item in tokenString.DecodeHttpForm())
            {
                this.claims.Add(item.Key, item.Value);
            }

            this.expires = this.ComputeExpiryTime();
        }

        private DateTime ComputeExpiryTime()
        {
            int unixTicksExpiryTime;
            string expiresOn = this[ExpiresOnClaim];

            if (!string.IsNullOrEmpty(expiresOn) && int.TryParse(expiresOn, out unixTicksExpiryTime))
            {
                return unixEpochStart.AddSeconds(unixTicksExpiryTime);
            }

            return DateTime.MaxValue;
        }
    }
}
