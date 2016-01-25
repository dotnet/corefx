// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.Contracts;

namespace System.Collections.Generic
{
    public static class IListExtensions
    {
        // Searches a section of the list for a given element using a binary search
        // algorithm. Elements of the list are compared to the search value using
        // the given IComparer interface. If comparer is null, elements of
        // the list are compared to the search value using the IComparable
        // interface, which in that case must be implemented by all elements of the
        // list and the given search value. This method assumes that the given
        // section of the list is already sorted; if this is not the case, the
        // result will be incorrect.
        //
        // The method returns the index of the given value in the list. If the
        // list does not contain the given value, the method returns a negative
        // integer. The bitwise complement operator (~) can be applied to a
        // negative result to produce the index of the first element (if any) that
        // is larger than the given search value. This is also the index at which
        // the search value should be inserted into the list in order for the list
        // to remain sorted.
        public static int BinarySearch(this IList list, int index, int count, object item, IComparer comparer)
        {
            if (list == null)
                throw new NullReferenceException(SR.ArgumentNull_IList);
            if (comparer == null)
                throw new ArgumentNullException(SR.ArgumentNull_IComparer);
            if (index < 0)
                throw new ArgumentOutOfRangeException("index", SR.ArgumentOutOfRange_NeedNonNegNum);
            if (count < 0)
                throw new ArgumentOutOfRangeException("count", SR.ArgumentOutOfRange_NeedNonNegNum);
            if (list.Count - index < count)
                throw new ArgumentException(SR.Argument_InvalidOffLen);
            Contract.Ensures(Contract.Result<int>() <= index + count);
            Contract.EndContractBlock();

            int low = index;
            int hi = index + count - 1;

            while (low <= hi)
            {
                int i = GetMedian(low, hi);
                int c;
                try
                {
                    c = comparer.Compare(list[i], item);
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException(SR.InvalidOperation_IComparerFailed, e);
                }
                if (c == 0) return i;
                if (c < 0)
                {
                    low = i + 1;
                }
                else {
                    hi = i - 1;
                }
            }

            return ~low;
        }

        public static int BinarySearch(this IList list, object item, IComparer comparer)
        {
            Contract.Ensures(Contract.Result<int>() <= list.Count);
            return list.BinarySearch(0, list.Count, item, comparer);
        }

        private static int GetMedian(int low, int hi)
        {
            Contract.Requires(low <= hi);
            Contract.Assert(hi - low >= 0, "Length overflow!");
            return low + ((hi - low) >> 1);
        }
    }
}
