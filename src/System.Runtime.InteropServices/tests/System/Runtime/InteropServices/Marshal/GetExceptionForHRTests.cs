// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Runtime.InteropServices.Tests
{
    public class GetExceptionForHRTests
    {
        [Theory]
        [InlineData(unchecked((int)0x80020006))]
        [InlineData(unchecked((int)0x80020101))]
        [ActiveIssue(30866)]
        public void GetExceptionForHR_EqualsErrorCode(int err)
        {
            Exception ex = Marshal.GetExceptionForHR(err);
            Assert.Equal(err, ex.HResult);
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void GetExceptionForHR_ThrowExceptionForHR_ThrowsSameException()
        {
            const int ErrorCode = unchecked((int)0x80131D0B);
            COMException getHRException = Marshal.GetExceptionForHR(ErrorCode) as COMException;
            Assert.Equal(ErrorCode, getHRException.HResult);
            try
            {
                Marshal.ThrowExceptionForHR(ErrorCode);
            }
            catch (COMException e)
            {
                Assert.Equal(ErrorCode, e.HResult);
                Assert.Equal(e.HResult, getHRException.HResult);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void GetExceptionForHR_InvalidHR_ReturnsNull(int errorCode)
        {
            Assert.Null(Marshal.GetExceptionForHR(errorCode));
            Assert.Null(Marshal.GetExceptionForHR(errorCode, IntPtr.Zero));
        }
    }
}
