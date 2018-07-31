// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace Microsoft.VisualBasic.Devices.Tests
{
    public class NetworkAvailableEventArgsTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Ctor_Bool(bool networkAvailable)
        {
            var args = new NetworkAvailableEventArgs(networkAvailable);
            Assert.Equal(networkAvailable, args.IsNetworkAvailable);
        }
    }
}
