// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

////////////////////////////////////////////////////////////////////////////
//
//
//  Purpose:  This Class defines behaviors specific to a writing system.
//            A writing system is the collection of scripts and
//            orthographic rules required to represent a language as text.
//
//
////////////////////////////////////////////////////////////////////////////

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace System.Globalization
{
    public partial class TextInfo : ICloneable, IDeserializationCallback
    {
        private enum Tristate : byte
        {
            NotInitialized,
            True,
            False,
        }

        private string _listSeparator;
        private bool _isReadOnly = false;

        /*    _cultureName is the name of the creating culture.
              _cultureData is the data that backs this class.
              _textInfoName is the actual name of the textInfo (from cultureData.STEXTINFO)
                      In the desktop, when we call the sorting dll, it doesn't
                      know how to resolve custom locle names to sort ids so we have to have already resolved this.
        */

        private readonly string _cultureName;      // Name of the culture that created this text info
        private readonly CultureData _cultureData; // Data record for the culture that made us, not for this textinfo
        private readonly string _textInfoName;     // Name of the text info we're using (ie: _cultureData.STEXTINFO)

        private Tristate _isAsciiCasingSameAsInvariant = Tristate.NotInitialized;

        // _invariantMode is defined for the perf reason as accessing the instance field is faster than access the static property GlobalizationMode.Invariant
        private readonly bool _invariantMode = GlobalizationMode.Invariant;

        // Invariant text info
        internal static TextInfo Invariant
        {
            get
            {
                if (s_Invariant == null)
                    s_Invariant = new TextInfo(CultureData.Invariant);
                return s_Invariant;
            }
        }
        internal volatile static TextInfo s_Invariant;

        //////////////////////////////////////////////////////////////////////////
        ////
        ////  TextInfo Constructors
        ////
        ////  Implements CultureInfo.TextInfo.
        ////
        //////////////////////////////////////////////////////////////////////////
        internal TextInfo(CultureData cultureData)
        {
            // This is our primary data source, we don't need most of the rest of this
            _cultureData = cultureData;
            _cultureName = _cultureData.CultureName;
            _textInfoName = _cultureData.STEXTINFO;

            FinishInitialization();
        }

        void IDeserializationCallback.OnDeserialization(object sender)
        {
            throw new PlatformNotSupportedException();
        }

        public virtual int ANSICodePage => _cultureData.IDEFAULTANSICODEPAGE;

        public virtual int OEMCodePage => _cultureData.IDEFAULTOEMCODEPAGE;

        public virtual int MacCodePage => _cultureData.IDEFAULTMACCODEPAGE;

        public virtual int EBCDICCodePage => _cultureData.IDEFAULTEBCDICCODEPAGE;

        // Just use the LCID from our text info name
        public int LCID => CultureInfo.GetCultureInfo(_textInfoName).LCID;

        public string CultureName => _textInfoName;

        public bool IsReadOnly => _isReadOnly;

        //////////////////////////////////////////////////////////////////////////
        ////
        ////  Clone
        ////
        ////  Is the implementation of ICloneable.
        ////
        //////////////////////////////////////////////////////////////////////////
        public virtual object Clone()
        {
            object o = MemberwiseClone();
            ((TextInfo)o).SetReadOnlyState(false);
            return o;
        }

        ////////////////////////////////////////////////////////////////////////
        //
        //  ReadOnly
        //
        //  Create a cloned readonly instance or return the input one if it is 
        //  readonly.
        //
        ////////////////////////////////////////////////////////////////////////
        public static TextInfo ReadOnly(TextInfo textInfo)
        {
            if (textInfo == null) { throw new ArgumentNullException(nameof(textInfo)); }
            if (textInfo.IsReadOnly) { return textInfo; }

            TextInfo clonedTextInfo = (TextInfo)(textInfo.MemberwiseClone());
            clonedTextInfo.SetReadOnlyState(true);

            return clonedTextInfo;
        }

        private void VerifyWritable()
        {
            if (_isReadOnly)
            {
                throw new InvalidOperationException(SR.InvalidOperation_ReadOnly);
            }
        }

        internal void SetReadOnlyState(bool readOnly)
        {
            _isReadOnly = readOnly;
        }


        ////////////////////////////////////////////////////////////////////////
        //
        //  ListSeparator
        //
        //  Returns the string used to separate items in a list.
        //
        ////////////////////////////////////////////////////////////////////////
        public virtual string ListSeparator
        {
            get
            {
                if (_listSeparator == null)
                {
                    _listSeparator = _cultureData.SLIST;
                }
                return _listSeparator;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value), SR.ArgumentNull_String);
                }
                VerifyWritable();
                _listSeparator = value;
            }
        }

        ////////////////////////////////////////////////////////////////////////
        //
        //  ToLower
        //
        //  Converts the character or string to lower case.  Certain locales
        //  have different casing semantics from the file systems in Win32.
        //
        ////////////////////////////////////////////////////////////////////////
        public unsafe virtual char ToLower(char c)
        {
            if (_invariantMode || (IsAscii(c) && IsAsciiCasingSameAsInvariant))
            {
                return ToLowerAsciiInvariant(c);
            }

            return ChangeCase(c, toUpper: false);
        }

        public unsafe virtual string ToLower(string str)
        {
            if (str == null) { throw new ArgumentNullException(nameof(str)); }

            if (_invariantMode)
            {
                return ToLowerAsciiInvariant(str);
            }

            return ChangeCase(str, toUpper: false);
        }

        private unsafe char ChangeCase(char c, bool toUpper)
        {
            Debug.Assert(!_invariantMode);
            
            char dst = default;
            ChangeCase(&c, 1, &dst, 1, toUpper);
            return dst;
        }

        private unsafe string ChangeCase(string source, bool toUpper)
        {
            Debug.Assert(!_invariantMode);
            Debug.Assert(source != null);

            // If the string is empty, we're done.
            if (source.Length == 0)
            {
                return string.Empty;
            }

            int sourcePos = 0;
            string result = null;

            // If this culture's casing for ASCII is the same as invariant, try to take
            // a fast path that'll work in managed code and ASCII rather than calling out
            // to the OS for culture-aware casing.
            if (IsAsciiCasingSameAsInvariant)
            {
                if (toUpper)
                {
                    // Loop through each character.
                    for (sourcePos = 0; sourcePos < source.Length; sourcePos++)
                    {
                        // If the character is lower-case, we're going to need to allocate a string.
                        char c = source[sourcePos];
                        if ((uint)(c - 'a') <= 'z' - 'a')
                        {
                            // Allocate the result string.
                            result = string.FastAllocateString(source.Length);
                            fixed (char* pResult = result)
                            {
                                // Store all of characters examined thus far.
                                if (sourcePos > 0)
                                {
                                    source.AsSpan(0, sourcePos).CopyTo(new Span<char>(pResult, sourcePos));
                                }

                                // And store the current character, upper-cased.
                                char* d = pResult + sourcePos;
                                *d++ = (char)(c & ~0x20);
                                sourcePos++;

                                // Then continue looping through the remainder of the characters. If we hit
                                // a non-ASCII character, bail to fall back to culture-aware casing.
                                for (; sourcePos < source.Length; sourcePos++)
                                {
                                    c = source[sourcePos];
                                    if ((uint)(c - 'a') <= 'z' - 'a')
                                    {
                                        *d++ = (char)(c & ~0x20);
                                    }
                                    else if (!IsAscii(c))
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        *d++ = c;
                                    }
                                }
                            }

                            break;
                        }
                        else if (!IsAscii(c))
                        {
                            // The character isn't ASCII; bail to fall back to a culture-aware casing.
                            break;
                        }
                    }
                }
                else // toUpper == false
                {
                    // Loop through each character.
                    for (sourcePos = 0; sourcePos < source.Length; sourcePos++)
                    {
                        // If the character is upper-case, we're going to need to allocate a string.
                        char c = source[sourcePos];
                        if ((uint)(c - 'A') <= 'Z' - 'A')
                        {
                            // Allocate the result string.
                            result = string.FastAllocateString(source.Length);
                            fixed (char* pResult = result)
                            {
                                // Store all of characters examined thus far.
                                if (sourcePos > 0)
                                {
                                    source.AsSpan(0, sourcePos).CopyTo(new Span<char>(pResult, sourcePos));
                                }

                                // And store the current character, lower-cased.
                                char* d = pResult + sourcePos;
                                *d++ = (char)(c | 0x20);
                                sourcePos++;

                                // Then continue looping through the remainder of the characters. If we hit
                                // a non-ASCII character, bail to fall back to culture-aware casing.
                                for (; sourcePos < source.Length; sourcePos++)
                                {
                                    c = source[sourcePos];
                                    if ((uint)(c - 'A') <= 'Z' - 'A')
                                    {
                                       *d++ = (char)(c | 0x20);
                                    }
                                    else if (!IsAscii(c))
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        *d++ = c;
                                    }
                                }
                            }

                            break;
                        }
                        else if (!IsAscii(c))
                        {
                            // The character isn't ASCII; bail to fall back to a culture-aware casing.
                            break;
                        }
                    }
                }

                // If we successfully iterated through all of the characters, we didn't need to fall back
                // to culture-aware casing.  In that case, if we allocated a result string, use it, otherwise
                // just return the original string, as no modifications were necessary.
                if (sourcePos == source.Length)
                {
                    return result ?? source;
                }
            }

            // Falling back to culture-aware casing.  Make sure we have a result string to write into.
            // If we need to allocate the result string, we'll also need to copy over to it any
            // characters already examined.
            if (result == null)
            {
                result = string.FastAllocateString(source.Length);
                if (sourcePos > 0)
                {
                    fixed (char* pResult = result)
                    {
                        source.AsSpan(0, sourcePos).CopyTo(new Span<char>(pResult, sourcePos));
                    }
                }
            }

            // Do the casing operation on everything after what we already processed.
            fixed (char* pSource = source)
            {
                fixed (char* pResult = result)
                {
                    ChangeCase(pSource + sourcePos, source.Length - sourcePos, pResult + sourcePos, result.Length - sourcePos, toUpper);
                }
            }

            return result;
        }

        internal unsafe void ChangeCase(ReadOnlySpan<char> source, Span<char> destination, bool toUpper)
        {
            Debug.Assert(!_invariantMode);
            Debug.Assert(destination.Length >= source.Length);

            if (source.IsEmpty)
            {
                return;
            }

            fixed (char* pSource = &MemoryMarshal.GetReference(source))
            fixed (char* pResult = &MemoryMarshal.GetReference(destination))
            {
                if (IsAsciiCasingSameAsInvariant)
                {
                    int length = 0;
                    char* a = pSource, b = pResult;
                    if (toUpper)
                    {
                        while (length < source.Length && *a < 0x80)
                        {
                            *b++ = ToUpperAsciiInvariant(*a++);
                            length++;
                        }
                    }
                    else
                    {
                        while (length < source.Length && *a < 0x80)
                        {
                            *b++ = ToLowerAsciiInvariant(*a++);
                            length++;
                        }
                    }

                    if (length != source.Length)
                    {
                        ChangeCase(a, source.Length - length, b, destination.Length - length, toUpper);
                    }
                }
                else
                {
                    ChangeCase(pSource, source.Length, pResult, destination.Length, toUpper);
                }
            }
        }

        private static unsafe string ToLowerAsciiInvariant(string s)
        {
            if (s.Length == 0)
            {
                return string.Empty;
            }
            
            fixed (char* pSource = s)
            {
                int i = 0;
                while (i < s.Length)
                {
                    if ((uint)(pSource[i] - 'A') <= (uint)('Z' - 'A'))
                    {
                        break;
                    }
                    i++;
                }
                
                if (i >= s.Length)
                {
                    return s;
                }

                string result = string.FastAllocateString(s.Length);
                fixed (char* pResult = result)
                {
                    for (int j = 0; j < i; j++)
                    {
                        pResult[j] = pSource[j];
                    }
                    
                    pResult[i] = (char)(pSource[i] | 0x20);
                    i++;

                    while (i < s.Length)
                    {
                        pResult[i] = ToLowerAsciiInvariant(pSource[i]);
                        i++;
                    }
                }

                return result;
            }
        }

        internal static void ToLowerAsciiInvariant(ReadOnlySpan<char> source, Span<char> destination)
        {
            Debug.Assert(destination.Length >= source.Length);

            for (int i = 0; i < source.Length; i++)
            {
                destination[i] = ToLowerAsciiInvariant(source[i]);
            }
        }

        private static unsafe string ToUpperAsciiInvariant(string s)
        {
            if (s.Length == 0)
            {
                return string.Empty;
            }
            
            fixed (char* pSource = s)
            {
                int i = 0;
                while (i < s.Length)
                {
                    if ((uint)(pSource[i] - 'a') <= (uint)('z' - 'a'))
                    {
                        break;
                    }
                    i++;
                }
                
                if (i >= s.Length)
                {
                    return s;
                }

                string result = string.FastAllocateString(s.Length);
                fixed (char* pResult = result)
                {
                    for (int j = 0; j < i; j++)
                    {
                        pResult[j] = pSource[j];
                    }
                    
                    pResult[i] = (char)(pSource[i] & ~0x20);
                    i++;

                    while (i < s.Length)
                    {
                        pResult[i] = ToUpperAsciiInvariant(pSource[i]);
                        i++;
                    }
                }

                return result;
            }
        }

        internal static void ToUpperAsciiInvariant(ReadOnlySpan<char> source, Span<char> destination)
        {
            Debug.Assert(destination.Length >= source.Length);

            for (int i = 0; i < source.Length; i++)
            {
                destination[i] = ToUpperAsciiInvariant(source[i]);
            }
        }

        private static char ToLowerAsciiInvariant(char c)
        {
            if ((uint)(c - 'A') <= (uint)('Z' - 'A'))
            {
                c = (char)(c | 0x20);
            }
            return c;
        }

        ////////////////////////////////////////////////////////////////////////
        //
        //  ToUpper
        //
        //  Converts the character or string to upper case.  Certain locales
        //  have different casing semantics from the file systems in Win32.
        //
        ////////////////////////////////////////////////////////////////////////
        public unsafe virtual char ToUpper(char c)
        {
            if (_invariantMode || (IsAscii(c) && IsAsciiCasingSameAsInvariant))
            {
                return ToUpperAsciiInvariant(c);
            }
            
            return ChangeCase(c, toUpper: true);
        }

        public unsafe virtual string ToUpper(string str)
        {
            if (str == null) { throw new ArgumentNullException(nameof(str)); }

            if (_invariantMode)
            {
                return ToUpperAsciiInvariant(str);
            }

            return ChangeCase(str, toUpper: true);
        }

        internal static char ToUpperAsciiInvariant(char c)
        {
            if ((uint)(c - 'a') <= (uint)('z' - 'a'))
            {
                c = (char)(c & ~0x20);
            }
            return c;
        }

        private static bool IsAscii(char c)
        {
            return c < 0x80;
        }

        private bool IsAsciiCasingSameAsInvariant
        {
            get
            {
                if (_isAsciiCasingSameAsInvariant == Tristate.NotInitialized)
                {
                    _isAsciiCasingSameAsInvariant = CultureInfo.GetCultureInfo(_textInfoName).CompareInfo.Compare("abcdefghijklmnopqrstuvwxyz",
                                                                             "ABCDEFGHIJKLMNOPQRSTUVWXYZ",
                                                                             CompareOptions.IgnoreCase) == 0 ? Tristate.True : Tristate.False;
                }
                return _isAsciiCasingSameAsInvariant == Tristate.True;
            }
        }

        // IsRightToLeft
        //
        // Returns true if the dominant direction of text and UI such as the relative position of buttons and scroll bars
        //
        public bool IsRightToLeft => _cultureData.IsRightToLeft;

        ////////////////////////////////////////////////////////////////////////
        //
        //  Equals
        //
        //  Implements Object.Equals().  Returns a boolean indicating whether
        //  or not object refers to the same CultureInfo as the current instance.
        //
        ////////////////////////////////////////////////////////////////////////
        public override bool Equals(object obj)
        {
            TextInfo that = obj as TextInfo;

            if (that != null)
            {
                return CultureName.Equals(that.CultureName);
            }

            return false;
        }

        ////////////////////////////////////////////////////////////////////////
        //
        //  GetHashCode
        //
        //  Implements Object.GetHashCode().  Returns the hash code for the
        //  CultureInfo.  The hash code is guaranteed to be the same for CultureInfo A
        //  and B where A.Equals(B) is true.
        //
        ////////////////////////////////////////////////////////////////////////
        public override int GetHashCode()
        {
            return CultureName.GetHashCode();
        }

        ////////////////////////////////////////////////////////////////////////
        //
        //  ToString
        //
        //  Implements Object.ToString().  Returns a string describing the
        //  TextInfo.
        //
        ////////////////////////////////////////////////////////////////////////
        public override string ToString()
        {
            return "TextInfo - " + _cultureData.CultureName;
        }

        //
        // Titlecasing:
        // -----------
        // Titlecasing refers to a casing practice wherein the first letter of a word is an uppercase letter
        // and the rest of the letters are lowercase.  The choice of which words to titlecase in headings
        // and titles is dependent on language and local conventions.  For example, "The Merry Wives of Windor"
        // is the appropriate titlecasing of that play's name in English, with the word "of" not titlecased.
        // In German, however, the title is "Die lustigen Weiber von Windsor," and both "lustigen" and "von"
        // are not titlecased.  In French even fewer words are titlecased: "Les joyeuses commeres de Windsor."
        //
        // Moreover, the determination of what actually constitutes a word is language dependent, and this can
        // influence which letter or letters of a "word" are uppercased when titlecasing strings.  For example
        // "l'arbre" is considered two words in French, whereas "can't" is considered one word in English.
        //
        public unsafe string ToTitleCase(string str)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }
            if (str.Length == 0)
            {
                return str;
            }

            StringBuilder result = new StringBuilder();
            string lowercaseData = null;
            // Store if the current culture is Dutch (special case)
            bool isDutchCulture = CultureName.StartsWith("nl-", StringComparison.OrdinalIgnoreCase);

            for (int i = 0; i < str.Length; i++)
            {
                UnicodeCategory charType;
                int charLen;

                charType = CharUnicodeInfo.InternalGetUnicodeCategory(str, i, out charLen);
                if (char.CheckLetter(charType))
                {
                    // Special case to check for Dutch specific titlecasing with "IJ" characters 
                    // at the beginning of a word
                    if (isDutchCulture && i < str.Length - 1 && (str[i] == 'i' || str[i] == 'I') && (str[i+1] == 'j' || str[i+1] == 'J'))
                    {
                        result.Append("IJ");
                        i += 2;
                    }
                    else
                    {
                        // Do the titlecasing for the first character of the word.
                        i = AddTitlecaseLetter(ref result, ref str, i, charLen) + 1;
                    }

                    //
                    // Convert the characters until the end of the this word
                    // to lowercase.
                    //
                    int lowercaseStart = i;

                    //
                    // Use hasLowerCase flag to prevent from lowercasing acronyms (like "URT", "USA", etc)
                    // This is in line with Word 2000 behavior of titlecasing.
                    //
                    bool hasLowerCase = (charType == UnicodeCategory.LowercaseLetter);
                    // Use a loop to find all of the other letters following this letter.
                    while (i < str.Length)
                    {
                        charType = CharUnicodeInfo.InternalGetUnicodeCategory(str, i, out charLen);
                        if (IsLetterCategory(charType))
                        {
                            if (charType == UnicodeCategory.LowercaseLetter)
                            {
                                hasLowerCase = true;
                            }
                            i += charLen;
                        }
                        else if (str[i] == '\'')
                        {
                            i++;
                            if (hasLowerCase)
                            {
                                if (lowercaseData == null)
                                {
                                    lowercaseData = ToLower(str);
                                }
                                result.Append(lowercaseData, lowercaseStart, i - lowercaseStart);
                            }
                            else
                            {
                                result.Append(str, lowercaseStart, i - lowercaseStart);
                            }
                            lowercaseStart = i;
                            hasLowerCase = true;
                        }
                        else if (!IsWordSeparator(charType))
                        {
                            // This category is considered to be part of the word.
                            // This is any category that is marked as false in wordSeprator array.
                            i+= charLen;
                        }
                        else
                        {
                            // A word separator. Break out of the loop.
                            break;
                        }
                    }

                    int count = i - lowercaseStart;

                    if (count > 0)
                    {
                        if (hasLowerCase)
                        {
                            if (lowercaseData == null)
                            {
                                lowercaseData = ToLower(str);
                            }
                            result.Append(lowercaseData, lowercaseStart, count);
                        }
                        else
                        {
                            result.Append(str, lowercaseStart, count);
                        }
                    }

                    if (i < str.Length)
                    {
                        // not a letter, just append it
                        i = AddNonLetter(ref result, ref str, i, charLen);
                    }
                }
                else
                {
                    // not a letter, just append it
                    i = AddNonLetter(ref result, ref str, i, charLen);
                }
            }
            return result.ToString();
        }

        private static int AddNonLetter(ref StringBuilder result, ref string input, int inputIndex, int charLen)
        {
            Debug.Assert(charLen == 1 || charLen == 2, "[TextInfo.AddNonLetter] CharUnicodeInfo.InternalGetUnicodeCategory returned an unexpected charLen!");
            if (charLen == 2)
            {
                // Surrogate pair
                result.Append(input[inputIndex++]);
                result.Append(input[inputIndex]);
            }
            else
            {
                result.Append(input[inputIndex]);
            }
            return inputIndex;
        }

        private int AddTitlecaseLetter(ref StringBuilder result, ref string input, int inputIndex, int charLen)
        {
            Debug.Assert(charLen == 1 || charLen == 2, "[TextInfo.AddTitlecaseLetter] CharUnicodeInfo.InternalGetUnicodeCategory returned an unexpected charLen!");

            if (charLen == 2)
            {
                // for surrogate pairs do a ToUpper operation on the substring
                ReadOnlySpan<char> src = input.AsSpan(inputIndex, 2);
                if (_invariantMode)
                {
                    result.Append(src); // surrogate pair in invariant mode, so changing case is a nop
                }
                else
                {
                    Span<char> dst = stackalloc char[2];
                    ChangeCase(src, dst, toUpper: true);
                    result.Append(dst);
                }
                inputIndex++;
            }
            else
            {
                switch (input[inputIndex])
                {
                    //
                    // For AppCompat, the Titlecase Case Mapping data from NDP 2.0 is used below.
                    case (char) 0x01C4:  // DZ with Caron -> Dz with Caron
                    case (char) 0x01C5:  // Dz with Caron -> Dz with Caron
                    case (char) 0x01C6:  // dz with Caron -> Dz with Caron
                        result.Append((char) 0x01C5);
                        break;
                    case (char) 0x01C7:  // LJ -> Lj
                    case (char) 0x01C8:  // Lj -> Lj
                    case (char) 0x01C9:  // lj -> Lj
                        result.Append((char) 0x01C8);
                        break;
                    case (char) 0x01CA:  // NJ -> Nj
                    case (char) 0x01CB:  // Nj -> Nj
                    case (char) 0x01CC:  // nj -> Nj
                        result.Append((char) 0x01CB);
                        break;
                    case (char) 0x01F1:  // DZ -> Dz
                    case (char) 0x01F2:  // Dz -> Dz
                    case (char) 0x01F3:  // dz -> Dz
                        result.Append((char) 0x01F2);
                        break;
                    default:
                        result.Append(ToUpper(input[inputIndex]));
                        break;
                }
            }
            return inputIndex;
        }

        //
        // Used in ToTitleCase():
        // When we find a starting letter, the following array decides if a category should be
        // considered as word seprator or not.
        //
        private const int c_wordSeparatorMask = 
            /* false */ (0 <<  0) | // UppercaseLetter = 0,
            /* false */ (0 <<  1) | // LowercaseLetter = 1,
            /* false */ (0 <<  2) | // TitlecaseLetter = 2,
            /* false */ (0 <<  3) | // ModifierLetter = 3,
            /* false */ (0 <<  4) | // OtherLetter = 4,
            /* false */ (0 <<  5) | // NonSpacingMark = 5,
            /* false */ (0 <<  6) | // SpacingCombiningMark = 6,
            /* false */ (0 <<  7) | // EnclosingMark = 7,
            /* false */ (0 <<  8) | // DecimalDigitNumber = 8,
            /* false */ (0 <<  9) | // LetterNumber = 9,
            /* false */ (0 << 10) | // OtherNumber = 10,
            /* true  */ (1 << 11) | // SpaceSeparator = 11,
            /* true  */ (1 << 12) | // LineSeparator = 12,
            /* true  */ (1 << 13) | // ParagraphSeparator = 13,
            /* true  */ (1 << 14) | // Control = 14,
            /* true  */ (1 << 15) | // Format = 15,
            /* false */ (0 << 16) | // Surrogate = 16,
            /* false */ (0 << 17) | // PrivateUse = 17,
            /* true  */ (1 << 18) | // ConnectorPunctuation = 18,
            /* true  */ (1 << 19) | // DashPunctuation = 19,
            /* true  */ (1 << 20) | // OpenPunctuation = 20,
            /* true  */ (1 << 21) | // ClosePunctuation = 21,
            /* true  */ (1 << 22) | // InitialQuotePunctuation = 22,
            /* true  */ (1 << 23) | // FinalQuotePunctuation = 23,
            /* true  */ (1 << 24) | // OtherPunctuation = 24,
            /* true  */ (1 << 25) | // MathSymbol = 25,
            /* true  */ (1 << 26) | // CurrencySymbol = 26,
            /* true  */ (1 << 27) | // ModifierSymbol = 27,
            /* true  */ (1 << 28) | // OtherSymbol = 28,
            /* false */ (0 << 29);  // OtherNotAssigned = 29;
        
        private static bool IsWordSeparator(UnicodeCategory category) 
        {
            return (c_wordSeparatorMask & (1 << (int) category)) != 0;
        }

        private static bool IsLetterCategory(UnicodeCategory uc)
        {
            return (uc == UnicodeCategory.UppercaseLetter
                 || uc == UnicodeCategory.LowercaseLetter
                 || uc == UnicodeCategory.TitlecaseLetter
                 || uc == UnicodeCategory.ModifierLetter
                 || uc == UnicodeCategory.OtherLetter);
        }
    }
}
