// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using System.Security;

namespace System.IO.Compression
{
    /// <summary>
    /// This class provides declaration for constants and PInvokes as well as some basic tools for exposing the
    /// native CLRCompression.dll (effectively, ZLib) library to managed code.
    ///
    /// See also: How to choose a compression level (in comments to <code>CompressionLevel</code>.
    /// </summary>
    internal static partial class ZLibNative
    {
        // This is the NULL pointer for using with ZLib pointers;
        // we prefer it to IntPtr.Zero to mimic the definition of Z_NULL in zlib.h:
        internal static readonly IntPtr ZNullPtr = IntPtr.Zero;

        public enum FlushCode : int
        {
            NoFlush = 0,
            SyncFlush = 2,
            Finish = 4,
        }

        public enum ErrorCode : int
        {
            Ok = 0,
            StreamEnd = 1,
            StreamError = -2,
            DataError = -3,
            MemError = -4,
            BufError = -5,
            VersionError = -6
        }

        /// <summary>
        /// <p>ZLib can accept any integer value between 0 and 9 (inclusive) as a valid compression level parameter:
        /// 1 gives best speed, 9 gives best compression, 0 gives no compression at all (the input data is simply copied a block at a time).
        /// <code>CompressionLevel.DefaultCompression</code> = -1 requests a default compromise between speed and compression
        /// (currently equivalent to level 6).</p>
        ///
        /// <p><strong>How to choose a compression level:</strong></p>
        ///
        /// <p>The names <code>NoCompression</code>, <code>BestSpeed</code>, <code>DefaultCompression</code> are taken over from the corresponding
        /// ZLib definitions, which map to our public NoCompression, Fastest, and Optimal respectively.
        /// <p><em>Optimal Compression:</em></p>
        /// <p><code>ZLibNative.CompressionLevel compressionLevel = ZLibNative.CompressionLevel.DefaultCompression;</code> <br />
        ///    <code>int windowBits = 15;  // or -15 if no headers required</code> <br />
        ///    <code>int memLevel = 8;</code> <br />
        ///    <code>ZLibNative.CompressionStrategy strategy = ZLibNative.CompressionStrategy.DefaultStrategy;</code> </p>
        ///
        ///<p><em>Fastest compression:</em></p>
        ///<p><code>ZLibNative.CompressionLevel compressionLevel = ZLibNative.CompressionLevel.BestSpeed;</code> <br />
        ///   <code>int windowBits = 15;  // or -15 if no headers required</code> <br />
        ///   <code>int memLevel = 8; </code> <br />
        ///   <code>ZLibNative.CompressionStrategy strategy = ZLibNative.CompressionStrategy.DefaultStrategy;</code> </p>
        ///
        /// <p><em>No compression (even faster, useful for data that cannot be compressed such some image formats):</em></p>
        /// <p><code>ZLibNative.CompressionLevel compressionLevel = ZLibNative.CompressionLevel.NoCompression;</code> <br />
        ///    <code>int windowBits = 15;  // or -15 if no headers required</code> <br />
        ///    <code>int memLevel = 7;</code> <br />
        ///    <code>ZLibNative.CompressionStrategy strategy = ZLibNative.CompressionStrategy.DefaultStrategy;</code> </p>
        /// </summary>
        public enum CompressionLevel : int
        {
            NoCompression = 0,
            BestSpeed = 1,
            DefaultCompression = -1
        }

        /// <summary>
        /// <p><strong>From the ZLib manual:</strong></p>
        /// <p><code>CompressionStrategy</code> is used to tune the compression algorithm.<br />
        /// Use the value <code>DefaultStrategy</code> for normal data, <code>Filtered</code> for data produced by a filter (or predictor),
        /// <code>HuffmanOnly</code> to force Huffman encoding only (no string match), or <code>Rle</code> to limit match distances to one
        /// (run-length encoding). Filtered data consists mostly of small values with a somewhat random distribution. In this case, the
        /// compression algorithm is tuned to compress them better. The effect of <code>Filtered</code> is to force more Huffman coding and]
        /// less string matching; it is somewhat intermediate between <code>DefaultStrategy</code> and <code>HuffmanOnly</code>.
        /// <code>Rle</code> is designed to be almost as fast as <code>HuffmanOnly</code>, but give better compression for PNG image data.
        /// The strategy parameter only affects the compression ratio but not the correctness of the compressed output even if it is not set
        /// appropriately. <code>Fixed</code> prevents the use of dynamic Huffman codes, allowing for a simpler decoder for special applications.</p>
        ///
        /// <p><strong>For NetFx use:</strong></p>
        /// <p>We have investigated compression scenarios for a bunch of different frequently occurring compression data and found that in all
        /// cases we investigated so far, <code>DefaultStrategy</code> provided best results</p>
        /// <p>See also: How to choose a compression level (in comments to <code>CompressionLevel</code>.</p>
        /// </summary>
        public enum CompressionStrategy : int
        {
            DefaultStrategy = 0
        }

        /// <summary>
        /// In version 1.2.3, ZLib provides on the <code>Deflated</code>-<code>CompressionMethod</code>.
        /// </summary>
        public enum CompressionMethod : int
        {
            Deflated = 8
        }

        /// <summary>
        /// <p><strong>From the ZLib manual:</strong></p>
        /// <p>ZLib's <code>windowBits</code> parameter is the base two logarithm of the window size (the size of the history buffer).
        /// It should be in the range 8..15 for this version of the library. Larger values of this parameter result in better compression
        /// at the expense of memory usage. The default value is 15 if deflateInit is used instead.<br />
        /// <strong>Note</strong>:
        /// <code>windowBits</code> can also be –8..–15 for raw deflate. In this case, -windowBits determines the window size.
        /// <code>Deflate</code> will then generate raw deflate data with no ZLib header or trailer, and will not compute an adler32 check value.<br />
        /// <p>See also: How to choose a compression level (in comments to <code>CompressionLevel</code>.</p>
        /// </summary>
        public const int Deflate_DefaultWindowBits = -15; // Legal values are 8..15 and -8..-15. 15 is the window size,
                                                          // negative val causes deflate to produce raw deflate data (no zlib header).

        /// <summary>
        /// <p>Zlib's <code>windowBits</code> parameter is the base two logarithm of the window size (the size of the history buffer).
        /// For GZip header encoding, <code>windowBits</code> should be equal to a value between 8..15 (to specify Window Size) added to
        /// 16. The range of values for GZip encoding is therefore 24..31.
        /// <strong>Note</strong>:
        /// The GZip header will have no file name, no extra data, no comment, no modification time (set to zero), no header crc, and
        /// the operating system will be set based on the OS that the ZLib library was compiled to. <code>ZStream.adler</code>
        /// is a crc32 instead of an adler32.</p>
        /// </summary>
        public const int GZip_DefaultWindowBits = 31;

        /// <summary>
        /// <p><strong>From the ZLib manual:</strong></p>
        /// <p>The <code>memLevel</code> parameter specifies how much memory should be allocated for the internal compression state.
        /// <code>memLevel</code> = 1 uses minimum memory but is slow and reduces compression ratio; <code>memLevel</code> = 9 uses maximum
        /// memory for optimal speed. The default value is 8.</p>
        /// <p>See also: How to choose a compression level (in comments to <code>CompressionLevel</code>.</p>
        /// </summary>
        public const int Deflate_DefaultMemLevel = 8;     // Memory usage by deflate. Legal range: [1..9]. 8 is ZLib default.
                                                          // More is faster and better compression with more memory usage.
        public const int Deflate_NoCompressionMemLevel = 7;

        /**
         * Do not remove the nested typing of types inside of <code>System.IO.Compression.ZLibNative</code>.
         * This was done on purpose to:
         *
         * - Achieve the right encapsulation in a situation where <code>ZLibNative</code> may be compiled division-wide
         *   into different assemblies that wish to consume <code>CLRCompression</code>. Since <code>internal</code>
         *   scope is effectively like <code>public</code> scope when compiling <code>ZLibNative</code> into a higher
         *   level assembly, we need a combination of inner types and <code>private</code>-scope members to achieve
         *   the right encapsulation.
         *
         * - Achieve late dynamic loading of <code>CLRCompression.dll</code> at the right time.
         *   The native assembly will not be loaded unless it is actually used since the loading is performed by a static
         *   constructor of an inner type that is not directly referenced by user code.
         *
         *   In Dev12 we would like to create a proper feature for loading native assemblies from user-specified
         *   directories in order to PInvoke into them. This would preferably happen in the native interop/PInvoke
         *   layer; if not we can add a Framework level feature.
         */

        /// <summary>
        /// The <code>ZLibStreamHandle</code> could be a <code>CriticalFinalizerObject</code> rather than a
        /// <code>SafeHandleMinusOneIsInvalid</code>. This would save an <code>IntPtr</code> field since
        /// <code>ZLibStreamHandle</code> does not actually use its <code>handle</code> field.
        /// Instead it uses a <code>private ZStream zStream</code> field which is the actual handle data
        /// structure requiring critical finalization.
        /// However, we would like to take advantage if the better debugability offered by the fact that a
        /// <em>releaseHandleFailed MDA</em> is raised if the <code>ReleaseHandle</code> method returns
        /// <code>false</code>, which can for instance happen if the underlying ZLib <code>XxxxEnd</code>
        /// routines return an failure error code.
        /// </summary>
        public sealed class ZLibStreamHandle : SafeHandle
        {
            public enum State { NotInitialized, InitializedForDeflate, InitializedForInflate, Disposed }

            private ZStream _zStream;

            private volatile State _initializationState;


            public ZLibStreamHandle()
                : base(new IntPtr(-1), true)
            {
                _zStream = new ZStream();
                _zStream.Init();

                _initializationState = State.NotInitialized;
                SetHandle(IntPtr.Zero);
            }

            public override bool IsInvalid
            {
                get { return handle == new IntPtr(-1); }
            }

            public State InitializationState
            {
                [Pure]
                get { return _initializationState; }
            }


            protected override bool ReleaseHandle()
            {
                switch (InitializationState)
                {
                    case State.NotInitialized: return true;
                    case State.InitializedForDeflate: return (DeflateEnd() == ErrorCode.Ok);
                    case State.InitializedForInflate: return (InflateEnd() == ErrorCode.Ok);
                    case State.Disposed: return true;
                    default: return false;  // This should never happen. Did we forget one of the State enum values in the switch?
                }
            }

            public IntPtr NextIn
            {
                get { return _zStream.nextIn; }
                set { _zStream.nextIn = value; }
            }

            public uint AvailIn
            {
                get { return _zStream.availIn; }
                set { _zStream.availIn = value; }
            }

            public IntPtr NextOut
            {
                get { return _zStream.nextOut; }
                set { _zStream.nextOut = value; }
            }

            public uint AvailOut
            {
                get { return _zStream.availOut; }
                set { _zStream.availOut = value; }
            }

            [Pure]
            private void EnsureNotDisposed()
            {
                if (InitializationState == State.Disposed)
                    throw new ObjectDisposedException(GetType().ToString());
            }


            [Pure]
            private void EnsureState(State requiredState)
            {
                if (InitializationState != requiredState)
                    throw new InvalidOperationException("InitializationState != " + requiredState.ToString());
            }


            public ErrorCode DeflateInit2_(CompressionLevel level, int windowBits, int memLevel, CompressionStrategy strategy)
            {
                EnsureNotDisposed();
                EnsureState(State.NotInitialized);

                ErrorCode errC = Interop.zlib.DeflateInit2_(ref _zStream, level, CompressionMethod.Deflated, windowBits, memLevel, strategy);
                _initializationState = State.InitializedForDeflate;

                return errC;
            }


            public ErrorCode Deflate(FlushCode flush)
            {
                EnsureNotDisposed();
                EnsureState(State.InitializedForDeflate);
                return Interop.zlib.Deflate(ref _zStream, flush);
            }


            public ErrorCode DeflateEnd()
            {
                EnsureNotDisposed();
                EnsureState(State.InitializedForDeflate);

                ErrorCode errC = Interop.zlib.DeflateEnd(ref _zStream);
                _initializationState = State.Disposed;

                return errC;
            }


            public ErrorCode InflateInit2_(int windowBits)
            {
                EnsureNotDisposed();
                EnsureState(State.NotInitialized);

                ErrorCode errC = Interop.zlib.InflateInit2_(ref _zStream, windowBits);
                _initializationState = State.InitializedForInflate;

                return errC;
            }


            public ErrorCode Inflate(FlushCode flush)
            {
                EnsureNotDisposed();
                EnsureState(State.InitializedForInflate);
                return Interop.zlib.Inflate(ref _zStream, flush);
            }


            public ErrorCode InflateEnd()
            {
                EnsureNotDisposed();
                EnsureState(State.InitializedForInflate);

                ErrorCode errC = Interop.zlib.InflateEnd(ref _zStream);
                _initializationState = State.Disposed;

                return errC;
            }

            // This can work even after XxflateEnd().
            public string GetErrorMessage() => _zStream.msg != ZNullPtr ? Marshal.PtrToStringAnsi(_zStream.msg) : string.Empty;
        }

        public static ErrorCode CreateZLibStreamForDeflate(out ZLibStreamHandle zLibStreamHandle, CompressionLevel level,
            int windowBits, int memLevel, CompressionStrategy strategy)
        {
            zLibStreamHandle = new ZLibStreamHandle();
            return zLibStreamHandle.DeflateInit2_(level, windowBits, memLevel, strategy);
        }


        public static ErrorCode CreateZLibStreamForInflate(out ZLibStreamHandle zLibStreamHandle, int windowBits)
        {
            zLibStreamHandle = new ZLibStreamHandle();
            return zLibStreamHandle.InflateInit2_(windowBits);
        }
    }
}
