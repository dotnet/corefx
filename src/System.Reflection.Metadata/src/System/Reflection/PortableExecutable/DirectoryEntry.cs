// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if SRM
namespace System.Reflection.PortableExecutable
{
    public struct DirectoryEntry
    {
        public readonly int RelativeVirtualAddress;
        public readonly int Size;

        public DirectoryEntry(int relativeVirtualAddress, int size)
        {
            RelativeVirtualAddress = relativeVirtualAddress;
            Size = size;
        }

        internal DirectoryEntry(ref PEBinaryReader reader)
        {
            RelativeVirtualAddress = reader.ReadInt32();
            Size = reader.ReadInt32();
        }
    }
}
#else
namespace Microsoft.Cci
{
    // TODO: merge with System.Reflection.PortableExecutable.DirectoryEntry
    internal struct DirectoryEntry
    {
        public readonly int RelativeVirtualAddress;
        public readonly int Size;

        public DirectoryEntry(
            int relativeVirtualAddress,
            int size)
        {
            RelativeVirtualAddress = relativeVirtualAddress;
            Size = size;
        }
    }
}
#endif
