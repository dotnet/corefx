// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Asn1;
using System.Security.Cryptography.X509Certificates.Asn1;
using Internal.Cryptography;

namespace System.Security.Cryptography.X509Certificates
{
    internal sealed class Pkcs9ExtensionRequest : X501Attribute
    {
        internal Pkcs9ExtensionRequest(IEnumerable<X509Extension> extensions)
            : base(Oids.Pkcs9ExtensionRequest, EncodeAttribute(extensions))
        {
        }

        private static byte[] EncodeAttribute(IEnumerable<X509Extension> extensions)
        {
            if (extensions == null)
                throw new ArgumentNullException(nameof(extensions));

            X509ExtensionAsn[] extensionsAsn = extensions.Where(e => e != null).Select(e => new X509ExtensionAsn(e)).ToArray();

            using (AsnWriter writer = AsnSerializer.Serialize(extensionsAsn, AsnEncodingRules.DER))
            {
                return writer.Encode();
            }
        }
    }
}
