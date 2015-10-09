// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
	internal static partial class Sys
	{
		[StructLayoutAttribute(LayoutKind.Sequential)]
		public unsafe struct TcpGlobalStatistics
		{
		    public readonly ulong ConnectionsAccepted;
		    public readonly ulong ConnectionsInitiated;
		    public readonly ulong ErrorsReceived;
		    public readonly ulong FailedConnectionAttempts;
		    public readonly ulong SegmentsReceived;
		    public readonly ulong SegmentsResent;
		    public readonly ulong SegmentsSent;
		}

		[DllImport(Libraries.SystemNative)]
		public static unsafe extern int GetTcpGlobalStatistics(out TcpGlobalStatistics statistics);
	}
}
