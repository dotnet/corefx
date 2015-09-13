// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Security.Cryptography
{
    public partial class CryptographicException : Exception
    {
        private static string GetMessage(int hr)
        {
            return Interop.libcrypto.ERR_error_string_n(hr);
        }
    }
}
