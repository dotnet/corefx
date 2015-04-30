// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;
using System.Runtime.CompilerServices;

namespace System.Text.Encodings.Web
{
    public static class TextEncoderExtensions
    {
        public static void Encode(this TextEncoder encoder, string value, TextWriter output)
        {
            Encode(encoder, value, 0, value.Length, output);
        }

        public static string Encode(this TextEncoder encoder, string value)
        {
            if (value == null) {
                return value;
            }

            int valueIndex = encoder.FindFirstCharacterToEncode(value);

            if (valueIndex == -1) 
            {
                return value;
            }

            int bufferSize = encoder.MaxOutputCharsPerInputChar * value.Length;
            char[] buffer = new char[bufferSize];

            if (valueIndex > 0)
            {
                var firstCharacterToEncode = valueIndex;
                value.CopyTo(0, buffer, 0, firstCharacterToEncode);                
            }

            int bufferIndex = valueIndex;

            char firstChar = value[valueIndex];
            char secondChar = firstChar;
            bool wasSurrogatePair = false;
            int charsWritten;

            // this loop processes character pairs (in case they are surrogates).
            // there is an if block below to process single last character.
            for (int secondCharIndex = valueIndex + 1; secondCharIndex < value.Length; secondCharIndex++) {
                if (!wasSurrogatePair) {
                    firstChar = secondChar;
                }
                else {
                    firstChar = value[secondCharIndex - 1];
                }
                secondChar = value[secondCharIndex];

                if (!encoder.Encodes(firstChar))
                {
                    wasSurrogatePair = false;
                    if (bufferIndex < buffer.Length)
                    {
                        buffer[bufferIndex++] = firstChar;
                    }               
                }
                else
                {
                    int nextScalar = UnicodeHelpers.GetScalarValueFromUtf16(firstChar, secondChar, out wasSurrogatePair);
                    if (!encoder.TryEncodeUnicodeScalar(nextScalar, buffer, bufferIndex, out charsWritten))
                    {
                        throw new NotImplementedException("this should never happen");
                    }

                    bufferIndex += charsWritten;
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
                if (!encoder.TryEncodeUnicodeScalar(nextScalar, buffer, bufferIndex, out charsWritten)) {
                    throw new NotImplementedException("this should never happen");
                }

                bufferIndex += charsWritten;
            }

            var result = new String(buffer, 0, bufferIndex);
            return result;
        }

        public static void Encode(this TextEncoder encoder, string value, int startIndex, int charCount, TextWriter output)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (output == null)
            {
                throw new ArgumentNullException("output");
            }
            ValidateRanges(startIndex, charCount, actualInputLength: value.Length);

            int valueIndex = encoder.FindFirstCharacterToEncode(value); // TODO (Pri 0): this does not work with substrings, and so causes an inefficiency

            if (valueIndex == -1)
            {
                output.Write(value.ToCharArray(startIndex, charCount)); // TODO: this could be optimized for small substrings. Same below
                return;
            }

            if (valueIndex > 0 && valueIndex > startIndex)
            {
                output.Write(value.ToCharArray(startIndex, valueIndex - startIndex));
            }

            unsafe
            {
                fixed (char* valuePointer = value)
                {
                    var subrangePointer = valuePointer + valueIndex;
                    EncodeCore(encoder, subrangePointer, charCount - (valueIndex - startIndex), output);
                }
            }
        }

        public static void Encode(this TextEncoder encoder, char[] value, int startIndex, int charCount, TextWriter output)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (output == null)
            {
                throw new ArgumentNullException("output");
            }
            ValidateRanges(startIndex, charCount, actualInputLength: value.Length);

            int valueIndex = encoder.FindFirstCharacterToEncode(new string(value, 0, charCount + startIndex));

            if (valueIndex == -1)
            {
                output.Write(value, startIndex, charCount);
                return;
            }

            if (valueIndex > 0 && valueIndex > startIndex)
            {
                output.Write(value, startIndex, valueIndex - startIndex);
            }

            unsafe
            {
                fixed (char* valuePointer = value)
                {
                    var subrangePointer = valuePointer + valueIndex;
                    EncodeCore(encoder, subrangePointer, charCount - (valueIndex - startIndex), output);
                }
            }
        }

        private static unsafe void EncodeCore(this TextEncoder encoder, char* value, int charCount, TextWriter output)
        {
            char[] buffer = new char[encoder.MaxOutputCharsPerInputChar];  // TODO (Pri 0): it's very unfortunate we cannot stackalloc this. 

            char firstChar = *value;
            char secondChar = firstChar;
            bool wasSurrogatePair = false;
            int charsWritten;

            // this loop processes character pairs (in case they are surrogates).
            // there is an if block below to process single last character.
            for (int secondCharIndex = 1; secondCharIndex < charCount; secondCharIndex++)
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

                if (!encoder.Encodes(firstChar))
                {
                    wasSurrogatePair = false;
                    output.Write(firstChar); 
                }
                else
                {
                    int nextScalar = UnicodeHelpers.GetScalarValueFromUtf16(firstChar, secondChar, out wasSurrogatePair);
                    if (!encoder.TryEncodeUnicodeScalar(nextScalar, buffer, 0, out charsWritten))
                    {
                        throw new NotImplementedException("this should never happen");
                    }
                    output.Write(buffer, 0, charsWritten); // TODO: We could make the buffer larger and write to output only if buffer is close to full

                    if (wasSurrogatePair)
                    {
                        secondCharIndex++;
                    }
                }
            }

            if (!wasSurrogatePair)
            {
                firstChar = value[charCount - 1];
                int nextScalar = UnicodeHelpers.GetScalarValueFromUtf16(firstChar, null, out wasSurrogatePair);
                if (!encoder.TryEncodeUnicodeScalar(nextScalar, buffer, 0, out charsWritten))
                {
                    throw new NotImplementedException("this should never happen");
                }
                output.Write(buffer, 0, charsWritten);
            }
        }

        private static void ValidateRanges(int startIndex, int charCount, int actualInputLength)
        {
            if (startIndex < 0 || startIndex > actualInputLength)
            {
                throw new ArgumentOutOfRangeException("startIndex");
            }
            if (charCount < 0 || charCount > (actualInputLength - startIndex))
            {
                throw new ArgumentOutOfRangeException("charCount");
            }
        }

        internal static bool TryCopyCharacters(this char[] source, char[] destination, int destinationIndex, out int writtenChars)
        {
            if (destination.Length - destinationIndex < source.Length)
            {
                writtenChars = 0;
                return false;
            }

            if (source.Length < 8)
            {
                for (int i = 0; i < source.Length; i++)
                {
                    destination[destinationIndex + i] = source[i];
                }
            }
            else
            {
                source.CopyTo(destination, destinationIndex);
            }
            writtenChars = source.Length;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool TryWriteScalarAsChar(this int unicodeScalar, char[] buffer, int index, out int writtenChars)
        {
            if (buffer.Length - index < 1)
            {
                writtenChars = 0;
                return false;
            }
            buffer[index] = (char)unicodeScalar; // TODO: this assumes allowed scalars are chars; is that ok in all cases?
            writtenChars = 1;
            return true;
        }
    }
}
