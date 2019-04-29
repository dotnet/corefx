// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Runtime.InteropServices
{
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = true)]
    public sealed class PrimaryInteropAssemblyAttribute : Attribute
    {
        public PrimaryInteropAssemblyAttribute(int major, int minor)
        {
            MajorVersion = major;
            MinorVersion = minor;
        }

        public int MajorVersion { get; }
        public int MinorVersion { get; }
    }
}
