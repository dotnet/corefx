// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

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

        private unsafe int FindFirstCharacterToEncode(PinnedCharBuffer charBuffer)
        {
            char* pCh = charBuffer.Pointer;
            if (pCh == null)
            {
                Debug.Assert(charBuffer.Length == 0);
                char dummy = default; // don't allow passing null pointer values to FindFirstCharacterToEncode
                pCh = &dummy;
            }

            return FindFirstCharacterToEncode(pCh, charBuffer.Length);
        }

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

            unsafe
            {
                fixed (char* valuePointer = value)
                {
                    PinnedCharBuffer buffer = new PinnedCharBuffer(valuePointer, value, 0, value.Length);

                    int indexOfFirstCharToEncode = FindFirstCharacterToEncode(buffer);
                    if (indexOfFirstCharToEncode < 0)
                    {
                        return value; // shortcut: there's no work to perform
                    }

                    StringBuilder stringBuilder = new StringBuilder(value, 0, indexOfFirstCharToEncode, 0 /* capacity, unused */);
                    Encode(new StringWriter(stringBuilder), buffer.Slice(indexOfFirstCharToEncode));
                    return stringBuilder.ToString();
                }
            }
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

            unsafe
            {
                fixed (char* valuePointer = value)
                {
                    PinnedCharBuffer buffer = new PinnedCharBuffer(valuePointer, value, startIndex, characterCount);

                    int indexOfFirstCharToEncode = FindFirstCharacterToEncode(buffer);
                    if (indexOfFirstCharToEncode < 0)
                    {
                        indexOfFirstCharToEncode = buffer.Length;
                    }

                    // If there's nothing for us to encode *and* we don't need to compute a substring, then write
                    // the string as-is. Otherwise, make a copy of the data and fall down the slower code paths.

                    if (startIndex == 0 && indexOfFirstCharToEncode == value.Length)
                    {
                        output.Write(value);
                    }
                    else
                    {
                        output.Write(value.ToCharArray(startIndex, indexOfFirstCharToEncode)); // this portion doesn't require encoding
                        Encode(output, buffer.Slice(indexOfFirstCharToEncode));
                    }
                }
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

            unsafe
            {
                fixed (char* valuePointer = value)
                {
                    PinnedCharBuffer buffer = new PinnedCharBuffer(valuePointer, value, startIndex, characterCount);

                    int indexOfFirstCharToEncode = FindFirstCharacterToEncode(buffer);
                    if (indexOfFirstCharToEncode < 0)
                    {
                        indexOfFirstCharToEncode = buffer.Length;
                    }
                    output.Write(value, startIndex, indexOfFirstCharToEncode);

                    buffer = buffer.Slice(indexOfFirstCharToEncode);
                    if (buffer.Length > 0)
                    {
                        Encode(output, buffer);
                    }
                }
            }
        }

        /// <summary>
        /// Encodes the supplied characters.
        /// </summary>
        private void Encode(TextWriter output, PinnedCharBuffer source)
        {
            char[] tempEncodedBuffer = new char[checked(MaxOutputCharactersPerInputCharacter * 2) /* max 2 surrogate chars per scalar */];

            while (source.Length > 0)
            {
                int scalarValue = source[0];

                if (!char.IsSurrogate((char)scalarValue))
                {
                    if (!WillEncode(scalarValue))
                    {
                        // single input char -> single output char (no escaping needed)
                        output.Write((char)scalarValue);
                        source = source.Slice(1);
                        continue;
                    }
                }
                else
                {
                    char firstChar = (char)scalarValue;
                    scalarValue = '\uFFFD'; // replacement char, just in case we can't read a full surrogate pair
                    if (char.IsHighSurrogate(firstChar))
                    {
                        if (source.Length > 1)
                        {
                            char secondChar = source[1];
                            if (char.IsLowSurrogate(secondChar))
                            {
                                scalarValue = char.ConvertToUtf32(firstChar, secondChar);
                                if (!WillEncode(scalarValue))
                                {
                                    // 2 input chars -> 2 output chars (no escaping needed)
                                    output.Write(firstChar);
                                    output.Write(secondChar);
                                    source = source.Slice(2);
                                    continue;
                                }
                            }
                        }
                    }
                }

                // If we got to this point, we need to encode.

                unsafe
                {
                    fixed (char* pDest = tempEncodedBuffer)
                    {
                        if (!TryEncodeUnicodeScalar(scalarValue, pDest, tempEncodedBuffer.Length, out int numCharsWrittenJustNow))
                        {
                            ThrowArgumentException_MaxOutputCharsPerInputChar();
                        }

                        output.Write(tempEncodedBuffer, 0, numCharsWrittenJustNow);
                    }
                }

                source = source.Slice(scalarValue > char.MaxValue ? 2 : 1);
            }
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

        private static void ThrowArgumentException_MaxOutputCharsPerInputChar()
        {
            // Ideally we'd throw a proper error message here, but we can't add localized error
            // text in an in-place update. So we'll instead smuggle the bad property name as
            // part of the error message. It's not great, but it's better than nothing since
            // it'll point callers toward this property.

            throw new ArgumentOutOfRangeException(nameof(MaxOutputCharactersPerInputCharacter));
        }
    }
}
