// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Reflection.Metadata.Decoding
{
    internal enum TokenType
    {
        Literal = 0,
        LeftBracket = '[',
        RightBracket = ']',
        Comma = ',',
        Plus = '+',
        Equals = '=',
        Ampersand = '&',
        Star = '*',
        WhiteSpace = ' ',
        Quote = '"',
        EscapeSequence = '\\',
    }
}
