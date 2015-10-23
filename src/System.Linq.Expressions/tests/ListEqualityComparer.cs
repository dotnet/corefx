// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;

namespace System.Collections.Generic
{
    class ListEqualityComparer<T> : IEqualityComparer<List<T>>
    {
        public static readonly IEqualityComparer<List<T>> Default = new ListEqualityComparer<T>();

        public bool Equals(List<T> x, List<T> y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return x.SequenceEqual(y);
        }

        public int GetHashCode(List<T> obj)
        {
            if (obj == null)
            {
                return 0;
            }

            var eq = EqualityComparer<T>.Default;

            var res = 17;

            unchecked
            {
                foreach (var element in obj)
                {
                    res += eq.GetHashCode(element);
                    res *= 17;
                }
            }
            
            return res;
        }
    }
}
