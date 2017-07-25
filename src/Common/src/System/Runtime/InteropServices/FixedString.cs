// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Runtime.InteropServices
{
    /// <summary>
    /// Contains definitions for various fixed size strings for creating blittable
    /// structs. Provides easy string property access. Set strings are always null
    /// terminated and will truncate if too large.
    /// 
    /// Usage: Instead of "fixed char _buffer[12]" use "FixedString.Size12 _buffer"
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal unsafe struct FixedString
    {
        // We cant derive structs in C#, this is the next best thing. Nested
        // class/struct defines have visibility to the nesting class/struct
        // privates, and given the pointer manipulation we must do we can
        // leverage it for a similar effect. It isn't perfect, but it reduces
        // copying of the riskier blocks of code.

        private const int BaseSize = 1;
        private char _firstChar;

        private string GetString(int maxSize)
        {
            fixed (char* c = &_firstChar)
            {
                // Go to null or end of buffer
                int end = 0;
                while (end < maxSize && c[end] != (char)0)
                    end++;
                return new string(c, 0, end);
            }
        }

        private bool Equals(string value, int maxSize)
        {
            if (value == null || value.Length > maxSize)
                return false;

            fixed (char* c = &_firstChar)
            {
                int i = 0;
                for (; i < value.Length; i++)
                {
                    // Fixed strings are always terminated at null
                    // and therefore can never match embedded nulls.
                    if (value[i] != c[i] || value[i] == '\0')
                        return false;
                }

                // If we've maxed out the buffer or reached the
                // null terminator, we're equal.
                return i == maxSize || c[i] == '\0';
            }
        }

        private void SetString(string value, int maxSize)
        {
            fixed (char* c = &_firstChar)
                StringToBuffer(value, c, maxSize - 1, nullTerminate: true);
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct Size14
        {
            private const int Size = 14;
            private FixedString _buffer;
            private unsafe fixed char _bufferExtension[Size - BaseSize];

            public string Value
            {
                get => _buffer.GetString(Size);
                set => _buffer.SetString(value, Size);
            }

            public override string ToString() => Value;
            public bool Equals(string value) => _buffer.Equals(value, Size);
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct Size260
        {
            private const int Size = 260;
            private FixedString _buffer;
            private unsafe fixed char _bufferExtension[Size - BaseSize];

            public string Value
            {
                get => _buffer.GetString(Size);
                set => _buffer.SetString(value, Size);
            }

            public override string ToString() => Value;
            public bool Equals(string value) => _buffer.Equals(value, Size);
        }

        /// <summary>
        /// Copy up to the specified number of characters to the designated buffer with an
        /// additional null terminator if not otherwise specified.
        /// </summary>
        /// <param name="value">The string to copy from.</param>
        /// <param name="destination">The buffer to copy to.</param>
        /// <param name="maxCharacters">Max number of characters to copy.</param>
        /// <param name="nullTerminate">Add a null to the end of the string (not counted in <paramref name="maxCharacters"/>).</param>
        public unsafe static void StringToBuffer(string value, char* destination, int maxCharacters, bool nullTerminate = true)
        {
            int count = maxCharacters;
            if (count == 0 || value == null || value.Length == 0)
            {
                if (nullTerminate)
                    *destination = '\0';
                return;
            }

            if (count < 0)
                count = value.Length;
            else if (value.Length < count)
                count = value.Length;

            fixed (char* start = value)
            {
                Buffer.MemoryCopy(
                    source: start,
                    destination: destination,
                    destinationSizeInBytes: count * sizeof(char),
                    sourceBytesToCopy: count * sizeof(char)
                );
            }

            if (nullTerminate)
            {
                destination += count = '\0';
            }
        }
    }
}
