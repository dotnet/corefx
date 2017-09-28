// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

internal partial class Interop
{
    internal partial class Kernel32
    {
        [DllImport(Interop.Libraries.Advapi32, CharSet = System.Runtime.InteropServices.CharSet.Unicode, SetLastError = true)]
        public extern static int LookupAccountSid(string systemName, byte[] pSid, StringBuilder szUserName, ref int userNameSize, StringBuilder szDomainName, ref int domainNameSize, ref int eUse);
    }
}
