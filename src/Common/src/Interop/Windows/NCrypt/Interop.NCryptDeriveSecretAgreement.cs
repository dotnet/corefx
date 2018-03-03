// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using Internal.Cryptography;
using Microsoft.Win32.SafeHandles;

internal static partial class Interop
{
    internal static partial class NCrypt
    {
        [Flags]
        internal enum SecretAgreementFlags
        {
            None = 0x00000000,
            UseSecretAsHmacKey = 0x00000001             // KDF_USE_SECRET_AS_HMAC_KEY_FLAG
        }

        /// <summary>
        ///     Generate a secret agreement for generating shared key material
        /// </summary>
        [DllImport(Interop.Libraries.NCrypt)]
        private static extern ErrorCode NCryptSecretAgreement(
            SafeNCryptKeyHandle hPrivKey,
            SafeNCryptKeyHandle hPubKey,
            [Out] out SafeNCryptSecretHandle phSecret,
            int dwFlags);


        /// <summary>
        /// Generate a secret agreement value for between two parties
        /// </summary>
        internal static SafeNCryptSecretHandle DeriveSecretAgreement(
            SafeNCryptKeyHandle privateKey,
            SafeNCryptKeyHandle otherPartyPublicKey)
        {
            ErrorCode error = NCryptSecretAgreement(
                privateKey,
                otherPartyPublicKey,
                out SafeNCryptSecretHandle secretAgreement,
                0);

            if (error != ErrorCode.ERROR_SUCCESS)
            {
                throw error.ToCryptographicException();
            }

            return secretAgreement;
        }
    }
}
