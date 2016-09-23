// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Security.Cryptography;

namespace Internal.Cryptography
{
    internal static class HashAlgorithmNames
    {
        // These are accepted by CNG
        public const string MD5 = "MD5";
        public const string SHA1 = "SHA1";
        public const string SHA256 = "SHA256";
        public const string SHA384 = "SHA384";
        public const string SHA512 = "SHA512";

        private const string ALL_NAMES = "MD5 SHA1 SHA256 SHA384 SHA512";

        /// <summary>
        /// Map HashAlgorithm type to string; desktop uses CryptoConfig functionality.
        /// </summary>
        public static string ToAlgorithmName(this HashAlgorithm hashAlgorithm)
        {
            if (hashAlgorithm is SHA1)
                return HashAlgorithmNames.SHA1;
            if (hashAlgorithm is SHA256)
                return HashAlgorithmNames.SHA256;
            if (hashAlgorithm is SHA384)
                return HashAlgorithmNames.SHA384;
            if (hashAlgorithm is SHA512)
                return HashAlgorithmNames.SHA512;
            if (hashAlgorithm is MD5)
                return HashAlgorithmNames.MD5;

            // Fallback to ToString() which can be extended by derived classes
            return hashAlgorithm.ToString();
        }

        /// <summary>
        /// Uppercase known hash algorithms. BCrypt is case-sensitive and requires uppercase.
        /// </summary>
        public static string ToUpper(string hashAlgorithName)
        {
            if (ALL_NAMES.IndexOf(hashAlgorithName, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return hashAlgorithName.ToUpperInvariant();
            }
            return hashAlgorithName;
        }
    }
}
