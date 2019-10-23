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

namespace System.Security.AccessControl
{
    public sealed class DirectorySecurity : FileSystemSecurity
    {
        #region Constructors

        public DirectorySecurity()
            : base(true)
        {
        }

        public DirectorySecurity(string name, AccessControlSections includeSections)
            : base(true, name, includeSections, true)
        {
            string fullPath = Path.GetFullPath(name);
        }
        #endregion
    }
}

