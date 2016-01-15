// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

using Microsoft.Win32.SafeHandles;

namespace Internal.Cryptography.Pal
{
    internal static class X500NameEncoder
    {
        private const string OidTagPrefix = "OID.";

        private static readonly char[] s_quoteNeedingChars =
        {
            ',',
            '+',
            '=',
            '\"',
            '\n',
            // \r is NOT in this list, because it isn't in Windows.
            '<',
            '>',
            '#',
            ';',
        };

        private static readonly List<char> s_useSemicolonSeparators = new List<char>(1) { ';' };
        private static readonly List<char> s_useCommaSeparators = new List<char>(1) { ',' };
        private static readonly List<char> s_useNewlineSeparators = new List<char>(2) { '\r', '\n' };
        private static readonly List<char> s_defaultSeparators = new List<char>(2) { ',', ';' };

        internal static string X500DistinguishedNameDecode(
            byte[] encodedName,
            bool printOid,
            X500DistinguishedNameFlags flags,
            bool addTrailingDelimieter=false)
        {
            bool reverse = (flags & X500DistinguishedNameFlags.Reversed) == X500DistinguishedNameFlags.Reversed;
            bool quoteIfNeeded = (flags & X500DistinguishedNameFlags.DoNotUseQuotes) != X500DistinguishedNameFlags.DoNotUseQuotes;
            string dnSeparator;

            if ((flags & X500DistinguishedNameFlags.UseSemicolons) == X500DistinguishedNameFlags.UseSemicolons)
            {
                dnSeparator = "; ";
            }
            else if ((flags & X500DistinguishedNameFlags.UseNewLines) == X500DistinguishedNameFlags.UseNewLines)
            {
                dnSeparator = Environment.NewLine;
            }
            else
            {
                // This is matching Windows (native) behavior, UseCommas does not need to be asserted,
                // it is just what happens if neither UseSemicolons nor UseNewLines is specified.
                dnSeparator = ", ";
            }

            using (SafeX509NameHandle x509Name = Interop.Crypto.DecodeX509Name(encodedName, encodedName.Length))
            {
                if (x509Name.IsInvalid)
                {
                    return "";
                }

                // We need to allocate a StringBuilder to hold the data as we're building it, and there's the usual
                // arbitrary process of choosing a number that's "big enough" to minimize reallocations without wasting
                // too much space in the average case.
                //
                // So, let's look at an example of what our output might be.
                //
                // GitHub.com's SSL cert has a "pretty long" subject (partially due to the unknown OIDs):
                //   businessCategory=Private Organization
                //   1.3.6.1.4.1.311.60.2.1.3=US
                //   1.3.6.1.4.1.311.60.2.1.2=Delaware
                //   serialNumber=5157550
                //   street=548 4th Street
                //   postalCode=94107
                //   C=US
                //   ST=California
                //   L=San Francisco
                //   O=GitHub, Inc.
                //   CN=github.com
                //
                // Which comes out to 228 characters using OpenSSL's default pretty-print
                // (openssl x509 -in github.cer -text -noout)
                // Throw in some "maybe-I-need-to-quote-this" quotes, and a couple of extra/extra-long O/OU values
                // and round that up to the next programmer number, and you get that 512 should avoid reallocations
                // in all but the most dire of cases.
                StringBuilder decodedName = new StringBuilder(512);
                int entryCount = Interop.Crypto.GetX509NameEntryCount(x509Name);
                bool printSpacing = false;

                for (int i = 0; i < entryCount; i++)
                {
                    int loc = reverse ? entryCount - i - 1 : i;

                    using (SafeSharedX509NameEntryHandle nameEntry = Interop.Crypto.GetX509NameEntry(x509Name, loc))
                    {
                        Interop.Crypto.CheckValidOpenSslHandle(nameEntry);

                        string thisOidValue;

                        using (SafeSharedAsn1ObjectHandle oidHandle = Interop.Crypto.GetX509NameEntryOid(nameEntry))
                        {
                            thisOidValue = Interop.Crypto.GetOidValue(oidHandle);
                        }

                        if (printSpacing)
                        {
                            decodedName.Append(dnSeparator);
                        }
                        else
                        {
                            printSpacing = true;
                        }

                        if (printOid)
                        {
                            AppendOid(decodedName, thisOidValue);
                        }

                        string rdnValue;

                        using (SafeSharedAsn1StringHandle valueHandle = Interop.Crypto.GetX509NameEntryData(nameEntry))
                        {
                            rdnValue = Interop.Crypto.Asn1StringToManagedString(valueHandle);
                        }

                        bool quote = quoteIfNeeded && NeedsQuoting(rdnValue);

                        if (quote)
                        {
                            decodedName.Append('"');

                            // If the RDN itself had a quote within it, that quote needs to be escaped
                            // with another quote.
                            rdnValue = rdnValue.Replace("\"", "\"\"");
                        }

                        decodedName.Append(rdnValue);

                        if (quote)
                        {
                            decodedName.Append('"');
                        }
                    }
                }

                if (addTrailingDelimieter)
                {
                    decodedName.Append(dnSeparator);
                }

                return decodedName.ToString();
            }
        }

        internal static byte[] X500DistinguishedNameEncode(
            string stringForm,
            X500DistinguishedNameFlags flags)
        {
            bool reverse = (flags & X500DistinguishedNameFlags.Reversed) == X500DistinguishedNameFlags.Reversed;
            bool noQuotes = (flags & X500DistinguishedNameFlags.DoNotUseQuotes) == X500DistinguishedNameFlags.DoNotUseQuotes;

            List<char> dnSeparators;

            // This rank ordering is based off of testing against the Windows implementation.
            if ((flags & X500DistinguishedNameFlags.UseSemicolons) == X500DistinguishedNameFlags.UseSemicolons)
            {
                // Just semicolon.
                dnSeparators = s_useSemicolonSeparators;
            }
            else if ((flags & X500DistinguishedNameFlags.UseCommas) == X500DistinguishedNameFlags.UseCommas)
            {
                // Just comma
                dnSeparators = s_useCommaSeparators;
            }
            else if ((flags & X500DistinguishedNameFlags.UseNewLines) == X500DistinguishedNameFlags.UseNewLines)
            {
                // CR or LF.  Not "and".  Whichever is first was the separator, the later one is trimmed as whitespace.
                dnSeparators = s_useNewlineSeparators;
            }
            else
            {
                // Comma or semicolon, but not CR or LF.
                dnSeparators = s_defaultSeparators;
            }

            Debug.Assert(dnSeparators.Count != 0);

            List<byte[][]> encodedSets = ParseDistinguishedName(stringForm, dnSeparators, noQuotes);

            if (reverse)
            {
                encodedSets.Reverse();
            }

            return DerEncoder.ConstructSequence(encodedSets);
        }

        private static bool NeedsQuoting(string rdnValue)
        {
            if (string.IsNullOrEmpty(rdnValue))
            {
                return true;
            }

            if (IsQuotableWhitespace(rdnValue[0]) ||
                IsQuotableWhitespace(rdnValue[rdnValue.Length - 1]))
            {
                return true;
            }

            int index = rdnValue.IndexOfAny(s_quoteNeedingChars);

            return index != -1;
        }

        private static bool IsQuotableWhitespace(char c)
        {
            // There's a whole lot of Unicode whitespace that isn't covered here; but this
            // matches what Windows deems quote-worthy.
            //
            // 0x20: Space
            // 0x09: Character Tabulation (tab)
            // 0x0A: Line Feed
            // 0x0B: Line Tabulation (vertical tab)
            // 0x0C: Form Feed
            // 0x0D: Carriage Return
            return (c == ' ' || (c >= 0x09 && c <= 0x0D));
        }

        private static void AppendOid(StringBuilder decodedName, string oidValue)
        {
            Oid oid = new Oid(oidValue);

            if (StringComparer.Ordinal.Equals(oid.FriendlyName, oidValue) ||
                string.IsNullOrEmpty(oid.FriendlyName))
            {
                decodedName.Append(OidTagPrefix);
                decodedName.Append(oid.Value);
            }
            else
            {
                decodedName.Append(oid.FriendlyName);
            }

            decodedName.Append('=');
        }

        private enum ParseState
        {
            Invalid,
            SeekTag,
            SeekTagEnd,
            SeekEquals,
            SeekValueStart,
            SeekValueEnd,
            SeekEndQuote,
            MaybeEndQuote,
            SeekComma,
        }

        private static List<byte[][]> ParseDistinguishedName(
            string stringForm,
            List<char> dnSeparators,
            bool noQuotes)
        {
            // 16 is way more RDNs than we should ever need. A fairly standard set of values is
            // { E, CN, O, OU, L, S, C } = 7;
            // The EV values add in
            // {
            //   STREET, PostalCode, SERIALNUMBER, 2.5.4.15,
            //   1.3.6.1.4.1.311.60.2.1.2, 1.3.6.1.4.1.311.60.2.1.3
            // } = 6
            //
            // 7 + 6 = 13, round up to the nearest power-of-two.
            const int InitalRdnSize = 16;
            List<byte[][]> encodedSets = new List<byte[][]>(InitalRdnSize);
            char[] chars = stringForm.ToCharArray();

            int pos;
            int end = chars.Length;

            int tagStart = -1;
            int tagEnd = -1;
            Oid tagOid = null;
            int valueStart = -1;
            int valueEnd = -1;
            bool hadEscapedQuote = false;

            const char KeyValueSeparator = '=';
            const char QuotedValueChar = '"';

            ParseState state = ParseState.SeekTag;

            for (pos = 0; pos < end; pos++)
            {
                char c = chars[pos];

                switch (state)
                {
                    case ParseState.SeekTag:
                        if (char.IsWhiteSpace(c))
                        {
                            continue;
                        }

                        if (char.IsControl(c))
                        {
                            state = ParseState.Invalid;
                            break;
                        }

                        // The first character in the tag start.
                        // We know that there's at least one valid
                        // character, so make end be start+1.
                        //
                        // Single letter values with no whitespace padding them
                        // (e.g. E=) would otherwise be ambiguous with length.
                        // (SeekEquals can't set the tagEnd value because it
                        // doesn't know if it was preceded by whitespace)

                        // Note that we make no check here for the dnSeparator(s).
                        // Two separators in a row is invalid (except for UseNewlines,
                        // and they are only allowed because they are whitespace).
                        //
                        // But the throw for an invalid value will come from when the
                        // OID fails to encode.
                        tagStart = pos;
                        tagEnd = pos + 1;
                        state = ParseState.SeekTagEnd;
                        break;

                    case ParseState.SeekTagEnd:
                        if (c == KeyValueSeparator)
                        {
                            goto case ParseState.SeekEquals;
                        }

                        if (char.IsWhiteSpace(c))
                        {
                            // Tag values aren't permitted whitespace, but there
                            // can be whitespace between the tag and the separator.
                            state = ParseState.SeekEquals;
                            break;
                        }

                        if (char.IsControl(c))
                        {
                            state = ParseState.Invalid;
                            break;
                        }

                        // We found another character in the tag, so move the
                        // end (non-inclusive) to the next character.
                        tagEnd = pos + 1;
                        break;

                    case ParseState.SeekEquals:
                        if (c == KeyValueSeparator)
                        {
                            Debug.Assert(tagStart >= 0);
                            tagOid = ParseOid(stringForm, tagStart, tagEnd);
                            tagStart = -1;

                            state = ParseState.SeekValueStart;
                            break;
                        }

                        if (!char.IsWhiteSpace(c))
                        {
                            state = ParseState.Invalid;
                            break;
                        }

                        break;

                    case ParseState.SeekValueStart:
                        if (char.IsWhiteSpace(c))
                        {
                            continue;
                        }

                        // If the first non-whitespace character is a quote,
                        // this is a quoted string.  Unless the flags say to
                        // not interpret quoted strings.
                        if (c == QuotedValueChar && !noQuotes)
                        {
                            state = ParseState.SeekEndQuote;
                            valueStart = pos + 1;
                            break;
                        }

                        // It's possible to just write "CN=,O=". So we might
                        // run into the RDN separator here.
                        if (dnSeparators.Contains(c))
                        {
                            valueStart = pos;
                            valueEnd = pos;
                            goto case ParseState.SeekComma;
                        }

                        state = ParseState.SeekValueEnd;
                        valueStart = pos;
                        valueEnd = pos + 1;
                        break;

                    case ParseState.SeekEndQuote:
                        // The only escape sequence in DN parsing is that a quoted
                        // value can embed quotes via "", the same as a C# verbatim
                        // string.  So, if we see a quote while looking for a close
                        // quote we need to remember that this might have been the
                        // end, but be open to the possibility that there's another
                        // quote coming.
                        if (c == QuotedValueChar)
                        {
                            state = ParseState.MaybeEndQuote;
                            valueEnd = pos;
                            break;
                        }

                        // Everything else is okay.
                        break;

                    case ParseState.MaybeEndQuote:
                        if (c == QuotedValueChar)
                        {
                            state = ParseState.SeekEndQuote;
                            hadEscapedQuote = true;
                            valueEnd = -1;
                            break;
                        }

                        // If the character wasn't another quote:
                        //   dnSeparator: process value, state transition to SeekTag
                        //   whitespace: state transition to SeekComma
                        //   anything else: invalid.
                        // since that's the same table as SeekComma, just change state
                        // and go there.
                        state = ParseState.SeekComma;
                        goto case ParseState.SeekComma;

                    case ParseState.SeekValueEnd:
                        // Every time we see a non-whitespace character we need to mark it
                        if (dnSeparators.Contains(c))
                        {
                            goto case ParseState.SeekComma;
                        }

                        if (char.IsWhiteSpace(c))
                        {
                            continue;
                        }

                        // Including control characters.
                        valueEnd = pos + 1;

                        break;

                    case ParseState.SeekComma:
                        if (dnSeparators.Contains(c))
                        {
                            Debug.Assert(tagOid != null);
                            Debug.Assert(valueEnd != -1);
                            Debug.Assert(valueStart != -1);

                            encodedSets.Add(ParseRdn(tagOid, chars, valueStart, valueEnd, hadEscapedQuote));
                            tagOid = null;
                            valueStart = -1;
                            valueEnd = -1;
                            state = ParseState.SeekTag;
                            break;
                        }

                        if (!char.IsWhiteSpace(c))
                        {
                            state = ParseState.Invalid;
                            break;
                        }

                        break;

                    default:
                        Debug.Fail(
                            string.Format(
                                "Invalid parser state. Position {0}, State {1}, Character {2}, String \"{3}\"",
                                pos,
                                state,
                                c,
                                stringForm));

                        throw new CryptographicException(SR.Cryptography_Invalid_X500Name);
                }

                if (state == ParseState.Invalid)
                {
                    break;
                }
            }

            // Okay, so we've run out of input.  There are a couple of valid states we can be in.
            // * 'CN='
            //   state: SeekValueStart.  Neither valueStart nor valueEnd has a value yet.
            // * 'CN=a'
            //   state: SeekValueEnd.  valueEnd was set to pos(a) + 1, close it off.
            // * 'CN=a '
            //   state: SeekValueEnd.  valueEnd is marking at the start of the whitespace.
            // * 'CN="a"'
            //   state: MaybeEndQuote.  valueEnd is marking at the end-quote.
            // * 'CN="a" '
            //   state: SeekComma.  This is the same as MaybeEndQuote.
            // * 'CN=a,'
            //   state: SeekTag.  There's nothing to do here.
            // * ''
            //   state: SeekTag.  There's nothing to do here.
            //
            // And, of course, invalid ones.
            // * 'CN="'
            //   state: SeekEndQuote.  Throw.
            // * 'CN':
            //   state: SeekEndTag.  Throw.
            switch (state)
            {
                // The last semantic character parsed was =.
                case ParseState.SeekValueStart:
                    valueStart = chars.Length;
                    valueEnd = valueStart;
                    goto case ParseState.SeekComma;

                // If we were in an unquoted value and just ran out of text
                case ParseState.SeekValueEnd:
                    Debug.Assert(!hadEscapedQuote);
                    goto case ParseState.SeekComma;

                // If the last character was a close quote, or it was a close quote
                // then some whitespace.
                case ParseState.MaybeEndQuote:
                case ParseState.SeekComma:
                    Debug.Assert(tagOid != null);
                    Debug.Assert(valueStart != -1);
                    Debug.Assert(valueEnd != -1);

                    encodedSets.Add(ParseRdn(tagOid, chars, valueStart, valueEnd, hadEscapedQuote));
                    break;

                // If the entire string was empty, or ended in a dnSeparator.
                case ParseState.SeekTag:
                    break;

                default:
                    // While this is an error, it should be due to bad input, so no Debug.Fail.
                    throw new CryptographicException(SR.Cryptography_Invalid_X500Name);
            }

            return encodedSets;
        }

        private static Oid ParseOid(string stringForm, int tagStart, int tagEnd)
        {
            int length = tagEnd - tagStart;

            if (length > OidTagPrefix.Length)
            {
                // Since we only care if the match starts exactly at tagStart, tell IndexOf
                // that we're only examining OidTagPrefix.Length characters.  So it won't do
                // more than one linear equality check.
                int prefixIndex = stringForm.IndexOf(
                    OidTagPrefix,
                    tagStart,
                    OidTagPrefix.Length,
                    StringComparison.OrdinalIgnoreCase);

                if (prefixIndex == tagStart)
                {
                    return new Oid(stringForm.Substring(tagStart + OidTagPrefix.Length, length - OidTagPrefix.Length));
                }
            }

            return new Oid(stringForm.Substring(tagStart, length));
        }

        private static byte[][] ParseRdn(Oid tagOid, char[] chars, int valueStart, int valueEnd, bool hadEscapedQuote)
        {
            bool ia5String = (tagOid.Value == Oids.EmailAddress);
            byte[][] encodedOid;

            try
            {
                encodedOid = DerEncoder.SegmentedEncodeOid(tagOid);
            }
            catch (CryptographicException e)
            {
                throw new CryptographicException(SR.Cryptography_Invalid_X500Name, e);
            }

            if (hadEscapedQuote)
            {
                char[] value = ExtractValue(chars, valueStart, valueEnd);

                return ParseRdn(encodedOid, value, ia5String);
            }

            return ParseRdn(encodedOid, chars, valueStart, valueEnd, ia5String);
        }

        private static byte[][] ParseRdn(
            byte[][] encodedOid,
            char[] chars,
            int valueStart,
            int valueEnd,
            bool ia5String)
        {
            byte[][] encodedValue;

            int length = valueEnd - valueStart;

            if (ia5String)
            {
                // An email address with an invalid value will throw.
                encodedValue = DerEncoder.SegmentedEncodeIA5String(chars, valueStart, length);
            }
            else if (DerEncoder.IsValidPrintableString(chars, valueStart, length))
            {
                encodedValue = DerEncoder.SegmentedEncodePrintableString(chars, valueStart, length);
            }
            else
            {
                encodedValue = DerEncoder.SegmentedEncodeUtf8String(chars, valueStart, length);
            }

            return DerEncoder.ConstructSegmentedSet(
                DerEncoder.ConstructSegmentedSequence(
                    encodedOid,
                    encodedValue));
        }

        private static byte[][] ParseRdn(byte[][] encodedOid, char[] value, bool ia5String)
        {
            byte[][] encodedValue;

            if (ia5String)
            {
                // An email address with an invalid value will throw.
                encodedValue = DerEncoder.SegmentedEncodeIA5String(value);
            }
            else if (DerEncoder.IsValidPrintableString(value))
            {
                encodedValue = DerEncoder.SegmentedEncodePrintableString(value);
            }
            else
            {
                encodedValue = DerEncoder.SegmentedEncodeUtf8String(value);
            }

            return DerEncoder.ConstructSegmentedSet(
                DerEncoder.ConstructSegmentedSequence(
                    encodedOid,
                    encodedValue));
        }

        private static char[] ExtractValue(char[] chars, int valueStart, int valueEnd)
        {
            // The string is guaranteed to be between ((valueEnd - valueStart) / 2) (all quotes) and
            // (valueEnd - valueStart - 1) (one escaped quote)
            List<char> builder = new List<char>(valueEnd - valueStart - 1);

            bool skippedQuote = false;

            for (int i = valueStart; i < valueEnd; i++)
            {
                char c = chars[i];

                if (c == '"' && !skippedQuote)
                {
                    skippedQuote = true;
                    continue;
                }

                // If we just skipped a quote, this will be one.
                // If this is a quote, we should have just skipped one.
                Debug.Assert(skippedQuote == (c == '"'));

                skippedQuote = false;
                builder.Add(c);
            }

            return builder.ToArray();
        }
    }
}
