// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Numerics;

namespace System.Security.Cryptography.Asn1
{
    internal sealed partial class AsnWriter
    {
        /// <summary>
        ///   Write an Integer value with tag UNIVERSAL 2.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="ObjectDisposedException">The writer has been Disposed.</exception>
        public void WriteInteger(long value)
        {
            WriteIntegerCore(Asn1Tag.Integer, value);
        }

        /// <summary>
        ///   Write an Integer value with tag UNIVERSAL 2.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="ObjectDisposedException">The writer has been Disposed.</exception>
        public void WriteInteger(ulong value)
        {
            WriteNonNegativeIntegerCore(Asn1Tag.Integer, value);
        }

        /// <summary>
        ///   Write an Integer value with tag UNIVERSAL 2.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="ObjectDisposedException">The writer has been Disposed.</exception>
        public void WriteInteger(BigInteger value)
        {
            WriteIntegerCore(Asn1Tag.Integer, value);
        }

        /// <summary>
        ///   Write an Integer value with a specified tag.
        /// </summary>
        /// <param name="value">The integer value to write, in signed big-endian byte order.</param>
        /// <exception cref="CryptographicException">
        ///   the 9 most sigificant bits are all set --OR--
        ///   the 9 most sigificant bits are all unset
        /// </exception>
        /// <exception cref="ObjectDisposedException">The writer has been Disposed.</exception>
        public void WriteInteger(ReadOnlySpan<byte> value)
        {
            WriteIntegerCore(Asn1Tag.Integer, value);
        }

        /// <summary>
        ///   Write an Integer value with a specified tag.
        /// </summary>
        /// <param name="tag">The tag to write.</param>
        /// <param name="value">The value to write.</param>
        /// <exception cref="ArgumentException">
        ///   <paramref name="tag"/>.<see cref="Asn1Tag.TagClass"/> is
        ///   <see cref="TagClass.Universal"/>, but
        ///   <paramref name="tag"/>.<see cref="Asn1Tag.TagValue"/> is not correct for
        ///   the method
        /// </exception>
        /// <exception cref="ObjectDisposedException">The writer has been Disposed.</exception>
        public void WriteInteger(Asn1Tag tag, long value)
        {
            CheckUniversalTag(tag, UniversalTagNumber.Integer);

            WriteIntegerCore(tag.AsPrimitive(), value);
        }

        /// <summary>
        ///   Write an Integer value with a specified tag.
        /// </summary>
        /// <param name="tag">The tag to write.</param>
        /// <param name="value">The value to write.</param>
        /// <exception cref="ArgumentException">
        ///   <paramref name="tag"/>.<see cref="Asn1Tag.TagClass"/> is
        ///   <see cref="TagClass.Universal"/>, but
        ///   <paramref name="tag"/>.<see cref="Asn1Tag.TagValue"/> is not correct for
        ///   the method
        /// </exception>
        /// <exception cref="ObjectDisposedException">The writer has been Disposed.</exception>
        public void WriteInteger(Asn1Tag tag, ulong value)
        {
            CheckUniversalTag(tag, UniversalTagNumber.Integer);

            WriteNonNegativeIntegerCore(tag.AsPrimitive(), value);
        }

        /// <summary>
        ///   Write an Integer value with a specified tag.
        /// </summary>
        /// <param name="tag">The tag to write.</param>
        /// <param name="value">The value to write.</param>
        /// <exception cref="ArgumentException">
        ///   <paramref name="tag"/>.<see cref="Asn1Tag.TagClass"/> is
        ///   <see cref="TagClass.Universal"/>, but
        ///   <paramref name="tag"/>.<see cref="Asn1Tag.TagValue"/> is not correct for
        ///   the method
        /// </exception>
        /// <exception cref="ObjectDisposedException">The writer has been Disposed.</exception>
        public void WriteInteger(Asn1Tag tag, BigInteger value)
        {
            CheckUniversalTag(tag, UniversalTagNumber.Integer);

            WriteIntegerCore(tag.AsPrimitive(), value);
        }

        /// <summary>
        ///   Write an Integer value with a specified tag.
        /// </summary>
        /// <param name="tag">The tag to write.</param>
        /// <param name="value">The integer value to write, in signed big-endian byte order.</param>
        /// <exception cref="ArgumentException">
        ///   <paramref name="tag"/>.<see cref="Asn1Tag.TagClass"/> is
        ///   <see cref="TagClass.Universal"/>, but
        ///   <paramref name="tag"/>.<see cref="Asn1Tag.TagValue"/> is not correct for
        ///   the method
        /// </exception>
        /// <exception cref="CryptographicException">
        ///   the 9 most sigificant bits are all set --OR--
        ///   the 9 most sigificant bits are all unset
        /// </exception>
        /// <exception cref="ObjectDisposedException">The writer has been Disposed.</exception>
        public void WriteInteger(Asn1Tag tag, ReadOnlySpan<byte> value)
        {
            CheckUniversalTag(tag, UniversalTagNumber.Integer);

            WriteIntegerCore(tag.AsPrimitive(), value);
        }

        /// <summary>
        ///   Write an Integer value with tag UNIVERSAL 2.
        /// </summary>
        /// <param name="value">The integer value to write, in unsigned big-endian byte order.</param>
        /// <exception cref="CryptographicException">
        ///   the 9 most sigificant bits are all unset
        /// </exception>
        /// <exception cref="ObjectDisposedException">The writer has been Disposed.</exception>
        public void WriteIntegerUnsigned(ReadOnlySpan<byte> value)
        {
            WriteIntegerUnsignedCore(Asn1Tag.Integer, value);
        }

        /// <summary>
        ///   Write an Integer value with a specified tag.
        /// </summary>
        /// <param name="tag">The tag to write.</param>
        /// <param name="value">The integer value to write, in unsigned big-endian byte order.</param>
        /// <exception cref="ArgumentException">
        ///   <paramref name="tag"/>.<see cref="Asn1Tag.TagClass"/> is
        ///   <see cref="TagClass.Universal"/>, but
        ///   <paramref name="tag"/>.<see cref="Asn1Tag.TagValue"/> is not correct for
        ///   the method
        /// </exception>
        /// <exception cref="CryptographicException">
        ///   the 9 most sigificant bits are all unset
        /// </exception>
        /// <exception cref="ObjectDisposedException">The writer has been Disposed.</exception>
        public void WriteIntegerUnsigned(Asn1Tag tag, ReadOnlySpan<byte> value)
        {
            CheckUniversalTag(tag, UniversalTagNumber.Integer);

            WriteIntegerUnsignedCore(tag.AsPrimitive(), value);
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

        private void WriteIntegerUnsignedCore(Asn1Tag tag, ReadOnlySpan<byte> value)
        {
            if (value.IsEmpty)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
            }

            // T-REC-X.690-201508 sec 8.3.2
            if (value.Length > 1 && value[0] == 0 && value[1] < 0x80)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
            }

            Debug.Assert(!tag.IsConstructed);
            WriteTag(tag);

            if (value[0] >= 0x80)
            {
                WriteLength(checked(value.Length + 1));
                _buffer[_offset] = 0;
                _offset++;
            }
            else
            {
                WriteLength(value.Length);
            }

            value.CopyTo(_buffer.AsSpan(_offset));
            _offset += value.Length;
        }

        private void WriteIntegerCore(Asn1Tag tag, ReadOnlySpan<byte> value)
        {
            CheckDisposed();

            if (value.IsEmpty)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
            }

            // T-REC-X.690-201508 sec 8.3.2
            if (value.Length > 1)
            {
                ushort bigEndianValue = (ushort)(value[0] << 8 | value[1]);
                const ushort RedundancyMask = 0b1111_1111_1000_0000;
                ushort masked = (ushort)(bigEndianValue & RedundancyMask);

                // If the first 9 bits are all 0 or are all 1, the value is invalid.
                if (masked == 0 || masked == RedundancyMask)
                {
                    throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
                }
            }

            Debug.Assert(!tag.IsConstructed);
            WriteTag(tag);
            WriteLength(value.Length);
            // WriteLength ensures the content-space
            value.CopyTo(_buffer.AsSpan(_offset));
            _offset += value.Length;
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
    }
}
