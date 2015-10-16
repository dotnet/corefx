// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
 
namespace System
{
    public partial struct ConsoleKeyInfo
    {
        private char _keyChar;
        private ConsoleKey _key;
        private ConsoleModifiers _mods;

        public ConsoleKeyInfo(char keyChar, ConsoleKey key, bool shift, bool alt, bool control)
        {
            // Limit ConsoleKey values to 0 to 255, but don't check whether the
            // key is a valid value in our ConsoleKey enum.  There are a few 
            // values in that enum that we didn't define, and reserved keys 
            // that might start showing up on keyboards in a few years.
            if (((int)key) < 0 || ((int)key) > 255)
            {
                throw new ArgumentOutOfRangeException("key", SR.ArgumentOutOfRange_ConsoleKey);
            }

            _keyChar = keyChar;
            _key = key;
            _mods = 0;
            if (shift)
                _mods |= ConsoleModifiers.Shift;
            if (alt)
                _mods |= ConsoleModifiers.Alt;
            if (control)
                _mods |= ConsoleModifiers.Control;
        }

        public char KeyChar
        {
            get { return _keyChar; }
        }

        public ConsoleKey Key
        {
            get { return _key; }
        }

        public ConsoleModifiers Modifiers
        {
            get { return _mods; }
        }

        public override bool Equals(Object value)
        {
            if (value is ConsoleKeyInfo)
                return Equals((ConsoleKeyInfo)value);
            else
                return false;
        }

        public bool Equals(ConsoleKeyInfo obj)
        {
            return obj._keyChar == _keyChar && obj._key == _key && obj._mods == _mods;
        }

        public static bool operator ==(ConsoleKeyInfo a, ConsoleKeyInfo b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(ConsoleKeyInfo a, ConsoleKeyInfo b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return (int)_keyChar | (int)_mods;
        }
    }
}



