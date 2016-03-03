// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Text.Unicode;
using Xunit;

namespace System.Text.Encodings.Web.Tests
{
    public static class TextEncoderSettingsExtensions
    {
        public static bool IsCharacterAllowed(this TextEncoderSettings settings, char character)
        {
            var bitmap = settings.GetAllowedCharacters();
            return bitmap.IsCharacterAllowed(character);
        }
    }

    public class TextEncoderSettingsTests
    {
        [Fact]
        public void Ctor_Parameterless_CreatesEmptyFilter()
        {
            var filter = new TextEncoderSettings();
            Assert.Equal(0, filter.GetAllowedCodePoints().Count());
        }

        [Fact]
        public void Ctor_OtherTextEncoderSettingsAsInterface()
        {
            var originalFilter = new OddTextEncoderSettings();
            var newFilter = new TextEncoderSettings(originalFilter);

            for (int i = 0; i <= Char.MaxValue; i++)
            {
                Assert.Equal((i % 2) == 1, newFilter.IsCharacterAllowed((char)i));
            }
        }

        [Fact]
        public void Ctor_OtherTextEncoderSettingsAsConcreteType_Clones()
        {
            var originalFilter = new TextEncoderSettings();
            originalFilter.AllowCharacter('x');
            
            var newFilter = new TextEncoderSettings(originalFilter);
            newFilter.AllowCharacter('y');
            
            Assert.True(originalFilter.IsCharacterAllowed('x'));
            Assert.False(originalFilter.IsCharacterAllowed('y'));
            Assert.True(newFilter.IsCharacterAllowed('x'));
            Assert.True(newFilter.IsCharacterAllowed('y'));
        }

        [Fact]
        public void Ctor_UnicodeRanges()
        {
            var filter = new TextEncoderSettings(UnicodeRanges.LatinExtendedA, UnicodeRanges.LatinExtendedC);

            for (int i = 0; i < 0x0100; i++)
            {
                Assert.False(filter.IsCharacterAllowed((char)i));
            }
            for (int i = 0x0100; i <= 0x017F; i++)
            {
                Assert.True(filter.IsCharacterAllowed((char)i));
            }
            for (int i = 0x0180; i < 0x2C60; i++)
            {
                Assert.False(filter.IsCharacterAllowed((char)i));
            }
            for (int i = 0x2C60; i <= 0x2C7F; i++)
            {
                Assert.True(filter.IsCharacterAllowed((char)i));
            }
            for (int i = 0x2C80; i <= Char.MaxValue; i++)
            {
                Assert.False(filter.IsCharacterAllowed((char)i));
            }
        }

        [Fact]
        public void AllowChar()
        {
            var filter = new TextEncoderSettings();
            filter.AllowCharacter('\u0100');
            
            Assert.True(filter.IsCharacterAllowed('\u0100'));
            Assert.False(filter.IsCharacterAllowed('\u0101'));
        }

        [Fact]
        public void AllowChars_Array()
        {
            var filter = new TextEncoderSettings();
            filter.AllowCharacters('\u0100', '\u0102');
            
            Assert.True(filter.IsCharacterAllowed('\u0100'));
            Assert.False(filter.IsCharacterAllowed('\u0101'));
            Assert.True(filter.IsCharacterAllowed('\u0102'));
            Assert.False(filter.IsCharacterAllowed('\u0103'));
        }

        [Fact]
        public void AllowChars_String()
        {
            var filter = new TextEncoderSettings();
            filter.AllowCharacters('\u0100', '\u0102');
            
            Assert.True(filter.IsCharacterAllowed('\u0100'));
            Assert.False(filter.IsCharacterAllowed('\u0101'));
            Assert.True(filter.IsCharacterAllowed('\u0102'));
            Assert.False(filter.IsCharacterAllowed('\u0103'));
        }

        [Fact]
        public void AllowFilter()
        {
            var filter = new TextEncoderSettings(UnicodeRanges.BasicLatin);
            filter.AllowCodePoints(new OddTextEncoderSettings().GetAllowedCodePoints());

            for (int i = 0; i <= 0x007F; i++)
            {
                Assert.True(filter.IsCharacterAllowed((char)i));
            }
            for (int i = 0x0080; i <= Char.MaxValue; i++)
            {
                Assert.Equal((i % 2) == 1, filter.IsCharacterAllowed((char)i));
            }
        }

        [Fact]
        public void AllowRange()
        {
            var filter = new TextEncoderSettings();
            filter.AllowRange(UnicodeRanges.LatinExtendedA);
            
            for (int i = 0; i < 0x0100; i++)
            {
                Assert.False(filter.IsCharacterAllowed((char)i));
            }
            for (int i = 0x0100; i <= 0x017F; i++)
            {
                Assert.True(filter.IsCharacterAllowed((char)i));
            }
            for (int i = 0x0180; i <= Char.MaxValue; i++)
            {
                Assert.False(filter.IsCharacterAllowed((char)i));
            }
        }

        [Fact]
        public void AllowRanges()
        {
            var filter = new TextEncoderSettings();
            filter.AllowRanges(UnicodeRanges.LatinExtendedA, UnicodeRanges.LatinExtendedC);

            for (int i = 0; i < 0x0100; i++)
            {
                Assert.False(filter.IsCharacterAllowed((char)i));
            }
            for (int i = 0x0100; i <= 0x017F; i++)
            {
                Assert.True(filter.IsCharacterAllowed((char)i));
            }
            for (int i = 0x0180; i < 0x2C60; i++)
            {
                Assert.False(filter.IsCharacterAllowed((char)i));
            }
            for (int i = 0x2C60; i <= 0x2C7F; i++)
            {
                Assert.True(filter.IsCharacterAllowed((char)i));
            }
            for (int i = 0x2C80; i <= Char.MaxValue; i++)
            {
                Assert.False(filter.IsCharacterAllowed((char)i));
            }
        }

        [Fact]
        public void Clear()
        {
            var filter = new TextEncoderSettings();
            for (int i = 1; i <= Char.MaxValue; i++)
            {
                filter.AllowCharacter((char)i);
            }

            // Act
            filter.Clear();
            for (int i = 0; i <= Char.MaxValue; i++)
            {
                Assert.False(filter.IsCharacterAllowed((char)i));
            }
        }

        [Fact]
        public void ForbidChar()
        {
            var filter = new TextEncoderSettings(UnicodeRanges.BasicLatin);
            filter.ForbidCharacter('x');
            
            Assert.True(filter.IsCharacterAllowed('w'));
            Assert.False(filter.IsCharacterAllowed('x'));
            Assert.True(filter.IsCharacterAllowed('y'));
            Assert.True(filter.IsCharacterAllowed('z'));
        }

        [Fact]
        public void ForbidChars_Array()
        {
            var filter = new TextEncoderSettings(UnicodeRanges.BasicLatin);
            filter.ForbidCharacters('x', 'z');
            
            Assert.True(filter.IsCharacterAllowed('w'));
            Assert.False(filter.IsCharacterAllowed('x'));
            Assert.True(filter.IsCharacterAllowed('y'));
            Assert.False(filter.IsCharacterAllowed('z'));
        }

        [Fact]
        public void ForbidChars_String()
        {
            var filter = new TextEncoderSettings(UnicodeRanges.BasicLatin);
            filter.ForbidCharacters('x', 'z');
            
            Assert.True(filter.IsCharacterAllowed('w'));
            Assert.False(filter.IsCharacterAllowed('x'));
            Assert.True(filter.IsCharacterAllowed('y'));
            Assert.False(filter.IsCharacterAllowed('z'));
        }

        [Fact]
        public void ForbidRange()
        {
            var filter = new TextEncoderSettings(new OddTextEncoderSettings());
            filter.ForbidRange(UnicodeRanges.Specials);
            
            for (int i = 0; i <= 0xFFEF; i++)
            {
                Assert.Equal((i % 2) == 1, filter.IsCharacterAllowed((char)i));
            }
            for (int i = 0xFFF0; i <= Char.MaxValue; i++)
            {
                Assert.False(filter.IsCharacterAllowed((char)i));
            }
        }

        [Fact]
        public void ForbidRanges()
        {
            var filter = new TextEncoderSettings(new OddTextEncoderSettings());
            filter.ForbidRanges(UnicodeRanges.BasicLatin, UnicodeRanges.Specials);
            
            for (int i = 0; i <= 0x007F; i++)
            {
                Assert.False(filter.IsCharacterAllowed((char)i));
            }
            for (int i = 0x0080; i <= 0xFFEF; i++)
            {
                Assert.Equal((i % 2) == 1, filter.IsCharacterAllowed((char)i));
            }
            for (int i = 0xFFF0; i <= Char.MaxValue; i++)
            {
                Assert.False(filter.IsCharacterAllowed((char)i));
            }
        }

        [Fact]
        public void GetAllowedCodePoints()
        {
            var expected = Enumerable.Range(UnicodeRanges.BasicLatin.FirstCodePoint, UnicodeRanges.BasicLatin.Length)
                .Concat(Enumerable.Range(UnicodeRanges.Specials.FirstCodePoint, UnicodeRanges.Specials.Length))
                .Except(new int[] { 'x' })
                .OrderBy(i => i)
                .ToArray();

            var filter = new TextEncoderSettings(UnicodeRanges.BasicLatin, UnicodeRanges.Specials);
            filter.ForbidCharacter('x');
            
            Assert.Equal(expected, filter.GetAllowedCodePoints().OrderBy(i => i).ToArray());
        }

        // a code point filter which allows only odd code points through
        private sealed class OddTextEncoderSettings : TextEncoderSettings
        {
            public override IEnumerable<int> GetAllowedCodePoints()
            {
                for (int i = 1; i <= Char.MaxValue; i += 2)
                {
                    yield return i;
                }
            }
        }
    }
}
