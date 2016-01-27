// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class NetSecurity
    {
        internal sealed class HeimdalNtlmException : Exception
        {
            public HeimdalNtlmException(string message) : base(message)
            {
            }

            public HeimdalNtlmException(int error)
                : base(SR.Format(SR.net_generic_heimntlm_operation_failed, error))
            {
                HResult = error;
            }

            public static void AssertOrThrowIfError(string message, int error)
            {
                if (error != 0)
                {
                    var ex = new HeimdalNtlmException(error);
                    Debug.Fail(message + ": " + ex);
                    throw ex;
                }
            }
        }
    }
}

