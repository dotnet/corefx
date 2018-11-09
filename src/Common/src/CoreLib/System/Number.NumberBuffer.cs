// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using Internal.Runtime.CompilerServices;

namespace System
{
    internal static partial class Number
    {
        //  We need 1 additional byte, per length, for the terminating null
        private const int DecimalNumberBufferLength = 29 + 1 + 1;  // 29 for the longest input + 1 for rounding
        private const int DoubleNumberBufferLength = 767 + 1 + 1;  // 767 for the longest input + 1 for rounding: 4.9406564584124654E-324 
        private const int Int32NumberBufferLength = 10 + 1;    // 10 for the longest input: 2,147,483,647
        private const int Int64NumberBufferLength = 19 + 1;    // 19 for the longest input: 9,223,372,036,854,775,807
        private const int SingleNumberBufferLength = 112 + 1 + 1;  // 112 for the longest input + 1 for rounding: 1.40129846E-45
        private const int UInt32NumberBufferLength = 10 + 1;   // 10 for the longest input: 4,294,967,295
        private const int UInt64NumberBufferLength = 20 + 1;   // 20 for the longest input: 18,446,744,073,709,551,615

        internal unsafe ref struct NumberBuffer
        {
            public int Precision;
            public int Scale;
            public bool Sign;
            public bool HasNonZeroTail;
            public NumberBufferKind Kind;
            public Span<char> Digits;

            public NumberBuffer(NumberBufferKind kind, char* digits, int digitsLength)
            {
                Debug.Assert(kind == NumberBufferKind.Integer
                    || kind == NumberBufferKind.Decimal
                    || kind == NumberBufferKind.FloatingPoint);
                Debug.Assert(digits != null);
                Debug.Assert(digitsLength > 0);

                Precision = 0;
                Scale = 0;
                Sign = false;
                HasNonZeroTail = false;
                Kind = kind;
                Digits = new Span<char>(digits, digitsLength);
            }

            public char* GetDigitsPointer()
            {
                // This is safe to do since we are a ref struct
                return (char*)(Unsafe.AsPointer(ref Digits[0]));
            }
        }

        internal enum NumberBufferKind : byte
        {
            Unknown = 0,
            Integer = 1,
            Decimal = 2,
            FloatingPoint = 3,
        }
    }
}
