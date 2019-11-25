// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*============================================================
**
**
**
** Purpose: Managed ACL wrapper for files & directories.
**
**
===========================================================*/

using System.IO;
using Microsoft.Win32.SafeHandles;

namespace System.Security.AccessControl
{
    public sealed class FileSecurity : FileSystemSecurity
    {
        #region Constructors

        public FileSecurity()
            : base(false)
        {
        }

        public FileSecurity(string fileName, AccessControlSections includeSections)
            : base(false, fileName, includeSections, false)
        {
            // This will validate the passed path
            Path.GetFullPath(fileName);
        }

        // Warning!  Be exceedingly careful with this constructor.  Do not make
        // it public.  We don't want to get into a situation where someone can
        // pass in the string foo.txt and a handle to bar.exe, and we do a
        // demand on the wrong file name.
        internal FileSecurity(SafeFileHandle handle, string fullPath, AccessControlSections includeSections)
            : base(false, handle, includeSections, false)
        {
        }
        #endregion
    }
}

