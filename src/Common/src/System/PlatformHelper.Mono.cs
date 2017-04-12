// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System
{
    static class PlatformHelper
    {
        internal static readonly bool IsWindows = CheckIfWindows();
        internal static readonly bool IsWin32 = IsWindows && CheckIfWin32();
        internal static readonly bool IsUap = IsWindows && CheckIfUap();
        internal static readonly bool IsUnix = CheckIfUnix();

        static bool CheckIfWindows()
        {
            // It should be replaced by a JIT intrisic
            throw new NotImplementedException();
        }

        static bool CheckIfWin32()
        {
            // It should be replaced by a JIT intrisic
            throw new NotImplementedException();
        }

        static bool CheckIfUap()
        {
            // It should be replaced by a JIT intrisic
            throw new NotImplementedException();
        }

        static bool CheckIfUnix()
        {
            // It should be replaced by a JIT intrisic
            throw new NotImplementedException();
        }
    }
}
