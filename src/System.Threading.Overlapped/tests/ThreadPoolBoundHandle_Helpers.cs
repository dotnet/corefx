// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Xunit;

public partial class ThreadPoolBoundHandleTests : FileCleanupTestBase
{
    struct BlittableType
    {
        public int i;
    }

    struct NonBlittableType
    {
        public string s;
    }

    private ThreadPoolBoundHandle CreateThreadPoolBoundHandle()
    {
        return CreateThreadPoolBoundHandle((SafeHandle)null);
    }

    private ThreadPoolBoundHandle CreateThreadPoolBoundHandle(SafeHandle handle)
    {
        handle = handle ?? HandleFactory.CreateAsyncFileHandleForWrite(GetTestFilePath());

        return ThreadPoolBoundHandle.BindHandle(handle);
    }
}
