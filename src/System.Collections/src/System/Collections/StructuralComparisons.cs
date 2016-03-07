// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace System.Collections
{
    public static class StructuralComparisons
    {
        private static volatile IComparer s_StructuralComparer;
        private static volatile IEqualityComparer s_StructuralEqualityComparer;

        public static IComparer StructuralComparer
        {
            get
            {
                IComparer comparer = s_StructuralComparer;
                if (comparer == null)
                {
                    comparer = new StructuralComparer();
                    s_StructuralComparer = comparer;
                }
                return comparer;
            }
        }

        public static IEqualityComparer StructuralEqualityComparer
        {
            get
            {
                IEqualityComparer comparer = s_StructuralEqualityComparer;
                if (comparer == null)
                {
                    comparer = new StructuralEqualityComparer();
                    s_StructuralEqualityComparer = comparer;
                }
                return comparer;
            }
        }
    }

    internal sealed class StructuralEqualityComparer : IEqualityComparer
    {
        public new bool Equals(Object x, Object y)
        {
            if (x != null)
            {
                IStructuralEquatable seObj = x as IStructuralEquatable;

                if (seObj != null) return seObj.Equals(y, this);
                if (y == null) return false;
                return x.Equals(y);
            }
            return (y == null);
        }

        public int GetHashCode(Object obj)
        {
            if (obj == null) return 0;

            IStructuralEquatable seObj = obj as IStructuralEquatable;

            if (seObj == null) return obj.GetHashCode();
            return seObj.GetHashCode(this);
        }
    }

    internal sealed class StructuralComparer : IComparer
    {
        public int Compare(Object x, Object y)
        {
            if (x == null) return y == null ? 0 : -1;
            if (y == null) return 1;

            IStructuralComparable scX = x as IStructuralComparable;

            if (scX == null) return Comparer<Object>.Default.Compare(x, y);
            return scX.CompareTo(y, this);
        }
    }
}
