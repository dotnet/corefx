// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Security.Cryptography
{
    partial class RandomNumberGeneratorImplementation
    {
        private static void GetBytes(ref byte pbBuffer, int count)
        {
            Debug.Assert(count > 0);

            Interop.BCrypt.NTSTATUS status = Interop.BCrypt.BCryptGenRandom(ref pbBuffer, count);
            if (status != Interop.BCrypt.NTSTATUS.STATUS_SUCCESS)
                throw Interop.BCrypt.CreateCryptographicException(status);
        }
    }
}
