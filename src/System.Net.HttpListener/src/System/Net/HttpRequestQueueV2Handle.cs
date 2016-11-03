// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;
using System.Threading;

namespace System.Net
{
    internal sealed class HttpRequestQueueV2Handle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private HttpRequestQueueV2Handle() : base(true)
        {
        }

        protected override bool ReleaseHandle()
        {
            return (Interop.HttpApi.HttpCloseRequestQueue(handle) == Interop.HttpApi.ERROR_SUCCESS);
        }
    }
}

