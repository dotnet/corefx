// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace System.Text.Unicode
{
    /// <summary>
    /// Contains helpers for dealing with Unicode code points.
    /// </summary>
    internal static unsafe class UnicodeHelpers
    {
        /// <summary>
        /// Used for invalid Unicode sequences or other unrepresentable values.
        /// </summary>
        private const char UNICODE_REPLACEMENT_CHAR = '\uFFFD';

        /// <summary>
        /// The last code point defined by the Unicode specification.
        /// </summary>
        internal const int UNICODE_LAST_CODEPOINT = 0x10FFFF;

        private static uint[] _definedCharacterBitmap;

        /// <summary>
        /// Helper method which creates a bitmap of all characters which are
        /// defined per the Unicode specification.
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static uint[] CreateDefinedCharacterBitmap()
        {
            // The stream should be exactly 8KB in size.
            var stream = typeof(UnicodeRange).GetTypeInfo().Assembly.GetManifestResourceStream("System.Text.Encodings.Web.Resources.unicode8definedcharacters.bin");

            if (stream == null)
            {
                throw new BadImageFormatException();
            }

            if (stream.Length != 8 * 1024)
            {
                Environment.FailFast("Corrupt data detected.");
            }

            // Read everything in as raw bytes.
            byte[] rawData = new byte[8 * 1024];
            for (int numBytesReadTotal = 0; numBytesReadTotal < rawData.Length;)
            {
                int numBytesReadThisIteration = stream.Read(rawData, numBytesReadTotal, rawData.Length - numBytesReadTotal);
                if (numBytesReadThisIteration == 0)
                {
                    Environment.FailFast("Corrupt data detected.");
                }
                numBytesReadTotal += numBytesReadThisIteration;
            }

            // Finally, convert the byte[] to a uint[].
            // The incoming bytes are little-endian.
            uint[] retVal = new uint[2 * 1024];
            for (int i = 0; i < retVal.Length; i++)
            {
                retVal[i] = (((uint)rawData[4 * i + 3]) << 24)
                    | (((uint)rawData[4 * i + 2]) << 16)
                    | (((uint)rawData[4 * i + 1]) << 8)
                    | (uint)rawData[4 * i];
            }

            // And we're done!
            Volatile.Write(ref _definedCharacterBitmap, retVal);
            return retVal;
        }

        /// <summary>
        /// Returns a bitmap of all characters which are defined per version 7.0.0
        /// of the Unicode specification.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static uint[] GetDefinedCharacterBitmap()
        {
            return Volatile.Read(ref _definedCharacterBitmap) ?? CreateDefinedCharacterBitmap();
        }

        /// <summary>
        /// Returns a value stating whether a character is defined per version 7.0.0
        /// of the Unicode specification. Certain classes of characters (control chars,
        /// private use, surrogates, some whitespace) are considered "undefined" for
        /// our purposes.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsCharacterDefined(char c)
        {
            uint codePoint = (uint)c;
            int index = (int)(codePoint >> 5);
            int offset = (int)(codePoint & 0x1FU);
            return ((GetDefinedCharacterBitmap()[index] >> offset) & 0x1U) != 0;
        }
    }
}
