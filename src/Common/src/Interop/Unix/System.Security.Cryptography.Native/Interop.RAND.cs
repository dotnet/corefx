// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Crypto
    {
        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_GetRandomBytes")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetRandomBytes(byte[] buf, int num);
    }
}
