// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Security.Cryptography.Asn1
{
    // https://tools.ietf.org/html/rfc5280#section-4.2.1.6
    //
    // GeneralName ::= CHOICE {
    //     otherName                 [0]  OtherName,
    //     rfc822Name                [1]  IA5String,
    //     dNSName                   [2]  IA5String,
    //     x400Address               [3]  ORAddress,
    //     directoryName             [4]  Name,
    //     ediPartyName              [5]  EDIPartyName,
    //     uniformResourceIdentifier [6]  IA5String,
    //     iPAddress                 [7]  OCTET STRING,
    //     registeredID              [8]  OBJECT IDENTIFIER
    // }
    [Choice]
    [StructLayout(LayoutKind.Sequential)]
    internal struct GeneralNameAsn
    {
        [ExpectedTag(0)]
        internal OtherNameAsn? OtherName;

        [ExpectedTag(1)]
        [IA5String]
        internal string Rfc822Name;

        [ExpectedTag(2)]
        [IA5String]
        internal string DnsName;

        [ExpectedTag(3)]
        [AnyValue]
        internal ReadOnlyMemory<byte>? X400Address;

        [ExpectedTag(4, ExplicitTag = true)]
        [AnyValue]
        internal ReadOnlyMemory<byte>? DirectoryName;

        [ExpectedTag(5)]
        internal EdiPartyNameAsn? EdiPartyName;

        [ExpectedTag(6)]
        [IA5String]
        internal string Uri;

        [ExpectedTag(7)]
        [OctetString]
        internal ReadOnlyMemory<byte>? IPAddress;

        [ExpectedTag(8)]
        [ObjectIdentifier]
        internal string RegisteredId;

        internal void Encode(AsnWriter writer)
        {
            AsnSerializer.Serialize(this, writer);
        }

        internal static void Decode(AsnReader reader, out GeneralNameAsn decoded)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            ReadOnlyMemory<byte> value = reader.GetEncodedValue();
            decoded = AsnSerializer.Deserialize<GeneralNameAsn>(value, reader.RuleSet);
        }

        internal static GeneralNameAsn Decode(ReadOnlyMemory<byte> encoded, AsnEncodingRules ruleSet)
        {
            AsnReader reader = new AsnReader(encoded, ruleSet);

            Decode(reader, out GeneralNameAsn decoded);
            reader.ThrowIfNotEmpty();
            return decoded;
        }
    }

    // https://tools.ietf.org/html/rfc5280#section-4.2.1.6
    //
    // OtherName ::= SEQUENCE {
    //     type-id    OBJECT IDENTIFIER,
    //     value      [0] EXPLICIT ANY DEFINED BY type-id }
    // }
    [StructLayout(LayoutKind.Sequential)]
    internal struct OtherNameAsn
    {
        [ObjectIdentifier]
        internal string TypeId;

        [ExpectedTag(0, ExplicitTag = true)]
        [AnyValue]
        internal ReadOnlyMemory<byte> Value;
    }

    // https://tools.ietf.org/html/rfc5280#section-4.2.1.6
    //
    // EDIPartyName ::= SEQUENCE {
    //     nameAssigner            [0]     DirectoryString OPTIONAL,
    //     partyName               [1]     DirectoryString
    // }
    [StructLayout(LayoutKind.Sequential)]
    internal struct EdiPartyNameAsn
    {
        [OptionalValue]
        [ExpectedTag(0, ExplicitTag = true)]
        internal DirectoryStringAsn? NameAssigner;

        [ExpectedTag(1, ExplicitTag = true)]
        internal DirectoryStringAsn PartyName;
    }

    // https://tools.ietf.org/html/rfc5280#section-4.1.2.4
    //
    // DirectoryString ::= CHOICE {
    //     teletexString           TeletexString (SIZE (1..MAX)),
    //     printableString         PrintableString (SIZE (1..MAX)),
    //     universalString         UniversalString (SIZE (1..MAX)),
    //     utf8String              UTF8String (SIZE (1..MAX)),
    //     bmpString               BMPString (SIZE (1..MAX))
    // }
    [Choice]
    [StructLayout(LayoutKind.Sequential)]
    internal struct DirectoryStringAsn
    {
        [ExpectedTag(TagClass.Universal, (int)UniversalTagNumber.TeletexString)]
        internal ReadOnlyMemory<byte>? TeletexString;

        [PrintableString]
        internal string PrintableString;

        [ExpectedTag(TagClass.Universal, (int)UniversalTagNumber.UniversalString)]
        internal ReadOnlyMemory<byte>? UniversalString;

        [UTF8String]
        internal string Utf8String;

        [BMPString]
        internal string BmpString;
    }
}
