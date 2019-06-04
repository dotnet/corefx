// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System
{
    public static partial class Environment
    {
#if PROJECTN
        [Internal.Runtime.CompilerServices.RelocatedType("System.Runtime.Extensions")]
#endif
        public enum SpecialFolderOption
        {
            None = 0,
            Create = SpecialFolderOptionValues.CSIDL_FLAG_CREATE,
            DoNotVerify = SpecialFolderOptionValues.CSIDL_FLAG_DONT_VERIFY,
            DefaultPath = SpecialFolderOptionValues.CSIDL_FLAG_DEFAULT_PATH,
            NotParentRelative = SpecialFolderOptionValues.CSIDL_FLAG_NOT_PARENT_RELATIVE,
        }

        // These values are specific to Windows and are known to SHGetFolderPath, however they are
        // also the values used in the SpecialFolderOption enum.  As such, we keep them as constants
        // with their Win32 names, but keep them here rather than in Interop.Kernel32 as they're
        // used on all platforms.
        private static class SpecialFolderOptionValues
        {
            /// <summary>
            /// Force folder creation in SHGetFolderPath. Equivalent of KF_FLAG_CREATE (0x00008000).
            /// </summary>
            internal const int CSIDL_FLAG_CREATE = 0x8000;

            /// <summary>
            /// Return an unverified folder path. Equivalent of KF_FLAG_DONT_VERIFY (0x00004000).
            /// </summary>
            internal const int CSIDL_FLAG_DONT_VERIFY = 0x4000;

            /// <summary>
            /// Return the default path for a known folder. Equivalent of KF_FLAG_DEFAULT_PATH (0x00000400).
            /// </summary>
            internal const int CSIDL_FLAG_DEFAULT_PATH = 0x400;

            /// <summary>
            /// Return the default path independent of the current location of its parent. CSIDL_FLAG_DEFAULT_PATH must also be set.
            /// Equivalent of KF_FLAG_NOT_PARENT_RELATIVE (0x00000200).
            /// </summary>
            internal const int CSIDL_FLAG_NOT_PARENT_RELATIVE = 0x200;
        }
    }
}
