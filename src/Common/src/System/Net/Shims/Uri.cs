// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System
{
    internal partial class UriShim
    {
        public static readonly string UriSchemeHttp = "http";
        public static readonly string UriSchemeHttps = "https";
        public static readonly string UriSchemeWs = "ws";
        public static readonly string UriSchemeWss = "wss";

        public static readonly string SchemeDelimiter = "://";

        //
        // IsHexDigit
        //
        //  Determines whether a character is a valid hexadecimal digit in the range
        //  [0..9] | [A..F] | [a..f]
        //
        // Inputs:
        //  <argument>  character
        //      Character to test
        //
        // Returns:
        //  true if <character> is a hexadecimal digit character
        //
        // Throws:
        //  Nothing
        //
        public static bool IsHexDigit(char character)
        {
            return ((character >= '0') && (character <= '9'))
                || ((character >= 'A') && (character <= 'F'))
                || ((character >= 'a') && (character <= 'f'));
        }

        //
        // Returns:
        //  Number in the range 0..15
        //
        // Throws:
        //  ArgumentException
        //
        public static int FromHex(char digit)
        {
            if (((digit >= '0') && (digit <= '9'))
                || ((digit >= 'A') && (digit <= 'F'))
                || ((digit >= 'a') && (digit <= 'f')))
            {
                return (digit <= '9')
                    ? ((int)digit - (int)'0')
                    : (((digit <= 'F')
                    ? ((int)digit - (int)'A')
                    : ((int)digit - (int)'a'))
                    + 10);
            }
            throw new ArgumentException("digit");
        }       
    }
}
