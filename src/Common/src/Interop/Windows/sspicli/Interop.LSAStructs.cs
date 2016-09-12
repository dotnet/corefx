// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct LSA_TRANSLATED_NAME
    {
        internal int Use;
        internal UNICODE_INTPTR_STRING Name;
        internal int DomainIndex;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct LSA_OBJECT_ATTRIBUTES
    {
        internal int Length;
        internal IntPtr RootDirectory;
        internal IntPtr ObjectName;
        internal int Attributes;
        internal IntPtr SecurityDescriptor;
        internal IntPtr SecurityQualityOfService;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct LSA_TRANSLATED_SID2
    {
        internal int Use;
        internal IntPtr Sid;
        internal int DomainIndex;
        uint Flags;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct LSA_TRUST_INFORMATION
    {
        internal UNICODE_INTPTR_STRING Name;
        internal IntPtr Sid;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct LSA_REFERENCED_DOMAIN_LIST
    {
        internal int Entries;
        internal IntPtr Domains;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct UNICODE_INTPTR_STRING
    {
        internal ushort Length;
        internal ushort MaxLength;
        internal IntPtr Buffer;
    }
}
