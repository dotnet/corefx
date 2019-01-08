// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Data.SqlClient
{
    internal sealed partial class TdsParser
    {
		internal static void GetGuidBytes(Guid guid, Span<byte> buffer)
        {
            byte[] bytes = guid.ToByteArray();
            bytes.AsSpan().CopyTo(buffer);
        }

		internal static void GetDoubleBytes(double value, Span<byte> buffer)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			bytes.AsSpan().CopyTo(buffer);
		}

		internal static void GetFloatBytes(float value, Span<byte> buffer)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			bytes.AsSpan().CopyTo(buffer);
		}
	}
}
