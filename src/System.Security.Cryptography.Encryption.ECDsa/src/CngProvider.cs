// ==++==
// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==

using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace System.Security.Cryptography
{
    /// <summary>
    ///     Utility class to strongly type providers used with CNG. Since all CNG APIs which require a
    ///     provider name take the name as a string, we use this string wrapper class to specifically mark
    ///     which parameters are expected to be providers.  We also provide a list of well known provider
    ///     names, which helps Intellisense users find a set of good providernames to use.
    /// </summary>
    internal sealed class CngProvider : IEquatable<CngProvider>
    {
        private static volatile CngProvider s_msSmartCardKsp;
        private static volatile CngProvider s_msSoftwareKsp;

        private string _provider;

        public CngProvider(string provider)
        {
            Contract.Ensures(!String.IsNullOrEmpty(_provider));

            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }
            if (provider.Length == 0)
            {
                throw new ArgumentException(SR.Format(SR.Cryptography_InvalidProviderName, provider), "provider");
            }

            _provider = provider;
        }

        /// <summary>
        ///     Name of the CNG provider
        /// </summary>
        public string Provider
        {
            get
            {
                Contract.Ensures(!String.IsNullOrEmpty(Contract.Result<string>()));
                return _provider;
            }
        }

        public static bool operator ==(CngProvider left, CngProvider right)
        {
            if (Object.ReferenceEquals(left, null))
            {
                return Object.ReferenceEquals(right, null);
            }

            return left.Equals(right);
        }

        [Pure]
        public static bool operator !=(CngProvider left, CngProvider right)
        {
            if (Object.ReferenceEquals(left, null))
            {
                return !Object.ReferenceEquals(right, null);
            }

            return !left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            Contract.Assert(_provider != null);

            return Equals(obj as CngProvider);
        }

        public bool Equals(CngProvider other)
        {
            if (Object.ReferenceEquals(other, null))
            {
                return false;
            }

            return _provider.Equals(other.Provider);
        }

        public override int GetHashCode()
        {
            Contract.Assert(_provider != null);
            return _provider.GetHashCode();
        }

        public override string ToString()
        {
            Contract.Assert(_provider != null);
            return _provider.ToString();
        }

        //
        // Well known NCrypt KSPs
        //

        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "CardKey", Justification = "This is not 'Smart Cardkey', but 'Smart Card Key'")]
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "SmartCard", Justification = "Smart Card is two words in the ncrypt usage")]
        public static CngProvider MicrosoftSmartCardKeyStorageProvider
        {
            get
            {
                Contract.Ensures(Contract.Result<CngProvider>() != null);

                if (s_msSmartCardKsp == null)
                {
                    s_msSmartCardKsp = new CngProvider("Microsoft Smart Card Key Storage Provider");        // MS_SMART_CARD_KEY_STORAGE_PROVIDER
                }

                return s_msSmartCardKsp;
            }
        }

        public static CngProvider MicrosoftSoftwareKeyStorageProvider
        {
            get
            {
                Contract.Ensures(Contract.Result<CngProvider>() != null);

                if (s_msSoftwareKsp == null)
                {
                    s_msSoftwareKsp = new CngProvider("Microsoft Software Key Storage Provider");           // MS_KEY_STORAGE_PROVIDER
                }

                return s_msSoftwareKsp;
            }
        }
    }
}
