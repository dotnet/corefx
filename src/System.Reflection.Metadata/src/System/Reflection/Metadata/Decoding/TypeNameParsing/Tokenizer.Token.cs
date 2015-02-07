// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace System.Reflection.Metadata.Decoding
{
    internal partial class Tokenizer
    {
        // Represents a self-contained unit within a tokenized string
        private class Token
        {
            private readonly char _value;
            private readonly TokenType _tokenType;

            private Token(char value, TokenType tokenType)
            {
                Debug.Assert(value != '\0');

                _value = value;
                _tokenType = tokenType;
            }

            public bool IsDelimiter
            {
                get
                {
                    switch (_tokenType)
                    {
                        case TokenType.Ampersand:
                        case TokenType.Comma:
                        case TokenType.Equals:
                        case TokenType.LeftBracket:
                        case TokenType.Plus:
                        case TokenType.RightBracket:
                        case TokenType.Star:
                        case TokenType.Quote:
                            return true;
                    }

                    Debug.Assert(IsWhiteSpace || IsLiteral || IsEscapeSequence);
                    return false;
                }
            }

            public bool IsIdentifier
            {
                get { return IsWhiteSpace || IsLiteral; }
            }

            public bool IsWhiteSpace
            {
                get { return _tokenType == TokenType.WhiteSpace; }
            }

            public bool IsLiteral
            {
                get { return _tokenType == TokenType.Literal; }
            }

            public bool IsEscapeSequence
            {
                get { return _tokenType == TokenType.EscapeSequence; }
            }

            public char Value
            {
                get { return _value; }
            }

            public TokenType TokenType
            {
                get { return _tokenType; }
            }

            public static Token Literal(char value)
            {
                return new Token(value, TokenType.Literal);
            }

            public static Token Delimiter(char value)
            {
                return new Token(value, (TokenType)value);
            }

            public static Token WhiteSpace(char value)
            {
                return new Token(value, TokenType.WhiteSpace);
            }

            public static Token EscapeSequence(char value)
            {
                return new Token(value, TokenType.EscapeSequence);
            }
        }
    }
}
