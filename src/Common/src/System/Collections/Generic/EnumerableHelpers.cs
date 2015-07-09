// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Collections.Generic
{
    /// <summary>Internal helper functions for working with enumerables.</summary>
    internal static class EnumerableHelpers
    {
        /// <summary>Converts an enumerable to an array using the same logic as does List{T}.</summary>
        internal static T[] ToArray<T>(IEnumerable<T> source)
        {
            int count;
            T[] results = ToArray(source, out count);
            Array.Resize(ref results, count);
            return results;
        }

        /// <summary>Converts an enumerable to an array using the same logic as does List{T}.</summary>
        /// <param name="length">The number of items stored in the resulting array, 0-indexed.</param>
        /// <returns>
        /// The resulting array.  The length of the array may be greater than <paramref name="length"/>,
        /// which is the actual number of elements in the array.
        /// </returns>
        internal static T[] ToArray<T>(IEnumerable<T> source, out int length)
        {
            T[] arr;
            int count = 0;

            ICollection<T> ic = source as ICollection<T>;
            if (ic != null)
            {
                count = ic.Count;
                if (count == 0)
                {
                    arr = Array.Empty<T>();
                }
                else
                {
                    // Allocate an array of the desired size, then copy the elements into it. Note that this has the same 
                    // issue regarding concurrency as other existing collections like List<T>. If the collection size 
                    // concurrently changes between the array allocation and the CopyTo, we could end up either getting an 
                    // exception from overrunning the array (if the size went up) or we could end up not filling as many 
                    // items as 'count' suggests (if the size went down).  This is only an issue for concurrent collections 
                    // that implement ICollection<T>, which as of .NET 4.6 is just ConcurrentDictionary<TKey, TValue>.
                    arr = new T[count];
                    ic.CopyTo(arr, 0);
                }
            }
            else
            {
                arr = Array.Empty<T>();
                foreach (var item in source)
                {
                    if (count == arr.Length)
                    {
                        // MaxArrayLength is defined in Array.MaxArrayLength and in gchelpers in CoreCLR.
                        // It represents the maximum number of elements that can be in an array where
                        // the size of the element is greater than one byte; a separate, slightly larger constant,
                        // is used when the size of the element is one.
                        const int MaxArrayLength = 0x7FEFFFFF;

                        // This is the same growth logic as in List<T>:
                        // If the array is currently empty, we make it a default size.  Otherwise, we attempt to 
                        // double the size of the array.  Doubling will overflow once the size of the array reaches
                        // 2^30, since doubling to 2^31 is 1 larger than Int32.MaxValue.  In that case, we instead 
                        // constrain the length to be MaxArrayLength (this overflow check works because of of the 
                        // cast to uint).  Because a slightly larger constant is used when T is one byte in size, we 
                        // could then end up in a situation where arr.Length is MaxArrayLength or slightly larger, such 
                        // that we constrain newLength to be MaxArrayLength but the needed number of elements is actually 
                        // larger than that.  For that case, we then ensure that the newLength is large enough to hold 
                        // the desired capacity.  This does mean that in the very rare case where we've grown to such a 
                        // large size, each new element added after MaxArrayLength will end up doing a resize.
                        const int DefaultCapacity = 4;
                        int newLength = count == 0 ? DefaultCapacity : count * 2;
                        if ((uint)newLength > MaxArrayLength)
                        {
                            newLength = MaxArrayLength;
                        }
                        if (newLength < count + 1)
                        {
                            newLength = count + 1;
                        }

                        Array.Resize(ref arr, newLength);
                    }
                    arr[count++] = item;
                }
            }

            length = count;
            return arr;
        }
    }
}
