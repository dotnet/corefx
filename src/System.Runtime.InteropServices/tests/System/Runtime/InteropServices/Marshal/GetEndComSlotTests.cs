// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Runtime.InteropServices.Tests.Common;
using Xunit;

namespace System.Runtime.InteropServices.Tests
{
    public partial class GetEndComSlotTests
    {
        [Fact]
        [PlatformSpecific(TestPlatforms.AnyUnix)]
        public void GetEndComSlot_Unix_ThrowsPlatformNotSupportedException()
        {
            Assert.Throws<PlatformNotSupportedException>(() => Marshal.GetEndComSlot(null));
        }

        [ConditionalTheory(typeof(PlatformDetection), nameof(PlatformDetection.IsNotNetNative))]
        [PlatformSpecific(TestPlatforms.Windows)]
        public void GetEndComSlot_NullObject_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => Marshal.GetEndComSlot(null));
        }
    }
}
