// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Reflection.Metadata.Decoding
{
    internal enum TokenType
    {
        Literal = 0,
        LeftBracket = (int)'[',
        RightBracket = (int)']',
        Comma = (int)',',
        Plus = (int)'+',
        Equals = (int)'=',
        Ampersand = (int)'&',
        Star = (int)'*',
        WhiteSpace = (int)' ',
        Quote = (int)'"',
        EscapeSequence = (int)'\\',
    }
}
