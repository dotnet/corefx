// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Text;

namespace System.IO
{
    /// <summary>Contains internal volume helpers that are shared between many projects.</summary>
    internal static partial class DriveInfoInternal
    {
        public static string[] GetLogicalDrives()
        {
            int drives = Interop.Kernel32.GetLogicalDrives();
            if (drives == 0)
            {
                throw Win32Marshal.GetExceptionForLastWin32Error();
            }

            // GetLogicalDrives returns a bitmask starting from 
            // position 0 "A" indicating whether a drive is present.
            // Loop over each bit, creating a string for each one
            // that is set.

            uint d = (uint)drives;
            int count = 0;
            while (d != 0)
            {
                if (((int)d & 1) != 0) count++;
                d >>= 1;
            }

            string[] result = new string[count];
            Span<char> root = stackalloc char[] { 'A', ':', '\\' };
            d = (uint)drives;
            count = 0;
            while (d != 0)
            {
                if (((int)d & 1) != 0)
                {
                    result[count++] = root.ToString();
                }
                d >>= 1;
                root[0]++;
            }
            return result;
        }

        public static string NormalizeDriveName(string driveName)
        {
            Debug.Assert(driveName != null);

            string name;

            if (driveName.Length == 1)
            {
                name = driveName + ":\\";
            }
            else
            {
                name = Path.GetPathRoot(driveName);
                // Disallow null or empty drive letters and UNC paths
                if (string.IsNullOrEmpty(name) || name.StartsWith("\\\\", StringComparison.Ordinal))
                {
                    throw new ArgumentException(SR.Arg_MustBeDriveLetterOrRootDir, nameof(driveName));
                }
            }
            // We want to normalize to have a trailing backslash so we don't have two equivalent forms and
            // because some Win32 API don't work without it.
            if (name.Length == 2 && name[1] == ':')
            {
                name = name + "\\";
            }

            // Now verify that the drive letter could be a real drive name.
            // On Windows this means it's between A and Z, ignoring case.
            char letter = driveName[0];
            if (!((letter >= 'A' && letter <= 'Z') || (letter >= 'a' && letter <= 'z')))
            {
                throw new ArgumentException(SR.Arg_MustBeDriveLetterOrRootDir, nameof(driveName));
            }

            return name;
        }
    }
}
