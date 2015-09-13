// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;

namespace System.Security.Cryptography
{
    public partial class CryptographicException : Exception
    {
        private static string GetMessage(int hr)
        {
            string message = Interop.mincore.GetMessage(hr);

#if DEBUG
            // Prepend the hexadecimal version of the hr value
            message = string.Format(
                CultureInfo.InvariantCulture,
                "(0x{0:X8}) {1}",
                hr,
                message);
#endif

            return message;
        }
    }
}
