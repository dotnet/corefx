// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
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

        [CLSCompliant(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public abstract bool TryEncodeUnicodeScalar(int unicodeScalar, Span<char> buffer, out int numberOfCharactersWritten);

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

        /// <summary>
        /// Finds index of the first character that needs to be encoded.
        /// </summary>
        /// <param name="text">The text buffer to search.</param>
        /// <returns></returns>
        /// <remarks>This method is seldom called directly. It's used by higher level helper APIs.</remarks>
        [CLSCompliant(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public abstract int FindFirstCharacterToEncode(ReadOnlySpan<char> text);
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

            ReadOnlySpan<char> valueSpan = value.AsSpan();

            int firstCharacterToEncode = FindFirstCharacterToEncode(valueSpan);

            if (firstCharacterToEncode == -1 || valueSpan.IsEmpty)
            {
                return value;
            }

            int bufferSize = MaxOutputCharactersPerInputCharacter * value.Length;

            Span<char> wholebuffer = bufferSize < 1024 ?
                stackalloc char[bufferSize] :
                new char[bufferSize];

            int totalWritten = EncodeIntoBuffer(wholebuffer, valueSpan, firstCharacterToEncode);

            return wholebuffer.Slice(0, totalWritten).ToString();
        }

        // NOTE: The order of the parameters to this method is a work around for https://github.com/dotnet/corefx/issues/4455
        // and the underlying Mono bug: https://bugzilla.xamarin.com/show_bug.cgi?id=36052.
        // If changing the signature of this method, ensure this issue isn't regressing on Mono.
        private int EncodeIntoBuffer(Span<char> buffer, ReadOnlySpan<char> value, int firstCharacterToEncode)
        {
            Debug.Assert(value.Length > 0);

            int totalWritten = 0;

            if (firstCharacterToEncode > 0)
            {
                value.Slice(0, firstCharacterToEncode).CopyTo(buffer);
                totalWritten += firstCharacterToEncode;
            }

            int valueIndex = firstCharacterToEncode;

            char firstChar = value[valueIndex];
            char secondChar = firstChar;
            bool wasSurrogatePair = false;
            int charsWritten;

            // this loop processes character pairs (in case they are surrogates).
            // there is an if block below to process single last character.
            int secondCharIndex;
            for (secondCharIndex = valueIndex + 1; secondCharIndex < value.Length; secondCharIndex++)
            {
                if (!wasSurrogatePair)
                {
                    firstChar = secondChar;
                }
                else
                {
                    firstChar = value[secondCharIndex - 1];
                }
                secondChar = value[secondCharIndex];

                if (!WillEncode(firstChar))
                {
                    wasSurrogatePair = false;
                    buffer[totalWritten++] = firstChar;
                }
                else
                {
                    int nextScalar = UnicodeHelpers.GetScalarValueFromUtf16(firstChar, secondChar, out wasSurrogatePair);
                    if (!TryEncodeUnicodeScalar(nextScalar, buffer.Slice(totalWritten), out charsWritten))
                    {
                        throw new ArgumentException("Argument encoder does not implement MaxOutputCharsPerInputChar correctly.");
                    }

                    totalWritten += charsWritten;
                    if (wasSurrogatePair)
                    {
                        secondCharIndex++;
                    }
                }
            }

            if (secondCharIndex == value.Length)
            {
                firstChar = value[value.Length - 1];
                int nextScalar = UnicodeHelpers.GetScalarValueFromUtf16(firstChar, null, out wasSurrogatePair);
                if (!TryEncodeUnicodeScalar(nextScalar, buffer.Slice(totalWritten), out charsWritten))
                {
                    throw new ArgumentException("Argument encoder does not implement MaxOutputCharsPerInputChar correctly.");
                }
                totalWritten += charsWritten;
            }

            return totalWritten;
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

            ReadOnlySpan<char> span = value.AsSpan(startIndex, characterCount);

            int firstIndexToEncode = FindFirstCharacterToEncode(span);

            if (firstIndexToEncode == -1) // nothing to encode; 
            {
                if (startIndex == 0 && characterCount == value.Length) // write whole string
                {
                    output.Write(value);
                    return;
                }
                for (int i = 0; i < span.Length; i++) // write substring
                {
                    output.Write(span[i]);
                }
                return;
            }

            // write prefix, then encode
            for (int i = 0; i < firstIndexToEncode; i++)
            {
                output.Write(span[i]);
            }

            EncodeCore(output, span.Slice(firstIndexToEncode));
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

            ReadOnlySpan<char> span = value.AsSpan(startIndex, characterCount);

            int firstIndexToEncode = FindFirstCharacterToEncode(span);

            if (firstIndexToEncode == -1) // nothing to encode; 
            {
                if (startIndex == 0 && characterCount == value.Length) // write whole string
                {
                    output.Write(value);
                    return;
                }
                for (int i = 0; i < span.Length; i++) // write substring
                {
                    output.Write(span[i]);
                }
                return;
            }

            // write prefix, then encode
            for (int i = 0; i < firstIndexToEncode; i++)
            {
                output.Write(span[i]);
            }

            EncodeCore(output, span.Slice(firstIndexToEncode));
        }

        private void EncodeCore(TextWriter output, ReadOnlySpan<char> value)
        {
            Debug.Assert(output != null);

            int bufferLength = MaxOutputCharactersPerInputCharacter;
            Span<char> buffer = stackalloc char[bufferLength];

            char firstChar = value[0];
            char secondChar = firstChar;
            bool wasSurrogatePair = false;
            int charsWritten;

            // this loop processes character pairs (in case they are surrogates).
            // there is an if block below to process single last character.
            for (int secondCharIndex = 1; secondCharIndex < value.Length; secondCharIndex++)
            {
                if (!wasSurrogatePair)
                {
                    firstChar = secondChar;
                }
                else
                {
                    firstChar = value[secondCharIndex - 1];
                }
                secondChar = value[secondCharIndex];

                if (!WillEncode(firstChar))
                {
                    wasSurrogatePair = false;
                    output.Write(firstChar);
                }
                else
                {
                    int nextScalar = UnicodeHelpers.GetScalarValueFromUtf16(firstChar, secondChar, out wasSurrogatePair);
                    if (!TryEncodeUnicodeScalar(nextScalar, buffer, out charsWritten))
                    {
                        throw new ArgumentException("Argument encoder does not implement MaxOutputCharsPerInputChar correctly.");
                    }
                    Write(output, buffer.Slice(0, charsWritten));

                    if (wasSurrogatePair)
                    {
                        secondCharIndex++;
                    }
                }
            }

            if (!wasSurrogatePair)
            {
                firstChar = value[value.Length - 1];
                int nextScalar = UnicodeHelpers.GetScalarValueFromUtf16(firstChar, null, out wasSurrogatePair);
                if (!TryEncodeUnicodeScalar(nextScalar, buffer, out charsWritten))
                {
                    throw new ArgumentException("Argument encoder does not implement MaxOutputCharsPerInputChar correctly.");
                }
                Write(output, buffer.Slice(0, charsWritten));
            }
        }

        internal static bool TryCopyCharacters(char[] source, Span<char> destination, out int numberOfCharactersWritten)
        {
            Debug.Assert(source != null);

            if (destination.Length < source.Length)
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
        internal static bool TryWriteScalarAsChar(int unicodeScalar, Span<char> destination, out int numberOfCharactersWritten)
        {
            Debug.Assert(unicodeScalar < ushort.MaxValue);
            if (destination.Length < 1)
            {
                numberOfCharactersWritten = 0;
                return false;
            }
            destination[0] = (char)unicodeScalar;
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

        private static void Write(TextWriter output, Span<char> input)
        {
            Debug.Assert(output != null);

            for (int i = 0; i < input.Length; i++)
            {
                output.Write(input[i]);
            }
        }
    }
}
