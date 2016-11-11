// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace System 
{
    /// <summary>
    /// Returns a pinnable reference to a standardized anchor location within various flavors of managed objects.
    /// 
    /// Regrettably, this family of helpers is inherently unportable. It also requires special handling for arrays and strings
    /// as these objects have their own header layouts. 
    /// </summary>
    internal static partial class PinnableReferenceHelpers
    {
        // Returns a reference to the first field laid out in memory. 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref byte GetPinnableReferenceForPlainObject(this object obj)
        {
            Debug.Assert(obj != null);
            Debug.Assert(!(obj is Array));
            Debug.Assert(!(obj is string));
            return ref Unsafe.As<PlainObjectLayout>(obj).Data;
        }

        // Returns a reference to the 0th element (or where the 0th element would be in the case of an empty array.)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref byte GetPinnableReferenceForArray<T>(this T[] array)
        {
            Debug.Assert(array != null);
            return ref Unsafe.As<ArrayLayout>(array).Data;
        }

        // Returns a reference to the first character (or where the first character would be in the case of an empty string.)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref byte GetPinnableReferenceForString(this string s)
        {
            Debug.Assert(s != null);
            return ref Unsafe.As<StringLayout>(s).Data;
        }
    }
}

