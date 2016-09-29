// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class zlib
    {
        /*
            private const string ZLibVersion = "1.2.3";       
        */
        private static readonly byte[] ZLibVersion = new byte[] { (byte)'1', (byte)'.', (byte)'2', (byte)'.', (byte)'3', 0 };

        [DllImport(Libraries.Zlib)]
        private extern unsafe static int deflateInit2_(byte* stream, int level, int method, int windowBits, int memLevel, int strategy,
                                                byte* version, int stream_size);

        [DllImport(Libraries.Zlib)]
        private extern unsafe static int deflate(byte* stream, int flush);

        [DllImport(Libraries.Zlib)]
        private extern unsafe static int deflateEnd(byte* strm);

        [DllImport(Libraries.Zlib)]
        internal extern unsafe static uint crc32(uint crc, byte* buffer, int len);

        [DllImport(Libraries.Zlib)]
        private extern unsafe static int inflateInit2_(byte* stream, int windowBits, byte* version, int stream_size);

        [DllImport(Libraries.Zlib)]
        private extern unsafe static int inflate(byte* stream, int flush);

        [DllImport(Libraries.Zlib)]
        private extern unsafe static int inflateEnd(byte* stream);

        internal static unsafe ZLibNative.ErrorCode DeflateInit2_(
                                            ref ZLibNative.ZStream stream,
                                            ZLibNative.CompressionLevel level,
                                            ZLibNative.CompressionMethod method,
                                            int windowBits,
                                            int memLevel,
                                            ZLibNative.CompressionStrategy strategy)
        {
            fixed (byte* versionString = ZLibVersion)
            fixed (ZLibNative.ZStream* streamBytes = &stream)
            {
                byte* pBytes = (byte*)streamBytes;
                return (ZLibNative.ErrorCode)deflateInit2_(pBytes, (int)level, (int)method, (int)windowBits, (int)memLevel, (int)strategy, versionString, sizeof(ZLibNative.ZStream));
            }
        }

        internal static unsafe uint crc32(uint crc, byte[] buffer, int offset, int len)
        {
            fixed (byte* buf = &buffer[offset])
                return crc32(crc, buf, len);
        }

        internal static unsafe ZLibNative.ErrorCode Deflate(ref ZLibNative.ZStream stream, ZLibNative.FlushCode flush)
        {
            fixed (ZLibNative.ZStream* streamBytes = &stream)
            {
                byte* pBytes = (byte*)streamBytes;
                return (ZLibNative.ErrorCode)deflate(pBytes, (int)flush);
            }
        }

        internal static unsafe ZLibNative.ErrorCode DeflateEnd(ref ZLibNative.ZStream stream)
        {
            fixed (ZLibNative.ZStream* streamBytes = &stream)
            {
                byte* pBytes = (byte*)streamBytes;
                return (ZLibNative.ErrorCode)deflateEnd(pBytes);
            }
        }

        internal static unsafe ZLibNative.ErrorCode InflateInit2_(
                                            ref ZLibNative.ZStream stream,
                                            int windowBits)
        {
            fixed (byte* versionString = ZLibVersion)
            fixed (ZLibNative.ZStream* streamBytes = &stream)
            {
                byte* pBytes = (byte*)streamBytes;
                return (ZLibNative.ErrorCode)inflateInit2_(pBytes, (int)windowBits, versionString, sizeof(ZLibNative.ZStream));
            }
        }

        internal static unsafe ZLibNative.ErrorCode Inflate(ref ZLibNative.ZStream stream, ZLibNative.FlushCode flush)
        {
            fixed (ZLibNative.ZStream* streamBytes = &stream)
            {
                byte* pBytes = (byte*)streamBytes;
                return (ZLibNative.ErrorCode)inflate(pBytes, (int)flush);
            }
        }

        internal static unsafe ZLibNative.ErrorCode InflateEnd(ref ZLibNative.ZStream stream)
        {
            fixed (ZLibNative.ZStream* streamBytes = &stream)
            {
                byte* pBytes = (byte*)streamBytes;
                return (ZLibNative.ErrorCode)inflateEnd(pBytes);
            }
        }
    }
}
