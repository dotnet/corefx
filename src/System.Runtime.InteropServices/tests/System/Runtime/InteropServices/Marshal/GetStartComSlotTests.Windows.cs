// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices.Tests.Common;
using Xunit;

namespace System.Runtime.InteropServices.Tests
{
    public partial class GetStartComSlotTests
    {
        [Theory]
        [InlineData(typeof(DualInterface), 7)]
        [InlineData(typeof(IUnknownInterface), 3)]
        [InlineData(typeof(IDispatchInterface), 7)]
        [InlineData(typeof(IInspectableInterface), 6)]
        [InlineData(typeof(IComImportObject), 7)]
        [InlineData(typeof(DualComObject), 7)]
        [InlineData(typeof(IUnknownComObject), 3)]
        [InlineData(typeof(IDispatchComObject), 7)]
        [InlineData(typeof(IInspectableComObject), 6)]
        [InlineData(typeof(NonDualComObject), 7)]
        [InlineData(typeof(AutoDispatchComObject), 7)]
        [InlineData(typeof(AutoDualComObject), 7)]
        [InlineData(typeof(NonDualComObjectEmpty), -1)]
        [InlineData(typeof(AutoDispatchComObjectEmpty), -1)]
        [InlineData(typeof(AutoDualComObjectEmpty), -1)]
        public void GetStartComSlot_Windows_ReturnsExpected(Type t, int expected)
        {
            Assert.Equal(expected, Marshal.GetStartComSlot(t));
        }
    }
}
