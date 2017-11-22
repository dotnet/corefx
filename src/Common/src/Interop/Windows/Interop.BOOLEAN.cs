// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal partial class Interop
{
    internal struct BOOLEAN
    {
        internal byte RawValue;

        internal BOOLEAN(bool b)
        {
            RawValue = b ? (byte)1 : (byte)0;
        }

        internal BOOLEAN(byte value)
        {
            RawValue = value;
        }

        internal bool IsTrue => RawValue != 0;

        internal bool IsFalse => RawValue == 0;

        public static implicit operator bool(BOOLEAN b) => b.IsTrue;

        public static implicit operator BOOLEAN(bool b) => new BOOLEAN(b);

        public override string ToString()
        {
            return IsTrue.ToString();
        }
    }
}
