// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;

namespace System.Security.Cryptography
{
    /// <summary>
    ///     Utility class to strongly type algorithms used with CNG. Since all CNG APIs which require an
    ///     algorithm name take the name as a string, we use this string wrapper class to specifically mark
    ///     which parameters are expected to be algorithms.  We also provide a list of well known algorithm
    ///     names, which helps Intellisense users find a set of good algorithm names to use.
    /// </summary>
    public sealed class CngAlgorithm : IEquatable<CngAlgorithm>
    {
        public CngAlgorithm(string algorithm)
        {
            if (algorithm == null)
                throw new ArgumentNullException(nameof(algorithm));
            if (algorithm.Length == 0)
                throw new ArgumentException(SR.Format(SR.Cryptography_InvalidAlgorithmName, algorithm), nameof(algorithm));

            _algorithm = algorithm;
        }

        /// <summary>
        ///     Name of the algorithm
        /// </summary>
        public string Algorithm
        {
            get
            {
                return _algorithm;
            }
        }

        public static bool operator ==(CngAlgorithm left, CngAlgorithm right)
        {
            if (object.ReferenceEquals(left, null))
            {
                return object.ReferenceEquals(right, null);
            }

            return left.Equals(right);
        }

        public static bool operator !=(CngAlgorithm left, CngAlgorithm right)
        {
            if (object.ReferenceEquals(left, null))
            {
                return !object.ReferenceEquals(right, null);
            }

            return !left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            Debug.Assert(_algorithm != null);

            return Equals(obj as CngAlgorithm);
        }

        public bool Equals(CngAlgorithm other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }

            return _algorithm.Equals(other.Algorithm);
        }

        public override int GetHashCode()
        {
            Debug.Assert(_algorithm != null);
            return _algorithm.GetHashCode();
        }

        public override string ToString()
        {
            Debug.Assert(_algorithm != null);
            return _algorithm;
        }

        //
        // Well known algorithms
        //

        public static CngAlgorithm Rsa
        {
            get
            {
                return s_rsa ?? (s_rsa = new CngAlgorithm("RSA")); // BCRYPT_RSA_ALGORITHM
            }
        }

        public static CngAlgorithm ECDiffieHellmanP256
        {
            get
            {
                return s_ecdhp256 ?? (s_ecdhp256 = new CngAlgorithm("ECDH_P256")); // BCRYPT_ECDH_P256_ALGORITHM
            }
        }

        public static CngAlgorithm ECDiffieHellmanP384
        {
            get
            {
                return s_ecdhp384 ?? (s_ecdhp384 = new CngAlgorithm("ECDH_P384")); // BCRYPT_ECDH_P384_ALGORITHM
            }
        }

        public static CngAlgorithm ECDiffieHellmanP521
        {
            get
            {
                return s_ecdhp521 ?? (s_ecdhp521 = new CngAlgorithm("ECDH_P521")); // BCRYPT_ECDH_P521_ALGORITHM
            }
        }

        public static CngAlgorithm ECDsaP256
        {
            get
            {
                return s_ecdsap256 ?? (s_ecdsap256 = new CngAlgorithm("ECDSA_P256")); // BCRYPT_ECDSA_P256_ALGORITHM
            }
        }

        public static CngAlgorithm ECDsaP384
        {
            get
            {
                return s_ecdsap384 ?? (s_ecdsap384 = new CngAlgorithm("ECDSA_P384")); // BCRYPT_ECDSA_P384_ALGORITHM
            }
        }

        public static CngAlgorithm ECDsaP521
        {
            get
            {
                return s_ecdsap521 ?? (s_ecdsap521 = new CngAlgorithm("ECDSA_P521")); // BCRYPT_ECDSA_P521_ALGORITHM
            }
        }

        public static CngAlgorithm MD5
        {
            get
            {
                return s_md5 ?? (s_md5 = new CngAlgorithm("MD5")); // BCRYPT_MD5_ALGORITHM
            }
        }

        public static CngAlgorithm Sha1
        {
            get
            {
                return s_sha1 ?? (s_sha1 = new CngAlgorithm("SHA1")); // BCRYPT_SHA1_ALGORITHM
            }
        }

        public static CngAlgorithm Sha256
        {
            get
            {
                return s_sha256 ?? (s_sha256 = new CngAlgorithm("SHA256")); // BCRYPT_SHA256_ALGORITHM
            }
        }

        public static CngAlgorithm Sha384
        {
            get
            {
                return s_sha384 ?? (s_sha384 = new CngAlgorithm("SHA384")); // BCRYPT_SHA384_ALGORITHM
            }
        }

        public static CngAlgorithm Sha512
        {
            get
            {
                return s_sha512 ?? (s_sha512 = new CngAlgorithm("SHA512")); // BCRYPT_SHA512_ALGORITHM
            }
        }

        private static CngAlgorithm s_ecdhp256;
        private static CngAlgorithm s_ecdhp384;
        private static CngAlgorithm s_ecdhp521;
        private static CngAlgorithm s_ecdsap256;
        private static CngAlgorithm s_ecdsap384;
        private static CngAlgorithm s_ecdsap521;
        private static CngAlgorithm s_md5;
        private static CngAlgorithm s_sha1;
        private static CngAlgorithm s_sha256;
        private static CngAlgorithm s_sha384;
        private static CngAlgorithm s_sha512;
        private static CngAlgorithm s_rsa;

        private readonly string _algorithm;
    }
}

