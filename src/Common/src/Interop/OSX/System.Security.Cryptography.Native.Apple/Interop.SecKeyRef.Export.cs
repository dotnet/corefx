// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.Apple;
using Microsoft.Win32.SafeHandles;

internal static partial class Interop
{
    internal static partial class AppleCrypto
    {
        private static readonly SafeCreateHandle s_nullExportString = new SafeCreateHandle();

        [DllImport(Libraries.AppleCryptoNative)]
        private static extern int AppleCryptoNative_SecKeyExport(
            SafeSecKeyRefHandle key,
            int exportPrivate,
            SafeCreateHandle cfExportPassphrase,
            out SafeCFDataHandle cfDataOut,
            out int pOSStatus);

        internal static byte[] SecKeyExport(
            SafeSecKeyRefHandle key,
            bool exportPrivate,
            string password)
        {
            SafeCreateHandle exportPassword = exportPrivate
                ? CoreFoundation.CFStringCreateWithCString(password)
                : s_nullExportString;

            int ret;
            SafeCFDataHandle cfData;
            int osStatus;

            try
            {
                ret = AppleCryptoNative_SecKeyExport(
                    key,
                    exportPrivate ? 1 : 0,
                    exportPassword,
                    out cfData,
                    out osStatus);
            }
            finally
            {
                if (exportPassword != s_nullExportString)
                {
                    exportPassword.Dispose();
                }
            }

            byte[] exportedData;

            using (cfData)
            {
                if (ret == 0)
                {
                    throw CreateExceptionForOSStatus(osStatus);
                }

                if (ret != 1)
                {
                    Debug.Fail($"AppleCryptoNative_SecKeyExport returned {ret}");
                    throw new CryptographicException();
                }

                exportedData = CoreFoundation.CFGetData(cfData);
            }

            return exportedData;
        }
    }
}
