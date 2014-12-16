// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;
using Xunit;

namespace Microsoft.Win32.Primitives.Tests
{
    public static class Win32ExceptionTestType
    {
        [Fact]
        public static void InstantiateException()
        {
            Win32Exception ex = new Win32Exception(5);
            Assert.True(ex.HResult == E_FAIL);
            Assert.True(ex.NativeErrorCode == 5);
        }

        public const int E_FAIL = unchecked((int)0x80004005);

        [Fact]
        public static void InstantiateExceptionWithLongErrorString()
        {
            // Ensure Win32Exception supports error strings greater than 256 chars.
            // ErrorCode 616(ERROR_PWD_TOO_RECENT) has error message string length
            // greater than 256 and hence using it to validate that we support lengths > 256.
            Win32Exception ex = new Win32Exception(616);
            Assert.True(!ex.Message.Equals("Unknown error (0x268)"));
        }
    }
}