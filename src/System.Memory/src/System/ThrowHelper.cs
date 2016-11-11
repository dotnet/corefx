// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace System
{
    internal static class ThrowHelper
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowArgumentNullException(ExceptionArgument argument)
        {
            throw new ArgumentNullException(argument.ToString());
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowArrayTypeMismatchException_ArrayTypeMustBeExactMatch(Type type)
        {
            throw new ArrayTypeMismatchException(SR.Format(SR.ArrayTypeMustBeExactMatch, type));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowArgumentException_InvalidTypeWithPointersNotSupported(Type type)
        {
            throw new ArgumentException(SR.Format(SR.Argument_InvalidTypeWithPointersNotSupported, type));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowArgumentException_DestinationTooShort()
        {
            throw new ArgumentException(SR.Argument_DestinationTooShort);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowIndexOutOfRangeException()
        {
            throw new IndexOutOfRangeException();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowArgumentOutOfRangeException(ExceptionArgument argument)
        {
            throw new ArgumentOutOfRangeException(argument.ToString());
        }
    }

    internal enum ExceptionArgument
    {
        array,
        length,
        start,
        obj,
    }
}