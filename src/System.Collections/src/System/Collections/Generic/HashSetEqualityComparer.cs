// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Collections.Generic
{
    /// <summary>
    /// Equality comparer for hashsets of hashsets
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal sealed class HashSetEqualityComparer<T> : IEqualityComparer<HashSet<T>>
    {
        private readonly IEqualityComparer<T> _comparer;

        public HashSetEqualityComparer()
        {
            // null comparer signals legacy use.
        }

        public HashSetEqualityComparer(IEqualityComparer<T> comparer)
        {
            _comparer = comparer ?? EqualityComparer<T>.Default;
        }

        public bool Equals(HashSet<T> x, HashSet<T> y)
            => _comparer == null ? HashSet<T>.LegacyHashSetEquals(x, y) : HashSet<T>.HashSetEquals(x, y, _comparer);

        public int GetHashCode(HashSet<T> obj)
        {
            if (obj == null)
            {
                return 0;
            }

            if (_comparer == null)
            {
                IEqualityComparer<T> comparer = EqualityComparer<T>.Default;
                // Legacy behaviour for compat.
                int hashCode = 0;
                foreach (T t in obj)
                {
                    hashCode = hashCode ^ (comparer.GetHashCode(t) & 0x7FFFFFFF);
                }

                return hashCode;
            }

            unchecked
            {
                int hashCode = 0x5EED5EED; // Arbitrary seed with a mix of 1s and 0s throughout the value.
                IEqualityComparer<T> comparer = _comparer;

                // Already know the set is distinct as considered by comparer, so we don't need to track duplicates.
                if (comparer.Equals(obj.Comparer))
                {
                    foreach (T item in obj)
                    {
                        hashCode ^= comparer.GetHashCode(item);
                    }

                    hashCode += obj.Count;
                }
                else
                {
                    HashSet<T> seen = new HashSet<T>(comparer);
                    foreach (T item in obj)
                    {
                        if (seen.Add(item))
                        {
                            hashCode ^= comparer.GetHashCode(item);
                        }
                    }

                    hashCode += seen.Count;
                }

                return hashCode;
            }
        }

        // Equals method for the comparer itself. 
        public override bool Equals(object obj)
        {
            HashSetEqualityComparer<T> comparer = obj as HashSetEqualityComparer<T>;
            if (comparer == null)
            {
                return false;
            }

            if (_comparer == null)
            {
                return comparer._comparer == null;
            }

            return comparer._comparer != null && _comparer.Equals(comparer._comparer);
        }

        public override int GetHashCode()
        {
            if (_comparer == null)
            {
                return 1; // Non-zero to differ from the HashSetEqualityComparer itself being null with most uses.
            }

            return _comparer.GetHashCode();
        }
    }
}
