// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Security.Cryptography
{
    internal sealed class RNGCryptoServiceProvider : RandomNumberGenerator
    {
        public sealed override void GetBytes(byte[] data)
        {
            ValidateGetBytesArgs(data);
            if (data.Length > 0)
            {
                Interop.BCrypt.NTSTATUS status = Interop.BCrypt.BCryptGenRandom(data, data.Length);
                if (status != Interop.BCrypt.NTSTATUS.STATUS_SUCCESS)
                    throw Interop.BCrypt.CreateCryptographicException(status);
            }
        }
    }
}
