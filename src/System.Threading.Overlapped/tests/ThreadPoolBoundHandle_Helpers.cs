// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Runtime.CompilerServices;
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

    private ThreadPoolBoundHandle CreateThreadPoolBoundHandle([CallerMemberName] string memberName = null, [CallerLineNumber] int lineNumber = 0)
    {
        return CreateThreadPoolBoundHandle((SafeHandle)null, memberName, lineNumber);
    }

    private ThreadPoolBoundHandle CreateThreadPoolBoundHandle(SafeHandle handle, [CallerMemberName] string memberName = null, [CallerLineNumber] int lineNumber = 0)
    {
        handle = handle ?? HandleFactory.CreateAsyncFileHandleForWrite(GetTestFilePath(null, memberName, lineNumber));
        return ThreadPoolBoundHandle.BindHandle(handle);
    }
}
