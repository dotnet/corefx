// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net.Mime;
using System.Diagnostics;
using System.Text;

namespace System.Net.Mail
{
    //
    // RFC 2822 Section 3.4.1 - Addr-Spec, Domain-Literals
    // A domain literal is a domain identifier that does not conform to the dot-atom format (Section 3.2.4) and must be
    // enclosed in brackets '[' ']'.  Domain literals may contain quoted-pairs.
    //
    internal static class DomainLiteralReader
    {
        //
        // Reads a domain literal in reverse
        // 
        // Preconditions: 
        //  - Index must be within the bounds of the data string.
        //  - The char at the given index is the initial bracket. (data[index] == EndSquareBracket)
        //
        // Return value: 
        // - The next index past the terminating bracket (data[index + 1] == StartSquareBracket). 
        //   e.g. In (user@[domain]), starting at index=12 (]) returns index=4 (@).
        //
        // A FormatException will be thrown if:
        // - A non-escaped character is encountered that is not valid in a domain literal, including Unicode.
        // - The final bracket is not found.
        //
        internal static int ReadReverse(string data, int index)
        {
            Debug.Assert(0 <= index && index < data.Length, "index was outside the bounds of the string: " + index);
            Debug.Assert(data[index] == MailBnfHelper.EndSquareBracket, "data did not end with a square bracket");

            // Skip the end bracket
            index--;

            do
            {
                // Check for valid whitespace
                index = WhitespaceReader.ReadFwsReverse(data, index);
                if (index < 0)
                {
                    break;
                }
                // Check for escaped characters
                int quotedCharCount = QuotedPairReader.CountQuotedChars(data, index, false);
                if (quotedCharCount > 0)
                {
                    // Skip quoted pairs
                    index = index - quotedCharCount;
                }
                // Check for the terminating bracket
                else if (data[index] == MailBnfHelper.StartSquareBracket)
                {
                    // We're done parsing
                    return index - 1;
                }
                // Check for invalid characters
                else if (data[index] > MailBnfHelper.Ascii7bitMaxValue || !MailBnfHelper.Dtext[data[index]])
                {
                    throw new FormatException(SR.Format(SR.MailHeaderFieldInvalidCharacter, data[index]));
                }
                // Valid char
                else
                {
                    index--;
                }
            }
            while (index >= 0);

            // We didn't find a matching '[', throw.
            throw new FormatException(SR.Format(SR.MailHeaderFieldInvalidCharacter,
                MailBnfHelper.EndSquareBracket));
        }
    }
}
