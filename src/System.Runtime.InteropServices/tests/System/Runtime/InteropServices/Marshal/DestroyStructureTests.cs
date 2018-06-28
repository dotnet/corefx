// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Runtime.InteropServices.Tests
{
    public class DestroyStructureTests
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct SomeTestStruct
        {
            public int i;
            public String s;
        }

        [StructLayout(LayoutKind.Auto)]
        public struct SomeTestStruct_Auto
        {
            public int i;
        }

        [Fact]
        public void DestroyStructure()
        {
            IntPtr ip;
            SomeTestStruct someTestStruct = new SomeTestStruct();

            ip = IntPtr.Zero;
            AssertExtensions.Throws<ArgumentNullException>("ptr", () => Marshal.DestroyStructure<SomeTestStruct>(ip));

            ip = new IntPtr(123);
            AssertExtensions.Throws<ArgumentNullException>("structureType", () => Marshal.DestroyStructure(ip, null));

            SomeTestStruct_Auto someTs_Auto = new SomeTestStruct_Auto();
            Assert.Throws<ArgumentException>(() => Marshal.DestroyStructure(ip, someTs_Auto.GetType()));
            
            ip = Marshal.AllocHGlobal(Marshal.SizeOf(someTestStruct));
            try
            {
                someTestStruct.s = null;
                Marshal.StructureToPtr(someTestStruct, ip, false);
                Marshal.DestroyStructure<SomeTestStruct>(ip);
            }
            finally
            {
                Marshal.FreeHGlobal(ip);
            }
        }
    }
}
