// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Tests.Asn1
{
    public abstract partial class Asn1ReaderTests
    {
        public enum PublicTagClass : byte
        {
            Universal = TagClass.Universal,
            Application = TagClass.Application,
            ContextSpecific = TagClass.ContextSpecific,
            Private = TagClass.Private,
        }

        public enum PublicEncodingRules
        {
            BER = AsnEncodingRules.BER,
            CER = AsnEncodingRules.CER,
            DER = AsnEncodingRules.DER,
        }

        public enum PublicNamedBitListMode
        {
            NamedZeroIsOne = NamedBitListMode.NamedZeroIsOne,
            NamedZeroIs128LittleEndian = NamedBitListMode.NamedZeroIs128LittleEndian,
            NamedZeroIs128BigEndian = NamedBitListMode.NamedZeroIs128BigEndian,
        }
    }
}
