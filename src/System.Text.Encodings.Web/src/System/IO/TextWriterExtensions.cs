// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.IO
{
    internal static class TextWriterExtensions
    {
        public unsafe static void Write(this TextWriter writer, ReadOnlySpan<char> buffer)
        {
            if (!buffer.IsEmpty)
            {
                foreach (char ch in buffer)
                {
                    writer.Write(ch);
                }
            }
        }
    }
}
