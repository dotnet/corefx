// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace System
{
    //
    // This pattern of inlinable "void Throw" routines that stack on top of NoInlining factory methods is a compromise between older JITs and newer JITs.
    // The package this file appears in is targeted specifically at older JITs. Older JITs cannot inline methods that have conditional throws in them,
    // thus we use these helpers to hide the throws. Newer JIT's will inline the Throw routines and will benefit from the code size reductions of
    // splitting the creation details into their own non-inlinable factory methods.
    //
    internal static class ThrowHelper
    {
        internal static void ThrowArgumentNullException(ExceptionArgument argument) { throw CreateArgumentNullException(argument); }
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Exception CreateArgumentNullException(ExceptionArgument argument) { return new ArgumentNullException(argument.ToString()); }

        internal static void ThrowArrayTypeMismatchException_ArrayTypeMustBeExactMatch(Type type) { throw CreateArrayTypeMismatchException_ArrayTypeMustBeExactMatch(type); }
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Exception CreateArrayTypeMismatchException_ArrayTypeMustBeExactMatch(Type type) { return new ArrayTypeMismatchException(SR.Format(SR.ArrayTypeMustBeExactMatch, type)); }

        internal static void ThrowArgumentException_InvalidTypeWithPointersNotSupported(Type type) { throw CreateArgumentException_InvalidTypeWithPointersNotSupported(type); }
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Exception CreateArgumentException_InvalidTypeWithPointersNotSupported(Type type) { return new ArgumentException(SR.Format(SR.Argument_InvalidTypeWithPointersNotSupported, type)); }

        internal static void ThrowArgumentException_DestinationTooShort() { throw CreateArgumentException_DestinationTooShort(); }
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Exception CreateArgumentException_DestinationTooShort() { return new ArgumentException(SR.Argument_DestinationTooShort); }

        internal static void ThrowIndexOutOfRangeException() { throw CreateIndexOutOfRangeException(); }
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Exception CreateIndexOutOfRangeException() { return new IndexOutOfRangeException(); }

        internal static void ThrowArgumentOutOfRangeException(ExceptionArgument argument) { throw CreateArgumentOutOfRangeException(argument); }
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Exception CreateArgumentOutOfRangeException(ExceptionArgument argument) { return new ArgumentOutOfRangeException(argument.ToString()); }
    }

    internal enum ExceptionArgument
    {
        array,
        length,
        start,
        obj,
    }
}