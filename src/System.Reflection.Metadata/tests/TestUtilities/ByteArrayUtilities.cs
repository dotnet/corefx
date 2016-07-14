﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Reflection.Metadata.Tests
{
    public static class ByteArrayUtilities
    {
        public static byte[] Slice(this BlobBuilder bytes, int start, int end)
        {
            return Slice(bytes.ToArray(), start, end);
        }

        public static byte[] Slice(this byte[] bytes, int start, int end)
        {
            if (end < 0)
            {
                end = bytes.Length + end;
            }

            var result = new byte[end - start];
            Array.Copy(bytes, start, result, 0, result.Length);
            return result;
        }
    }
}
