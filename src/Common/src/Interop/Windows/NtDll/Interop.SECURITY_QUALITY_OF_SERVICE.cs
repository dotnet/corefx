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
        /// <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/aa379574.aspx">SECURITY_QUALITY_OF_SERVICE</a> structure.
        /// Contains information used to support client impersonation. A client can specify this information when it connects to a server.
        /// </summary>
		[StructLayoutAttribute(LayoutKind.Sequential)]
        internal unsafe struct SECURITY_QUALITY_OF_SERVICE
        {
			/// <summary>
            /// Specifies the size, in bytes, of this structure.
            /// </summary>
            internal UInt32 length;
			
			/// <summary>
            /// Specifies the information given to the server about the client, and how the server may represent, or impersonate, the client.
            /// </summary>
            [MarshalAs(UnmanagedType.I4)]
            internal int impersonationLevel;
			
			/// <summary>
            /// Specifies whether the server is to be given a snapshot of the client's security context (called static tracking), or is to be
			/// continually updated to track changes to the client's security context (called dynamic tracking).
            /// </summary>
            internal byte contextDynamicTrackingMode;
			
			/// <summary>
            /// Specifies whether the server may enable or disable privileges and groups that the client's security context may include.
            /// </summary>
            internal byte effectiveOnly;
        }
	}
}
