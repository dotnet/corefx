// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Enable CHECK_ACCURATE_ENSURE to ensure that the AsnWriter is not ever
// abusing the normal EnsureWriteCapacity + ArrayPool behaviors of rounding up.
//#define CHECK_ACCURATE_ENSURE

using System.Buffers;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Security.Cryptography.Asn1
{
    // Uses a masked overlay of the tag class encoding.
    // T-REC-X.690-201508 sec 8.1.2.2
    internal enum TagClass : byte
    {
        Universal = 0,
        Application = 0b0100_0000,
        ContextSpecific = 0b1000_0000,
        Private = 0b1100_0000,
    }

    // ITU-T-REC.X.680-201508 sec 8.6
    internal enum UniversalTagNumber
    {
        EndOfContents = 0,
        Boolean = 1,
        Integer = 2,
        BitString = 3,
        OctetString = 4,
        Null = 5,
        ObjectIdentifier = 6,
        ObjectDescriptor = 7,
        External = 8,
        InstanceOf = External,
        Real = 9,
        Enumerated = 10,
        Embedded = 11,
        UTF8String = 12,
        RelativeObjectIdentifier = 13,
        Time = 14,
        // 15 is reserved
        Sequence = 16,
        SequenceOf = Sequence,
        Set = 17,
        SetOf = Set,
        NumericString = 18,
        PrintableString = 19,
        TeletexString = 20,
        T61String = TeletexString,
        VideotexString = 21,
        IA5String = 22,
        UtcTime = 23,
        GeneralizedTime = 24,
        GraphicString = 25,
        VisibleString = 26,
        ISO646String = VisibleString,
        GeneralString = 27,
        UniversalString = 28,
        UnrestrictedCharacterString = 29,
        BMPString = 30,
        Date = 31,
        TimeOfDay = 32,
        DateTime = 33,
        Duration = 34,
        ObjectIdentifierIRI = 35,
        RelativeObjectIdentifierIRI = 36,
    }

    // Represents a BER-family encoded tag.
    // T-REC-X.690-201508 sec 8.1.2
    internal struct Asn1Tag : IEquatable<Asn1Tag>
    {
        private const byte ClassMask = 0b1100_0000;
        private const byte ConstructedMask = 0b0010_0000;
        private const byte ControlMask = ClassMask | ConstructedMask;
        private const byte TagNumberMask = 0b0001_1111;

        internal static readonly Asn1Tag EndOfContents = new Asn1Tag(0, (int)UniversalTagNumber.EndOfContents);
        internal static readonly Asn1Tag Boolean = new Asn1Tag(0, (int)UniversalTagNumber.Boolean);
        internal static readonly Asn1Tag Integer = new Asn1Tag(0, (int)UniversalTagNumber.Integer);
        internal static readonly Asn1Tag PrimitiveBitString = new Asn1Tag(0, (int)UniversalTagNumber.BitString);
        internal static readonly Asn1Tag PrimitiveOctetString = new Asn1Tag(0, (int)UniversalTagNumber.OctetString);
        internal static readonly Asn1Tag Null = new Asn1Tag(0, (int)UniversalTagNumber.Null);
        internal static readonly Asn1Tag ObjectIdentifier = new Asn1Tag(0, (int)UniversalTagNumber.ObjectIdentifier);
        internal static readonly Asn1Tag Enumerated = new Asn1Tag(0, (int)UniversalTagNumber.Enumerated);
        internal static readonly Asn1Tag Sequence = new Asn1Tag(ConstructedMask, (int)UniversalTagNumber.Sequence);
        internal static readonly Asn1Tag SetOf = new Asn1Tag(ConstructedMask, (int)UniversalTagNumber.SetOf);
        internal static readonly Asn1Tag UtcTime = new Asn1Tag(0, (int)UniversalTagNumber.UtcTime);
        internal static readonly Asn1Tag GeneralizedTime = new Asn1Tag(0, (int)UniversalTagNumber.GeneralizedTime);

        private readonly byte _controlFlags;
        private readonly int _tagValue;

        public TagClass TagClass => (TagClass)(_controlFlags & ClassMask);
        public bool IsConstructed => (_controlFlags & ConstructedMask) != 0;
        public int TagValue => _tagValue;

        private Asn1Tag(byte controlFlags, int tagValue)
        {
            _controlFlags = (byte)(controlFlags & ControlMask);
            _tagValue = tagValue;
        }

        public Asn1Tag(UniversalTagNumber universalTagNumber, bool isConstructed = false)
            : this(isConstructed ? ConstructedMask : (byte)0, (int)universalTagNumber)
        {
            // T-REC-X.680-201508 sec 8.6 (Table 1)
            const UniversalTagNumber ReservedIndex = (UniversalTagNumber)15;

            if (universalTagNumber < UniversalTagNumber.EndOfContents ||
                universalTagNumber > UniversalTagNumber.RelativeObjectIdentifierIRI ||
                universalTagNumber == ReservedIndex)
            {
                throw new ArgumentOutOfRangeException(nameof(universalTagNumber), universalTagNumber, null);
            }
        }

        public Asn1Tag(TagClass tagClass, int tagValue, bool isConstructed = false)
            : this((byte)((byte)tagClass | (isConstructed ? ConstructedMask : 0)), tagValue)
        {
            if (tagClass < TagClass.Universal || tagClass > TagClass.Private)
            {
                throw new ArgumentOutOfRangeException(nameof(tagClass), tagClass, null);
            }
        }

        public Asn1Tag AsConstructed()
        {
            return new Asn1Tag((byte)(_controlFlags | ConstructedMask), _tagValue);
        }

        public Asn1Tag AsPrimitive()
        {
            return new Asn1Tag((byte)(_controlFlags & ~ConstructedMask), _tagValue);
        }

        public static bool TryParse(ReadOnlySpan<byte> source, out Asn1Tag tag, out int bytesRead)
        {
            tag = default(Asn1Tag);
            bytesRead = 0;

            if (source.IsEmpty)
            {
                return false;
            }

            byte first = source[bytesRead];
            bytesRead++;
            uint tagValue = (uint)(first & TagNumberMask);

            if (tagValue == TagNumberMask)
            {
                // Multi-byte encoding
                // T-REC-X.690-201508 sec 8.1.2.4
                const byte ContinuationFlag = 0x80;
                const byte ValueMask = ContinuationFlag - 1;

                tagValue = 0;
                byte current;

                do
                {
                    if (source.Length <= bytesRead)
                    {
                        bytesRead = 0;
                        return false;
                    }

                    current = source[bytesRead];
                    byte currentValue = (byte)(current & ValueMask);
                    bytesRead++;

                    // If TooBigToShift is shifted left 7, the content bit shifts out.
                    // So any value greater than or equal to this cannot be shifted without loss.
                    const int TooBigToShift = 0b00000010_00000000_00000000_00000000;

                    if (tagValue >= TooBigToShift)
                    {
                        bytesRead = 0;
                        return false;
                    }

                    tagValue <<= 7;
                    tagValue |= currentValue;

                    // The first byte cannot have the value 0 (T-REC-X.690-201508 sec 8.1.2.4.2.c)
                    if (tagValue == 0)
                    {
                        bytesRead = 0;
                        return false;
                    }
                }
                while ((current & ContinuationFlag) == ContinuationFlag);

                // This encoding is only valid for tag values greater than 30.
                // (T-REC-X.690-201508 sec 8.1.2.3, 8.1.2.4)
                if (tagValue <= 30)
                {
                    bytesRead = 0;
                    return false;
                }

                // There's not really any ambiguity, but prevent negative numbers from showing up.
                if (tagValue > int.MaxValue)
                {
                    bytesRead = 0;
                    return false;
                }
            }

            Debug.Assert(bytesRead > 0);
            tag = new Asn1Tag(first, (int)tagValue);
            return true;
        }

        public int CalculateEncodedSize()
        {
            const int SevenBits = 0b0111_1111;
            const int FourteenBits = 0b0011_1111_1111_1111;
            const int TwentyOneBits = 0b0001_1111_1111_1111_1111_1111;
            const int TwentyEightBits = 0b0000_1111_1111_1111_1111_1111_1111_1111;

            if (TagValue < TagNumberMask)
                return 1;
            if (TagValue <= SevenBits)
                return 2;
            if (TagValue <= FourteenBits)
                return 3;
            if (TagValue <= TwentyOneBits)
                return 4;
            if (TagValue <= TwentyEightBits)
                return 5;

            return 6;
        }

        public bool TryWrite(Span<byte> destination, out int bytesWritten)
        {
            int spaceRequired = CalculateEncodedSize();

            if (destination.Length < spaceRequired)
            {
                bytesWritten = 0;
                return false;
            }

            if (spaceRequired == 1)
            {
                byte value = (byte)(_controlFlags | TagValue);
                destination[0] = value;
                bytesWritten = 1;
                return true;
            }

            byte firstByte = (byte)(_controlFlags | TagNumberMask);
            destination[0] = firstByte;

            int remaining = TagValue;
            int idx = spaceRequired - 1;

            while (remaining > 0)
            {
                int segment = remaining & 0x7F;

                // The last byte doesn't get the marker, which we write first.
                if (remaining != TagValue)
                {
                    segment |= 0x80;
                }

                Debug.Assert(segment <= byte.MaxValue);
                destination[idx] = (byte)segment;
                remaining >>= 7;
                idx--;
            }

            Debug.Assert(idx == 0);
            bytesWritten = spaceRequired;
            return true;
        }

        public bool Equals(Asn1Tag other)
        {
            return _controlFlags == other._controlFlags && _tagValue == other._tagValue;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Asn1Tag && Equals((Asn1Tag)obj);
        }

        public override int GetHashCode()
        {
            // Most TagValue values will be in the 0-30 range,
            // the GetHashCode value only has collisions when TagValue is
            // between 2^29 and uint.MaxValue
            return (_controlFlags << 24) ^ _tagValue;
        }

        public static bool operator ==(Asn1Tag left, Asn1Tag right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Asn1Tag left, Asn1Tag right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            const string ConstructedPrefix = "Constructed ";
            string classAndValue;

            if (TagClass == TagClass.Universal)
            {
                classAndValue = ((UniversalTagNumber)TagValue).ToString();
            }
            else
            {
                classAndValue = TagClass + "-" + TagValue;
            }

            if (IsConstructed)
            {
                return ConstructedPrefix + classAndValue;
            }

            return classAndValue;
        }
    }

    internal enum AsnEncodingRules
    {
        BER,
        CER,
        DER,
    }

    internal class AsnReader
    {
        // T-REC-X.690-201508 sec 9.2
        internal const int MaxCERSegmentSize = 1000;

        // T-REC-X.690-201508 sec 8.1.5 says only 0000 is legal.
        private const int EndOfContentsEncodedLength = 2;

        private ReadOnlyMemory<byte> _data;
        private readonly AsnEncodingRules _ruleSet;

        public bool HasData => !_data.IsEmpty;

        public AsnReader(ReadOnlyMemory<byte> data, AsnEncodingRules ruleSet)
        {
            CheckEncodingRules(ruleSet);

            _data = data;
            _ruleSet = ruleSet;
        }

        public static bool TryPeekTag(ReadOnlySpan<byte> source, out Asn1Tag tag, out int bytesRead)
        {
            return Asn1Tag.TryParse(source, out tag, out bytesRead);
        }

        public Asn1Tag PeekTag()
        {
            if (TryPeekTag(_data.Span, out Asn1Tag tag, out int bytesRead))
            {
                return tag;
            }

            throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
        }

        private static bool TryReadLength(
            ReadOnlySpan<byte> source,
            AsnEncodingRules ruleSet,
            out int? length,
            out int bytesRead)
        {
            length = null;
            bytesRead = 0;

            CheckEncodingRules(ruleSet);

            if (source.IsEmpty)
            {
                return false;
            }

            // T-REC-X.690-201508 sec 8.1.3

            byte lengthOrLengthLength = source[bytesRead];
            bytesRead++;
            const byte MultiByteMarker = 0x80;

            // 0x00-0x7F are direct length values.
            // 0x80 is BER/CER indefinite length.
            // 0x81-0xFE says that the length takes the next 1-126 bytes.
            // 0xFF is forbidden.
            if (lengthOrLengthLength == MultiByteMarker)
            {
                // T-REC-X.690-201508 sec 10.1 (DER: Length forms)
                if (ruleSet == AsnEncodingRules.DER)
                {
                    bytesRead = 0;
                    return false;
                }

                // Null length == indefinite.
                return true;
            }

            if (lengthOrLengthLength < MultiByteMarker)
            {
                length = lengthOrLengthLength;
                return true;
            }

            if (lengthOrLengthLength == 0xFF)
            {
                bytesRead = 0;
                return false;
            }

            byte lengthLength = (byte)(lengthOrLengthLength & ~MultiByteMarker);

            // +1 for lengthOrLengthLength
            if (lengthLength + 1 > source.Length)
            {
                bytesRead = 0;
                return false;
            }

            // T-REC-X.690-201508 sec 9.1 (CER: Length forms)
            // T-REC-X.690-201508 sec 10.1 (DER: Length forms)
            bool minimalRepresentation =
                ruleSet == AsnEncodingRules.DER || ruleSet == AsnEncodingRules.CER;

            // The ITU-T specifications tecnically allow lengths up to ((2^128) - 1), but
            // since Span's length is a signed Int32 we're limited to identifying memory
            // that is within ((2^31) - 1) bytes of the tag start.
            if (minimalRepresentation && lengthLength > sizeof(int))
            {
                bytesRead = 0;
                return false;
            }

            uint parsedLength = 0;

            for (int i = 0; i < lengthLength; i++)
            {
                byte current = source[bytesRead];
                bytesRead++;

                if (parsedLength == 0)
                {
                    if (minimalRepresentation && current == 0)
                    {
                        bytesRead = 0;
                        return false;
                    }

                    if (!minimalRepresentation && current != 0)
                    {
                        // Under BER rules we could have had padding zeros, so
                        // once the first data bits come in check that we fit within
                        // sizeof(int) due to Span bounds.

                        if (lengthLength - i > sizeof(int))
                        {
                            bytesRead = 0;
                            return false;
                        }
                    }
                }

                parsedLength <<= 8;
                parsedLength |= current;
            }

            // This value cannot be represented as a Span length.
            if (parsedLength > int.MaxValue)
            {
                bytesRead = 0;
                return false;
            }

            if (minimalRepresentation && parsedLength < MultiByteMarker)
            {
                bytesRead = 0;
                return false;
            }

            Debug.Assert(bytesRead > 0);
            length = (int)parsedLength;
            return true;
        }

        internal Asn1Tag ReadTagAndLength(out int? contentsLength, out int bytesRead)
        {
            if (TryPeekTag(_data.Span, out Asn1Tag tag, out int tagBytesRead) &&
                TryReadLength(_data.Slice(tagBytesRead).Span, _ruleSet, out int? length, out int lengthBytesRead))
            {
                int allBytesRead = tagBytesRead + lengthBytesRead;

                if (tag.IsConstructed)
                {
                    // T-REC-X.690-201508 sec 9.1 (CER: Length forms) says constructed is always indefinite.
                    if (_ruleSet == AsnEncodingRules.CER && length != null)
                    {
                        throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
                    }
                }
                else if (length == null)
                {
                    // T-REC-X.690-201508 sec 8.1.3.2 says primitive encodings must use a definite form.
                    throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
                }

                bytesRead = allBytesRead;
                contentsLength = length;
                return tag;
            }

            throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
        }

        private static void ValidateEndOfContents(Asn1Tag tag, int? length, int headerLength)
        {
            // T-REC-X.690-201508 sec 8.1.5 excludes the BER 8100 length form for 0.
            if (tag.IsConstructed || length != 0 || headerLength != EndOfContentsEncodedLength)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
            }
        }

        private static ReadOnlyMemory<byte> SeekEndOfContents(
            ReadOnlyMemory<byte> source,
            AsnEncodingRules ruleSet,
            int initialSliceOffset = 0)
        {
            ReadOnlyMemory<byte> cur = source.Slice(initialSliceOffset);
            int totalLen = 0;

            while (!cur.IsEmpty)
            {
                AsnReader reader = new AsnReader(cur, ruleSet);
                Asn1Tag tag = reader.ReadTagAndLength(out int ? length, out int bytesRead);
                ReadOnlyMemory<byte> nestedContents = reader.PeekContentBytes();

                int localLen = bytesRead + nestedContents.Length;

                if (tag == Asn1Tag.EndOfContents)
                {
                    ValidateEndOfContents(tag, length, bytesRead);

                    return source.Slice(0, totalLen + initialSliceOffset);
                }

                // If the current value was an indefinite-length-encoded value
                // then we need to skip over the EOC as well.  But we didn't want to
                // include it as part of the content span.
                //
                // T-REC-X.690-201508 sec 8.1.1.1 / 8.1.1.3 indicate that the
                // End -of-Contents octets are "after" the contents octets, not
                // "at the end" of them.
                if (length == null)
                {
                    localLen += EndOfContentsEncodedLength;
                }

                totalLen += localLen;
                cur = cur.Slice(localLen);
            }

            throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
        }

        /// <summary>
        /// Get a ReadOnlyMemory view of the next encoded value without consuming it.
        /// For indefinite length encodings this includes the End of Contents marker.
        /// </summary>
        /// <returns>A ReadOnlyMemory view of the next encoded value.</returns>
        /// <exception cref="CryptographicException">
        /// The reader is positioned at a point where the tag or length is invalid
        /// under the current encoding rules.
        /// </exception>
        /// <seealso cref="PeekContentBytes"/>
        /// <seealso cref="GetEncodedValue"/>
        public ReadOnlyMemory<byte> PeekEncodedValue()
        {
            Asn1Tag tag = ReadTagAndLength(out int ? length, out int bytesRead);

            if (length == null)
            {
                ReadOnlyMemory<byte> tagLengthAndContents = SeekEndOfContents(_data, _ruleSet, bytesRead);
                return Slice(_data, 0, tagLengthAndContents.Length + EndOfContentsEncodedLength);
            }

            return Slice(_data, 0, bytesRead + length.Value);
        }

        /// <summary>
        /// Get a ReadOnlyMemory view of the content octets (bytes) of the next encoded
        /// value without consuming it.
        /// </summary>
        /// <returns>A ReadOnlyMemory view of the contents octets of the next encoded value.</returns>
        /// <exception cref="CryptographicException">
        /// The reader is positioned at a point where the tag or length is invalid
        /// under the current encoding rules.
        /// </exception>
        /// <seealso cref="PeekEncodedValue"/>
        public ReadOnlyMemory<byte> PeekContentBytes()
        {
            Asn1Tag tag = ReadTagAndLength(out int ? length, out int bytesRead);

            if (length == null)
            {
                return SeekEndOfContents(_data.Slice(bytesRead), _ruleSet);
            }

            return Slice(_data, bytesRead, length.Value);
        }

        public void SkipValue()
        {
            GetEncodedValue();
        }

        /// <summary>
        /// Get a ReadOnlyMemory view of the next encoded value, and move the reader past it.
        /// For an indefinite length encoding this includes the End of Contents marker.
        /// </summary>
        /// <returns>A ReadOnlyMemory view of the next encoded value.</returns>
        /// <seealso cref="PeekEncodedValue"/>
        public ReadOnlyMemory<byte> GetEncodedValue()
        {
            ReadOnlyMemory<byte> encodedValue = PeekEncodedValue();
            _data = _data.Slice(encodedValue.Length);
            return encodedValue;
        }

        private static bool ReadBooleanValue(
            ReadOnlySpan<byte> source,
            AsnEncodingRules ruleSet)
        {
            // T-REC-X.690-201508 sec 8.2.1
            if (source.Length != 1)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
            }

            byte val = source[0];

            // T-REC-X.690-201508 sec 8.2.2
            if (val == 0)
            {
                return false;
            }

            // T-REC-X.690-201508 sec 11.1
            if (val != 0xFF && (ruleSet == AsnEncodingRules.DER || ruleSet == AsnEncodingRules.CER))
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
            }

            return true;
        }

        public bool ReadBoolean() => ReadBoolean(Asn1Tag.Boolean);

        public bool ReadBoolean(Asn1Tag expectedTag)
        {
            Asn1Tag tag = ReadTagAndLength(out int ? length, out int headerLength);
            CheckExpectedTag(tag, expectedTag, UniversalTagNumber.Boolean);

            // T-REC-X.690-201508 sec 8.2.1
            if (tag.IsConstructed)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
            }

            bool value = ReadBooleanValue(
                Slice(_data, headerLength, length.Value).Span,
                _ruleSet);

            _data = _data.Slice(headerLength + length.Value);
            return value;
        }

        private ReadOnlyMemory<byte> GetIntegerContents(
            Asn1Tag expectedTag,
            UniversalTagNumber tagNumber,
            out int headerLength)
        {
            Asn1Tag tag = ReadTagAndLength(out int ? length, out headerLength);
            CheckExpectedTag(tag, expectedTag, tagNumber);

            // T-REC-X.690-201508 sec 8.3.1
            if (tag.IsConstructed || length < 1)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
            }

            // Slice first so that an out of bounds value triggers a CryptographicException.
            ReadOnlyMemory<byte> contents = Slice(_data, headerLength, length.Value);
            ReadOnlySpan<byte> contentSpan = contents.Span;

            // T-REC-X.690-201508 sec 8.3.2
            if (contents.Length > 1)
            {
                ushort bigEndianValue = (ushort)(contentSpan[0] << 8 | contentSpan[1]);
                const ushort RedundancyMask = 0b1111_1111_1000_0000;
                ushort masked = (ushort)(bigEndianValue & RedundancyMask);

                // If the first 9 bits are all 0 or are all 1, the value is invalid.
                if (masked == 0 || masked == RedundancyMask)
                {
                    throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
                }
            }

            return contents;
        }

        public ReadOnlyMemory<byte> GetIntegerBytes() =>
            GetIntegerBytes(Asn1Tag.Integer);

        public ReadOnlyMemory<byte> GetIntegerBytes(Asn1Tag expectedTag)
        {
            ReadOnlyMemory<byte> contents =
                GetIntegerContents(expectedTag, UniversalTagNumber.Integer, out int headerLength);

            _data = _data.Slice(headerLength + contents.Length);
            return contents;
        }

        private bool TryReadSignedInteger(
            int sizeLimit,
            Asn1Tag expectedTag,
            UniversalTagNumber tagNumber,
            out long value)
        {
            Debug.Assert(sizeLimit <= sizeof(long));

            ReadOnlyMemory<byte> contents = GetIntegerContents(expectedTag, tagNumber, out int headerLength);

            if (contents.Length > sizeLimit)
            {
                value = 0;
                return false;
            }

            ReadOnlySpan<byte> contentSpan = contents.Span;

            bool isNegative = contentSpan[0] >= 0x80;
            long accum = isNegative ? -1 : 0;

            for (int i = 0; i < contents.Length; i++)
            {
                accum <<= 8;
                accum |= contentSpan[i];
            }

            _data = _data.Slice(headerLength + contents.Length);
            value = accum;
            return true;
        }

        private bool TryReadUnsignedInteger(
            int sizeLimit,
            Asn1Tag expectedTag,
            UniversalTagNumber tagNumber,
            out ulong value)
        {
            Debug.Assert(sizeLimit <= sizeof(ulong));

            ReadOnlyMemory<byte> contents = GetIntegerContents(expectedTag, tagNumber, out int headerLength);
            ReadOnlySpan<byte> contentSpan = contents.Span;
            int contentLength = contents.Length;

            bool isNegative = contentSpan[0] >= 0x80;

            if (isNegative)
            {
                // TODO/Review: Should this be "false", an Exception, or not a scenario?
                value = 0;
                return false;
            }

            // Ignore any padding zeros.
            if (contentSpan.Length > 1 && contentSpan[0] == 0)
            {
                contentSpan = contentSpan.Slice(1);
            }

            if (contentSpan.Length > sizeLimit)
            {
                value = 0;
                return false;
            }

            ulong accum = 0;

            for (int i = 0; i < contentSpan.Length; i++)
            {
                accum <<= 8;
                accum |= contentSpan[i];
            }

            _data = _data.Slice(headerLength + contentLength);
            value = accum;
            return true;
        }

        public bool TryReadInt32(out int value) =>
            TryReadInt32(Asn1Tag.Integer, out value);

        public bool TryReadInt32(Asn1Tag expectedTag, out int value)
        {
            if (TryReadSignedInteger(sizeof(int), expectedTag, UniversalTagNumber.Integer, out long longValue))
            {
                value = (int)longValue;
                return true;
            }

            value = 0;
            return false;
        }

        public bool TryReadUInt32(out uint value) =>
            TryReadUInt32(Asn1Tag.Integer, out value);

        public bool TryReadUInt32(Asn1Tag expectedTag, out uint value)
        {
            if (TryReadUnsignedInteger(sizeof(uint), expectedTag, UniversalTagNumber.Integer, out ulong ulongValue))
            {
                value = (uint)ulongValue;
                return true;
            }

            value = 0;
            return false;
        }

        public bool TryReadInt64(out long value) =>
            TryReadInt64(Asn1Tag.Integer, out value);
        
        public bool TryReadInt64(Asn1Tag expectedTag, out long value)
        {
            return TryReadSignedInteger(sizeof(long), expectedTag, UniversalTagNumber.Integer, out value);
        }

        public bool TryReadUInt64(out ulong value) =>
            TryReadUInt64(Asn1Tag.Integer, out value);

        public bool TryReadUInt64(Asn1Tag expectedTag, out ulong value)
        {
            return TryReadUnsignedInteger(sizeof(ulong), expectedTag, UniversalTagNumber.Integer, out value);
        }

        public bool TryReadInt16(out short value) =>
            TryReadInt16(Asn1Tag.Integer, out value);

        public bool TryReadInt16(Asn1Tag expectedTag, out short value)
        {
            if (TryReadSignedInteger(sizeof(short), expectedTag, UniversalTagNumber.Integer, out long longValue))
            {
                value = (short)longValue;
                return true;
            }

            value = 0;
            return false;
        }

        public bool TryReadUInt16(out ushort value) =>
            TryReadUInt16(Asn1Tag.Integer, out value);

        public bool TryReadUInt16(Asn1Tag expectedTag, out ushort value)
        {
            if (TryReadUnsignedInteger(sizeof(ushort), expectedTag, UniversalTagNumber.Integer, out ulong ulongValue))
            {
                value = (ushort)ulongValue;
                return true;
            }

            value = 0;
            return false;
        }

        public bool TryReadInt8(out sbyte value) =>
            TryReadInt8(Asn1Tag.Integer, out value);

        public bool TryReadInt8(Asn1Tag expectedTag, out sbyte value)
        {
            if (TryReadSignedInteger(sizeof(sbyte), expectedTag, UniversalTagNumber.Integer, out long longValue))
            {
                value = (sbyte)longValue;
                return true;
            }

            value = 0;
            return false;
        }

        public bool TryReadUInt8(out byte value) =>
            TryReadUInt8(Asn1Tag.Integer, out value);

        public bool TryReadUInt8(Asn1Tag expectedTag, out byte value)
        {
            if (TryReadUnsignedInteger(sizeof(byte), expectedTag, UniversalTagNumber.Integer, out ulong ulongValue))
            {
                value = (byte)ulongValue;
                return true;
            }

            value = 0;
            return false;
        }

        private void ParsePrimitiveBitStringContents(
            ReadOnlyMemory<byte> source,
            out int unusedBitCount,
            out ReadOnlyMemory<byte> value,
            out byte? normalizedLastByte)
        {
            // T-REC-X.690-201508 sec 9.2
            if (_ruleSet == AsnEncodingRules.CER && source.Length > MaxCERSegmentSize)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
            }

            // T-REC-X.690-201508 sec 8.6.2.3
            if (source.Length == 0)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
            }

            ReadOnlySpan<byte> sourceSpan = source.Span;
            unusedBitCount = sourceSpan[0];
            normalizedLastByte = null;

            // T-REC-X.690-201508 sec 8.6.2.2
            if (unusedBitCount > 7)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
            }

            if (source.Length == 1)
            {
                // T-REC-X.690-201508 sec 8.6.2.4
                if (unusedBitCount > 0)
                {
                    throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
                }

                Debug.Assert(unusedBitCount == 0);
                value = ReadOnlyMemory<byte>.Empty;
                return;
            }

            // Build a mask for the bits that are used so the normalized value can be computed
            //
            // If 3 bits are "unused" then build a mask for them to check for 0.
            // 1 << 3 => 0b0000_1000
            // subtract 1 => 0b0000_0111
            // Invert that to be the positive mask: 0b111_1000
            int mask = ~((1 << unusedBitCount) - 1);
            byte lastByte = sourceSpan[sourceSpan.Length - 1];
            byte maskedByte = (byte)(lastByte & mask);

            if (maskedByte != lastByte)
            {
                // T-REC-X.690-201508 sec 11.2.1
                if (_ruleSet == AsnEncodingRules.DER || _ruleSet == AsnEncodingRules.CER)
                {
                    throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
                }

                normalizedLastByte = maskedByte;
            }

            value = source.Slice(1);
        }

        private delegate void BitStringCopyAction(
            ReadOnlyMemory<byte> value,
            byte? normalizedLastByte,
            Span<byte> destination);

        private static void CopyBitStringValue(
            ReadOnlyMemory<byte> value,
            byte? normalizedLastByte,
            Span<byte> destination)
        {
            if (value.Length == 0)
            {
                return;
            }

            if (normalizedLastByte == null)
            {
                value.Span.CopyTo(destination);
            }
            else
            {
                int lastIdx = value.Length - 1;

                value.Span.Slice(0, lastIdx).CopyTo(destination);
                destination[lastIdx] = normalizedLastByte.Value;
            }
        }

        private int ProcessConstructedBitString(
            ReadOnlyMemory<byte> source,
            ref Span<byte> destination,
            BitStringCopyAction copyAction,
            bool isIndefinite,
            ref int lastUnusedBitCount,
            ref int lastSegmentLength,
            out int bytesRead)
        {
            if (source.IsEmpty)
            {
                if (isIndefinite)
                {
                    throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
                }

                bytesRead = 0;
                return 0;
            }

            int totalRead = 0;
            int totalContent = 0;
            ReadOnlyMemory<byte> cur = source;

            while (!cur.IsEmpty)
            {
                AsnReader reader = new AsnReader(cur, _ruleSet);
                Asn1Tag tag = reader.ReadTagAndLength(out int? length, out int headerLength);

                if (tag.TagClass != TagClass.Universal)
                {
                    throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
                }

                totalRead += headerLength;

                if (isIndefinite && tag.TagValue == (int)UniversalTagNumber.EndOfContents)
                {
                    ValidateEndOfContents(tag, length, headerLength);

                    bytesRead = totalRead;
                    return totalContent;
                }

                if (tag.TagValue != (int)UniversalTagNumber.BitString)
                {
                    // T-REC-X.690-201508 sec 8.6.4.1 (in particular, Note 2)
                    throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
                }

                if (_ruleSet == AsnEncodingRules.CER)
                {
                    if (tag.IsConstructed || lastSegmentLength != MaxCERSegmentSize)
                    {
                        // T-REC-X.690-201508 sec 9.2
                        throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
                    }
                }

                cur = cur.Slice(headerLength);

                if (tag.IsConstructed)
                {
                    totalContent += ProcessConstructedBitString(
                        Slice(cur, 0, length),
                        ref destination,
                        copyAction,
                        length == null,
                        ref lastUnusedBitCount,
                        ref lastSegmentLength,
                        out int nestedContentRead);

                    totalRead += nestedContentRead;
                    cur = cur.Slice(nestedContentRead);
                }
                else
                {
                    if (lastUnusedBitCount != 0)
                    {
                        // T-REC-X.690-201508 sec 8.6.4 (only the last segment can have unused bits)
                        throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
                    }

                    int lengthValue = length.Value;

                    ParsePrimitiveBitStringContents(
                        Slice(cur, 0, lengthValue),
                        out lastUnusedBitCount,
                        out ReadOnlyMemory<byte> value,
                        out byte? normalizedLastByte);

                    totalRead += lengthValue;
                    totalContent += value.Length;
                    lastSegmentLength = lengthValue;
                    cur = cur.Slice(lengthValue);

                    if (copyAction != null)
                    {
                        copyAction(value, normalizedLastByte, destination);

                        destination = destination.Slice(value.Length);
                    }
                }
            }

            if (isIndefinite)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
            }

            bytesRead = totalRead;
            return totalContent;
        }

        private bool TryCopyConstructedBitStringValue(
            ReadOnlyMemory<byte> source,
            Span<byte> dest,
            bool isIndefinite,
            out int unusedBitCount,
            out int bytesRead,
            out int bytesWritten)
        {
            int lastUnusedBitCount = 0;
            int lastSegmentSize = MaxCERSegmentSize;

            Span<byte> tmpDest = dest;

            // Call ProcessConstructedBitString with a null copy-action to get the required byte
            // count and verify that the data is well-formed before copying into dest.
            int contentLength = ProcessConstructedBitString(
                source,
                ref tmpDest,
                null,
                isIndefinite,
                ref lastUnusedBitCount,
                ref lastSegmentSize,
                out int encodedLength);

            // Since the unused bits byte from the segments don't count, only one segment
            // returns 999 (or less), the second segment bumps the count to 1000, and is legal.
            //
            // T-REC-X.690-201508 sec 9.2
            if (_ruleSet == AsnEncodingRules.CER && contentLength < MaxCERSegmentSize)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
            }

            if (dest.Length < contentLength)
            {
                unusedBitCount = 0;
                bytesRead = 0;
                bytesWritten = 0;
                return false;
            }

            unusedBitCount = lastUnusedBitCount;
            tmpDest = dest;
            lastSegmentSize = MaxCERSegmentSize;
            lastUnusedBitCount = 0;

            // Now call it again with the method which fixes the normalized byte in the last segment.
            bytesWritten = ProcessConstructedBitString(
                source,
                ref tmpDest,
                (value, lastByte, destination) => CopyBitStringValue(value, lastByte, destination),
                isIndefinite,
                ref lastUnusedBitCount,
                ref lastSegmentSize,
                out bytesRead);

            Debug.Assert(unusedBitCount == lastUnusedBitCount);
            Debug.Assert(bytesWritten == contentLength);
            Debug.Assert(bytesRead == encodedLength);
            return true;
        }

        private bool TryGetPrimitiveBitStringValue(
            Asn1Tag expectedTag,
            out Asn1Tag actualTag,
            out int? contentsLength,
            out int headerLength,
            out int unusedBitCount,
            out ReadOnlyMemory<byte> value,
            out byte? normalizedLastByte)
        {
            actualTag = ReadTagAndLength(out contentsLength, out headerLength);
            CheckExpectedTag(actualTag, expectedTag, UniversalTagNumber.BitString);

            if (actualTag.IsConstructed)
            {
                if (_ruleSet == AsnEncodingRules.DER)
                {
                    throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
                }

                unusedBitCount = 0;
                value = default(ReadOnlyMemory<byte>);
                normalizedLastByte = null;
                return false;
            }

            Debug.Assert(contentsLength.HasValue);
            ReadOnlyMemory<byte> encodedValue = Slice(_data, headerLength, contentsLength.Value);

            ParsePrimitiveBitStringContents(
                encodedValue,
                out unusedBitCount,
                out value,
                out normalizedLastByte);

            return true;
        }

        public bool TryGetPrimitiveBitStringValue(out int unusedBitCount, out ReadOnlyMemory<byte> contents)
            => TryGetPrimitiveBitStringValue(Asn1Tag.PrimitiveBitString, out unusedBitCount, out contents);

        /// <summary>
        /// Gets a ReadOnlyMemory view over the data value portion of the contents of a bit string.
        /// </summary>
        /// <param name="expectedTag">The expected tag to read</param>
        /// <param name="unusedBitCount">The encoded value for the number of unused bits.</param>
        /// <param name="value">The data value portion of the bit string contents.</param>
        /// <returns>
        ///   <c>true</c> if the bit string uses a primitive encoding and the "unused" bits have value 0,
        ///   <c>false</c> otherwise.
        /// </returns>
        /// <exception cref="CryptographicException">
        ///  <ul>
        ///   <li>No data remains</li>
        ///   <li>The tag read does not match the expected tag</li>
        ///   <li>The length is invalid under the chosen encoding rules</li>
        ///   <li>The unusedBitCount value is out of bounds</li>
        ///   <li>A CER or DER encoding was chosen and an "unused" bit was set to 1</li>
        ///   <li>A CER encoding was chosen and the primitive content length exceeds the maximum allowed</li>
        /// </ul>
        /// </exception>
        public bool TryGetPrimitiveBitStringValue(
            Asn1Tag expectedTag,
            out int unusedBitCount,
            out ReadOnlyMemory<byte> value)
        {
            bool isPrimitive = TryGetPrimitiveBitStringValue(
                expectedTag,
                out Asn1Tag actualTag,
                out int? contentsLength,
                out int headerLength,
                out unusedBitCount,
                out value,
                out byte? normalizedLastByte);

            if (isPrimitive)
            {
                // A BER reader which encountered a situation where an "unused" bit was not
                // set to 0.
                if (normalizedLastByte != null)
                {
                    unusedBitCount = 0;
                    value = default(ReadOnlyMemory<byte>);
                    return false;
                }

                // Skip the tag+length (header) and the unused bit count byte (1) and the contents.
                _data = _data.Slice(headerLength + value.Length + 1);
            }

            return isPrimitive;
        }

        public bool TryCopyBitStringBytes(
            Span<byte> destination,
            out int unusedBitCount,
            out int bytesWritten)
        {
            return TryCopyBitStringBytes(
                Asn1Tag.PrimitiveBitString,
                destination,
                out unusedBitCount,
                out bytesWritten);
        }

        public bool TryCopyBitStringBytes(
            Asn1Tag expectedTag,
            Span<byte> destination,
            out int unusedBitCount,
            out int bytesWritten)
        {
            if (TryGetPrimitiveBitStringValue(
                expectedTag,
                out Asn1Tag actualTag,
                out int? contentsLength,
                out int headerLength,
                out unusedBitCount,
                out ReadOnlyMemory<byte> value,
                out byte? normalizedLastByte))
            {
                if (value.Length > destination.Length)
                {
                    bytesWritten = 0;
                    unusedBitCount = 0;
                    return false;
                }

                CopyBitStringValue(value, normalizedLastByte, destination);

                bytesWritten = value.Length;
                // contents doesn't include the unusedBitCount value, so add one byte for that.
                _data = _data.Slice(headerLength + value.Length + 1);
                return true;
            }

            Debug.Assert(actualTag.IsConstructed);

            bool read = TryCopyConstructedBitStringValue(
                Slice(_data, headerLength, contentsLength),
                destination,
                contentsLength == null,
                out unusedBitCount,
                out int bytesRead,
                out bytesWritten);

            if (read)
            {
                _data = _data.Slice(headerLength + bytesRead);
            }

            return read;
        }

        public TFlagsEnum GetNamedBitListValue<TFlagsEnum>() where TFlagsEnum : struct =>
            GetNamedBitListValue<TFlagsEnum>(Asn1Tag.PrimitiveBitString);

        public TFlagsEnum GetNamedBitListValue<TFlagsEnum>(Asn1Tag expectedTag) where TFlagsEnum : struct
        {
            Type tFlagsEnum = typeof(TFlagsEnum);

            return (TFlagsEnum)Enum.ToObject(tFlagsEnum, GetNamedBitListValue(expectedTag, tFlagsEnum));
        }

        public Enum GetNamedBitListValue(Type tFlagsEnum) =>
            GetNamedBitListValue(Asn1Tag.PrimitiveBitString, tFlagsEnum);

        public Enum GetNamedBitListValue(Asn1Tag expectedTag, Type tFlagsEnum)
        {
            // This will throw an ArgumentException if TEnum isn't an enum type,
            // so we don't need to validate it.
            Type backingType = tFlagsEnum.GetEnumUnderlyingType();

            if (!tFlagsEnum.IsDefined(typeof(FlagsAttribute), false))
            {
                throw new ArgumentException(
                    SR.Cryptography_Asn_NamedBitListRequiresFlagsEnum,
                    nameof(tFlagsEnum));
            }

            int sizeLimit = Marshal.SizeOf(backingType);
            Span<byte> stackSpan = stackalloc byte[sizeLimit];
            ReadOnlyMemory<byte> saveData = _data;

            if (!TryCopyBitStringBytes(expectedTag, stackSpan, out int unusedBitCount, out int bytesWritten))
            {
                throw new CryptographicException(
                    SR.Format(SR.Cryptography_Asn_NamedBitListValueTooBig, tFlagsEnum.Name));
            }

            if (bytesWritten == 0)
            {
                // The mode isn't relevant, zero is always zero.
                return (Enum)Enum.ToObject(tFlagsEnum, 0);
            }

            ReadOnlySpan<byte> valueSpan = stackSpan.Slice(0, bytesWritten);

            // Now that the 0-bounds check is out of the way:
            // 
            // T-REC-X.690-201508 sec 11.2.2
            if (_ruleSet == AsnEncodingRules.DER ||
                _ruleSet == AsnEncodingRules.CER)
            {
                byte lastByte = valueSpan[bytesWritten - 1];

                // No unused bits tests 0x01, 1 is 0x02, 2 is 0x04, etc.
                // We already know that TryCopyBitStringBytes checked that the
                // declared unused bits were 0, this checks that the last "used" bit
                // isn't also zero.
                byte testBit = (byte)(1 << unusedBitCount);

                if ((lastByte & testBit) == 0)
                {
                    _data = saveData;
                    throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
                }
            }

            // Consider a NamedBitList defined as
            //
            //   SomeList ::= BIT STRING {
            //     a(0), b(1), c(2), d(3), e(4), f(5), g(6), h(7), i(8), j(9), k(10)
            //   }
            //
            // The BIT STRING encoding of (a | j) is
            //   unusedBitCount = 6,
            //   contents: 0x80 0x40  (0b10000000_01000000)
            //
            // A the C# exposure of this structure we adhere to is
            //
            // [Flags]
            // enum SomeList
            // {
            //     A = 1,
            //     B = 1 << 1,
            //     C = 1 << 2,
            //     ...
            // }
            //
            // Which happens to be exactly backwards from how the bits are encoded, but the complexity
            // only needs to live here.
            return (Enum)Enum.ToObject(tFlagsEnum, InterpretNamedBitListReversed(valueSpan));
        }

        private static long InterpretNamedBitListReversed(ReadOnlySpan<byte> valueSpan)
        {
            Debug.Assert(valueSpan.Length <= sizeof(long));

            long accum = 0;
            long currentBitValue = 1;

            for (int byteIdx = 0; byteIdx < valueSpan.Length; byteIdx++)
            {
                byte byteVal = valueSpan[byteIdx];

                for (int bitIndex = 7; bitIndex >= 0; bitIndex--)
                {
                    int test = 1 << bitIndex;

                    if ((byteVal & test) != 0)
                    {
                        accum |= currentBitValue;
                    }

                    currentBitValue <<= 1;
                }
            }

            return accum;
        }

        public ReadOnlyMemory<byte> GetEnumeratedBytes() =>
            GetEnumeratedBytes(Asn1Tag.Enumerated);

        public ReadOnlyMemory<byte> GetEnumeratedBytes(Asn1Tag expectedTag)
        {
            // T-REC-X.690-201508 sec 8.4 says the contents are the same as for integers.
            ReadOnlyMemory<byte> contents =
                GetIntegerContents(expectedTag, UniversalTagNumber.Enumerated, out int headerLength);

            _data = _data.Slice(headerLength + contents.Length);
            return contents;
        }

        public TEnum GetEnumeratedValue<TEnum>() where TEnum : struct
        {
            Type tEnum = typeof(TEnum);

            return (TEnum)Enum.ToObject(tEnum, GetEnumeratedValue(tEnum));
        }

        public TEnum GetEnumeratedValue<TEnum>(Asn1Tag expectedTag) where TEnum : struct
        {
            Type tEnum = typeof(TEnum);

            return (TEnum)Enum.ToObject(tEnum, GetEnumeratedValue(expectedTag, tEnum));
        }

        public Enum GetEnumeratedValue(Type tEnum) =>
            GetEnumeratedValue(Asn1Tag.Enumerated, tEnum);

        public Enum GetEnumeratedValue(Asn1Tag expectedTag, Type tEnum)
        {
            const UniversalTagNumber tagNumber = UniversalTagNumber.Enumerated;
            
            // This will throw an ArgumentException if TEnum isn't an enum type,
            // so we don't need to validate it.
            Type backingType = tEnum.GetEnumUnderlyingType();

            if (tEnum.IsDefined(typeof(FlagsAttribute), false))
            {
                throw new ArgumentException(
                    SR.Cryptography_Asn_EnumeratedValueRequiresNonFlagsEnum,
                    nameof(tEnum));
            }

            // T-REC-X.690-201508 sec 8.4 says the contents are the same as for integers.
            int sizeLimit = Marshal.SizeOf(backingType);

            if (backingType == typeof(int) ||
                backingType == typeof(long) ||
                backingType == typeof(short) ||
                backingType == typeof(sbyte))
            {
                if (!TryReadSignedInteger(sizeLimit, expectedTag, tagNumber, out long value))
                {
                    throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
                }

                return (Enum)Enum.ToObject(tEnum, value);
            }

            if (backingType == typeof(uint) ||
                backingType == typeof(ulong) ||
                backingType == typeof(ushort) ||
                backingType == typeof(byte))
            {
                if (!TryReadUnsignedInteger(sizeLimit, expectedTag, tagNumber, out ulong value))
                {
                    throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
                }

                return (Enum)Enum.ToObject(tEnum, value);
            }

            Debug.Fail($"No handler for type {backingType.Name}");
            throw new CryptographicException();
        }

        private bool TryGetOctetStringBytes(
            Asn1Tag expectedTag,
            out ReadOnlyMemory<byte> contents,
            out int headerLength,
            UniversalTagNumber universalTagNumber = UniversalTagNumber.OctetString)
        {
            Asn1Tag tag = ReadTagAndLength(out int ? length, out headerLength);
            CheckExpectedTag(tag, expectedTag, universalTagNumber);

            if (tag.IsConstructed)
            {
                if (_ruleSet == AsnEncodingRules.DER)
                {
                    throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
                }

                contents = default(ReadOnlyMemory<byte>);
                return false;
            }

            Debug.Assert(length.HasValue);
            ReadOnlyMemory<byte> encodedValue = Slice(_data, headerLength, length.Value);

            if (_ruleSet == AsnEncodingRules.CER && encodedValue.Length > MaxCERSegmentSize)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
            }

            contents = encodedValue;
            return true;
        }

        private bool TryGetOctetStringBytes(
            Asn1Tag expectedTag,
            UniversalTagNumber universalTagNumber,
            out ReadOnlyMemory<byte> contents)
        {
            if (TryGetOctetStringBytes(expectedTag, out contents, out int headerLength, universalTagNumber))
            {
                _data = _data.Slice(headerLength + contents.Length);
                return true;
            }

            return false;
        }

        public bool TryGetOctetStringBytes(out ReadOnlyMemory<byte> contents) =>
            TryGetOctetStringBytes(Asn1Tag.PrimitiveOctetString, out contents);

        /// <summary>
        /// Gets the source data for an OctetString under a primitive encoding.
        /// </summary>
        /// <param name="expectedTag">The expected tag value</param>
        /// <param name="contents">The content bytes for the OctetString payload.</param>
        /// <returns>
        ///   <c>true</c> if the octet string uses a primitive encoding, <c>false</c> otherwise.
        /// </returns>
        /// <exception cref="CryptographicException">
        ///  <ul>
        ///   <li>No data remains</li>
        ///   <li>The tag read did not match the expected tag</li>
        ///   <li>The length is invalid under the chosen encoding rules</li>
        ///   <li>A CER encoding was chosen and the primitive content length exceeds the maximum allowed</li>
        /// </ul>
        /// </exception>
        public bool TryGetOctetStringBytes(Asn1Tag expectedTag, out ReadOnlyMemory<byte> contents)
        {
            return TryGetOctetStringBytes(expectedTag, UniversalTagNumber.OctetString, out contents);
        }

        private static int CopyConstructedOctetString(
            ReadOnlyMemory<byte> source,
            ref Span<byte> destination,
            bool write,
            AsnEncodingRules ruleSet,
            bool isIndefinite,
            ref int lastSegmentLength,
            out int bytesRead)
        {
            if (source.IsEmpty)
            {
                if (isIndefinite)
                {
                    throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
                }

                bytesRead = 0;
                return 0;
            }

            int totalRead = 0;
            int totalContent = 0;
            ReadOnlyMemory<byte> cur = source;

            while (!cur.IsEmpty)
            {
                AsnReader reader = new AsnReader(cur, ruleSet);
                Asn1Tag tag = reader.ReadTagAndLength(out int? length, out int headerLength);
                
                if (tag.TagClass != TagClass.Universal)
                {
                    throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
                }

                totalRead += headerLength;

                if (isIndefinite && tag.TagValue == (int)UniversalTagNumber.EndOfContents)
                {
                    ValidateEndOfContents(tag, length, headerLength);

                    bytesRead = totalRead;
                    return totalContent;
                }

                if (tag.TagValue != (int)UniversalTagNumber.OctetString)
                {
                    // T-REC-X.690-201508 sec 8.7.3.2 (in particular, Note 2)
                    throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
                }

                if (ruleSet == AsnEncodingRules.CER)
                {
                    if (tag.IsConstructed || lastSegmentLength != MaxCERSegmentSize)
                    {
                        // T-REC-X.690-201508 sec 9.2
                        throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
                    }
                }

                cur = cur.Slice(headerLength);

                if (length == null)
                {
                    Debug.Assert(tag.IsConstructed);

                    totalContent += CopyConstructedOctetString(
                        cur,
                        ref destination,
                        write,
                        ruleSet,
                        true,
                        ref lastSegmentLength,
                        out int nestedBytesRead);

                    totalRead += nestedBytesRead;
                    cur = cur.Slice(nestedBytesRead);
                }
                else if (tag.IsConstructed)
                {
                    totalContent += CopyConstructedOctetString(
                        Slice(cur, 0, length.Value),
                        ref destination,
                        write,
                        ruleSet,
                        false,
                        ref lastSegmentLength,
                        out int nestedContentRead);

                    totalRead += nestedContentRead;
                    cur = cur.Slice(nestedContentRead);
                }
                else
                {
                    int lengthValue = length.Value;

                    // T-REC-X.690-201508 sec 9.2
                    if (ruleSet == AsnEncodingRules.CER && lengthValue > MaxCERSegmentSize)
                    {
                        throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
                    }

                    totalRead += lengthValue;
                    totalContent += lengthValue;
                    lastSegmentLength = lengthValue;

                    ReadOnlyMemory<byte> segment = Slice(cur, 0, lengthValue);
                    cur = cur.Slice(lengthValue);

                    if (write)
                    {
                        segment.Span.CopyTo(destination);
                        destination = destination.Slice(lengthValue);
                    }
                }
            }

            if (isIndefinite)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
            }

            bytesRead = totalRead;
            return totalContent;
        }

        private static bool TryCopyConstructedOctetStringValue(
            ReadOnlyMemory<byte> source,
            Span<byte> dest,
            bool write,
            AsnEncodingRules ruleSet,
            bool isIndefinite,
            out int bytesRead,
            out int bytesWritten)
        {
            int lastSegmentSize = MaxCERSegmentSize;

            Span<byte> tmpDest = dest;

            int contentLength = CopyConstructedOctetString(
                source,
                ref tmpDest,
                false,
                ruleSet,
                isIndefinite,
                ref lastSegmentSize,
                out int encodedLength);

            // T-REC-X.690-201508 sec 9.2
            if (ruleSet == AsnEncodingRules.CER && contentLength <= MaxCERSegmentSize)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
            }

            if (!write)
            {
                bytesRead = encodedLength;
                bytesWritten = contentLength;
                return true;
            }

            if (dest.Length < contentLength)
            {
                bytesRead = 0;
                bytesWritten = 0;
                return false;
            }

            tmpDest = dest;
            lastSegmentSize = MaxCERSegmentSize;

            bytesWritten = CopyConstructedOctetString(
                source,
                ref tmpDest,
                true,
                ruleSet,
                isIndefinite,
                ref lastSegmentSize,
                out bytesRead);

            Debug.Assert(bytesWritten == contentLength);
            Debug.Assert(bytesRead == encodedLength);
            return true;
        }

        public bool TryCopyOctetStringBytes(
            Span<byte> destination,
            out int bytesWritten)
        {
            return TryCopyOctetStringBytes(
                Asn1Tag.PrimitiveOctetString,
                destination,
                out bytesWritten);
        }

        public bool TryCopyOctetStringBytes(
            Asn1Tag expectedTag,
            Span<byte> destination,
            out int bytesWritten)
        {
            if (TryGetOctetStringBytes(
                expectedTag,
                out ReadOnlyMemory<byte> contents,
                out int headerLength))
            {
                if (contents.Length > destination.Length)
                {
                    bytesWritten = 0;
                    return false;
                }

                contents.Span.CopyTo(destination);
                bytesWritten = contents.Length;
                _data = _data.Slice(headerLength + contents.Length);
                return true;
            }

            Asn1Tag tag = ReadTagAndLength(out int ? length, out headerLength);

            bool copied = TryCopyConstructedOctetStringValue(
                Slice(_data, headerLength, length),
                destination,
                true,
                _ruleSet,
                length == null,
                out int bytesRead,
                out bytesWritten);

            if (copied)
            {
                _data = _data.Slice(headerLength + bytesRead);
            }

            return copied;
        }

        public void ReadNull() => ReadNull(Asn1Tag.Null);

        public void ReadNull(Asn1Tag expectedTag)
        {
            Asn1Tag tag = ReadTagAndLength(out int ? length, out int headerLength);
            CheckExpectedTag(tag, expectedTag, UniversalTagNumber.Null);

            // T-REC-X.690-201508 sec 8.8.1
            // T-REC-X.690-201508 sec 8.8.2
            if (tag.IsConstructed || length != 0)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
            }

            _data = _data.Slice(headerLength);
        }
        
        private static BigInteger ReadSubIdentifier(
            ReadOnlySpan<byte> source,
            out int bytesRead)
        {
            Debug.Assert(source.Length > 0);

            // T-REC-X.690-201508 sec 8.19.2 (last sentence)
            if (source[0] == 0x80)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
            }

            BigInteger accum = BigInteger.Zero;

            for (int idx = 0; idx < source.Length; idx++)
            {
                byte cur = source[idx];

                accum <<= 7;
                accum |= (cur & 0x7F);

                // If the high bit isn't set this marks the end of the sub-identifier.
                if (cur < 0x80)
                {
                    bytesRead = idx + 1;
                    return accum;
                }
            }

            throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
        }

        private string ReadObjectIdentifierAsString(Asn1Tag expectedTag, out int totalBytesRead)
        {
            Asn1Tag tag = ReadTagAndLength(out int ? length, out int headerLength);
            CheckExpectedTag(tag, expectedTag, UniversalTagNumber.ObjectIdentifier);

            // T-REC-X.690-201508 sec 8.19.1
            // T-REC-X.690-201508 sec 8.19.2 says the minimum length is 1
            if (tag.IsConstructed || length < 1)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
            }

            ReadOnlyMemory<byte> contentsMemory = Slice(_data, headerLength, length.Value);
            ReadOnlySpan<byte> contents = contentsMemory.Span;

            BigInteger firstIdentifier = ReadSubIdentifier(contents, out int bytesRead);
            byte firstArc;

            // T-REC-X.690-201508 sec 8.19.4
            // The first two subidentifiers (X.Y) are encoded as (X * 40) + Y, because Y is
            // bounded [0, 39] for X in {0, 1}, and only X in {0, 1, 2} are legal.
            // So:
            // * identifier < 40 => X = 0, Y = identifier.
            // * identifier < 80 => X = 1, Y = identifier - 40.
            // * else: X = 2, Y = identifier - 80.

            if (firstIdentifier < 40)
            {
                firstArc = 0;
            }
            else if (firstIdentifier < 80)
            {
                firstArc = 1;
                firstIdentifier -= 40;
            }
            else
            {
                firstArc = 2;
                firstIdentifier -= 80;
            }

            StringBuilder builder = new StringBuilder(contents.Length * 4);
            builder.Append(firstArc);
            builder.Append('.');
            builder.Append(firstIdentifier.ToString());

            contents = contents.Slice(bytesRead);

            while (!contents.IsEmpty)
            {
                BigInteger subIdentifier = ReadSubIdentifier(contents, out bytesRead);
                builder.Append('.');
                builder.Append(subIdentifier.ToString());

                contents = contents.Slice(bytesRead);
            }

            totalBytesRead = headerLength + length.Value;
            return builder.ToString();
        }

        public string ReadObjectIdentifierAsString() =>
            ReadObjectIdentifierAsString(Asn1Tag.ObjectIdentifier);
        
        public string ReadObjectIdentifierAsString(Asn1Tag expectedTag)
        {
            string oidValue = ReadObjectIdentifierAsString(expectedTag, out int bytesRead);

            _data = _data.Slice(bytesRead);

            return oidValue;
        }

        public Oid ReadObjectIdentifier(bool skipFriendlyName = false) =>
            ReadObjectIdentifier(Asn1Tag.ObjectIdentifier, skipFriendlyName);

        public Oid ReadObjectIdentifier(Asn1Tag expectedTag, bool skipFriendlyName=false)
        {
            string oidValue = ReadObjectIdentifierAsString(expectedTag, out int bytesRead);
            Oid oid = skipFriendlyName ? new Oid(oidValue, oidValue) : new Oid(oidValue);

            // Don't slice until the return object has been created.
            _data = _data.Slice(bytesRead);

            return oid;
        }

        private bool TryCopyCharacterStringBytes(
            Asn1Tag expectedTag,
            UniversalTagNumber universalTagNumber,
            Span<byte> destination,
            bool write,
            out int bytesRead,
            out int bytesWritten)
        {
            if (TryGetOctetStringBytes(
                expectedTag,
                out ReadOnlyMemory<byte> contents,
                out int headerLength,
                universalTagNumber))
            {
                bytesWritten = contents.Length;

                if (write)
                {
                    if (destination.Length < bytesWritten)
                    {
                        bytesWritten = 0;
                        bytesRead = 0;
                        return false;
                    }

                    contents.Span.CopyTo(destination);
                }

                bytesRead = headerLength + bytesWritten;
                return true;
            }

            Asn1Tag tag = ReadTagAndLength(out int ? length, out headerLength);

            bool copied = TryCopyConstructedOctetStringValue(
                Slice(_data, headerLength, length),
                destination,
                write,
                _ruleSet,
                length == null,
                out int contentBytesRead,
                out bytesWritten);

            bytesRead = headerLength + contentBytesRead;
            return copied;
        }

        private static unsafe string GetCharacterString(
            ReadOnlyMemory<byte> source,
            Text.Encoding encoding)
        {
            fixed (byte* bytePtr = &source.Span.DangerousGetPinnableReference())
            {
                try
                {
                    return encoding.GetString(bytePtr, source.Length);
                }
                catch (DecoderFallbackException e)
                {
                    throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding, e);
                }
            }
        }

        private static unsafe bool TryCopyCharacterString(
            ReadOnlyMemory<byte> source,
            Text.Encoding encoding,
            Span<char> destination,
            out int charsWritten)
        {
            fixed (byte* bytePtr = &source.Span.DangerousGetPinnableReference())
            fixed (char* charPtr = &destination.DangerousGetPinnableReference())
            {
                try
                {
                    int charCount = encoding.GetCharCount(bytePtr, source.Length);

                    if (charCount > destination.Length)
                    {
                        charsWritten = 0;
                        return false;
                    }

                    charsWritten = encoding.GetChars(bytePtr, source.Length, charPtr, destination.Length);
                    Debug.Assert(charCount == charsWritten);
                }
                catch (DecoderFallbackException e)
                {
                    throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding, e);
                }

                return true;
            }
        }

        private string GetCharacterString(
            Asn1Tag expectedTag,
            UniversalTagNumber universalTagNumber,
            Text.Encoding encoding)
        {
            if (TryGetOctetStringBytes(
                expectedTag,
                out ReadOnlyMemory<byte> contents,
                out int headerLength,
                universalTagNumber))
            {
                string s = GetCharacterString(contents, encoding);

                _data = _data.Slice(headerLength + contents.Length);

                return s;
            }

            bool parsed = TryCopyCharacterStringBytes(
                expectedTag,
                universalTagNumber,
                Span<byte>.Empty,
                false,
                out int bytesRead,
                out int bytesWritten);

            Debug.Assert(parsed, "TryCopyCharacterStringBytes returned false in counting mode");

            byte[] rented = ArrayPool<byte>.Shared.Rent(bytesWritten);

            try
            {
                if (!TryCopyCharacterStringBytes(expectedTag, universalTagNumber, rented, true, out bytesRead, out bytesWritten))
                {
                    Debug.Fail("TryCopyCharacterStringBytes failed with a precomputed size");
                    throw new CryptographicException();
                }

                string s = GetCharacterString(
                    new ReadOnlyMemory<byte>(rented, 0, bytesWritten),
                    encoding);

                _data = _data.Slice(bytesRead);

                return s;
            }
            finally
            {
                Array.Clear(rented, 0, bytesWritten);
                ArrayPool<byte>.Shared.Return(rented);
            }
        }

        private bool TryCopyCharacterString(
            Asn1Tag expectedTag,
            UniversalTagNumber universalTagNumber,
            Text.Encoding encoding,
            Span<char> destination,
            out int charsWritten)
        {
            if (TryGetOctetStringBytes(
                expectedTag,
                out ReadOnlyMemory<byte> contents,
                out int headerLength,
                universalTagNumber))
            {
                bool copied = TryCopyCharacterString(contents, encoding, destination, out charsWritten);

                if (copied)
                {
                    _data = _data.Slice(headerLength + contents.Length);
                }

                return copied;
            }

            bool parsed = TryCopyCharacterStringBytes(
                expectedTag,
                universalTagNumber,
                Span<byte>.Empty,
                false,
                out int bytesRead,
                out int bytesWritten);

            Debug.Assert(parsed, "TryCopyCharacterStringBytes returned false in counting mode");

            byte[] rented = ArrayPool<byte>.Shared.Rent(bytesWritten);

            try
            {
                if (!TryCopyCharacterStringBytes(expectedTag, universalTagNumber, rented, true, out bytesRead, out bytesWritten))
                {
                    Debug.Fail("TryCopyCharacterStringBytes failed with a precomputed size");
                    throw new CryptographicException();
                }

                bool copied = TryCopyCharacterString(
                    new ReadOnlyMemory<byte>(rented, 0, bytesWritten), 
                    encoding,
                    destination,
                    out charsWritten);

                if (copied)
                {
                    _data = _data.Slice(bytesRead);
                }

                return copied;
            }
            finally
            {
                Array.Clear(rented, 0, bytesWritten);
                ArrayPool<byte>.Shared.Return(rented);
            }
        }

        /// <summary>
        /// Gets the source data for a character string under a primitive encoding and tagged as
        /// the universal class tag for the encoding type.
        /// </summary>
        /// <param name="encodingType">The UniversalTagNumber for the string encoding type.</param>
        /// <param name="contents">The content bytes for the UTF8String payload.</param>
        /// <returns>
        ///   <c>true</c> if the character string uses a primitive encoding, <c>false</c> otherwise.
        /// </returns>
        /// <exception cref="CryptographicException">
        ///  <ul>
        ///   <li>No data remains</li>
        ///   <li>The tag read does not match the expected tag</li>
        ///   <li>The length is invalid under the chosen encoding rules</li>
        ///   <li>A CER encoding was chosen and the primitive content length exceeds the maximum allowed</li>
        /// </ul>
        /// </exception>
        public bool TryGetCharacterStringBytes(UniversalTagNumber encodingType, out ReadOnlyMemory<byte> contents)
        {
            return TryGetCharacterStringBytes(new Asn1Tag(encodingType), encodingType, out contents);
        }

        /// <summary>
        /// Gets the source data for a character string under a primitive encoding.  The contents
        /// are not validated as belonging to the requested encoding type.
        /// </summary>
        /// <param name="expectedTag">The expected tag</param>
        /// <param name="encodingType">The UniversalTagNumber for the string encoding type.</param>
        /// <param name="contents">The content bytes for the UTF8String payload.</param>
        /// <returns>
        ///   <c>true</c> if the character string uses a primitive encoding, <c>false</c> otherwise.
        /// </returns>
        /// <exception cref="CryptographicException">
        ///  <ul>
        ///   <li>No data remains</li>
        ///   <li>The tag read does not match the expected tag</li>
        ///   <li>The length is invalid under the chosen encoding rules</li>
        ///   <li>A CER encoding was chosen and the primitive content length exceeds the maximum allowed</li>
        /// </ul>
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="encodingType"/> is not a known character string encoding type.
        /// </exception>
        public bool TryGetCharacterStringBytes(
            Asn1Tag expectedTag,
            UniversalTagNumber encodingType,
            out ReadOnlyMemory<byte> contents)
        {
            CheckCharacterStringEncodingType(encodingType);
            return TryGetOctetStringBytes(expectedTag, encodingType, out contents);
        }

        public bool TryCopyCharacterStringBytes(
            UniversalTagNumber encodingType,
            Span<byte> destination,
            out int bytesWritten)
        {
            return TryCopyCharacterStringBytes(
                new Asn1Tag(encodingType),
                encodingType,
                destination,
                out bytesWritten);
        }

        public bool TryCopyCharacterStringBytes(
            Asn1Tag expectedTag,
            UniversalTagNumber encodingType,
            Span<byte> destination,
            out int bytesWritten)
        {
            CheckCharacterStringEncodingType(encodingType);

            bool copied = TryCopyCharacterStringBytes(
                expectedTag,
                encodingType,
                destination,
                true,
                out int bytesRead,
                out bytesWritten);

            if (copied)
            {
                _data = _data.Slice(bytesRead);
            }

            return copied;
        }

        public bool TryCopyCharacterString(
            UniversalTagNumber encodingType,
            Span<char> destination,
            out int charsWritten)
        {
            return TryCopyCharacterString(
                new Asn1Tag(encodingType),
                encodingType,
                destination,
                out charsWritten);
        }

        public bool TryCopyCharacterString(
            Asn1Tag expectedTag,
            UniversalTagNumber encodingType,
            Span<char> destination,
            out int charsWritten)
        {
            Text.Encoding encoding = AsnCharacterStringEncodings.GetEncoding(encodingType);
            return TryCopyCharacterString(expectedTag, encodingType, encoding, destination, out charsWritten);
        }

        public string GetCharacterString(UniversalTagNumber encodingType) =>
            GetCharacterString(new Asn1Tag(encodingType), encodingType);
            
        public string GetCharacterString(Asn1Tag expectedTag, UniversalTagNumber encodingType)
        {
            Text.Encoding encoding = AsnCharacterStringEncodings.GetEncoding(encodingType);
            return GetCharacterString(expectedTag, encodingType, encoding);
        }

        public AsnReader ReadSequence() => ReadSequence(Asn1Tag.Sequence);

        public AsnReader ReadSequence(Asn1Tag expectedTag)
        {
            Asn1Tag tag = ReadTagAndLength(out int ? length, out int headerLength);
            CheckExpectedTag(tag, expectedTag, UniversalTagNumber.Sequence);

            // T-REC-X.690-201508 sec 8.9.1
            // T-REC-X.690-201508 sec 8.10.1
            if (!tag.IsConstructed)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
            }

            ReadOnlyMemory<byte> contents;
            int suffix = 0;

            if (length != null)
            {
                contents = Slice(_data, headerLength, length.Value);
            }
            else
            {
                contents = SeekEndOfContents(_data.Slice(headerLength), _ruleSet, 0);
                suffix = EndOfContentsEncodedLength;
            }

            _data = _data.Slice(headerLength + contents.Length + suffix);
            return new AsnReader(contents, _ruleSet);
        }

        /// <summary>
        /// Builds a new AsnReader over the bytes bounded by the current position which
        /// corresponds to an ASN.1 SET OF value, validating the CER or DER sort ordering
        /// unless suppressed.
        /// </summary>
        /// <param name="skipSortOrderValidation">
        ///   <c>false</c> to validate the sort ordering of the contents, <c>true</c> to
        ///   allow reading the data without verifying it was properly sorted by the writer.
        /// </param>
        /// <returns>An AsnReader over the current position, bounded by the contained length value.</returns>
        public AsnReader ReadSetOf(bool skipSortOrderValidation = false) =>
            ReadSetOf(Asn1Tag.SetOf, skipSortOrderValidation);

        public AsnReader ReadSetOf(Asn1Tag expectedTag, bool skipSortOrderValidation = false)
        {
            Asn1Tag tag = ReadTagAndLength(out int ? length, out int headerLength);
            CheckExpectedTag(tag, expectedTag, UniversalTagNumber.SetOf);

            // T-REC-X.690-201508 sec 8.12.1
            if (!tag.IsConstructed)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
            }

            ReadOnlyMemory<byte> contents;
            int suffix = 0;

            if (length != null)
            {
                contents = Slice(_data, headerLength, length.Value);
            }
            else
            {
                contents = SeekEndOfContents(_data.Slice(headerLength), _ruleSet);
                suffix = EndOfContentsEncodedLength;
            }

            if (!skipSortOrderValidation)
            {
                // T-REC-X.690-201508 sec 11.6
                // BER data is not required to be sorted.
                if (_ruleSet == AsnEncodingRules.DER ||
                    _ruleSet == AsnEncodingRules.CER)
                {
                    AsnReader reader = new AsnReader(contents, _ruleSet);

                    ReadOnlySpan<byte> current = ReadOnlySpan<byte>.Empty;

                    while (reader.HasData)
                    {
                        ReadOnlySpan<byte> previous = current;
                        current = reader.GetEncodedValue().Span;

                        int end = Math.Min(previous.Length, current.Length);
                        int i;

                        for (i = 0; i < end; i++)
                        {
                            byte currentVal = current[i];
                            byte previousVal = previous[i];

                            if (currentVal > previousVal)
                            {
                                break;
                            }

                            if (currentVal < previousVal)
                            {
                                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
                            }
                        }

                        if (i == end)
                        {
                            // If everything was a tie then we treat the shorter thing as if it were
                            // followed by an infinite number of 0x00s.  So "previous" better not have
                            // more data, or if it does, none of it can be non-zero.
                            //
                            // Note: It doesn't seem possible for the tiebreaker to matter.
                            // In DER everything is length prepended, so the content is only compared
                            // if the tag and length were the same.
                            //
                            // In CER you could have an indefinite octet string, but it will contain
                            // primitive octet strings and EoC. So at some point an EoC is compared
                            // against a tag, and the sort order is determined.
                            //
                            // But since the spec calls it out, maybe there's something degenerate, so
                            // we'll guard against it anyways.

                            for (; i < previous.Length; i++)
                            {
                                if (previous[i] != 0)
                                {
                                    throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
                                }
                            }
                        }
                    }
                }
            }

            _data = _data.Slice(headerLength + contents.Length + suffix);
            return new AsnReader(contents, _ruleSet);
        }

        private static byte GetDigit(byte b)
        {
            if (b >= '0' && b <= '9')
            {
                return (byte)(b - '0');
            }

            throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
        }

        private static DateTimeOffset ParseUtcTime(ReadOnlySpan<byte> contentOctets, int twoDigitYearMax)
        {
            // The full allowed formats (T-REC-X.680-201510 sec 47.3)
            // YYMMDDhhmmZ  (a, b1, c1)
            // YYMMDDhhmm+hhmm (a, b1, c2+)
            // YYMMDDhhmm-hhmm (a, b1, c2-)
            // YYMMDDhhmmssZ (a, b2, c1)
            // YYMMDDhhmmss+hhmm (a, b2, c2+)
            // YYMMDDhhmmss-hhmm (a, b2, c2-)

            const int AB1C1Length = 11;
            const int AB1C2Length = AB1C1Length + 4;
            const int AB2C1Length = AB1C1Length + 2;
            const int AB2C2Length = AB2C1Length + 4;

            // 11, 13, 15, 17 are legal.
            // Range check + odd.
            if (contentOctets.Length < AB1C1Length ||
                contentOctets.Length > AB2C2Length ||
                (contentOctets.Length & 1) != 1)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
            }

            int year = 10 * GetDigit(contentOctets[0]) + GetDigit(contentOctets[1]);
            int month = 10 * GetDigit(contentOctets[2]) + GetDigit(contentOctets[3]);
            int day = 10 * GetDigit(contentOctets[4]) + GetDigit(contentOctets[5]);
            int hour = 10 * GetDigit(contentOctets[6]) + GetDigit(contentOctets[7]);
            int minute = 10 * GetDigit(contentOctets[8]) + GetDigit(contentOctets[9]);
            int second = 0;
            int offsetHour = 0;
            int offsetMinute = 0;
            bool minus = false;

            if (contentOctets.Length == AB1C1Length)
            {
                if (contentOctets[10] != 'Z')
                {
                    throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
                }
            }
            else if (contentOctets.Length == AB1C2Length)
            {
                if (contentOctets[10] == '-')
                {
                    minus = true;
                }
                else if (contentOctets[10] != '+')
                {
                    throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
                }

                offsetHour = 10 * GetDigit(contentOctets[11]) + GetDigit(contentOctets[12]);
                offsetMinute = 10 * GetDigit(contentOctets[13]) + GetDigit(contentOctets[14]);
            }
            else
            {
                second = 10 * GetDigit(contentOctets[10]) + GetDigit(contentOctets[11]);

                if (contentOctets.Length == AB2C1Length)
                {
                    if (contentOctets[12] != 'Z')
                    {
                        throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
                    }
                }
                else
                {
                    Debug.Assert(contentOctets.Length == AB2C2Length);

                    if (contentOctets[12] == '-')
                    {
                        minus = true;
                    }
                    else if (contentOctets[12] != '+')
                    {
                        throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
                    }

                    offsetHour = 10 * GetDigit(contentOctets[13]) + GetDigit(contentOctets[14]);
                    offsetMinute = 10 * GetDigit(contentOctets[15]) + GetDigit(contentOctets[16]);
                }
            }

            TimeSpan offset = new TimeSpan(offsetHour, offsetMinute, 0);

            if (minus)
            {
                offset = TimeSpan.Zero - offset;
            }

            int y = year % 100;
            int scaledYear = ((twoDigitYearMax / 100 - (y > twoDigitYearMax % 100 ? 1 : 0)) * 100 + y);

            try
            {
                return new DateTimeOffset(scaledYear, month, day, hour, minute, second, offset);
            }
            catch (Exception e)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding, e);
            }
        }

        public DateTimeOffset GetUtcTime(int twoDigitYearMax = 2049) =>
            GetUtcTime(Asn1Tag.UtcTime, twoDigitYearMax);

        /// <summary>
        /// Gets the DateTimeOffset represented by a UTCTime value.
        /// </summary>
        /// <param name="expectedTag">The expected tag</param>
        /// <param name="twoDigitYearMax">
        /// The largest year to represent with this value.
        /// The default value, 2049, represents the 1950-2049 range for X.509 certificates.
        /// </param>
        /// <returns>
        /// A DateTimeOffset representing the value encoded in the UTCTime.
        /// </returns>
        /// <seealso cref="System.Globalization.Calendar.TwoDigitYearMax"/>
        public DateTimeOffset GetUtcTime(Asn1Tag expectedTag, int twoDigitYearMax = 2049)
        {
            // T-REC-X.680-201510 sec 47.3 says it is IMPLICIT VisibleString, which means
            // that BER is allowed to do complex constructed forms.

            // The full allowed formats (T-REC-X.680-201510 sec 47.3)
            // YYMMDDhhmmZ  (a, b1, c1)
            // YYMMDDhhmm+hhmm (a, b1, c2+)
            // YYMMDDhhmm-hhmm (a, b1, c2-)
            // YYMMDDhhmmssZ (a, b2, c1)
            // YYMMDDhhmmss+hhmm (a, b2, c2+)
            // YYMMDDhhmmss-hhmm (a, b2, c2-)

            // CER and DER are restricted to YYMMDDhhmmssZ
            // T-REC-X.690-201510 sec 11.8
            
            // Optimize for the CER/DER primitive encoding:
            if (TryGetOctetStringBytes(
                expectedTag,
                out ReadOnlyMemory<byte> primitiveOctets,
                out int headerLength,
                UniversalTagNumber.UtcTime))
            {
                if (primitiveOctets.Length == 13)
                {
                    DateTimeOffset value = ParseUtcTime(primitiveOctets.Span, twoDigitYearMax);
                    _data = _data.Slice(headerLength + primitiveOctets.Length);
                    return value;
                }
            }

            // T-REC-X.690-201510 sec 11.8
            if (_ruleSet == AsnEncodingRules.DER || _ruleSet == AsnEncodingRules.CER)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
            }

            // The longest legal format is (a, b2, c2), which comes out to 17 characters/bytes.
            byte[] rented = ArrayPool<byte>.Shared.Rent(17);
            ReadOnlySpan<byte> contentOctets = ReadOnlySpan<byte>.Empty;

            try
            {
                if (TryCopyCharacterStringBytes(
                    expectedTag,
                    UniversalTagNumber.UtcTime,
                    rented,
                    true,
                    out int bytesRead,
                    out int contentLength))
                {
                    contentOctets = Slice(rented, 0, contentLength);

                    DateTimeOffset value = ParseUtcTime(contentOctets, twoDigitYearMax);
                    // Includes the header
                    _data = _data.Slice(bytesRead);
                    return value;
                }
            }
            finally
            {
                Array.Clear(rented, 0, contentOctets.Length);
                ArrayPool<byte>.Shared.Return(rented);
            }

            throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
        }

        private static DateTimeOffset ParseGeneralizedTime(
            AsnEncodingRules ruleSet,
            ReadOnlySpan<byte> contentOctets,
            bool disallowFractions)
        {
            // T-REC-X.680-201510 sec 46 defines a lot of formats for GeneralizedTime.
            //
            // All formats start with yyyyMMdd.
            //
            // "Local time" formats are
            //   [date]HH.fractionOfAnHourToAnArbitraryPrecision
            //   [date]HHmm.fractionOfAMinuteToAnArbitraryPrecision
            //   [date]HHmmss.fractionOfASecondToAnArbitraryPrecision
            //
            // "UTC time" formats are the local formats suffixed with 'Z'
            //
            // "UTC offset time" formats are the local formats suffixed with
            //  +HH
            //  +HHmm
            //  -HH
            //  -HHmm
            // Additionally, it us unclear if the following formats are supposed to be supported,
            // because the ISO 8601:2004 spec is behind a paywall.
            //  +HH:mm
            //  -HH:mm
            //
            // Also, every instance of '.' is actually "period or comma".

            // Since DateTimeOffset doesn't have a notion of
            // "I'm a local time, but with an unknown offset", the computer's current offset will
            // be used.

            // T-REC-X.690-201510 sec 11.7 binds CER and DER to a much smaller set of inputs:
            //  * Only the UTC/Z format can be used.
            //  * HHmmss must always be used
            //  * If fractions are present they will be separated by period, never comma.
            //  * If fractions are present the last digit mustn't be 0.

            bool strict = ruleSet == AsnEncodingRules.DER || ruleSet == AsnEncodingRules.CER;
            if (strict && contentOctets.Length < 15)
            {
                // yyyyMMddHHmmssZ
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
            }
            else if (contentOctets.Length < 10)
            {
                // yyyyMMddHH
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
            }

            int offset = 0;
            int year =
                1000 * GetDigit(contentOctets[offset++]) +
                100 * GetDigit(contentOctets[offset++]) +
                10 * GetDigit(contentOctets[offset++]) +
                GetDigit(contentOctets[offset++]);

            int month = 10 * GetDigit(contentOctets[offset++]) + GetDigit(contentOctets[offset++]);
            int day = 10 * GetDigit(contentOctets[offset++]) + GetDigit(contentOctets[offset++]);
            int hour = 10 * GetDigit(contentOctets[offset++]) + GetDigit(contentOctets[offset++]);
            int? minute = null;
            int? second = null;
            ulong fraction = 0;
            ulong fractionScale = 1;
            TimeSpan? timeOffset = null;
            bool isZulu = false;

            const byte HmsState = 0;
            const byte FracState = 1;
            const byte SuffixState = 2;
            byte state = HmsState;
            
            if (contentOctets.Length > offset)
            {
                byte octet = contentOctets[offset];

                if (octet == 'Z' || octet == '-' || octet == '+')
                {
                    state = SuffixState;
                }
                else if (octet == '.' || octet == ',')
                {
                    state = FracState;
                }
                else if (contentOctets.Length - 1 <= offset)
                {
                    throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
                }
                else
                {
                    minute = 10 * GetDigit(contentOctets[offset++]) + GetDigit(contentOctets[offset++]);
                }
            }

            if (state == HmsState && contentOctets.Length > offset)
            {
                byte octet = contentOctets[offset];

                if (octet == 'Z' || octet == '-' || octet == '+')
                {
                    state = SuffixState;
                }
                else if (octet == '.' || octet == ',')
                {
                    state = FracState;
                }
                else if (contentOctets.Length - 1 <= offset)
                {
                    throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
                }
                else
                {
                    second = 10 * GetDigit(contentOctets[offset++]) + GetDigit(contentOctets[offset++]);
                }
            }

            if (state == HmsState && contentOctets.Length > offset)
            {
                byte octet = contentOctets[offset];

                if (octet == 'Z' || octet == '-' || octet == '+')
                {
                    state = SuffixState;
                }
                else if (octet == '.' || octet == ',')
                {
                    state = FracState;
                }
                else
                {
                    throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
                }
            }

            if (state == FracState)
            {
                if (disallowFractions)
                {
                    throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
                }

                Debug.Assert(contentOctets.Length > offset);
                byte octet = contentOctets[offset++];

                if (octet == '.')
                {
                    // Always valid
                }
                else if (octet == ',')
                {
                    // Valid for BER, but not CER or DER.
                    if (strict)
                    {
                        throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
                    }
                }
                else
                {
                    Debug.Fail($"Unhandled value '{octet:X2}' in {nameof(FracState)}");
                    throw new CryptographicException();
                }

                // There are 36,000,000,000 ticks per hour, and hour is our largest scale.
                // In case the double -> Ticks conversion allows for rounding up we can allow
                // for a 12th digit.
                const ulong MaxScale = 1_000_000_000_000;

                if (contentOctets.Length <= offset)
                {
                    throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
                }

                for (; offset < contentOctets.Length; offset++)
                {
                    octet = contentOctets[offset];

                    if (octet == 'Z' || octet == '-' || octet == '+')
                    {
                        state = SuffixState;
                        break;
                    }

                    if (fractionScale < MaxScale)
                    {
                        fraction *= 10;
                        fraction += GetDigit(contentOctets[offset]);
                        fractionScale *= 10;
                    }
                    else
                    {
                        GetDigit(contentOctets[offset]);
                    }
                }
            }

            if (state == SuffixState)
            {
                Debug.Assert(contentOctets.Length > offset);
                byte octet = contentOctets[offset++];

                if (octet == 'Z')
                {
                    timeOffset = TimeSpan.Zero;
                    isZulu = true;
                }
                else
                {
                    bool isMinus;

                    if (octet == '+')
                    {
                        isMinus = false;
                    }
                    else if (octet == '-')
                    {
                        isMinus = true;
                    }
                    else
                    {
                        Debug.Fail($"Unhandled value '{octet:X2}' in {nameof(SuffixState)}");
                        throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
                    }

                    if (contentOctets.Length - 1 <= offset)
                    {
                        throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
                    }

                    int offsetHour = 10 * GetDigit(contentOctets[offset++]) + GetDigit(contentOctets[offset++]);
                    int offsetMinute = 0;

                    if (contentOctets.Length > offset)
                    {
                        if (contentOctets[offset] == ':')
                        {
                            offset++;
                        }
                    }

                    if (contentOctets.Length - 1 > offset)
                    {
                        offsetMinute = 10 * GetDigit(contentOctets[offset++]) + GetDigit(contentOctets[offset++]);
                    }

                    TimeSpan tmp = new TimeSpan(offsetHour, offsetMinute, 0);

                    if (isMinus)
                    {
                        tmp = TimeSpan.Zero - tmp;
                    }

                    timeOffset = tmp;
                }
            }

            // Was there data after a suffix?
            if (offset != contentOctets.Length)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
            }

            // T-REC-X.690-201510 sec 11.7
            if (strict)
            {
                if (!isZulu || !second.HasValue)
                {
                    throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
                }

                if (fraction != 0 && fraction % 10 == 0)
                {
                    throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
                }
            }

            double frac = (double)fraction / fractionScale;
            TimeSpan fractionSpan = TimeSpan.Zero;

            if (!minute.HasValue)
            {
                minute = 0;
                second = 0;

                if (fraction != 0)
                {
                    // No minutes means this is fractions of an hour
                    fractionSpan = TimeSpan.FromHours(frac);
                }
            }
            else if (!second.HasValue)
            {
                second = 0;

                if (fraction != 0)
                {
                    // No seconds means this is fractions of a minute
                    fractionSpan = TimeSpan.FromMinutes(frac);
                }
            }
            else if (fraction != 0)
            {
                // Both minutes and seconds means fractions of a second.
                fractionSpan = TimeSpan.FromSeconds(frac);
            }
            
            DateTimeOffset value;

            if (timeOffset == null)
            {
                value = new DateTimeOffset(new DateTime(year, month, day, hour, minute.Value, second.Value));
            }
            else
            {
                value = new DateTimeOffset(year, month, day, hour, minute.Value, second.Value, timeOffset.Value);
            }

            value += fractionSpan;
            return value;
        }

        public DateTimeOffset GetGeneralizedTime(bool disallowFractions=false) =>
            GetGeneralizedTime(Asn1Tag.GeneralizedTime, disallowFractions);

        public DateTimeOffset GetGeneralizedTime(Asn1Tag expectedTag, bool disallowFractions=false)
        {
            if (TryGetOctetStringBytes(
                expectedTag,
                out ReadOnlyMemory<byte> primitiveOctets,
                out int headerLength,
                UniversalTagNumber.GeneralizedTime))
            {
                DateTimeOffset value = ParseGeneralizedTime(_ruleSet, primitiveOctets.Span, disallowFractions);
                _data = _data.Slice(headerLength + primitiveOctets.Length);
                return value;
            }

            // T-REC-X.690-201510 sec 9.2
            // T-REC-X.690-201510 sec 10.2
            if (_ruleSet == AsnEncodingRules.DER || _ruleSet == AsnEncodingRules.CER)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
            }

            int upperBound = PeekContentBytes().Length;

            byte[] rented = ArrayPool<byte>.Shared.Rent(upperBound);
            ReadOnlySpan<byte> contentOctets = ReadOnlySpan<byte>.Empty;

            try
            {
                if (TryCopyCharacterStringBytes(
                    expectedTag,
                    UniversalTagNumber.GeneralizedTime,
                    rented,
                    true,
                    out int bytesRead,
                    out int contentLength))
                {
                    contentOctets = Slice(rented, 0, contentLength);

                    DateTimeOffset value = ParseGeneralizedTime(_ruleSet, contentOctets, disallowFractions);
                    // Includes the header
                    _data = _data.Slice(bytesRead);
                    return value;
                }
            }
            finally
            {
                Array.Clear(rented, 0, contentOctets.Length);
                ArrayPool<byte>.Shared.Return(rented);
            }

            throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
        }

        private static ReadOnlySpan<byte> Slice(ReadOnlySpan<byte> source, int offset, int length)
        {
            Debug.Assert(offset >= 0);

            if (length < 0 || source.Length - offset < length)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
            }

            return source.Slice(offset, length);
        }

        private static ReadOnlyMemory<byte> Slice(ReadOnlyMemory<byte> source, int offset, int? length)
        {
            Debug.Assert(offset >= 0);

            if (length == null)
            {
                return source.Slice(offset);
            }

            int lengthVal = length.Value;

            if (lengthVal < 0 || source.Length - offset < lengthVal)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
            }

            return source.Slice(offset, lengthVal);
        }

        private static void CheckEncodingRules(AsnEncodingRules ruleSet)
        {
            if (ruleSet != AsnEncodingRules.BER &&
                ruleSet != AsnEncodingRules.CER &&
                ruleSet != AsnEncodingRules.DER)
            {
                throw new ArgumentOutOfRangeException(nameof(ruleSet));
            }
        }

        private static void CheckExpectedTag(Asn1Tag tag, Asn1Tag expectedTag, UniversalTagNumber tagNumber)
        {
            if (expectedTag.TagClass == TagClass.Universal && expectedTag.TagValue != (int)tagNumber)
            {
                throw new ArgumentException(
                    SR.Cryptography_Asn_UniversalValueIsFixed,
                    nameof(expectedTag));
            }

            if (expectedTag.TagClass != tag.TagClass || expectedTag.TagValue != tag.TagValue)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
            }
        }

        private static void CheckCharacterStringEncodingType(UniversalTagNumber encodingType)
        {
            // T-REC-X.680-201508 sec 41
            switch (encodingType)
            {
                case UniversalTagNumber.BMPString:
                case UniversalTagNumber.GeneralString:
                case UniversalTagNumber.GraphicString:
                case UniversalTagNumber.IA5String:
                case UniversalTagNumber.ISO646String:
                case UniversalTagNumber.NumericString:
                case UniversalTagNumber.PrintableString:
                case UniversalTagNumber.TeletexString:
                // T61String is an alias for TeletexString (already listed)
                case UniversalTagNumber.UniversalString:
                case UniversalTagNumber.UTF8String:
                case UniversalTagNumber.VideotexString:
                // VisibleString is an alias for ISO646String (already listed)
                    return;
            }

            throw new ArgumentOutOfRangeException(nameof(encodingType));
        }
    }

    internal static class AsnCharacterStringEncodings
    {
        private static readonly Text.Encoding s_utf8Encoding = new UTF8Encoding(false, throwOnInvalidBytes: true);
        private static readonly Text.Encoding s_bmpEncoding = new BMPEncoding();
        private static readonly Text.Encoding s_ia5Encoding = new IA5Encoding();
        private static readonly Text.Encoding s_visibleStringEncoding = new VisibleStringEncoding();
        private static readonly Text.Encoding s_printableStringEncoding = new PrintableStringEncoding();

        internal static Text.Encoding GetEncoding(UniversalTagNumber encodingType)
        {
            switch (encodingType)
            {
                case UniversalTagNumber.UTF8String:
                    return s_utf8Encoding;
                case UniversalTagNumber.PrintableString:
                    return s_printableStringEncoding;
                case UniversalTagNumber.IA5String:
                    return s_ia5Encoding;
                case UniversalTagNumber.VisibleString:
                    return s_visibleStringEncoding;
                case UniversalTagNumber.BMPString:
                    return s_bmpEncoding;
                default:
                    throw new ArgumentOutOfRangeException(nameof(encodingType), encodingType, null);
            }
        }
    }

    internal abstract class SpanBasedEncoding : Text.Encoding
    {
        protected SpanBasedEncoding()
            : base(0, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback)
        {
        }

        protected abstract int GetBytes(ReadOnlySpan<char> chars, Span<byte> bytes, bool write);
        protected abstract int GetChars(ReadOnlySpan<byte> bytes, Span<char> chars, bool write);

        public override int GetByteCount(char[] chars, int index, int count)
        {
            return GetByteCount(new ReadOnlySpan<char>(chars, index, count));
        }

        public override unsafe int GetByteCount(char* chars, int count)
        {
            return GetByteCount(new ReadOnlySpan<char>(chars, count));
        }

        public override int GetByteCount(string s)
        {
            return GetByteCount(s.AsReadOnlySpan());
        }

        private int GetByteCount(ReadOnlySpan<char> chars)
        {
            return GetBytes(chars, Span<byte>.Empty, write: false);
        }

        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            return GetBytes(
                new ReadOnlySpan<char>(chars, charIndex, charCount),
                new Span<byte>(bytes, byteIndex, bytes.Length - byteIndex),
                write: true);
        }

        public override unsafe int GetBytes(char* chars, int charCount, byte* bytes, int byteCount)
        {
            return GetBytes(
                new ReadOnlySpan<char>(chars, charCount),
                new Span<byte>(bytes, byteCount),
                write: true);
        }

        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            return GetCharCount(new ReadOnlySpan<byte>(bytes, index, count));
        }

        public override unsafe int GetCharCount(byte* bytes, int count)
        {
            return GetCharCount(new ReadOnlySpan<byte>(bytes, count));
        }

        private int GetCharCount(ReadOnlySpan<byte> bytes)
        {
            return GetChars(bytes, Span<char>.Empty, write: false);
        }

        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            return GetChars(
                new ReadOnlySpan<byte>(bytes, byteIndex, byteCount),
                new Span<char>(chars, charIndex, chars.Length - charIndex),
                write: true);
        }

        public override unsafe int GetChars(byte* bytes, int byteCount, char* chars, int charCount)
        {
            return GetChars(
                new ReadOnlySpan<byte>(bytes, byteCount),
                new Span<char>(chars, charCount),
                write: true);
        }
    }

    internal class IA5Encoding : RestrictedAsciiStringEncoding
    {
        // T-REC-X.680-201508 sec 41, Table 8.
        // ISO International Register of Coded Character Sets to be used with Escape Sequences 001
        //   is ASCII 0x00 - 0x1F
        // ISO International Register of Coded Character Sets to be used with Escape Sequences 006
        //   is ASCII 0x21 - 0x7E
        // Space is ASCII 0x20, delete is ASCII 0x7F.
        //
        // The net result is all of 7-bit ASCII
        internal IA5Encoding()
            : base(0x00, 0x7F)
        {
        }
    }

    internal class VisibleStringEncoding : RestrictedAsciiStringEncoding
    {
        // T-REC-X.680-201508 sec 41, Table 8.
        // ISO International Register of Coded Character Sets to be used with Escape Sequences 006
        //   is ASCII 0x21 - 0x7E
        // Space is ASCII 0x20.
        internal VisibleStringEncoding()
            : base(0x20, 0x7E)
        {
        }
    }

    internal class PrintableStringEncoding : RestrictedAsciiStringEncoding
    {
        // T-REC-X.680-201508 sec 41.4
        internal PrintableStringEncoding()
            : base("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 '()+,-./:=?")
        {
        }
    }

    internal abstract class RestrictedAsciiStringEncoding : SpanBasedEncoding
    {
        private readonly bool[] _isAllowed;

        protected RestrictedAsciiStringEncoding(byte minCharAllowed, byte maxCharAllowed)
        {
            Debug.Assert(minCharAllowed <= maxCharAllowed);
            Debug.Assert(maxCharAllowed <= 0x7F);

            bool[] isAllowed = new bool[0x80];

            for (byte charCode = minCharAllowed; charCode <= maxCharAllowed; charCode++)
            {
                isAllowed[charCode] = true;
            }

            _isAllowed = isAllowed;
        }

        protected RestrictedAsciiStringEncoding(IEnumerable<char> allowedChars)
        {
            bool[] isAllowed = new bool[0x7F];

            foreach (char c in allowedChars)
            {
                if (c >= isAllowed.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(allowedChars));
                }

                Debug.Assert(isAllowed[c] == false);
                isAllowed[c] = true;
            }

            _isAllowed = isAllowed;
        }

        public override int GetMaxByteCount(int charCount)
        {
            return charCount;
        }

        public override int GetMaxCharCount(int byteCount)
        {
            return byteCount;
        }

        protected override int GetBytes(ReadOnlySpan<char> chars, Span<byte> bytes, bool write)
        {
            if (chars.IsEmpty)
            {
                return 0;
            }

            for (int i = 0; i < chars.Length; i++)
            {
                char c = chars[i];

                if ((uint)c > (uint)_isAllowed.Length || !_isAllowed[c])
                {
                    EncoderFallback.CreateFallbackBuffer().Fallback(c, i);

                    Debug.Fail("Fallback should have thrown");
                    throw new CryptographicException();
                }

                if (write)
                {
                    bytes[i] = (byte)c;
                }
            }

            return chars.Length;
        }

        protected override int GetChars(ReadOnlySpan<byte> bytes, Span<char> chars, bool write)
        {
            if (bytes.IsEmpty)
            {
                return 0;
            }

            for (int i = 0; i < bytes.Length; i++)
            {
                byte b = bytes[i];

                if ((uint)b > (uint)_isAllowed.Length || !_isAllowed[b])
                {
                    DecoderFallback.CreateFallbackBuffer().Fallback(
                        new[] { b },
                        i);

                    Debug.Fail("Fallback should have thrown");
                    throw new CryptographicException();
                }

                if (write)
                {
                    chars[i] = (char)b;
                }
            }

            return bytes.Length;
        }
    }

    /// <summary>
    /// Big-Endian UCS-2 encoding (the same as UTF-16BE, but disallowing surrogate pairs to leave plane 0)
    /// </summary>
    internal class BMPEncoding : SpanBasedEncoding
    {
        protected override int GetBytes(ReadOnlySpan<char> chars, Span<byte> bytes, bool write)
        {
            if (chars.IsEmpty)
            {
                return 0;
            }

            int writeIdx = 0;

            for (int i = 0; i < chars.Length; i++)
            {
                char c = chars[i];

                if (char.IsSurrogate(c))
                {
                    EncoderFallback.CreateFallbackBuffer().Fallback(c, i);

                    Debug.Fail("Fallback should have thrown");
                    throw new CryptographicException();
                }

                ushort val16 = c;

                if (write)
                {
                    bytes[writeIdx + 1] = (byte)val16;
                    bytes[writeIdx] = (byte)(val16 >> 8);
                }

                writeIdx += 2;
            }

            return writeIdx;
        }

        protected override int GetChars(ReadOnlySpan<byte> bytes, Span<char> chars, bool write)
        {
            if (bytes.IsEmpty)
            {
                return 0;
            }

            if (bytes.Length % 2 != 0)
            {
                DecoderFallback.CreateFallbackBuffer().Fallback(
                    bytes.Slice(bytes.Length - 1).ToArray(),
                    bytes.Length - 1);

                Debug.Fail("Fallback should have thrown");
                throw new CryptographicException();
            }

            int writeIdx = 0;

            for (int i = 0; i < bytes.Length; i += 2)
            {
                int val = bytes[i] << 8 | bytes[i + 1];
                char c = (char)val;

                if (char.IsSurrogate(c))
                {
                    DecoderFallback.CreateFallbackBuffer().Fallback(
                        bytes.Slice(i, 2).ToArray(),
                        i);

                    Debug.Fail("Fallback should have thrown");
                    throw new CryptographicException();
                }

                if (write)
                {
                    chars[writeIdx] = c;
                }

                writeIdx++;
            }

            return writeIdx;
        }

        public override int GetMaxByteCount(int charCount)
        {
            checked
            {
                return charCount * 2;
            }
        }

        public override int GetMaxCharCount(int byteCount)
        {
            return byteCount / 2;
        }
    }

    internal class AsnWriter
    {
        private byte[] _buffer;
        private int _offset;
        private Stack<(Asn1Tag,int)> _nestingStack;

        public AsnEncodingRules RuleSet { get; }

        public AsnWriter(AsnEncodingRules ruleSet)
        {
            if (ruleSet != AsnEncodingRules.BER &&
                ruleSet != AsnEncodingRules.CER &&
                ruleSet != AsnEncodingRules.DER)
            {
                throw new ArgumentOutOfRangeException(nameof(ruleSet));
            }

            RuleSet = ruleSet;
        }

        private void EnsureWriteCapacity(int pendingCount)
        {
            if (pendingCount < 0)
            {
                throw new OverflowException();
            }

            if (_buffer == null || _buffer.Length - _offset < pendingCount)
            {
#if CHECK_ACCURATE_ENSURE
                // A debug paradigm to make sure that throughout the execution nothing ever writes
                // past where the buffer was "allocated".  This causes quite a number of reallocs
                // and copies, so it's a #define opt-in.
                byte[] newBytes = new byte[_offset + pendingCount];
                
                if (_buffer != null)
                {
                    Buffer.BlockCopy(_buffer, 0, newBytes, 0, _offset);
                }
#else
                const int BlockSize = 1024;
                // While the ArrayPool may have similar logic, make sure we don't run into a lot of
                // "grow a little" by asking in 1k steps.
                int inflatedBytes = _offset + pendingCount + (BlockSize - 1);

                if (inflatedBytes < 0)
                {
                    throw new OverflowException();
                }

                int blocks = inflatedBytes / BlockSize;
                byte[] newBytes = ArrayPool<byte>.Shared.Rent(BlockSize * blocks);

                if (_buffer != null)
                {
                    Buffer.BlockCopy(_buffer, 0, newBytes, 0, _offset);
                    Array.Clear(_buffer, 0, _offset);
                    ArrayPool<byte>.Shared.Return(_buffer);
                }
#endif

#if DEBUG
                // Ensure no "implicit 0" is happening
                for (int i = _offset; i < newBytes.Length; i++)
                {
                    newBytes[i] ^= 0xFF;
                }
#endif

                _buffer = newBytes;
            }
        }

        private void WriteTag(Asn1Tag tag)
        {
            int spaceRequired = tag.CalculateEncodedSize();
            EnsureWriteCapacity(spaceRequired);

            if (!tag.TryWrite(_buffer.AsSpan().Slice(_offset, spaceRequired), out int written) ||
                written != spaceRequired)
            {
                Debug.Fail($"TryWrite failed or written was wrong value ({written} vs {spaceRequired})");
                throw new CryptographicException();
            }

            _offset += spaceRequired;
        }

        // T-REC-X.690-201508 sec 8.1.3
        private void WriteLength(int length)
        {
            const byte MultiByteMarker = 0x80;
            Debug.Assert(length >= -1);

            // If the indefinite form has been requested.
            // T-REC-X.690-201508 sec 8.1.3.6
            if (length == -1)
            {
                EnsureWriteCapacity(1);
                _buffer[_offset] = MultiByteMarker;
                _offset++;
                return;
            }

            Debug.Assert(length >= 0);

            // T-REC-X.690-201508 sec 8.1.3.3, 8.1.3.4
            if (length < MultiByteMarker)
            {
                // Pre-allocate the pending data since we know how much.
                EnsureWriteCapacity(1 + length);
                _buffer[_offset] = (byte)length;
                _offset++;
                return;
            }

            // The rest of the method implements T-REC-X.680-201508 sec 8.1.3.5
            int lengthLength = GetEncodedLengthSubsequentByteCount(length);

            // Pre-allocate the pending data since we know how much.
            EnsureWriteCapacity(lengthLength + 1 + length);
            _buffer[_offset] = (byte)(MultiByteMarker | lengthLength);

            // No minus one because offset didn't get incremented yet.
            int idx = _offset + lengthLength;

            int remaining = length;

            do
            {
                _buffer[idx] = (byte)remaining;
                remaining >>= 8;
                idx--;
            } while (remaining > 0);

            Debug.Assert(idx == _offset);
            _offset += lengthLength + 1;
        }

        // T-REC-X.690-201508 sec 8.1.3.5
        private static int GetEncodedLengthSubsequentByteCount(int length)
        {
            if (length <= 0x7F)
                return 0;
            if (length <= byte.MaxValue)
                return 1;
            if (length <= ushort.MaxValue)
                return 2;
            if (length <= 0x00FFFFFF)
                return 3;

            return 4;
        }

        public void WriteEncodedValue(ReadOnlyMemory<byte> preEncodedValue)
        {
            AsnReader reader = new AsnReader(preEncodedValue, RuleSet);

            // Is it legal under the current rules?
            ReadOnlyMemory<byte> parsedBack = reader.GetEncodedValue();

            if (reader.HasData)
            {
                throw new ArgumentException(SR.Cryptography_WriteEncodedValue_OneValueAtATime, nameof(preEncodedValue));
            }

            Debug.Assert(parsedBack.Length == preEncodedValue.Length);

            EnsureWriteCapacity(preEncodedValue.Length);
            preEncodedValue.Span.CopyTo(_buffer.AsSpan().Slice(_offset));
            _offset += preEncodedValue.Length;
        }

        // T-REC-X.690-201508 sec 8.1.5
        private void WriteEndOfContents()
        {
            EnsureWriteCapacity(2);
            _buffer[_offset++] = 0;
            _buffer[_offset++] = 0;
        }

        public void WriteBoolean(bool value)
        {
            WriteBooleanCore(Asn1Tag.Boolean, value);
        }

        public void WriteBoolean(Asn1Tag tag, bool value)
        {
            CheckUniversalTag(tag, UniversalTagNumber.Boolean);

            WriteBooleanCore(tag.AsPrimitive(), value);
        }

        // T-REC-X.690-201508 sec 11.1, 8.2
        private void WriteBooleanCore(Asn1Tag tag, bool value)
        { 
            Debug.Assert(!tag.IsConstructed);
            WriteTag(tag);
            WriteLength(1);
            // Ensured by WriteLength
            Debug.Assert(_offset < _buffer.Length);
            _buffer[_offset] = (byte)(value ? 0xFF : 0x00);
            _offset++;
        }

        public void WriteInteger(long value)
        {
            WriteIntegerCore(Asn1Tag.Integer, value);
        }

        public void WriteInteger(ulong value)
        {
            WriteNonNegativeIntegerCore(Asn1Tag.Integer, value);
        }

        public void WriteInteger(BigInteger value)
        {
            WriteIntegerCore(Asn1Tag.Integer, value);
        }

        public void WriteInteger(Asn1Tag tag, long value)
        {
            CheckUniversalTag(tag, UniversalTagNumber.Integer);

            WriteIntegerCore(tag.AsPrimitive(), value);
        }

        // T-REC-X.690-201508 sec 8.3
        private void WriteIntegerCore(Asn1Tag tag, long value)
        {
            if (value >= 0)
            {
                WriteNonNegativeIntegerCore(tag, (ulong)value);
                return;
            }

            int valueLength;

            if (value >= sbyte.MinValue)
                valueLength = 1;
            else if (value >= short.MinValue)
                valueLength = 2;
            else if (value >= unchecked((long)0xFFFFFFFF_FF800000))
                valueLength = 3;
            else if (value >= int.MinValue)
                valueLength = 4;
            else if (value >= unchecked((long)0xFFFFFF80_00000000))
                valueLength = 5;
            else if (value >= unchecked((long)0xFFFF8000_00000000))
                valueLength = 6;
            else if (value >= unchecked((long)0xFF800000_00000000))
                valueLength = 7;
            else
                valueLength = 8;

            Debug.Assert(!tag.IsConstructed);
            WriteTag(tag);
            WriteLength(valueLength);

            long remaining = value;
            int idx = _offset + valueLength - 1;

            do
            {
                _buffer[idx] = (byte)remaining;
                remaining >>= 8;
                idx--;
            } while (idx >= _offset);

#if DEBUG
            if (valueLength > 1)
            {
                // T-REC-X.690-201508 sec 8.3.2
                // Cannot start with 9 bits of 1 (or 9 bits of 0, but that's not this method).
                Debug.Assert(_buffer[_offset] != 0xFF || _buffer[_offset + 1] < 0x80);
            }
#endif

            _offset += valueLength;
        }

        public void WriteInteger(Asn1Tag tag, ulong value)
        {
            CheckUniversalTag(tag, UniversalTagNumber.Integer);

            WriteNonNegativeIntegerCore(tag.AsPrimitive(), value);
        }

        // T-REC-X.690-201508 sec 8.3
        private void WriteNonNegativeIntegerCore(Asn1Tag tag, ulong value)
        {
            int valueLength;
            
            // 0x80 needs two bytes: 0x00 0x80
            if (value < 0x80)
                valueLength = 1;
            else if (value < 0x8000)
                valueLength = 2;
            else if (value < 0x800000)
                valueLength = 3;
            else if (value < 0x80000000)
                valueLength = 4;
            else if (value < 0x80_00000000)
                valueLength = 5;
            else if (value < 0x8000_00000000)
                valueLength = 6;
            else if (value < 0x800000_00000000)
                valueLength = 7;
            else if (value < 0x80000000_00000000)
                valueLength = 8;
            else
                valueLength = 9;

            // Clear the constructed bit, if it was set.
            Debug.Assert(!tag.IsConstructed);
            WriteTag(tag);
            WriteLength(valueLength);

            ulong remaining = value;
            int idx = _offset + valueLength - 1;

            do
            {
                _buffer[idx] = (byte)remaining;
                remaining >>= 8;
                idx--;
            } while (idx >= _offset);

#if DEBUG
            if (valueLength > 1)
            {
                // T-REC-X.690-201508 sec 8.3.2
                // Cannot start with 9 bits of 0 (or 9 bits of 1, but that's not this method).
                Debug.Assert(_buffer[_offset] != 0 || _buffer[_offset + 1] > 0x7F);
            }
#endif

            _offset += valueLength;
        }

        public void WriteInteger(Asn1Tag tag, BigInteger value)
        {
            CheckUniversalTag(tag, UniversalTagNumber.Integer);

            WriteIntegerCore(tag.AsPrimitive(), value);
        }

        // T-REC-X.690-201508 sec 8.3
        private void WriteIntegerCore(Asn1Tag tag, BigInteger value)
        {
            // TODO: Split this for netstandard vs netcoreapp for span-perf?.
            byte[] encoded = value.ToByteArray();
            Array.Reverse(encoded);

            Debug.Assert(!tag.IsConstructed);
            WriteTag(tag);
            WriteLength(encoded.Length);
            Buffer.BlockCopy(encoded, 0, _buffer, _offset, encoded.Length);
            _offset += encoded.Length;
        }

        public void WriteBitString(ReadOnlySpan<byte> bitString, int unusedBitCount=0)
        {
            WriteBitStringCore(Asn1Tag.PrimitiveBitString, bitString, unusedBitCount);
        }

        public void WriteBitString(Asn1Tag tag, ReadOnlySpan<byte> bitString, int unusedBitCount=0)
        {
            CheckUniversalTag(tag, UniversalTagNumber.BitString);

            // Primitive or constructed, doesn't matter.
            WriteBitStringCore(tag, bitString, unusedBitCount);
        }

        // T-REC-X.690-201508 sec 8.6
        private void WriteBitStringCore(Asn1Tag tag, ReadOnlySpan<byte> bitString, int unusedBitCount)
        {
            // T-REC-X.690-201508 sec 8.6.2.2
            if (unusedBitCount < 0 || unusedBitCount > 7)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(unusedBitCount),
                    unusedBitCount,
                    SR.Cryptography_Asn_UnusedBitCountRange);
            }

            // T-REC-X.690-201508 sec 8.6.2.3
            if (bitString.Length == 0 && unusedBitCount != 0)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
            }

            // If 3 bits are "unused" then build a mask for them to check for 0.
            // 1 << 3 => 0b0000_1000
            // subtract 1 => 0b000_0111
            int mask = (1 << unusedBitCount) - 1;
            byte lastByte = bitString.IsEmpty ? (byte)0 : bitString[bitString.Length - 1];

            if ((lastByte & mask) != 0)
            {
                // T-REC-X.690-201508 sec 11.2
                //
                // This could be ignored for BER, but since DER is more common and
                // it likely suggests a program error on the caller, leave it enabled for
                // BER for now.
                // TODO: Probably warrants a distinct message.
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
            }

            if (RuleSet == AsnEncodingRules.CER)
            {
                // T-REC-X.690-201508 sec 9.2
                //
                // If it's not within a primitive segment, use the constructed encoding.
                // (>= instead of > because of the unused bit count byte)
                if (bitString.Length >= AsnReader.MaxCERSegmentSize)
                {
                    WriteConstructedCerBitString(tag, bitString, unusedBitCount);
                    return;
                }
            }

            // Clear the constructed flag, if present.
            WriteTag(tag.AsPrimitive());
            // The unused bits byte requires +1.
            WriteLength(bitString.Length + 1);
            _buffer[_offset] = (byte)unusedBitCount;
            _offset++;
            bitString.CopyTo(_buffer.AsSpan().Slice(_offset));
            _offset += bitString.Length;
        }

        // T-REC-X.690-201508 sec 9.2, 8.6
        private void WriteConstructedCerBitString(Asn1Tag tag, ReadOnlySpan<byte> payload, int unusedBitCount)
        {
            const int MaxCERSegmentSize = AsnReader.MaxCERSegmentSize;
            // Every segment has an "unused bit count" byte.
            const int MaxCERContentSize = MaxCERSegmentSize - 1;
            Debug.Assert(payload.Length > MaxCERContentSize);

            WriteTag(tag.AsConstructed());
            // T-REC-X.690-201508 sec 9.1
            // Constructed CER uses the indefinite form.
            WriteLength(-1);

            int fullSegments = Math.DivRem(payload.Length, MaxCERContentSize, out int lastContentSize);

            // The tag size is 1 byte.
            // The length will always be encoded as 82 03 E8 (3 bytes)
            // And 1000 content octets (by T-REC-X.690-201508 sec 9.2)
            const int FullSegmentEncodedSize = 1004;
            Debug.Assert(
                FullSegmentEncodedSize == 1 + 1 + MaxCERSegmentSize + GetEncodedLengthSubsequentByteCount(MaxCERSegmentSize));

            int remainingEncodedSize;

            if (lastContentSize == 0)
            {
                remainingEncodedSize = 0;
            }
            else
            {
                // One byte of tag, minimum one byte of length, and one byte of unused bit count.
                remainingEncodedSize = 3 + lastContentSize + GetEncodedLengthSubsequentByteCount(lastContentSize);
            }

            // Reduce the number of copies by pre-calculating the size.
            // +2 for End-Of-Contents
            int expectedSize = fullSegments * FullSegmentEncodedSize + remainingEncodedSize + 2;
            EnsureWriteCapacity(expectedSize);

            byte[] ensureNoExtraCopy = _buffer;
            int savedOffset = _offset;

            ReadOnlySpan<byte> remainingData = payload;
            Span<byte> dest;
            Asn1Tag primitiveBitString = Asn1Tag.PrimitiveBitString;

            while (remainingData.Length > MaxCERContentSize)
            {
                // T-REC-X.690-201508 sec 8.6.4.1
                WriteTag(primitiveBitString);
                WriteLength(MaxCERSegmentSize);
                // 0 unused bits in this segment.
                _buffer[_offset] = 0;
                _offset++;

                dest = _buffer.AsSpan().Slice(_offset);
                remainingData.Slice(0, MaxCERContentSize).CopyTo(dest);

                remainingData = remainingData.Slice(MaxCERContentSize);
                _offset += MaxCERContentSize;
            }

            WriteTag(primitiveBitString);
            WriteLength(remainingData.Length + 1);

            _buffer[_offset] = (byte)unusedBitCount;
            _offset++;

            dest = _buffer.AsSpan().Slice(_offset);
            remainingData.CopyTo(dest);
            _offset += remainingData.Length;

            WriteEndOfContents();

            Debug.Assert(_offset - savedOffset == expectedSize, $"expected size was {expectedSize}, actual was {_offset - savedOffset}");
            Debug.Assert(_buffer == ensureNoExtraCopy, $"_buffer was replaced during {nameof(WriteConstructedCerBitString)}");
        }

        public void WriteNamedBitList(object enumValue)
        {
            if (enumValue == null)
                throw new ArgumentNullException(nameof(enumValue));

            WriteNamedBitList(Asn1Tag.PrimitiveBitString, enumValue);
        }

        public void WriteNamedBitList<TEnum>(TEnum enumValue) where TEnum : struct
        {
            WriteNamedBitList(Asn1Tag.PrimitiveBitString, enumValue);
        }

        public void WriteNamedBitList(Asn1Tag tag, object enumValue)
        {
            if (enumValue == null)
                throw new ArgumentNullException(nameof(enumValue));

            WriteNamedBitList(tag, enumValue.GetType(), enumValue);
        }

        public void WriteNamedBitList<TEnum>(Asn1Tag tag, TEnum enumValue) where TEnum : struct 
        {
            WriteNamedBitList(tag, typeof(TEnum), enumValue);
        }

        private void WriteNamedBitList(Asn1Tag tag, Type tEnum, object enumValue)
        {
            Type backingType = tEnum.GetEnumUnderlyingType();

            if (!tEnum.IsDefined(typeof(FlagsAttribute), false))
            {
                throw new ArgumentException(SR.Cryptography_Asn_NamedBitListRequiresFlagsEnum, nameof(tEnum));
            }

            ulong integralValue;

            if (backingType == typeof(ulong))
            {
                integralValue = Convert.ToUInt64(enumValue);
            }
            else
            {
                // All other types fit in a (signed) long.
                long numericValue = Convert.ToInt64(enumValue);
                integralValue = unchecked((ulong)numericValue);
            }

            WriteNamedBitList(tag, integralValue);
        }

        // T-REC-X.680-201508 sec 22
        // T-REC-X.690-201508 sec 8.6, 11.2.2
        private void WriteNamedBitList(Asn1Tag tag, ulong integralValue)
        {
            Span<byte> temp = stackalloc byte[sizeof(ulong)];
            // Reset to all zeros, since we're just going to or-in bits we need.
            temp.Clear();

            int indexOfHighestSetBit = -1;

            for (int i = 0; integralValue != 0; integralValue >>= 1, i++)
            {
                if ((integralValue & 1) != 0)
                {
                    temp[i / 8] |= (byte)(0x80 >> (i % 8));
                    indexOfHighestSetBit = i;
                }
            }

            if (indexOfHighestSetBit < 0)
            {
                // No bits were set; this is an empty bit string.
                // T-REC-X.690-201508 sec 11.2.2-note2
                WriteBitString(tag, ReadOnlySpan<byte>.Empty);
            }
            else
            {
                // At least one bit was set.
                // Determine the shortest length necessary to represent the bit string.

                // Since "bit 0" gets written down 0 => 1.
                // Since "bit 8" is in the second byte 8 => 2.
                // That makes the formula ((bit / 8) + 1) instead of ((bit + 7) / 8).
                int byteLen = (indexOfHighestSetBit / 8) + 1;
                int unusedBitCount = 7 - (indexOfHighestSetBit % 8);

                WriteBitString(
                    tag,
                    temp.Slice(0, byteLen),
                    unusedBitCount);
            }
        }

        public void WriteOctetString(ReadOnlySpan<byte> octetString)
        {
            WriteOctetString(Asn1Tag.PrimitiveOctetString, octetString);
        }

        public void WriteOctetString(Asn1Tag tag, ReadOnlySpan<byte> octetString)
        {
            CheckUniversalTag(tag, UniversalTagNumber.OctetString);

            // Primitive or constructed, doesn't matter.
            WriteOctetStringCore(tag, octetString);
        }

        // T-REC-X.690-201508 sec 8.7
        private void WriteOctetStringCore(Asn1Tag tag, ReadOnlySpan<byte> octetString)
        {
            if (RuleSet == AsnEncodingRules.CER)
            {
                // If it's bigger than a primitive segment, use the constructed encoding
                // T-REC-X.690-201508 sec 9.2
                if (octetString.Length > AsnReader.MaxCERSegmentSize)
                {
                    WriteConstructedCerOctetString(tag, octetString);
                    return;
                }
            }

            // Clear the constructed flag, if present.
            WriteTag(tag.AsPrimitive());
            WriteLength(octetString.Length);
            octetString.CopyTo(_buffer.AsSpan().Slice(_offset));
            _offset += octetString.Length;
        }

        // T-REC-X.690-201508 sec 9.2, 8.7
        private void WriteConstructedCerOctetString(Asn1Tag tag, ReadOnlySpan<byte> payload)
        {
            const int MaxCERSegmentSize = AsnReader.MaxCERSegmentSize;
            Debug.Assert(payload.Length > MaxCERSegmentSize);

            WriteTag(tag.AsConstructed());
            WriteLength(-1);

            int fullSegments = Math.DivRem(payload.Length, MaxCERSegmentSize, out int lastSegmentSize);

            // The tag size is 1 byte.
            // The length will always be encoded as 82 03 E8 (3 bytes)
            // And 1000 content octets (by T-REC-X.690-201508 sec 9.2)
            const int FullSegmentEncodedSize = 1004;
            Debug.Assert(
                FullSegmentEncodedSize == 1 + 1 + MaxCERSegmentSize + GetEncodedLengthSubsequentByteCount(MaxCERSegmentSize));

            int remainingEncodedSize;

            if (lastSegmentSize == 0)
            {
                remainingEncodedSize = 0;
            }
            else
            {
                // One byte of tag, and minimum one byte of length.
                remainingEncodedSize = 2 + lastSegmentSize + GetEncodedLengthSubsequentByteCount(lastSegmentSize);
            }

            // Reduce the number of copies by pre-calculating the size.
            // +2 for End-Of-Contents
            int expectedSize = fullSegments * FullSegmentEncodedSize + remainingEncodedSize + 2;
            EnsureWriteCapacity(expectedSize);

            byte[] ensureNoExtraCopy = _buffer;
            int savedOffset = _offset;

            ReadOnlySpan<byte> remainingData = payload;
            Span<byte> dest;
            Asn1Tag primitiveOctetString = Asn1Tag.PrimitiveOctetString;

            while (remainingData.Length > MaxCERSegmentSize)
            {
                // T-REC-X.690-201508 sec 8.7.3.2-note2
                WriteTag(primitiveOctetString);
                WriteLength(MaxCERSegmentSize);

                dest = _buffer.AsSpan().Slice(_offset);
                remainingData.Slice(0, MaxCERSegmentSize).CopyTo(dest);

                _offset += MaxCERSegmentSize;
                remainingData = remainingData.Slice(MaxCERSegmentSize);
            }

            WriteTag(primitiveOctetString);
            WriteLength(remainingData.Length);
            dest = _buffer.AsSpan().Slice(_offset);
            remainingData.CopyTo(dest);
            _offset += remainingData.Length;

            WriteEndOfContents();

            Debug.Assert(_offset - savedOffset == expectedSize, $"expected size was {expectedSize}, actual was {_offset - savedOffset}");
            Debug.Assert(_buffer == ensureNoExtraCopy, $"_buffer was replaced during {nameof(WriteConstructedCerOctetString)}");
        }

        public void WriteNull()
        {
            WriteNullCore(Asn1Tag.Null);
        }

        public void WriteNull(Asn1Tag tag)
        {
            CheckUniversalTag(tag, UniversalTagNumber.Null);

            WriteNullCore(tag.AsPrimitive());
        }

        // T-REC-X.690-201508 sec 8.8
        private void WriteNullCore(Asn1Tag tag)
        {
            Debug.Assert(!tag.IsConstructed);
            WriteTag(tag);
            WriteLength(0);
        }

        public void WriteObjectIdentifier(Oid oid)
        {
            if (oid == null)
                throw new ArgumentNullException(nameof(oid));

            WriteObjectIdentifier(oid.Value);
        }

        public void WriteObjectIdentifier(string oidValue)
        {
            if (oidValue == null)
                throw new ArgumentNullException(nameof(oidValue));

            WriteObjectIdentifier(oidValue.AsReadOnlySpan());
        }

        public void WriteObjectIdentifier(ReadOnlySpan<char> oidValue)
        {
            WriteObjectIdentifierCore(Asn1Tag.ObjectIdentifier, oidValue);
        }

        public void WriteObjectIdentifier(Asn1Tag tag, Oid oid)
        {
            if (oid == null)
                throw new ArgumentNullException(nameof(oid));

            WriteObjectIdentifier(tag, oid.Value);
        }

        public void WriteObjectIdentifier(Asn1Tag tag, string oidValue)
        {
            if (oidValue == null)
                throw new ArgumentNullException(nameof(oidValue));

            WriteObjectIdentifier(tag, oidValue.AsReadOnlySpan());
        }

        public void WriteObjectIdentifier(Asn1Tag tag, ReadOnlySpan<char> oidValue)
        {
            CheckUniversalTag(tag, UniversalTagNumber.ObjectIdentifier);

            WriteObjectIdentifierCore(tag.AsPrimitive(), oidValue);
        }

        // T-REC-X.690-201508 sec 8.19
        private void WriteObjectIdentifierCore(Asn1Tag tag, ReadOnlySpan<char> oidValue)
        {
            // T-REC-X.690-201508 sec 8.19.4
            // The first character is in { 0, 1, 2 }, the second will be a '.', and a third (digit)
            // will also exist.
            if (oidValue.Length < 3)
                throw new CryptographicException(SR.Argument_InvalidOidValue);
            if (oidValue[1] != '.')
                throw new CryptographicException(SR.Argument_InvalidOidValue);

            // The worst case is "1.1.1.1.1", which takes 4 bytes (5 components, with the first two condensed)
            // Longer numbers get smaller: "2.1.127" is only 2 bytes. (81d (0x51) and 127 (0x7F))
            // So length / 2 should prevent any reallocations.
            byte[] tmp = ArrayPool<byte>.Shared.Rent(oidValue.Length / 2);
            int tmpOffset = 0;

            try
            {
                int firstComponent;

                switch (oidValue[0])
                {
                    case '0':
                        firstComponent = 0;
                        break;
                    case '1':
                        firstComponent = 1;
                        break;
                    case '2':
                        firstComponent = 2;
                        break;
                    default:
                        throw new CryptographicException(SR.Argument_InvalidOidValue);
                }

                // The first two components are special:
                // ITU X.690 8.19.4:
                //   The numerical value of the first subidentifier is derived from the values of the first two
                //   object identifier components in the object identifier value being encoded, using the formula:
                //       (X*40) + Y
                //   where X is the value of the first object identifier component and Y is the value of the
                //   second object identifier component.
                //       NOTE - This packing of the first two object identifier components recognizes that only
                //          three values are allocated from the root node, and at most 39 subsequent values from
                //          nodes reached by X = 0 and X = 1.

                // skip firstComponent and the trailing .
                ReadOnlySpan<char> remaining = oidValue.Slice(2);

                BigInteger subIdentifier = ParseSubIdentifier(ref remaining);
                subIdentifier += 40 * firstComponent;

                int localLen = EncodeSubIdentifier(tmp.AsSpan().Slice(tmpOffset), ref subIdentifier);
                tmpOffset += localLen;

                while (!remaining.IsEmpty)
                {
                    subIdentifier = ParseSubIdentifier(ref remaining);
                    localLen = EncodeSubIdentifier(tmp.AsSpan().Slice(tmpOffset), ref subIdentifier);
                    tmpOffset += localLen;
                }

                Debug.Assert(!tag.IsConstructed);
                WriteTag(tag);
                WriteLength(tmpOffset);
                Buffer.BlockCopy(tmp, 0, _buffer, _offset, tmpOffset);
                _offset += tmpOffset;
            }
            finally
            {
                Array.Clear(tmp, 0, tmpOffset);
                ArrayPool<byte>.Shared.Return(tmp);
            }
        }

        private static BigInteger ParseSubIdentifier(ref ReadOnlySpan<char> oidValue)
        {
            int endIndex = oidValue.IndexOf('.');

            if (endIndex == -1)
            {
                endIndex = oidValue.Length;
            }
            else if (endIndex == oidValue.Length - 1)
            {
                throw new CryptographicException(SR.Argument_InvalidOidValue);
            }

            // The following code is equivalent to
            // BigInteger.TryParse(temp, NumberStyles.None, CultureInfo.InvariantCulture, out value)
            // TODO: Split this for netstandard vs netcoreapp for span-perf?.
            BigInteger value = BigInteger.Zero;

            for (int position = 0; position < endIndex; position++)
            {
                if (position > 0 && value == 0)
                {
                    // T-REC X.680-201508 sec 12.26
                    throw new CryptographicException(SR.Argument_InvalidOidValue);
                }

                value *= 10;
                value += AtoI(oidValue[position]);
            }

            oidValue = oidValue.Slice(Math.Min(oidValue.Length, endIndex + 1));
            return value;
        }

        private static int AtoI(char c)
        {
            if (c >= '0' && c <= '9')
            {
                return c - '0';
            }

            throw new CryptographicException(SR.Argument_InvalidOidValue);
        }

        // ITU-T-X.690-201508 sec 8.19.5
        private static int EncodeSubIdentifier(Span<byte> dest, ref BigInteger subIdentifier)
        {
            Debug.Assert(dest.Length > 0);

            if (subIdentifier.IsZero)
            {
                dest[0] = 0;
                return 1;
            }
           
            BigInteger unencoded = subIdentifier;
            int idx = 0;

            do
            {
                BigInteger cur = unencoded & 0x7F;
                byte curByte = (byte)cur;

                if (subIdentifier != unencoded)
                {
                    curByte |= 0x80;
                }

                unencoded >>= 7;
                dest[idx] = curByte;
                idx++;
            }
            while (unencoded != BigInteger.Zero);

            Reverse(dest.Slice(0, idx));
            return idx;
        }

        public void WriteEnumeratedValue(object enumValue)
        {
            if (enumValue == null)
                throw new ArgumentNullException(nameof(enumValue));

            WriteEnumeratedValue(Asn1Tag.Enumerated, enumValue);
        }

        public void WriteEnumeratedValue<TEnum>(TEnum value) where TEnum : struct
        {
            WriteEnumeratedValue(Asn1Tag.Enumerated, value);
        }

        public void WriteEnumeratedValue(Asn1Tag tag, object enumValue)
        {
            if (enumValue == null)
                throw new ArgumentNullException(nameof(enumValue));

            WriteEnumeratedValue(tag.AsPrimitive(), enumValue.GetType(), enumValue);
        }

        public void WriteEnumeratedValue<TEnum>(Asn1Tag tag, TEnum value) where TEnum : struct
        {
            WriteEnumeratedValue(tag.AsPrimitive(), typeof(TEnum), value);
        }

        // T-REC-X.690-201508 sec 8.4
        private void WriteEnumeratedValue(Asn1Tag tag, Type tEnum, object enumValue)
        {
            CheckUniversalTag(tag, UniversalTagNumber.Enumerated);
            
            Type backingType = tEnum.GetEnumUnderlyingType();

            if (tEnum.IsDefined(typeof(FlagsAttribute), false))
            {
                throw new ArgumentException(
                    SR.Cryptography_Asn_EnumeratedValueRequiresNonFlagsEnum,
                    nameof(tEnum));
            }

            if (backingType == typeof(ulong))
            {
                ulong numericValue = Convert.ToUInt64(enumValue);
                // T-REC-X.690-201508 sec 8.4
                WriteNonNegativeIntegerCore(tag, numericValue);
            }
            else
            {
                // All other types fit in a (signed) long.
                long numericValue = Convert.ToInt64(enumValue);
                // T-REC-X.690-201508 sec 8.4
                WriteIntegerCore(tag, numericValue);
            }
        }

        public void PushSequence()
        {
            PushSequenceCore(Asn1Tag.Sequence);
        }

        public void PushSequence(Asn1Tag tag)
        {
            CheckUniversalTag(tag, UniversalTagNumber.Sequence);

            // Assert the constructed flag, in case it wasn't.
            PushSequenceCore(tag.AsConstructed());
        }

        // T-REC-X.690-201508 sec 8.9, 8.10
        private void PushSequenceCore(Asn1Tag tag)
        {
            PushTag(tag.AsConstructed());
        }

        public void PopSequence()
        {
            PopSequence(Asn1Tag.Sequence);
        }

        public void PopSequence(Asn1Tag tag)
        {
            // PopSequence shouldn't be used to pop a SetOf.
            CheckUniversalTag(tag, UniversalTagNumber.Sequence);

            // Assert the constructed flag, in case it wasn't.
            PopSequenceCore(tag.AsConstructed());
        }

        // T-REC-X.690-201508 sec 8.9, 8.10
        private void PopSequenceCore(Asn1Tag tag)
        {
            PopTag(tag);
        }

        public void PushSetOf()
        {
            PushSetOf(Asn1Tag.SetOf);
        }

        public void PushSetOf(Asn1Tag tag)
        {
            CheckUniversalTag(tag, UniversalTagNumber.SetOf);

            // Assert the constructed flag, in case it wasn't.
            PushSetOfCore(tag.AsConstructed());
        }

        // T-REC-X.690-201508 sec 8.12
        // The writer claims SetOf, and not Set, so as to avoid the field
        // ordering clause of T-REC-X.690-201508 sec 9.3
        private void PushSetOfCore(Asn1Tag tag)
        {
            PushTag(tag);
        }

        public void PopSetOf()
        {
            PopSetOfCore(Asn1Tag.SetOf);
        }

        public void PopSetOf(Asn1Tag tag)
        {
            CheckUniversalTag(tag, UniversalTagNumber.SetOf);

            // Assert the constructed flag, in case it wasn't.
            PopSetOfCore(tag.AsConstructed());
        }

        // T-REC-X.690-201508 sec 8.12
        private void PopSetOfCore(Asn1Tag tag)
        {
            // T-REC-X.690-201508 sec 11.6
            bool sortContents = RuleSet == AsnEncodingRules.CER || RuleSet == AsnEncodingRules.DER;

            PopTag(tag, sortContents);
        }
        
        public void WriteUtcTime(DateTimeOffset value)
        {
            WriteUtcTimeCore(Asn1Tag.UtcTime, value);
        }

        public void WriteUtcTime(Asn1Tag tag, DateTimeOffset value)
        {
            CheckUniversalTag(tag, UniversalTagNumber.UtcTime);

            // Clear the constructed flag, if present.
            WriteUtcTimeCore(tag.AsPrimitive(), value);
        }

        // T-REC-X.680-201508 sec 47
        // T-REC-X.690-201508 sec 11.8
        private void WriteUtcTimeCore(Asn1Tag tag, DateTimeOffset value)
        { 
            // Because UtcTime is IMPLICIT VisibleString it technically can have
            // a constructed form.
            // DER says character strings must be primitive.
            // CER says character strings <= 1000 encoded bytes must be primitive.
            // So we'll just make BER be primitive, too.
            Debug.Assert(!tag.IsConstructed);
            WriteTag(tag);

            // BER allows for omitting the seconds, but that's not an option we need to expose.
            // BER allows for non-UTC values, but that's also not an option we need to expose.
            // So the format is always yyMMddHHmmssZ (13)
            const int UtcTimeValueLength = 13;
            WriteLength(UtcTimeValueLength);

            DateTimeOffset normalized = value.ToUniversalTime();

            int year = normalized.Year;
            int month = normalized.Month;
            int day = normalized.Day;
            int hour = normalized.Hour;
            int minute = normalized.Minute;
            int second = normalized.Second;

            Span<byte> baseSpan = _buffer.AsSpan().Slice(_offset);
            StandardFormat format = new StandardFormat('D', 2);

            if (!Utf8Formatter.TryFormat(year % 100, baseSpan.Slice(0, 2), out _, format) ||
                !Utf8Formatter.TryFormat(month, baseSpan.Slice(2, 2), out _, format) ||
                !Utf8Formatter.TryFormat(day, baseSpan.Slice(4, 2), out _, format) ||
                !Utf8Formatter.TryFormat(hour, baseSpan.Slice(6, 2), out _, format) ||
                !Utf8Formatter.TryFormat(minute, baseSpan.Slice(8, 2), out _, format) ||
                !Utf8Formatter.TryFormat(second, baseSpan.Slice(10, 2), out _, format))
            {
                Debug.Fail($"Utf8Formatter.TryFormat failed to build components of {normalized:O}");
                throw new CryptographicException();
            }
            
            _buffer[_offset + 12] = (byte)'Z';

            _offset += UtcTimeValueLength;
        }

        public void WriteGeneralizedTime(DateTimeOffset value, bool omitFractionalSeconds = false)
        {
            WriteGeneralizedTimeCore(Asn1Tag.GeneralizedTime, value, omitFractionalSeconds);
        }

        public void WriteGeneralizedTime(Asn1Tag tag, DateTimeOffset value, bool omitFractionalSeconds = false)
        {
            CheckUniversalTag(tag, UniversalTagNumber.GeneralizedTime);

            // Clear the constructed flag, if present.
            WriteGeneralizedTimeCore(tag.AsPrimitive(), value, omitFractionalSeconds);
        }

        // T-REC-X.680-201508 sec 46
        // T-REC-X.690-201508 sec 11.7
        private void WriteGeneralizedTimeCore(Asn1Tag tag, DateTimeOffset value, bool omitFractionalSeconds)
        {
            // GeneralizedTime under BER allows many different options:
            // * (HHmmss), (HHmm), (HH)
            // * "(value).frac", "(value),frac"
            // * frac == 0 may be omitted or emitted
            // non-UTC offset in various formats
            //
            // We're not allowing any of them.
            // Just encode as the CER/DER common restrictions.
            //
            // This results in the following formats:
            // yyyyMMddHHmmssZ
            // yyyyMMddHHmmss.f?Z
            //
            // where "f?" is anything from "f" to "fffffff" (tenth of a second down to 100ns/1-tick)
            // with no trailing zeros.
            DateTimeOffset normalized = value.ToUniversalTime();

            if (normalized.Year > 9999)
            {
                // This is unreachable since DateTimeOffset guards against this internally.
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            // We're only loading in sub-second ticks.
            // Ticks are defined as 1e-7 seconds, so their printed form
            // is at the longest "0.1234567", or 9 bytes.
            Span<byte> fraction = stackalloc byte[0];

            if (!omitFractionalSeconds)
            {
                long floatingTicks = normalized.Ticks % TimeSpan.TicksPerSecond;
                
                if (floatingTicks != 0)
                {
                    // We're only loading in sub-second ticks.
                    // Ticks are defined as 1e-7 seconds, so their printed form
                    // is at the longest "0.1234567", or 9 bytes.
                    fraction = stackalloc byte[9];

                    decimal decimalTicks = floatingTicks;
                    decimalTicks /= TimeSpan.TicksPerSecond;

                    if (!Utf8Formatter.TryFormat(decimalTicks, fraction, out int bytesWritten, new StandardFormat('G')))
                    {
                        Debug.Fail($"Utf8Formatter.TryFormat could not format {floatingTicks} / TicksPerSecond");
                        throw new CryptographicException();
                    }

                    Debug.Assert(bytesWritten > 2, $"{bytesWritten} should be > 2");
                    Debug.Assert(fraction[0] == (byte)'0');
                    Debug.Assert(fraction[1] == (byte)'.');

                    fraction = fraction.Slice(1, bytesWritten - 1);
                }
            }

            // yyyy, MM, dd, hh, mm, ss
            const int IntegerPortionLength = 4 + 2 + 2 + 2 + 2 + 2;
            // Z, and the optional fraction.
            int totalLength = IntegerPortionLength + 1 + fraction.Length;

            // Because GeneralizedTime is IMPLICIT VisibleString it technically can have
            // a constructed form.
            // DER says character strings must be primitive.
            // CER says character strings <= 1000 encoded bytes must be primitive.
            // So we'll just make BER be primitive, too.
            Debug.Assert(!tag.IsConstructed);
            WriteTag(tag);
            WriteLength(totalLength);

            int year = normalized.Year;
            int month = normalized.Month;
            int day = normalized.Day;
            int hour = normalized.Hour;
            int minute = normalized.Minute;
            int second = normalized.Second;

            Span<byte> baseSpan = _buffer.AsSpan().Slice(_offset);
            StandardFormat d4 = new StandardFormat('D', 4);
            StandardFormat d2 = new StandardFormat('D', 2);

            if (!Utf8Formatter.TryFormat(year, baseSpan.Slice(0, 4), out _, d4) ||
                !Utf8Formatter.TryFormat(month, baseSpan.Slice(4, 2), out _, d2) ||
                !Utf8Formatter.TryFormat(day, baseSpan.Slice(6, 2), out _, d2) ||
                !Utf8Formatter.TryFormat(hour, baseSpan.Slice(8, 2), out _, d2) ||
                !Utf8Formatter.TryFormat(minute, baseSpan.Slice(10, 2), out _, d2) ||
                !Utf8Formatter.TryFormat(second, baseSpan.Slice(12, 2), out _, d2))
            {
                Debug.Fail($"Utf8Formatter.TryFormat failed to build components of {normalized:O}");
                throw new CryptographicException();
            }
            
            _offset += IntegerPortionLength;
            fraction.CopyTo(baseSpan.Slice(IntegerPortionLength));
            _offset += fraction.Length;

            _buffer[_offset] = (byte)'Z';
            _offset++;
        }

        /// <summary>
        /// Transfer the encoded representation of the data to <paramref name="dest"/>.
        /// </summary>
        /// <param name="dest">Write destination.</param>
        /// <param name="bytesWritten">The number of bytes which were written to <paramref name="dest"/>.</param>
        /// <returns><c>true</c> if the encode succeeded, <c>false</c> if <paramref name="dest"/> is too small.</returns>
        /// <exception cref="InvalidOperationException">
        ///   A <see cref="PushSequence()"/> or <see cref="PushSetOf()"/> has not been closed via
        ///   <see cref="PopSequence()"/> or <see cref="PopSetOf()"/>.
        /// </exception>
        public bool TryEncode(Span<byte> dest, out int bytesWritten)
        {
            if ((_nestingStack?.Count ?? 0) != 0)
                throw new InvalidOperationException(SR.Cryptography_AsnWriter_EncodeUnbalancedStack);

            // If the stack is closed out then everything is a definite encoding (BER, DER) or a
            // required indefinite encoding (CER). So we're correctly sized up, and ready to copy.
            if (dest.Length < _offset)
            {
                bytesWritten = 0;
                return false;
            }

            if (_offset == 0)
            {
                bytesWritten = 0;
                return true;
            }

            bytesWritten = _offset;
            _buffer.AsSpan().Slice(0, _offset).CopyTo(dest);
            return true;
        }

        public byte[] Encode()
        {
            if ((_nestingStack?.Count ?? 0) != 0)
            {
                throw new InvalidOperationException(SR.Cryptography_AsnWriter_EncodeUnbalancedStack);
            }

            if (_offset == 0)
            {
                return Array.Empty<byte>();
            }

            // If the stack is closed out then everything is a definite encoding (BER, DER) or a
            // required indefinite encoding (CER). So we're correctly sized up, and ready to copy.
            return _buffer.AsSpan().Slice(0, _offset).ToArray();
        }

        private void PushTag(Asn1Tag tag)
        {
            if (_nestingStack == null)
            {
                _nestingStack = new Stack<(Asn1Tag,int)>();
            }

            Debug.Assert(tag.IsConstructed);
            WriteTag(tag);
            _nestingStack.Push((tag, _offset));
            // Indicate that the length is indefinite.
            // We'll come back and clean this up (as appropriate) in PopTag.
            WriteLength(-1);
        }

        private void PopTag(Asn1Tag tag, bool sortContents=false)
        {
            if (_nestingStack == null || _nestingStack.Count == 0)
            {
                throw new ArgumentException(SR.Cryptography_AsnWriter_PopWrongTag, nameof(tag));
            }

            (Asn1Tag stackTag, int lenOffset) = _nestingStack.Peek();

            Debug.Assert(tag.IsConstructed);
            if (stackTag != tag)
            {
                throw new ArgumentException(SR.Cryptography_AsnWriter_PopWrongTag, nameof(tag));
            }

            _nestingStack.Pop();

            if (sortContents)
            {
                SortContents(_buffer, lenOffset + 1, _offset);
            }

            // BER could use the indefinite encoding that CER does.
            // But since the definite encoding form is easier to read (doesn't require a contextual
            // parser to find the end-of-contents marker) some ASN.1 readers (including the previous
            // incarnation of AsnReader) may choose not to support it.
            //
            // So, BER will use the DER rules here, in the interest of broader compatibility.

            // T-REC-X.690-201508 sec 9.1 (constructed CER => indefinite length)
            // T-REC-X.690-201508 sec 8.1.3.6
            if (RuleSet == AsnEncodingRules.CER)
            {
                WriteEndOfContents();
                return;
            }

            int containedLength = _offset - 1 - lenOffset;
            Debug.Assert(containedLength >= 0);

            int shiftSize = GetEncodedLengthSubsequentByteCount(containedLength);

            // Best case, length fits in the compact byte
            if (shiftSize == 0)
            {
                _buffer[lenOffset] = (byte)containedLength;
                return;
            }

            // We're currently at the end, so ensure we have room for N more bytes.
            EnsureWriteCapacity(shiftSize);

            // Buffer.BlockCopy correctly does forward-overlapped, so use it.
            int start = lenOffset + 1;
            Buffer.BlockCopy(_buffer, start, _buffer, start + shiftSize, containedLength);

            int tmp = _offset;
            _offset = lenOffset;
            WriteLength(containedLength);
            Debug.Assert(_offset - lenOffset - 1 == shiftSize);
            _offset = tmp + shiftSize;
        }

        public void WriteCharacterString(UniversalTagNumber encodingType, string str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            WriteCharacterString(encodingType, str.AsReadOnlySpan());
        }

        public void WriteCharacterString(UniversalTagNumber encodingType, ReadOnlySpan<char> str)
        {
            Text.Encoding encoding = AsnCharacterStringEncodings.GetEncoding(encodingType);

            WriteCharacterStringCore(new Asn1Tag(encodingType), encoding, str);
        }

        public void WriteCharacterString(Asn1Tag tag, UniversalTagNumber encodingType, string str)
        {   
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            WriteCharacterString(tag, encodingType, str.AsReadOnlySpan());
        }

        public void WriteCharacterString(Asn1Tag tag, UniversalTagNumber encodingType, ReadOnlySpan<char> str)
        {
            CheckUniversalTag(tag, encodingType);

            Text.Encoding encoding = AsnCharacterStringEncodings.GetEncoding(encodingType);
            WriteCharacterStringCore(tag, encoding, str);
        }

        // T-REC-X.690-201508 sec 8.23
        private void WriteCharacterStringCore(Asn1Tag tag, Text.Encoding encoding, ReadOnlySpan<char> str)
        {
            int size = -1;

            // T-REC-X.690-201508 sec 9.2
            if (RuleSet == AsnEncodingRules.CER)
            {
                // TODO: Split this for netstandard vs netcoreapp for span?.
                unsafe
                {
                    fixed (char* strPtr = &str.DangerousGetPinnableReference())
                    {
                        size = encoding.GetByteCount(strPtr, str.Length);

                        // If it exceeds the primitive segment size, use the constructed encoding.
                        if (size > AsnReader.MaxCERSegmentSize)
                        {
                            WriteConstructedCerCharacterString(tag, encoding, str, size);
                            return;
                        }
                    }
                }
            }

            // TODO: Split this for netstandard vs netcoreapp for span?.
            unsafe
            {
                fixed (char* strPtr = &str.DangerousGetPinnableReference())
                {
                    if (size < 0)
                    {
                        size = encoding.GetByteCount(strPtr, str.Length);
                    }

                    // Clear the constructed tag, if present.
                    WriteTag(tag.AsPrimitive());
                    WriteLength(size);
                    Span<byte> dest = _buffer.AsSpan().Slice(_offset, size);

                    fixed (byte* destPtr = &dest.DangerousGetPinnableReference())
                    {
                        int written = encoding.GetBytes(strPtr, str.Length, destPtr, dest.Length);

                        if (written != size)
                        {
                            Debug.Fail($"Encoding produced different answer for GetByteCount ({size}) and GetBytes ({written})");
                            throw new InvalidOperationException();
                        }
                    }

                    _offset += size;
                }
            }
        }

        private void WriteConstructedCerCharacterString(Asn1Tag tag, Text.Encoding encoding, ReadOnlySpan<char> str, int size)
        {
            Debug.Assert(size > AsnReader.MaxCERSegmentSize);

            byte[] tmp;

            // TODO: Split this for netstandard vs netcoreapp for span?.
            unsafe
            {
                fixed (char* strPtr = &str.DangerousGetPinnableReference())
                {
                    tmp = ArrayPool<byte>.Shared.Rent(size);

                    fixed (byte* destPtr = tmp)
                    {
                        int written = encoding.GetBytes(strPtr, str.Length, destPtr, tmp.Length);

                        if (written != size)
                        {
                            Debug.Fail(
                                $"Encoding produced different answer for GetByteCount ({size}) and GetBytes ({written})");
                            throw new InvalidOperationException();
                        }
                    }
                }
            }

            WriteConstructedCerOctetString(tag, tmp.AsSpan().Slice(0, size));
            Array.Clear(tmp, 0, size);
            ArrayPool<byte>.Shared.Return(tmp);
        }

        private static void SortContents(byte[] buffer, int start, int end)
        {
            Debug.Assert(buffer != null);
            Debug.Assert(end >= start);

            int len = end - start;

            if (len == 0)
            {
                return;
            }

            // Since BER can read everything and the reader does not mutate data
            // just use a BER reader for identifying the positions of the values
            // within this memory segment.
            //
            // Since it's not mutating, any restrictions imposed by CER or DER will
            // still be maintained.
            var reader = new AsnReader(new ReadOnlyMemory<byte>(buffer, start, len), AsnEncodingRules.BER);

            List<(int, int)> positions = new List<(int, int)>();

            int pos = start;

            while (reader.HasData)
            {
                ReadOnlyMemory<byte> encoded = reader.GetEncodedValue();
                positions.Add((pos, encoded.Length));
                pos += encoded.Length;
            }

            Debug.Assert(pos == end);

            var comparer = new SetOfValueComparer(buffer);
            positions.Sort(comparer);

            byte[] tmp = ArrayPool<byte>.Shared.Rent(len);

            pos = 0;

            foreach ((int offset, int length) in positions)
            {
                Buffer.BlockCopy(buffer, offset, tmp, pos, length);
                pos += length;
            }

            Debug.Assert(pos == len);

            Buffer.BlockCopy(tmp, 0, buffer, start, len);
            Array.Clear(tmp, 0, len);
            ArrayPool<byte>.Shared.Return(tmp);
        }

        private static void Reverse(Span<byte> span)
        {
            int i = 0;
            int j = span.Length - 1;

            while (i < j)
            {
                byte tmp = span[i];
                span[i] = span[j];
                span[j] = tmp;

                i++;
                j--;
            }
        }

        private static void CheckUniversalTag(Asn1Tag tag, UniversalTagNumber universalTagNumber)
        {
            if (tag.TagClass == TagClass.Universal && tag.TagValue != (int)universalTagNumber)
            {
                throw new ArgumentException(
                    SR.Cryptography_Asn_UniversalValueIsFixed,
                    nameof(tag));
            }
        }

        private class SetOfValueComparer : IComparer<(int, int)>
        {
            private readonly byte[] _data;

            public SetOfValueComparer(byte[] data)
            {
                _data = data;
            }

            public int Compare((int, int) x, (int, int) y)
            {
                (int xOffset, int xLength) = x;
                (int yOffset, int yLength) = y;

                int min = Math.Min(xLength, yLength);
                int diff;

                for (int i = 0; i < min; i++)
                {
                    int xVal = _data[xOffset + i];
                    byte yVal = _data[yOffset + i];
                    diff = xVal - yVal;

                    if (diff != 0)
                    {
                        return diff;
                    }
                }

                // The sorting rules (T-REC-X.690-201508 sec 11.6) say that the shorter one
                // counts as if it are padded with as many 0x00s on the right as required for
                // comparison.
                //
                // But, since a shorter definite value will have already had the length bytes
                // compared, it was already different.  And a shorter indefinite value will
                // have hit end-of-contents, making it already different.
                //
                // This is here because the spec says it should be, but no values are known
                // which will make diff != 0.
                diff = xLength - yLength;

                if (diff != 0)
                {
                    return diff;
                }

                // Whichever had the lowest index wins (once sorted, stay sorted)
                return xOffset - yOffset;
            }
        }
    }
}
