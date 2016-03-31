// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Net
{
    internal static class IntPtrHelper
    {
        internal static IntPtr Add(IntPtr a, int b)
        {
            return (IntPtr)((long)a + (long)b);
        }

        internal static long Subtract(IntPtr a, IntPtr b)
        {
            return ((long)a - (long)b);
        }
    }
}
