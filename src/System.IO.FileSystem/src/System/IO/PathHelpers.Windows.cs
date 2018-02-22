// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;

namespace System.IO
{
    internal static partial class PathHelpers
    {
        /// <summary>
        /// Return fully normalized path. It uses ArrayPool to reduce allocation.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="allowTrailingSeparator"></param>
        internal unsafe static ReadOnlySpan<char> FastNormalizePath(string path, bool allowTrailingSeparator = false)
        {
            if (path.IndexOf('\0') != -1)
                return ReadOnlySpan<char>.Empty;

            if (PathInternal.IsExtended(path))
            {
                if (path.Length > 4 && path[path.Length - 1] == '\\' && path[path.Length - 2] != ':')
                {
                    return allowTrailingSeparator ? path.AsSpan().Slice(0, path.Length - 1) : ReadOnlySpan<char>.Empty;
                }

                return path.AsSpan();
            }

            uint result = 260;
            char[] buffer = null;

            do
            {
                if (buffer != null)
                    ArrayPool<char>.Shared.Return(buffer);

                buffer = ArrayPool<char>.Shared.Rent((int)result);

                fixed (char* c = buffer)
                {
                    result = Interop.Kernel32.GetFullPathNameW(path, (uint)buffer.Length, c, IntPtr.Zero);
                }

                if (result == 0)
                {
                    return ReadOnlySpan<char>.Empty;
                }
            } while (result > buffer.Length);

            if (buffer[result - 1] == '\\')
            {
                if (!allowTrailingSeparator)
                {
                    return ReadOnlySpan<char>.Empty;
                }
                else if (result != 3)
                {
                    // C:\ is a special case, we can't remove the trailing slash in \\?\ format.
                    // There is no valid path that can come back at 3 characters other than C:\
                    return new ReadOnlySpan<char>(buffer, 0, (int)result - 1);
                }
            }

            return new ReadOnlySpan<char>(buffer, 0, (int)result);
        }
    }
}
