﻿using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.X509Certificates.Asn1
{
    [StructLayout(LayoutKind.Sequential)]
    internal partial struct CertificationRequestAsn
    {
        internal System.Security.Cryptography.X509Certificates.Asn1.CertificationRequestInfoAsn CertificationRequestInfo;
        internal System.Security.Cryptography.Asn1.AlgorithmIdentifierAsn SignatureAlgorithm;
        internal ReadOnlyMemory<byte> SignatureValue;
      
        internal void Encode(AsnWriter writer)
        {
            Encode(writer, Asn1Tag.Sequence);
        }
    
        internal void Encode(AsnWriter writer, Asn1Tag tag)
        {
            writer.PushSequence(tag);
            
            CertificationRequestInfo.Encode(writer);
            SignatureAlgorithm.Encode(writer);
            writer.WriteBitString(SignatureValue.Span);
            writer.PopSequence(tag);
        }

        internal static CertificationRequestAsn Decode(ReadOnlyMemory<byte> encoded, AsnEncodingRules ruleSet)
        {
            return Decode(Asn1Tag.Sequence, encoded, ruleSet);
        }
        
        internal static CertificationRequestAsn Decode(Asn1Tag expectedTag, ReadOnlyMemory<byte> encoded, AsnEncodingRules ruleSet)
        {
            AsnReader reader = new AsnReader(encoded, ruleSet);
            
            Decode(reader, expectedTag, out CertificationRequestAsn decoded);
            reader.ThrowIfNotEmpty();
            return decoded;
        }

        internal static void Decode(AsnReader reader, out CertificationRequestAsn decoded)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            Decode(reader, Asn1Tag.Sequence, out decoded);
        }

        internal static void Decode(AsnReader reader, Asn1Tag expectedTag, out CertificationRequestAsn decoded)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            decoded = default;
            AsnReader sequenceReader = reader.ReadSequence(expectedTag);
            
            System.Security.Cryptography.X509Certificates.Asn1.CertificationRequestInfoAsn.Decode(sequenceReader, out decoded.CertificationRequestInfo);
            System.Security.Cryptography.Asn1.AlgorithmIdentifierAsn.Decode(sequenceReader, out decoded.SignatureAlgorithm);

            if (sequenceReader.TryGetPrimitiveBitStringValue(out _, out ReadOnlyMemory<byte> tmpSignatureValue))
            {
                decoded.SignatureValue = tmpSignatureValue;
            }
            else
            {
                decoded.SignatureValue = sequenceReader.ReadBitString(out _);
            }


            sequenceReader.ThrowIfNotEmpty();
        }
    }
}
