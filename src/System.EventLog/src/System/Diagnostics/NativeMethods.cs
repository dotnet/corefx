// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Win32
{
    using System;
    using System.Runtime.InteropServices;

    internal static class NativeMethods
    {

#if !SILVERLIGHT
        public readonly static HandleRef NullHandleRef = new HandleRef(null, IntPtr.Zero);

        public const int WAIT_OBJECT_0 = 0x00000000;
        public const int WAIT_FAILED = unchecked((int)0xFFFFFFFF);
        public const int WAIT_TIMEOUT = 0x00000102;
        public const int WAIT_ABANDONED = 0x00000080;
        public const int WAIT_ABANDONED_0 = WAIT_ABANDONED;

        // copied from winerror.h
        public const int ERROR_INSUFFICIENT_BUFFER = 122;
#endif // !SILVERLIGHT

        public const int FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100;
        public const int FORMAT_MESSAGE_IGNORE_INSERTS = 0x00000200;
        public const int FORMAT_MESSAGE_FROM_STRING = 0x00000400;
        public const int FORMAT_MESSAGE_FROM_HMODULE = 0x00000800;
        public const int FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;
        public const int FORMAT_MESSAGE_ARGUMENT_ARRAY = 0x00002000;
        public const int FORMAT_MESSAGE_MAX_WIDTH_MASK = 0x000000FF;

#if !SILVERLIGHT
#if !FEATURE_PAL
        public const int LOAD_LIBRARY_AS_DATAFILE = 0x00000002;
        public const int SEEK_READ = 0x2;
        public const int FORWARDS_READ = 0x4;
        public const int BACKWARDS_READ = 0x8;
        public const int ERROR_EVENTLOG_FILE_CHANGED = 1503;
#endif // !FEATURE_PAL
#endif // !SILVERLIGHT

#if !SILVERLIGHT || FEATURE_NETCORE
        public const int ERROR_FILE_NOT_FOUND = 2;
#endif // !SILVERLIGHT || FEATURE_NETCORE

    }
    
}
