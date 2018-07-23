// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Runtime.InteropServices.Tests
{
    public class ThrowExceptionForHRTests
    {
        [Theory]
        [InlineData(unchecked((int)0x80020006))]
        [InlineData(unchecked((int)0x80020101))]
        public void ThrowExceptionForHR_NoErrorInfo_ReturnsValidException(int errorCode)
        {
            ClearCurrentIErrorInfo();

            bool calledCatch = false;
            try
            {
                Marshal.ThrowExceptionForHR(errorCode);
            }
            catch (Exception ex)
            {
                calledCatch = true;

                Assert.IsType<COMException>(ex);
                Assert.Equal(errorCode, ex.HResult);
                Assert.Null(ex.InnerException);
                Assert.Null(ex.HelpLink);
                Assert.NotEmpty(ex.Message);
                if (PlatformDetection.IsNetCore)
                {
                    Assert.Equal("System.Private.CoreLib", ex.Source);
                }
                else
                {
                    Assert.Equal("mscorlib", ex.Source);
                }
                Assert.Contains(nameof(ThrowExceptionForHR_NoErrorInfo_ReturnsValidException), ex.StackTrace);
                Assert.Contains(nameof(Marshal.ThrowExceptionForHR), ex.TargetSite.Name);
            }

            Assert.True(calledCatch, "Expected an exception to be thrown.");
        }

        public static IEnumerable<object[]> ThrowExceptionForHR_ErrorInfo_TestData()
        {
            yield return new object[] { unchecked((int)0x80020006), IntPtr.Zero };
            yield return new object[] { unchecked((int)0x80020101), IntPtr.Zero };
            yield return new object[] { unchecked((int)0x80020006), (IntPtr)(-1) };
            yield return new object[] { unchecked((int)0x80020101), (IntPtr)(-1) };
        }

        [Theory]
        [MemberData(nameof(ThrowExceptionForHR_ErrorInfo_TestData))]
        public void ThrowExceptionForHR_ErrorInfo_ReturnsValidException(int errorCode, IntPtr errorInfo)
        {
            ClearCurrentIErrorInfo();

            bool calledCatch = false;
            try
            {
                Marshal.ThrowExceptionForHR(errorCode, errorInfo);
            }
            catch (Exception ex)
            {
                calledCatch = true;

                Assert.IsType<COMException>(ex);
                Assert.Equal(errorCode, ex.HResult);
                Assert.Null(ex.InnerException);
                Assert.Null(ex.HelpLink);
                Assert.NotEmpty(ex.Message);
                if (PlatformDetection.IsNetCore)
                {
                    Assert.Equal("System.Private.CoreLib", ex.Source);
                }
                else
                {
                    Assert.Equal("mscorlib", ex.Source);
                }
                Assert.Contains(nameof(ThrowExceptionForHR_ErrorInfo_ReturnsValidException), ex.StackTrace);
                Assert.Contains(nameof(Marshal.ThrowExceptionForHR), ex.TargetSite.Name);
            }

            Assert.True(calledCatch, "Expected an exception to be thrown.");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void ThrowExceptionForHR_InvalidHR_Nop(int errorCode)
        {
            Marshal.ThrowExceptionForHR(errorCode);
            Marshal.ThrowExceptionForHR(errorCode, IntPtr.Zero);
        }

        private static void ClearCurrentIErrorInfo()
        {
            // Ensure that if the thread's current IErrorInfo
            // is set during a run that it is thrown away prior
            // to interpreting the HRESULT.
            Marshal.GetExceptionForHR(unchecked((int)0x80040001));
        }
    }
}
