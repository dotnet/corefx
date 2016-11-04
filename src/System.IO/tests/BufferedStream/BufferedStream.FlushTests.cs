// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using Xunit;

namespace System.IO.Tests
{
    public class BufferedStreamFlushTests
    {
        [Theory]
        public async void ShouldNotFlushUnderlyingStreamIfReadOnly()
        {
            var underlying = new DelegateStream(
                // These properties are necessary for the codepath in question to be hit.
                canReadFunc: () => true,
                canWriteFunc: () => false,
                canSeekFunc: () => true,
                readFunc: (buffer, offset, count) => 123,
                writeFunc: (buffer, offset, count) =>
                {
                    throw new NotSupportedException();
                }
            );
            // These properties are necessary for the codepath in question to be hit.
            Debug.Assert(underlying.CanRead && !underlying.CanWrite && underlying.CanSeek);

            var wrapper = new CallTrackingStream(underlying);

            var buffered = new BufferedStream(wrapper);
            buffered.ReadByte();

            buffered.Flush();
            Assert.Equal(0, wrapper.TimesCalled(nameof(wrapper.Flush)));

            await buffered.FlushAsync();
            Assert.Equal(0, wrapper.TimesCalled(nameof(wrapper.FlushAsync)));
        }
    }
}
