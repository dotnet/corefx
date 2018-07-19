﻿using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Internal.IO
{
    public static class StringExtensions
    {
        public delegate void SpanAction<T, in TArg>(Span<T> span, TArg arg);

        public static bool Contains(this string s, char value)
        {
            return s.IndexOf(value) != -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool EqualsOrdinal(this ReadOnlySpan<char> span, ReadOnlySpan<char> value)
        {
            if (span.Length != value.Length)
                return false;
            if (value.Length == 0)  // span.Length == value.Length == 0
                return true;
            return span.SequenceEqual(value);
        }

        public unsafe static string Create<TState>(int length, TState state, SpanAction<char, TState> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            if (length <= 0)
            {
                if (length == 0)
                    return string.Empty;
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            string result = new string('\0', length);
            fixed (char* r = result)
            {
                action(new Span<char>(r, length), state);
            }
            return result;
        }
    }
}
