// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System.Buffers.Text
{
    /// <summary>
    /// Convert between binary data and UTF-8 encoded text that is represented in base 64.
    /// </summary>
    public static partial class Base64
    {
        /// <summary>
        /// Encode the span of binary data into UTF-8 encoded text represented as base 64.
        ///
        /// <param name="bytes">The input span which contains binary data that needs to be encoded.</param>
        /// <param name="utf8">The output span which contains the result of the operation, i.e. the UTF-8 encoded text in base 64.</param>
        /// <param name="consumed">The number of input bytes consumed during the operation. This can be used to slice the input for subsequent calls, if necessary.</param>
        /// <param name="written">The number of bytes written into the output span. This can be used to slice the output for subsequent calls, if necessary.</param>
        /// <param name="isFinalBlock">True (default) when the input span contains the entire data to decode. 
        /// Set to false only if it is known that the input span contains partial data with more data to follow.</param>
        /// <returns>It returns the OperationStatus enum values:
        /// - Done - on successful processing of the entire input span
        /// - DestinationTooSmall - if there is not enough space in the output span to fit the encoded input
        /// - NeedMoreData - only if isFinalBlock is false, otherwise the output is padded if the input is not a multiple of 3
        /// It does not return InvalidData since that is not possible for base 64 encoding.</returns>
        /// </summary> 
        public static OperationStatus EncodeToUtf8(ReadOnlySpan<byte> bytes, Span<byte> utf8, out int consumed, out int written, bool isFinalBlock = true)
        {
            ref byte srcBytes = ref bytes.DangerousGetPinnableReference();
            ref byte destBytes = ref utf8.DangerousGetPinnableReference();

            int srcLength = bytes.Length;
            int destLength = utf8.Length;

            int sourceIndex = 0;
            int destIndex = 0;
            int result = 0;

            if (destLength >= GetMaxEncodedToUtf8Length(srcLength))
            {
                while (sourceIndex < srcLength - 2)
                {
                    result = Encode(ref Unsafe.Add(ref srcBytes, sourceIndex));
                    Unsafe.WriteUnaligned(ref Unsafe.Add(ref destBytes, destIndex), result);
                    destIndex += 4;
                    sourceIndex += 3;
                }
            }
            else
            {
                while (sourceIndex < srcLength - 2)
                {
                    result = Encode(ref Unsafe.Add(ref srcBytes, sourceIndex));
                    if (destIndex > destLength - 4) goto DestinationSmallExit;
                    Unsafe.WriteUnaligned(ref Unsafe.Add(ref destBytes, destIndex), result);
                    destIndex += 4;
                    sourceIndex += 3;
                }
            }
            
            if (isFinalBlock != true) goto NeedMoreDataExit;

            if (sourceIndex == srcLength - 1)
            {
                result = EncodeAndPadTwo(ref Unsafe.Add(ref srcBytes, sourceIndex));
                if (destIndex > destLength - 4) goto DestinationSmallExit;
                Unsafe.WriteUnaligned(ref Unsafe.Add(ref destBytes, destIndex), result);
                destIndex += 4;
                sourceIndex += 1;
            }
            else if (sourceIndex == srcLength - 2)
            {
                result = EncodeAndPadOne(ref Unsafe.Add(ref srcBytes, sourceIndex));
                if (destIndex > destLength - 4) goto DestinationSmallExit;
                Unsafe.WriteUnaligned(ref Unsafe.Add(ref destBytes, destIndex), result);
                destIndex += 4;
                sourceIndex += 2;
            }

            consumed = sourceIndex;
            written = destIndex;
            return OperationStatus.Done;

            NeedMoreDataExit:
            consumed = sourceIndex;
            written = destIndex;
            return OperationStatus.NeedMoreData;

            DestinationSmallExit:
            consumed = sourceIndex;
            written = destIndex;
            return OperationStatus.DestinationTooSmall;
        }

        /// <summary>
        /// Returns the maximum length (in bytes) of the result if you were to encode binary data within a byte span of size "length".
        /// </summary>
        /// <exception cref="System.OverflowException">
        /// Thrown when the specified <paramref name="length"/> is larger than 1610612733 (since encode inflates the data by 4/3).
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetMaxEncodedToUtf8Length(int length)
        {
            Debug.Assert(length >= 0);
            checked
            {
                return (((length + 2) / 3) * 4);
            }
        }

        /// <summary>
        /// Encode the span of binary data (in-place) into UTF-8 encoded text represented as base 64. 
        /// The encoded text output is larger than the binary data contained in the input (the operation inflates the data).
        ///
        /// <param name="buffer">The input span which contains binary data that needs to be encoded. 
        /// It needs to be large enough to fit the result of the operation.</param>
        /// <param name="dataLength">The amount of binary data contained within the buffer that needs to be encoded 
        /// (and needs to be smaller than the buffer length).</param>
        /// <param name="written">The number of bytes written into the buffer.</param>
        /// <returns>It returns the OperationStatus enum values:
        /// - Done - on successful processing of the entire buffer
        /// - DestinationTooSmall - if there is not enough space in the buffer beyond dataLength to fit the result of encoding the input
        /// It does not return NeedMoreData since this method tramples the data in the buffer and hence can only be called once with all the data in the buffer.
        /// It does not return InvalidData since that is not possible for base 64 encoding.</returns>
        /// </summary> 
        public static OperationStatus EncodeToUtf8InPlace(Span<byte> buffer, int dataLength, out int written)
        {
            var encodedLength = GetMaxEncodedToUtf8Length(dataLength);
            if (buffer.Length < encodedLength) goto FalseExit;

            var leftover = dataLength - dataLength / 3 * 3; // how many bytes after packs of 3

            var destinationIndex = encodedLength - 4;
            var sourceIndex = dataLength - leftover;

            // encode last pack to avoid conditional in the main loop
            if (leftover != 0)
            {
                var sourceSlice = buffer.Slice(sourceIndex, leftover);
                var desitnationSlice = buffer.Slice(destinationIndex, 4);
                destinationIndex -= 4;
                var result = EncodeToUtf8(sourceSlice, desitnationSlice, out _, out int tempWritten);
                Debug.Assert(result == OperationStatus.Done);
            }

            for (int index = sourceIndex - 3; index >= 0; index -= 3)
            {
                var sourceSlice = buffer.Slice(index, 3);
                var desitnationSlice = buffer.Slice(destinationIndex, 4);
                destinationIndex -= 4;
                var result = EncodeToUtf8(sourceSlice, desitnationSlice, out _, out int tempWritten);
                Debug.Assert(result == OperationStatus.Done);
            }

            written = encodedLength;
            return OperationStatus.Done;

            FalseExit:
            written = 0;
            return OperationStatus.DestinationTooSmall;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Encode(byte b0, byte b1, byte b2, out byte r0, out byte r1, out byte r2, out byte r3)
        {
            int i0 = b0 >> 2;
            r0 = s_encodingMap[i0];

            int i1 = (b0 & 0x3) << 4 | (b1 >> 4);
            r1 = s_encodingMap[i1];

            int i2 = (b1 & 0xF) << 2 | (b2 >> 6);
            r2 = s_encodingMap[i2];

            int i3 = b2 & 0x3F;
            r3 = s_encodingMap[i3];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Encode(byte b0, byte b1, byte b2, out byte r0, out byte r1, out byte r2)
        {
            int i0 = b0 >> 2;
            r0 = s_encodingMap[i0];

            int i1 = (b0 & 0x3) << 4 | (b1 >> 4);
            r1 = s_encodingMap[i1];

            int i2 = (b1 & 0xF) << 2 | (b2 >> 6);
            r2 = s_encodingMap[i2];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Encode(byte b0, byte b1, out byte r0, out byte r1)
        {
            int i0 = b0 >> 2;
            r0 = s_encodingMap[i0];

            int i1 = (b0 & 0x3) << 4 | (b1 >> 4);
            r1 = s_encodingMap[i1];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Encode(ref byte threeBytes)
        {
            byte b0 = threeBytes;
            byte b1 = Unsafe.Add(ref threeBytes, 1);
            byte b2 = Unsafe.Add(ref threeBytes, 2);

            Encode(b0, b1, b2, out byte r0, out byte r1, out byte r2, out byte r3);

            int result = r3 << 24 | r2 << 16 | r1 << 8 | r0;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int EncodeAndPadOne(ref byte twoBytes)
        {
            Encode(twoBytes, Unsafe.Add(ref twoBytes, 1), 0, out byte r0, out byte r1, out byte r2);
            int result = s_encodingPad << 24 | r2 << 16 | r1 << 8 | r0;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int EncodeAndPadTwo(ref byte oneByte)
        {
            Encode(oneByte, 0, out byte r0, out byte r1);
            int result = s_encodingPad << 24 | s_encodingPad << 16 | r1 << 8 | r0;
            return result;
        }

        // Pre-computing this table using a custom string(s_characters) and GenerateEncodingMapAndVerify (found in tests)
        static readonly byte[] s_encodingMap = {
            65, 66, 67, 68, 69, 70, 71, 72,         //A..H
            73, 74, 75, 76, 77, 78, 79, 80,         //I..P
            81, 82, 83, 84, 85, 86, 87, 88,         //Q..X
            89, 90, 97, 98, 99, 100, 101, 102,      //Y..Z, a..f
            103, 104, 105, 106, 107, 108, 109, 110, //g..n
            111, 112, 113, 114, 115, 116, 117, 118, //o..v
            119, 120, 121, 122, 48, 49, 50, 51,     //w..z, 0..3
            52, 53, 54, 55, 56, 57, 43, 47          //4..9, +, /
        };

        const byte s_encodingPad = (byte)'='; // '=', for padding
    }
}
