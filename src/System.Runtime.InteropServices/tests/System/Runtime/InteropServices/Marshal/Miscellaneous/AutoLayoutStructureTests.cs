// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Runtime.InteropServices.Tests
{
    public class AutoLayoutStructureTests
    {
        [StructLayout(LayoutKind.Auto)]
        public struct SomeTestStruct_Auto
        {
            public int i;
        }

        [STAThread]
        [Fact]
        public void AutoLayoutStructureTest()
        {
            SomeTestStruct_Auto someTs_Auto = new SomeTestStruct_Auto();
            try
            {
                Marshal.StructureToPtr(someTs_Auto, new IntPtr(123), true);
                Assert.True(false, "Marshal.StructureToPtr did not throw an exception.");
            }
            catch (ArgumentException ex)
            {
                Assert.True(ex.ParamName == "structure", "Thrown ArgumentException is incorrect.");
                Assert.True(ex.Message.Contains("The specified structure must be blittable or have layout information."), "Thrown ArgumentException is incorrect.");
            }
            catch (Exception)
            {
                Assert.True(false, "Marshal.StructureToPtr threw unexpected exception.");
            }
        }
    }
}
