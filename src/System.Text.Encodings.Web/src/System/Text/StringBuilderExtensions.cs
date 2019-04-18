// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text
{
    internal static class StringBuilderExtensions
    {
        public unsafe static void Append(this StringBuilder builder, ReadOnlySpan<char> buffer)
        {
            if (!buffer.IsEmpty)
            {
                fixed (char* pChars = buffer)
                {
                    builder.Append(pChars, buffer.Length);
                }
            }
        }
    }
}
