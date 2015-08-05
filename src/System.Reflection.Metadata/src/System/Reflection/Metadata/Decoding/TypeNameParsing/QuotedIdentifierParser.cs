// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Reflection.Metadata.Decoding
{
    internal static class QuotedIdentifierParser
    {
        public static string Parse(StringReader reader, bool required)
        {
            Tokenizer tokenizer = new Tokenizer(reader, Delimiters.QuotedIdentifier);

            // Consume '"'
            tokenizer.Skip(TokenType.Quote);

            // Within a quoted identifier, all leading/trailing white space is treated as significant
            string identifier = tokenizer.ReadId(required ? IdentifierOptions.Required : IdentifierOptions.None);

            // Consume '"'
            tokenizer.Skip(TokenType.Quote);

            return identifier;
        }
    }
}
