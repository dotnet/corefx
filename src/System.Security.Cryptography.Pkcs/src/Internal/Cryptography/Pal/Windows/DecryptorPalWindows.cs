// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Cryptography.Asn1;
using System.Security.Cryptography.Pkcs;
using Microsoft.Win32.SafeHandles;

namespace Internal.Cryptography.Pal.Windows
{
    internal sealed partial class DecryptorPalWindows : DecryptorPal
    {
        private DecryptorPalWindows(
            SafeCryptMsgHandle hCryptMsg,
            RecipientInfoCollection recipientInfos,
            AlgorithmIdentifierAsn contentEncryptionAlgorithm)
            : base(recipientInfos)
        {
            _hCryptMsg = hCryptMsg;
            _contentEncryptionAlgorithm = contentEncryptionAlgorithm;
        }

        public sealed override void Dispose()
        {
            if (_hCryptMsg != null && !_hCryptMsg.IsInvalid)
            {
                _hCryptMsg.Dispose();
                _hCryptMsg = null;
            }
        }

        private SafeCryptMsgHandle _hCryptMsg;
        private AlgorithmIdentifierAsn _contentEncryptionAlgorithm;
    }
}
