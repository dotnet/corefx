// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;

#if !netstandard
using Internal.Runtime.CompilerServices;
#endif

#if !netstandard11
using System.Numerics;
#endif

namespace System
{
    internal static partial class SpanHelpers
    {
        private static readonly Vector<UInt16> s_equalityTester = new Vector<UInt16>(UInt16.MaxValue);
        private static readonly Vector<UInt16> s_whiteSpace9Mask = new Vector<UInt16>(0x9);
        private static readonly Vector<UInt16> s_whiteSpace13Mask = new Vector<UInt16>(0xd);
        private static readonly Vector<UInt16> s_whiteSpace32Mask = new Vector<UInt16>(0x20);
        private static readonly Vector<UInt16> s_whiteSpace133Mask = new Vector<UInt16>(0x85);
        private static readonly Vector<UInt16> s_whiteSpace160Mask = new Vector<UInt16>(0xa0);
        private static readonly Vector<UInt16> s_isLatin1Mask = new Vector<UInt16>(0xFF);

        public static bool IsWhiteSpace(ref char first, int length)
        {
            Debug.Assert(length >= 0);

            int i = 0;
#if !netstandard11
            if (Vector.IsHardwareAccelerated && length >= 2 * Vector<UInt16>.Count)
            {
                while (i < length - Vector<UInt16>.Count)
                {
                    Vector<UInt16> value = Unsafe.ReadUnaligned<Vector<UInt16>>(ref Unsafe.As<char, byte>(ref Unsafe.Add(ref first, i)));
                    if (Vector.GreaterThanAny<UInt16>(value, s_isLatin1Mask)) break;

                    if (Vector.GreaterThanAny<UInt16>(value, s_whiteSpace13Mask) ||
                        Vector.LessThanAny<UInt16>(value, s_whiteSpace9Mask))
                    {
                        Vector<UInt16> comparison = Vector<UInt16>.Zero;
                        comparison |= Vector.Equals(value, s_whiteSpace32Mask);
                        comparison |= Vector.Equals(value, s_whiteSpace133Mask);
                        comparison |= Vector.Equals(value, s_whiteSpace160Mask);
                        if (!s_equalityTester.Equals(comparison)) break;
                    }

                    i += Vector<UInt16>.Count;
                }
            }
#endif
            while (i < length)
            {
                if (!char.IsWhiteSpace(Unsafe.Add(ref first, i))) return false;
                i++;
            }
            return true;
        }

        public static int TrimStart(ref char first, int length)
        {
            Debug.Assert(length >= 0);

            int i = 0;
#if !netstandard11
            if (Vector.IsHardwareAccelerated && length >= 2 * Vector<UInt16>.Count)
            {
                while (i < length - Vector<UInt16>.Count)
                {
                    Vector<UInt16> value = Unsafe.ReadUnaligned<Vector<UInt16>>(ref Unsafe.As<char, byte>(ref Unsafe.Add(ref first, i)));
                    if (Vector.GreaterThanAny<UInt16>(value, s_isLatin1Mask)) break;

                    if (Vector.GreaterThanAny<UInt16>(value, s_whiteSpace13Mask) ||
                        Vector.LessThanAny<UInt16>(value, s_whiteSpace9Mask))
                    {
                        Vector<UInt16> comparison = Vector<UInt16>.Zero;
                        comparison |= Vector.Equals(value, s_whiteSpace32Mask);
                        comparison |= Vector.Equals(value, s_whiteSpace133Mask);
                        comparison |= Vector.Equals(value, s_whiteSpace160Mask);
                        if (!s_equalityTester.Equals(comparison)) break;
                    }

                    i += Vector<UInt16>.Count;
                }
            }
#endif
            while (i < length)
            {
                if (!char.IsWhiteSpace(Unsafe.Add(ref first, i))) break;
                i++;
            }
            return i;
        }

        public static int TrimStart(ref char first, char trimChar, int length)
        {
            Debug.Assert(length >= 0);

            int i = 0;
#if !netstandard11
            if (Vector.IsHardwareAccelerated && length >= 2 * Vector<UInt16>.Count)
            {
                var mask = new Vector<UInt16>(trimChar);
                while (i < length - Vector<UInt16>.Count)
                {
                    if (Unsafe.ReadUnaligned<Vector<UInt16>>(ref Unsafe.As<char, byte>(ref Unsafe.Add(ref first, i))) != mask)
                    {
                        break;
                    }
                    i += Vector<UInt16>.Count;
                }
            }
#endif
            while (i < length)
            {
                if (Unsafe.Add(ref first, i) != trimChar) break;
                i++;
            }
            return i;
        }

        public static int TrimStartAny(ref char span, int spanLength, ref char trimChars, int trimCharsLength)
        {
            Debug.Assert(spanLength >= 0);
            Debug.Assert(trimCharsLength >= 0);

            int i = 0;
#if !netstandard11
            if (Vector.IsHardwareAccelerated && spanLength >= 2 * Vector<UInt16>.Count)
            {
                while (i < spanLength - Vector<UInt16>.Count)
                {
                    Vector<UInt16> value = Unsafe.ReadUnaligned<Vector<UInt16>>(ref Unsafe.As<char, byte>(ref Unsafe.Add(ref span, i)));
                    Vector<UInt16> comparison = Vector<UInt16>.Zero;
                    int j = 0;
                    for (; j < trimCharsLength; j++)
                    {
                        var mask = new Vector<UInt16>(Unsafe.Add(ref trimChars, j));
                        comparison |= Vector.Equals(value, mask);
                    }
                    if (!s_equalityTester.Equals(comparison)) break;
                    i += Vector<UInt16>.Count;
                }
            }
#endif
            while (i < spanLength)
            {
                int j = 0;
                while (j < trimCharsLength)
                {
                    if (Unsafe.Add(ref span, i) == Unsafe.Add(ref trimChars, j)) break;
                    j++;
                }
                if (j == trimCharsLength) break;
                i++;
            }
            return i;
        }

        public static int TrimEnd(ref char first, int length)
        {
            Debug.Assert(length >= 0);

            int i = length - 1;
#if !netstandard11
            if (Vector.IsHardwareAccelerated && length >= 2 * Vector<UInt16>.Count)
            {
                do
                {
                    i -= Vector<UInt16>.Count;
                    Vector<UInt16> value = Unsafe.ReadUnaligned<Vector<UInt16>>(ref Unsafe.As<char, byte>(ref Unsafe.Add(ref first, i)));
                    if (Vector.GreaterThanAny<UInt16>(value, s_isLatin1Mask)) break;

                    if (Vector.GreaterThanAny<UInt16>(value, s_whiteSpace13Mask) ||
                        Vector.LessThanAny<UInt16>(value, s_whiteSpace9Mask))
                    {
                        Vector<UInt16> comparison = Vector<UInt16>.Zero;
                        comparison |= Vector.Equals(value, s_whiteSpace32Mask);
                        comparison |= Vector.Equals(value, s_whiteSpace133Mask);
                        comparison |= Vector.Equals(value, s_whiteSpace160Mask);
                        if (!s_equalityTester.Equals(comparison))
                        {
                            i += Vector<UInt16>.Count;
                            break;
                        }
                    }
                } while (i > Vector<UInt16>.Count);
            }
#endif
            while (i >= 0)
            {
                if (!char.IsWhiteSpace(Unsafe.Add(ref first, i))) break;
                i--;
            }
            return i + 1;
        }

        public static int TrimEnd(ref char first, char trimChar, int length)
        {
            Debug.Assert(length >= 0);

            int i = length - 1;
#if !netstandard11
            if (Vector.IsHardwareAccelerated && length >= 2 * Vector<UInt16>.Count)
            {
                var mask = new Vector<UInt16>(trimChar);
                do
                {
                    i -= Vector<UInt16>.Count;
                    if (Unsafe.ReadUnaligned<Vector<UInt16>>(ref Unsafe.As<char, byte>(ref Unsafe.Add(ref first, i))) != mask)
                    {
                        i += Vector<UInt16>.Count;
                        break;
                    }
                } while (i > Vector<UInt16>.Count);
            }
#endif
            while (i >= 0)
            {
                if (Unsafe.Add(ref first, i) != trimChar) break;
                i--;
            }
            return i + 1;
        }

        public static int TrimEndAny(ref char span, int spanLength, ref char trimChars, int trimCharsLength)
        {
            Debug.Assert(spanLength >= 0);
            Debug.Assert(trimCharsLength >= 0);

            int i = spanLength - 1;
#if !netstandard11
            if (Vector.IsHardwareAccelerated && spanLength >= 2 * Vector<UInt16>.Count)
            {
                do
                {
                    i -= Vector<UInt16>.Count;
                    Vector<UInt16> value = Unsafe.ReadUnaligned<Vector<UInt16>>(ref Unsafe.As<char, byte>(ref Unsafe.Add(ref span, i)));
                    Vector<UInt16> comparison = Vector<UInt16>.Zero;
                    int j = 0;
                    for (; j < trimCharsLength; j++)
                    {
                        var mask = new Vector<UInt16>(Unsafe.Add(ref trimChars, j));
                        comparison |= Vector.Equals(value, mask);
                    }
                    if (!s_equalityTester.Equals(comparison))
                    {
                        i += Vector<UInt16>.Count;
                        break;
                    }
                } while (i > Vector<UInt16>.Count);
            }
#endif
            while (i >= 0)
            {
                int j = 0;
                while (j < trimCharsLength)
                {
                    if (Unsafe.Add(ref span, i) == Unsafe.Add(ref trimChars, j)) break;
                    j++;
                }
                if (j == trimCharsLength) break;
                i--;
            }
            return i + 1;
        }
    }
}
