// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit;

namespace System.Runtime.InteropServices.RuntimeInformationTests
{
    public class CheckArchitectureTests
    {
        [Fact]
        public void Verify32Bit()
        {
            if (!RuntimeInformation.Is64BitOS())
            {
                Assert.False(RuntimeInformation.Is64BitProcess());
            }
            else
            {
                if (IntPtr.Size == 4)
                {
                    Assert.False(RuntimeInformation.Is64BitProcess());
                }
                else
                {
                    Assert.True(RuntimeInformation.Is64BitProcess());
                }
            }
        }

        [Fact]
        public void Verify64Bit()
        {
            if (RuntimeInformation.Is64BitOS())
            {
                Assert.True(RuntimeInformation.Is64BitProcess());
            }
        }
    }
}
