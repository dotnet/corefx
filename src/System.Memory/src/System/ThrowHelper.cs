// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace System
{
    internal static class ThrowHelper
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static Exception CreateThrowArgumentNullException(ExceptionArgument argument)
        {
            return new ArgumentNullException(argument.ToString());
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static Exception CreateThrowArrayTypeMismatchException_ArrayTypeMustBeExactMatch(Type type)
        {
            return new ArrayTypeMismatchException(SR.Format(SR.ArrayTypeMustBeExactMatch, type));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static Exception CreateThrowArgumentException_InvalidTypeWithPointersNotSupported(Type type)
        {
            return new ArgumentException(SR.Format(SR.Argument_InvalidTypeWithPointersNotSupported, type));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static Exception CreateThrowArgumentException_DestinationTooShort()
        {
            return new ArgumentException(SR.Argument_DestinationTooShort);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static Exception CreateThrowIndexOutOfRangeException()
        {
            return new IndexOutOfRangeException();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static Exception CreateThrowArgumentOutOfRangeException(ExceptionArgument argument)
        {
            return new ArgumentOutOfRangeException(argument.ToString());
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