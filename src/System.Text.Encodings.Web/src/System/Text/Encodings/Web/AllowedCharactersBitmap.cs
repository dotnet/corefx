// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Unicode;

namespace System.Text.Internal
{
    internal readonly struct AllowedCharactersBitmap
    {
        private const int ALLOWED_CHARS_BITMAP_LENGTH = 0x10000 / (8 * sizeof(uint));
        private readonly uint[] _allowedCharacters;

        // should be called in place of the default ctor
        public static AllowedCharactersBitmap CreateNew()
        {
            return new AllowedCharactersBitmap(new uint[ALLOWED_CHARS_BITMAP_LENGTH]);
        }

        private AllowedCharactersBitmap(uint[] allowedCharacters)
        {
            if(allowedCharacters == null)
            {
                throw new ArgumentNullException(nameof(allowedCharacters));
            }
            _allowedCharacters = allowedCharacters;
        }

        // Marks a character as allowed (can be returned unencoded)
        public void AllowCharacter(char character)
        {
            int codePoint = character;
            int index = codePoint >> 5;
            int offset = codePoint & 0x1F;
            _allowedCharacters[index] |= 0x1U << offset;
        }

        // Marks a character as forbidden (must be returned encoded)
        public void ForbidCharacter(char character)
        {
            int codePoint = character;
            int index = codePoint >> 5;
            int offset = codePoint & 0x1F;
            _allowedCharacters[index] &= ~(0x1U << offset);
        }

        // Forbid codepoints which aren't mapped to characters or which are otherwise always disallowed
        // (includes categories Cc, Cs, Co, Cn, Zs [except U+0020 SPACE], Zl, Zp)
        public void ForbidUndefinedCharacters()
        {
            uint[] definedCharactersBitmap = UnicodeHelpers.GetDefinedCharacterBitmap();
            Debug.Assert(definedCharactersBitmap.Length == _allowedCharacters.Length);
            for (int i = 0; i < _allowedCharacters.Length; i++)
            {
                _allowedCharacters[i] &= definedCharactersBitmap[i];
            }
        }

        // Marks all characters as forbidden (must be returned encoded)
        public void Clear()
        {
            Array.Clear(_allowedCharacters, 0, _allowedCharacters.Length);
        }

        // Creates a deep copy of this bitmap
        public AllowedCharactersBitmap Clone()
        {
            return new AllowedCharactersBitmap((uint[])_allowedCharacters.Clone());
        }

        // Determines whether the given character can be returned unencoded.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsCharacterAllowed(char character) => IsUnicodeScalarAllowed(character);

        // Determines whether the given character can be returned unencoded.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsUnicodeScalarAllowed(uint unicodeScalar)
        {
            uint index = unicodeScalar >> 5;
            if (index >= (uint)_allowedCharacters.Length)
            {
                return false; // would be out-of-bounds read (unicodeScalar isn't BMP char)
            }

            return ((_allowedCharacters[index] >> ((int)unicodeScalar & 0x1F) /* offset */) & 1U) != 0;
        }
    }
}
