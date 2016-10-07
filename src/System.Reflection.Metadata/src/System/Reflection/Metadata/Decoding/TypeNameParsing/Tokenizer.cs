// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace System.Reflection.Metadata.Decoding
{
    // Tokenizes a string into delimiters, literals and whitespace
    internal partial struct Tokenizer
    {
        private readonly string _input;
        private int _position;
        private readonly ImmutableArray<TokenType> _delimiters;

        public Tokenizer(string input, int startIndex, ImmutableArray<TokenType> delimiters)
        {
            Debug.Assert(input != null);
            Debug.Assert(input.Length > 0);
            Debug.Assert(startIndex >= 0);
            Debug.Assert(startIndex <= input.Length);

            _input = input;
            _position = startIndex;
            _delimiters = delimiters;
        }

        public bool CanRead
        {
            get
            {
                if (_position < _input.Length)
                {
                    // Treat null as end of string
                    return PeekChar() != '\0';
                }

                return false;
            }
        }

        public string Input
        {
            get { return _input; }
        }

        public int Position
        {
            get { return _position; }
            set
            {
                Debug.Assert(value >= 0 && value <= _input.Length);
                _position = value;
            }
        }

        public char PeekChar()
        {
            return _input[_position];
        }

        public char ReadChar()
        {
            char c = PeekChar();

            _position++;

            return c;
        }

        public Tokenizer Clone()
        {
            return new Tokenizer(_input, _position, _delimiters);
        }

        internal Tokenizer WithDelimiters(ImmutableArray<TokenType> delimiters)
        {
            return new Tokenizer(_input, _position, delimiters);
        }

        public TokenType Peek(int lookAhead = 1)
        {
            return PeekToken(lookAhead).TokenType;
        }

        public TokenType PeekNext()
        {
            return Peek(2);
        }

        public void Skip(TokenType expected)
        {
            Token token = ReadToken();
            if (token.IsEndOfInput)
            {
                throw FormatException(TypeNameFormatErrorId.DelimiterExpected_EncounteredEndOfString, SR.TypeFormat_DelimiterExpected_EncounteredEndOfString, (char)expected);
            }

            if (token.TokenType != expected)
            {
                throw FormatException(TypeNameFormatErrorId.DelimiterExpected, SR.TypeFormat_DelimiterExpected, (char)expected, token.Value);
            }
        }

        public bool SkipIf(TokenType expected)
        {
            if (Peek() != expected)
                return false;

            Skip(expected);
            return true;
        }

        public void Close()
        {
            Token token = ReadToken();
            if (!token.IsEndOfInput)
                throw FormatException(TypeNameFormatErrorId.EndOfStringExpected_EncounteredExtraCharacters, SR.TypeFormat_EndOfStringExpected_EncounteredExtraCharacters, token.Value);
        }

        public string ReadId(IdentifierOptions options)
        {
            string identifier = ReadId();

            CheckIdentifier(options, identifier);

            if ((options & IdentifierOptions.Trim) == IdentifierOptions.Trim)
            {
                identifier = identifier.Trim((char)TokenType.WhiteSpace);
                CheckIdentifierAfterTrim(options, identifier);
            }
            else if ((options & IdentifierOptions.TrimStart) == IdentifierOptions.TrimStart)
            {
                identifier = identifier.TrimStart((char)TokenType.WhiteSpace);
                CheckIdentifierAfterTrim(options, identifier);
            }

            return identifier;
        }

        private string ReadId()
        {
            // TODO: Use a string builder cache
            StringBuilder identifier = new StringBuilder();

            Token token;
            while (!(token = PeekToken(ParseOptions.IncludeWhiteSpace)).IsEndOfInput)
            {
                if (!token.IsIdentifier)
                    break;

                ReadToken(ParseOptions.IncludeWhiteSpace);
                identifier.Append(token.Value);
            }

            return identifier.ToString();
        }

        private void CheckIdentifier(IdentifierOptions options, string identifier)
        {
            if (IsValidIdentifier(options, identifier))
                return;

            // Are we at the end of the string?
            Token token = ReadToken(ParseOptions.IncludeWhiteSpace);    // Consume token, so "position" is correct
            if (token.IsEndOfInput)
            {
                throw FormatException(TypeNameFormatErrorId.IdExpected_EncounteredEndOfString, SR.TypeFormat_IdExpected_EncounteredEndOfString);
            }

            // Otherwise, we must have hit a delimiter as whitespace will have been consumed as part of the identifier
            throw FormatException(TypeNameFormatErrorId.IdExpected_EncounteredDelimiter, SR.TypeFormat_IdExpected_EncounteredDelimiter, token.Value);
        }

        private void CheckIdentifierAfterTrim(IdentifierOptions options, string identifier)
        {
            if (!IsValidIdentifier(options, identifier))
                throw FormatException(TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace, SR.TypeFormat_IdExpected_EncounteredWhiteSpace);
        }

        private bool IsValidIdentifier(IdentifierOptions options, string identifier)
        {
            if ((options & IdentifierOptions.Required) != IdentifierOptions.Required)
                return true;

            return identifier.Length != 0;
        }

        private Token PeekToken(ParseOptions options = ParseOptions.None)
        {
            return PeekToken(1, options);
        }

        private Token PeekToken(int lookAhead, ParseOptions options = ParseOptions.None)
        {
            Tokenizer clone = Clone();

            Token token;
            while (!(token = clone.ReadToken(options)).IsEndOfInput)
            {
                lookAhead--;
                if (lookAhead == 0)
                    break;
            }

            return token;
        }

        private Token ReadToken(ParseOptions options = ParseOptions.None)
        {
            if (!CanRead)
                return Token.EndOfInput();

            Token token = GetToken();
            if (token.IsEndOfInput)
                return token;

            token = HandleEscapeSequence(token);

            if (options != ParseOptions.IncludeWhiteSpace)
            {
                token = HandleWhiteSpace(token);
            }

            return token;
        }

        private Token GetToken()
        {
            if (!CanRead)
                return Token.EndOfInput();

            char c = ReadChar();

            if (IsDelimiter(c))
            {
                return Token.Delimiter(c);
            }

            if (IsWhiteSpace(c))
            {
                return Token.WhiteSpace(c);
            }

            if (IsEscapeSequence(c))
            {
                return Token.EscapeSequence(c);
            }

            // Otherwise, must be a literal
            return Token.Literal(c);
        }

        private static bool IsWhiteSpace(char c)
        {
            return (TokenType)c == TokenType.WhiteSpace;
        }

        private static bool IsEscapeSequence(char c)
        {
            return (TokenType)c == TokenType.EscapeSequence;
        }

        private bool IsDelimiter(char c)
        {
            return _delimiters.Contains((TokenType)c);
        }

        private Token HandleEscapeSequence(Token token)
        {
            // Are we looking at escape sequence?
            if (!token.IsEscapeSequence)
                return token;

            // Escape sequence must be followed by a delimiter or escape sequence
            token = GetToken();
            if (token.IsEndOfInput)
                throw FormatException(TypeNameFormatErrorId.EscapedDelimiterExpected_EncounteredEndOfString, SR.TypeFormat_EscapedDelimiterExpected_EncounteredEndOfString);

            // Next delimiter is always considered a literal
            if (!token.IsDelimiter && !token.IsEscapeSequence)
                throw FormatException(TypeNameFormatErrorId.EscapedDelimiterExpected, SR.TypeFormat_EscapedDelimiterExpected, token.Value);

            // Treat the next thing as literal
            return Token.Literal(token.Value);
        }

        private Token HandleWhiteSpace(Token token)
        {
            while (!token.IsEndOfInput && token.IsWhiteSpace)
            {
                token = GetToken();
            }

            return token;
        }

        public FormatException FormatException(TypeNameFormatErrorId errorId, string format, params object[] arguments)
        {
            string message = String.Format(CultureInfo.CurrentCulture, format, arguments);
            string positionMessage = String.Format(CultureInfo.CurrentCulture, SR.TypeFormat_Position, _position);

            return new TypeNameFormatException(message + " " + positionMessage,
                                               errorId,
                                               _position);
        }
    }
}