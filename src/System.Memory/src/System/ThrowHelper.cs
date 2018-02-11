// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using System.Buffers;

namespace System
{
    //
    // This pattern of easily inlinable "void Throw" routines that stack on top of NoInlining factory methods
    // is a compromise between older JITs and newer JITs (RyuJIT in Core CLR 1.1.0+ and desktop CLR in 4.6.3+).
    // This package is explicitly targeted at older JITs as newer runtimes expect to implement Span intrinsically for
    // best performance.
    //
    // The aim of this pattern is three-fold
    // 1. Extracting the throw makes the method preforming the throw in a conditional branch smaller and more inlinable
    // 2. Extracting the throw from generic method to non-generic method reduces the repeated codegen size for value types
    // 3a. Newer JITs will not inline the methods that only throw and also recognise them, move the call to cold section
    //     and not add stack prep and unwind before calling https://github.com/dotnet/coreclr/pull/6103
    // 3b. Older JITs will inline the throw itself and move to cold section; but not inline the non-inlinable exception
    //     factory methods - still maintaining advantages 1 & 2
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

        internal static void ThrowArgumentOutOfRangeException_PrecisionTooLarge() { throw CreateArgumentOutOfRangeException_PrecisionTooLarge(); }
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Exception CreateArgumentOutOfRangeException_PrecisionTooLarge() { return new ArgumentOutOfRangeException("precision", SR.Format(SR.Argument_PrecisionTooLarge, StandardFormat.MaxPrecision)); }

        internal static void ThrowArgumentOutOfRangeException_SymbolDoesNotFit() { throw CreateArgumentOutOfRangeException_SymbolDoesNotFit(); }
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Exception CreateArgumentOutOfRangeException_SymbolDoesNotFit() { return new ArgumentOutOfRangeException("symbol", SR.Argument_BadFormatSpecifier); }

        internal static void ThrowInvalidOperationException_OutstandingReferences() { throw CreateInvalidOperationException_OutstandingReferences(); }
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Exception CreateInvalidOperationException_OutstandingReferences() { return new InvalidOperationException(SR.OutstandingReferences); }

        internal static void ThrowObjectDisposedException_MemoryDisposed(string objectName) { throw CreateObjectDisposedException_MemoryDisposed(objectName); }
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Exception CreateObjectDisposedException_MemoryDisposed(string objectName) { return new ObjectDisposedException(objectName, SR.MemoryDisposed); }

        internal static void ThrowFormatException_BadFormatSpecifier() { throw CreateFormatException_BadFormatSpecifier(); }
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Exception CreateFormatException_BadFormatSpecifier() { return new FormatException(SR.Argument_BadFormatSpecifier); }

        internal static void ThrowArgumentException_OverlapAlignmentMismatch() { throw CreateArgumentException_OverlapAlignmentMismatch(); }
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Exception CreateArgumentException_OverlapAlignmentMismatch() { return new ArgumentException(SR.Argument_OverlapAlignmentMismatch); }

        // TODO
        internal static void ThrowArgumentException_ItemsMustHaveSameLength() { throw CreateArgumentException_ItemsMustHaveSameLength(); }
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Exception CreateArgumentException_ItemsMustHaveSameLength() { return new ArgumentException("Items must have same length as keys"); }//SR.Argument_ItemsMustHaveSameLength); }

        // coreclr does not have an exception for bad IComparable but instead throws with comparer == null
        internal static void ThrowArgumentException_BadComparer(object comparer) { throw CreateArgumentException_BadComparer(comparer); }
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Exception CreateArgumentException_BadComparer(object comparer) { return new ArgumentException(
            string.Format("Unable to sort because the IComparer.Compare() method returns inconsistent results. Either a value does not compare equal to itself, or one value repeatedly compared to another value yields different results. IComparer: '{0}'.", comparer)); }//SR.Format(SR.Arg_BogusIComparer, comparer));; }
        // here we throw if bad comparable, including the case when user uses Comparer<TKey>.Default and TKey is IComparable<TKey>
        internal static void ThrowArgumentException_BadComparable(Type comparableType) { throw CreateArgumentException_BadComparable(comparableType); }
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Exception CreateArgumentException_BadComparable(Type comparableType) {
            return new ArgumentException(
            string.Format("Unable to sort because the IComparable.CompareTo() method returns inconsistent results. Either a value does not compare equal to itself, or one value repeatedly compared to another value yields different results. IComparable: '{0}'.", comparableType.FullName));
        }//SR.Format(SR.Arg_BogusIComparable, comparableType));; }

        //
        // Enable use of ThrowHelper from TryFormat() routines without introducing dozens of non-code-coveraged "bytesWritten = 0; return false" boilerplate.
        //
        public static bool TryFormatThrowFormatException(out int bytesWritten)
        {
            bytesWritten = 0;
            ThrowHelper.ThrowFormatException_BadFormatSpecifier();
            return false;
        }

        //
        // Enable use of ThrowHelper from TryParse() routines without introducing dozens of non-code-coveraged "value= default; bytesConsumed = 0; return false" boilerplate.
        //
        public static bool TryParseThrowFormatException<T>(out T value, out int bytesConsumed)
        {
            value = default;
            bytesConsumed = 0;
            ThrowHelper.ThrowFormatException_BadFormatSpecifier();
            return false;
        }
    }

    internal enum ExceptionArgument
    {
        array,
        length,
        start,
        text,
        obj,
        ownedMemory,
        pointer,
        comparable,
        comparer,
        comparison
    }
}
