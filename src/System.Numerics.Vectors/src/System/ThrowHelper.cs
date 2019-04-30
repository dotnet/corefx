// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace System
{
    // partially copied from shared\System\ThrowHelper.cs
    internal static partial class ThrowHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ThrowForUnsupportedVectorBaseType<T>() where T : struct
        {
            if (typeof(T) != typeof(byte) && typeof(T) != typeof(sbyte) &&
                typeof(T) != typeof(short) && typeof(T) != typeof(ushort) &&
                typeof(T) != typeof(int) && typeof(T) != typeof(uint) &&
                typeof(T) != typeof(long) && typeof(T) != typeof(ulong) &&
                typeof(T) != typeof(float) && typeof(T) != typeof(double))
            {
                ThrowArgTypeNotSupportedException();
            }
        }

        private static void ThrowArgTypeNotSupportedException()
        {
            throw new NotSupportedException(SR.Arg_TypeNotSupported);
        }
    }
}
