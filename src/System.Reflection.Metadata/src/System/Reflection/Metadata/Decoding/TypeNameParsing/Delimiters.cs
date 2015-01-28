// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;

namespace System.Reflection.Metadata.Decoding
{
    internal static class Delimiters
    {
        public static readonly ImmutableArray<TokenType> QuotedIdentifier                         = ImmutableArray.Create(TokenType.Quote);
        public static readonly ImmutableArray<TokenType> AssemblyName                             = ImmutableArray.Create(TokenType.Comma,       TokenType.Equals,     TokenType.Quote);
        public static readonly ImmutableArray<TokenType> AssemblyNameWithinGenericTypeArgument    = ImmutableArray.Create(TokenType.Comma,       TokenType.Equals,     TokenType.Quote,             TokenType.RightBracket);
        public static readonly ImmutableArray<TokenType> TypeName                                 = ImmutableArray.Create(TokenType.Comma,       TokenType.Plus,       TokenType.LeftBracket,       TokenType.RightBracket,       TokenType.Ampersand,       TokenType.Star);
    }
}
