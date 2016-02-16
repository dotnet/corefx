// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class NetNtlmNative
    {
        internal sealed class NtlmException : Exception
        {
            public NtlmException(string message) : base(message)
            {
            }

            public NtlmException(int error)
                : base(SR.Format(SR.net_generic_ntlm_operation_failed, error))
            {
                HResult = error;
            }

            public static void ThrowIfError(int error)
            {
                if (error != 0)
                {
                    var ex = new NtlmException(error);
                    throw ex;
                }
            }
        }
    }
}

