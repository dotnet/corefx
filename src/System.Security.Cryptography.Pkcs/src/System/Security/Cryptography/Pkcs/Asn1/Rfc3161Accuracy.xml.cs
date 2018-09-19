﻿using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1
{
    [StructLayout(LayoutKind.Sequential)]
    internal partial struct Rfc3161Accuracy
    {
        internal int? Seconds;
        internal int? Millis;
        internal int? Micros;
      
        internal void Encode(AsnWriter writer)
        {
            Encode(writer, Asn1Tag.Sequence);
        }
    
        internal void Encode(AsnWriter writer, Asn1Tag tag)
        {
            writer.PushSequence(tag);
            

            if (Seconds.HasValue)
            {
                writer.WriteInteger(Seconds.Value);
            }


            if (Millis.HasValue)
            {
                writer.WriteInteger(new Asn1Tag(TagClass.ContextSpecific, 0), Millis.Value);
            }


            if (Micros.HasValue)
            {
                writer.WriteInteger(new Asn1Tag(TagClass.ContextSpecific, 1), Micros.Value);
            }

            writer.PopSequence(tag);
        }

        internal static Rfc3161Accuracy Decode(ReadOnlyMemory<byte> encoded, AsnEncodingRules ruleSet)
        {
            return Decode(Asn1Tag.Sequence, encoded, ruleSet);
        }
        
        internal static Rfc3161Accuracy Decode(Asn1Tag expectedTag, ReadOnlyMemory<byte> encoded, AsnEncodingRules ruleSet)
        {
            AsnReader reader = new AsnReader(encoded, ruleSet);
            
            Decode(reader, expectedTag, out Rfc3161Accuracy decoded);
            reader.ThrowIfNotEmpty();
            return decoded;
        }

        internal static void Decode(AsnReader reader, out Rfc3161Accuracy decoded)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            Decode(reader, Asn1Tag.Sequence, out decoded);
        }

        internal static void Decode(AsnReader reader, Asn1Tag expectedTag, out Rfc3161Accuracy decoded)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            decoded = default;
            AsnReader sequenceReader = reader.ReadSequence(expectedTag);
            

            if (sequenceReader.HasData && sequenceReader.PeekTag().HasSameClassAndValue(Asn1Tag.Integer))
            {

                if (sequenceReader.TryReadInt32(out int tmpSeconds))
                {
                    decoded.Seconds = tmpSeconds;
                }
                else
                {
                    sequenceReader.ThrowIfNotEmpty();
                }

            }


            if (sequenceReader.HasData && sequenceReader.PeekTag().HasSameClassAndValue(new Asn1Tag(TagClass.ContextSpecific, 0)))
            {

                if (sequenceReader.TryReadInt32(new Asn1Tag(TagClass.ContextSpecific, 0), out int tmpMillis))
                {
                    decoded.Millis = tmpMillis;
                }
                else
                {
                    sequenceReader.ThrowIfNotEmpty();
                }

            }


            if (sequenceReader.HasData && sequenceReader.PeekTag().HasSameClassAndValue(new Asn1Tag(TagClass.ContextSpecific, 1)))
            {

                if (sequenceReader.TryReadInt32(new Asn1Tag(TagClass.ContextSpecific, 1), out int tmpMicros))
                {
                    decoded.Micros = tmpMicros;
                }
                else
                {
                    sequenceReader.ThrowIfNotEmpty();
                }

            }


            sequenceReader.ThrowIfNotEmpty();
        }
    }
}
