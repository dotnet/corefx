// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Security.Cryptography
{
    internal sealed class RNGCryptoServiceProvider : RandomNumberGenerator
    {
        public sealed override void GetBytes(byte[] data)
        {
            ValidateGetBytesArgs(data);
            if (data.Length > 0)
            {
                if (!Interop.Crypto.GetRandomBytes(data, data.Length))
                {
                    throw Interop.Crypto.CreateOpenSslCryptographicException();
                }
            }
        }
    }
}
