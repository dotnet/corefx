using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Collections.Generic
{

    /// <summary>
    /// Equality comparer for hashsets of hashsets
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class HashSetEqualityComparer<T> : IEqualityComparer<HashSet<T>>
    {

        private IEqualityComparer<T> m_comparer;

        public HashSetEqualityComparer()
        {
            m_comparer = EqualityComparer<T>.Default;
        }

        public HashSetEqualityComparer(IEqualityComparer<T> comparer)
        {
            if (comparer == null)
            {
                m_comparer = EqualityComparer<T>.Default;
            }
            else
            {
                m_comparer = comparer;
            }
        }

        // using m_comparer to keep equals properties in tact; don't want to choose one of the comparers
        public bool Equals(HashSet<T> x, HashSet<T> y)
        {
            return HashSet<T>.HashSetEquals(x, y, m_comparer);
        }

        public int GetHashCode(HashSet<T> obj)
        {
            int hashCode = 0;
            if (obj != null)
            {
                foreach (T t in obj)
                {
                    hashCode = hashCode ^ (m_comparer.GetHashCode(t) & 0x7FFFFFFF);
                }
            } // else returns hashcode of 0 for null hashsets
            return hashCode;
        }

        // Equals method for the comparer itself. 
        public override bool Equals(Object obj)
        {
            HashSetEqualityComparer<T> comparer = obj as HashSetEqualityComparer<T>;
            if (comparer == null)
            {
                return false;
            }
            return (this.m_comparer == comparer.m_comparer);
        }

        public override int GetHashCode()
        {
            return m_comparer.GetHashCode();
        }
    }
}

