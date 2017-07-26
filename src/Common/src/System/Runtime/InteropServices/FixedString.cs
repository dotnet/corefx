// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.IO
{
    internal static class FixedString
    {
        /// <summary>
        /// Returns a string from the given span, terminating the string at null if present.
        /// </summary>
        internal unsafe static string GetString(this Span<char> span)
        {
            int stringLength = span.GetStringLength();
            if (stringLength == 0)
                return string.Empty;

            fixed (char* c = &span[0])
            {
                return new string(c, 0, stringLength);
            }
        }

        /// <summary>
        /// Gets the null-terminated string length of the given span.
        /// </summary>
        internal unsafe static int GetStringLength(this Span<char> span)
        {
            int length = span.IndexOf('\0');
            return length < 0 ? span.Length : length;
        }

        /// <summary>
        /// Returns true if the given string equals the given span.
        /// The span's logical length is to the first null if present.
        /// </summary>
        internal unsafe static bool EqualsString(this Span<char> span, string value)
        {
            if (value == null || value.Length > span.Length)
                return false;

            int stringLength = span.GetStringLength();
            if (stringLength != value.Length)
                return false;

            fixed (char* c = value)
            {
                var source = new ReadOnlySpan<char>(c, value.Length);
                return span.StartsWith(source);
            }
        }

        internal unsafe static void SetString(this Span<char> span, string value, int maxSize)
        {
            if (string.IsNullOrEmpty(value))
            {
                span[0] = '\0';
                return;
            }

            fixed (char* c = value)
            {
                int count = Math.Min(value.Length, span.Length);
                ReadOnlySpan<char> source = new ReadOnlySpan<char>(c, count);
                source.CopyTo(span);
                if (count < span.Length)
                    span[count] = '\0';
            }
        }
    }
}
