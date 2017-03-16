// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Internal.NativeCrypto;
using System;
using System.Diagnostics.Contracts;

namespace System.Security.Cryptography
{
    /// <summary>
    ///     Utility class to strongly type algorithms used with CNG. Since all CNG APIs which require an
    ///     algorithm name take the name as a string, we use this string wrapper class to specifically mark
    ///     which parameters are expected to be algorithms.  We also provide a list of well known algorithm
    ///     names, which helps Intellisense users find a set of good algorithm names to use.
    /// </summary>
    internal sealed class CngAlgorithm : IEquatable<CngAlgorithm>
    {
        private static volatile CngAlgorithm s_ecdhp256;
        private static volatile CngAlgorithm s_ecdhp384;
        private static volatile CngAlgorithm s_ecdhp521;
        private static volatile CngAlgorithm s_ecdsap256;
        private static volatile CngAlgorithm s_ecdsap384;
        private static volatile CngAlgorithm s_ecdsap521;
        private static volatile CngAlgorithm s_md5;
        private static volatile CngAlgorithm s_sha1;
        private static volatile CngAlgorithm s_sha256;
        private static volatile CngAlgorithm s_sha384;
        private static volatile CngAlgorithm s_sha512;

        private string _algorithm;

        public CngAlgorithm(string algorithm)
        {
            Contract.Ensures(!String.IsNullOrEmpty(_algorithm));

            if (algorithm == null)
            {
                throw new ArgumentNullException("algorithm");
            }
            if (algorithm.Length == 0)
            {
                throw new ArgumentException(SR.Format(SR.Cryptography_InvalidAlgorithmName, algorithm), "algorithm");
            }

            _algorithm = algorithm;
        }

        /// <summary>
        ///     Name of the algorithm
        /// </summary>
        public string Algorithm
        {
            get
            {
                Contract.Ensures(!String.IsNullOrEmpty(Contract.Result<string>()));
                return _algorithm;
            }
        }

        public static bool operator ==(CngAlgorithm left, CngAlgorithm right)
        {
            if (Object.ReferenceEquals(left, null))
            {
                return Object.ReferenceEquals(right, null);
            }

            return left.Equals(right);
        }

        [Pure]
        public static bool operator !=(CngAlgorithm left, CngAlgorithm right)
        {
            if (Object.ReferenceEquals(left, null))
            {
                return !Object.ReferenceEquals(right, null);
            }

            return !left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            Contract.Assert(_algorithm != null);

            return Equals(obj as CngAlgorithm);
        }

        public bool Equals(CngAlgorithm other)
        {
            if (Object.ReferenceEquals(other, null))
            {
                return false;
            }

            return _algorithm.Equals(other.Algorithm);
        }

        public override int GetHashCode()
        {
            Contract.Assert(_algorithm != null);
            return _algorithm.GetHashCode();
        }

        public override string ToString()
        {
            Contract.Assert(_algorithm != null);
            return _algorithm;
        }

        //
        // Well known algorithms
        //

        public static CngAlgorithm ECDiffieHellmanP256
        {
            get
            {
                Contract.Ensures(Contract.Result<CngAlgorithm>() != null);

                if (s_ecdhp256 == null)
                {
                    s_ecdhp256 = new CngAlgorithm(BCryptNative.AlgorithmName.ECDHP256);
                }

                return s_ecdhp256;
            }
        }

        public static CngAlgorithm ECDiffieHellmanP384
        {
            get
            {
                Contract.Ensures(Contract.Result<CngAlgorithm>() != null);

                if (s_ecdhp384 == null)
                {
                    s_ecdhp384 = new CngAlgorithm(BCryptNative.AlgorithmName.ECDHP384);
                }

                return s_ecdhp384;
            }
        }

        public static CngAlgorithm ECDiffieHellmanP521
        {
            get
            {
                Contract.Ensures(Contract.Result<CngAlgorithm>() != null);

                if (s_ecdhp521 == null)
                {
                    s_ecdhp521 = new CngAlgorithm(BCryptNative.AlgorithmName.ECDHP521);
                }

                return s_ecdhp521;
            }
        }

        public static CngAlgorithm ECDsaP256
        {
            get
            {
                Contract.Ensures(Contract.Result<CngAlgorithm>() != null);

                if (s_ecdsap256 == null)
                {
                    s_ecdsap256 = new CngAlgorithm(BCryptNative.AlgorithmName.ECDsaP256);
                }

                return s_ecdsap256;
            }
        }

        public static CngAlgorithm ECDsaP384
        {
            get
            {
                Contract.Ensures(Contract.Result<CngAlgorithm>() != null);

                if (s_ecdsap384 == null)
                {
                    s_ecdsap384 = new CngAlgorithm(BCryptNative.AlgorithmName.ECDsaP384);
                }

                return s_ecdsap384;
            }
        }

        public static CngAlgorithm ECDsaP521
        {
            get
            {
                Contract.Ensures(Contract.Result<CngAlgorithm>() != null);

                if (s_ecdsap521 == null)
                {
                    s_ecdsap521 = new CngAlgorithm(BCryptNative.AlgorithmName.ECDsaP521);
                }

                return s_ecdsap521;
            }
        }

        public static CngAlgorithm MD5
        {
            get
            {
                Contract.Ensures(Contract.Result<CngAlgorithm>() != null);

                if (s_md5 == null)
                {
                    s_md5 = new CngAlgorithm(BCryptNative.AlgorithmName.MD5);
                }

                return s_md5;
            }
        }

        public static CngAlgorithm Sha1
        {
            get
            {
                Contract.Ensures(Contract.Result<CngAlgorithm>() != null);

                if (s_sha1 == null)
                {
                    s_sha1 = new CngAlgorithm(BCryptNative.AlgorithmName.Sha1);
                }

                return s_sha1;
            }
        }

        public static CngAlgorithm Sha256
        {
            get
            {
                Contract.Ensures(Contract.Result<CngAlgorithm>() != null);

                if (s_sha256 == null)
                {
                    s_sha256 = new CngAlgorithm(BCryptNative.AlgorithmName.Sha256);
                }

                return s_sha256;
            }
        }

        public static CngAlgorithm Sha384
        {
            get
            {
                Contract.Ensures(Contract.Result<CngAlgorithm>() != null);

                if (s_sha384 == null)
                {
                    s_sha384 = new CngAlgorithm(BCryptNative.AlgorithmName.Sha384);
                }

                return s_sha384;
            }
        }

        public static CngAlgorithm Sha512
        {
            get
            {
                Contract.Ensures(Contract.Result<CngAlgorithm>() != null);

                if (s_sha512 == null)
                {
                    s_sha512 = new CngAlgorithm(BCryptNative.AlgorithmName.Sha512);
                }

                return s_sha512;
            }
        }
    }
}
