// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;

using Microsoft.Win32.SafeHandles;

namespace Internal.Cryptography.Pal.OpenSsl
{
    internal sealed partial class DecryptorPalOpenSsl : DecryptorPal
    {
        private SafeCmsHandle _decodedMessage;

        internal DecryptorPalOpenSsl(SafeCmsHandle decodedMessage, RecipientInfoCollection recipientInfos)
            : base(recipientInfos)
        {
            _decodedMessage = decodedMessage;
        }

        public sealed override void Dispose()
        {
            if (_decodedMessage != null && !_decodedMessage.IsInvalid)
            {
                _decodedMessage.Dispose();
                _decodedMessage = null;
            }
        }

        public sealed override ContentInfo TryDecrypt(
            RecipientInfo recipientInfo,
            X509Certificate2 cert,
            X509Certificate2Collection originatorCerts,
            X509Certificate2Collection extraStore,
            out Exception exception)
        {
            // TODO(3334): Decrypt look at CMS_decrypt & functions in OpenSsl. 
            throw new NotImplementedException();
        }
    }
}
