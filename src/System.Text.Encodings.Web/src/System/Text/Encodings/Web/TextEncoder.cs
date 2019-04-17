// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.IO;

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
        /*
         * ABSTRACT METHODS
         * 
         * There are only two abstract methods that any encoder instance must override.
         * All other functionality can be built on top of these two methods.
         */

        /// <summary>
        /// Encodes the single scalar value <paramref name="value"/> to <paramref name="buffer"/> as UTF-16.
        /// </summary>
        /// <param name="value">The scalar value to be encoded.</param>
        /// <param name="buffer">The buffer which will receive the encoded scalar value.</param>
        /// <returns>The number of <see cref="char"/> elements written to <paramref name="buffer"/>, or -1
        /// if <paramref name="buffer"/> is too small to hold the encoded representation of <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> cannot be encoded using the current encoder.
        /// </exception>
        public abstract int EncodeSingleRune(Rune value, Span<char> buffer);

        /// <summary>
        /// Returns a value indicating whether <paramref name="value"/> would be encoded by the current encoder.
        /// </summary>
        /// <param name="value">The scalar value to query.</param>
        /// <returns><see langword="true"/> if the current encoder instance would encode <paramref name="value"/>;
        /// <see langword="false"/> otherwise.</returns>
        public abstract bool RuneMustBeEncoded(Rune value);

        /*
         * WRAPPER INSTANCE METHODS
         * 
         * These methods are the intended public API surface of any TextEncoder instance.
         * We can provide default implementations built on top of the two abstract methods,
         * but derived classes can override these methods to provide optimized implementations
         * if desired.
         */

        /// <summary>
        /// Returns the index of the first element in <paramref name="text"/> that would be encoded by
        /// the current encoder, or -1 if no element of <paramref name="text"/> would be encoded.
        /// </summary>
        public virtual int FindFirstCharacterToEncode(ReadOnlySpan<char> text)
        {
            int originalTextLength = text.Length;

            while (!text.IsEmpty)
            {
                // Read off each scalar value from the UTF-16 input stream, breaking when we encounter an
                // invalid sequence (which will later go through U+FFFD replacement + escaping) or when we
                // encounter a scalar value that must be encoded. We'll keep slicing text as we pull off
                // scalar values, so if we must return early a simple subtraction will give us the offset
                // in the original input data where we found data which must be escaped.

                if (Rune.DecodeFromUtf16(text, out Rune thisRune, out int thisRuneLengthInChars) != OperationStatus.Done
                    || RuneMustBeEncoded(thisRune))
                {
                    return originalTextLength - text.Length;
                }

                text = text.Slice(thisRuneLengthInChars);
            }

            return -1; // read all data - nothing required escaping
        }

        /// <summary>
        /// Encodes the supplied string and returns the encoded text as a new string.
        /// </summary>
        /// <param name="value">String to encode.</param>
        /// <returns>Encoded string.</returns>
        public virtual string Encode(string value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            // Fast path: if no character in the input string requires encoding, we
            // can return the string as-is without any further work.

            int idxOfFirstCharToEncode = FindFirstCharacterToEncode(value);
            if (idxOfFirstCharToEncode < 0)
            {
                return value;
            }

            // Slow path: if any character in the input string requires encoding, we'll
            // copy over verbatim all of the leading chars we knew to be ok, then we'll
            // start a scalar-by-scalar transcoding sequence.

            Span<char> scratchBuffer = stackalloc char[16];

            StringBuilder builder = new StringBuilder();
            builder.Append(value, 0, idxOfFirstCharToEncode);

            ReadOnlySpan<char> remainingText = value.AsSpan(idxOfFirstCharToEncode);

            do
            {
                // DecodeFromUtf16 performs U+FFFD replacement automatically when presented with invalid data
                Rune.DecodeFromUtf16(remainingText, out Rune thisRune, out int thisRuneLengthInChars);
                if (RuneMustBeEncoded(thisRune))
                {
                    // Escape this scalar value and write the escaped value to the destination.

                    int escapedRuneCharCount = EncodeSingleRune(thisRune, scratchBuffer);
                    if (escapedRuneCharCount < 0)
                    {
                        // TODO: Resize the buffer instead of failing eagerly
                        throw new ArgumentOutOfRangeException(nameof(value));
                    }

                    builder.Append(scratchBuffer.Slice(0, escapedRuneCharCount));
                }
                else
                {
                    // Don't escape this scalar value; write it as-is. Since U+FFFD replacement may have
                    // been performed we're going to bounce the scalar value through the scratch buffer
                    // first instead of copying data directly from the input stream.

                    builder.Append(scratchBuffer.Slice(0, thisRune.EncodeToUtf16(scratchBuffer)));
                }

                remainingText = remainingText.Slice(thisRuneLengthInChars);
            } while (!remainingText.IsEmpty);

            return builder.ToString();
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
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (output is null)
            {
                throw new ArgumentNullException(nameof(output));
            }
            ValidateRanges(startIndex, characterCount, actualInputLength: value.Length);

            Encode(output, value.AsSpan(startIndex, characterCount));
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
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (output is null)
            {
                throw new ArgumentNullException(nameof(output));
            }
            ValidateRanges(startIndex, characterCount, actualInputLength: value.Length);

            Encode(output, value.AsSpan(startIndex, characterCount));
        }

        private void Encode(TextWriter output, ReadOnlySpan<char> remainingText)
        {
            int idxOfFirstCharToEncode = FindFirstCharacterToEncode(remainingText);
            if (idxOfFirstCharToEncode < 0)
            {
                output.Write(remainingText);
                return;
            }

            // Slow path: if any character in the input string requires encoding, we'll
            // copy over verbatim all of the leading chars we knew to be ok, then we'll
            // start a scalar-by-scalar transcoding sequence.

            Span<char> scratchBuffer = stackalloc char[16];

            do
            {
                // DecodeFromUtf16 performs U+FFFD replacement automatically when presented with invalid data
                Rune.DecodeFromUtf16(remainingText, out Rune thisRune, out int thisRuneLengthInChars);
                if (RuneMustBeEncoded(thisRune))
                {
                    // Escape this scalar value and write the escaped value to the destination.

                    int escapedRuneCharCount = EncodeSingleRune(thisRune, scratchBuffer);
                    if (escapedRuneCharCount < 0)
                    {
                        // TODO: Resize the buffer instead of failing eagerly
                        throw new ArgumentOutOfRangeException("value");
                    }

                    output.Write(scratchBuffer.Slice(0, escapedRuneCharCount));
                }
                else
                {
                    // Don't escape this scalar value; write it as-is. Since U+FFFD replacement may have
                    // been performed we're going to bounce the scalar value through the scratch buffer
                    // first instead of copying data directly from the input stream.

                    output.Write(scratchBuffer.Slice(0, thisRune.EncodeToUtf16(scratchBuffer)));
                }

                remainingText = remainingText.Slice(thisRuneLengthInChars);
            } while (!remainingText.IsEmpty);
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
    }
}
