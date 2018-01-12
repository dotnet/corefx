// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Buffers.Text
{
    public static partial class Utf8Formatter
    {
        #region Constants

        private const byte OpenBrace = (byte)'{';
        private const byte CloseBrace = (byte)'}';

        private const byte OpenParen = (byte)'(';
        private const byte CloseParen = (byte)')';

        private const byte Dash = (byte)'-';

        #endregion Constants

        /// <summary>
        /// Formats a Guid as a UTF8 string.
        /// </summary>
        /// <param name="value">Value to format</param>
        /// <param name="buffer">Buffer to write the UTF8-formatted value to</param>
        /// <param name="bytesWritten">Receives the length of the formatted text in bytes</param>
        /// <param name="format">The standard format to use</param>
        /// <returns>
        /// true for success. "bytesWritten" contains the length of the formatted text in bytes.
        /// false if buffer was too short. Iteratively increase the size of the buffer and retry until it succeeds. 
        /// </returns>
        /// <remarks>
        /// Formats supported:
        ///     D (default)     nnnnnnnn-nnnn-nnnn-nnnn-nnnnnnnnnnnn
        ///     B               {nnnnnnnn-nnnn-nnnn-nnnn-nnnnnnnnnnnn}
        ///     P               (nnnnnnnn-nnnn-nnnn-nnnn-nnnnnnnnnnnn)
        ///     N               nnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnn
        /// </remarks>
        /// <exceptions>
        /// <cref>System.FormatException</cref> if the format is not valid for this data type.
        /// </exceptions>
        public static bool TryFormat(Guid value, Span<byte> buffer, out int bytesWritten, StandardFormat format = default)
        {
            const int INSERT_DASHES = unchecked((int)0x80000000);
            const int NO_DASHES = 0;
            const int INSERT_CURLY_BRACES = (CloseBrace << 16) | (OpenBrace << 8);
            const int INSERT_ROUND_BRACES = (CloseParen << 16) | (OpenParen << 8);
            const int NO_BRACES = 0;
            const int LEN_GUID_BASE = 32;
            const int LEN_ADD_DASHES = 4;
            const int LEN_ADD_BRACES = 2;

            // This is a 32-bit value whose contents (where 0 is the low byte) are:
            // 0th byte: minimum required length of the output buffer,
            // 1st byte: the ASCII byte to insert for the opening brace position (or 0 if no braces),
            // 2nd byte: the ASCII byte to insert for the closing brace position (or 0 if no braces),
            // 3rd byte: high bit set if dashes are to be inserted.
            // 
            // The reason for keeping a single flag instead of separate vars is that we can avoid register spillage
            // as we build up the output value.
            int flags;

            switch (FormattingHelpers.GetSymbolOrDefault(format, 'D'))
            {
                case 'D': // nnnnnnnn-nnnn-nnnn-nnnn-nnnnnnnnnnnn
                    flags = INSERT_DASHES + NO_BRACES + LEN_GUID_BASE + LEN_ADD_DASHES;
                    break;

                case 'B': // {nnnnnnnn-nnnn-nnnn-nnnn-nnnnnnnnnnnn}
                    flags = INSERT_DASHES + INSERT_CURLY_BRACES + LEN_GUID_BASE + LEN_ADD_DASHES + LEN_ADD_BRACES;
                    break;

                case 'P': // (nnnnnnnn-nnnn-nnnn-nnnn-nnnnnnnnnnnn)
                    flags = INSERT_DASHES + INSERT_ROUND_BRACES + LEN_GUID_BASE + LEN_ADD_DASHES + LEN_ADD_BRACES;
                    break;

                case 'N': // nnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnn
                    flags = NO_BRACES + NO_DASHES + LEN_GUID_BASE;
                    break;

                default:
                    return ThrowHelper.TryFormatThrowFormatException(out bytesWritten);
            }

            // At this point, the low byte of flags contains the minimum required length

            if ((byte)flags > buffer.Length)
            {
                bytesWritten = 0;
                return false;
            }

            bytesWritten = (byte)flags;
            flags >>= 8;

            // At this point, the low byte of flags contains the opening brace char (if any)

            if ((byte)flags != 0)
            {
                buffer[0] = (byte)flags;
                buffer = buffer.Slice(1);
            }
            flags >>= 8;

            // At this point, the low byte of flags contains the closing brace char (if any)
            // And since we're performing arithmetic shifting the high bit of flags is set (flags is negative) if dashes are required

            // The JIT is smart enough to elide bounds checking on accesses to guidAsBytes[00 .. 15]
            // since the Span returned by GetSpanForBlittable is known to have length 16 [= sizeof(Guid)].

            var guidAsBytes = FormattingHelpers.GetSpanForBlittable(ref value);

            // When a GUID is blitted, the first three components are little-endian, and the last component is big-endian.

            // The line below forces the JIT to hoist the bounds check for the following segment.
            // The JIT will optimize away the read, but it cannot optimize away the bounds check
            // because it may have an observeable side effect (throwing).
            // We use 8 instead of 7 so that we also capture the dash if we're asked to insert one.

            { var unused = buffer[8]; }
            FormattingHelpers.WriteHexByte(guidAsBytes[3], buffer, 0, FormattingHelpers.HexCasing.Lowercase);
            FormattingHelpers.WriteHexByte(guidAsBytes[2], buffer, 2, FormattingHelpers.HexCasing.Lowercase);
            FormattingHelpers.WriteHexByte(guidAsBytes[1], buffer, 4, FormattingHelpers.HexCasing.Lowercase);
            FormattingHelpers.WriteHexByte(guidAsBytes[0], buffer, 6, FormattingHelpers.HexCasing.Lowercase);

            if (flags < 0 /* use dash? */)
            {
                buffer[8] = Dash;
                buffer = buffer.Slice(9);
            }
            else
            {
                buffer = buffer.Slice(8);
            }

            { var unused = buffer[4]; }
            FormattingHelpers.WriteHexByte(guidAsBytes[5], buffer, 0, FormattingHelpers.HexCasing.Lowercase);
            FormattingHelpers.WriteHexByte(guidAsBytes[4], buffer, 2, FormattingHelpers.HexCasing.Lowercase);

            if (flags < 0 /* use dash? */)
            {
                buffer[4] = Dash;
                buffer = buffer.Slice(5);
            }
            else
            {
                buffer = buffer.Slice(4);
            }

            { var unused = buffer[4]; }
            FormattingHelpers.WriteHexByte(guidAsBytes[7], buffer, 0, FormattingHelpers.HexCasing.Lowercase);
            FormattingHelpers.WriteHexByte(guidAsBytes[6], buffer, 2, FormattingHelpers.HexCasing.Lowercase);

            if (flags < 0 /* use dash? */)
            {
                buffer[4] = Dash;
                buffer = buffer.Slice(5);
            }
            else
            {
                buffer = buffer.Slice(4);
            }

            { var unused = buffer[4]; }
            FormattingHelpers.WriteHexByte(guidAsBytes[8], buffer, 0, FormattingHelpers.HexCasing.Lowercase);
            FormattingHelpers.WriteHexByte(guidAsBytes[9], buffer, 2, FormattingHelpers.HexCasing.Lowercase);

            if (flags < 0 /* use dash? */)
            {
                buffer[4] = Dash;
                buffer = buffer.Slice(5);
            }
            else
            {
                buffer = buffer.Slice(4);
            }

            { var unused = buffer[11]; } // can't hoist bounds check on the final brace (if exists)
            FormattingHelpers.WriteHexByte(guidAsBytes[10], buffer, 0, FormattingHelpers.HexCasing.Lowercase);
            FormattingHelpers.WriteHexByte(guidAsBytes[11], buffer, 2, FormattingHelpers.HexCasing.Lowercase);
            FormattingHelpers.WriteHexByte(guidAsBytes[12], buffer, 4, FormattingHelpers.HexCasing.Lowercase);
            FormattingHelpers.WriteHexByte(guidAsBytes[13], buffer, 6, FormattingHelpers.HexCasing.Lowercase);
            FormattingHelpers.WriteHexByte(guidAsBytes[14], buffer, 8, FormattingHelpers.HexCasing.Lowercase);
            FormattingHelpers.WriteHexByte(guidAsBytes[15], buffer, 10, FormattingHelpers.HexCasing.Lowercase);

            if ((byte)flags != 0)
            {
                buffer[12] = (byte)flags;
            }

            return true;
        }
    }
}
