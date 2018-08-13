// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Security.Cryptography;
using Internal.NativeCrypto;

namespace Internal.Cryptography
{
    internal static class RC4BCryptModes
    {
        internal static SafeAlgorithmHandle GetHandle(int effectiveKeyLength)
        {
            SafeAlgorithmHandle hAlg = Cng.BCryptOpenAlgorithmProvider(Cng.BCRYPT_RC4_ALGORITHM, null, Cng.OpenAlgorithmProviderFlags.NONE);

            if (effectiveKeyLength != 0)
            {
                Cng.SetEffectiveKeyLength(hAlg, effectiveKeyLength);
            }

            return hAlg;
        }
    }
}
