// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Net
{
    /// <summary>
    /// All external references for System.Net namespaces must be added to this class.
    /// </summary>
    internal static class ExternDll
    {
        // OneCore API Sets
        public const string APIMSWINCOREKERNEL32LEGACYL1 = "api-ms-win-core-kernel32-legacy-l1-1-0.dll";
        public const string APIMSWINCORESYNCHL1 = "api-ms-win-core-synch-l1-1-0.dll";
        public const string APIMSWINCOREIOL1 = "api-ms-win-core-io-l1-1-0.dll";
        public const string APIMSWINCOREHANDLEL1 = "api-ms-win-core-handle-l1-1-0.dll";
        public const string APIMSWINCOREHEAPOBSOLETEL1 = "api-ms-win-core-heap-obsolete-l1-1-0.dll";
        public const string APIMSWINCOREREGISTRYL1 = "api-ms-win-core-registry-l1-1-0.dll";
        public const string APIMSWINCOREDEBUGL1 = "api-ms-win-core-debug-l1-1-0.dll";
        public const string APIMSWINCORELOCALIZATIONL12 = "api-ms-win-core-localization-l1-2-0.dll";
        public const string APIMSWINCORELIBRARYLOADERL1 = "api-ms-win-core-libraryloader-l1-1-0.dll";

        // OneCore Extension API Sets

        // OneCore Non-API-Set dlls
        public const string NTDLL = "ntdll.dll";
        public const string IPHLPAPI = "iphlpapi.dll";
        public const string WS2_32 = "ws2_32.dll";
        public const string MSWSOCK = "mswsock.dll";
        public const string WEBSOCKET = "websocket.dll";
        public const string WINHTTP = "winhttp.dll";
        public const string CRYPT32 = "crypt32.dll";
        public const string SECUR32 = "sspicli.dll";
    }
}
