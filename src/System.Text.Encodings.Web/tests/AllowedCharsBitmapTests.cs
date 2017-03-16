// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Internal;
using System.Text.Unicode;
using Xunit;

namespace System.Text.Encodings.Web.Tests
{
    public class AllowedCharsBitmapTests
    {
        [Fact]
        public void Ctor_EmptyByDefault()
        {
            var bitmap = AllowedCharactersBitmap.CreateNew();
            for (int i = 0; i <= Char.MaxValue; i++)
            {
                Assert.False(bitmap.IsCharacterAllowed((char)i));
            }
        }

        [Fact]
        public void Allow_Forbid_ZigZag()
        {
            var bitmap = AllowedCharactersBitmap.CreateNew();

            // The only chars which are allowed are those whose code points are multiples of 3 or 7
            // who aren't also multiples of 5. Exception: multiples of 35 are allowed.
            for (int i = 0; i <= Char.MaxValue; i += 3)
            {
                if (i % 3 == 0) { bitmap.AllowCharacter((char)i); }
                if (i % 5 == 0) { bitmap.AllowCharacter((char)i); }
                if (i % 7 == 0) { bitmap.AllowCharacter((char)i); }
            }
            
            for (int i = 0; i <= Char.MaxValue; i++)
            {
                bool isAllowed = false;
                if (i % 3 == 0) { isAllowed = true; }
                if (i % 5 == 0) { isAllowed = false; }
                if (i % 7 == 0) { isAllowed = true; }
                Assert.Equal(isAllowed, bitmap.IsCharacterAllowed((char)i));
            }
        }

        [Fact]
        public void Clear_ForbidsEverything()
        {
            var bitmap = AllowedCharactersBitmap.CreateNew();
            for (int i = 1; i <= Char.MaxValue; i++)
            {
                bitmap.AllowCharacter((char)i);
            }
            
            bitmap.Clear();
            for (int i = 0; i <= Char.MaxValue; i++)
            {
                Assert.False(bitmap.IsCharacterAllowed((char)i));
            }
        }

        [Fact]
        public void Clone_MakesDeepCopy()
        {
            var originalBitmap = AllowedCharactersBitmap.CreateNew();
            originalBitmap.AllowCharacter('x');
            
            var clonedBitmap = originalBitmap.Clone();
            clonedBitmap.AllowCharacter('y');
            
            Assert.True(originalBitmap.IsCharacterAllowed('x'));
            Assert.False(originalBitmap.IsCharacterAllowed('y'));
            Assert.True(clonedBitmap.IsCharacterAllowed('x'));
            Assert.True(clonedBitmap.IsCharacterAllowed('y'));
        }

        [Fact]
        public void ForbidUndefinedCharacters_RemovesUndefinedChars()
        {
            // We only allow odd-numbered characters in this test so that
            // we can validate that we properly merged the two bitmaps together
            // rather than simply overwriting the target.
            var bitmap = AllowedCharactersBitmap.CreateNew();
            for (int i = 1; i <= Char.MaxValue; i += 2)
            {
                bitmap.AllowCharacter((char)i);
            }
            
            bitmap.ForbidUndefinedCharacters();
            for (int i = 0; i <= Char.MaxValue; i++)
            {
                if (i % 2 == 0)
                {
                    Assert.False(bitmap.IsCharacterAllowed((char)i)); // these chars were never allowed in the original description
                }
                else
                {
                    Assert.Equal(UnicodeHelpers.IsCharacterDefined((char)i), bitmap.IsCharacterAllowed((char)i));
                }
            }
        }
    }
}
