// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Http
    {
        [DllImport(Libraries.HttpNative, EntryPoint = "HttpNative_EasyCreate")]
        public static extern SafeCurlHandle EasyCreate();

        [DllImport(Libraries.HttpNative, EntryPoint = "HttpNative_EasyDestroy")]
        private static extern void EasyDestroy(IntPtr handle);

        [DllImport(Libraries.HttpNative, EntryPoint = "HttpNative_EasySetOptionString", CharSet = CharSet.Ansi)]
        public static extern CURLcode EasySetOptionString(SafeCurlHandle curl, CURLoption option, string value);

        [DllImport(Libraries.HttpNative, EntryPoint = "HttpNative_EasySetOptionLong")]
        public static extern CURLcode EasySetOptionLong(SafeCurlHandle curl, CURLoption option, long value);

        [DllImport(Libraries.HttpNative, EntryPoint = "HttpNative_EasySetOptionPointer")]
        public static extern CURLcode EasySetOptionPointer(SafeCurlHandle curl, CURLoption option, IntPtr value);

        [DllImport(Libraries.HttpNative, EntryPoint = "HttpNative_EasySetOptionPointer")]
        public static extern CURLcode EasySetOptionPointer(SafeCurlHandle curl, CURLoption option, SafeHandle value);

        [DllImport(Libraries.HttpNative, EntryPoint = "HttpNative_EasyGetErrorString")]
        public static extern IntPtr EasyGetErrorString(int code);

        [DllImport(Libraries.HttpNative, EntryPoint = "HttpNative_EasyGetInfoPointer")]
        public static extern CURLcode EasyGetInfoPointer(IntPtr handle, CURLINFO info, out IntPtr value);

        [DllImport(Libraries.HttpNative, EntryPoint = "HttpNative_EasyGetInfoLong")]
        public static extern CURLcode EasyGetInfoLong(SafeCurlHandle handle, CURLINFO info, out long value);

        [DllImport(Libraries.HttpNative, EntryPoint = "HttpNative_EasyPerform")]
        public static extern CURLcode EasyPerform(SafeCurlHandle curl);

        [DllImport(Libraries.HttpNative, EntryPoint = "HttpNative_EasyUnpause")]
        public static extern CURLcode EasyUnpause(SafeCurlHandle easy);

        public delegate CurlSeekResult SeekCallback(IntPtr userPointer, long offset, int origin);

        public delegate ulong ReadWriteCallback(IntPtr buffer, ulong bufferSize, ulong nitems, IntPtr userPointer);

        public delegate CURLcode SslCtxCallback(IntPtr curl, IntPtr sslCtx, IntPtr userPointer);

        [DllImport(Libraries.HttpNative, EntryPoint = "HttpNative_RegisterSeekCallback")]
        public static extern void RegisterSeekCallback(
            SafeCurlHandle curl,
            SeekCallback callback,
            IntPtr userPointer,
            ref SafeCallbackHandle callbackHandle);

        [DllImport(Libraries.HttpNative, EntryPoint = "HttpNative_RegisterReadWriteCallback")]
        public static extern void RegisterReadWriteCallback(
            SafeCurlHandle curl,
            ReadWriteFunction functionType,
            ReadWriteCallback callback,
            IntPtr userPointer,
            ref SafeCallbackHandle callbackHandle);

        [DllImport(Libraries.HttpNative, EntryPoint = "HttpNative_RegisterSslCtxCallback")]
        public static extern CURLcode RegisterSslCtxCallback(
            SafeCurlHandle curl,
            SslCtxCallback callback,
            IntPtr userPointer,
            ref SafeCallbackHandle callbackHandle);

        [DllImport(Libraries.HttpNative, EntryPoint = "HttpNative_FreeCallbackHandle")]
        private static extern void FreeCallbackHandle(IntPtr handle);

        // Curl options are of the format <type base> + <n>
        private const int CurlOptionLongBase = 0;
        private const int CurlOptionObjectPointBase = 10000;

        // Enum for constants defined for the enum CURLoption in curl.h
        internal enum CURLoption
        {
            CURLOPT_INFILESIZE = CurlOptionLongBase + 14,
            CURLOPT_VERBOSE = CurlOptionLongBase + 41,
            CURLOPT_NOBODY = CurlOptionLongBase + 44,
            CURLOPT_UPLOAD = CurlOptionLongBase + 46,
            CURLOPT_POST = CurlOptionLongBase + 47,
            CURLOPT_FOLLOWLOCATION = CurlOptionLongBase + 52,
            CURLOPT_PROXYPORT = CurlOptionLongBase + 59,
            CURLOPT_POSTFIELDSIZE = CurlOptionLongBase + 60,
            CURLOPT_MAXREDIRS = CurlOptionLongBase + 68,
            CURLOPT_HTTP_VERSION = CurlOptionLongBase + 84,
            CURLOPT_NOSIGNAL = CurlOptionLongBase + 99,
            CURLOPT_PROXYTYPE = CurlOptionLongBase + 101,
            CURLOPT_HTTPAUTH = CurlOptionLongBase + 107,
            CURLOPT_PROTOCOLS = CurlOptionLongBase + 181,
            CURLOPT_REDIR_PROTOCOLS = CurlOptionLongBase + 182,

            CURLOPT_URL = CurlOptionObjectPointBase + 2,
            CURLOPT_PROXY = CurlOptionObjectPointBase + 4,
            CURLOPT_PROXYUSERPWD = CurlOptionObjectPointBase + 6,
            CURLOPT_COOKIE = CurlOptionObjectPointBase + 22,
            CURLOPT_HTTPHEADER = CurlOptionObjectPointBase + 23,
            CURLOPT_CUSTOMREQUEST = CurlOptionObjectPointBase + 36,
            CURLOPT_ACCEPT_ENCODING = CurlOptionObjectPointBase + 102,
            CURLOPT_PRIVATE = CurlOptionObjectPointBase + 103,
            CURLOPT_COPYPOSTFIELDS = CurlOptionObjectPointBase + 165,
            CURLOPT_USERNAME = CurlOptionObjectPointBase + 173,
            CURLOPT_PASSWORD = CurlOptionObjectPointBase + 174,
        }

        internal enum ReadWriteFunction
        {
            Write = 0,
            Read = 1,
            Header = 2,
        }

        // Curl info are of the format <type base> + <n>
        private const int CurlInfoStringBase = 0x100000;
        private const int CurlInfoLongBase = 0x200000;

        // Enum for constants defined for CURL_HTTP_VERSION
        internal enum CurlHttpVersion
        {
            CURL_HTTP_VERSION_NONE = 0,
            CURL_HTTP_VERSION_1_0 = 1,
            CURL_HTTP_VERSION_1_1 = 2,
            CURL_HTTP_VERSION_2_0 = 3,
        };

        // Enum for constants defined for the enum CURLINFO in curl.h
        internal enum CURLINFO
        {
            CURLINFO_PRIVATE = CurlInfoStringBase + 21,
            CURLINFO_HTTPAUTH_AVAIL = CurlInfoLongBase + 23,
        }

        // AUTH related constants
        [Flags]
        internal enum CURLAUTH
        {
            None = 0,
            Basic = 1 << 0,
            Digest = 1 << 1,
            Negotiate = 1 << 2,
        }

        // Enum for constants defined for the enum curl_proxytype in curl.h
        internal enum curl_proxytype
        {
            CURLPROXY_HTTP = 0,
        }

        [Flags]
        internal enum CurlProtocols
        {
            CURLPROTO_HTTP = (1 << 0),
            CURLPROTO_HTTPS = (1 << 1),
        }

        // Enum for constants defined for the results of CURL_SEEKFUNCTION
        internal enum CurlSeekResult : int
        {
            CURL_SEEKFUNC_OK = 0,
            CURL_SEEKFUNC_FAIL = 1,
            CURL_SEEKFUNC_CANTSEEK = 2,
        }

        // constants defined for the results of a CURL_READ or CURL_WRITE function
        internal const ulong CURL_READFUNC_ABORT = 0x10000000;
        internal const ulong CURL_READFUNC_PAUSE = 0x10000001;
        internal const ulong CURL_WRITEFUNC_PAUSE = 0x10000001;

        internal sealed class SafeCurlHandle : SafeHandle
        {
            public SafeCurlHandle() : base(IntPtr.Zero, true)
            {
            }

            public override bool IsInvalid
            {
                get { return handle == IntPtr.Zero; }
            }

            protected override bool ReleaseHandle()
            {
                EasyDestroy(handle);
                SetHandle(IntPtr.Zero);
                return true;
            }
        }

        internal sealed class SafeCallbackHandle : SafeHandle
        {
            public SafeCallbackHandle()
                : base(IntPtr.Zero, true)
            {
            }

            public override bool IsInvalid
            {
                get { return handle == IntPtr.Zero; }
            }

            protected override bool ReleaseHandle()
            {
                FreeCallbackHandle(handle);
                SetHandle(IntPtr.Zero);
                return true;
            }
        }
    }
}
