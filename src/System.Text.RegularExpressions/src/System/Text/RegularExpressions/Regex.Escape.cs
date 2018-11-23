// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;

namespace System.Text.RegularExpressions
{
    public partial class Regex
    {
        private const int EscapeMaxBufferSize = 256;

        /// <summary>
        /// Escapes a minimal set of metacharacters (\, *, +, ?, |, {, [, (, ), ^, $, ., #, and
        /// whitespace) by replacing them with their \ codes. This converts a string so that
        /// it can be used as a constant within a regular expression safely. (Note that the
        /// reason # and whitespace must be escaped is so the string can be used safely
        /// within an expression parsed with x mode. If future Regex features add
        /// additional metacharacters, developers should depend on Escape to escape those
        /// characters as well.)
        /// </summary>
        public static string Escape(string str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            return EscapeImpl(targetSpan: false, str.AsSpan(), Span<char>.Empty, out _, out _);
        }

        /// <summary>
        /// Escapes a minimal set of metacharacters (\, *, +, ?, |, {, [, (, ), ^, $, ., #, and
        /// whitespace) by replacing them with their \ codes. This converts a string so that
        /// it can be used as a constant within a regular expression safely. (Note that the
        /// reason # and whitespace must be escaped is so the string can be used safely
        /// within an expression parsed with x mode. If future Regex features add
        /// additional metacharacters, developers should depend on Escape to escape those
        /// characters as well.)
        /// </summary>
        /// <returns>Returns the amount of chars written into the output Span.</returns>
        public static bool TryEscape(ReadOnlySpan<char> str, Span<char> destination, out int charsWritten)
        {
            EscapeImpl(targetSpan: true, str, destination, out charsWritten, out bool spanSuccess);

            return spanSuccess;
        }

        /// <summary>
        /// Unescapes any escaped characters in the input string.
        /// </summary>
        public static string Unescape(string str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            return UnescapeImpl(targetSpan: false, str.AsSpan(), Span<char>.Empty, out _, out _);
        }

        /// <summary>
        /// Unescapes any escaped characters in the input text.
        /// </summary>
        public static bool TryUnescape(ReadOnlySpan<char> str, Span<char> destination, out int charsWritten)
        {
            UnescapeImpl(targetSpan: true, str, destination, out charsWritten, out bool spanSuccess);

            return spanSuccess;
        }

        /// <summary>
        /// Escapes all metacharacters (including |,(,),[,{,|,^,$,*,+,?,\, spaces and #)
        /// </summary>
        private static string EscapeImpl(bool targetSpan, ReadOnlySpan<char> input, Span<char> destination, out int charsWritten, out bool spanSuccess)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (CharCategory.IsMetachar(input[i]))
                {
                    return EscapeImpl(targetSpan, input, i, destination, out charsWritten, out spanSuccess);
                }
            }

            // If nothing to escape, return the input.
            return input.CopyInput(targetSpan, destination, out charsWritten, out spanSuccess);
        }

        private static string EscapeImpl(bool targetSpan, ReadOnlySpan<char> input, int i, Span<char> destination, out int charsWritten, out bool spanSuccess)
        {
            // For small inputs we allocate on the stack. In most cases a buffer three 
            // times larger the original string should be sufficient as usually not all 
            // characters need to be encoded.
            // For larger string we rent the input string's length plus a fixed 
            // conservative amount of chars from the ArrayPool.
            Span<char> buffer = input.Length <= (EscapeMaxBufferSize / 3) ? stackalloc char[EscapeMaxBufferSize] : default;
            ValueStringBuilder vsb = !buffer.IsEmpty ?
                new ValueStringBuilder(buffer) :
                new ValueStringBuilder(input.Length + 200);

            char ch = input[i];
            vsb.Append(input.Slice(0, i));

            do
            {
                vsb.Append('\\');
                switch (ch)
                {
                    case '\n':
                        ch = 'n';
                        break;
                    case '\r':
                        ch = 'r';
                        break;
                    case '\t':
                        ch = 't';
                        break;
                    case '\f':
                        ch = 'f';
                        break;
                }
                vsb.Append(ch);
                i++;
                int lastpos = i;

                while (i < input.Length)
                {
                    ch = input[i];
                    if (CharCategory.IsMetachar(ch))
                        break;

                    i++;
                }

                vsb.Append(input.Slice(lastpos, i - lastpos));
            } while (i < input.Length);

            return vsb.CopyOutput(targetSpan, destination, out charsWritten, out spanSuccess);
        }

        /// <summary>
        /// Unescapes all metacharacters (including (,),[,],{,},|,^,$,*,+,?,\, spaces and #)
        /// </summary>
        private static string UnescapeImpl(bool targetSpan, ReadOnlySpan<char> input, Span<char> destination, out int charsWritten, out bool spanSuccess)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '\\')
                {
                    return UnescapeImpl(targetSpan, input, i, destination, out charsWritten, out spanSuccess);
                }
            }

            // If nothing to escape, return the input.
            return input.CopyInput(targetSpan, destination, out charsWritten, out spanSuccess);
        }

        private static string UnescapeImpl(bool targetSpan, ReadOnlySpan<char> input, int i, Span<char> destination, out int charsWritten, out bool spanSuccess)
        {
            var parser = new ParserMin(input);

            // In the worst case the escaped string has the same length.
            // For small inputs we use stack allocation.
            Span<char> buffer = input.Length <= EscapeMaxBufferSize ? stackalloc char[EscapeMaxBufferSize] : default;
            ValueStringBuilder vsb = !buffer.IsEmpty ?
                new ValueStringBuilder(buffer) :
                new ValueStringBuilder(input.Length);

            vsb.Append(input.Slice(0, i));
            do
            {
                i++;
                parser.AdvanceTo(i);
                if (i < input.Length)
                {
                    vsb.Append(parser.ScanCharEscape());
                }
                i = parser.Position;

                int lastpos = i;
                while (i < input.Length && input[i] != '\\')
                    i++;
                vsb.Append(input.Slice(lastpos, i - lastpos));
            } while (i < input.Length);

            return vsb.CopyOutput(targetSpan, destination, out charsWritten, out spanSuccess);
        }
    }
}
