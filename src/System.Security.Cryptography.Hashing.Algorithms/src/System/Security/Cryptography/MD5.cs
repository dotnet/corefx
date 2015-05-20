// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;

using Internal.Cryptography;

namespace System.Security.Cryptography
{
    //
    // If you change anything in this class, you must make the same change in the other *Provider classes. This is a pain but given that the
    // preexisting contract from the desktop locks all of these into deriving directly from the abstract HashAlgorithm class, 
    // it can't be helped.
    //

    public abstract class MD5 : HashAlgorithm
    {
        protected MD5()
        {
        }

        public static MD5 Create()
        {
            return new Implementation();
        }

        private sealed class Implementation : MD5
        {
            public Implementation()
            {
                _hashProvider = HashProviderDispenser.CreateHashProvider(HashAlgorithmNames.MD5);
            }

            public sealed override int HashSize
            {
                get
                {
                    return _hashProvider.HashSizeInBytes * 8;
                }
            }

            protected sealed override void HashCore(byte[] array, int ibStart, int cbSize)
            {
                _hashProvider.AppendHashData(array, ibStart, cbSize);
            }

            protected sealed override byte[] HashFinal()
            {
                return _hashProvider.FinalizeHashAndReset();
            }

            public sealed override void Initialize()
            {
                // Nothing to do here. We expect HashAlgorithm to invoke HashFinal() and Initialize() as a pair. This reflects the 
                // reality that our native crypto providers (e.g. CNG) expose hash finalization and object reinitialization as an atomic operation.
                return;
            }

            protected sealed override void Dispose(bool disposing)
            {
                _hashProvider.Dispose(disposing);
                base.Dispose(disposing);
            }

            private readonly HashProvider _hashProvider;
        }
    }
}
