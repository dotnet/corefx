// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace System.Reflection.Metadata.Decoding
{
    // Tokenizes a string into delimiters, literals and whitespace
    internal partial class Tokenizer
    {
        private readonly StringReader _reader;
        private readonly ImmutableArray<TokenType> _delimiters;

        public Tokenizer(StringReader reader, ImmutableArray<TokenType> delimiters)
        {
            Debug.Assert(reader != null);

            _reader = reader;
            _delimiters = delimiters;
        }

        public StringReader UnderlyingReader
        {
            get { return _reader; }
        }

        public TokenType? Peek(int lookAhead = 1)
        {
            Token token = PeekToken(lookAhead);
            if (token == null)
                return null;

            return token.TokenType;
        }

        public TokenType? PeekNext()
        {
            return Peek(2);
        }

        public void Skip(TokenType expected)
        {
            Token token = ReadToken();
            if (token == null)
            {
                throw FormatException(TypeNameFormatErrorId.DelimiterExpected_EncounteredEndOfString, Strings.TypeFormat_DelimiterExpected_EncounteredEndOfString, (char)expected);
            }

            if (token.TokenType != expected)
            {
                throw FormatException(TypeNameFormatErrorId.DelimiterExpected, Strings.TypeFormat_DelimiterExpected, (char)expected, token.Value);
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
            if (token != null)
                throw FormatException(TypeNameFormatErrorId.EndOfStringExpected_EncounteredExtraCharacters, Strings.TypeFormat_EndOfStringExpected_EncounteredExtraCharacters, token.Value);
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
            while ((token = PeekToken(ParseOptions.IncludeWhiteSpace)) != null)
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
            if (token == null)
            {
                throw FormatException(TypeNameFormatErrorId.IdExpected_EncounteredEndOfString, Strings.TypeFormat_IdExpected_EncounteredEndOfString);
            }

            // Otherwise, we must have hit a delimiter as whitespace will have been consumed as part of the identifier
            throw FormatException(TypeNameFormatErrorId.IdExpected_EncounteredDelimiter, Strings.TypeFormat_IdExpected_EncounteredDelimiter, token.Value);
        }

        private void CheckIdentifierAfterTrim(IdentifierOptions options, string identifier)
        {
            if (!IsValidIdentifier(options, identifier))
                throw FormatException(TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace, Strings.TypeFormat_IdExpected_EncounteredWhiteSpace);
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
            StringReader reader = _reader.Clone();

            Token token;
            while ((token = ParseTokenFrom(reader, options)) != null)
            {
                lookAhead--;
                if (lookAhead == 0)
                    break;
            }

            return token;
        }

        private Token ReadToken(ParseOptions options = ParseOptions.None)
        {
            return ParseTokenFrom(_reader, options);
        }

        private Token ParseTokenFrom(StringReader reader, ParseOptions options)
        {
            Token token = GetTokenFrom(reader);
            if (token == null)
                return null;

            token = HandleEscapeSequence(token, reader);

            if (options != ParseOptions.IncludeWhiteSpace)
            {
                token = HandleWhiteSpace(token, reader);
            }

            return token;
        }

        private Token GetTokenFrom(StringReader reader)
        {
            if (!reader.CanRead)
                return null;

            return GetToken(reader.Read());
        }

        private Token GetToken(char c)
        {
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

        private bool IsWhiteSpace(char c)
        {
            return (TokenType)c == TokenType.WhiteSpace;
        }

        private bool IsEscapeSequence(char c)
        {
            return (TokenType)c == TokenType.EscapeSequence;
        }

        private bool IsDelimiter(char c)
        {
            return _delimiters.Contains((TokenType)c);
        }

        private Token HandleEscapeSequence(Token token, StringReader reader)
        {
            // Are we looking at escape sequence?
            if (!token.IsEscapeSequence)
                return token;

            // Escape sequence must be followed by a delimiter or escape sequence
            token = GetTokenFrom(reader);
            if (token == null)
                throw FormatException(reader, TypeNameFormatErrorId.EscapedDelimiterExpected_EncounteredEndOfString, Strings.TypeFormat_EscapedDelimiterExpected_EncounteredEndOfString);

            // Next delimiter is always considered a literal
            if (!token.IsDelimiter && !token.IsEscapeSequence)
                throw FormatException(reader, TypeNameFormatErrorId.EscapedDelimiterExpected, Strings.TypeFormat_EscapedDelimiterExpected, token.Value);

            // Treat the next thing as literal
            return Token.Literal(token.Value);
        }

        private Token HandleWhiteSpace(Token token, StringReader reader)
        {
            while (token != null && token.IsWhiteSpace)
            {
                token = GetTokenFrom(reader);
            }

            return token;
        }

        public FormatException FormatException(TypeNameFormatErrorId errorId, string format, params object[] arguments)
        {
            return FormatException(_reader, errorId, format, arguments);
        }

        private static FormatException FormatException(StringReader reader, TypeNameFormatErrorId errorId, string format, params object[] arguments)
        {
            string message = String.Format(CultureInfo.CurrentCulture, format, arguments);
            string positionMessage = String.Format(CultureInfo.CurrentCulture, Strings.TypeFormat_Position, reader.Position);

            return new TypeNameFormatException(message + " " + positionMessage,
                                               errorId,
                                               reader.Position);
        }
    }
}