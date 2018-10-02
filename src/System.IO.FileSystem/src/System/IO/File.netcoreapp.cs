// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Diagnostics;

#if MS_IO_REDIST
namespace Microsoft.IO
#else
namespace System.IO
#endif
{
    public static partial class File
    {
        private static void InternalWriteAllBytes(string path, ReadOnlySpan<byte> bytes)
        {
            Debug.Assert(path != null);
            Debug.Assert(path.Length != 0);

            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                fs.Write(bytes);
            }
        }

        public static Utf8String ReadAllTextUtf8(string path)
        {
            // bufferSize == 1 used to avoid unnecessary buffer in FileStream
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 1))
            {
                long fileLength = fs.Length;
                if (fileLength > int.MaxValue)
                {
                    throw new IOException(SR.IO_FileTooLong2GB);
                }
                else if (fileLength == 0)
                {
#if !MS_IO_REDIST
                    // Some file systems (e.g. procfs on Linux) return 0 for length even when there's content.
                    // Thus we need to assume 0 doesn't mean empty.
                    return ReadAllTextUtf8UnknownLength(fs);
#endif
                }

                return Utf8String.Create((int)fileLength, fs, (span, fileStream) =>
                {
                    while (!span.IsEmpty)
                    {
                        int n = fileStream.Read(span);
                        if (n == 0)
                            throw Error.GetEndOfFile();
                        span = span.Slice(n);
                    }
                });
            }
        }

#if !MS_IO_REDIST
        private static Utf8String ReadAllTextUtf8UnknownLength(FileStream fs)
        {
            byte[] rentedArray = null;
            Span<byte> buffer = stackalloc byte[512];
            try
            {
                int bytesRead = 0;
                while (true)
                {
                    if (bytesRead == buffer.Length)
                    {
                        uint newLength = (uint)buffer.Length * 2;
                        if (newLength > MaxByteArrayLength)
                        {
                            newLength = (uint)Math.Max(MaxByteArrayLength, buffer.Length + 1);
                        }

                        byte[] tmp = ArrayPool<byte>.Shared.Rent((int)newLength);
                        buffer.CopyTo(tmp);
                        if (rentedArray != null)
                        {
                            ArrayPool<byte>.Shared.Return(rentedArray);
                        }
                        buffer = rentedArray = tmp;
                    }

                    Debug.Assert(bytesRead < buffer.Length);
                    int n = fs.Read(buffer.Slice(bytesRead));
                    if (n == 0)
                    {
                        return new Utf8String(buffer.Slice(0, bytesRead));
                    }
                    bytesRead += n;
                }
            }
            finally
            {
                if (rentedArray != null)
                {
                    ArrayPool<byte>.Shared.Return(rentedArray);
                }
            }
        }
#endif

        public static void WriteAllTextUtf8(string path, Utf8String contents)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (path.Length == 0)
                throw new ArgumentException(SR.Argument_EmptyPath, nameof(path));

            // Utf8String.AsSpan() extension method below handles null inputs correctly.
            InternalWriteAllBytes(path, contents.AsBytes());
        }
    }
}
