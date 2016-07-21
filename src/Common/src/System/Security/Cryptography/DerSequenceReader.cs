// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Text;
using System.Threading;

namespace System.Security.Cryptography
{
    /// <summary>
    /// Reads data encoded via the Distinguished Encoding Rules for Abstract
    /// Syntax Notation 1 (ASN.1) data.
    /// </summary>
    internal class DerSequenceReader
    {
        internal const byte ConstructedFlag = 0x20;
        internal const byte ContextSpecificTagFlag = 0x80;

        internal static DateTimeFormatInfo s_validityDateTimeFormatInfo;

        private readonly byte[] _data;
        private readonly int _end;
        private int _position;

        internal int ContentLength { get; private set; }

        private DerSequenceReader(bool startAtPayload, byte[] data)
        {
            Debug.Assert(startAtPayload, "This overload is only for bypassing the sequence tag");
            Debug.Assert(data != null, "Data is null");

            _data = data;
            _position = 0;
            _end = data.Length;

            ContentLength = data.Length;
        }

        internal DerSequenceReader(byte[] data)
            : this(data, 0, data.Length)
        {
        }

        internal DerSequenceReader(byte[] data, int offset, int length)
            : this(DerTag.Sequence, data, offset, length)
        {
        }

        /// <summary>
        /// This constructor allows the DerSequenceReader to be extended to read sets. For
        /// safe check on the tag and length in the encoding use ReadSet. 
        /// </summary>
        internal DerSequenceReader(DerTag tagToEat, byte[] data, int offset, int length)
        {
            _data = data;
            _end = offset + length;

            Debug.Assert(data != null, "Data is null");
            Debug.Assert(offset >= 0, "Offset is negative");
            Debug.Assert(length >= 2, "Length is too short");
            Debug.Assert(length <= data.Length - offset, "Array is too short");

            _position = offset;
            EatTag(tagToEat);
            ContentLength = EatLength();
        }

        internal static DerSequenceReader CreateForPayload(byte[] payload)
        {
            return new DerSequenceReader(true, payload);
        }

        internal bool HasData
        {
            get { return _position < _end; }
        }

        internal byte PeekTag()
        {
            if (_data.Length <= _position)
                throw new InvalidOperationException(SR.Cryptography_Der_Invalid_Encoding);
            return _data[_position];
        }

        internal void SkipValue()
        {
            EatTag((DerTag)PeekTag());
            int contentLength = EatLength();
            _position += contentLength;
        }

        /// <summary>
        /// Returns the next value encoded (this includes tag and length)
        /// </summary>
        internal byte[] ReadNextEncodedValue()
        {
            int lengthLength;
            int contentLength = ScanContentLength(_data, _position + 1, out lengthLength);
            // Length of tag, encoded length, and the content
            int totalLength = 1 + lengthLength + contentLength;

            byte[] encodedValue = new byte[totalLength];
            Buffer.BlockCopy(_data, _position, encodedValue, 0, totalLength);

            _position += totalLength;
            return encodedValue;
        }

        internal byte[] ReadRemainingData()
        {
            if (_end <= _position)
                return Array.Empty<byte>();

            int remainingBytes = _end - _position;
            byte[] remainingData = new byte[remainingBytes];
            Buffer.BlockCopy(_data, _position, remainingData, 0, remainingBytes);
            return remainingData;
        }

        /// <summary>
        /// Splits a DER encoded value in a byte[] into a byte[][3] containing {tag, encodedLength, value}
        /// </summary>
        /// <param name="payload">DER value</param>
        /// <param name="offset">REference parameter on where to start splitting. it will change to the value
        /// after the end of the element being split as specified by the encoded length</param>
        /// <returns>DER value split as byte[][3] containing {tag, encodedLength, value}</returns>
        private static byte[][] SplitValue(byte[] payload, ref int offset)
        {
            int lengthLength;
            int contentLength = ScanContentLength(payload, offset + 1, out lengthLength);

            byte[] encodedLength = new byte[lengthLength];

            byte tag = payload[offset];
            offset++;

            Buffer.BlockCopy(payload, offset, encodedLength, 0, lengthLength);
            offset += lengthLength;

            byte[] value;
            if (contentLength != 0)
            {
                value = new byte[contentLength];
                Buffer.BlockCopy(payload, offset, value, 0, contentLength);
            }
            else
            {
                value = Array.Empty<byte>();
            }

            offset += contentLength;

            return new byte[][] {
                new byte[] { tag },
                encodedLength,
                value
            };
        }

        /// <summary>
        /// This method returns the next value encoded (including tag and length), as
        /// a byte[][3] to be fed to DerEncoder when needed.
        /// </summary>
        internal byte[][] ReadAndSplitNextEncodedValue()
        {
            return SplitValue(_data, ref _position);
        }

        /// <summary>
        /// Splits a DER encoded value in a byte[] into a byte[][3] containing {tag, encodedLength, value}
        /// </summary>
        /// <param name="payload">DER payload to split</param>
        /// <returns>paylot split into a byte[][3]</returns>
        internal static byte[][] SplitValue(byte[] payload)
        {
            int offset = 0;
            return SplitValue(payload, ref offset);
        }

        internal int ReadInteger()
        {
            byte[] integerBytes = ReadIntegerBytes();

            // integerBytes is currently Big-Endian, need to reverse it for
            // Little-Endian to pass into BigInteger.
            Array.Reverse(integerBytes);
            BigInteger bigInt = new BigInteger(integerBytes);
            return (int)bigInt;
        }

        internal byte[] ReadIntegerBytes()
        {
            EatTag(DerTag.Integer);

            return ReadContentAsBytes();
        }

        internal byte[] ReadOctetString()
        {
            EatTag(DerTag.OctetString);

            return ReadContentAsBytes();
        }

        internal string ReadOidAsString()
        {
            EatTag(DerTag.ObjectIdentifier);
            int contentLength = EatLength();

            // For desktop compat, empty Oids are supported
            if (contentLength == 0)
                return string.Empty;

            // Each byte could cause 3 decimal characters to be written, plus a period. Over-allocate
            // and avoid re-alloc.
            StringBuilder builder = new StringBuilder(contentLength * 4);

            // The first byte is ((X * 40) + Y), where X is the first segment and Y the second.
            // ISO/IEC 8825-1:2003 section 8.19.4

            byte firstByte = _data[_position];
            byte first = (byte)(firstByte / 40);
            byte second = (byte)(firstByte % 40);

            builder.Append(first);
            builder.Append('.');
            builder.Append(second);

            // For the rest of the segments, the high bit on the byte is a continuation marker,
            // and data is loaded into a BigInteger 7 bits at a time.
            //
            // When the high bit is 0, the segment ends, so emit a '.' between it and the next one.
            //
            // ISO/IEC 8825-1:2003 section 8.19.2, and the .NET representation of Oid.Value.
            bool needDot = true;
            BigInteger bigInt = new BigInteger(0);

            for (int i = 1; i < contentLength; i++)
            {
                byte current = _data[_position + i];
                byte data = (byte)(current & 0x7F);

                if (needDot)
                {
                    builder.Append('.');
                    needDot = false;
                }

                bigInt <<= 7;
                bigInt += data;

                if (current == data)
                {
                    builder.Append(bigInt);
                    bigInt = 0;
                    needDot = true;
                }
            }

            _position += contentLength;
            return builder.ToString();
        }

        internal byte[] ReadBitString()
        {
            // Bitstrings are primitives with tag and length, followed by a number
            // of unused bits in the last byte, followed by the payload that can be read 
            // as a byte array. The unused bits in the last byte of the array should then be cleared.
            EatTag(DerTag.BitString);
            int length = EatLength();
            
            byte numUnusedBits = _data[_position];
            _position += 1;

            if (length == 1)
                return Array.Empty<byte>();
            
            byte[] octets = new byte[length-1];
            Buffer.BlockCopy(_data, _position, octets, 0, octets.Length);

            // DER forces the padding to be all zeros, but to future-proof and to allow for extensibility
            // to a BER reader in the future we clear the unused bits in the last byte. 
            byte mask = (byte)~((1 << numUnusedBits) - 1);
            octets[octets.Length - 1] &= mask;

            _position += length;
            return octets;
        }

        internal Oid ReadOid()
        {
            return new Oid(ReadOidAsString());
        }

        /// <summary>
        /// Use for explicitly defined sequences. This returns a reader that eats both tags. 
        /// </summary>
        /// <returns>DerSequenceReader ready to read the inner contents of a explicit sequence</returns>
        internal DerSequenceReader ReadExplicitContextSequence()
        {
            byte tag = PeekTag();
            if ((tag & ContextSpecificTagFlag) == 0)
            {
                throw new InvalidOperationException(SR.Format(
                    SR.InvalidOperation_Unexpected_Tag,
                    ContextSpecificTagFlag.ToString("X2"),
                    tag.ToString("X2"),
                    _position));
            }

            EatTag((DerTag)tag);
            EatLength();

            return ReadSequence();
        }

        private DerSequenceReader ReadCollectionWithTag(DerTag expected)
        {
            // DerSequenceReader wants to read its own tag, so don't EatTag here.
            CheckTag(expected, _data, _position);

            int lengthLength;
            int contentLength = ScanContentLength(_data, _position + 1, out lengthLength);
            int totalLength = 1 + lengthLength + contentLength;

            DerSequenceReader reader = new DerSequenceReader(expected, _data, _position, totalLength);
            _position += totalLength;
            return reader;
        }

        internal DerSequenceReader ReadSequence()
        {
            return ReadCollectionWithTag(DerTag.Sequence);
        }

        internal DerSequenceReader ReadSet()
        {
            return ReadCollectionWithTag(DerTag.Set);
        }

        internal string ReadIA5String()
        {
            EatTag(DerTag.IA5String);
            int contentLength = EatLength();

            // IA5 (International Alphabet - 5) is functionally equivalent to 7-bit ASCII.

            string ia5String = System.Text.Encoding.ASCII.GetString(_data, _position, contentLength);
            _position += contentLength;

            return ia5String;
        }

        internal DateTime ReadUtcTime()
        {
            return ReadTime(DerTag.UTCTime, "yyMMddHHmmss'Z'");
        }

        internal DateTime ReadGeneralizedTime()
        {
            // Currently only supports reading times with no fractional seconds or time differentials
            // as RFC 2630 doesn't allow these. In case this is done, the format string has to be parsed
            // to follow rules on X.680 and X.690.
            return ReadTime(DerTag.GeneralizedTime, "yyyyMMddHHmmss'Z'");
        }

        private DateTime ReadTime(DerTag timeTag, string formatString)
        {
            EatTag(timeTag);
            int contentLength = EatLength();

            string decodedTime = System.Text.Encoding.ASCII.GetString(_data, _position, contentLength);
            _position += contentLength;
            
            Debug.Assert(
                decodedTime[decodedTime.Length-1] == 'Z', 
                $"The date doesn't follow the X.690 format, ending with {decodedTime[decodedTime.Length - 1]}");

            DateTime time;

            DateTimeFormatInfo fi = LazyInitializer.EnsureInitialized(
                ref s_validityDateTimeFormatInfo,
                () =>
                {
                    var clone = (DateTimeFormatInfo)CultureInfo.InvariantCulture.DateTimeFormat.Clone();
                    clone.Calendar.TwoDigitYearMax = 2049;

                    return clone;
                });

            if (!DateTime.TryParseExact(
                    decodedTime,
                    formatString,
                    fi,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                    out time))
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
            }

            return time;
        }

        private byte[] ReadContentAsBytes()
        {
            int contentLength = EatLength();

            byte[] octets = new byte[contentLength];
            Buffer.BlockCopy(_data, _position, octets, 0, contentLength);

            _position += contentLength;
            return octets;
        }

        private void EatTag(DerTag expected)
        {
            CheckTag(expected, _data, _position);
            _position++;
        }

        private static void CheckTag(DerTag expected, byte[] data, int position)
        {
            if (data.Length <= position)
                throw new InvalidOperationException(SR.Cryptography_Der_Invalid_Encoding);
            byte actual = data[position];

            // Context-specific datatypes cannot be tag-verified
            if ((actual & ContextSpecificTagFlag) != 0)
            {
                return;
            }

            byte relevant = (byte)(actual & 0x1F);
            byte expectedByte = (byte)((byte)expected & 0x1F);

            if (expectedByte != relevant)
            {
                throw new InvalidOperationException(SR.Format(
                    SR.InvalidOperation_Unexpected_Tag,
                    expectedByte.ToString("X2"),
                    actual.ToString("X2"),
                    position));
            }
        }

        private int EatLength()
        {
            int bytesConsumed;
            int answer = ScanContentLength(_data, _position, out bytesConsumed);

            _position += bytesConsumed;
            return answer;
        }

        private static int ScanContentLength(byte[] data, int offset, out int bytesConsumed)
        {
            if (data.Length <= offset)
                throw new InvalidOperationException(SR.Cryptography_Der_Invalid_Encoding);
            byte lengthOrLengthLength = data[offset];

            if (lengthOrLengthLength < 0x80)
            {
                bytesConsumed = 1;

                if (lengthOrLengthLength > data.Length - offset - 1)
                    throw new InvalidOperationException(SR.Cryptography_Der_Invalid_Length);
                
                return lengthOrLengthLength;
            }

            // The one byte which was lengthLength, plus the number of bytes it said to consume.
            bytesConsumed = 1 + (lengthOrLengthLength & 0x7F);

            int end = offset + bytesConsumed;
            int accum = 0;
            
            // data[offset] is lengthLength, so start at data[offset + 1] and stop before
            // data[offset + 1 + lengthLength], aka data[end].
            for (int i = offset + 1; i < end; i++)
            {
                accum <<= 8;
                accum += data[i];
            }

            if (accum > data.Length - end)
                throw new InvalidOperationException(SR.Cryptography_Der_Invalid_Length);

            return accum;
        }

        internal enum DerTag : byte
        {
            Boolean = 0x01,
            Integer = 0x02,
            BitString = 0x03,
            OctetString = 0x04,
            Null = 0x05,
            ObjectIdentifier = 0x06,
            UTF8String = 0x0C,
            Sequence = 0x10,
            Set = 0x11,
            PrintableString = 0x13,
            T61String = 0x14,
            IA5String = 0x16,
            UTCTime = 0x17,
            GeneralizedTime = 0x18,
        }
    }
}
