// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class libcorefx_io
    {
        internal struct FileStats
        {
            private Flags Flags;
            internal int Mode;
            internal int Uid;
            internal int Gid;
            internal int Size;
            internal int AccessTime;
            internal int ModificationTime;
            internal int StatusChangeTime;
            internal int CreationTime;

            internal bool HasCreationTime
            {
                get { return (this.Flags & Flags.HasCreationTime) != 0; }
            }

            [Flags]
            private enum Flags
            {
                None = 0,
                HasCreationTime = 1,
            }
        }

        [DllImport(Libraries.LibCoreFxIO, SetLastError = true)]
        internal static extern int GetFileStatsFromDescriptor(int fileDescriptor, out FileStats stats);

        [DllImport(Libraries.LibCoreFxIO, CharSet = CharSet.Ansi, SetLastError = true)]
        internal static extern int GetFileStatsFromPath(string path, out FileStats stats);
    }
}
