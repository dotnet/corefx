// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Runtime.InteropServices.Tests.Common;
using Xunit;

namespace System.Runtime.InteropServices.Tests
{
    public partial class GetStartComSlotTests
    {
        [Fact]
        public void GetStartComSlot_NullType_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => Marshal.GetStartComSlot(null));
        }
    }
}
