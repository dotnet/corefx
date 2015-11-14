// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit;

namespace System.Threading.Tasks.Tests
{
    /// <summary>
    /// Single-use, thread-safe flag.
    /// </summary>
    public class Flag
    {
        private int _flag = 0;

        public bool IsTripped { get { return _flag == 1; } }

        public void Trip()
        {
            // Flip flag, and make sure it's only done once.
            Assert.Equal(0, Interlocked.CompareExchange(ref _flag, 1, 0));
        }
    }
}
