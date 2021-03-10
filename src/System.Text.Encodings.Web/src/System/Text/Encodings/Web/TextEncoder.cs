// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Unicode;

namespace System.Text.Encodings.Web
{
    /// <summary>
    /// An abstraction representing various text encoders. 
    /// </summary>
    /// <remarks>
    /// TextEncoder subclasses can be used to do HTML encoding, URI encoding, and JavaScript encoding. 
    /// Instances of such subclasses can be accessed using <see cref="HtmlEncoder.Default"/>, <see cref="UrlEncoder.Default"/>, and <see cref="JavaScriptEncoder.Default"/>.
    /// </remarks>
    public abstract class TextEncoder
    {
        private const int EncodeStartingOutputBufferSize = 1024; // bytes or chars, depending

        // Fast cache for Ascii
        private byte[][] _asciiEscape = new byte[0x80][];

        // Keep a reference to Array.Empty<byte> as this is used as a singleton for comparisons
        // and there is no guarantee that Array.Empty<byte>() will always be the same instance.
        private static readonly byte[] s_noEscape = Array.Empty<byte>();

        // The following pragma disables a warning complaining about non-CLS compliant members being abstract, 
        // and wants me to mark the type as non-CLS compliant. 
        // It is true that this type cannot be extended by all CLS compliant languages. 
        // Having said that, if I marked the type as non-CLS all methods that take it as parameter will now have to be marked CLSCompliant(false), 
        // yet consumption of concrete encoders is totally CLS compliant, 
        // as it?s mainly to be done by calling helper methods in TextEncoderExtensions class, 
        // and so I think the warning is a bit too aggressive.  

        /// <summary>
        /// Encodes a Unicode scalar into a buffer.
        /// </summary>
        /// <param name="unicodeScalar">Unicode scalar.</param>
        /// <param name="buffer">The destination of the encoded text.</param>
        /// <param name="bufferLength">Length of the destination <paramref name="buffer"/> in chars.</param>
        /// <param name="numberOfCharactersWritten">Number of characters written to the <paramref name="buffer"/>.</param>
        /// <returns>Returns false if <paramref name="bufferLength"/> is too small to fit the encoded text, otherwise returns true.</returns>
        /// <remarks>This method is seldom called directly. One of the TextEncoder.Encode overloads should be used instead.
        /// Implementations of <see cref="TextEncoder"/> need to be thread safe and stateless.
        /// </remarks>
#pragma warning disable 3011
        [CLSCompliant(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public unsafe abstract bool TryEncodeUnicodeScalar(int unicodeScalar, char* buffer, int bufferLength, out int numberOfCharactersWritten);

        // all subclasses have the same implementation of this method.
        // but this cannot be made virtual, because it will cause a virtual call to Encodes, and it destroys perf, i.e. makes common scenario 2x slower 

        /// <summary>
        /// Finds index of the first character that needs to be encoded.
        /// </summary>
        /// <param name="text">The text buffer to search.</param>
        /// <param name="textLength">The number of characters in the <paramref name="text"/>.</param>
        /// <returns></returns>
        /// <remarks>This method is seldom called directly. It's used by higher level helper APIs.</remarks>
        [CLSCompliant(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public unsafe abstract int FindFirstCharacterToEncode(char* text, int textLength);
#pragma warning restore

        /// <summary>
        /// Determines if a given Unicode scalar will be encoded.
        /// </summary>
        /// <param name="unicodeScalar">Unicode scalar.</param>
        /// <returns>Returns true if the <paramref name="unicodeScalar"/> will be encoded by this encoder, otherwise returns false.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public abstract bool WillEncode(int unicodeScalar);

        // this could be a field, but I am trying to make the abstraction pure.

        /// <summary>
        /// Maximum number of characters that this encoder can generate for each input character.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public abstract int MaxOutputCharactersPerInputCharacter { get; }

        /// <summary>
        /// Encodes the supplied string and returns the encoded text as a new string.
        /// </summary>
        /// <param name="value">String to encode.</param>
        /// <returns>Encoded string.</returns>
        public virtual string Encode(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            int indexOfFirstCharToEncode = FindFirstCharacterToEncode(value.AsSpan());
            if (indexOfFirstCharToEncode < 0)
            {
                return value; // shortcut: there's no work to perform
            }

            ReadOnlySpan<char> remainingInput = value.AsSpan(indexOfFirstCharToEncode);
            ValueStringBuilder stringBuilder = new ValueStringBuilder(stackalloc char[EncodeStartingOutputBufferSize]);

#if !netcoreapp
            // Can't call string.Concat later in the method, so memcpy now.
            stringBuilder.Append(value.AsSpan(0, indexOfFirstCharToEncode));
#endif

            // On each iteration of the main loop, we'll make sure we have at least this many chars left in the
            // destination buffer. This should prevent us from making very chatty calls where we only make progress
            // one char at a time.
            int minBufferBumpEachIteration = Math.Max(MaxOutputCharactersPerInputCharacter, EncodeStartingOutputBufferSize);

            do
            {
                // AppendSpan mutates the VSB length to include the newly-added span. This potentially overallocates.
                Span<char> destBuffer = stringBuilder.AppendSpan(Math.Max(remainingInput.Length, minBufferBumpEachIteration));
                Encode(remainingInput, destBuffer, out int charsConsumedJustNow, out int charsWrittenJustNow);
                if (charsWrittenJustNow == 0 || (uint)charsWrittenJustNow > (uint)destBuffer.Length)
                {
                    ThrowArgumentException_MaxOutputCharsPerInputChar(); // couldn't make forward progress or returned bogus data
                }
                remainingInput = remainingInput.Slice(charsConsumedJustNow);
                // It's likely we didn't populate the entire span. If this is the case, adjust the VSB length
                // to reflect that there's unused buffer at the end of the VSB instance.
                stringBuilder.Length -= destBuffer.Length - charsWrittenJustNow;
            } while (!remainingInput.IsEmpty);

#if netcoreapp
            string retVal = string.Concat(value.AsSpan(0, indexOfFirstCharToEncode), stringBuilder.AsSpan());
            stringBuilder.Dispose();
            return retVal;
#else
            return stringBuilder.ToString();
#endif
        }

        /// <summary>
        /// Encodes the supplied string into a <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="output">Encoded text is written to this output.</param>
        /// <param name="value">String to be encoded.</param>
        public void Encode(TextWriter output, string value)
        {
            Encode(output, value, 0, value.Length);
        }

        /// <summary>
        ///  Encodes a substring into a <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="output">Encoded text is written to this output.</param>
        /// <param name="value">String whose substring is to be encoded.</param>
        /// <param name="startIndex">The index where the substring starts.</param>
        /// <param name="characterCount">Number of characters in the substring.</param>
        public virtual void Encode(TextWriter output, string value, int startIndex, int characterCount)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }
            ValidateRanges(startIndex, characterCount, actualInputLength: value.Length);

            int indexOfFirstCharToEncode = FindFirstCharacterToEncode(value.AsSpan(startIndex, characterCount));
            if (indexOfFirstCharToEncode < 0)
            {
                indexOfFirstCharToEncode = characterCount;
            }

            // memcpy all characters that don't require encoding, then encode any remaining chars

            output.WritePartialString(value, startIndex, indexOfFirstCharToEncode);
            if (indexOfFirstCharToEncode != characterCount)
            {
                Encode(output, value.AsSpan(startIndex + indexOfFirstCharToEncode, characterCount - indexOfFirstCharToEncode));
            }
        }

        /// <summary>
        ///  Encodes characters from an array into a <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="output">Encoded text is written to the output.</param>
        /// <param name="value">Array of characters to be encoded.</param>
        /// <param name="startIndex">The index where the substring starts.</param>
        /// <param name="characterCount">Number of characters in the substring.</param>
        public virtual void Encode(TextWriter output, char[] value, int startIndex, int characterCount)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }
            ValidateRanges(startIndex, characterCount, actualInputLength: value.Length);

            int indexOfFirstCharToEncode = FindFirstCharacterToEncode(value.AsSpan(startIndex, characterCount));
            if (indexOfFirstCharToEncode < 0)
            {
                indexOfFirstCharToEncode = characterCount;
            }
            output.Write(value, startIndex, indexOfFirstCharToEncode);

            if (indexOfFirstCharToEncode != characterCount)
            {
                Encode(output, value.AsSpan(startIndex + indexOfFirstCharToEncode, characterCount - indexOfFirstCharToEncode));
            }
        }

        /// <summary>
        /// Encodes the supplied UTF-8 text.
        /// </summary>
        /// <param name="utf8Source">A source buffer containing the UTF-8 text to encode.</param>
        /// <param name="utf8Destination">The destination buffer to which the encoded form of <paramref name="utf8Source"/>
        /// will be written.</param>
        /// <param name="bytesConsumed">The number of bytes consumed from the <paramref name="utf8Source"/> buffer.</param>
        /// <param name="bytesWritten">The number of bytes written to the <paramref name="utf8Destination"/> buffer.</param>
        /// <param name="isFinalBlock"><see langword="true"/> if there is further source data that needs to be encoded;
        /// <see langword="false"/> if there is no further source data that needs to be encoded.</param>
        /// <returns>An <see cref="OperationStatus"/> describing the result of the encoding operation.</returns>
        /// <remarks>The buffers <paramref name="utf8Source"/> and <paramref name="utf8Destination"/> must not overlap.</remarks>
        public unsafe virtual OperationStatus EncodeUtf8(
            ReadOnlySpan<byte> utf8Source,
            Span<byte> utf8Destination,
            out int bytesConsumed,
            out int bytesWritten,
            bool isFinalBlock = true)
        {
            int originalUtf8SourceLength = utf8Source.Length;
            int originalUtf8DestinationLength = utf8Destination.Length;
            
            const int TempUtf16CharBufferLength = 24; // arbitrarily chosen, but sufficient for any reasonable implementation
            char* pTempCharBuffer = stackalloc char[TempUtf16CharBufferLength];

            const int TempUtf8ByteBufferLength = TempUtf16CharBufferLength * 3 /* max UTF-8 output code units per UTF-16 input code unit */;
            byte* pTempUtf8Buffer = stackalloc byte[TempUtf8ByteBufferLength];

            uint nextScalarValue;
            int utf8BytesConsumedForScalar = 0;
            int nonEscapedByteCount = 0;
            OperationStatus opStatus = OperationStatus.Done;

            while (!utf8Source.IsEmpty)
            {
                // For performance, read until we require escaping.
                do
                {
                    nextScalarValue = utf8Source[nonEscapedByteCount];
                    if (UnicodeUtility.IsAsciiCodePoint(nextScalarValue))
                    {
                        // Check Ascii cache.
                        byte[] encodedBytes = GetAsciiEncoding((byte)nextScalarValue);

                        if (ReferenceEquals(encodedBytes, s_noEscape))
                        {
                            if (++nonEscapedByteCount <= utf8Destination.Length)
                            {
                                // Source data can be copied as-is.
                                continue;
                            }

                            --nonEscapedByteCount;
                            opStatus = OperationStatus.DestinationTooSmall;
                            break;
                        }

                        if (encodedBytes == null)
                        {
                            // We need to escape and update the cache, so break out of this loop.
                            opStatus = OperationStatus.Done;
                            utf8BytesConsumedForScalar = 1;
                            break;
                        }

                        // For performance, handle the non-escaped bytes and encoding here instead of breaking out of the loop.
                        if (nonEscapedByteCount > 0)
                        {
                            // We previously verified the destination size.
                            Debug.Assert(nonEscapedByteCount <= utf8Destination.Length);

                            utf8Source.Slice(0, nonEscapedByteCount).CopyTo(utf8Destination);
                            utf8Source = utf8Source.Slice(nonEscapedByteCount);
                            utf8Destination = utf8Destination.Slice(nonEscapedByteCount);
                            nonEscapedByteCount = 0;
                        }

                        if (!((ReadOnlySpan<byte>)encodedBytes).TryCopyTo(utf8Destination))
                        {
                            opStatus = OperationStatus.DestinationTooSmall;
                            break;
                        }

                        utf8Destination = utf8Destination.Slice(encodedBytes.Length);
                        utf8Source = utf8Source.Slice(1);
                        continue;
                    }

                    // Code path for non-Ascii.
                    opStatus = UnicodeHelpers.DecodeScalarValueFromUtf8(utf8Source.Slice(nonEscapedByteCount), out nextScalarValue, out utf8BytesConsumedForScalar);
                    if (opStatus == OperationStatus.Done)
                    {
                        if (!WillEncode((int)nextScalarValue))
                        {
                            nonEscapedByteCount += utf8BytesConsumedForScalar;
                            if (nonEscapedByteCount <= utf8Destination.Length)
                            {
                                // Source data can be copied as-is.
                                continue;
                            }

                            nonEscapedByteCount -= utf8BytesConsumedForScalar;
                            opStatus = OperationStatus.DestinationTooSmall;
                        }
                    }

                    // We need to escape.
                    break;
                } while (nonEscapedByteCount < utf8Source.Length);

                if (nonEscapedByteCount > 0)
                {
                    // We previously verified the destination size.
                    Debug.Assert(nonEscapedByteCount <= utf8Destination.Length);

                    utf8Source.Slice(0, nonEscapedByteCount).CopyTo(utf8Destination);
                    utf8Source = utf8Source.Slice(nonEscapedByteCount);
                    utf8Destination = utf8Destination.Slice(nonEscapedByteCount);
                    nonEscapedByteCount = 0;
                }

                if (utf8Source.IsEmpty)
                {
                    goto Done;
                }

                // This code path is hit for ill-formed input data (where decoding has replaced it with U+FFFD)
                // and for well-formed input data that must be escaped.

                if (opStatus != OperationStatus.Done) // Optimize happy path.
                {
                    if (opStatus == OperationStatus.NeedMoreData)
                    {
                        if (!isFinalBlock)
                        {
                            bytesConsumed = originalUtf8SourceLength - utf8Source.Length;
                            bytesWritten = originalUtf8DestinationLength - utf8Destination.Length;
                            return OperationStatus.NeedMoreData;
                        }
                        // else treat this as a normal invalid subsequence.
                    }
                    else if (opStatus == OperationStatus.DestinationTooSmall)
                    {
                        goto ReturnDestinationTooSmall;
                    }
                }

                if (TryEncodeUnicodeScalar((int)nextScalarValue, pTempCharBuffer, TempUtf16CharBufferLength, out int charsWrittenJustNow))
                {
                    // Now that we have it as UTF-16, transcode it to UTF-8.
                    // Need to copy it to a temporary buffer first, otherwise GetBytes might throw an exception
                    // due to lack of output space.

                    int transcodedByteCountThisIteration = Encoding.UTF8.GetBytes(pTempCharBuffer, charsWrittenJustNow, pTempUtf8Buffer, TempUtf8ByteBufferLength);
                    ReadOnlySpan<byte> transcodedUtf8BytesThisIteration = new ReadOnlySpan<byte>(pTempUtf8Buffer, transcodedByteCountThisIteration);

                    // Update cache for Ascii
                    if (UnicodeUtility.IsAsciiCodePoint(nextScalarValue))
                    {
                        _asciiEscape[nextScalarValue] = transcodedUtf8BytesThisIteration.ToArray();
                    }

                    if (!transcodedUtf8BytesThisIteration.TryCopyTo(utf8Destination))
                    {
                        goto ReturnDestinationTooSmall;
                    }

                    utf8Destination = utf8Destination.Slice(transcodedByteCountThisIteration);
                }
                else
                {
                    // We really don't expect this to fail. If that happens we'll report an error to our caller.
                    bytesConsumed = originalUtf8SourceLength - utf8Source.Length;
                    bytesWritten = originalUtf8DestinationLength - utf8Destination.Length;
                    return OperationStatus.InvalidData;
                }

                utf8Source = utf8Source.Slice(utf8BytesConsumedForScalar);
            }

        Done:
            // Input buffer has been fully processed!
            bytesConsumed = originalUtf8SourceLength;
            bytesWritten = originalUtf8DestinationLength - utf8Destination.Length;
            return OperationStatus.Done;

        ReturnDestinationTooSmall:
            bytesConsumed = originalUtf8SourceLength - utf8Source.Length;
            bytesWritten = originalUtf8DestinationLength - utf8Destination.Length;
            return OperationStatus.DestinationTooSmall;
        }

        /// <summary>
        /// Shim function which can call virtual method <see cref="EncodeUtf8"/> using fast dispatch.
        /// </summary>
        internal static OperationStatus EncodeUtf8Shim(TextEncoder encoder, ReadOnlySpan<byte> utf8Source, Span<byte> utf8Destination, out int bytesConsumed, out int bytesWritten, bool isFinalBlock)
        {
            return encoder.EncodeUtf8(utf8Source, utf8Destination, out bytesConsumed, out bytesWritten, isFinalBlock);
        }

        /// <summary>
        /// Encodes the supplied characters.
        /// </summary>
        /// <param name="source">A source buffer containing the characters to encode.</param>
        /// <param name="destination">The destination buffer to which the encoded form of <paramref name="source"/>
        /// will be written.</param>
        /// <param name="charsConsumed">The number of characters consumed from the <paramref name="source"/> buffer.</param>
        /// <param name="charsWritten">The number of characters written to the <paramref name="destination"/> buffer.</param>
        /// <param name="isFinalBlock"><see langword="true"/> if there is further source data that needs to be encoded;
        /// <see langword="false"/> if there is no further source data that needs to be encoded.</param>
        /// <returns>An <see cref="OperationStatus"/> describing the result of the encoding operation.</returns>
        /// <remarks>The buffers <paramref name="source"/> and <paramref name="destination"/> must not overlap.</remarks>
        public virtual OperationStatus Encode(
            ReadOnlySpan<char> source,
            Span<char> destination,
            out int charsConsumed,
            out int charsWritten,
            bool isFinalBlock = true)
        {
            if (source.IsEmpty)
            {
                // There's nothing to do.
                charsConsumed = 0;
                charsWritten = 0;
                return OperationStatus.Done;
            }

            // The Encode method is intended to be called in a loop, potentially where the source buffer
            // is much larger than the destination buffer. We don't want to walk the entire source buffer
            // on each invocation of this method, so we'll slice the source buffer to be no larger than
            // the destination buffer to avoid performing unnecessary work. The potential exists for us to
            // split the source in the middle of a UTF-16 surrogate pair. If this happens,
            // FindFirstCharacterToEncode will report the split surrogate as "needs encoding", we'll fall
            // back down the slow path, and the slow path will handle the surrogate appropriately.

            ReadOnlySpan<char> sourceSearchSpace = source;
            if (destination.Length < source.Length)
            {
                sourceSearchSpace = source.Slice(0, destination.Length);
            }

            int idxOfFirstCharToEncode = FindFirstCharacterToEncode(sourceSearchSpace);
            if (idxOfFirstCharToEncode < 0)
            {
                idxOfFirstCharToEncode = sourceSearchSpace.Length;
            }

            source.Slice(0, idxOfFirstCharToEncode).CopyTo(destination); // memcpy data that doesn't need to be encoded
            if (idxOfFirstCharToEncode == source.Length)
            {
                charsConsumed = source.Length;
                charsWritten = source.Length;
                return OperationStatus.Done; // memcopied all chars, nothing more to do
            }

            // If we got to this point, we couldn't memcpy the entire source buffer into the destination.
            // Either the destination was too short or we found data that needs to be encoded.

            OperationStatus opStatus = EncodeCore(source.Slice(idxOfFirstCharToEncode), destination.Slice(idxOfFirstCharToEncode), out int remainingCharsConsumed, out int remainingCharsWritten, isFinalBlock);
            charsConsumed = idxOfFirstCharToEncode + remainingCharsConsumed;
            charsWritten = idxOfFirstCharToEncode + remainingCharsWritten;
            return opStatus;

            OperationStatus EncodeCore(ReadOnlySpan<char> source, Span<char> destination, out int charsConsumed, out int charsWritten, bool isFinalBlock)
            {
                Debug.Assert(!source.IsEmpty, "Caller should've handled fully-consumed source in fast path.");

                if (destination.IsEmpty)
                {
                    destination = Array.Empty<char>(); // normalize empty destination buffers to non-nullptr reference; TryEncodeUnicodeScalar requires this
                }

                int destinationOffset = 0;
                int sourceOffset = 0;
                while ((uint)sourceOffset < (uint)source.Length)
                {
                    int scalarValue = source[sourceOffset];

                    if (!UnicodeUtility.IsSurrogateCodePoint((uint)scalarValue))
                    {
                        if (!WillEncode(scalarValue))
                        {
                            // single input char -> single output char (no escaping needed)
                            if ((uint)destinationOffset >= (uint)destination.Length)
                            {
                                goto DestinationTooSmall;
                            }

                            destination[destinationOffset++] = (char)scalarValue;
                            sourceOffset++;
                            continue;
                        }
                    }
                    else
                    {
                        uint firstCodePoint = (uint)scalarValue;
                        scalarValue = '\uFFFD'; // replacement char, just in case we can't read a full surrogate pair
                        if (UnicodeUtility.IsHighSurrogateCodePoint(firstCodePoint))
                        {
                            int nextSourceIdx = sourceOffset + 1;
                            if ((uint)nextSourceIdx >= (uint)source.Length)
                            {
                                if (!isFinalBlock)
                                {
                                    goto NeedMoreData;
                                }
                            }
                            else
                            {
                                uint nextCodePoint = source[nextSourceIdx];
                                if (UnicodeUtility.IsLowSurrogateCodePoint(nextCodePoint))
                                {
                                    scalarValue = (int)UnicodeUtility.GetScalarFromUtf16SurrogatePair(firstCodePoint, nextCodePoint);
                                    if (!WillEncode(scalarValue))
                                    {
                                        // 2 input chars -> 2 output chars (no escaping needed)
                                        if ((uint)(destinationOffset + 1) >= (uint)destination.Length)
                                        {
                                            goto DestinationTooSmall;
                                        }

                                        destination[destinationOffset] = (char)firstCodePoint;
                                        destination[destinationOffset + 1] = (char)nextCodePoint;
                                        destinationOffset += 2;
                                        sourceOffset += 2;
                                        continue;
                                    }
                                }
                            }
                        }
                    }

                    // If we got to this point, we need to encode.

                    int numCharsWrittenJustNow;
                    unsafe
                    {
                        fixed (char* pDest = &MemoryMarshal.GetReference(destination))
                        {
                            Debug.Assert(pDest != null); // should've been handled on method entry
                            Debug.Assert((uint)destinationOffset <= (uint)destination.Length);

                            if (!TryEncodeUnicodeScalar(scalarValue, pDest + destinationOffset, destination.Length - destinationOffset, out numCharsWrittenJustNow))
                            {
                                goto DestinationTooSmall;
                            }
                        }
                    }

                    Debug.Assert(numCharsWrittenJustNow <= destination.Length - destinationOffset, "TryEncodeUnicodeScalar wrote past end of buffer?");
                    sourceOffset += UnicodeUtility.GetUtf16SequenceLength((uint)scalarValue);
                    destinationOffset += numCharsWrittenJustNow;
                }

                OperationStatus retVal = OperationStatus.Done;

            ReturnCommon:
                Debug.Assert(sourceOffset <= source.Length);
                Debug.Assert(destinationOffset <= destination.Length);
                charsConsumed = sourceOffset;
                charsWritten = destinationOffset;
                return retVal;

            NeedMoreData:
                retVal = OperationStatus.NeedMoreData;
                goto ReturnCommon;

            DestinationTooSmall:
                retVal = OperationStatus.DestinationTooSmall;
                goto ReturnCommon;
            }
        }

        private void Encode(TextWriter output, ReadOnlySpan<char> value)
        {
            Debug.Assert(output != null);
            Debug.Assert(!value.IsEmpty, "Caller should've special-cased 'no encoding needed'.");

            // On each iteration of the main loop, we'll make sure we have at least this many chars left in the
            // destination buffer. This should prevent us from making very chatty calls where we only make progress
            // one char at a time.
            int minBufferBumpEachIteration = Math.Max(MaxOutputCharactersPerInputCharacter, EncodeStartingOutputBufferSize);
            char[] rentedArray = ArrayPool<char>.Shared.Rent(Math.Max(value.Length, minBufferBumpEachIteration));
            Span<char> scratchBuffer = rentedArray;

            do
            {
                Encode(value, scratchBuffer, out int charsConsumedJustNow, out int charsWrittenJustNow);
                if (charsWrittenJustNow == 0 || (uint)charsWrittenJustNow > (uint)scratchBuffer.Length)
                {
                    ThrowArgumentException_MaxOutputCharsPerInputChar(); // couldn't make forward progress or returned bogus data
                }

                output.Write(rentedArray, 0, charsWrittenJustNow); // write char[], not Span<char>, for best compat & performance
                value = value.Slice(charsConsumedJustNow);
            } while (!value.IsEmpty);

            ArrayPool<char>.Shared.Return(rentedArray);
        }

        private unsafe int FindFirstCharacterToEncode(ReadOnlySpan<char> text)
        {
            fixed (char* pText = &MemoryMarshal.GetReference(text))
            {
                return FindFirstCharacterToEncode(pText, text.Length);
            }
        }

        /// <summary>
        /// Given a UTF-8 text input buffer, finds the first element in the input buffer which would be
        /// escaped by the current encoder instance.
        /// </summary>
        /// <param name="utf8Text">The UTF-8 text input buffer to search.</param>
        /// <returns>
        /// The index of the first element in <paramref name="utf8Text"/> which would be escaped by the
        /// current encoder instance, or -1 if no data in <paramref name="utf8Text"/> requires escaping.
        /// </returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual int FindFirstCharacterToEncodeUtf8(ReadOnlySpan<byte> utf8Text)
        {
            int originalUtf8TextLength = utf8Text.Length;

            // Loop through the input text, terminating when we see ill-formed UTF-8 or when we decode a scalar value
            // that must be encoded. If we see either of these things then we'll return its index in the original
            // input sequence. If we consume the entire text without seeing either of these, return -1 to indicate
            // that the text can be copied as-is without escaping.

            int i = 0;
            while (i < utf8Text.Length)
            {
                byte value = utf8Text[i];
                if (UnicodeUtility.IsAsciiCodePoint(value))
                {
                    if (!ReferenceEquals(GetAsciiEncoding(value), s_noEscape))
                    {
                        return originalUtf8TextLength - utf8Text.Length + i;
                    }

                    i++;
                }
                else
                {
                    if (i > 0)
                    {
                        utf8Text = utf8Text.Slice(i);
                    }

                    if (UnicodeHelpers.DecodeScalarValueFromUtf8(utf8Text, out uint nextScalarValue, out int bytesConsumedThisIteration) != OperationStatus.Done
                      || WillEncode((int)nextScalarValue))
                    {
                        return originalUtf8TextLength - utf8Text.Length;
                    }

                    i = bytesConsumedThisIteration;
                }
            }

            return -1; // no input data needs to be escaped
        }

        /// <summary>
        /// Shim function which can call virtual method <see cref="FindFirstCharacterToEncodeUtf8"/> using fast dispatch.
        /// </summary>
        internal static int FindFirstCharacterToEncodeUtf8Shim(TextEncoder encoder, ReadOnlySpan<byte> utf8Text)
        {
            return encoder.FindFirstCharacterToEncodeUtf8(utf8Text);
        }

        internal static unsafe bool TryCopyCharacters(char[] source, char* destination, int destinationLength, out int numberOfCharactersWritten)
        {
            Debug.Assert(source != null && destination != null && destinationLength >= 0);

            if (destinationLength < source.Length)
            {
                numberOfCharactersWritten = 0;
                return false;
            }

            for (int i = 0; i < source.Length; i++)
            {
                destination[i] = source[i];
            }

            numberOfCharactersWritten = source.Length;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe bool TryWriteScalarAsChar(int unicodeScalar, char* destination, int destinationLength, out int numberOfCharactersWritten)
        {
            Debug.Assert(destination != null && destinationLength >= 0);

            Debug.Assert(unicodeScalar < ushort.MaxValue);
            if (destinationLength < 1)
            {
                numberOfCharactersWritten = 0;
                return false;
            }
            *destination = (char)unicodeScalar;
            numberOfCharactersWritten = 1;
            return true;
        }

        private static void ValidateRanges(int startIndex, int characterCount, int actualInputLength)
        {
            if (startIndex < 0 || startIndex > actualInputLength)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            }
            if (characterCount < 0 || characterCount > (actualInputLength - startIndex))
            {
                throw new ArgumentOutOfRangeException(nameof(characterCount));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte[] GetAsciiEncoding(byte value)
        {
            byte[] encoding = _asciiEscape[value];
            if (encoding == null)
            {
                if (!WillEncode(value))
                {
                    encoding = s_noEscape;
                    _asciiEscape[value] = encoding;
                }
            }

            return encoding;
        }

        private static void ThrowArgumentException_MaxOutputCharsPerInputChar()
        {
            throw new ArgumentException("Argument encoder does not implement MaxOutputCharsPerInputChar correctly.");
        }
    }
}
