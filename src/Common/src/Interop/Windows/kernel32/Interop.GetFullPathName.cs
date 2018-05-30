// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

internal partial class Interop
{
    internal partial class Kernel32
    {
        /// <summary>
        /// <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/aa364963.aspx">GetFullPathName</a> method.
        /// Retrieves the full path and file name of the specified file.
        /// WARNING: This method does not implicitly handle long paths. Use GetFullPathName or PathHelper.
        /// </summary>
        [DllImport(Interop.Libraries.Kernel32, SetLastError = true, CharSet = CharSet.Unicode)]
        [ResourceExposure(ResourceScope.Machine)]
        internal static extern unsafe int GetFullPathName
            (
                /// <summary>
                /// The name of the file.
                /// In the ANSI version of this function, the name is limited to MAX_PATH which is defined as 260 characters. To extend this limit to 
                /// 32,767 wide characters, call the Unicode version of the function (GetFullPathNameW), and prepend "\\?\" to the path. 
                /// Starting in Windows 10, version 1607, for the unicode version of this function (GetFullPathNameW), you can opt-in to remove the 
                /// MAX_PATH character limitation without prepending "\\?\". See the "Maximum Path Limitation" section of Naming Files, Paths, and Namespaces for details. 
                /// </summary>
                string path,
                
                /// <summary>
                /// The size of the buffer to receive the null-terminated string for the drive and path, in TCHARs.
                /// </summary>
                int numBufferChars,
                
                /// <summary>
                /// A pointer to a buffer that receives the null-terminated string for the drive and path.
                /// </summary>
                System.Text.StringBuilder buffer,
                
                /// <summary>
                /// A pointer to a buffer that receives the address (within lpBuffer) of the final file name component in the path.
                /// </summary>
                IntPtr lpFilePartOrNull
            );

    }
}
