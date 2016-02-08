// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

using Xunit;

namespace System.IO.Tests
{
    public static class DirectoryNotFoundExceptionInteropTests
    {
        [Fact]
        public static void TestFrom_HR()
        {
            var hrs = new int[] { HResults.COR_E_DIRECTORYNOTFOUND, HResults.STG_E_PATHNOTFOUND, HResults.CTL_E_PATHNOTFOUND };
            foreach (var hr in hrs)
            {
                DirectoryNotFoundException exception = Marshal.GetExceptionForHR(hr) as DirectoryNotFoundException;
                Assert.NotNull(exception);

                // Don't validate the message.  Currently .NET Native does not produce HR-specific messages
                ExceptionUtility.ValidateExceptionProperties(exception, hResult: hr, validateMessage: false);
            }
        }
    }
}
