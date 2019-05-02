// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

#pragma warning disable CS3011 // Only CLS-compliant members can be abstract

namespace System.Text.Encodings.Web
{
    public abstract partial class HtmlEncoder : TextEncoder
    {
        public static HtmlEncoder Default { get { throw null; } }
        public static HtmlEncoder Create(TextEncoderSettings settings) { throw null; }
        public static HtmlEncoder Create(params System.Text.Unicode.UnicodeRange[] allowedRanges) { throw null; }
    }
    public abstract partial class JavaScriptEncoder : TextEncoder
    {
        public static JavaScriptEncoder Default { get { throw null; } }
        public static JavaScriptEncoder Create(TextEncoderSettings settings) { throw null; }
        public static JavaScriptEncoder Create(params System.Text.Unicode.UnicodeRange[] allowedRanges) { throw null; }
    }
    public abstract partial class TextEncoder
    {
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public abstract int MaxOutputCharactersPerInputCharacter { get; }
        public virtual string Encode(string value) { throw null; }
        public void Encode(System.IO.TextWriter output, string value) { }
        public virtual void Encode(System.IO.TextWriter output, string value, int startIndex, int characterCount) { }
        public virtual void Encode(System.IO.TextWriter output, char[] value, int startIndex, int characterCount) { }
        private void Encode(System.IO.TextWriter output, System.ReadOnlySpan<char> remainingText) { throw null; }
        [System.CLSCompliant(false)]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public unsafe abstract int FindFirstCharacterToEncode(char* text, int textLength);
        [System.CLSCompliant(false)]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public unsafe abstract bool TryEncodeUnicodeScalar(int unicodeScalar, char* buffer, int bufferLength, out int numberOfCharactersWritten);
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public abstract bool WillEncode(int unicodeScalar);
    }
    public partial class TextEncoderSettings
    {
        public TextEncoderSettings() { }
        public TextEncoderSettings(TextEncoderSettings other) { }
        public TextEncoderSettings(params System.Text.Unicode.UnicodeRange[] allowedRanges) { }
        public virtual void AllowCharacter(char character) { }
        public virtual void AllowCharacters(params char[] characters) { }
        public virtual void AllowCodePoints(System.Collections.Generic.IEnumerable<int> codePoints) { }
        public virtual void AllowRange(System.Text.Unicode.UnicodeRange range) { }
        public virtual void AllowRanges(params System.Text.Unicode.UnicodeRange[] ranges) { }
        public virtual void Clear() { }
        public virtual void ForbidCharacter(char character) { }
        public virtual void ForbidCharacters(params char[] characters) { }
        public virtual void ForbidRange(System.Text.Unicode.UnicodeRange range) { }
        public virtual void ForbidRanges(params System.Text.Unicode.UnicodeRange[] ranges) { }
        public virtual System.Collections.Generic.IEnumerable<int> GetAllowedCodePoints() { throw null; }
    }
    public abstract partial class UrlEncoder : TextEncoder
    {
        public static UrlEncoder Default { get { throw null; } }
        public static UrlEncoder Create(TextEncoderSettings settings) { throw null; }
        public static UrlEncoder Create(params System.Text.Unicode.UnicodeRange[] allowedRanges) { throw null; }
    }
}
namespace System.Text.Unicode
{
    public sealed class UnicodeRange
    {
        public UnicodeRange(int firstCodePoint, int length) { }
        public int FirstCodePoint { get { throw null; } }
        public int Length { get { throw null; } }
        public static UnicodeRange Create(char firstCharacter, char lastCharacter) { throw null; }
    }
    public static partial class UnicodeRanges
    {
        public static UnicodeRange None { get { throw null; } }
        public static UnicodeRange All { get { return null; } }
        private static UnicodeRange CreateEmptyRange(ref UnicodeRange range) { throw null; }
        private static UnicodeRange CreateRange(ref UnicodeRange range, char first, char last) { throw null; }
        public static UnicodeRange BasicLatin { get { throw null; } }
        public static UnicodeRange Latin1Supplement { get { throw null; } }
        public static UnicodeRange LatinExtendedA { get { throw null; } }
        public static UnicodeRange LatinExtendedB { get { throw null; } }
        public static UnicodeRange IpaExtensions { get { throw null; } }
        public static UnicodeRange SpacingModifierLetters { get { throw null; } }
        public static UnicodeRange CombiningDiacriticalMarks { get { throw null; } }
        public static UnicodeRange GreekandCoptic { get { throw null; } }
        public static UnicodeRange Cyrillic { get { throw null; } }
        public static UnicodeRange CyrillicSupplement { get { throw null; } }
        public static UnicodeRange Armenian { get { throw null; } }
        public static UnicodeRange Hebrew { get { throw null; } }
        public static UnicodeRange Arabic { get { throw null; } }
        public static UnicodeRange Syriac { get { throw null; } }
        public static UnicodeRange ArabicSupplement { get { throw null; } }
        public static UnicodeRange Thaana { get { throw null; } }
        public static UnicodeRange NKo { get { throw null; } }
        public static UnicodeRange Samaritan { get { throw null; } }
        public static UnicodeRange Mandaic { get { throw null; } }
        public static UnicodeRange ArabicExtendedA { get { throw null; } }
        public static UnicodeRange Devanagari { get { throw null; } }
        public static UnicodeRange Bengali { get { throw null; } }
        public static UnicodeRange Gurmukhi { get { throw null; } }
        public static UnicodeRange Gujarati { get { throw null; } }
        public static UnicodeRange Oriya { get { throw null; } }
        public static UnicodeRange Tamil { get { throw null; } }
        public static UnicodeRange Telugu { get { throw null; } }
        public static UnicodeRange Kannada { get { throw null; } }
        public static UnicodeRange Malayalam { get { throw null; } }
        public static UnicodeRange Sinhala { get { throw null; } }
        public static UnicodeRange Thai { get { throw null; } }
        public static UnicodeRange Lao { get { throw null; } }
        public static UnicodeRange Tibetan { get { throw null; } }
        public static UnicodeRange Myanmar { get { throw null; } }
        public static UnicodeRange Georgian { get { throw null; } }
        public static UnicodeRange HangulJamo { get { throw null; } }
        public static UnicodeRange Ethiopic { get { throw null; } }
        public static UnicodeRange EthiopicSupplement { get { throw null; } }
        public static UnicodeRange Cherokee { get { throw null; } }
        public static UnicodeRange UnifiedCanadianAboriginalSyllabics { get { throw null; } }
        public static UnicodeRange Ogham { get { throw null; } }
        public static UnicodeRange Runic { get { throw null; } }
        public static UnicodeRange Tagalog { get { throw null; } }
        public static UnicodeRange Hanunoo { get { throw null; } }
        public static UnicodeRange Buhid { get { throw null; } }
        public static UnicodeRange Tagbanwa { get { throw null; } }
        public static UnicodeRange Khmer { get { throw null; } }
        public static UnicodeRange Mongolian { get { throw null; } }
        public static UnicodeRange UnifiedCanadianAboriginalSyllabicsExtended { get { throw null; } }
        public static UnicodeRange Limbu { get { throw null; } }
        public static UnicodeRange TaiLe { get { throw null; } }
        public static UnicodeRange NewTaiLue { get { throw null; } }
        public static UnicodeRange KhmerSymbols { get { throw null; } }
        public static UnicodeRange Buginese { get { throw null; } }
        public static UnicodeRange TaiTham { get { throw null; } }
        public static UnicodeRange CombiningDiacriticalMarksExtended { get { throw null; } }
        public static UnicodeRange Balinese { get { throw null; } }
        public static UnicodeRange Sundanese { get { throw null; } }
        public static UnicodeRange Batak { get { throw null; } }
        public static UnicodeRange Lepcha { get { throw null; } }
        public static UnicodeRange OlChiki { get { throw null; } }
        public static UnicodeRange SundaneseSupplement { get { throw null; } }
        public static UnicodeRange VedicExtensions { get { throw null; } }
        public static UnicodeRange PhoneticExtensions { get { throw null; } }
        public static UnicodeRange PhoneticExtensionsSupplement { get { throw null; } }
        public static UnicodeRange CombiningDiacriticalMarksSupplement { get { throw null; } }
        public static UnicodeRange LatinExtendedAdditional { get { throw null; } }
        public static UnicodeRange GreekExtended { get { throw null; } }
        public static UnicodeRange GeneralPunctuation { get { throw null; } }
        public static UnicodeRange SuperscriptsandSubscripts { get { throw null; } }
        public static UnicodeRange CurrencySymbols { get { throw null; } }
        public static UnicodeRange CombiningDiacriticalMarksforSymbols { get { throw null; } }
        public static UnicodeRange LetterlikeSymbols { get { throw null; } }
        public static UnicodeRange NumberForms { get { throw null; } }
        public static UnicodeRange Arrows { get { throw null; } }
        public static UnicodeRange MathematicalOperators { get { throw null; } }
        public static UnicodeRange MiscellaneousTechnical { get { throw null; } }
        public static UnicodeRange ControlPictures { get { throw null; } }
        public static UnicodeRange OpticalCharacterRecognition { get { throw null; } }
        public static UnicodeRange EnclosedAlphanumerics { get { throw null; } }
        public static UnicodeRange BoxDrawing { get { throw null; } }
        public static UnicodeRange BlockElements { get { throw null; } }
        public static UnicodeRange GeometricShapes { get { throw null; } }
        public static UnicodeRange MiscellaneousSymbols { get { throw null; } }
        public static UnicodeRange Dingbats { get { throw null; } }
        public static UnicodeRange MiscellaneousMathematicalSymbolsA { get { throw null; } }
        public static UnicodeRange SupplementalArrowsA { get { throw null; } }
        public static UnicodeRange BraillePatterns { get { throw null; } }
        public static UnicodeRange SupplementalArrowsB { get { throw null; } }
        public static UnicodeRange MiscellaneousMathematicalSymbolsB { get { throw null; } }
        public static UnicodeRange SupplementalMathematicalOperators { get { throw null; } }
        public static UnicodeRange MiscellaneousSymbolsandArrows { get { throw null; } }
        public static UnicodeRange Glagolitic { get { throw null; } }
        public static UnicodeRange LatinExtendedC { get { throw null; } }
        public static UnicodeRange Coptic { get { throw null; } }
        public static UnicodeRange GeorgianSupplement { get { throw null; } }
        public static UnicodeRange Tifinagh { get { throw null; } }
        public static UnicodeRange EthiopicExtended { get { throw null; } }
        public static UnicodeRange CyrillicExtendedA { get { throw null; } }
        public static UnicodeRange SupplementalPunctuation { get { throw null; } }
        public static UnicodeRange CjkRadicalsSupplement { get { throw null; } }
        public static UnicodeRange KangxiRadicals { get { throw null; } }
        public static UnicodeRange IdeographicDescriptionCharacters { get { throw null; } }
        public static UnicodeRange CjkSymbolsandPunctuation { get { throw null; } }
        public static UnicodeRange Hiragana { get { throw null; } }
        public static UnicodeRange Katakana { get { throw null; } }
        public static UnicodeRange Bopomofo { get { throw null; } }
        public static UnicodeRange HangulCompatibilityJamo { get { throw null; } }
        public static UnicodeRange Kanbun { get { throw null; } }
        public static UnicodeRange BopomofoExtended { get { throw null; } }
        public static UnicodeRange CjkStrokes { get { throw null; } }
        public static UnicodeRange KatakanaPhoneticExtensions { get { throw null; } }
        public static UnicodeRange EnclosedCjkLettersandMonths { get { throw null; } }
        public static UnicodeRange CjkCompatibility { get { throw null; } }
        public static UnicodeRange CjkUnifiedIdeographsExtensionA { get { throw null; } }
        public static UnicodeRange YijingHexagramSymbols { get { throw null; } }
        public static UnicodeRange CjkUnifiedIdeographs { get { throw null; } }
        public static UnicodeRange YiSyllables { get { throw null; } }
        public static UnicodeRange YiRadicals { get { throw null; } }
        public static UnicodeRange Lisu { get { throw null; } }
        public static UnicodeRange Vai { get { throw null; } }
        public static UnicodeRange CyrillicExtendedB { get { throw null; } }
        public static UnicodeRange Bamum { get { throw null; } }
        public static UnicodeRange ModifierToneLetters { get { throw null; } }
        public static UnicodeRange LatinExtendedD { get { throw null; } }
        public static UnicodeRange SylotiNagri { get { throw null; } }
        public static UnicodeRange CommonIndicNumberForms { get { throw null; } }
        public static UnicodeRange Phagspa { get { throw null; } }
        public static UnicodeRange Saurashtra { get { throw null; } }
        public static UnicodeRange DevanagariExtended { get { throw null; } }
        public static UnicodeRange KayahLi { get { throw null; } }
        public static UnicodeRange Rejang { get { throw null; } }
        public static UnicodeRange HangulJamoExtendedA { get { throw null; } }
        public static UnicodeRange Javanese { get { throw null; } }
        public static UnicodeRange MyanmarExtendedB { get { throw null; } }
        public static UnicodeRange Cham { get { throw null; } }
        public static UnicodeRange MyanmarExtendedA { get { throw null; } }
        public static UnicodeRange TaiViet { get { throw null; } }
        public static UnicodeRange MeeteiMayekExtensions { get { throw null; } }
        public static UnicodeRange EthiopicExtendedA { get { throw null; } }
        public static UnicodeRange LatinExtendedE { get { throw null; } }
        public static UnicodeRange CherokeeSupplement { get { throw null; } }
        public static UnicodeRange MeeteiMayek { get { throw null; } }
        public static UnicodeRange HangulSyllables { get { throw null; } }
        public static UnicodeRange HangulJamoExtendedB { get { throw null; } }
        public static UnicodeRange CjkCompatibilityIdeographs { get { throw null; } }
        public static UnicodeRange AlphabeticPresentationForms { get { throw null; } }
        public static UnicodeRange ArabicPresentationFormsA { get { throw null; } }
        public static UnicodeRange VariationSelectors { get { throw null; } }
        public static UnicodeRange VerticalForms { get { throw null; } }
        public static UnicodeRange CombiningHalfMarks { get { throw null; } }
        public static UnicodeRange CjkCompatibilityForms { get { throw null; } }
        public static UnicodeRange SmallFormVariants { get { throw null; } }
        public static UnicodeRange ArabicPresentationFormsB { get { throw null; } }
        public static UnicodeRange HalfwidthandFullwidthForms { get { throw null; } }
        public static UnicodeRange Specials { get { throw null; } }
    }
}
