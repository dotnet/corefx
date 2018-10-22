// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text.RegularExpressions
{
    internal ref struct ParserMin
    {
        private readonly RegexOptions _options;
        private InputWalker _pattern;

        public int Position => _pattern.Position;

        public ParserMin(ReadOnlySpan<char> pattern, RegexOptions options = RegexOptions.None)
        {
            _options = options;
            _pattern = new InputWalker(pattern);
        }

        /// <summary>
        /// Scans \ code for escape codes that map to single Unicode chars.
        /// </summary>
        public char ScanCharEscape()
        {
            char ch = _pattern.RightCharMoveRight();

            if (ch >= '0' && ch <= '7')
            {
                _pattern.MoveLeft();
                return ScanOctal();
            }

            switch (ch)
            {
                case 'x':
                    return ScanHex(2);
                case 'u':
                    return ScanHex(4);
                case 'a':
                    return '\u0007';
                case 'b':
                    return '\b';
                case 'e':
                    return '\u001B';
                case 'f':
                    return '\f';
                case 'n':
                    return '\n';
                case 'r':
                    return '\r';
                case 't':
                    return '\t';
                case 'v':
                    return '\u000B';
                case 'c':
                    return ScanControl();
                default:
                    if (!_options.UseOptionE() && RegexCharClass.IsWordChar(ch))
                        throw MakeException(RegexParseError.UnrecognizedEscape, SR.Format(SR.UnrecognizedEscape, ch));
                    return ch;
            }
        }

        /// <summary>
        /// Returns n <= 0xF for a hex digit.
        /// </summary>
        private static int HexDigit(char ch)
        {
            int d;

            if ((uint)(d = ch - '0') <= 9)
                return d;

            if (unchecked((uint)(d = ch - 'a')) <= 5)
                return d + 0xa;

            if ((uint)(d = ch - 'A') <= 5)
                return d + 0xa;

            return -1;
        }

        /// <summary>
        /// Scans exactly c hex digits (c=2 for \xFF, c=4 for \uFFFF)
        /// </summary>
        private char ScanHex(int c)
        {
            int i = 0;
            int d;

            if (_pattern.CharsRight() >= c)
            {
                for (; c > 0 && ((d = HexDigit(_pattern.RightCharMoveRight())) >= 0); c -= 1)
                {
                    i *= 0x10;
                    i += d;
                }
            }

            if (c > 0)
                throw MakeException(RegexParseError.TooFewHex, SR.TooFewHex);

            return (char)i;
        }

        /// <summary>
        /// Grabs and converts an ASCII control character.
        /// </summary>
        private char ScanControl()
        {
            if (_pattern.CharsRight() == 0)
                throw MakeException(RegexParseError.MissingControl, SR.MissingControl);

            char ch = _pattern.RightCharMoveRight();

            // \ca interpreted as \cA

            if (ch >= 'a' && ch <= 'z')
                ch = (char)(ch - ('a' - 'A'));

            if (unchecked(ch = (char)(ch - '@')) < ' ')
                return ch;

            throw MakeException(RegexParseError.UnrecognizedControl, SR.UnrecognizedControl);
        }

        /// <summary>
        /// Scans up to three octal digits (stops before exceeding 0377).
        /// </summary>
        private char ScanOctal()
        {
            // Consume octal chars only up to 3 digits and value 0377
            int c = 3;
            int d;
            int i;

            if (c > _pattern.CharsRight())
                c = _pattern.CharsRight();

            for (i = 0; c > 0 && unchecked((uint)(d = _pattern.RightChar() - '0')) <= 7; c -= 1)
            {
                _pattern.MoveRight();
                i *= 8;
                i += d;
                if (_options.UseOptionE() && i >= 0x20)
                    break;
            }

            // Octal codes only go up to 255.  Any larger and the behavior that Perl follows
            // is simply to truncate the high bits.
            i &= 0xFF;

            return (char)i;
        }

        /// <summary>
        /// Zaps to a specific parsing position.
        /// </summary>
        /// <param name="position"></param>
        public void AdvanceTo(int position)
        {
            _pattern.Textto(position);
        }

        /// <summary>
        /// Fills in a RegexParseException
        /// </summary>
        private RegexParseException MakeException(RegexParseError error, string message)
        {
            return new RegexParseException(error, _pattern.Position, SR.Format(SR.MakeException, _pattern.Input.ToString(), _pattern.Position, message));
        }
    }
}
