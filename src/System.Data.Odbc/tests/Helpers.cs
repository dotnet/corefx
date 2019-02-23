// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Data.Odbc.Tests
{
    public static class Helpers
    {
        public const string OdbcIsAvailable = nameof(Helpers) + "." + nameof(CheckOdbcIsAvailable);
        public const string OdbcNotAvailable = nameof(Helpers) + "." + nameof(CheckOdbcNotAvailable);

        public static bool CheckOdbcNotAvailable() => !CheckOdbcIsAvailable();

        private static bool CheckOdbcIsAvailable() =>
#if TargetsWindows
                !PlatformDetection.IsWindowsNanoServer && (!PlatformDetection.IsWindowsServerCore || Environment.Is64BitProcess);
#else
                NativeLibrary.TryLoad(Interop.Libraries.Odbc32, out _);
#endif
    }
}
