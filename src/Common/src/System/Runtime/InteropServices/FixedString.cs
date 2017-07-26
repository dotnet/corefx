// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System
{
    internal static class FixedBufferExtensions
    {
        /// <summary>
        /// Returns a string from the given span, terminating the string at null if present.
        /// </summary>
        internal unsafe static string GetStringFromFixedBuffer(this ReadOnlySpan<char> span)
        {
            fixed (char* c = &span.DangerousGetPinnableReference())
            {
                return new string(c, 0, span.GetFixedBufferStringLength());
            }
        }

        /// <summary>
        /// Gets the null-terminated string length of the given span.
        /// </summary>
        internal unsafe static int GetFixedBufferStringLength(this ReadOnlySpan<char> span)
        {
            int length = span.IndexOf('\0');
            return length < 0 ? span.Length : length;
        }

        /// <summary>
        /// Returns true if the given string equals the given span.
        /// The span's logical length is to the first null if present.
        /// </summary>
        internal unsafe static bool FixedBufferEqualsString(this ReadOnlySpan<char> span, string value)
        {
            if (value == null || value.Length > span.Length)
                return false;

            int stringLength = span.GetFixedBufferStringLength();
            if (stringLength != value.Length)
                return false;

            return span.StartsWith(value.AsReadOnlySpan());
        }
    }
}
