// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Text;
using Xunit;

namespace System.Globalization.Tests
{
    public class InvariantModeTests
    {
        [Fact]
        public unsafe void TestZZZ()
        {
            int*[] inarr = new int*[] { (int*)1, (int*)2 };
            object[] oarr = new object[inarr.Length];
            Array.Copy(inarr, oarr, inarr.Length);
        }
    }
}