// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Runtime.InteropServices.Tests
{
    public class CleanupUnusedObjectsInCurrentContextTests
    {
        [Fact]
        public void CleanupUnusedObjectsInCurrentContext_Invoke_Success()
        {
            Marshal.CleanupUnusedObjectsInCurrentContext();
            Assert.False(Marshal.AreComObjectsAvailableForCleanup());

            Marshal.CleanupUnusedObjectsInCurrentContext();
            Assert.False(Marshal.AreComObjectsAvailableForCleanup());
        }
    }
}
