// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;

#if !netstandard
using Internal.Runtime.CompilerServices;
#endif

namespace System
{
    internal static partial class SpanSortHelpersCommon
    {
        // This is the threshold where Introspective sort switches to Insertion sort.
        // Empirically, 16 seems to speed up most cases without slowing down others, at least for integers.
        // Large value types may benefit from a smaller number.
        internal const int IntrosortSizeThreshold = 16;

        internal static int FloorLog2PlusOne(int n)
        {
            Debug.Assert(n >= 2);
            int result = 2;
            n >>= 2;
            while (n > 0)
            {
                ++result;
                n >>= 1;
            }
            return result;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Swap<T>(ref T items, int i, int j)
        {
            Debug.Assert(i != j);
            Swap(ref Unsafe.Add(ref items, i), ref Unsafe.Add(ref items, j));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }

        // This started out with just LessThan.
        // However, due to bogus comparers, comparables etc.
        // we need to preserve semantics completely to get same result.
        internal interface IDirectComparer<in T>
        {
            bool GreaterThan(T x, T y);
            bool LessThan(T x, T y);
        }

        //
        // Type specific DirectComparer(s) to ensure optimal code-gen
        //
        internal struct SByteDirectComparer : IDirectComparer<sbyte>
        {
            public bool GreaterThan(sbyte x, sbyte y) => x > y;
            public bool LessThan(sbyte x, sbyte y) => x < y;
        }
        internal struct ByteDirectComparer : IDirectComparer<byte>
        {
            public bool GreaterThan(byte x, byte y) => x > y;
            public bool LessThan(byte x, byte y) => x < y;
        }
        internal struct Int16DirectComparer : IDirectComparer<short>
        {
            public bool GreaterThan(short x, short y) => x > y;
            public bool LessThan(short x, short y) => x < y;
        }
        internal struct UInt16DirectComparer : IDirectComparer<ushort>
        {
            public bool GreaterThan(ushort x, ushort y) => x > y;
            public bool LessThan(ushort x, ushort y) => x < y;
        }
        internal struct Int32DirectComparer : IDirectComparer<int>
        {
            public bool GreaterThan(int x, int y) => x > y;
            public bool LessThan(int x, int y) => x < y;
        }
        internal struct UInt32DirectComparer : IDirectComparer<uint>
        {
            public bool GreaterThan(uint x, uint y) => x > y;
            public bool LessThan(uint x, uint y) => x < y;
        }
        internal struct Int64DirectComparer : IDirectComparer<long>
        {
            public bool GreaterThan(long x, long y) => x > y;
            public bool LessThan(long x, long y) => x < y;
        }
        internal struct UInt64DirectComparer : IDirectComparer<ulong>
        {
            public bool GreaterThan(ulong x, ulong y) => x > y;
            public bool LessThan(ulong x, ulong y) => x < y;
        }
        internal struct SingleDirectComparer : IDirectComparer<float>
        {
            public bool GreaterThan(float x, float y) => x > y;
            public bool LessThan(float x, float y) => x < y;
        }
        internal struct DoubleDirectComparer : IDirectComparer<double>
        {
            public bool GreaterThan(double x, double y) => x > y;
            public bool LessThan(double x, double y) => x < y;
        }

        internal interface IIsNaN<T>
        {
            bool IsNaN(T value);
        }
        internal struct SingleIsNaN : IIsNaN<float>
        {
            public bool IsNaN(float value) => float.IsNaN(value);
        }
        internal struct DoubleIsNaN : IIsNaN<double>
        {
            public bool IsNaN(double value) => double.IsNaN(value);
        }
    }
}
