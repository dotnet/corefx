// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.InteropServices;
using Internal.Runtime.CompilerServices;

namespace System
{
    internal static partial class Number
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal unsafe ref struct BigInteger
        {
            // The longest binary mantissa requires: explicit mantissa bits + abs(min exponent)
            // * Half:     10 +    14 =    24
            // * Single:   23 +   126 =   149
            // * Double:   52 +  1022 =  1074
            // * Quad:    112 + 16382 = 16494
            private const int BitsForLongestBinaryMantissa = 1074;

            // The longest digit sequence requires: ceil(log2(pow(10, max significant digits + 1 rounding digit)))
            // * Half:    ceil(log2(pow(10,    21 + 1))) =    74
            // * Single:  ceil(log2(pow(10,   112 + 1))) =   376
            // * Double:  ceil(log2(pow(10,   767 + 1))) =  2552
            // * Quad:    ceil(log2(pow(10, 11563 + 1))) = 38415
            private const int BitsForLongestDigitSequence = 2552;

            // We require BitsPerBlock additional bits for shift space used during the pre-division preparation
            private const int MaxBits = BitsForLongestBinaryMantissa + BitsForLongestDigitSequence + BitsPerBlock;

            private const int BitsPerBlock = sizeof(int) * 8;
            private const int MaxBlockCount = (MaxBits + (BitsPerBlock - 1)) / BitsPerBlock;

            private static readonly uint[] s_Pow10UInt32Table = new uint[]
            {
                1,          // 10^0
                10,         // 10^1
                100,        // 10^2
                1000,       // 10^3
                10000,      // 10^4
                100000,     // 10^5
                1000000,    // 10^6
                10000000,   // 10^7
            };

            private static readonly int[] s_Pow10BigNumTableIndices = new int[]
            {
                0,          // 10^8
                2,          // 10^16
                5,          // 10^32
                10,         // 10^64
                18,         // 10^128
                33,         // 10^256
                61,         // 10^512
                116,        // 10^1024
            };

            private static readonly uint[] s_Pow10BigNumTable = new uint[]
            {
                // 10^8
                1,          // _length
                100000000,  // _blocks

                // 10^16
                2,          // _length
                0x6FC10000, // _blocks
                0x002386F2,

                // 10^32
                4,          // _length
                0x00000000, // _blocks
                0x85ACEF81,
                0x2D6D415B,
                0x000004EE,

                // 10^64
                7,          // _length
                0x00000000, // _blocks
                0x00000000,
                0xBF6A1F01,
                0x6E38ED64,
                0xDAA797ED,
                0xE93FF9F4,
                0x00184F03,

                // 10^128
                14,         // _length
                0x00000000, // _blocks
                0x00000000,
                0x00000000,
                0x00000000,
                0x2E953E01,
                0x03DF9909,
                0x0F1538FD,
                0x2374E42F,
                0xD3CFF5EC,
                0xC404DC08,
                0xBCCDB0DA,
                0xA6337F19,
                0xE91F2603,
                0x0000024E,

                // 10^256
                27,         // _length
                0x00000000, // _blocks
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x982E7C01,
                0xBED3875B,
                0xD8D99F72,
                0x12152F87,
                0x6BDE50C6,
                0xCF4A6E70,
                0xD595D80F,
                0x26B2716E,
                0xADC666B0,
                0x1D153624,
                0x3C42D35A,
                0x63FF540E,
                0xCC5573C0,
                0x65F9EF17,
                0x55BC28F2,
                0x80DCC7F7,
                0xF46EEDDC,
                0x5FDCEFCE,
                0x000553F7,

                // 10^512
                54,         // _length
                0x00000000, // _blocks
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0xFC6CF801,
                0x77F27267,
                0x8F9546DC,
                0x5D96976F,
                0xB83A8A97,
                0xC31E1AD9,
                0x46C40513,
                0x94E65747,
                0xC88976C1,
                0x4475B579,
                0x28F8733B,
                0xAA1DA1BF,
                0x703ED321,
                0x1E25CFEA,
                0xB21A2F22,
                0xBC51FB2E,
                0x96E14F5D,
                0xBFA3EDAC,
                0x329C57AE,
                0xE7FC7153,
                0xC3FC0695,
                0x85A91924,
                0xF95F635E,
                0xB2908EE0,
                0x93ABADE4,
                0x1366732A,
                0x9449775C,
                0x69BE5B0E,
                0x7343AFAC,
                0xB099BC81,
                0x45A71D46,
                0xA2699748,
                0x8CB07303,
                0x8A0B1F13,
                0x8CAB8A97,
                0xC1D238D9,
                0x633415D4,
                0x0000001C,
                
                // 10^1024
                107,        // _length
                0x00000000, // _blocks
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x2919F001,
                0xF55B2B72,
                0x6E7C215B,
                0x1EC29F86,
                0x991C4E87,
                0x15C51A88,
                0x140AC535,
                0x4C7D1E1A,
                0xCC2CD819,
                0x0ED1440E,
                0x896634EE,
                0x7DE16CFB,
                0x1E43F61F,
                0x9FCE837D,
                0x231D2B9C,
                0x233E55C7,
                0x65DC60D7,
                0xF451218B,
                0x1C5CD134,
                0xC9635986,
                0x922BBB9F,
                0xA7E89431,
                0x9F9F2A07,
                0x62BE695A,
                0x8E1042C4,
                0x045B7A74,
                0x1ABE1DE3,
                0x8AD822A5,
                0xBA34C411,
                0xD814B505,
                0xBF3FDEB3,
                0x8FC51A16,
                0xB1B896BC,
                0xF56DEEEC,
                0x31FB6BFD,
                0xB6F4654B,
                0x101A3616,
                0x6B7595FB,
                0xDC1A47FE,
                0x80D98089,
                0x80BDA5A5,
                0x9A202882,
                0x31EB0F66,
                0xFC8F1F90,
                0x976A3310,
                0xE26A7B7E,
                0xDF68368A,
                0x3CE3A0B8,
                0x8E4262CE,
                0x75A351A2,
                0x6CB0B6C9,
                0x44597583,
                0x31B5653F,
                0xC356E38A,
                0x35FAABA6,
                0x0190FBA0,
                0x9FC4ED52,
                0x88BC491B,
                0x1640114A,
                0x005B8041,
                0xF4F3235E,
                0x1E8D4649,
                0x36A8DE06,
                0x73C55349,
                0xA7E6BD2A,
                0xC1A6970C,
                0x47187094,
                0xD2DB49EF,
                0x926C3F5B,
                0xAE6209D4,
                0x2D433949,
                0x34F4A3C6,
                0xD4305D94,
                0xD9D61A05,
                0x00000325,

                // 9 Trailing blocks to ensure MaxBlockCount
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
            };

            private static readonly uint[] s_MultiplyDeBruijnBitPosition = new uint[]
            {
                0, 9, 1, 10, 13, 21, 2, 29, 11, 14, 16, 18, 22, 25, 3, 30,
                8, 12, 20, 28, 15, 17, 24, 7, 19, 27, 23, 6, 26, 5, 4, 31
            };

            private int _length;
            private fixed uint _blocks[MaxBlockCount];

            public BigInteger(uint value)
            {
                _blocks[0] = value;
                _length = (value == 0) ? 0 : 1;
            }

            public BigInteger(ulong value)
            {
                var lower = (uint)(value);
                var upper = (uint)(value >> 32);

                _blocks[0] = lower;
                _blocks[1] = upper;

                _length = (upper == 0) ? 1 : 2;
            }

            public static void Add(ref BigInteger lhs, uint value, ref BigInteger result)
            {
                if (lhs.IsZero())
                {
                    result.SetUInt32(value);
                    return;
                }

                if (value == 0)
                {
                    result.SetValue(ref lhs);
                    return;
                }

                int lhsLength = lhs._length;
                int index = 0;
                uint carry = value;

                while (index < lhsLength)
                {
                    ulong sum = (ulong)(lhs._blocks[index]) + carry;
                    lhs._blocks[index] = (uint)(sum);
                    carry = (uint)(sum >> 32);

                    index++;
                }

                if (carry != 0)
                {
                    Debug.Assert(unchecked((uint)(lhsLength)) + 1 <= MaxBlockCount);
                    result._blocks[index] = carry;
                    result._length = (lhsLength + 1);
                }
            }

            public static int Compare(ref BigInteger lhs, ref BigInteger rhs)
            {
                Debug.Assert(unchecked((uint)(lhs._length)) <= MaxBlockCount);
                Debug.Assert(unchecked((uint)(rhs._length)) <= MaxBlockCount);

                int lhsLength = lhs._length;
                int rhsLength = rhs._length;

                int lengthDelta = (lhsLength - rhsLength);

                if (lengthDelta != 0)
                {
                    return lengthDelta;
                }

                if (lhsLength == 0)
                {
                    Debug.Assert(rhsLength == 0);
                    return 0;
                }

                for (int index = (lhsLength - 1); index >= 0; index--)
                {
                    long delta = (long)(lhs._blocks[index]) - rhs._blocks[index];

                    if (delta != 0)
                    {
                        return delta > 0 ? 1 : -1;
                    }
                }

                return 0;
            }

            public static uint CountSignificantBits(uint value)
            {
                return (value != 0) ? (1 + LogBase2(value)) : 0;
            }

            public static uint CountSignificantBits(ulong value)
            {
                uint upper = (uint)(value >> 32);

                if (upper != 0)
                {
                    return 32 + CountSignificantBits(upper);
                }

                return CountSignificantBits((uint)(value));
            }

            public static uint CountSignificantBits(ref BigInteger value)
            {
                if (value.IsZero())
                {
                    return 0;
                }

                // We don't track any unused blocks, so we only need to do a BSR on the
                // last index and add that to the number of bits we skipped.

                uint lastIndex = (uint)(value._length - 1);
                return (lastIndex * BitsPerBlock) + CountSignificantBits(value._blocks[lastIndex]);
            }

            public static void DivRem(ref BigInteger lhs, ref BigInteger rhs, out BigInteger quo, out BigInteger rem)
            {
                // This is modified from the CoreFX BigIntegerCalculator.DivRem.cs implementation:
                // https://github.com/dotnet/corefx/blob/0bb106232745aedfc0d0c5a84ff2b244bf190317/src/System.Runtime.Numerics/src/System/Numerics/BigIntegerCalculator.DivRem.cs

                Debug.Assert(!rhs.IsZero());

                quo = new BigInteger(0);
                rem = new BigInteger(0);

                if (lhs.IsZero())
                {
                    return;
                }

                int lhsLength = lhs._length;
                int rhsLength = rhs._length;

                if ((lhsLength == 1) && (rhsLength == 1))
                {
                    uint quotient = Math.DivRem(lhs._blocks[0], rhs._blocks[0], out uint remainder);
                    quo = new BigInteger(quotient);
                    rem = new BigInteger(remainder);
                    return;
                }

                if (rhsLength == 1)
                {
                    // We can make the computation much simpler if the rhs is only one block

                    int quoLength = lhsLength;

                    ulong rhsValue = rhs._blocks[0];
                    ulong carry = 0;

                    for (int i = quoLength - 1; i >= 0; i--)
                    {
                        ulong value = (carry << 32) | lhs._blocks[i];
                        ulong digit = Math.DivRem(value, rhsValue, out carry);

                        if ((digit == 0) && (i == (quoLength - 1)))
                        {
                            quoLength--;
                        }
                        else
                        {
                            quo._blocks[i] = (uint)(digit);
                        }
                    }

                    quo._length = quoLength;
                    rem.SetUInt32((uint)(carry));

                    return;
                }
                else if (rhsLength > lhsLength)
                {
                    // Handle the case where we have no quotient
                    rem.SetValue(ref lhs);
                    return;
                }
                else
                {
                    int quoLength = lhsLength - rhsLength + 1;
                    rem.SetValue(ref lhs);
                    int remLength = lhsLength;

                    // Executes the "grammar-school" algorithm for computing q = a / b.
                    // Before calculating q_i, we get more bits into the highest bit
                    // block of the divisor. Thus, guessing digits of the quotient
                    // will be more precise. Additionally we'll get r = a % b.

                    uint divHi = rhs._blocks[rhsLength - 1];
                    uint divLo = rhs._blocks[rhsLength - 2];

                    // We measure the leading zeros of the divisor
                    int shiftLeft = (int)(LeadingZeroCount(divHi));
                    int shiftRight = 32 - shiftLeft;

                    // And, we make sure the most significant bit is set
                    if (shiftLeft > 0)
                    {
                        divHi = (divHi << shiftLeft) | (divLo >> shiftRight);
                        divLo = (divLo << shiftLeft);

                        if (rhsLength > 2)
                        {
                            divLo |= (rhs._blocks[rhsLength - 3] >> shiftRight);
                        }
                    }

                    // Then, we divide all of the bits as we would do it using
                    // pen and paper: guessing the next digit, subtracting, ...
                    for (int i = lhsLength; i >= rhsLength; i--)
                    {
                        int n = i - rhsLength;
                        uint t = i < lhsLength ? rem._blocks[i] : 0;

                        ulong valHi = ((ulong)(t) << 32) | rem._blocks[i - 1];
                        uint valLo = i > 1 ? rem._blocks[i - 2] : 0;

                        // We shifted the divisor, we shift the dividend too
                        if (shiftLeft > 0)
                        {
                            valHi = (valHi << shiftLeft) | (valLo >> shiftRight);
                            valLo = (valLo << shiftLeft);

                            if (i > 2)
                            {
                                valLo |= (rem._blocks[i - 3] >> shiftRight);
                            }
                        }

                        // First guess for the current digit of the quotient,
                        // which naturally must have only 32 bits...
                        ulong digit = valHi / divHi;

                        if (digit > uint.MaxValue)
                        {
                            digit = uint.MaxValue;
                        }

                        // Our first guess may be a little bit to big
                        while (DivideGuessTooBig(digit, valHi, valLo, divHi, divLo))
                        {
                            digit--;
                        }

                        if (digit > 0)
                        {
                            // Now it's time to subtract our current quotient
                            uint carry = SubtractDivisor(ref rem, n, ref rhs, digit);

                            if (carry != t)
                            {
                                Debug.Assert(carry == t + 1);

                                // Our guess was still exactly one too high
                                carry = AddDivisor(ref rem, n, ref rhs);
                                digit--;

                                Debug.Assert(carry == 1);
                            }
                        }

                        // We have the digit!
                        if (quoLength != 0)
                        {
                            if ((digit == 0) && (n == (quoLength - 1)))
                            {
                                quoLength--;
                            }
                            else
                            {
                                quo._blocks[n] = (uint)(digit);
                            }
                        }

                        if (i < remLength)
                        {
                            remLength--;
                        }
                    }

                    quo._length = quoLength;

                    // We need to check for the case where remainder is zero

                    for (int i = remLength - 1; i >= 0; i--)
                    {
                        if (rem._blocks[i] == 0)
                        {
                            remLength--;
                        }
                    }

                    rem._length = remLength;
                }
            }

            public static uint HeuristicDivide(ref BigInteger dividend, ref BigInteger divisor)
            {
                int divisorLength = divisor._length;

                if (dividend._length < divisorLength)
                {
                    return 0;
                }

                // This is an estimated quotient. Its error should be less than 2.
                // Reference inequality:
                // a/b - floor(floor(a)/(floor(b) + 1)) < 2
                int lastIndex = (divisorLength - 1);
                uint quotient = dividend._blocks[lastIndex] / (divisor._blocks[lastIndex] + 1);

                if (quotient != 0)
                {
                    // Now we use our estimated quotient to update each block of dividend.
                    // dividend = dividend - divisor * quotient
                    int index = 0;

                    ulong borrow = 0;
                    ulong carry = 0;

                    do
                    {
                        ulong product = ((ulong)(divisor._blocks[index]) * quotient) + carry;
                        carry = product >> 32;

                        ulong difference = (ulong)(dividend._blocks[index]) - (uint)(product) - borrow;
                        borrow = (difference >> 32) & 1;

                        dividend._blocks[index] = (uint)(difference);

                        index++;
                    }
                    while (index < divisorLength);

                    // Remove all leading zero blocks from dividend
                    while ((divisorLength > 0) && (dividend._blocks[divisorLength - 1] == 0))
                    {
                        divisorLength--;
                    }

                    dividend._length = divisorLength;
                }

                // If the dividend is still larger than the divisor, we overshot our estimate quotient. To correct,
                // we increment the quotient and subtract one more divisor from the dividend (Because we guaranteed the error range).
                if (Compare(ref dividend, ref divisor) >= 0)
                {
                    quotient++;

                    // dividend = dividend - divisor
                    int index = 0;
                    ulong borrow = 0;

                    do
                    {
                        ulong difference = (ulong)(dividend._blocks[index]) - divisor._blocks[index] - borrow;
                        borrow = (difference >> 32) & 1;

                        dividend._blocks[index] = (uint)(difference);

                        index++;
                    }
                    while (index < divisorLength);

                    // Remove all leading zero blocks from dividend
                    while ((divisorLength > 0) && (dividend._blocks[divisorLength - 1] == 0))
                    {
                        divisorLength--;
                    }

                    dividend._length = divisorLength;
                }

                return quotient;
            }

            public static uint LeadingZeroCount(uint value)
            {
                return 32 - CountSignificantBits(value);
            }

            public static uint LeadingZeroCount(ulong value)
            {
                return 64 - CountSignificantBits(value);
            }

            public static uint LogBase2(uint value)
            {
                Debug.Assert(value != 0);

                // This comes from the Stanford Bit Widdling Hacks by Sean Eron Anderson:
                // http://graphics.stanford.edu/~seander/bithacks.html#IntegerLogDeBruijn

                value |= (value >> 1); // first round down to one less than a power of 2 
                value |= (value >> 2);
                value |= (value >> 4);
                value |= (value >> 8);
                value |= (value >> 16);

                uint index = (value * 0x07C4ACDD) >> 27;
                return s_MultiplyDeBruijnBitPosition[(int)(index)];
            }

            public static uint LogBase2(ulong value)
            {
                Debug.Assert(value != 0);

                uint upper = (uint)(value >> 32);

                if (upper != 0)
                {
                    return 32 + LogBase2(upper);
                }

                return LogBase2((uint)(value));
            }

            public static void Multiply(ref BigInteger lhs, uint value, ref BigInteger result)
            {
                if (lhs.IsZero() || (value == 1))
                {
                    result.SetValue(ref lhs);
                    return;
                }

                if (value == 0)
                {
                    result.SetZero();
                    return;
                }

                int lhsLength = lhs._length;
                int index = 0;
                uint carry = 0;

                while (index < lhsLength)
                {
                    ulong product = ((ulong)(lhs._blocks[index]) * value) + carry;
                    result._blocks[index] = (uint)(product);
                    carry = (uint)(product >> 32);

                    index++;
                }

                if (carry != 0)
                {
                    Debug.Assert(unchecked((uint)(lhsLength)) + 1 <= MaxBlockCount);
                    result._blocks[index] = carry;
                    result._length = (lhsLength + 1);
                }
            }

            public static void Multiply(ref BigInteger lhs, ref BigInteger rhs, ref BigInteger result)
            {
                if (lhs.IsZero() || rhs.IsOne())
                {
                    result.SetValue(ref lhs);
                    return;
                }

                if (rhs.IsZero())
                {
                    result.SetZero();
                    return;
                }

                ref readonly BigInteger large = ref lhs;
                int largeLength = lhs._length;

                ref readonly BigInteger small = ref rhs;
                int smallLength = rhs._length;

                if (largeLength < smallLength)
                {
                    large = ref rhs;
                    largeLength = rhs._length;

                    small = ref lhs;
                    smallLength = lhs._length;
                }

                int maxResultLength = smallLength + largeLength;
                Debug.Assert(unchecked((uint)(maxResultLength)) <= MaxBlockCount);

                // Zero out result internal blocks.
                Buffer.ZeroMemory((byte*)(result.GetBlocksPointer()), (maxResultLength * sizeof(uint)));

                int smallIndex = 0;
                int resultStartIndex = 0;

                while (smallIndex < smallLength)
                {
                    // Multiply each block of large BigNum.
                    if (small._blocks[smallIndex] != 0)
                    {
                        int largeIndex = 0;
                        int resultIndex = resultStartIndex;

                        ulong carry = 0;

                        do
                        {
                            ulong product = result._blocks[resultIndex] + ((ulong)(small._blocks[smallIndex]) * large._blocks[largeIndex]) + carry;
                            carry = product >> 32;
                            result._blocks[resultIndex] = (uint)(product);

                            resultIndex++;
                            largeIndex++;
                        }
                        while (largeIndex < largeLength);

                        result._blocks[resultIndex] = (uint)(carry);
                    }

                    smallIndex++;
                    resultStartIndex++;
                }

                if ((maxResultLength > 0) && (result._blocks[maxResultLength - 1] == 0))
                {
                    result._length = (maxResultLength - 1);
                }
                else
                {
                    result._length = maxResultLength;
                }
            }

            public static void Pow10(uint exponent, out BigInteger result)
            {
                // We leverage two arrays - s_Pow10UInt32Table and s_Pow10BigNumTable to speed up the Pow10 calculation.
                //
                // s_Pow10UInt32Table stores the results of 10^0 to 10^7.
                // s_Pow10BigNumTable stores the results of 10^8, 10^16, 10^32, 10^64, 10^128, 10^256, and 10^512
                //
                // For example, let's say exp = 0b111111. We can split the exp to two parts, one is small exp, 
                // which 10^smallExp can be represented as uint, another part is 10^bigExp, which must be represented as BigNum. 
                // So the result should be 10^smallExp * 10^bigExp.
                //
                // Calculating 10^smallExp is simple, we just lookup the 10^smallExp from s_Pow10UInt32Table. 
                // But here's a bad news: although uint can represent 10^9, exp 9's binary representation is 1001. 
                // That means 10^(1011), 10^(1101), 10^(1111) all cannot be stored as uint, we cannot easily say something like: 
                // "Any bits <= 3 is small exp, any bits > 3 is big exp". So instead of involving 10^8, 10^9 to s_Pow10UInt32Table, 
                // consider 10^8 and 10^9 as a bigNum, so they fall into s_Pow10BigNumTable. Now we can have a simple rule: 
                // "Any bits <= 3 is small exp, any bits > 3 is big exp".
                //
                // For 0b111111, we first calculate 10^(smallExp), which is 10^(7), now we can shift right 3 bits, prepare to calculate the bigExp part, 
                // the exp now becomes 0b000111.
                //
                // Apparently the lowest bit of bigExp should represent 10^8 because we have already shifted 3 bits for smallExp, so s_Pow10BigNumTable[0] = 10^8.
                // Now let's shift exp right 1 bit, the lowest bit should represent 10^(8 * 2) = 10^16, and so on...
                //
                // That's why we just need the values of s_Pow10BigNumTable be power of 2.
                //
                // More details of this implementation can be found at: https://github.com/dotnet/coreclr/pull/12894#discussion_r128890596

                // Validate that `s_Pow10BigNumTable` has exactly enough trailing elements to fill a BigInteger (which contains MaxBlockCount + 1 elements)
                // We validate here, since this is the only current consumer of the array
                Debug.Assert((s_Pow10BigNumTableIndices[s_Pow10BigNumTableIndices.Length - 1] + MaxBlockCount + 2) == s_Pow10BigNumTable.Length);

                BigInteger temp1 = new BigInteger(s_Pow10UInt32Table[exponent & 0x7]);
                ref BigInteger lhs = ref temp1;

                BigInteger temp2 = new BigInteger(0);
                ref BigInteger product = ref temp2;

                exponent >>= 3;
                uint index = 0;

                while (exponent != 0)
                {
                    // If the current bit is set, multiply it with the corresponding power of 10
                    if ((exponent & 1) != 0)
                    {
                        // Multiply into the next temporary
                        ref BigInteger rhs = ref *(BigInteger*)(Unsafe.AsPointer(ref s_Pow10BigNumTable[s_Pow10BigNumTableIndices[index]]));
                        Multiply(ref lhs, ref rhs, ref product);

                        // Swap to the next temporary
                        ref BigInteger temp = ref product;
                        product = ref lhs;
                        lhs = ref temp;
                    }

                    // Advance to the next bit
                    ++index;
                    exponent >>= 1;
                }

                result = new BigInteger(0);
                result.SetValue(ref lhs);
            }

            public static void PrepareHeuristicDivide(ref BigInteger dividend, ref BigInteger divisor)
            {
                uint hiBlock = divisor._blocks[divisor._length - 1];

                if ((hiBlock < 8) || (hiBlock > 429496729))
                {
                    // Inspired by http://www.ryanjuckett.com/programming/printing-floating-point-numbers/
                    // Perform a bit shift on all values to get the highest block of the divisor into
                    // the range [8,429496729]. We are more likely to make accurate quotient estimations
                    // in heuristicDivide() with higher divisor values so
                    // we shift the divisor to place the highest bit at index 27 of the highest block.
                    // This is safe because (2^28 - 1) = 268435455 which is less than 429496729. This means
                    // that all values with a highest bit at index 27 are within range.
                    uint hiBlockLog2 = LogBase2(hiBlock);
                    uint shift = (59 - hiBlockLog2) % 32;

                    divisor.ShiftLeft(shift);
                    dividend.ShiftLeft(shift);
                }
            }

            public static void ShiftLeft(ulong input, uint shift, ref BigInteger output)
            {
                if (shift == 0)
                {
                    return;
                }

                uint blocksToShift = Math.DivRem(shift, 32, out uint remainingBitsToShift);

                if (blocksToShift > 0)
                {
                    // If blocks shifted, we should fill the corresponding blocks with zero.
                    output.ExtendBlocks(0, blocksToShift);
                }

                if (remainingBitsToShift == 0)
                {
                    // We shift 32 * n (n >= 1) bits. No remaining bits.
                    output.ExtendBlock((uint)(input));

                    uint highBits = (uint)(input >> 32);

                    if (highBits != 0)
                    {
                        output.ExtendBlock(highBits);
                    }
                }
                else
                {
                    // Extract the high position bits which would be shifted out of range.
                    uint highPositionBits = (uint)(input) >> (int)(64 - remainingBitsToShift);

                    // Shift the input. The result should be stored to current block.
                    ulong shiftedInput = input << (int)(remainingBitsToShift);
                    output.ExtendBlock((uint)(shiftedInput));

                    uint highBits = (uint)(input >> 32);

                    if (highBits != 0)
                    {
                        output.ExtendBlock(highBits);
                    }

                    if (highPositionBits != 0)
                    {
                        // If the high position bits is not 0, we should store them to next block.
                        output.ExtendBlock(highPositionBits);
                    }
                }
            }

            private static unsafe uint AddDivisor(ref BigInteger lhs, int lhsStartIndex, ref BigInteger rhs)
            {
                int lhsLength = lhs._length;
                int rhsLength = rhs._length;

                Debug.Assert(lhsLength >= 0);
                Debug.Assert(rhsLength >= 0);
                Debug.Assert(lhsLength >= rhsLength);

                // Repairs the dividend, if the last subtract was too much

                ulong carry = 0UL;

                for (int i = 0; i < rhsLength; i++)
                {
                    ref uint lhsValue = ref lhs._blocks[lhsStartIndex + i];

                    ulong digit = (lhsValue + carry) + rhs._blocks[i];
                    lhsValue = unchecked((uint)digit);
                    carry = digit >> 32;
                }

                return (uint)(carry);
            }

            private static bool DivideGuessTooBig(ulong q, ulong valHi, uint valLo, uint divHi, uint divLo)
            {
                Debug.Assert(q <= 0xFFFFFFFF);

                // We multiply the two most significant limbs of the divisor
                // with the current guess for the quotient. If those are bigger
                // than the three most significant limbs of the current dividend
                // we return true, which means the current guess is still too big.

                ulong chkHi = divHi * q;
                ulong chkLo = divLo * q;

                chkHi = chkHi + (chkLo >> 32);
                chkLo = chkLo & uint.MaxValue;

                if (chkHi < valHi)
                    return false;

                if (chkHi > valHi)
                    return true;

                if (chkLo < valLo)
                    return false;

                if (chkLo > valLo)
                    return true;

                return false;
            }

            private static unsafe uint SubtractDivisor(ref BigInteger lhs, int lhsStartIndex, ref BigInteger rhs, ulong q)
            {
                int lhsLength = lhs._length - lhsStartIndex;
                int rhsLength = rhs._length;

                Debug.Assert((lhsLength) >= 0);
                Debug.Assert(rhsLength >= 0);
                Debug.Assert(lhsLength >= rhsLength);
                Debug.Assert(q <= uint.MaxValue);

                // Combines a subtract and a multiply operation, which is naturally
                // more efficient than multiplying and then subtracting...

                ulong carry = 0;

                for (int i = 0; i < rhsLength; i++)
                {
                    carry += rhs._blocks[i] * q;
                    uint digit = unchecked((uint)carry);
                    carry = carry >> 32;

                    ref uint lhsValue = ref lhs._blocks[lhsStartIndex + i];

                    if (lhsValue < digit)
                    {
                        carry++;
                    }

                    lhsValue = unchecked(lhsValue - digit);
                }

                return (uint)(carry);
            }

            public void Add(uint value)
            {
                Add(ref this, value, ref this);
            }

            public void ExtendBlock(uint blockValue)
            {
                _blocks[_length] = blockValue;
                _length++;
            }

            public void ExtendBlocks(uint blockValue, uint blockCount)
            {
                Debug.Assert(blockCount > 0);

                if (blockCount == 1)
                {
                    ExtendBlock(blockValue);

                    return;
                }

                Buffer.ZeroMemory((byte*)(GetBlocksPointer() + _length), ((blockCount - 1) * sizeof(uint)));
                _length += (int)(blockCount);
                _blocks[_length - 1] = blockValue;
            }

            public uint GetBlock(uint index)
            {
                Debug.Assert(index < _length);
                return _blocks[index];
            }

            public bool IsOne()
            {
                return (_length == 1)
                    && (_blocks[0] == 1);
            }

            public bool IsZero()
            {
                return _length == 0;
            }

            public void Multiply(uint value)
            {
                Multiply(ref this, value, ref this);
            }

            public void Multiply(ref BigInteger value)
            {
                var result = new BigInteger(0);
                Multiply(ref this, ref value, ref result);

                Buffer.Memcpy((byte*)(GetBlocksPointer()), (byte*)(result.GetBlocksPointer()), (result._length) * sizeof(uint));
                _length = result._length;
            }

            public void Multiply10()
            {
                if (IsZero())
                {
                    return;
                }

                int index = 0;
                int length = _length;
                ulong carry = 0;

                while (index < length)
                {
                    var block = (ulong)(_blocks[index]);
                    ulong product = (block << 3) + (block << 1) + carry;
                    carry = product >> 32;
                    _blocks[index] = (uint)(product);

                    index++;
                }

                if (carry != 0)
                {
                    Debug.Assert(unchecked((uint)(_length)) + 1 <= MaxBlockCount);
                    _blocks[index] = (uint)(carry);
                    _length += 1;
                }
            }

            public void MultiplyPow10(uint exponent)
            {
                Pow10(exponent, out BigInteger poweredValue);

                if (poweredValue._length == 1)
                {
                    Multiply(poweredValue._blocks[0]);
                }
                else
                {
                    Multiply(ref poweredValue);
                }
            }

            public void SetUInt32(uint value)
            {
                _blocks[0] = value;
                _length = 1;
            }

            public void SetUInt64(ulong value)
            {
                var lower = (uint)(value);
                var upper = (uint)(value >> 32);

                _blocks[0] = lower;
                _blocks[1] = upper;

                _length = (upper == 0) ? 1 : 2;
            }

            public void SetValue(ref BigInteger rhs)
            {
                int rhsLength = rhs._length;
                Buffer.Memcpy((byte*)(GetBlocksPointer()), (byte*)(rhs.GetBlocksPointer()), (rhsLength * sizeof(uint)));
                _length = rhsLength;
            }

            public void SetZero()
            {
                _length = 0;
            }

            public void ShiftLeft(uint shift)
            {
                // Process blocks high to low so that we can safely process in place
                var length = _length;

                if ((length == 0) || (shift == 0))
                {
                    return;
                }

                uint blocksToShift = Math.DivRem(shift, 32, out uint remainingBitsToShift);

                // Copy blocks from high to low
                int readIndex = (length - 1);
                int writeIndex = readIndex + (int)(blocksToShift);

                // Check if the shift is block aligned
                if (remainingBitsToShift == 0)
                {
                    Debug.Assert(writeIndex < MaxBlockCount);

                    while (readIndex >= 0)
                    {
                        _blocks[writeIndex] = _blocks[readIndex];
                        readIndex--;
                        writeIndex--;
                    }

                    _length += (int)(blocksToShift);

                    // Zero the remaining low blocks
                    Buffer.ZeroMemory((byte*)(GetBlocksPointer()), (blocksToShift * sizeof(uint)));
                }
                else
                {
                    // We need an extra block for the partial shift
                    writeIndex++;
                    Debug.Assert(writeIndex < MaxBlockCount);

                    // Set the length to hold the shifted blocks
                    _length = writeIndex + 1;

                    // Output the initial blocks
                    uint lowBitsShift = (32 - remainingBitsToShift);
                    uint highBits = 0;
                    uint block = _blocks[readIndex];
                    uint lowBits = block >> (int)(lowBitsShift);
                    while (readIndex > 0)
                    {
                        _blocks[writeIndex] = highBits | lowBits;
                        highBits = block << (int)(remainingBitsToShift);

                        --readIndex;
                        --writeIndex;

                        block = _blocks[readIndex];
                        lowBits = block >> (int)lowBitsShift;
                    }

                    // Output the final blocks
                    _blocks[writeIndex] = highBits | lowBits;
                    _blocks[writeIndex - 1] = block << (int)(remainingBitsToShift);

                    // Zero the remaining low blocks
                    Buffer.ZeroMemory((byte*)(GetBlocksPointer()), (blocksToShift * sizeof(uint)));

                    // Check if the terminating block has no set bits
                    if (_blocks[_length - 1] == 0)
                    {
                        _length--;
                    }
                }
            }

            public ulong ToUInt64()
            {
                if (_length > 1)
                {
                    return ((ulong)(_blocks[1]) << 32) + _blocks[0];
                }

                if (_length > 0)
                {
                    return _blocks[0];
                }

                return 0;
            }

            private uint* GetBlocksPointer()
            {
                // This is safe to do since we are a ref struct
                return (uint*)(Unsafe.AsPointer(ref _blocks[0]));
            }
        }
    }
}
