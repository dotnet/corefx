// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Internal.Runtime.CompilerServices;

namespace System
{
    internal static partial class Number
    {
        private const int NumberMaxDigits = 50;

        private const double Log10V2 = 0.30102999566398119521373889472449;

        // DriftFactor = 1 - Log10V2 - epsilon (a small number account for drift of floating point multiplication)
        private const double DriftFactor = 0.69;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal unsafe ref struct NumberBuffer
        {
            public int precision;                       //  0
            public int scale;                           //  4
            private int _sign;                          //  8
            private NumberBufferKind _kind;             // 12
            private char* _allDigits;                   // 16
            private DigitsAndNullTerminator _digits;    // 20 or 24

            public bool sign { get => _sign != 0; set => _sign = value ? 1 : 0; }
            public char* digits => (char*)Unsafe.AsPointer(ref _digits);
            public NumberBufferKind kind { get => _kind; set => _kind = value; }

            [StructLayout(LayoutKind.Sequential, Size = (NumberMaxDigits + 1) * sizeof(char))]
            private struct DigitsAndNullTerminator { }
        }

        internal enum NumberBufferKind
        {
            Unknown = 0,
            Integer = 1,
            Decimal = 2,
            Double = 3
        }
    }
}
