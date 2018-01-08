// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Intrinsics.X86;

namespace System.Reflection.Internal
{
    internal static partial class BitArithmetic
    {
        internal static int CountBits(uint v)
        {
            if (Popcnt.IsSupported)
            {
                return Popcnt.PopCount(v);
            }
            else
            {
                return CountBitsPrivate(v);
            }
        }

        internal static int CountBits(ulong v)
        {
            if (Popcnt.IsSupported)
            {
                return (int)Popcnt.PopCount(v);
            }
            else
            {
                return CountBitsPrivate(v);
            }
        }
    }
}
