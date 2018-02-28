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
        internal static ReadOnlySpan<char> FastNormalizePath(string path, bool allowTrailingSeparator = false)
        {
            PooledBuffer pooledBuffer = new PooledBuffer();
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

            char[] buffer = null;
            uint result = PathInternal.MaxShortPath;

            while (buffer == null || result > buffer.Length)
            {
                buffer = pooledBuffer.Rent((int)result);
                result = Interop.Kernel32.GetFullPathWraper(path, (uint)buffer.Length, buffer);                               
            }

            if (result != 0 && buffer[result - 1] == '\\')
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

    internal struct PooledBuffer : IDisposable
    {
        public char[] _buffer;

        public char[] Rent(int minLength)
        {
            if (_buffer != null)
                ArrayPool<char>.Shared.Return(_buffer);

            return _buffer = ArrayPool<char>.Shared.Rent(minLength);
        }

        public void Dispose()
        {
            if (_buffer != null)
                ArrayPool<char>.Shared.Return(_buffer);
            _buffer = null;
        }
    }
}
