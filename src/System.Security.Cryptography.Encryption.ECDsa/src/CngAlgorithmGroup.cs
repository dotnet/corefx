// ==++==
// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==

using System;
using System.Diagnostics.Contracts;

namespace System.Security.Cryptography
{
    /// <summary>
    ///     Utility class to strongly type algorithm groups used with CNG. Since all CNG APIs which require or
    ///     return an algorithm group name take the name as a string, we use this string wrapper class to
    ///     specifically mark which parameters and return values are expected to be algorithm groups.  We also
    ///     provide a list of well known algorithm group names, which helps Intellisense users find a set of
    ///     good algorithm group names to use.
    /// </summary>
    internal sealed class CngAlgorithmGroup : IEquatable<CngAlgorithmGroup>
    {
        private static volatile CngAlgorithmGroup s_dh;
        private static volatile CngAlgorithmGroup s_dsa;
        private static volatile CngAlgorithmGroup s_ecdh;
        private static volatile CngAlgorithmGroup s_ecdsa;
        private static volatile CngAlgorithmGroup s_rsa;

        private string _algorithmGroup;

        public CngAlgorithmGroup(string algorithmGroup)
        {
            Contract.Ensures(!String.IsNullOrEmpty(_algorithmGroup));

            if (algorithmGroup == null)
            {
                throw new ArgumentNullException("algorithmGroup");
            }
            if (algorithmGroup.Length == 0)
            {
                throw new ArgumentException(SR.Format(SR.Cryptography_InvalidAlgorithmGroup, algorithmGroup), "algorithmGroup");
            }

            _algorithmGroup = algorithmGroup;
        }

        /// <summary>
        ///     Name of the algorithm group
        /// </summary>
        public string AlgorithmGroup
        {
            get
            {
                Contract.Ensures(!String.IsNullOrEmpty(Contract.Result<string>()));
                return _algorithmGroup;
            }
        }

        [Pure]
        public static bool operator ==(CngAlgorithmGroup left, CngAlgorithmGroup right)
        {
            if (Object.ReferenceEquals(left, null))
            {
                return Object.ReferenceEquals(right, null);
            }


            return left.Equals(right);
        }

        [Pure]
        public static bool operator !=(CngAlgorithmGroup left, CngAlgorithmGroup right)
        {
            if (Object.ReferenceEquals(left, null))
            {
                return !Object.ReferenceEquals(right, null);
            }

            return !left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            Contract.Assert(_algorithmGroup != null);

            return Equals(obj as CngAlgorithmGroup);
        }

        public bool Equals(CngAlgorithmGroup other)
        {
            if (Object.ReferenceEquals(other, null))
            {
                return false;
            }

            return _algorithmGroup.Equals(other.AlgorithmGroup);
        }

        public override int GetHashCode()
        {
            Contract.Assert(_algorithmGroup != null);
            return _algorithmGroup.GetHashCode();
        }

        public override string ToString()
        {
            Contract.Assert(_algorithmGroup != null);
            return _algorithmGroup;
        }

        //
        // Well known algorithm groups
        //

        public static CngAlgorithmGroup DiffieHellman
        {
            get
            {
                Contract.Ensures(Contract.Result<CngAlgorithmGroup>() != null);

                if (s_dh == null)
                {
                    s_dh = new CngAlgorithmGroup("DH");                 // NCRYPT_DH_ALGORITHM_GROUP
                }

                return s_dh;
            }
        }

        public static CngAlgorithmGroup Dsa
        {
            get
            {
                Contract.Ensures(Contract.Result<CngAlgorithmGroup>() != null);

                if (s_dsa == null)
                {
                    s_dsa = new CngAlgorithmGroup("DSA");               // NCRYPT_DSA_ALGORITHM_GROUP
                }

                return s_dsa;
            }
        }

        public static CngAlgorithmGroup ECDiffieHellman
        {
            [Pure]
            get
            {
                Contract.Ensures(Contract.Result<CngAlgorithmGroup>() != null);

                if (s_ecdh == null)
                {
                    s_ecdh = new CngAlgorithmGroup("ECDH");             // NCRYPT_ECDH_ALGORITHM_GROUP
                }

                return s_ecdh;
            }
        }

        public static CngAlgorithmGroup ECDsa
        {
            [Pure]
            get
            {
                Contract.Ensures(Contract.Result<CngAlgorithmGroup>() != null);

                if (s_ecdsa == null)
                {
                    s_ecdsa = new CngAlgorithmGroup("ECDSA");           // NCRYPT_ECDSA_ALGORITHM_GROUP
                }

                return s_ecdsa;
            }
        }

        public static CngAlgorithmGroup Rsa
        {
            get
            {
                Contract.Ensures(Contract.Result<CngAlgorithmGroup>() != null);

                if (s_rsa == null)
                {
                    s_rsa = new CngAlgorithmGroup("RSA");               // NCRYPT_RSA_ALGORITHM_GROUP
                }

                return s_rsa;
            }
        }
    }
}
