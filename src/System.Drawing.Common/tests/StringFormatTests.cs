﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Text;
using Xunit;

namespace System.Drawing.Tests
{
    public class StringFormatTests
    {
        private const int RandomLanguageCode = 10;
        private const int EnglishLanguageCode = 2057;

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void Ctor_Default()
        {
            var format = new StringFormat();
            Assert.Equal(StringAlignment.Near, format.Alignment);
            Assert.Equal(0, format.DigitSubstitutionLanguage);
            Assert.Equal(StringDigitSubstitute.User, format.DigitSubstitutionMethod);
            Assert.Equal((StringFormatFlags)0, format.FormatFlags);
            Assert.Equal(HotkeyPrefix.None, format.HotkeyPrefix);
            Assert.Equal(StringAlignment.Near, format.LineAlignment);
            Assert.Equal(StringTrimming.Character, format.Trimming);
        }

        [ConditionalTheory(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        [InlineData(StringFormatFlags.DirectionRightToLeft | StringFormatFlags.DirectionVertical)]
        [InlineData((StringFormatFlags)(-1))]
        public void Ctor_Options(StringFormatFlags options)
        {
            var format = new StringFormat(options);
            Assert.Equal(StringAlignment.Near, format.Alignment);
            Assert.Equal(0, format.DigitSubstitutionLanguage);
            Assert.Equal(StringDigitSubstitute.User, format.DigitSubstitutionMethod);
            Assert.Equal(options, format.FormatFlags);
            Assert.Equal(HotkeyPrefix.None, format.HotkeyPrefix);
            Assert.Equal(StringAlignment.Near, format.LineAlignment);
            Assert.Equal(StringTrimming.Character, format.Trimming);
        }

        [ConditionalTheory(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        [InlineData(StringFormatFlags.DirectionRightToLeft | StringFormatFlags.DirectionVertical, RandomLanguageCode)]
        [InlineData(StringFormatFlags.NoClip, EnglishLanguageCode)]
        [InlineData((StringFormatFlags)(-1), -1)]
        public void Ctor_Options_Language(StringFormatFlags options, int language)
        {
            var format = new StringFormat(options, language);
            Assert.Equal(StringAlignment.Near, format.Alignment);
            Assert.Equal(0, format.DigitSubstitutionLanguage);
            Assert.Equal(StringDigitSubstitute.User, format.DigitSubstitutionMethod);
            Assert.Equal(options, format.FormatFlags);
            Assert.Equal(HotkeyPrefix.None, format.HotkeyPrefix);
            Assert.Equal(StringAlignment.Near, format.LineAlignment);
            Assert.Equal(StringTrimming.Character, format.Trimming);
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void Ctor_Format()
        {
            var original = new StringFormat(StringFormatFlags.NoClip, EnglishLanguageCode);
            var format = new StringFormat(original);
            Assert.Equal(StringAlignment.Near, format.Alignment);
            Assert.Equal(0, format.DigitSubstitutionLanguage);
            Assert.Equal(StringDigitSubstitute.User, format.DigitSubstitutionMethod);
            Assert.Equal(StringFormatFlags.NoClip, format.FormatFlags);
            Assert.Equal(HotkeyPrefix.None, format.HotkeyPrefix);
            Assert.Equal(StringAlignment.Near, format.LineAlignment);
            Assert.Equal(StringTrimming.Character, format.Trimming);

            // The new format is a clone.
            original.FormatFlags = StringFormatFlags.NoFontFallback;
            Assert.Equal(StringFormatFlags.NoClip, format.FormatFlags);
        }

        [Fact]
        public void Ctor_NullFormat_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("format", () => new StringFormat(null));
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void Ctor_DisposedFormat_ThrowsArgumentException()
        {
            var format = new StringFormat();
            format.Dispose();
            Assert.Throws<ArgumentException>(null, () => new StringFormat(format));
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void Dispose_MultipleTimes_Success()
        {
            var format = new StringFormat();
            format.Dispose();
            format.Dispose();
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void Clone_Valid_Success()
        {
            var original = new StringFormat(StringFormatFlags.NoClip, EnglishLanguageCode);
            var format = (StringFormat)original.Clone();
            Assert.Equal(StringAlignment.Near, format.Alignment);
            Assert.Equal(0, format.DigitSubstitutionLanguage);
            Assert.Equal(StringDigitSubstitute.User, format.DigitSubstitutionMethod);
            Assert.Equal(StringFormatFlags.NoClip, format.FormatFlags);
            Assert.Equal(HotkeyPrefix.None, format.HotkeyPrefix);
            Assert.Equal(StringAlignment.Near, format.LineAlignment);
            Assert.Equal(StringTrimming.Character, format.Trimming);

            // The new format is a clone.
            original.FormatFlags = StringFormatFlags.NoFontFallback;
            Assert.Equal(StringFormatFlags.NoClip, format.FormatFlags);
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void Clone_Disposed_ThrowsArgumentException()
        {
            var format = new StringFormat();
            format.Dispose();
            Assert.Throws<ArgumentException>(null, () => format.Clone());
        }

        [ConditionalTheory(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        [InlineData(0, StringDigitSubstitute.None, 0)]
        [InlineData(EnglishLanguageCode, StringDigitSubstitute.Traditional, EnglishLanguageCode)]
        [InlineData(int.MaxValue, StringDigitSubstitute.Traditional + 1, 65535)]
        [InlineData(-1, StringDigitSubstitute.User - 1, 65535)]
        public void SetDigitSubstitution_Invoke_SetsProperties(int language, StringDigitSubstitute substitute, int expectedLanguage)
        {
            var format = new StringFormat();
            format.SetDigitSubstitution(language, substitute);
            Assert.Equal(expectedLanguage, format.DigitSubstitutionLanguage);
            Assert.Equal(substitute, format.DigitSubstitutionMethod);
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void SetDigitSubstitution_Disposed_ThrowsArgumentException()
        {
            var format = new StringFormat();
            format.Dispose();
            Assert.Throws<ArgumentException>(null, () => format.SetDigitSubstitution(0, StringDigitSubstitute.None));
        }

        [ConditionalTheory(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        [InlineData(0, new float[0])]
        [InlineData(10, new float[] { 1, 2.3f, 4, float.PositiveInfinity, float.NaN })]
        public void SetTabStops_GetTabStops_ReturnsExpected(float firstTabOffset, float[] tabStops)
        {
            var format = new StringFormat();
            format.SetTabStops(firstTabOffset, tabStops);

            Assert.Equal(tabStops, format.GetTabStops(out float actualFirstTabOffset));
            Assert.Equal(firstTabOffset, actualFirstTabOffset);
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void SetTabStops_NullTabStops_ThrowsNullReferenceException()
        {
            var format = new StringFormat();
            Assert.Throws<NullReferenceException>(() => format.SetTabStops(0, null));
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void SetTabStops_NegativeFirstTabOffset_ThrowsArgumentException()
        {
            var format = new StringFormat();
            Assert.Throws<ArgumentException>(null, () => format.SetTabStops(-1, new float[0]));
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void SetTabStops_NegativeInfinityInTabStops_ThrowsNotImplementedException()
        {
            var format = new StringFormat();
            Assert.Throws<NotImplementedException>(() => format.SetTabStops(0, new float[] { float.NegativeInfinity }));
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void SetTabStops_Disposed_ThrowsArgumentException()
        {
            var format = new StringFormat();
            format.Dispose();
            Assert.Throws<ArgumentException>(null, () => format.SetTabStops(0, new float[0]));
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void GetTabStops_Disposed_ThrowsArgumentException()
        {
            var format = new StringFormat();
            format.Dispose();
            Assert.Throws<ArgumentException>(null, () => format.GetTabStops(out float firstTabOffset));
        }

        public static IEnumerable<object[]> SetMeasurableCharacterRanges_TestData()
        {
            yield return new object[] { new CharacterRange[0] };
            yield return new object[] { new CharacterRange[] { new CharacterRange(1, 2) } };
            yield return new object[] { new CharacterRange[] { new CharacterRange(-1, -1) } };
            yield return new object[] { new CharacterRange[32] };
        }

        [ConditionalTheory(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        [MemberData(nameof(SetMeasurableCharacterRanges_TestData))]
        public void SetMeasurableCharacterRanges_Valid_Success(CharacterRange[] ranges)
        {
            var format = new StringFormat();
            format.SetMeasurableCharacterRanges(ranges);
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void SetMeasurableCharacterRanges_NullRanges_ThrowsNullReferenceException()
        {
            var format = new StringFormat();
            Assert.Throws<NullReferenceException>(() => format.SetMeasurableCharacterRanges(null));
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void SetMeasurableCharacterRanges_RangesTooLarge_ThrowsOverflowException()
        {
            var format = new StringFormat();
            Assert.Throws<OverflowException>(() => format.SetMeasurableCharacterRanges(new CharacterRange[33]));
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void SetMeasurableCharacterRanges_Disposed_ThrowsArgumentException()
        {
            var format = new StringFormat();
            format.Dispose();
            Assert.Throws<ArgumentException>(null, () => format.SetMeasurableCharacterRanges(new CharacterRange[0]));
        }

        [ConditionalTheory(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        [InlineData(StringAlignment.Center)]
        [InlineData(StringAlignment.Far)]
        [InlineData(StringAlignment.Near)]
        public void Alignment_SetValid_GetReturnsExpected(StringAlignment alignment)
        {
            var format = new StringFormat { Alignment = alignment };
            Assert.Equal(alignment, format.Alignment);
        }

        [ConditionalTheory(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        [InlineData(StringAlignment.Near - 1)]
        [InlineData(StringAlignment.Far + 1)]
        public void Alignment_SetInvalid_ThrowsInvalidEnumArgumentException(StringAlignment alignment)
        {
            var format = new StringFormat();
            Assert.Throws<InvalidEnumArgumentException>("value", () => format.Alignment = alignment);
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void Alignment_GetSetWhenDisposed_ThrowsArgumentException()
        {
            var format = new StringFormat();
            format.Dispose();
            Assert.Throws<ArgumentException>(null, () => format.Alignment);
            Assert.Throws<ArgumentException>(null, () => format.Alignment = StringAlignment.Center);
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void DigitSubstituionMethod_GetSetWhenDisposed_ThrowsArgumentException()
        {
            var format = new StringFormat();
            format.Dispose();
            Assert.Throws<ArgumentException>(null, () => format.DigitSubstitutionMethod);
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void DigitSubstituionLanguage_GetSetWhenDisposed_ThrowsArgumentException()
        {
            var format = new StringFormat();
            format.Dispose();
            Assert.Throws<ArgumentException>(null, () => format.DigitSubstitutionLanguage);
        }

        [ConditionalTheory(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        [InlineData(StringFormatFlags.DirectionRightToLeft)]
        [InlineData((StringFormatFlags)int.MinValue)]
        [InlineData((StringFormatFlags)int.MaxValue)]
        public void FormatFlags_Set_GetReturnsExpected(StringFormatFlags formatFlags)
        {
            var format = new StringFormat { FormatFlags = formatFlags };
            Assert.Equal(formatFlags, format.FormatFlags);
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void FormatFlags_GetSetWhenDisposed_ThrowsArgumentException()
        {
            var format = new StringFormat();
            format.Dispose();
            Assert.Throws<ArgumentException>(null, () => format.FormatFlags);
            Assert.Throws<ArgumentException>(null, () => format.FormatFlags = StringFormatFlags.NoClip);
        }

        [ConditionalTheory(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        [InlineData(StringAlignment.Center)]
        [InlineData(StringAlignment.Far)]
        [InlineData(StringAlignment.Near)]
        public void LineAlignment_SetValid_GetReturnsExpected(StringAlignment alignment)
        {
            var format = new StringFormat { LineAlignment = alignment };
            Assert.Equal(alignment, format.LineAlignment);
        }

        [ConditionalTheory(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        [InlineData(StringAlignment.Near - 1)]
        [InlineData(StringAlignment.Far + 1)]
        public void LineAlignment_SetInvalid_ThrowsInvalidEnumArgumentException(StringAlignment alignment)
        {
            var format = new StringFormat();
            Assert.Throws<InvalidEnumArgumentException>("value", () => format.LineAlignment = alignment);
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void LineAlignment_GetSetWhenDisposed_ThrowsArgumentException()
        {
            var format = new StringFormat();
            format.Dispose();
            Assert.Throws<ArgumentException>(null, () => format.LineAlignment);
            Assert.Throws<ArgumentException>(null, () => format.LineAlignment = StringAlignment.Center);
        }

        [ConditionalTheory(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        [InlineData(HotkeyPrefix.Hide)]
        [InlineData(HotkeyPrefix.None)]
        [InlineData(HotkeyPrefix.Show)]
        public void HotKeyPrefix_SetValid_GetReturnsExpected(HotkeyPrefix prefix)
        {
            var format = new StringFormat { HotkeyPrefix = prefix };
            Assert.Equal(prefix, format.HotkeyPrefix);
        }

        [ConditionalTheory(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        [InlineData(HotkeyPrefix.None - 1)]
        [InlineData(HotkeyPrefix.Hide + 1)]
        public void HotKeyPrefix_SetInvalid_ThrowsInvalidEnumArgumentException(HotkeyPrefix prefix)
        {
            var format = new StringFormat();
            Assert.Throws<InvalidEnumArgumentException>("value", () => format.HotkeyPrefix = prefix);
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void HotkeyPrefix_GetSetWhenDisposed_ThrowsArgumentException()
        {
            var format = new StringFormat();
            format.Dispose();
            Assert.Throws<ArgumentException>(null, () => format.HotkeyPrefix);
            Assert.Throws<ArgumentException>(null, () => format.HotkeyPrefix = HotkeyPrefix.Hide);
        }

        [ConditionalTheory(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        [InlineData(StringTrimming.Word)]
        public void Trimming_SetValid_GetReturnsExpected(StringTrimming trimming)
        {
            var format = new StringFormat { Trimming = trimming };
            Assert.Equal(trimming, format.Trimming);
        }

        [ConditionalTheory(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        [InlineData(StringTrimming.None - 1)]
        [InlineData(StringTrimming.EllipsisPath + 1)]
        public void Trimming_SetInvalid_ThrowsInvalidEnumArgumentException(StringTrimming trimming)
        {
            var format = new StringFormat();
            Assert.Throws<InvalidEnumArgumentException>("value", () => format.Trimming = trimming);
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void Trimming_GetSetWhenDisposed_ThrowsArgumentException()
        {
            var format = new StringFormat();
            format.Dispose();
            Assert.Throws<ArgumentException>(null, () => format.Trimming);
            Assert.Throws<ArgumentException>(null, () => format.Trimming = StringTrimming.Word);
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void GenericDefault_Get_ReturnsExpected()
        {
            StringFormat format = StringFormat.GenericDefault;
            Assert.NotSame(format, StringFormat.GenericDefault);

            Assert.Equal(StringAlignment.Near, format.Alignment);
            Assert.Equal(0, format.DigitSubstitutionLanguage);
            Assert.Equal(StringDigitSubstitute.User, format.DigitSubstitutionMethod);
            Assert.Equal((StringFormatFlags)0, format.FormatFlags);
            Assert.Equal(HotkeyPrefix.None, format.HotkeyPrefix);
            Assert.Equal(StringAlignment.Near, format.LineAlignment);
            Assert.Equal(StringTrimming.Character, format.Trimming);
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void GenericTypographic_Get_ReturnsExpected()
        {
            StringFormat format = StringFormat.GenericTypographic;
            Assert.NotSame(format, StringFormat.GenericTypographic);

            Assert.Equal(StringAlignment.Near, format.Alignment);
            Assert.Equal(0, format.DigitSubstitutionLanguage);
            Assert.Equal(StringDigitSubstitute.User, format.DigitSubstitutionMethod);
            Assert.Equal(StringFormatFlags.FitBlackBox | StringFormatFlags.LineLimit | StringFormatFlags.NoClip, format.FormatFlags);
            Assert.Equal(HotkeyPrefix.None, format.HotkeyPrefix);
            Assert.Equal(StringAlignment.Near, format.LineAlignment);
            Assert.Equal(StringTrimming.None, format.Trimming);
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void ToString_Flags_ReturnsExpected()
        {
            var format = new StringFormat(StringFormatFlags.DirectionVertical);
            Assert.Equal("[StringFormat, FormatFlags=DirectionVertical]", format.ToString());
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void ToString_Disposed_ThrowsArgumentException()
        {
            var format = new StringFormat(StringFormatFlags.DirectionVertical);
            format.Dispose();
            Assert.Throws<ArgumentException>(null, () => format.ToString());
        }
    }
}
