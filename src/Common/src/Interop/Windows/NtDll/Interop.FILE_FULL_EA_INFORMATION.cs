// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class NtDll
    {
		/// <summary>
        /// <a href="https://docs.microsoft.com/en-us/windows-hardware/drivers/ddi/content/wdm/ns-wdm-_file_full_ea_information">FILE_FULL_EA_INFORMATION</a> structure.
        /// Provides extended attribute (EA) information. This structure is used primarily by network drivers.
        /// </summary>
        [StructLayoutAttribute(LayoutKind.Sequential)]
        internal unsafe struct FILE_FULL_EA_INFORMATION
        {
			/// <summary>
            /// The offset of the next FILE_FULL_EA_INFORMATION-type entry. This member is zero if no other entries follow this one.
            /// </summary>
            internal UInt32 nextEntryOffset;
            
			/// <summary>
            /// Can be zero or can be set with FILE_NEED_EA, indicating that the file to which the EA belongs cannot be interpreted without understanding the associated extended attributes.
            /// </summary>
			internal Byte flags;
            
			/// <summary>
            /// The length in bytes of the EaName array. This value does not include a null-terminator to EaName.
            /// </summary>
			internal Byte EaNameLength;
            
			/// <summary>
            /// The length in bytes of each EA value in the array.
            /// </summary>
			internal UInt16 EaValueLength;
            
			/// <summary>
            /// An array of characters naming the EA for this entry.
            /// </summary>
			internal Byte EaName;
        }
    }
}
