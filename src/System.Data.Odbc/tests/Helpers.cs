using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Odbc.Tests
{
    public static class Helpers
    {
        public const string OdbcIsAvailable = nameof(Helpers) + "." + nameof(CheckOdbcIsAvailable);
        public const string OdbcNotAvailable = nameof(Helpers) + "." + nameof(CheckOdbcNotAvailable);

        public static bool CheckOdbcNotAvailable() => !CheckOdbcIsAvailable();

        private static bool CheckOdbcIsAvailable() => 
            PlatformDetection.IsWindows ? 
                PlatformDetection.IsNotWindowsNanoServer && PlatformDetection.IsNotWindowsServerCore :
                Interop.Libdl.dlopen((
                    PlatformDetection.IsOSX ?
                        "libodbc.2.dylib" : 
                        "libodbc.so.2"
                ), Interop.Libdl.RTLD_NOW) != IntPtr.Zero;
    }
}
