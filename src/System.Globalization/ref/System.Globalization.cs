// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------


namespace System.Globalization
{
    public static partial class CharUnicodeInfo
    {
        public static double GetNumericValue(char ch) { return default(double); }
        public static double GetNumericValue(string s, int index) { return default(double); }
        public static System.Globalization.UnicodeCategory GetUnicodeCategory(char ch) { return default(System.Globalization.UnicodeCategory); }
        public static System.Globalization.UnicodeCategory GetUnicodeCategory(string s, int index) { return default(System.Globalization.UnicodeCategory); }
    }
    public partial class CultureNotFoundException : System.ArgumentException
    {
        public CultureNotFoundException() { }
        public CultureNotFoundException(string message) { }
        public CultureNotFoundException(string message, System.Exception innerException) { }
        public CultureNotFoundException(string paramName, string message) { }
        public CultureNotFoundException(string message, string invalidCultureName, System.Exception innerException) { }
        public CultureNotFoundException(string paramName, string invalidCultureName, string message) { }
        public virtual string InvalidCultureName { get { return default(string); } }
        public override string Message { get { return default(string); } }
    }

    public partial class RegionInfo
    {
        public RegionInfo(string name) { }
        public virtual string CurrencySymbol { get { return default(string); } }
        public static System.Globalization.RegionInfo CurrentRegion { get { return default(System.Globalization.RegionInfo); } }
        public virtual string DisplayName { get { return default(string); } }
        public virtual string EnglishName { get { return default(string); } }
        public virtual bool IsMetric { get { return default(bool); } }
        public virtual string ISOCurrencySymbol { get { return default(string); } }
        public virtual string Name { get { return default(string); } }
        public virtual string NativeName { get { return default(string); } }
        public virtual string TwoLetterISORegionName { get { return default(string); } }
        public override bool Equals(object value) { return default(bool); }
        public override int GetHashCode() { return default(int); }
        public override string ToString() { return default(string); }
    }
    public partial class StringInfo
    {
        public StringInfo() { }
        public StringInfo(string value) { }
        public int LengthInTextElements { get { return default(int); } }
        public string String { get { return default(string); } set { } }
        public override bool Equals(object value) { return default(bool); }
        public override int GetHashCode() { return default(int); }
        public static string GetNextTextElement(string str) { return default(string); }
        public static string GetNextTextElement(string str, int index) { return default(string); }
        public static System.Globalization.TextElementEnumerator GetTextElementEnumerator(string str) { return default(System.Globalization.TextElementEnumerator); }
        public static System.Globalization.TextElementEnumerator GetTextElementEnumerator(string str, int index) { return default(System.Globalization.TextElementEnumerator); }
        public static int[] ParseCombiningCharacters(string str) { return default(int[]); }
    }
    public partial class TextElementEnumerator : System.Collections.IEnumerator
    {
        internal TextElementEnumerator() { }
        public object Current { get { return default(object); } }
        public int ElementIndex { get { return default(int); } }
        public string GetTextElement() { return default(string); }
        public bool MoveNext() { return default(bool); }
        public void Reset() { }
    }
    public enum UnicodeCategory
    {
        ClosePunctuation = 21,
        ConnectorPunctuation = 18,
        Control = 14,
        CurrencySymbol = 26,
        DashPunctuation = 19,
        DecimalDigitNumber = 8,
        EnclosingMark = 7,
        FinalQuotePunctuation = 23,
        Format = 15,
        InitialQuotePunctuation = 22,
        LetterNumber = 9,
        LineSeparator = 12,
        LowercaseLetter = 1,
        MathSymbol = 25,
        ModifierLetter = 3,
        ModifierSymbol = 27,
        NonSpacingMark = 5,
        OpenPunctuation = 20,
        OtherLetter = 4,
        OtherNotAssigned = 29,
        OtherNumber = 10,
        OtherPunctuation = 24,
        OtherSymbol = 28,
        ParagraphSeparator = 13,
        PrivateUse = 17,
        SpaceSeparator = 11,
        SpacingCombiningMark = 6,
        Surrogate = 16,
        TitlecaseLetter = 2,
        UppercaseLetter = 0,
    }
}
