// ==++==
// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==

using System;
using System.Diagnostics;
using System.Security;
using System.Diagnostics.Contracts;
using Microsoft.Win32.SafeHandles;

namespace System.Security.Cryptography
{
    /// <summary>
    ///     Wrapper for NCrypt's implementation of elliptic curve DSA
    /// </summary>
    internal sealed class ECDsaCng : ECDsa
    {
        private static KeySizes[] s_legalKeySizes = new KeySizes[] { new KeySizes(256, 384, 128), new KeySizes(521, 521, 0) };

        private CngKey _key;
        private CngAlgorithm _hashAlgorithm = CngAlgorithm.Sha256;

        //
        // Constructors
        //

        public ECDsaCng()
            : this(521)
        {
        }

        public ECDsaCng(int keySize)
        {

            if (!NCryptNative.NCryptSupported)
            {
                throw new PlatformNotSupportedException(SR.Cryptography_PlatformNotSupported);
            }

            KeySize = keySize;
        }

        [SecuritySafeCritical]
        public ECDsaCng(CngKey key)
        {
            Contract.Ensures(_key != null && _key.AlgorithmGroup == CngAlgorithmGroup.ECDsa);

            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            if (key.AlgorithmGroup != CngAlgorithmGroup.ECDsa)
            {
                throw new ArgumentException(SR.Cryptography_ArgECDsaRequiresECDsaKey, "key");
            }

            if (!NCryptNative.NCryptSupported)
            {
                throw new PlatformNotSupportedException(SR.Cryptography_PlatformNotSupported);
            }

            // Make a copy of the key so that we continue to work if it gets disposed before this algorithm
            //
            // This requires an assert for UnmanagedCode since we'll need to access the raw handles of the key
            // and the handle constructor of CngKey.  The assert is safe since ECDsaCng will never expose the
            // key handles to calling code (without first demanding UnmanagedCode via the Handle property of
            // CngKey).
            //
            // We also need to dispose of the key handle since CngKey.Handle returns a duplicate
            using (SafeNCryptKeyHandle keyHandle = key.Handle)
            {
                Key = CngKey.Open(keyHandle, key.IsEphemeral ? CngKeyHandleOpenOptions.EphemeralKey : CngKeyHandleOpenOptions.None);
            }

            KeySize = _key.KeySize;
        }

        // this API is very unfortunate. It invites allocations at each call.
        public override KeySizes[] LegalKeySizes
        {
            get
            {
                KeySizes[] ret = new KeySizes[s_legalKeySizes.Length];
                s_legalKeySizes.CopyTo(ret, 0);
                return ret;
            }
        }

        /// <summary>
        ///     Hash algorithm to use when generating a signature over arbitrary data
        /// </summary>
        internal CngAlgorithm HashAlgorithm
        {
            get
            {
                Contract.Ensures(Contract.Result<CngAlgorithm>() != null);
                return _hashAlgorithm;
            }

            set
            {
                Contract.Ensures(_hashAlgorithm != null);

                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _hashAlgorithm = value;
            }
        }

        /// <summary>
        ///     Key to use for signing
        /// </summary>
        public CngKey Key
        {
            get
            {
                Contract.Ensures(Contract.Result<CngKey>() != null);
                Contract.Ensures(Contract.Result<CngKey>().AlgorithmGroup == CngAlgorithmGroup.ECDsa);
                Contract.Ensures(_key != null && _key.AlgorithmGroup == CngAlgorithmGroup.ECDsa);

                // If the size of the key no longer matches our stored value, then we need to replace it with
                // a new key of the correct size.
                if (_key != null && _key.KeySize != KeySize)
                {
                    _key.Dispose();
                    _key = null;
                }

                if (_key == null)
                {
                    // Map the current key size to a CNG algorithm name
                    CngAlgorithm algorithm = null;
                    switch (KeySize)
                    {
                        case 256:
                        algorithm = CngAlgorithm.ECDsaP256;
                        break;

                        case 384:
                        algorithm = CngAlgorithm.ECDsaP384;
                        break;

                        case 521:
                        algorithm = CngAlgorithm.ECDsaP521;
                        break;

                        default:
                        Debug.Assert(false, "Illegal key size set");
                        break;
                    }

                    _key = CngKey.Create(algorithm);
                }

                return _key;
            }

            private set
            {
                Contract.Requires(value != null);
                Contract.Ensures(_key != null && _key.AlgorithmGroup == CngAlgorithmGroup.ECDsa);

                if (value.AlgorithmGroup != CngAlgorithmGroup.ECDsa)
                {
                    throw new ArgumentException(SR.Cryptography_ArgECDsaRequiresECDsaKey);
                }

                if (_key != null)
                {
                    _key.Dispose();
                }

                //
                // We do not duplicate the handle because the only time the user has access to the key itself
                // to dispose underneath us is when they construct via the CngKey constructor, which does a
                // copy. Otherwise all key lifetimes are controlled directly by the ECDsaCng class.
                //

                _key = value;
                KeySize = _key.KeySize;
            }
        }

        /// <summary>
        ///     Clean up the algorithm
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (_key != null)
                {
                    _key.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        [SecuritySafeCritical]
        public override byte[] SignHash(byte[] hash)
        {
            if (hash == null)
            {
                throw new ArgumentNullException("hash");
            }

            // This looks odd, but the key handle is actually a duplicate so we need to dispose it
            using (SafeNCryptKeyHandle keyHandle = Key.Handle)
            {
                return NCryptNative.SignHash(keyHandle, hash);
            }
        }

        [SecuritySafeCritical]
        public override bool VerifyHash(byte[] hash, byte[] signature)
        {
            if (hash == null)
            {
                throw new ArgumentNullException("hash");
            }
            if (signature == null)
            {
                throw new ArgumentNullException("signature");
            }

            // This looks odd, but Key.Handle is really a duplicate so we need to dispose it
            using (SafeNCryptKeyHandle keyHandle = Key.Handle)
            {
                return NCryptNative.VerifySignature(keyHandle, hash, signature);
            }
        }
    }
}
